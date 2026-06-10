using TraineeManagementApi.DTOs;

namespace TraineeManagementApi.Services;

public interface ITraineeService
{
    Task<PagedResponse<TraineeResponse>> GetAllTraineeAsync(
        string? search,
        string? status,
        int pageNumber,
        int pageSize);

    Task<TraineeResponse> GetTraineeByIdAsync(string id);
    Task<TraineeResponse> CreateTraineeAsync(CreateTraineeRequest request);
    Task<TraineeResponse> UpdateTraineeAsync(string id, UpdateTraineeRequest request);
    Task<bool> DeleteTraineeAsync(string id);
}
