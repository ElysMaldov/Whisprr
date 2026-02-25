using System.Text.Json;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using Whisprr.Caching.Modules;

namespace Whisprr.SocialScouter.Modules.Caching;

public class RedisCachingService(IDatabase database, IOptions<RedisOptions> options) : CachingService
{
  public override required string Prefix { get; init; } = options.Value.KeyPrefix;

  public override async Task<bool> SetAsync<T>(string key, T value, TimeSpan? expiry)
  {
    var cacheKey = BuildCacheKey(key);
    var json = JsonSerializer.Serialize(value);
    return await database.StringSetAsync(cacheKey, json, expiry ?? default);
  }

  public override async Task<T?> GetAsync<T>(string key) where T : default
  {
    var cacheKey = BuildCacheKey(key);
    var json = await database.StringGetAsync(cacheKey);

    if (json.IsNullOrEmpty)
    {
      return default;
    }

    // TODO if performance gains is needed, at generators for known models and reflection as fallback for unknown models
    return JsonSerializer.Deserialize<T>((string)json!);
  }
}
