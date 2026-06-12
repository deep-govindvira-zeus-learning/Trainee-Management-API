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

    public async Task<LearningTaskResponse> UpdateByIdAsync(string id, UpdateLearningTaskRequest request)
    {
        var existing = await _context.LearningTasks.FindAsync(id);

        if (existing == null)
        {
            throw new Exception($"Learning Task with {id} not found");
        }

        existing.Title = request.Title;
        existing.Description = request.Description;
        existing.ExpectedTechStack = request.ExpectedTechStack;
        existing.DueDate = request.DueDate;
        existing.Status = request.Status;

        await _context.SaveChangesAsync();

        return LearningTaskConverter.ToLearningTaskResponse(existing);
    }

    public async Task<bool> DeleteByIdAsync(string id)
    {
        var task = await _context.LearningTasks.FindAsync(id);
        _context.LearningTasks.Remove(task);
        await _context.SaveChangesAsync();
        return true;
    }
}
