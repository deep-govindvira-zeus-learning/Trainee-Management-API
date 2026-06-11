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
    public async Task<IActionResult> GetAllMentor()
    {
        var responses = await _service.GetAllMentorAsync();
        return Ok(responses);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetMentorById(string id)
    {
        var response = await _service.GetMentorByIdAsync(id);
        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> CreateMentor([FromBody] CreateMentorRequest request)
    {
        var response = await _service.CreateMentorAsync(request);
        return CreatedAtAction(nameof(GetMentorById), new { id = response.Id }, response);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMentor(string id)
    {
        await _service.DeleteMentorAsync(id);
        return NoContent();
    }    
}
