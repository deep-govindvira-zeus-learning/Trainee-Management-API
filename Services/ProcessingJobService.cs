using Microsoft.EntityFrameworkCore;
using TraineeManagementApi.Data;
using TraineeManagementApi.DTOs;
using Microsoft.Extensions.Logging;

namespace TraineeManagementApi.Services;

public class ProcessingJobService : IProcessingJobService
{
    private readonly AppDbContext _context;
    private readonly ILogger<ProcessingJobService> _logger;

    public ProcessingJobService(AppDbContext appDbContext, ILogger<ProcessingJobService> logger)
    {
        _context = appDbContext;
        _logger = logger;
    }

    public async Task<ProcessingJobResponse?> GetByIdAsync(Guid id)
    {
        _logger.LogInformation("Fetching processing job with ID: {JobId}", id);

        var job = await _context.ProcessingJobs.AsNoTracking().FirstOrDefaultAsync(j => j.Id == id);
        
        if (job == null)
        {
            _logger.LogWarning("Processing job with ID: {JobId} was not found in the database", id);
            throw new KeyNotFoundException($"Job with ID '{id}' was not found.");
        }

        _logger.LogInformation("Successfully retrieved processing job with ID: {JobId}", id);
        return ProcessingJobResponseConverter.ToProcessingJobResponse(job);
    }
}
