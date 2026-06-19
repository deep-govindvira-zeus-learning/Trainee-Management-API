using System.ComponentModel.DataAnnotations;

namespace TraineeManagementApi.Models;

public class Submission
{
    [Key]
    public string Id { get; set; }

    // Navigation property for the uploaded files tracking system
    public ICollection<SubmissionFile> Files { get; set; } = new List<SubmissionFile>();

    [Required(ErrorMessage = "AssignmentId is required.")]
    public string AssignmentId { get; set; }

    public Assignment Assignment { get; set; }

    [Required(ErrorMessage = "SubmissionUrl is required.")]
    [Url(ErrorMessage = "Invalid submission URL format.")]
    public string SubmissionUrl { get; set; }

    public string Notes { get; set; } = string.Empty;

    [Required(ErrorMessage = "SubmittedDate is required.")]
    public DateOnly SubmittedDate { get; set; }

    [Required(ErrorMessage = "Status is required.")]
    public string Status { get; set; } // Submitted / Resubmitted
}
