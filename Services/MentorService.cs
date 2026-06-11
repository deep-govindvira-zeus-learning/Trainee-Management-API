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
}
