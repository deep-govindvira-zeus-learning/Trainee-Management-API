using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TraineeManagementApi.Data;
using TraineeManagementApi.Models;
using TraineeManagementApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddValidation();
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseInMemoryDatabase("TraineeManagementDb");
});


builder.Services.AddScoped<ITraineeService, TraineeService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    // Ensure the in-memory store is fully initiated
    context.Database.EnsureCreated();

    // Check if data already exists to avoid redundant duplication during restarts
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
                LastName = "Dhamelia",
                Email = "divyang.dhamelia@zeuslearning.com",
                TechStack = "React, Node",
                Status = "Completed",
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow
            }
        );
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

// app.MapGet("/api/health", () =>
// {
//     return Results.Ok(new
//     {
//         status = "running",
//         application = "Trainee Management API",
//         timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss")
//     });
// });

app.Run();
