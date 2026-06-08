using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using TraineeManagementApi.Data;
using TraineeManagementApi.DTOs;
using TraineeManagementApi.Models;

namespace TraineeManagementApi.Services;

public class TraineeService : ITraineeService
{
    private readonly AppDbContext _context;

    public TraineeService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<TraineeResponse>> GetAllTraineeAsync(string? search)
    {
        var query = _context.Trainees.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.ToLower();

            query = query.Where(trainee => 
                trainee.FirstName.ToLower().Contains(search) ||
                trainee.LastName.ToLower().Contains(search) ||
                trainee.Email.ToLower().Contains(search) ||
                trainee.TechStack.ToLower().Contains(search)
            );
        }

        var trainees = await query.ToListAsync();

        return TraineeConverter.ToTraineeResponseList(trainees);
    }

    public async Task<TraineeResponse?> GetTraineeByIdAsync(string id)
    {
        var trainee = await _context.Trainees.FindAsync(id);
        if (trainee == null) return null;
        return TraineeConverter.ToTraineeResponse(trainee);
    }

    public async Task<TraineeResponse> CreateTraineeAsync(CreateTraineeRequest request)
    {
        Trainee trainee = TraineeConverter.ToTrainee(request);
        await _context.Trainees.AddAsync(trainee);
        await _context.SaveChangesAsync();
        return TraineeConverter.ToTraineeResponse(trainee);
    }


    public async Task<TraineeResponse> UpdateTraineeAsync(string id, UpdateTraineeRequest request)
    {
        var existing = await _context.Trainees.FindAsync(id);
        if (existing == null) return null;

        existing.FirstName = request.FirstName;
        existing.LastName = request.LastName;
        existing.Email = request.Email;
        existing.TechStack = request.TechStack;
        existing.Status = request.Status;
        existing.UpdatedDate = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return TraineeConverter.ToTraineeResponse(existing);
    }

    public async Task<bool> DeleteTraineeAsync(string id)
    {
        var trainee = await _context.Trainees.FindAsync(id);
        if (trainee == null) return false;

        _context.Trainees.Remove(trainee);
        await _context.SaveChangesAsync();
        return true;
    }
}

