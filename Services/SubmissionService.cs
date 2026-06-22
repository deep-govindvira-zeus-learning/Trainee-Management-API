using System.Security.Cryptography;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using TraineeManagementApi.Data;
using TraineeManagementApi.DTOs;
using TraineeManagementApi.Models;
using TraineeManagementApi.Services;

public class SubmissionService : ISubmissionService
{
    private readonly AppDbContext _context;
    private readonly ILogger<SubmissionService> _logger;

    private readonly IFileStorageService _fileStorageService;
    private readonly long _maxFileSize;
    private readonly string[] _allowedExtensions;

    public SubmissionService(AppDbContext context, ILogger<SubmissionService> logger, IFileStorageService fileStorageService, IConfiguration configuration)
    {
        _context = context;
        _logger = logger;
        _fileStorageService = fileStorageService;
        _maxFileSize = configuration.GetValue<long>("FileStorage:MaxSizeBytes", 10485760); // Default 10MB
        _allowedExtensions = configuration.GetSection("FileStorage:AllowedExtensions").Get<string[]>()
                             ?? new[] { ".pdf", ".docx", ".zip" };

    }

    public async Task<List<SubmissionResponse>> GetAllAsync()
    {
        _logger.LogInformation("Retrieving all submissions.");

        try
        {
            var submissions = await _context.Submissions
                .AsNoTracking()
                .ToListAsync();

            return SubmissionConverter.ToSubmissionResponseList(submissions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "A database exception occurred while fetching all submissions.");
            throw;
        }
    }

    public async Task<SubmissionResponse> GetByIdAsync(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            _logger.LogWarning("GetByIdAsync called with an empty or null ID.");
            throw new ArgumentException("Submission ID cannot be null or empty.", nameof(id));
        }

        _logger.LogInformation("Fetching submission with ID: {AssignmentId}", id);

        try
        {
            var submission = await _context.Submissions
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == id);

            if (submission == null)
            {
                _logger.LogWarning("Submission with ID {SubmissionId} was not found.", id);
                throw new KeyNotFoundException($"Submission with ID '{id}' was not found.");
            }

            return SubmissionConverter.ToSubmissionResponse(submission);
        }
        catch (Exception ex) when (ex is not KeyNotFoundException && ex is not ArgumentException)
        {
            _logger.LogError(ex, "A database exception occurred while fetching submission ID: {SubmissionId}", id);
            throw;
        }
    }

    public async Task<SubmissionResponse> CreateAsync(CreateSubmissionRequest request)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        var assignmentExists = await _context.Assignments.AnyAsync(a => a.Id == request.AssignmentId);
        if (!assignmentExists)
        {
            _logger.LogWarning("Submission failed. Assignment ID {Id} does not exist.", request.AssignmentId);
            throw new KeyNotFoundException($"Assignment with ID '{request.AssignmentId}' was not found.");
        }

        if (request.Status != "Submitted" && request.Status != "Resubmitted")
        {
            throw new ArgumentException("Status must be either 'Submitted' or 'Resubmitted'.", nameof(request.Status));
        }

        try
        {
            var submission = SubmissionConverter.ToSubmission(request);

            await _context.Submissions.AddAsync(submission);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Successfully recorded submission with ID: {SubmissionId}", submission.Id);

            return SubmissionConverter.ToSubmissionResponse(submission);
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Database update exception occurred saving submission.");
            throw;
        }
    }

    private async Task validateFilesAsync(string submissionId, List<IFormFile> files)
    {
        // Validation 1: Check for empty or missing payload
        if (files == null || files.Count == 0)
        {
            throw new Exception("No files were uploaded.");
        }

        // Authorization Check: Verify relational record mapping context existence once
        var submissionExists = await _context.Submissions.AnyAsync(s => s.Id == submissionId);
        if (!submissionExists)
        {
            throw new Exception("User is not authorized or resource does not exist.");
        }

        var uploadedFilesResult = new List<SubmissionFileResponse>();

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
                throw new Exception($"File '{file.FileName}' size exceeds the allowed limit.");
            }

            // Validation 4: Extract extension cleanly and match with allowed whitelist patterns
            var extension = Path.GetExtension(file.FileName).ToLower();
            if (!_allowedExtensions.Contains(extension))
            {
                throw new Exception($"Unsupported or untrusted file extension for '{file.FileName}'.");
            }
        }
    }

    public async Task<List<SubmissionFileResponse>> UploadFilesAsync(string submissionId, List<IFormFile> files, string userIdentity)
    {
        await validateFilesAsync(submissionId, files);

        var submissionFileResponseList = new List<SubmissionFileResponse>();

        foreach (var file in files)
        {
            var submissionFile = await SubmissionFileConverter.ToSubmissionFileAsync(submissionId, file, _fileStorageService, userIdentity);

            _context.SubmissionFiles.Add(submissionFile);

            submissionFileResponseList.Add(SubmissionFileConverter.ToSubmissionFileResponse(submissionFile));
        }

        // Save all database records in a single transaction batch
        await _context.SaveChangesAsync();

        _logger.LogInformation("{Count} files successfully mapped for submission {SubmissionId}",
            files.Count, submissionId);

        return submissionFileResponseList;
    }

}