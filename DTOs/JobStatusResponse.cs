namespace TraineeManagementApi.DTOs;

public record JobStatusResponse(
    Guid JobId,
    string SubmissionId,
    string FileId,
    Guid MessageId,
    Guid CorrelationId,
    string Status,
    int Attempts,
    string? ErrorSummary,
    string? GeneratedChecksum,
    DateTimeOffset RequestedAt,
    DateTimeOffset? StartedAt,
    DateTimeOffset? CompletedAt
);
