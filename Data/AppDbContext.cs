using Microsoft.EntityFrameworkCore;
using TraineeManagementApi.Models;

namespace TraineeManagementApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }

    public DbSet<Trainee> Trainees { get; set; }
}