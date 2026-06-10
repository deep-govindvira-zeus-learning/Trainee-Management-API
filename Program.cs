using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TraineeManagementApi.Configurations;
using TraineeManagementApi.Data;
using TraineeManagementApi.Models;
using TraineeManagementApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddValidation();
// builder.Services.AddDbContext<AppDbContext>(options =>
// {
//     options.UseInMemoryDatabase("TraineeManagementDb");
// });

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));

// db connection
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
);


builder.Services.AddScoped<ITraineeService, TraineeService>();
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
}

using (var scope = app.Services.CreateScope())
{
    var context =
        scope.ServiceProvider.GetRequiredService<AppDbContext>();

    if (!context.Users.Any())
    {
        context.Users.AddRange(new User
        {
            Id = Guid.NewGuid().ToString(),
            Username = "admin",
            Email = "admin@test.com",
            PasswordHash =
               BCrypt.Net.BCrypt.HashPassword("Admin@123"),
            Role = "Admin",
            CreatedDate = DateTime.UtcNow,
            UpdatedDate = DateTime.UtcNow
        },
        new User
        {
            Id = Guid.NewGuid().ToString(),
            Username = "kedar",
            Email = "kedar@test.com",
            PasswordHash =
               BCrypt.Net.BCrypt.HashPassword("kedar@123"),
            Role = "Mentor",
            CreatedDate = DateTime.UtcNow,
            UpdatedDate = DateTime.UtcNow
        },
       new User
       {
           Id = Guid.NewGuid().ToString(),
           Username = "deep",
           Email = "deep@test.com",
           PasswordHash =
               BCrypt.Net.BCrypt.HashPassword("deep@123"),
           Role = "Trainee",
           CreatedDate = DateTime.UtcNow,
           UpdatedDate = DateTime.UtcNow
       });

        // context.Users.Add(new User
        // {
        //     Id = Guid.NewGuid().ToString(),
        //     Username = "admin",
        //     Email = "admin@test.com",
        //     PasswordHash =
        //         BCrypt.Net.BCrypt.HashPassword("Admin@123"),
        //     Role = "Admin",
        //     CreatedDate = DateTime.UtcNow,
        //     UpdatedDate = DateTime.UtcNow
        // });

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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
