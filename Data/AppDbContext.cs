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
    public DbSet<Mentor> Mentors { get; set; }
    public DbSet<LearningTask> LearningTasks { get; set; }
    public DbSet<Assignment> Assignments { get; set; }

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

        modelBuilder.Entity<Mentor>(entity =>
        {
            entity.HasCheckConstraint(
                "CK_Mentor_Status",
               "`Status` IN ('Active', 'Inactive')"
            );
        });

        modelBuilder.Entity<LearningTask>(entity =>
        {
            entity.HasCheckConstraint(
                "CK_LearningTask_Status",
               "`Status` IN ('Draft', 'Published', 'Closed')"
            );
        });

        modelBuilder.Entity<Assignment>(entity =>
        {
            entity.HasCheckConstraint(
                "CK_Assignment_Status",
               "`Status` IN ('Assigned', 'InProgress', 'Submitted', 'Reviewed', 'Completed')"
            );
        });

        // modelBuilder.Entity<Assignment>().HasOne(a => a.Trainee)
        //     .WithMany()
        //     .HasForeignKey(a => a.TraineeId)
        //     .OnDelete(DeleteBehavior.Restrict);

        // modelBuilder.Entity<Assignment>().HasOne(a => a.Mentor)
        //     .WithMany()
        //     .HasForeignKey(a => a.MentorId)
        //     .OnDelete(DeleteBehavior.Restrict);

        // modelBuilder.Entity<Assignment>().HasOne(a => a.LearningTask)
        //     .WithMany()
        //     .HasForeignKey(a => a.LearningTask)
        //     .OnDelete(DeleteBehavior.Restrict);

    }
}