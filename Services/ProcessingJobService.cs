using Microsoft.EntityFrameworkCore;
using TraineeManagementApi.Data;
using TraineeManagementApi.DTOs;
using Microsoft.Extensions.Logging;

namespace TraineeManagementApi.Services;

public class ProcessingJobService : IProcessingJobService
{
    private readonly AppDbContext _context;
    private readonly ILogger<ProcessingJobService> _logger;
    private readonly ISubmissionPublisher _submissionPublisher;


    public ProcessingJobService(AppDbContext appDbContext, ILogger<ProcessingJobService> logger, ISubmissionPublisher submissionPublisher)
    {
        _context = appDbContext;
        _logger = logger;
        _submissionPublisher = submissionPublisher;
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

      public async Task<ProcessingJobResponse?> RetryById(Guid id)
    {
        _logger.LogInformation("Fetching processing job with ID: {JobId}", id);

        var job = await _context.ProcessingJobs.AsNoTracking().FirstOrDefaultAsync(j => j.Id == id);
                
        await _submissionPublisher.Publish(SubmissionProcessingRequestedConverter.ToSubmissionProcessingRequested(job));

        return ProcessingJobResponseConverter.ToProcessingJobResponse(job);
    }
}
