using System.Collections.Concurrent;

namespace DrawnUi.Maui.Draw;

internal interface IEasyCachingProvider
{
    void Set<T>(string cacheKey, T value, TimeSpan fromSeconds);

    CacheValue<T> Get<T>(string cacheKey);

    bool Exists(string cacheKey);
}

public class SimpleCachingProvider : IEasyCachingProvider
{
    protected ConcurrentDictionary<string, object> Cache { get; } = new();

    public void Set<T>(string cacheKey, T value, TimeSpan fromSeconds)
    {
        Cache.TryAdd(cacheKey, value);
    }

    public CacheValue<T> Get<T>(string cacheKey)
    {
        if (Cache.TryGetValue(cacheKey, out var value))
        {
            return new CacheValue<T>((T)value, true);
        }
        return new CacheValue<T>(default, false);
    }

    public bool Exists(string cacheKey)
    {
        return Cache.ContainsKey(cacheKey);
    }
}