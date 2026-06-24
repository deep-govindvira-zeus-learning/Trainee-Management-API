using TraineeManagementApi.Models;

namespace TraineeManagementApi.DTOs;

public static class ProcessingJobResponseConverter
{
    public static ProcessingJobResponse ToProcessingJobResponse(ProcessingJob job)
    {
        return new ProcessingJobResponse
        {
            JobId = job.Id,
            SubmissionId = job.SubmissionId,
            FileId = job.FileId,
            MessageId = job.MessageId,
            CorrelationId = job.CorrelationId,
            Status = job.Status.ToString(),
            Attempts = job.Attempts,
            ErrorSummary = job.ErrorSummary,
            GeneratedChecksum = job.GeneratedChecksum,
            RequestedAt = job.RequestedAt,
            StartedAt = job.StartedAt,
            CompletedAt = job.CompletedAt
        };
    }
}