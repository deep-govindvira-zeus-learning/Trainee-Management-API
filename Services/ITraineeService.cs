using TraineeManagementApi.DTOs;

namespace TraineeManagementApi.Services;

public interface ITraineeService
{
    Task<List<TraineeResponse>> GetAllTraineeAsync(string? search);
    Task<TraineeResponse?> GetTraineeByIdAsync(string id);
    Task<TraineeResponse> CreateTraineeAsync(CreateTraineeRequest request);
    Task<TraineeResponse> UpdateTraineeAsync(string id, UpdateTraineeRequest request);
    Task<bool> DeleteTraineeAsync(string id);
}
