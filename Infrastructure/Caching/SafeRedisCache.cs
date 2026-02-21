using Microsoft.Extensions.Caching.Distributed;

public class SafeRedisCache
{
    private readonly IDistributedCache _cache;
    private readonly ILogger<SafeRedisCache> _logger;

    public SafeRedisCache(
        IDistributedCache cache,
        ILogger<SafeRedisCache> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public async Task<string?> GetAsync(string key)
    {
        try
        {
            return await _cache.GetStringAsync(key);
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Redis GET failed. Skipping cache.");
            return null;
        }
    }

    public async Task SetAsync(string key, string value, DistributedCacheEntryOptions options)
    {
        try
        {
            await _cache.SetStringAsync(key, value, options);
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Redis SET failed. Skipping cache.");
        }
    }

    public async Task RemoveAsync(string key)
    {
        try
        {
            await _cache.RemoveAsync(key);
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Redis REMOVE failed. Skipping cache.");
        }
    }
}
