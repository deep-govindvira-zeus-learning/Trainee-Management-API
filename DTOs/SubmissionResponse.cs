namespace TraineeManagementApi.DTOs;

public class SubmissionResponse
{
    public string Id { get; set; } = string.Empty;
    public string AssignmentId { get; set; } = string.Empty;

    public string SubmissionUrl { get; set; } = string.Empty;

    public string Notes { get; set; } = string.Empty;
    public DateOnly SubmittedDate { get; set; }

    public string Status { get; set; } = string.Empty;
    public List<SubmissionFileResponse> Files { get; set; } = new List<SubmissionFileResponse>();
}
