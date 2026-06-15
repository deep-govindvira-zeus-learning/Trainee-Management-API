using Microsoft.EntityFrameworkCore;
using TraineeManagementApi.Data;
using TraineeManagementApi.DTOs;
using TraineeManagementApi.Models;
using TraineeManagementApi.Services;

public class SubmissionService : ISubmissionService
{
    private readonly AppDbContext _context;
    private readonly ILogger<SubmissionService> _logger;

    public SubmissionService(AppDbContext context, ILogger<SubmissionService> logger)
    {
        _context = context;
        _logger = logger;
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
}