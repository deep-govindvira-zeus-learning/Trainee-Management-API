using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using TraineeManagementApi.Services;

public class RedisCacheService : ICacheService
{
    private readonly IDistributedCache _cache;
    private readonly ILogger<RedisCacheService> _logger;

    public RedisCacheService(IDistributedCache cache, ILogger<RedisCacheService> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        try
        {
            var cachedData = await _cache.GetStringAsync(key);
            if (string.IsNullOrEmpty(cachedData)) {
                _logger.LogInformation("Cache MISS for key: {Key}", key);
                return default;
            }

            _logger.LogInformation("Cache HIT for key: {Key}", key);
            return JsonSerializer.Deserialize<T>(cachedData);
        }
        catch (Exception ex)
        {
            // Fail-safe: Log error, do not crash API, force MySQL fallback
            _logger.LogError(ex, "Redis connection failed during GET for key: {Key}", key);
            return default;
        }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan expiration)
    {
        try
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration
            };
            var serializedData = JsonSerializer.Serialize(value);
            await _cache.SetStringAsync(key, serializedData, options);
            _logger.LogInformation("Cache SET for key: {Key} with expiration: {Expiration}", key, expiration);
        }
        catch (Exception ex)
        {
            // Fail-safe: Log error and move on
            _logger.LogError(ex, "Redis connection failed during SET for key: {Key}", key);
        }
    }

    public async Task RemoveAsync(string key)
    {
        try
        {
            await _cache.RemoveAsync(key);
            _logger.LogInformation("Cache REMOVE for key: {Key}", key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Redis connection failed during REMOVE for key: {Key}", key);
        }
    }
}
