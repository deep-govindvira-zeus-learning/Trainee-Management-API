namespace TraineeManagementApi.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using TraineeManagementApi.Services;

[ApiController]
[Route("api/health")]
public class HealthController : ControllerBase
{
    private readonly ITraineeService _service;
    private readonly HealthCheckService _healthCheckService;

    public HealthController(ITraineeService service, HealthCheckService healthCheckService)
    {
        _service = service;
        _healthCheckService = healthCheckService;
    }

    [HttpGet]
    public async Task<IActionResult> GetHealth()
    {

        var trainees = await _service.GetAllAsync("", "", 0, 0);
        return Ok(new
        {
            status = "healthy",
            application = "Trainee Management API",
            timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss"),
            trainees
        });
    }

    [HttpGet("live")]
    public IActionResult GetLive()
    {
        return Ok(new { status = "Healthy" });
    }

    [HttpGet("ready")]
    public async Task<IActionResult> GetReady()
    {
        // Executes all registered dependency checks in parallel
        var report = await _healthCheckService.CheckHealthAsync();

        var sanitizedResponse = new
        {
            status = report.Status.ToString(),
            totalDurationMs = report.TotalDuration.TotalMilliseconds,
            dependencies = report.Entries.Select(entry => new
            {
                name = entry.Key,
                status = entry.Value.Status.ToString(),
                durationMs = entry.Value.Duration.TotalMilliseconds
            })
        };

        if (report.Status == HealthStatus.Unhealthy)
        {
            // Returns a 503 Service Unavailable if any dependency breaks
            return StatusCode(StatusCodes.Status503ServiceUnavailable, sanitizedResponse);
        }

        return Ok(sanitizedResponse);
    }

}
