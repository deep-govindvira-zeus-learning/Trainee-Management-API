using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using Org.BouncyCastle.Ocsp;
using TraineeManagementApi.Data;
using TraineeManagementApi.DTOs;
using TraineeManagementApi.Models;

namespace TraineeManagementApi.Services;

public class TraineeService : ITraineeService
{
    private readonly AppDbContext _context;
    private readonly ILogger<TraineeService> _logger;

    public TraineeService(AppDbContext context, ILogger<TraineeService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<PagedResponse<TraineeResponse>> GetAllTraineeAsync(
        string? search,
        string? status,
        int pageNumber,
        int pageSize)
    {
        var query = _context.Trainees.AsQueryable();

        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(trainee => trainee.Status == status);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.ToLower();
            query = query.Where(trainee =>
                trainee.FirstName.ToLower().Contains(search) ||
                trainee.LastName.ToLower().Contains(search) ||
                trainee.Email.ToLower().Contains(search) ||
                trainee.TechStack.ToLower().Contains(search)
            );
        }

        int totalRecords = await query.CountAsync();

        int validPageNumber = pageNumber < 1 ? 1 : pageNumber;
        int validPageSize = pageSize < 1 ? 10 : pageSize;

        var trainees = await query
            .OrderBy(t => t.Id)
            .Skip((validPageNumber - 1) * validPageSize)
            .Take(validPageSize)
            .ToListAsync();

        var traineeResponses = TraineeConverter.ToTraineeResponseList(trainees);

        return new PagedResponse<TraineeResponse>
        {
            PageNumber = validPageNumber,
            PageSize = validPageSize,
            TotalRecords = totalRecords,
            Data = traineeResponses
        };
    }

    public async Task<TraineeResponse> GetTraineeByIdAsync(string id)
    {
        var trainee = await _context.Trainees.FindAsync(id);
        if (trainee == null) return null;
        return TraineeConverter.ToTraineeResponse(trainee);
    }

    public async Task<TraineeResponse> CreateTraineeAsync(CreateTraineeRequest request)
    {
        try
        {
            Trainee trainee = TraineeConverter.ToTrainee(request);
            await _context.Trainees.AddAsync(trainee);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Trainee '{Firstname}' with ID '{Id}' created.", trainee.FirstName, trainee.Id);

            return TraineeConverter.ToTraineeResponse(trainee);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create trainee for Email: {Email}", request.Email);
            return null;
        }
    }


    public async Task<TraineeResponse> UpdateTraineeAsync(string id, UpdateTraineeRequest request)
    {
        try
        {
            var existing = await _context.Trainees.FindAsync(id);

            if (existing == null)
            {
                _logger.LogWarning("Failed to update trainee. Trainee with ID: {TraineeId} was not found.", id);
                return null;
            }

            existing.FirstName = request.FirstName;
            existing.LastName = request.LastName;
            existing.Email = request.Email;
            existing.TechStack = request.TechStack;
            existing.Status = request.Status;
            existing.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Updated trainee with ID: {TraineeId}", id);

            return TraineeConverter.ToTraineeResponse(existing);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while updating trainee with ID: {TraineeId}", id);
            return null;
        }
    }

    public async Task<bool> DeleteTraineeAsync(string id)
    {
        try
        {
            var trainee = await _context.Trainees.FindAsync(id);

            if (trainee == null)
            {
                _logger.LogWarning("Failed to delete trainee. Trainee with ID: {TraineeId} was not found.", id);
                return false;
            }

            _context.Trainees.Remove(trainee);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Deleted trainee with ID: {TraineeId}", id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while deleting trainee with ID: {TraineeId}", id);
            throw;
        }
    }
}

