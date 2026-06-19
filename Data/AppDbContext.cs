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
    public DbSet<Submission> Submissions { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<SubmissionFile> SubmissionFiles { get; set; }


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

        modelBuilder.Entity<User>()
            .Property(u => u.Role)
            .HasConversion<string>();

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


        modelBuilder.Entity<Submission>(entity =>
        {
            entity.HasCheckConstraint(
                "CK_Submission_Status",
               "`Status` IN ('Submitted', 'Resubmitted')"
            );
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasCheckConstraint(
                "CK_Review_Status",
               "`ReviewStatus` IN ('Accepted', 'ChangesRequired', 'Rejected')"
            );
        });

        modelBuilder.Entity<Assignment>()
            .HasOne(a => a.Trainee)
            .WithMany()
            .HasForeignKey(a => a.TraineeId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Assignment>()
            .HasOne(a => a.Mentor)
            .WithMany()
            .HasForeignKey(a => a.MentorId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Assignment>()
            .HasOne(a => a.LearningTask)
            .WithMany()
            .HasForeignKey(a => a.LearningTaskId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Submission>()
            .HasOne(s => s.Assignment)
            .WithMany()
            .HasForeignKey(a => a.AssignmentId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Review>()
           .HasOne(s => s.Submission)
           .WithMany()
           .HasForeignKey(a => a.SubmissionId)
           .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Review>()
            .HasOne(s => s.Mentor)
            .WithMany()
            .HasForeignKey(a => a.MentorId)
            .OnDelete(DeleteBehavior.Restrict);


        modelBuilder.Entity<SubmissionFile>(builder =>
        {
            builder.ToTable("SubmissionFiles");
            builder.HasKey(f => f.Id);

            builder.Property(f => f.OriginalFileName).IsRequired().HasMaxLength(255);
            builder.Property(f => f.StorageName).IsRequired().HasMaxLength(100);
            builder.Property(f => f.ContentType).IsRequired().HasMaxLength(100);
            builder.Property(f => f.Checksum).IsRequired().HasMaxLength(64);
            builder.Property(f => f.UploadedBy).IsRequired().HasMaxLength(100);

            builder.HasIndex(f => f.StorageName).IsUnique();

            // Configure relation mapping targeting your string key schema
            builder.HasOne(f => f.Submission)
                   .WithMany(s => s.Files)
                   .HasForeignKey(f => f.SubmissionId)
                   .OnDelete(DeleteBehavior.Cascade);
        });
    }
}