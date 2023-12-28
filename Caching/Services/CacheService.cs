using StackExchange.Redis;

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
        throw new NotImplementedException();
    }

    public bool SetData<T>(string key, T value, DateTimeOffset expirationTime)
    {
        throw new NotImplementedException();
    }

    public object RemoveData(string key)
    {
        throw new NotImplementedException();
    }
}
