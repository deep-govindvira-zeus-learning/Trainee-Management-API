namespace TraineeManagementApi.Controllers;

using Microsoft.AspNetCore.Mvc;
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

        var trainees = await _service.GetAllAsync("", "", 0, 0);
        return Ok(new
        {
            status = "healthy",
            application = "Trainee Management API",
            timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss"),
            trainees
        });
    }
}
