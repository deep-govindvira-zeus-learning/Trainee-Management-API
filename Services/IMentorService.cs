using TraineeManagementApi.DTOs;

namespace TraineeManagementApi.Services;

public interface IMentorService
{
    Task<List<MentorResponse>> GetAllMentorAsync();

    Task<MentorResponse> GetMentorByIdAsync(string id);
    Task<MentorResponse> CreateMentorAsync(CreateMentorRequest request);
    Task<MentorResponse> UpdateMentorAsync(string id, UpdateMentorRequest request);
    Task<bool> DeleteMentorAsync(string id);
}