using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace RedisExample.Services;

public class RedisCacheService : IRedisCacheService
{
    private readonly IDistributedCache? _cache;
    public RedisCacheService(IDistributedCache? cache)
    {
        _cache = cache;
    }
    
    /*public T? Get<T>(string key) => _cache.Get<T>(key);
    public void Set<T>(string key, T value) => _cache.Set(key, value);*/

    public T GetCachedData<T>(string key)
    {
        var jsonData = _cache.GetString(key);

        if (jsonData is null)
            return default(T);

        return JsonSerializer.Deserialize<T>(jsonData)!;
    }
    
    public void SetCachedData<T>(string key, T data, TimeSpan chacheDuration)
    {
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = chacheDuration
        };
        
        var jsonData = JsonSerializer.Serialize(data);
        _cache.SetString(key, jsonData, options);
    }
}