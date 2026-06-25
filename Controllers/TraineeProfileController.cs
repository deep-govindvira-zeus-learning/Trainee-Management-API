using Microsoft.AspNetCore.Mvc;

namespace TraineeManagementApi.Controllers;

[ApiController]
[Route("api/trainee-profiles")]
public class TraineeProfileController : ControllerBase
{
    private readonly ITrainingDirectoryClient _directoryClient;
    private readonly ILogger<TraineeProfileController> _logger;

    public TraineeProfileController(ITrainingDirectoryClient directoryClient, ILogger<TraineeProfileController> logger)
    {
        _directoryClient = directoryClient;
        _logger = logger;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTraineeProfile(string id, CancellationToken cancellationToken)
    {
        // 1. Capture incoming correlation ID from Postman
        var correlationId = HttpContext.Request.Headers["X-Correlation-ID"].ToString();

        if (string.IsNullOrEmpty(correlationId))
        {
            correlationId = Guid.NewGuid().ToString();

            // CRITICAL FIX: Push the newly generated ID back into the Request Headers 
            // so that CorrelationIdManualPropagationHandler can pick up this EXACT same ID!
            HttpContext.Request.Headers["X-Correlation-ID"] = correlationId;
        }

        _logger.LogInformation("[Gateway] Received request for Trainee {Id} with Correlation ID: {CorrelationId}", id, correlationId);

        // Call the internal service via the resilient typed HTTP client
        var profile = await _directoryClient.GetProfileAsync(id, cancellationToken);

        if (profile == null)
        {
            _logger.LogWarning("[Gateway] Trainee {Id} not found in directory service.", id);
            return NotFound(new { Message = $"Trainee with ID {id} not found." });
        }

        _logger.LogInformation("[Gateway] Successfully processed request for Trainee {Id}.", id);

        return Ok(profile);
    }
}
