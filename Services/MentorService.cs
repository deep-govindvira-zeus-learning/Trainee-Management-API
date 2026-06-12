using Microsoft.EntityFrameworkCore;
using TraineeManagementApi.Data;
using TraineeManagementApi.DTOs;

namespace TraineeManagementApi.Services;

public class MentorService : IMentorService
{
    private readonly AppDbContext _context;
    private readonly ILogger<MentorService> _logger;

    public MentorService(AppDbContext context, ILogger<MentorService> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<List<MentorResponse>> GetAllAsync()
    {
        _logger.LogInformation("Fetching all mentors from the database.");
        try
        {
            var mentors = await _context.Mentors.ToListAsync();
            _logger.LogInformation("Successfully retrieved {Count} mentors.", mentors.Count);
            return MentorConverter.ToMentorResponseList(mentors);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while fetching all mentors.");
            throw;
        }
    }

    public async Task<MentorResponse> GetByIdAsync(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            _logger.LogWarning("GetByIdAsync called with an empty or null ID.");
            throw new ArgumentNullException(nameof(id), "Mentor ID cannot be null or empty.");
        }

        _logger.LogInformation("Fetching mentor with ID: {MentorId}", id);
        
        var mentor = await _context.Mentors.FindAsync(id);

        if (mentor == null)
        {
            _logger.LogWarning("Mentor with ID: {MentorId} was not found.", id);
            throw new KeyNotFoundException($"Mentor with ID: {id} does not exist.");
        }

        return MentorConverter.ToMentorResponse(mentor);
    }

    public async Task<MentorResponse> CreateAsync(CreateMentorRequest request)
    {
        if (request == null)
        {
            _logger.LogWarning("CreateAsync called with a null request payload.");
            throw new ArgumentNullException(nameof(request), "Create mentor request cannot be null.");
        }

        _logger.LogInformation("Creating a new mentor with Email: {Email}", request.Email);

        try
        {
            var mentor = MentorConverter.ToMentor(request);
            await _context.Mentors.AddAsync(mentor);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Successfully created mentor with ID: {MentorId}", mentor.Id);
            return MentorConverter.ToMentorResponse(mentor);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create mentor with Email: {Email}", request.Email);
            throw;
        }
    }

    public async Task<bool> DeleteByIdAsync(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            _logger.LogWarning("DeleteByIdAsync called with an empty or null ID.");
            throw new ArgumentNullException(nameof(id), "Mentor ID cannot be null or empty.");
        }

        _logger.LogInformation("Attempting to delete mentor with ID: {MentorId}", id);

        var mentor = await _context.Mentors.FindAsync(id);

        if (mentor == null)
        {
            _logger.LogWarning("Delete failed. Mentor with ID: {MentorId} not found.", id);
            throw new KeyNotFoundException($"Mentor with ID: {id} does not exist.");
        }

        try
        {
            _context.Mentors.Remove(mentor);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Successfully deleted mentor with ID: {MentorId}", id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while deleting mentor with ID: {MentorId}", id);
            throw;
        }
    }

    public async Task<MentorResponse> UpdateByIdAsync(string id, UpdateMentorRequest request)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            _logger.LogWarning("UpdateByIdAsync called with an empty or null ID.");
            throw new ArgumentNullException(nameof(id), "Mentor ID cannot be null or empty.");
        }

        if (request == null)
        {
            _logger.LogWarning("UpdateByIdAsync called with a null request payload for ID: {MentorId}", id);
            throw new ArgumentNullException(nameof(request), "Update mentor request cannot be null.");
        }

        _logger.LogInformation("Attempting to update mentor with ID: {MentorId}", id);

        var existing = await _context.Mentors.FindAsync(id);

        if (existing == null)
        {
            _logger.LogWarning("Update failed. Mentor with ID: {MentorId} not found.", id);
            throw new KeyNotFoundException($"Mentor with ID: {id} does not exist.");
        }

        try
        {
            existing.FirstName = request.FirstName;
            existing.LastName = request.LastName;
            existing.Email = request.Email;
            existing.Expertise = request.Expertise;
            existing.Status = request.Status;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Successfully updated mentor with ID: {MentorId}", id);
            return MentorConverter.ToMentorResponse(existing);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while updating mentor with ID: {MentorId}", id);
            throw;
        }
    }
}
