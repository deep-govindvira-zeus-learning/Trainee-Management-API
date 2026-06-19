using System.Security.Cryptography;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TraineeManagementApi.Data;
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
    private readonly IFileStorageService _storageService;
    private readonly AppDbContext _context;
    private readonly ILogger<SubmissionsController> _logger;
    private readonly long _maxFileSize;
    private readonly string[] _allowedExtensions;


    public SubmissionsController(ISubmissionService service, IFileStorageService storageService,
            AppDbContext context,
            IConfiguration configuration,
            ILogger<SubmissionsController> logger)
    {
        _service = service;
        _storageService = storageService;
        _context = context;
        _logger = logger;
        _maxFileSize = configuration.GetValue<long>("FileStorage:MaxSizeBytes", 10485760); // Default 10MB
        _allowedExtensions = configuration.GetSection("FileStorage:AllowedExtensions").Get<string[]>()
                             ?? new[] { ".pdf", ".docx", ".zip" };

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

        // Validation 1: Check for empty or missing payload
        if (files == null || files.Count == 0)
        {
            return BadRequest(new { Message = "No files were uploaded." });
        }

        // Authorization Check: Verify relational record mapping context existence once
        var submissionExists = await _context.Submissions.AnyAsync(s => s.Id == submissionId);
        if (!submissionExists)
        {
            return Forbid("User is not authorized or resource does not exist.");
        }

        var uploadedFilesResult = new List<object>();

        foreach (var file in files)
        {
            // Validation 2: Skip or reject empty individual files
            if (file == null || file.Length == 0)
            {
                continue; // Or return BadRequest if strict validation is needed
            }

            // Validation 3: Check maximum configured file payload boundary limits
            if (file.Length > _maxFileSize)
            {
                return StatusCode(StatusCodes.Status413PayloadTooLarge, new { Message = $"File '{file.FileName}' size exceeds the allowed limit." });
            }

            // Validation 4: Extract extension cleanly and match with allowed whitelist patterns
            var extension = Path.GetExtension(file.FileName).ToLower();
            if (!_allowedExtensions.Contains(extension))
            {
                return BadRequest(new { Message = $"Unsupported or untrusted file extension for '{file.FileName}'." });
            }
        }

        foreach (var file in files)
        {
            // Validation 2: Skip or reject empty individual files
            if (file == null || file.Length == 0)
            {
                continue; // Or return BadRequest if strict validation is needed
            }

            // Validation 3: Check maximum configured file payload boundary limits
            if (file.Length > _maxFileSize)
            {
                return StatusCode(StatusCodes.Status413PayloadTooLarge, new { Message = $"File '{file.FileName}' size exceeds the allowed limit." });
            }

            // Validation 4: Extract extension cleanly and match with allowed whitelist patterns
            var extension = Path.GetExtension(file.FileName).ToLower();
            if (!_allowedExtensions.Contains(extension))
            {
                return BadRequest(new { Message = $"Unsupported or untrusted file extension for '{file.FileName}'." });
            }

            string checksum;
            string storageName;

            using (var stream = file.OpenReadStream())
            {
                using var sha256 = SHA256.Create();
                byte[] hashBytes = await sha256.ComputeHashAsync(stream);
                checksum = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

                stream.Position = 0; // Reset pointer position
                storageName = await _storageService.SaveAsync(stream, extension);
            }

            // Establish decoupled structural metadata reference mapping entity
            var submissionFile = new SubmissionFile
            {
                Id = Guid.NewGuid().ToString(),
                SubmissionId = submissionId,
                OriginalFileName = Path.GetFileName(file.FileName), // Prevent path traversal exploits
                StorageName = storageName,
                ContentType = file.ContentType,
                SizeInBytes = file.Length,
                Checksum = checksum,
                UploadedBy = userIdentity
            };

            _context.SubmissionFiles.Add(submissionFile);

            uploadedFilesResult.Add(new
            {
                submissionFile.Id,
                submissionFile.SubmissionId,
                submissionFile.OriginalFileName,
                submissionFile.ContentType,
                submissionFile.SizeInBytes,
                submissionFile.Checksum,
                submissionFile.UploadedBy,
                submissionFile.CreatedDate
            });
        }


        // Save all database records in a single transaction batch
        await _context.SaveChangesAsync();

        _logger.LogInformation("{Count} files successfully mapped for submission {SubmissionId}",
            files.Count, submissionId);

        return Ok(uploadedFilesResult);
    }

    [HttpGet("api/submission-files/{id}/download")]
    public async Task<IActionResult> DownloadFile(string id)
    {
        var metadata = await _context.SubmissionFiles.FindAsync(id);
        if (metadata == null) return NotFound();

        if (User.Identity?.Name != null && metadata.UploadedBy != User.Identity.Name)
        {
            return Forbid();
        }

        if (!await _storageService.ExistsAsync(metadata.StorageName))
        {
            _logger.LogWarning("Metadata exists for file {Id} but physical asset is missing from storage.", id);
            return NotFound(new { Message = "The file resource could not be found on disk." });
        }

        var stream = await _storageService.OpenReadAsync(metadata.StorageName);
        return File(stream, metadata.ContentType, metadata.OriginalFileName);
    }

    [HttpDelete("api/submission-files/{id}")]
    public async Task<IActionResult> DeleteFile(string id)
    {
        var metadata = await _context.SubmissionFiles.FindAsync(id);
        if (metadata == null) return NotFound();

        if (User.Identity?.Name != null && metadata.UploadedBy != User.Identity.Name)
        {
            return Forbid();
        }

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            _context.SubmissionFiles.Remove(metadata);
            await _context.SaveChangesAsync();

            await _storageService.DeleteAsync(metadata.StorageName);
            await transaction.CommitAsync();

            _logger.LogInformation("Successfully purged metadata record and storage node for item: {Id}", id);
            return NoContent();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Transaction execution failure while rolling back file tracking record for metadata reference {Id}", id);
            return StatusCode(500, new { Message = "An error occurred during transaction execution." });
        }
    }
}
