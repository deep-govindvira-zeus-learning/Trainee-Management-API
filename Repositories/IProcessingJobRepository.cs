using TraineeManagementApi.Models;

namespace TraineeManagementApi.Repositories;

public interface IProcessingJobRepository
{
    Task<ProcessingJob?> GetByIdAsync(Guid id);
}
