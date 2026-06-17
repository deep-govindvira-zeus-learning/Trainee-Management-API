using System.ComponentModel.DataAnnotations;

namespace TraineeManagementApi.Models;

public class CreateReviewRequest
{
    [Required(ErrorMessage = "SubmissionId is required.")]
    public string SubmissionId { get; set; } = string.Empty;

    [Required(ErrorMessage = "MentorId is required.")]
    public string MentorId { get; set; } = string.Empty;

    [Required(ErrorMessage = "Feedback is required.")]
    public string Feedback { get; set; } = string.Empty;

    [Range(0, 100, ErrorMessage = "Score must be between 0 and 100.")]
    public int? Score { get; set; } // Nullable for optional scores

    [Required(ErrorMessage = "ReviewStatus is required.")]
    [AllowedValues("Accepted", "ChangesRequired", "Rejected", ErrorMessage = "Status must be Accepted, ChangesRequired or Rejected.")]
    public string ReviewStatus { get; set; } = string.Empty;
}
