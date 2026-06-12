namespace TraineeManagementApi.DTOs;

public static class LearningTaskConverter
{
    public static LearningTask ToLearningTask(CreateLearningTaskRequest request)
    {
        return new LearningTask
        {
            Id = Guid.NewGuid().ToString(),
            Title = request.Title,
            Description = request.Description,
            ExpectedTechStack = request.ExpectedTechStack,
            DueDate = request.DueDate,
            Status = request.Status
        };
    }

    public static LearningTaskResponse ToLearningTaskResponse(LearningTask task)
    {
        return new LearningTaskResponse
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            ExpectedTechStack = task.ExpectedTechStack,
            DueDate = task.DueDate,
            Status = task.Status,
            CreatedDate = task.CreatedDate,
            UpdatedDate = task.UpdatedDate
        };
    }

    public static List<LearningTaskResponse> ToLearningTaskResponseList(List<LearningTask> tasks)
    {
        List<LearningTaskResponse> responses = new();
        foreach (var task in tasks) responses.Add(ToLearningTaskResponse(task));
        return responses;
    }
}