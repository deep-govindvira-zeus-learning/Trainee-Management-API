namespace TraineeManagementApi.DTOs;

public class SubmissionResponse
{
    public string Id { get; set; }
    public string AssignmentId { get; set; }

    public string SubmissionUrl { get; set; }

    public string Notes { get; set; } = string.Empty;
    public DateOnly SubmittedDate { get; set; }

    public string Status { get; set; }
}
