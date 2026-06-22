using System.Data;
using Microsoft.EntityFrameworkCore;
using TraineeManagementApi.Data;
using TraineeManagementApi.DTOs;

namespace TraineeManagementApi.Services;

public class AssignmentService : IAssignmentService
{
    private readonly AppDbContext _context;
    private readonly ILogger<AssignmentService> _logger;
    private readonly ICacheService _cacheService;
    private static readonly TimeSpan CacheTtl = TimeSpan.FromMinutes(10);

    public AssignmentService(
        AppDbContext context, 
        ILogger<AssignmentService> logger, 
        ICacheService cacheService)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
    }

    public async Task<List<AssignmentResponse>> GetAllAsync()
    {
        _logger.LogInformation("Fetching all assignments from the database.");

        try
        {
            var assignments = await _context.Assignments
                .AsNoTracking()
                .ToListAsync();

            return AssignmentConverter.ToAssignmentResponseList(assignments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "A database exception occurred while fetching all assignments.");
            throw;
        }
    }

    public async Task<AssignmentResponse> GetByIdAsync(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            _logger.LogWarning("GetByIdAsync called with an empty or null ID.");
            throw new ArgumentException("Assignment ID cannot be null or empty.", nameof(id));
        }

        string cacheKey = $"task-assignment:{id}";

        var cachedResponse = await _cacheService.GetAsync<AssignmentResponse>(cacheKey);
        if (cachedResponse != null)
        {
            _logger.LogInformation("Cache HIT for assignment ID: {AssignmentId}", id);
            return cachedResponse;
        }

        _logger.LogInformation("Cache MISS. Fetching assignment with ID: {AssignmentId} from MySQL.", id);

        try
        {
            var assignment = await _context.Assignments
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == id);

            if (assignment == null)
            {
                _logger.LogWarning("Assignment with ID {AssignmentId} was not found.", id);
                throw new KeyNotFoundException($"Assignment with ID '{id}' was not found.");
            }

            var response = AssignmentConverter.ToAssignmentResponse(assignment);

            await _cacheService.SetAsync(cacheKey, response, CacheTtl);

            return response;
        }
        catch (Exception ex) when (ex is not KeyNotFoundException && ex is not ArgumentException)
        {
            _logger.LogError(ex, "A database exception occurred while fetching assignment ID: {AssignmentId}", id);
            throw;
        }
    }

    public async Task<AssignmentResponse> CreateAsync(CreateAssignmentRequest request)
    {
        if (request == null)
        {
            _logger.LogError("CreateAssignmentRequest is null.");
            throw new ArgumentNullException(nameof(request));
        }

        var traineeExists = await _context.Trainees.AnyAsync(t => t.Id == request.TraineeId);
        if (!traineeExists)
        {
            _logger.LogWarning("Database check failed: Trainee with ID {TraineeId} does not exist.", request.TraineeId);
            throw new KeyNotFoundException($"Trainee with ID '{request.TraineeId}' was not found.");
        }

        var mentorExists = await _context.Mentors.AnyAsync(m => m.Id == request.MentorId);
        if (!mentorExists)
        {
            _logger.LogWarning("Database check failed: Mentor with ID {MentorId} does not exist.", request.MentorId);
            throw new KeyNotFoundException($"Mentor with ID '{request.MentorId}' was not found.");
        }

        var taskExists = await _context.LearningTasks.AnyAsync(l => l.Id == request.LearningTaskId);
        if (!taskExists)
        {
            _logger.LogWarning("Database check failed: LearningTask with ID {LearningTaskId} does not exist.", request.LearningTaskId);
            throw new KeyNotFoundException($"Learning Task with ID '{request.LearningTaskId}' was not found.");
        }

        if (request.DueDate < request.AssignedDate)
        {
            throw new InvalidOperationException("The due date cannot be earlier than the assigned date.");
        }

        try
        {
            var assignment = AssignmentConverter.ToAssignment(request);

            await _context.Assignments.AddAsync(assignment);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Successfully created assignment with ID: {AssignmentId} in MySQL.", assignment.Id);

            // Invalidation Note: No cache eviction needed for GetById since this ID is brand new.
            return AssignmentConverter.ToAssignmentResponse(assignment);
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "A database update exception occurred while saving the assignment.");
            throw;
        }
    }

    public async Task<AssignmentResponse> UpdateStatusByIdAsync(string id, UpdateAssignmentStatusRequest request)
    {
        string status = request.Status;

        if (string.IsNullOrWhiteSpace(id))
        {
            throw new ArgumentException("Assignment ID cannot be null or empty.", nameof(id));
        }

        if (string.IsNullOrWhiteSpace(status))
        {
            throw new ArgumentException("Status cannot be null or empty.", nameof(status));
        }

        _logger.LogInformation("Updating status for assignment ID: {AssignmentId} to {Status}", id, status);

        try
        {
            var assignment = await _context.Assignments
                .FirstOrDefaultAsync(a => a.Id == id);

            if (assignment == null)
            {
                _logger.LogWarning("Assignment with ID {AssignmentId} not found for status update.", id);
                throw new KeyNotFoundException($"Assignment with ID '{id}' was not found.");
            }

            assignment.Status = status;
            await _context.SaveChangesAsync();

            _logger.LogInformation("Successfully updated status for assignment ID: {AssignmentId} in MySQL.", id);

            string cacheKey = $"task-assignment:{id}";
            await _cacheService.RemoveAsync(cacheKey);
            _logger.LogInformation("Proactively invalidated cache key: {CacheKey}", cacheKey);

            return AssignmentConverter.ToAssignmentResponse(assignment);
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Database error updating status for assignment ID: {AssignmentId}", id);
            throw;
        }
    }
}
