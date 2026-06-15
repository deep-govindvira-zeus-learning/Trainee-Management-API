using System.ComponentModel.DataAnnotations;

namespace TraineeManagementApi.DTOs;

public class UpdateAssignmentStatusRequest
{
    [Required(ErrorMessage = "Status is required.")]
    [AllowedValues("Assigned", "InProgress", "Submitted", "Reviewed", "Completed", ErrorMessage = "Status must be Assigned, InProgress, Submitted, Reviewed or Completed.")]
    public string Status { get; set; }
}