using Microsoft.Extensions.Options;
using StackExchange.Redis;
using Whisprr.Caching.Modules;

namespace Whisprr.SocialScouter.Modules.Caching;

public class RedisCachingService(IDatabase database, IOptions<RedisOptions> options) : CachingService
{
  public override Task<bool> SetAsync<T>(string key, T value, TimeSpan? expiry = null)
  {
    throw new NotImplementedException();
  }

  public override Task<T?> GetAsync<T>(string key) where T : default
  {
    throw new NotImplementedException();
  }

}