using System.ComponentModel.DataAnnotations;

namespace TraineeManagementApi.Models;

public class ReviewResponse
{
    public string Id { get; set; } = string.Empty;
    public string SubmissionId { get; set; } = string.Empty;
    public string MentorId { get; set; } = string.Empty;
    public string Feedback { get; set; } = string.Empty;
    public int? Score { get; set; } // Nullable for optional scores
    public string ReviewStatus { get; set; } = string.Empty; // Accepted / ChangesRequired / Rejected
    public DateOnly ReviewedDate { get; set; }
}
