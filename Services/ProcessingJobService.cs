using TraineeManagementApi.DTOs;
using TraineeManagementApi.Repositories;

namespace TraineeManagementApi.Services;

public class ProcessingJobService : IProcessingJobService
{
    private readonly IProcessingJobRepository _repository;

    public ProcessingJobService(IProcessingJobRepository repository)
    {
        _repository = repository;
    }

    public async Task<ProcessingJobResponse?> GetByIdAsync(Guid id)
    {
        var job = await _repository.GetByIdAsync(id);
        
        if (job == null)
        {
            throw new KeyNotFoundException($"Job with ID '{id}' was not found.");
        }

        return ProcessingJobResponseConverter.ToProcessingJobResponse(job);
    }
}
