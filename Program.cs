using System.Text;
using System.Text.Json.Serialization;
using Core.Interfaces;
using Infrastructure.Storage;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using TraineeManagementApi.Data;
using TraineeManagementApi.Helper;
using TraineeManagementApi.Middleware;
using TraineeManagementApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // This converts all enums to strings automatically in your JSON API
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetValue<string>("Redis:ConnectionString");
    options.InstanceName = "TrainingPlatform_";
});


builder.Services.AddSerilog((services, lc) => lc.ReadFrom.Configuration(builder.Configuration));

var logPath = Path.Combine(Directory.GetCurrentDirectory(), "Logs/app_logs.txt");
builder.Logging.AddProvider(new CustomFileLoggerProvider(logPath));

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
builder.Services.AddScoped<ISubmissionService, SubmissionService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<ISubmissionFileService, SubmissionFileService>();
builder.Services.AddScoped<IFileStorageService, LocalFileStorageService>();
builder.Services.AddSingleton<ICacheService, RedisCacheService>();
builder.Services.AddScoped<ISubmissionPublisher, RabbitMqSubmissionPublisher>(); 
builder.Services.AddScoped<IProcessingJobService, ProcessingJobService>();


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

    DbInitializer.Seed(context);
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


// This intercepts and logs every HTTP request to the console automatically
app.UseSerilogRequestLogging(); 

app.UseExceptionHandler();

app.UseRouting();

app.UseCors(ReactCorsPolicy);

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
