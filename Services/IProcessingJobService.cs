using TraineeManagementApi.DTOs;

namespace TraineeManagementApi.Services;

public interface IProcessingJobService
{
    Task<ProcessingJobResponse?> GetByIdAsync(Guid id);

    Task<ProcessingJobResponse?> RetryById(Guid id);
}
