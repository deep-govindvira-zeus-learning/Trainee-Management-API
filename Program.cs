using System.Text;
using System.Text.Json.Serialization;
using Core.Interfaces;
using Infrastructure.Storage;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Polly;
using Serilog;
using TraineeManagementApi.Data;
using TraineeManagementApi.Helper;
using TraineeManagementApi.Middleware;
using TraineeManagementApi.Services;

var builder = WebApplication.CreateBuilder(args);

// --- Controller & JSON Setup ---
builder.Services.AddControllers(options =>
{
    options.SuppressAsyncSuffixInActionNames = false;
})
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

// --- Caching & Logging ---
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetValue<string>("Redis:ConnectionString");
    options.InstanceName = "TrainingPlatform_";
});

builder.Services.AddSerilog((services, lc) => lc.ReadFrom.Configuration(builder.Configuration));

var logPath = Path.Combine(Directory.GetCurrentDirectory(), "Logs/app_logs.txt");
builder.Logging.AddProvider(new CustomFileLoggerProvider(logPath));

// --- CORS Policy ---
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

// --- Global Exception & Validation Setup ---
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddOpenApi();
builder.Services.AddValidation();

// --- InMemory DB ---
// builder.Services.AddDbContext<AppDbContext>(options =>
// {
//     options.UseInMemoryDatabase("TraineeManagementDb");
// });

// --- Database Setup ---
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
);

// --- Core Application Dependencies ---
builder.Services.AddHttpContextAccessor(); // REQUIRED for header extraction
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

// --- Authentication Setup ---
var jwt = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwt["key"]!);

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
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

// --- Manual Correlation ID Handler Registration ---
builder.Services.AddTransient<CorrelationIdManualPropagationHandler>();

// --- Resilient HTTP Client Setup ---
builder.Services.AddHttpClient<ITrainingDirectoryClient, TrainingDirectoryClient>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5138"); 
    client.DefaultRequestHeaders.Add("Accept", "application/json");
})
.AddHttpMessageHandler<CorrelationIdManualPropagationHandler>() // Force manual forwarder
.AddStandardResilienceHandler(options =>
{
    options.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(15);
    options.AttemptTimeout.Timeout = TimeSpan.FromSeconds(3);
    options.Retry.MaxRetryAttempts = 3;
    options.Retry.BackoffType = DelayBackoffType.Exponential;
    options.Retry.UseJitter = true;
    options.Retry.Delay = TimeSpan.FromSeconds(1);
    options.CircuitBreaker.FailureRatio = 0.5;
    options.CircuitBreaker.SamplingDuration = TimeSpan.FromSeconds(10); 
    options.CircuitBreaker.BreakDuration = TimeSpan.FromSeconds(30);
});

// ========================================================
// HTTP PIPELINE BUILD
// ========================================================
var app = builder.Build();

// Seed Database Context
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    DbInitializer.Seed(context);
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUi(options =>
    {
        options.DocumentPath = "/openapi/v1.json";
    });
}

app.UseSerilogRequestLogging(); 
app.UseExceptionHandler();
app.UseRouting();
app.UseCors(ReactCorsPolicy);
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();