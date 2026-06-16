namespace TraineeManagementApi.DTOs;

public static class AssignmentConverter
{
    public static AssignmentResponse ToAssignmentResponse(Assignment assignment)
    {
        return new AssignmentResponse
        {
            Id = assignment.Id,
            TraineeId = assignment.TraineeId,
            MentorId = assignment.MentorId,
            LearningTaskId = assignment.LearningTaskId,
            AssignedDate = assignment.AssignedDate,
            DueDate = assignment.DueDate,
            Status = assignment.Status,
            Remarks = assignment.Remarks,
            CreatedDate = assignment.CreatedDate,
            UpdatedDate = assignment.UpdatedDate
        };
    }

    public static Assignment ToAssignment(CreateAssignmentRequest request)
    {
        return new Assignment
        {
            Id = Guid.NewGuid().ToString(),
            TraineeId = request.TraineeId,
            MentorId = request.MentorId,
            LearningTaskId = request.LearningTaskId,
            AssignedDate = request.AssignedDate,
            DueDate = request.DueDate,
            Status = request.Status,
            Remarks = request.Remarks
        };
    }

    public static List<AssignmentResponse> ToAssignmentResponseList(List<Assignment> assignments)
    {
        return assignments.Select(ToAssignmentResponse).ToList();
    }
}