using System.ComponentModel.DataAnnotations;

namespace TraineeManagementApi.Models;

public class Review
{
    [Key]
    public string Id { get; set; }

    [Required(ErrorMessage = "SubmissionId is required.")]
    public string SubmissionId { get; set; }
    public Submission Submission { get; set; }

    [Required(ErrorMessage = "MentorId is required.")]
    public string MentorId { get; set; }
    public Mentor Mentor { get; set; }

    [Required(ErrorMessage = "Feedback is required.")]
    public string Feedback { get; set; }

    [Range(0, 100, ErrorMessage = "Score must be between 0 and 100.")]
    public int? Score { get; set; } // Nullable for optional scores

    [Required(ErrorMessage = "ReviewStatus is required.")]
    public string ReviewStatus { get; set; } // Accepted / ChangesRequired / Rejected

    [Required(ErrorMessage = "ReviewedDate is required.")]
    [DataType(DataType.Date, ErrorMessage = "Invalid date format.")]
    public DateOnly ReviewedDate { get; set; }
}
