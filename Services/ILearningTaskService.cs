using TraineeManagementApi.DTOs;

namespace TraineeManagementApi.Services;

public interface ILearningTaskService
{
    Task<List<LearningTaskResponse>> GetAllAsync();
    Task<LearningTaskResponse> GetByIdAsync(string id);
}
