using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TraineeManagementApi.Services;

namespace TraineeManagementApi.Controllers;

[ApiController]
[Route("/api/learning-tasks")]
[Authorize]
public class LearningTaskController : ControllerBase
{
    private readonly ILearningTaskService _service;

    public LearningTaskController(ILearningTaskService service)
    {
        _service = service;
    }


    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
    {
        var responses = await _service.GetAllAsync();
        return Ok(responses); // Returns 200 OK with data
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetByIdAsync(string id)
    {
        var response = await _service.GetByIdAsync(id);
        return Ok(response);
    }


    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] CreateLearningTaskRequest request)
    {
        // var response = await _service.CreateMentorAsync(request);
        // return CreatedAtAction(nameof(GetMentorById), new { id = response.Id }, response);
        return CreatedAtAction(nameof(GetByIdAsync), new { id = ""});
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateByIdAsync(string id, [FromBody] UpdateLearningTaskRequest request)
    {
        // var response = await _service.UpdateMentorAsync(id, request);
        // return Ok(response);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteByIdAsync(string id)
    {
        // await _service.DeleteLearningTaskAsync(id);
        return NoContent();
    }  
}