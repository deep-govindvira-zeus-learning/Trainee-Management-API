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
    public async Task<IActionResult> GetMentorById(string id)
    {
        var response = await _service.GetByIdAsync(id);
        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> CreateMentor([FromBody] CreateMentorRequest request)
    {
        var response = await _service.CreateAsync(request);
        return CreatedAtAction(nameof(GetMentorById), new { id = response.Id }, response);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMentor(string id)
    {
        await _service.DeleteByIdAsync(id);
        return NoContent();
    }  

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateMentor(string id, [FromBody] UpdateMentorRequest request)
    {
        var response = await _service.UpdateByIdAsync(id, request);
        return Ok(response);
    }
}
