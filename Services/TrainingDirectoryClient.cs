using System.Net;
using System.Net.Http.Json;

public record TraineeProfileResponse(string Id, string Name, string ProcessingTier);

public interface ITrainingDirectoryClient
{
    Task<TraineeProfileResponse?> GetProfileAsync(string traineeId, CancellationToken cancellationToken);
}

public class TrainingDirectoryClient : ITrainingDirectoryClient
{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<TrainingDirectoryClient> _logger;

    public TrainingDirectoryClient(
        HttpClient httpClient, 
        IHttpContextAccessor httpContextAccessor,
        ILogger<TrainingDirectoryClient> logger)
    {
        _httpClient = httpClient;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public async Task<TraineeProfileResponse?> GetProfileAsync(string traineeId, CancellationToken cancellationToken)
    {
        // Propagate Correlation ID from incoming request context
        var correlationId = _httpContextAccessor.HttpContext?.Request.Headers["X-Correlation-ID"].ToString() 
                            ?? Guid.NewGuid().ToString();
        
        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/trainees/{traineeId}");
        request.Headers.Add("X-Correlation-ID", correlationId);

        try 
        {
            var response = await _httpClient.SendAsync(request, cancellationToken);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null; // Graceful handoff for known 404 resource states
            }

            // Throw exception for 5xx/408 status codes so resilience handlers can catch them
            response.EnsureSuccessStatusCode(); 

            return await response.Content.ReadFromJsonAsync<TraineeProfileResponse>(cancellationToken: cancellationToken);
        }
        catch (HttpRequestException ex) when (ex.StatusCode is HttpStatusCode.BadRequest or HttpStatusCode.Unauthorized)
        {
            _logger.LogError(ex, "Non-transient error occurred. Skipping retries.");
            throw; // Let application-level middleware handle bad requests or auth failures
        }
        catch (Exception ex)
        {
            _logger.LogWarning($"Request failed or timed out. Falling back to degraded state. Error: {ex.Message}");
            return GetFallbackProfile(traineeId);
        }
    }

    // Task 3.19: Fallback Behaviour
    private TraineeProfileResponse GetFallbackProfile(string traineeId)
    {
        return new TraineeProfileResponse(traineeId, "Unknown Trainee", "Degraded/Cached Mode");
    }
}
