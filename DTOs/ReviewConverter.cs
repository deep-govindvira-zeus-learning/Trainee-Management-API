using TraineeManagementApi.Models;

namespace TraineeManagementApi.DTOs;

public static class ReviewConverter
{
    public static ReviewResponse ToReviewResponse(Review review)
    {
        return new ReviewResponse
        {
            Id = review.Id,
            SubmissionId = review.SubmissionId,
            MentorId = review.MentorId,
            Feedback = review.Feedback,
            Score = review.Score,
            ReviewStatus = review.ReviewStatus,
            ReviewedDate = review.ReviewedDate
        };
    }

    public static Review ToReview(CreateReviewRequest request)
    {
        return new Review
        {
            Id = Guid.NewGuid().ToString(),
            SubmissionId = request.SubmissionId,
            MentorId = request.MentorId,
            Feedback = request.Feedback,
            Score = request.Score,
            ReviewStatus = request.ReviewStatus,
            ReviewedDate = DateOnly.FromDateTime(DateTime.UtcNow)
        };
    }

    public static List<ReviewResponse> ToReviewResponseList(List<Review> reviews)
    {
        return reviews.Select(ToReviewResponse).ToList();
    }
}