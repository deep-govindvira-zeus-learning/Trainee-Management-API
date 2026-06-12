using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TraineeManagementApi.Data;
using TraineeManagementApi.Middleware;
using TraineeManagementApi.Models;
using TraineeManagementApi.Services;

var builder = WebApplication.CreateBuilder(args);

const string ReactCorsPolicy = "_reactDevelopmentCors";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: ReactCorsPolicy,
        policy =>
        {
            policy
                .WithOrigins("http://localhost:3000", "http://localhost:5173")
                // .AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});


// Add services to the container.

builder.Services.AddControllers();

// Register Problem Details and the Global Exception Handler
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddValidation();

// builder.Services.AddDbContext<AppDbContext>(options =>
// {
//     options.UseInMemoryDatabase("TraineeManagementDb");
// });

// db connection
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
);


builder.Services.AddScoped<ITraineeService, TraineeService>();
builder.Services.AddScoped<IMentorService, MentorService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ILearningTaskService, LearningTaskService>();
builder.Services.AddScoped<IAssignmentService, AssignmentService>();

builder.Services.AddControllers(options =>
{
    options.SuppressAsyncSuffixInActionNames = false;
});


var jwt = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwt["key"]!);

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
        new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwt["Issuer"],
            ValidAudience = jwt["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    context.Database.EnsureCreated();

    if (!context.Trainees.Any())
    {
        context.Trainees.AddRange(
            new Trainee
            {
                Id = "208b07fc-0511-4387-8f57-80f332381a4a",
                FirstName = "Deep",
                LastName = "Govindvira",
                Email = "deep.govindvira@zeuslearning.com",
                TechStack = "C#, .NET",
                Status = "Active",
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow
            },
            new Trainee
            {
                Id = "3631c4b3-c824-4374-a1ef-204237fccfac",
                FirstName = "Yash",
                LastName = "Gokulgandhi",
                Email = "yash.gokulgandhi@zeuslearning.com",
                TechStack = "Java, Spring Boot",
                Status = "InActive",
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow
            },
            new Trainee
            {
                Id = "522d1097-e6e4-4b91-807f-ca8358fd618e",
                FirstName = "Divyang",
                LastName = "Dhameliya",
                Email = "divyang.dhameliya@zeuslearning.com",
                TechStack = "React, Node",
                Status = "Completed",
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow
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
                FirstName = "Kedar",
                LastName = "Kale",
                Email = "kedar.kale@zeuslearning.com",
                Expertise = "C#, .NET, MERN, Angular, AWS",
                Status = "Active",
            },
            new Mentor
            {
                Id = "8f9241fe-9464-4d57-97d8-6964c67ff98d",
                FirstName = "Tanuj",
                LastName = "Kude",
                Email = "tanuj.kude@zeuslearning.com",
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
            Role = "Admin",
            CreatedDate = DateTime.UtcNow,
            UpdatedDate = DateTime.UtcNow
        });
        context.SaveChanges();
    }

    if (!context.LearningTasks.Any())
    {
        context.LearningTasks.AddRange(new LearningTask
        {
            Id = "036d2863-ff85-4861-8843-aa9f150bedec",
            Title = "Task Tracker",
            Description = "This task is about HTML, CSS, Javascript.",
            ExpectedTechStack = "HTML, CSS, Javascript",
            DueDate = new DateOnly(2026, 6, 7),
            Status = "Closed"
        },
        new LearningTask
        {
            Id = "0906992e-ded5-4f40-9cbf-b05767b52ba2",
            Title = "Trainee Management Api - 1",
            Description = "This task is about learning C#, .NET, MVC.",
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
            Remarks = "This is remark.",
            Status = "Assigned"
        });
        context.SaveChanges();
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUi(options =>
    {
        options.DocumentPath = "/openapi/v1.json";
    });

}


app.UseExceptionHandler();

app.UseRouting();

app.UseCors(ReactCorsPolicy);

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
