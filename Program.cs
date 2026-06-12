using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using TraineeManagementApi.Data;
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
                Id = Guid.NewGuid().ToString(),
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
                Id = Guid.NewGuid().ToString(),
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
                Id = Guid.NewGuid().ToString(),
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
                Id = Guid.NewGuid().ToString(),
                FirstName = "Kedar",
                LastName = "Kale",
                Email = "kedar.kale@zeuslearning.com",
                Expertise = "C#, .NET, MERN, Angular, AWS",
                Status = "Active",
            },
            new Mentor
            {
                Id = Guid.NewGuid().ToString(),
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


// app.UseExceptionHandler(); 

app.UseRouting();

app.UseCors(ReactCorsPolicy);

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
