namespace TraineeManagementApi.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TraineeManagementApi.DTOs;
using TraineeManagementApi.Services;

[ApiController]
[Route("api/trainees")]
[Authorize]
public class TraineesController : ControllerBase
{
    private readonly ITraineeService _service;

    public TraineesController(ITraineeService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllTrainee(
    [FromQuery] string? search,
    [FromQuery] string? status,
    [FromQuery] int pageNumber = 1,
    [FromQuery] int pageSize = 10)
    {
        var trainees = await _service.GetAllAsync(search, status, pageNumber, pageSize);
        return Ok(trainees);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetByIdAsync(string id)
    {
        var response = await _service.GetByIdAsync(id);
        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] CreateTraineeRequest request)
    {
        var response = await _service.CreateAsync(request);
        return CreatedAtAction(nameof(GetByIdAsync), new { id = response.Id }, response);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateByIdAsync(string id, [FromBody] UpdateTraineeRequest request)
    {
        var response = await _service.UpdateByIdAsync(id, request);
        return Ok(response);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteByIdAsync(string id)
    {
        await _service.DeleteByIdAsync(id);
        return NoContent();
    }
}
