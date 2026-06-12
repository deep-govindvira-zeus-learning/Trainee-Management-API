namespace TraineeManagementApi.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TraineeManagementApi.DTOs;
using TraineeManagementApi.Services;

[ApiController]
[Route("api/mentors")]
[Authorize]
public class MentorController : ControllerBase
{
    private readonly IMentorService _service;

    public MentorController(IMentorService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
    {
        var responses = await _service.GetAllAsync();
        return Ok(responses);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetByIdAsync(string id)
    {
        var response = await _service.GetByIdAsync(id);
        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] CreateMentorRequest request)
    {
        var response = await _service.CreateAsync(request);
        return CreatedAtAction(nameof(GetByIdAsync), new { id = response.Id }, response);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateByIdAsync(string id, [FromBody] UpdateMentorRequest request)
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
