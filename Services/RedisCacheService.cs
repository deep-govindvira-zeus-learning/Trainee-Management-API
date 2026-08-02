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

    // Same config key + default as the InstanceName passed to AddStackExchangeRedisCache in
    // Program.cs, so raw KEYS-pattern scans stay in sync with the prefix IDistributedCache
    // applies under the hood, instead of duplicating "TrainingPlatform_" as a magic string.
    private readonly string _instanceName;

    public RedisCacheService(IConnectionMultiplexer redis, IDistributedCache cache, ILogger<RedisCacheService> logger, IConfiguration configuration)
    {
        _cache = cache;
        _logger = logger;
            _redis = redis;
        _db = _redis.GetDatabase();
        _instanceName = configuration["Redis:InstanceName"] ?? "TrainingPlatform_";

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

        // IDistributedCache stores keys under the configured InstanceName prefix, but this
        // method talks to Redis directly, so the prefix has to be applied here explicitly.
        string wildcardPattern = $"{_instanceName}{pattern}";

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
