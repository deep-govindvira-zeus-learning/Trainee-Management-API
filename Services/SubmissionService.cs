using System.Security.Cryptography;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using TraineeManagementApi.Data;
using TraineeManagementApi.DTOs;
using TraineeManagementApi.Models;
using TraineeManagementApi.Services;

public class SubmissionService : ISubmissionService
{
    private readonly AppDbContext _context;
    private readonly ILogger<SubmissionService> _logger;
    private readonly IFileStorageService _fileStorageService;
    private readonly ICacheService _cacheService; // Added for distributed caching

    private readonly long _maxFileSize;
    private readonly string[] _allowedExtensions;
    
    private static readonly TimeSpan CacheTtl = TimeSpan.FromMinutes(10);
    private const string CacheKeyAll = "submission-summary:all";

    public SubmissionService(
        AppDbContext context, 
        ILogger<SubmissionService> logger, 
        IFileStorageService fileStorageService, 
        IConfiguration configuration,
        ICacheService cacheService) // Injected cache service
    {
        _context = context;
        _logger = logger;
        _fileStorageService = fileStorageService;
        _cacheService = cacheService;
        _maxFileSize = configuration.GetValue<long>("FileStorage:MaxSizeBytes", 10485760); 
        _allowedExtensions = configuration.GetSection("FileStorage:AllowedExtensions").Get<string[]>()
                             ?? new[] { ".pdf", ".docx", ".zip" };
    }


    public async Task<List<SubmissionResponse>> GetAllAsync()
    {
        _logger.LogInformation("Retrieving all submissions.");

        // 1. Try Cache Get (Safe from failure due to RedisCacheService try/catch wrapper)
        var cachedList = await _cacheService.GetAsync<List<SubmissionResponse>>(CacheKeyAll);
        if (cachedList != null)
        {
            _logger.LogInformation("Cache HIT for key: {Key}", CacheKeyAll);
            return cachedList;
        }

        _logger.LogInformation("Cache MISS for key: {Key}. Fetching from MySQL.", CacheKeyAll);

        try
        {
            var submissions = await _context.Submissions
                .Include(s => s.Files)
                .AsNoTracking()
                .ToListAsync();

            var responseList = SubmissionConverter.ToSubmissionResponseList(submissions);

            // 2. Populate Cache on Miss (Only metadata response summaries, no file blobs)
            await _cacheService.SetAsync(CacheKeyAll, responseList, CacheTtl);

            return responseList;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "A database exception occurred while fetching all submissions.");
            throw;
        }
    }


    public async Task<SubmissionResponse> GetByIdAsync(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            _logger.LogWarning("GetByIdAsync called with an empty or null ID.");
            throw new ArgumentException("Submission ID cannot be null or empty.", nameof(id));
        }

        // Predictable Key Convention: task-3.6 constraint
        string cacheKey = $"submission-summary:{id}";

        // 1. Try Cache Get
        var cachedSubmission = await _cacheService.GetAsync<SubmissionResponse>(cacheKey);
        if (cachedSubmission != null)
        {
            _logger.LogInformation("Cache HIT for key: {Key}", cacheKey);
            return cachedSubmission;
        }

        _logger.LogInformation("Cache MISS for key: {Key}. Fetching from MySQL.", cacheKey);

        try
        {
            var submission = await _context.Submissions
                .Include(s => s.Files) 
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == id);

            if (submission == null)
            {
                _logger.LogWarning("Submission with ID {SubmissionId} was not found.", id);
                throw new KeyNotFoundException($"Submission with ID '{id}' was not found.");
            }

            var response = SubmissionConverter.ToSubmissionResponse(submission);

            // 2. Populate Cache on Miss
            await _cacheService.SetAsync(cacheKey, response, CacheTtl);

            return response;
        }
        catch (Exception ex) when (ex is not KeyNotFoundException && ex is not ArgumentException)
        {
            _logger.LogError(ex, "A database exception occurred while fetching submission ID: {SubmissionId}", id);
            throw;
        }
    }

    public async Task<SubmissionResponse> CreateAsync(CreateSubmissionRequest request)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        var assignmentExists = await _context.Assignments.AnyAsync(a => a.Id == request.AssignmentId);
        if (!assignmentExists)
        {
            _logger.LogWarning("Submission failed. Assignment ID {Id} does not exist.", request.AssignmentId);
            throw new KeyNotFoundException($"Assignment with ID '{request.AssignmentId}' was not found.");
        }

        if (request.Status != "Submitted" && request.Status != "Resubmitted")
        {
            throw new ArgumentException("Status must be either 'Submitted' or 'Resubmitted'.", nameof(request.Status));
        }

        try
        {
            var submission = SubmissionConverter.ToSubmission(request);

            await _context.Submissions.AddAsync(submission);
            await _context.SaveChangesAsync(); // MySQL updated safely first

            _logger.LogInformation("Successfully recorded submission with ID: {SubmissionId}", submission.Id);

            // Invalidation Strategy: Clear the parent tracking list so it re-fetches the new entity
            await _cacheService.RemoveAsync(CacheKeyAll);

            return SubmissionConverter.ToSubmissionResponse(submission);
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Database update exception occurred saving submission.");
            throw;
        }
    }

    public async Task<List<SubmissionFileResponse>> UploadFilesAsync(string submissionId, List<IFormFile> files, string userIdentity)
    {
        await validateFilesAsync(submissionId, files);

        var submissionFileResponseList = new List<SubmissionFileResponse>();

        foreach (var file in files)
        {
            var submissionFile = await SubmissionFileConverter.ToSubmissionFileAsync(submissionId, file, _fileStorageService, userIdentity);
            _context.SubmissionFiles.Add(submissionFile);

            var submissionFileResponse = SubmissionFileConverter.ToSubmissionFileResponse(submissionFile);
            submissionFileResponseList.Add(submissionFileResponse);
        }

        await _context.SaveChangesAsync(); // State committed to source of truth

        _logger.LogInformation("{Count} files successfully mapped for submission {SubmissionId}", files.Count, submissionId);

        // Invalidation Strategy: Evict the individual key and the generic query tracker 
        // to force subsequent reads to map the newly added files.
        string individualKey = $"submission-summary:{submissionId}";
        await _cacheService.RemoveAsync(individualKey);
        await _cacheService.RemoveAsync(CacheKeyAll);

        return submissionFileResponseList;
    }

    private async Task validateFilesAsync(string submissionId, List<IFormFile> files)
    {
        if (files == null || files.Count == 0)
        {
            throw new Exception("No files were uploaded.");
        }

        var submissionExists = await _context.Submissions.AnyAsync(s => s.Id == submissionId);
        if (!submissionExists)
        {
            throw new Exception("User is not authorized or resource does not exist.");
        }

        foreach (var file in files)
        {
            if (file == null || file.Length == 0)
            {
                continue;
            }

            if (file.Length > _maxFileSize)
            {
                throw new Exception($"File '{file.FileName}' size exceeds the allowed limit.");
            }

            var extension = Path.GetExtension(file.FileName).ToLower();
            if (!_allowedExtensions.Contains(extension))
            {
                throw new Exception($"Unsupported or untrusted file extension for '{file.FileName}'.");
            }
        }
    }
}
