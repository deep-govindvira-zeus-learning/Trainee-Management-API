using TraineeManagementApi.DTOs;

namespace TraineeManagementApi.Services;

public interface ITraineeService
{
    Task<PagedResponse<TraineeResponse>> GetAllAsync(
        string? search,
        string? status,
        int pageNumber,
        int pageSize);

    Task<TraineeResponse> GetByIdAsync(string id);
    Task<TraineeResponse> CreateAsync(CreateTraineeRequest request);
    Task<TraineeResponse> UpdateByIdAsync(string id, UpdateTraineeRequest request);
    Task<bool> DeleteByIdAsync(string id);
}
