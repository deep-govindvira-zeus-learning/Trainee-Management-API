using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TraineeManagementApi.DTOs;
using TraineeManagementApi.Services;

namespace TraineeManagementApi.Controllers;

[ApiController]
[Route("/api/task-assignments")]
[Authorize]
public class AssignmentController : ControllerBase
{
    private readonly IAssignmentService _service;

    public AssignmentController(IAssignmentService service)
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
    public async Task<IActionResult> CreateAsync([FromBody] CreateAssignmentRequest request)
    {
        var response = await _service.CreateAsync(request);
        return CreatedAtAction(nameof(GetByIdAsync), new { id = response.Id }, response);
    }

    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateStatus(string id, [FromBody] UpdateAssignmentStatusRequest request)
    {
        var response = await _service.UpdateStatusAsync(id, request);
        return Ok(response);
    }
}
