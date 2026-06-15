using Microsoft.EntityFrameworkCore;
using TraineeManagementApi.Data;
using TraineeManagementApi.DTOs;
using TraineeManagementApi.Models;

namespace TraineeManagementApi.Services;

public class ReviewService : IReviewService
{
    private readonly AppDbContext _context;
    private readonly ILogger<ReviewService> _logger;

    public ReviewService(AppDbContext context, ILogger<ReviewService> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<List<ReviewResponse>> GetAllAsync()
    {
        _logger.LogInformation("Fetching all reviews from the database.");

        try
        {
            var reviews = await _context.Reviews
                .AsNoTracking()
                .ToListAsync();

            return ReviewConverter.ToReviewResponseList(reviews);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "A database exception occurred while fetching all reviews.");
            throw;
        }
    }

    public async Task<ReviewResponse> GetByIdAsync(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            _logger.LogWarning("GetByIdAsync called with an empty or null ID.");
            throw new ArgumentException("Review ID cannot be null or empty.", nameof(id));
        }

        _logger.LogInformation("Fetching review with ID: {ReviewId}", id);

        try
        {
            var review = await _context.Reviews
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == id);

            if (review == null)
            {
                _logger.LogWarning("Review with ID {ReviewId} was not found.", id);
                throw new KeyNotFoundException($"Review with ID '{id}' was not found.");
            }

            return ReviewConverter.ToReviewResponse(review);
        }
        catch (Exception ex) when (ex is not KeyNotFoundException && ex is not ArgumentException)
        {
            _logger.LogError(ex, "A database exception occurred while fetching review ID: {ReviewId}", id);
            throw;
        }
    }

    public async Task<ReviewResponse> CreateAsync(CreateReviewRequest request)
    {
        if (request == null)
        {
            _logger.LogError("CreateReviewRequest is null.");
            throw new ArgumentNullException(nameof(request));
        }

        bool submissionExists = await _context.Submissions.AnyAsync(s => s.Id == request.SubmissionId);
        if (submissionExists == false)
        {
            _logger.LogWarning("Database check failed: Submission with ID {Id} does not exist.", request.SubmissionId);
            throw new KeyNotFoundException($"Submission with ID '{request.SubmissionId}' was not found.");
        }

        bool mentorExists = await _context.Mentors.AnyAsync(m => m.Id == request.MentorId);
        if (mentorExists == false)
        {
            _logger.LogWarning("Database check failed: Mentor with ID {Id} does not exist.", request.MentorId);
            throw new KeyNotFoundException($"Mentor with ID '{request.MentorId}' was not found.");
        }

        var review = ReviewConverter.ToReview(request);

        try
        {
            await _context.Reviews.AddAsync(review);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Successfully recorded review with ID: {ReviewId}", review.Id);

            return ReviewConverter.ToReviewResponse(review);
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "A database error occurred while saving the review.");
            throw;
        }
    }
}