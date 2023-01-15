using NewLife.Caching;

namespace ThingsGateway.Core;

/// <summary>
/// SqlSugar二级缓存（必须是内存缓存）
/// </summary>
public class SqlSugarCache : ICacheService, ISingleton
{
    private static readonly ICache _cache = Cache.Default;

    public void Add<V>(string key, V value)
    {
        _cache.Set(key, value);
    }

    public void Add<V>(string key, V value, int cacheDurationInSeconds)
    {
        _cache.Set(key, value, cacheDurationInSeconds);
    }

    public bool ContainsKey<V>(string key)
    {
        return _cache.ContainsKey(key);
    }

    public V Get<V>(string key)
    {
        return _cache.Get<V>(key);
    }

    public IEnumerable<string> GetAllKey<V>()
    {
        return _cache.Keys;
    }

    public V GetOrCreate<V>(string cacheKey, Func<V> create, int cacheDurationInSeconds = int.MaxValue)
    {
        if (!_cache.TryGetValue<V>(cacheKey, out V value))
        {
            value = create();
            _cache.Set(cacheKey, value, cacheDurationInSeconds);
        }
        return value;
    }

    public void Remove<V>(string key)
    {
        _cache.Remove(key);
    }
}