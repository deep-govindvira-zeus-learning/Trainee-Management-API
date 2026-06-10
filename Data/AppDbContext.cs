using Microsoft.EntityFrameworkCore;
using TraineeManagementApi.Models;

namespace TraineeManagementApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {

    }

    public DbSet<Trainee> Trainees { get; set; }

    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Trainee>(entity =>
        {
            entity.HasCheckConstraint(
                "CK_Trainee_Status",
                "`Status` IN ('Active', 'Inactive', 'Completed')"
            );
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasCheckConstraint(
                "CK_User_Role",
                "`Role` IN ('Admin', 'Mentor', 'Trainee')"
            );
        });
    }

}