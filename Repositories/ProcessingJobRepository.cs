using Microsoft.EntityFrameworkCore;
using TraineeManagementApi.Data;
using TraineeManagementApi.Models;

namespace TraineeManagementApi.Repositories;

public class ProcessingJobRepository : IProcessingJobRepository
{
    private readonly AppDbContext _context;

    public ProcessingJobRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ProcessingJob?> GetByIdAsync(Guid id)
    {
        return await _context.ProcessingJobs
            .AsNoTracking()
            .FirstOrDefaultAsync(j => j.Id == id);
    }
}
