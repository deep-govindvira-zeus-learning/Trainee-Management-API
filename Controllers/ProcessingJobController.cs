using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TraineeManagementApi.Data;
using TraineeManagementApi.DTOs;

namespace TraineeManagementApi.Controllers;

[ApiController]
[Route("api/processing-jobs")]
public class ProcessingJobsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ProcessingJobsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<JobStatusResponse>> GetJobStatus(Guid id)
    {
        var job = await _context.ProcessingJobs.AsNoTracking().FirstOrDefaultAsync(j => j.Id == id);

        if (job == null)
        {
            return NotFound(new { Message = $"Job with ID '{id}' was not found." });
        }

        return Ok(new JobStatusResponse(
            job.Id,
            job.SubmissionId,
            job.FileId,
            job.MessageId,
            job.CorrelationId,
            job.Status.ToString(),
            job.Attempts,
            job.ErrorSummary,
            job.GeneratedChecksum,
            job.RequestedAt,
            job.StartedAt,
            job.CompletedAt
        ));
    }
}
