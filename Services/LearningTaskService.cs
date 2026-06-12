using Microsoft.EntityFrameworkCore;
using TraineeManagementApi.Data;
using TraineeManagementApi.DTOs;

namespace TraineeManagementApi.Services;

public class LearningTaskService : ILearningTaskService
{
    private readonly AppDbContext _context;
    private readonly ILogger<LearningTaskService> _logger;

    public LearningTaskService(AppDbContext context, ILogger<LearningTaskService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<LearningTaskResponse>> GetAllAsync()
    {
        _logger.LogInformation("Retrieving all learning tasks.");
        try
        {
            var tasks = await _context.LearningTasks.ToListAsync();
            _logger.LogInformation("Successfully retrieved {Count} learning tasks.", tasks.Count);
            return LearningTaskConverter.ToLearningTaskResponseList(tasks);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve all learning tasks.");
            throw new Exception("Error retrieving all learning tasks.", ex);
        }
    }

    public async Task<LearningTaskResponse> GetByIdAsync(string id)
    {
        _logger.LogInformation("Retrieving learning task with ID: {Id}", id);
        try
        {
            var task = await _context.LearningTasks.FindAsync(id);

            if (task == null)
            {
                _logger.LogWarning("Learning Task with ID {Id} was not found.", id);
                throw new KeyNotFoundException($"Learning Task with ID {id} not found.");
            }

            _logger.LogInformation("Successfully retrieved learning task with ID: {Id}", id);
            return LearningTaskConverter.ToLearningTaskResponse(task);
        }
        catch (KeyNotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving learning task with ID: {Id}", id);
            throw new Exception($"Error retrieving learning task with ID {id}.", ex);
        }
    }

    public async Task<LearningTaskResponse> CreateAsync(CreateLearningTaskRequest request)
    {
        _logger.LogInformation("Creating a new learning task with Title: {Title}", request.Title);
        try
        {
            LearningTask task = LearningTaskConverter.ToLearningTask(request);
            await _context.LearningTasks.AddAsync(task);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation("Successfully created learning task with ID: {Id}", task.Id);
            return LearningTaskConverter.ToLearningTaskResponse(task);
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Database error occurred while creating a learning task.");
            throw new Exception("Database error occurred while creating the learning task.", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while creating a learning task.");
            throw new Exception("An unexpected error occurred while creating the learning task.", ex);
        }
    }

    public async Task<LearningTaskResponse> UpdateByIdAsync(string id, UpdateLearningTaskRequest request)
    {
        _logger.LogInformation("Updating learning task with ID: {Id}", id);
        try
        {
            var existing = await _context.LearningTasks.FindAsync(id);

            if (existing == null)
            {
                _logger.LogWarning("Learning Task with ID {Id} not found for update.", id);
                throw new KeyNotFoundException($"Learning Task with ID {id} not found.");
            }

            existing.Title = request.Title;
            existing.Description = request.Description;
            existing.ExpectedTechStack = request.ExpectedTechStack;
            existing.DueDate = request.DueDate;
            existing.Status = request.Status;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Successfully updated learning task with ID: {Id}", id);
            return LearningTaskConverter.ToLearningTaskResponse(existing);
        }
        catch (KeyNotFoundException)
        {
            throw;
        }
        catch (DbUpdateConcurrencyException ex)
        {
            _logger.LogError(ex, "Concurrency error occurred while updating learning task with ID: {Id}", id);
            throw new Exception($"Concurrency error occurred while updating learning task with ID {id}.", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating learning task with ID: {Id}", id);
            throw new Exception($"Error updating learning task with ID {id}.", ex);
        }
    }

    public async Task<bool> DeleteByIdAsync(string id)
    {
        _logger.LogInformation("Deleting learning task with ID: {Id}", id);
        try
        {
            var task = await _context.LearningTasks.FindAsync(id);

            if (task == null)
            {
                _logger.LogWarning("Learning Task with ID {Id} not found for deletion.", id);
                throw new KeyNotFoundException($"Learning Task with ID {id} not found.");
            }

            _context.LearningTasks.Remove(task);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation("Successfully deleted learning task with ID: {Id}", id);
            return true;
        }
        catch (KeyNotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting learning task with ID: {Id}", id);
            throw new Exception($"Error deleting learning task with ID {id}.", ex);
        }
    }
}
