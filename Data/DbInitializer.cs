using TraineeManagementApi.Data;
using TraineeManagementApi.Models;

namespace TraineeManagementApi.Data;

public static class DbInitializer
{
    public static void Seed(AppDbContext context)
    {
        context.Database.EnsureCreated();

        if (!context.Trainees.Any())
        {
            context.Trainees.AddRange(
                new Trainee
                {
                    Id = "208b07fc-0511-4387-8f57-80f332381a4a",
                    FirstName = "Trainee-1",
                    LastName = "Trainee-1-Surname",
                    Email = "trainne.1@example.com",
                    TechStack = "C#, .NET",
                    Status = "Active"
                },
                new Trainee
                {
                    Id = "3631c4b3-c824-4374-a1ef-204237fccfac",
                    FirstName = "Trainee-2",
                    LastName = "Trainee-2-Surname",
                    Email = "trainne.2@example.com",
                    TechStack = "Java, Spring Boot",
                    Status = "InActive",
                },
                new Trainee
                {
                    Id = "522d1097-e6e4-4b91-807f-ca8358fd618e",
                    FirstName = "Trainee-3",
                    LastName = "Trainee-3-Surname",
                    Email = "trainne.3@example.com",
                    TechStack = "React, Node",
                    Status = "Completed",
                }
            );
            context.SaveChanges();
        }

        if (!context.Mentors.Any())
        {
            context.Mentors.AddRange(
                new Mentor
                {
                    Id = "2fdc755a-5118-4ed8-9e21-300133d7c088",
                    FirstName = "Mentor-1",
                    LastName = "Mentor-1-Surname",
                    Email = "mentor.1@example.com",
                    Expertise = "C#, .NET, MERN, Angular, AWS",
                    Status = "Active",
                },
                new Mentor
                {
                    Id = "8f9241fe-9464-4d57-97d8-6964c67ff98d",
                    FirstName = "Mentor-2",
                    LastName = "Mentor-2-Surname",
                    Email = "mentor.2@example.com",
                    Expertise = "C#, .NET",
                    Status = "Inactive",
                }
            );
            context.SaveChanges();
        }

        if (!context.Users.Any())
        {
            context.Users.AddRange(new User
            {
                Id = Guid.NewGuid().ToString(),
                Username = "admin",
                Email = "admin@test.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                Role = UserRole.Admin,
            }, new User
            {
                Id = Guid.NewGuid().ToString(),
                Username = "trainee",
                Email = "trainee@test.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Trainee@123"),
                Role = UserRole.Trainee,
            }, new User
            {
                Id = Guid.NewGuid().ToString(),
                Username = "mentor",
                Email = "mentor@test.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Mentor@123"),
                Role = UserRole.Mentor,
            });
            context.SaveChanges();
        }

        if (!context.LearningTasks.Any())
        {
            context.LearningTasks.AddRange(new LearningTask
            {
                Id = "036d2863-ff85-4861-8843-aa9f150bedec",
                Title = "Task-1",
                Description = "This is description of task-1.",
                ExpectedTechStack = "HTML, CSS, Javascript",
                DueDate = new DateOnly(2026, 6, 7),
                Status = "Closed"
            },
            new LearningTask
            {
                Id = "0906992e-ded5-4f40-9cbf-b05767b52ba2",
                Title = "Task-2",
                Description = "This is description of task-2.",
                ExpectedTechStack = "C#, .NET",
                DueDate = new DateOnly(2026, 6, 15),
                Status = "Published"
            });
            context.SaveChanges();
        }

        if (!context.Assignments.Any())
        {
            context.Assignments.AddRange(new Assignment
            {
                Id = "036d2863-ff85-4861-8843-aa9f150bedec",
                TraineeId = "208b07fc-0511-4387-8f57-80f332381a4a",
                MentorId = "2fdc755a-5118-4ed8-9e21-300133d7c088",
                LearningTaskId = "036d2863-ff85-4861-8843-aa9f150bedec",
                AssignedDate = new DateOnly(2026, 7, 6),
                DueDate = new DateOnly(2026, 8, 9),
                Remarks = "This is remark of this assignment.",
                Status = "Assigned"
            });
            context.SaveChanges();
        }

        if (!context.Submissions.Any())
        {
            context.Submissions.AddRange(new Submission
            {
                Id = "48a044de-9e2e-4b66-aef5-2b0287a9d0a7",
                AssignmentId = "036d2863-ff85-4861-8843-aa9f150bedec",
                SubmissionUrl = "https://github.com/deep-govindvira-zeus-learning",
                Notes = "This is note of this submission.",
                Status = "Submitted",
                SubmittedDate = new DateOnly(2026, 9, 9)
            });
            context.SaveChanges();
        }

        if (!context.Reviews.Any())
        {
            context.Reviews.AddRange(new Review
            {
                Id = "48a0lde-9e2e-90sd-aef5-2b0287a9d0a7",
                SubmissionId = "48a044de-9e2e-4b66-aef5-2b0287a9d0a7",
                MentorId = "2fdc755a-5118-4ed8-9e21-300133d7c088",
                Feedback = "This is feedback of this review.",
                Score = 7,
                ReviewStatus = "Accepted",
                ReviewedDate = DateOnly.FromDateTime(DateTime.UtcNow)
            });
            context.SaveChanges();
        }
    }
}
