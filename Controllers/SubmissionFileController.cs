using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TraineeManagementApi.Services;

namespace TraineeManagementApi.Controllers;

[ApiController]
[Route("api/submission-files")]
[Authorize]
public class SubmissionFileController : ControllerBase
{
    private readonly ISubmissionFileService _service;

    public SubmissionFileController(ISubmissionFileService submissionFileService)
    {
        _service = submissionFileService;
    }

    [HttpGet("{id}/download")]
    public async Task<IActionResult> DownloadFile(string id)
    {
        var (stream, contentType, fileName) = await _service.DownloadFileAsync(id);
        return File(stream, contentType, fileName);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteFile(string id)
    {
        await _service.DeleteFileAsync(id);
        return NoContent();
    }
}
