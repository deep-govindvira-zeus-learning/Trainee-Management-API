using TraineeManagementApi.DTOs;

namespace TraineeManagementApi.Services;

public interface IMentorService
{
    Task<List<MentorResponse>> GetAllAsync();

    Task<MentorResponse> GetByIdAsync(string id);
    Task<MentorResponse> CreateAsync(CreateMentorRequest request);
    Task<MentorResponse> UpdateByIdAsync(string id, UpdateMentorRequest request);
    Task<bool> DeleteByIdAsync(string id);
}