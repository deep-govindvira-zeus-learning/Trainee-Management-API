using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;
using System.Text.Json;
using TraineeManagementApi.Services;

public class RedisCacheService : ICacheService
{
    private readonly IDistributedCache _cache;
    private readonly ILogger<RedisCacheService> _logger;
    private readonly IConnectionMultiplexer _redis;
    private readonly IDatabase _db;


    public RedisCacheService(IConnectionMultiplexer redis, IDistributedCache cache, ILogger<RedisCacheService> logger)
    {
        _cache = cache;
        _logger = logger;
            _redis = redis;
        _db = _redis.GetDatabase();

    }

    public async Task<T?> GetAsync<T>(string key)
    {
        try
        {
            var cachedData = await _cache.GetStringAsync(key);
            if (string.IsNullOrEmpty(cachedData))
            {
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

    public async Task RemoveByPatternAsync(string pattern)
    {
        var endpoints = _redis.GetEndPoints();
        var keysToDelete = new List<RedisKey>();

        string wildcardPattern = $"{pattern}";

        foreach (var endpoint in endpoints)
        {
            var server = _redis.GetServer(endpoint);

            await foreach (var key in server.KeysAsync(pattern: wildcardPattern))
            {
                keysToDelete.Add(key);
            }
        }

        if (keysToDelete.Count > 0)
        {
            await _db.KeyDeleteAsync(keysToDelete.ToArray());
        }
    }

}
