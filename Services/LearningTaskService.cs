using Microsoft.EntityFrameworkCore;
using TraineeManagementApi.Data;
using TraineeManagementApi.DTOs;

namespace TraineeManagementApi.Services;


public class LearningTaskService : ILearningTaskService
{
    private readonly AppDbContext _context;

    public LearningTaskService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<LearningTaskResponse>> GetAllAsync()
    {
        var tasks = await _context.LearningTasks.ToListAsync();
        return LearningTaskConverter.ToLearningTaskResponseList(tasks);
    }

    public async Task<LearningTaskResponse> GetByIdAsync(string id)
    {
        var task = await _context.LearningTasks.FindAsync(id);
        return LearningTaskConverter.ToLearningTaskResponse(task);
    }

}
