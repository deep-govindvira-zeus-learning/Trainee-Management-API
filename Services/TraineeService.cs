using Microsoft.EntityFrameworkCore;
using TraineeManagementApi.Data;
using TraineeManagementApi.DTOs;
using TraineeManagementApi.Models;
using Microsoft.Extensions.Logging;

namespace TraineeManagementApi.Services;

public class TraineeService : ITraineeService
{
    private readonly AppDbContext _context;
    private readonly ICacheService _cacheService;
    private readonly ILogger<TraineeService> _logger;

    public TraineeService(AppDbContext context, ILogger<TraineeService> logger, ICacheService cacheService)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        // FIX: Fixed the incorrect nameof validation variable matching
        _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
    }

    public async Task<PagedResponse<TraineeResponse>> GetAllAsync(
        string? search,
        string? status,
        int pageNumber,
        int pageSize)
    {
        _logger.LogInformation("Fetching trainees with search: {Search}, status: {Status}, pageNumber: {PageNumber}, pageSize: {PageSize}", search, status, pageNumber, pageSize);

        int validPageNumber = pageNumber < 1 ? 1 : pageNumber;
        int validPageSize = pageSize < 1 ? 10 : pageSize;
        string normSearch = (search ?? string.Empty).Trim().ToLowerInvariant();
        string normStatus = (status ?? string.Empty).Trim().ToLowerInvariant();

        // Fetch the current global list version from Redis (Defaults to "1" if missing)
        string currentVersion = await _cacheService.GetAsync<string>("trainees:list:version") ?? "1";

        // Include the version string directly in your cache key structure
        string cacheKey = $"trainees:list:v={currentVersion}:s={normSearch}:st={normStatus}:p={validPageNumber}:sz={validPageSize}";

        var cachedData = await _cacheService.GetAsync<PagedResponse<TraineeResponse>>(cacheKey);
        if (cachedData != null)
        {
            _logger.LogInformation("Cache HIT for key: {CacheKey}", cacheKey);
            return cachedData;
        }

        _logger.LogInformation("Cache MISS for key: {CacheKey}. Fetching from MySQL database.", cacheKey);

        try
        {
            var query = _context.Trainees.AsQueryable();

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(trainee => trainee.Status == status);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(trainee =>
                    trainee.FirstName.Contains(search) ||
                    trainee.LastName.Contains(search) ||
                    trainee.Email.Contains(search) ||
                    trainee.TechStack.Contains(search)
                );
            }

            int totalRecords = await query.CountAsync();

            var trainees = await query
                .OrderBy(t => t.Id)
                .Skip((validPageNumber - 1) * validPageSize)
                .Take(validPageSize)
                .ToListAsync();

            var traineeResponses = TraineeConverter.ToTraineeResponseList(trainees);

            var response = new PagedResponse<TraineeResponse>
            {
                PageNumber = validPageNumber,
                PageSize = validPageSize,
                TotalRecords = totalRecords,
                Data = traineeResponses
            };

            await _cacheService.SetAsync(cacheKey, response, TimeSpan.FromMinutes(5));
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while fetching the paginated list of trainees from MySQL.");
            throw;
        }
    }

    public async Task<TraineeResponse> GetByIdAsync(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            _logger.LogWarning("GetByIdAsync called with an empty or null ID.");
            throw new ArgumentException("Trainee ID cannot be null or empty.", nameof(id));
        }

        // ADDED: Task 3.6 Cache-aside for individual entity reads
        string cacheKey = $"trainee:{id}";
        var cachedTrainee = await _cacheService.GetAsync<TraineeResponse>(cacheKey);
        if (cachedTrainee != null)
        {
            _logger.LogInformation("Cache HIT for key: {CacheKey}", cacheKey);
            return cachedTrainee;
        }

        _logger.LogInformation("Cache MISS for key: {CacheKey}. Fetching from MySQL database.", cacheKey);

        var trainee = await _context.Trainees.FindAsync(id);
        if (trainee == null)
        {
            _logger.LogWarning("Trainee with ID: {TraineeId} was not found.", id);
            throw new KeyNotFoundException($"Trainee with ID '{id}' was not found.");
        }

        var response = TraineeConverter.ToTraineeResponse(trainee);

        // Populate cache after miss (TTL: 10 minutes)
        await _cacheService.SetAsync(cacheKey, response, TimeSpan.FromMinutes(10));
        return response;
    }

    public async Task<TraineeResponse> CreateAsync(CreateTraineeRequest request)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));

        _logger.LogInformation("Creating a new trainee with Email: {Email}", request.Email);

        try
        {
            Trainee trainee = TraineeConverter.ToTrainee(request);
            await _context.Trainees.AddAsync(trainee);
            await _context.SaveChangesAsync();

            // ADDED: Task 3.7 Invalidation. New entry shifts list page collections.
            await InvalidateListCacheAsync();

            _logger.LogInformation("Trainee '{FirstName}' with ID '{TraineeId}' successfully created.", trainee.FirstName, trainee.Id);
            return TraineeConverter.ToTraineeResponse(trainee);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create trainee for Email: {Email}", request.Email);
            throw;
        }
    }

    public async Task<TraineeResponse> UpdateByIdAsync(string id, UpdateTraineeRequest request)
    {
        if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException("Trainee ID cannot be null or empty.", nameof(id));
        if (request == null) throw new ArgumentNullException(nameof(request));

        _logger.LogInformation("Updating trainee with ID: {TraineeId}", id);

        try
        {
            var existing = await _context.Trainees.FindAsync(id);
            if (existing == null)
            {
                _logger.LogWarning("Failed to update trainee. Trainee with ID: {TraineeId} was not found.", id);
                throw new KeyNotFoundException($"Trainee with ID '{id}' was not found.");
            }

            existing.FirstName = request.FirstName;
            existing.LastName = request.LastName;
            existing.Email = request.Email;
            existing.TechStack = request.TechStack;
            existing.Status = request.Status;
            existing.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // ADDED: Task 3.7 Invalidation. Evict individual and clear collections.
            await _cacheService.RemoveAsync($"trainee:{id}");
            await InvalidateListCacheAsync();

            _logger.LogInformation("Successfully updated trainee with ID: {TraineeId} and invalidated cache entries.", id);
            return TraineeConverter.ToTraineeResponse(existing);
        }
        catch (Exception ex) when (ex is not KeyNotFoundException)
        {
            _logger.LogError(ex, "An error occurred while updating trainee with ID: {TraineeId}", id);
            throw;
        }
    }

    public async Task<bool> DeleteByIdAsync(string id)
    {
        if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException("Trainee ID cannot be null or empty.", nameof(id));

        _logger.LogInformation("Deleting trainee with ID: {TraineeId}", id);

        try
        {
            var trainee = await _context.Trainees.FindAsync(id);
            if (trainee == null)
            {
                _logger.LogWarning("Failed to delete trainee. Trainee with ID: {TraineeId} was not found.", id);
                throw new KeyNotFoundException($"Trainee with ID '{id}' was not found.");
            }

            _context.Trainees.Remove(trainee);
            await _context.SaveChangesAsync();

            // ADDED: Task 3.7 Invalidation. Evict individual and clear collections.
            await _cacheService.RemoveAsync($"trainee:{id}");
            await InvalidateListCacheAsync();

            _logger.LogInformation("Successfully deleted trainee with ID: {TraineeId} and cleared cache.", id);
            return true;
        }
        catch (Exception ex) when (ex is not KeyNotFoundException)
        {
            _logger.LogError(ex, "An error occurred while deleting trainee with ID: {TraineeId}", id);
            throw;
        }
    }

    private async Task InvalidateListCacheAsync()
    {
        // Get the current version string
        string currentVersionStr = await _cacheService.GetAsync<string>("trainees:list:version") ?? "1";
        int.TryParse(currentVersionStr, out int currentVersion);

        // Increment it. The next "GetAllAsync" will instantly look for a brand new key string!
        string nextVersion = (currentVersion + 1).ToString();
        await _cacheService.SetAsync("trainees:list:version", nextVersion, TimeSpan.FromDays(1));
    }

}
