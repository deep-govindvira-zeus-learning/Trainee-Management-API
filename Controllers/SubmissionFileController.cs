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
        var (stream, contentType, fileName) = await _service.DownloadFileAsync(id, CurrentUsername, IsPrivileged);
        return File(stream, contentType, fileName);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteFile(string id)
    {
        await _service.DeleteFileAsync(id, CurrentUsername, IsPrivileged);
        return NoContent();
    }

    // The uploader's identity is recorded on SubmissionFile.UploadedBy as User.Identity.Name
    // (see SubmissionsController.UploadFiles / SubmissionFileConverter), so we compare against
    // the same claim here to determine ownership.
    private string CurrentUsername => User.Identity?.Name ?? string.Empty;

    // Admins and Mentors need broad access to review/manage any trainee's submissions;
    // Trainees are restricted to files they uploaded themselves.
    private bool IsPrivileged => User.IsInRole("Admin") || User.IsInRole("Mentor");
}
