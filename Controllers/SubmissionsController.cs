
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TraineeManagementApi.DTOs;
using TraineeManagementApi.Models;
using TraineeManagementApi.Services;

namespace TraineeManagementApi.Controllers;

[ApiController]
[Route("api/submissions")]
[Authorize]
public class SubmissionsController : ControllerBase
{
    private readonly ISubmissionService _service;

    public SubmissionsController(ISubmissionService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
    {
        var results = await _service.GetAllAsync();
        return Ok(results);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetByIdAsync(string id)
    {
        var result = await _service.GetByIdAsync(id);
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<SubmissionResponse>> CreateAsync([FromBody] CreateSubmissionRequest request)
    {
        var result = await _service.CreateAsync(request);
        return CreatedAtAction(nameof(GetByIdAsync), new { id = result.Id }, result);
    }

    [HttpPost("{submissionId}/files")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadFiles(
        [FromRoute] string submissionId,
        [FromForm] List<IFormFile> files)
    {
        string userIdentity = User.Identity?.Name ?? "AnonymousTestUser";
        var responses = await _service.UploadFilesAsync(submissionId, files, userIdentity);
        return Ok(responses);
    }
}
