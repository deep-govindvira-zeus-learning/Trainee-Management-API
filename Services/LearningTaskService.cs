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

    public async Task<LearningTaskResponse> CreateAsync(CreateLearningTaskRequest request)
    {
        LearningTask task = LearningTaskConverter.ToLearningTask(request);
        await _context.LearningTasks.AddAsync(task);
        await _context.SaveChangesAsync();
        return LearningTaskConverter.ToLearningTaskResponse(task);
    }
}
