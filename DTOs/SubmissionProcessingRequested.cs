namespace TraineeManagementApi.DTOs;

public record SubmissionProcessingRequested
{
    public Guid MessageId { get; set; } = Guid.NewGuid();
    public Guid CorrelationId { get; set; }
    public string SubmissionId { get; set; }
    public string FileId { get; set; }
    public DateTimeOffset RequestedAt { get; set; } = DateTimeOffset.UtcNow;
    public string ContractVersion { get; set; } = "1.0.0";
}
