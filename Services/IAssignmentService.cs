using TraineeManagementApi.DTOs;

namespace TraineeManagementApi.Services;

public interface IAssignmentService
{
    Task<List<AssignmentResponse>> GetAllAsync();
    Task<AssignmentResponse> GetByIdAsync(string id);
    Task<AssignmentResponse> CreateAsync(CreateAssignmentRequest request);
    Task<AssignmentResponse> UpdateStatusByIdAsync(string id, UpdateAssignmentStatusRequest request);
}
