namespace TraineeManagementApi.Models;

public class ProcessingJob
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string SubmissionId { get; set; } = string.Empty;
    public string FileId { get; set; } = string.Empty;
    public Guid MessageId { get; set; }
    public Guid CorrelationId { get; set; }
    public JobStatus Status { get; set; } = JobStatus.Queued;
    public int Attempts { get; set; } = 0;
    public string? ErrorSummary { get; set; }
    public string? GeneratedChecksum { get; set; }
    public string? OutputFilePath { get; set; }
    public DateTimeOffset RequestedAt { get; set; }
    public DateTimeOffset? StartedAt { get; set; }
    public DateTimeOffset? CompletedAt { get; set; }
}
