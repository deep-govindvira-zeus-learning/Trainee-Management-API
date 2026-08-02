using Microsoft.Extensions.Configuration;
using TraineeManagementApi.Data;
using TraineeManagementApi.Models;

namespace TraineeManagementApi.Data;

public static class DbInitializer
{
    // Seed credentials come from configuration/secrets (e.g. user-secrets, environment
    // variables, or a "SeedData" section in appsettings — which is gitignored) rather than
    // being hardcoded in source. The literals below are only a local-dev fallback so a fresh
    // checkout still boots without extra setup, and are never appropriate for a shared/staging
    // database.
    public static void Seed(AppDbContext context, IConfiguration configuration)
    {
        context.Database.EnsureCreated();

        var seedSection = configuration.GetSection("SeedData");

        var admin = new User
        {
            Id = Guid.NewGuid().ToString(),
            Username = "admin",
            Email = "admin@test.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(seedSection["AdminPassword"] ?? "admin@123"),
            Role = UserRole.Admin,
        };

        var userTrainee1 = new User
        {
            Id = Guid.NewGuid().ToString(),
            Username = "trainee1",
            Email = "trainee1@test.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(seedSection["TraineePassword"] ?? "trainee1@123"),
            Role = UserRole.Trainee,
        };

        var userMentor1 = new User
        {
            Id = Guid.NewGuid().ToString(),
            Username = "mentor1",
            Email = "mentor1@test.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(seedSection["MentorPassword"] ?? "mentor1@123"),
            Role = UserRole.Mentor,
        };

        if (!context.Users.Any())
        {
            context.Users.AddRange(admin, userTrainee1, userMentor1);
            context.SaveChanges();
        }

        var trainee1 = new Trainee
        {
            Id = Guid.NewGuid().ToString(),
            FirstName = "Trainee-1",
            LastName = "Trainee-1-LastName",
            Email = "trainne1@test.com",
            TechStack = "C#, .NET",
            Status = "Active"
        };

        if (!context.Trainees.Any())
        {
            context.Trainees.AddRange(trainee1);
            context.SaveChanges();
        }

        var mentor1 = new Mentor
        {
            Id = Guid.NewGuid().ToString(),
            FirstName = "Mentor-1",
            LastName = "Mentor-1-LastName",
            Email = "mentor1@test.com",
            Expertise = "C#, .NET, MERN, Angular, AWS",
            Status = "Active",
        };

        if (!context.Mentors.Any())
        {
            context.Mentors.AddRange(mentor1);
            context.SaveChanges();
        }

        var learningTask1 = new LearningTask
        {
            Id = Guid.NewGuid().ToString(),
            Title = "Task-1",
            Description = "This is description of task-1.",
            ExpectedTechStack = "HTML, CSS, Javascript",
            DueDate = new DateOnly(2026, 6, 7),
            Status = "Closed"
        };

        if (!context.LearningTasks.Any())
        {
            context.LearningTasks.AddRange(learningTask1);
            context.SaveChanges();
        }

        var assignment1 = new Assignment
        {
            Id = Guid.NewGuid().ToString(),
            TraineeId = trainee1.Id,
            MentorId = mentor1.Id,
            LearningTaskId = learningTask1.Id,
            AssignedDate = new DateOnly(2026, 7, 6),
            DueDate = new DateOnly(2026, 8, 9),
            Remarks = "This is remark of this assignment.",
            Status = "Assigned"
        };

        if (!context.Assignments.Any())
        {
            context.Assignments.AddRange(assignment1);
            context.SaveChanges();
        }

        var submission1 = new Submission
        {
            Id = Guid.NewGuid().ToString(),
            AssignmentId = assignment1.Id,
            SubmissionUrl = "https://github.com/deep-govindvira-zeus-learning",
            Notes = "This is note of this submission.",
            Status = "Submitted",
            SubmittedDate = new DateOnly(2026, 9, 9)
        };

        if (!context.Submissions.Any())
        {
            context.Submissions.AddRange(submission1);
            context.SaveChanges();
        }

        Review review = new Review
        {
            Id = Guid.NewGuid().ToString(),
            SubmissionId = submission1.Id,
            MentorId = mentor1.Id,
            Feedback = "This is feedback of this review.",
            Score = 7,
            ReviewStatus = "Accepted",
            ReviewedDate = DateOnly.FromDateTime(DateTime.UtcNow)
        };

        if (!context.Reviews.Any())
        {
            context.Reviews.AddRange(review);
            context.SaveChanges();
        }
    }
}
