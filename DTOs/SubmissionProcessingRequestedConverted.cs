using TraineeManagementApi.Models;

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

    public static SubmissionProcessingRequested ToSubmissionProcessingRequested(ProcessingJob job)
    {
        if (job == null) throw new ArgumentNullException(nameof(job));

        return new SubmissionProcessingRequested
        {
            MessageId = job.MessageId,
            CorrelationId = job.CorrelationId,
            SubmissionId = job.SubmissionId,
            FileId = job.FileId,
            RequestedAt = job.RequestedAt
        };
    }
}