namespace RedisExample.Services;

public interface IRedisCacheService
{
    T GetCachedData<T>(string key);
    void SetCachedData<T>(string key, T data, TimeSpan chacheDuration);
}