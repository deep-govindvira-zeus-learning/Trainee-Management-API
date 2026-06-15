using TraineeManagementApi.Models;

namespace TraineeManagementApi.DTOs;

public static class SubmissionConverter
{
    public static SubmissionResponse ToSubmissionResponse(Submission submission)
    {
        return new SubmissionResponse
        {
            Id = submission.Id,
            AssignmentId = submission.AssignmentId,
            SubmissionUrl = submission.SubmissionUrl,
            Notes = submission.Notes,
            Status = submission.Status,
            SubmittedDate = submission.SubmittedDate,
        };
    }

    public static Submission ToSubmission(CreateSubmissionRequest request)
    {
        return new Submission
        {
            Id = Guid.NewGuid().ToString(),
            AssignmentId = request.AssignmentId,
            SubmissionUrl = request.SubmissionUrl,
            Notes = request.Notes ?? string.Empty,
            SubmittedDate = DateOnly.FromDateTime(DateTime.UtcNow),
            Status = request.Status
        };
    }

    public static List<SubmissionResponse> ToSubmissionResponseList(List<Submission> submissions)
    {

        return submissions.Select(submission => new SubmissionResponse
        {
            Id = submission.Id,
            AssignmentId = submission.AssignmentId,
            SubmissionUrl = submission.SubmissionUrl,
            Notes = submission.Notes,
            Status = submission.Status,
            SubmittedDate = submission.SubmittedDate
        }).ToList();
    }
}