using System.ComponentModel.DataAnnotations;

namespace TraineeManagementApi.DTOs;


public class CreateAssignmentRequest
{
    [Required(ErrorMessage = "TraineeId is required.")]
    public string TraineeId { get; set; } = string.Empty;

    [Required(ErrorMessage = "MentorId is required.")]
    public string MentorId { get; set; } = string.Empty;

    [Required(ErrorMessage = "LearningTaskId is required.")]
    public string LearningTaskId { get; set; } = string.Empty;

    [Required(ErrorMessage = "AssignedDate is required.")]
    [DataType(DataType.Date, ErrorMessage = "Invalid date format.")]
    public DateOnly AssignedDate { get; set; }

    [Required(ErrorMessage = "DueDate is required.")]
    [DataType(DataType.Date, ErrorMessage = "Invalid date format.")]
    public DateOnly DueDate { get; set; }

    [Required(ErrorMessage = "Status is required.")]
    [AllowedValues("Assigned", "InProgress", "Submitted", "Reviewed", "Completed", ErrorMessage = "Status must be Assigned, InProgress, Submitted, Reviewed or Completed.")]
    public string Status { get; set; } = string.Empty;

    public string Remarks { get; set; } = string.Empty;
}