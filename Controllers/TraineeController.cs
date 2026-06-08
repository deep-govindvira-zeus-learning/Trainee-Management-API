namespace TraineeManagementApi.Controllers;

using Microsoft.AspNetCore.Mvc;
using TraineeManagementApi.DTOs;
using TraineeManagementApi.Services;

[ApiController]
[Route("api/trainees")]
public class TraineesController : ControllerBase
{
    private readonly ITraineeService _service;

    public TraineesController(ITraineeService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllTrainee([FromQuery] string? search)
    {
        var trainees = await _service.GetAllTraineeAsync(search);
        return Ok(trainees);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTraineeById(string id)
    {
        var response = await _service.GetTraineeByIdAsync(id);
        if (response == null) return NotFound($"Trainee with ID {id} was not found.");
        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> CreateTrainee([FromBody] CreateTraineeRequest request)
    {
        var response = await _service.CreateTraineeAsync(request);
        if (response == null) return BadRequest($"Trainee is not created.");
        return CreatedAtAction(nameof(GetTraineeById), new { id = response.Id }, response);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTrainee(string id, [FromBody] UpdateTraineeRequest request)
    {
        if (request == null) return BadRequest();
        var response = await _service.UpdateTraineeAsync(id, request);
        if (response == null) return NotFound($"Trainee with ID {id} was not found.");
        return Ok(response);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTrainee(string id)
    {
        var deleted = await _service.DeleteTraineeAsync(id);
        if (!deleted) return NotFound();
        return NoContent();
    }
}
