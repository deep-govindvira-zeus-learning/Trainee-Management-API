using TraineeManagementApi.Models;

namespace TraineeManagementApi.Services;

public interface IReviewService
{
    Task<List<ReviewResponse>> GetAllAsync();
    Task<ReviewResponse> GetByIdAsync(string id);
    Task<ReviewResponse> CreateAsync(CreateReviewRequest request);
}