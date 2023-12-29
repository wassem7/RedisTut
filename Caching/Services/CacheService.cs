using Newtonsoft.Json;
using StackExchange.Redis;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Caching.Services;

public class CacheService : ICacheService
{
    private IDatabase _cacheDb;

    public CacheService()
    {
        var redis = ConnectionMultiplexer.Connect("localhost:6379");
        _cacheDb = redis.GetDatabase(2);
    }

    public T GetData<T>(string key)
    {
        var value = _cacheDb.StringGet(key);
        if (!value.IsNullOrEmpty)
        {
            return JsonSerializer.Deserialize<T>(value);
        }

        return default;
    }

    public bool SetData<T>(string key, T value, DateTimeOffset expirationTime)
    {
        var expiry = expirationTime.DateTime.Subtract(DateTime.Now);
        return _cacheDb.StringSet(key, JsonSerializer.Serialize(value), expiry);
    }

    public object RemoveData(string key)
    {
        var keyExists = _cacheDb.KeyExists(key);
        if (keyExists)
        {
            return _cacheDb.KeyDelete(key);
        }

        return false;
    }
}
