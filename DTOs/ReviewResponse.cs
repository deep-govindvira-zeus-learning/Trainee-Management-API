using System.ComponentModel.DataAnnotations;

namespace TraineeManagementApi.Models;

public class ReviewResponse
{
    public string Id { get; set; }
    public string SubmissionId { get; set; }
    public string MentorId { get; set; }
    public string Feedback { get; set; }
    public int? Score { get; set; } // Nullable for optional scores
    public string ReviewStatus { get; set; } // Accepted / ChangesRequired / Rejected
    public DateOnly ReviewedDate { get; set; }
}
