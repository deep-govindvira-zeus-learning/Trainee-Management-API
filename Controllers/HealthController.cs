namespace TraineeManagementApi.Controllers;

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
        return Ok(new
        {
            status = "running",
            application = "Trainee Management API",
            timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss"),
            trainees = await _service.GetAllTraineeAsync("")
        });
    }
}
