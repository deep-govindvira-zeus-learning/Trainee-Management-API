using TraineeManagementApi.DTOs;
using TraineeManagementApi.Models;

namespace TraineeManagementApi.Services;

public interface ISubmissionService
{
    Task<List<SubmissionResponse>> GetAllAsync();
    Task<SubmissionResponse> GetByIdAsync(string id);
    Task<SubmissionResponse> CreateAsync(CreateSubmissionRequest request);
    Task<List<SubmissionFileResponse>> UploadFilesAsync(string submissionId, List<IFormFile> files, string userIdentity);
}
