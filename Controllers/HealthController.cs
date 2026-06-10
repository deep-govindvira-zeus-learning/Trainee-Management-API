namespace TraineeManagementApi.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TraineeManagementApi.DTOs;
using TraineeManagementApi.Services;

[ApiController]
[Route("api/health")]
public class HealthController : ControllerBase
{
    private readonly ITraineeService _service;

    public HealthController(ITraineeService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetHealth()
    {
        try
        {
            var trainees = await _service.GetAllTraineeAsync("");
            return Ok(new
            {
                status = "healthy",
                application = "Trainee Management API",
                timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss"),
                trainees
            });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                status = "unhealthy",
                application = "Trainee Management API",
                timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss"),
                error = "Failed to connect database",
                exceptionMessage = ex.Message
            });
        }
    }
}
