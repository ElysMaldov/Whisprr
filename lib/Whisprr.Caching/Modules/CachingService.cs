namespace Whisprr.Caching.Modules;

public abstract class CachingService
{
  public required string Prefix { get; init; }
  protected virtual string BuildCacheKey(string key) => $"{Prefix}:{key}";
  public abstract Task<bool> SetAsync<T>(string key, T value, TimeSpan? expiry = null);
  public abstract Task<T?> GetAsync<T>(string key);
}

