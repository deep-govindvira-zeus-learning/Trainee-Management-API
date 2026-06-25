public record TraineeProfileResponse(Guid TraineeId, string Name, string Status, string Tier);

public class TrainingDirectoryClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<TrainingDirectoryClient> _logger;

    public TrainingDirectoryClient(HttpClient httpClient, ILogger<TrainingDirectoryClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<TraineeProfileResponse?> GetProfileAsync(Guid traineeId, CancellationToken cancellationToken)
    {
        try
        {
            // The correlation ID header is attached automatically via a delegating handler (Step 3)
            var response = await _httpClient.GetAsync($"/api/trainees/{traineeId}", cancellationToken);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }

            // Explicitly ensure success status or throw for resilience tracking
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<TraineeProfileResponse>(cancellationToken: cancellationToken);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP request failed for trainee profile {TraineeId}", traineeId);
            throw; 
        }
    }
}
