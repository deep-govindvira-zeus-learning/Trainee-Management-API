namespace TraineeManagementApi.DTOs;

public static class SubmissionProcessingRequestedConverter
{
    public static SubmissionProcessingRequested ToSubmissionProcessingRequested(SubmissionFileResponse submissionFileResponse)
    {
        return new SubmissionProcessingRequested
        {
            MessageId = Guid.NewGuid(),
            CorrelationId = Guid.NewGuid(),
            SubmissionId = submissionFileResponse.SubmissionId,
            FileId = submissionFileResponse.Id,
            RequestedAt = DateTimeOffset.UtcNow

        };
    }
}