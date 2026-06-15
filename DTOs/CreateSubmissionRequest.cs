using System.ComponentModel.DataAnnotations;

namespace TraineeManagementApi.Models;

public class CreateSubmissionRequest
{
    [Required(ErrorMessage = "TaskAssignmentId is required.")]
    public string AssignmentId { get; set; }

    [Required(ErrorMessage = "SubmissionUrl is required.")]
    [Url(ErrorMessage = "Invalid submission URL format.")]
    public string SubmissionUrl { get; set; }

    [Required(ErrorMessage = "Notes is required.")]
    public string Notes { get; set; } = string.Empty;

    [Required(ErrorMessage = "SubmittedDate is required.")]
    [DataType(DataType.Date, ErrorMessage = "Invalid date format.")]
    public DateOnly SubmittedDate { get; set; }

    [Required(ErrorMessage = "Status is required.")]
    [AllowedValues("Submitted", "Resubmitted", ErrorMessage = "Status must be Submitted or Resubmitted.")]
    public string Status { get; set; }
}
