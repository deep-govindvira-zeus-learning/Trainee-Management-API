using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TraineeManagementApi.DTOs;
using TraineeManagementApi.Services;

namespace TraineeManagementApi.Controllers;

[ApiController]
[Authorize]
[Route("api/processing-jobs")]
public class ProcessingJobsController : ControllerBase
{
    private readonly IProcessingJobService _jobService;

    public ProcessingJobsController(IProcessingJobService jobService)
    {
        _jobService = jobService;
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProcessingJobResponse>> GetByIdAsync(Guid id)
    {
        var response = await _jobService.GetByIdAsync(id);
        return Ok(response);
    }

    [HttpPost("{id:guid}/retry")]
    public async Task<ActionResult<ProcessingJobResponse>> RetryAsync(Guid id)
    {
        var response = await _jobService.RetryById(id);
        return Ok(response);
    }
}
