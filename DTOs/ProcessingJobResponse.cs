namespace TraineeManagementApi.DTOs;

public class ProcessingJobResponse
{
    public Guid JobId { get; set; }
    public string SubmissionId { get; set; } = string.Empty;
    public string FileId { get; set; } = string.Empty;
    public Guid MessageId { get; set; }
    public Guid CorrelationId { get; set; }
    public string Status { get; set; } = string.Empty;
    public int Attempts { get; set; }
    public string? ErrorSummary { get; set; }
    public string? GeneratedChecksum { get; set; }
    public DateTimeOffset RequestedAt { get; set; }
    public DateTimeOffset? StartedAt { get; set; }
    public DateTimeOffset? CompletedAt { get; set; }
}
