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
        _context = context;
        _logger = logger;
    }

    public async Task<List<MentorResponse>> GetAllMentorAsync()
    {
        var mentors = await _context.Mentors.ToListAsync();
        return MentorConverter.ToMentorResponseList(mentors);
    }

    public async Task<MentorResponse> GetMentorByIdAsync(string id)
    {
        var mentor = await _context.Mentors.FindAsync(id);
        return MentorConverter.ToMentorResponse(mentor);
    }

    public async Task<MentorResponse> CreateMentorAsync(CreateMentorRequest request)
    {
        Mentor mentor = MentorConverter.ToMentor(request);
        await _context.Mentors.AddAsync(mentor);
        await _context.SaveChangesAsync();
        return MentorConverter.ToMentorResponse(mentor);
    }

    public async Task<bool> DeleteMentorAsync(string id)
    {
        var mentor = await _context.Mentors.FindAsync(id);
        _context.Mentors.Remove(mentor);
        await _context.SaveChangesAsync();
        return true;
    }
}
