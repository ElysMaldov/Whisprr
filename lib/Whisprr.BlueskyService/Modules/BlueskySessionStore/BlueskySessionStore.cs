
using Whisprr.BlueskyService.Models.Domain;
using Whisprr.Caching.Modules;

namespace Whisprr.BlueskyService.Modules.BlueskySessionStore;

public class BlueskySessionStore(CachingService cache) : IBlueskySessionStore
{
  private string _sessionKey = "bluesky-session";

  public async Task SaveSessionAsync(BlueskySession session)
  {
    await cache.SetAsync(_sessionKey, session);
  }

  public async Task<BlueskySession?> GetSessionAsync()
  {
    var session = await cache.GetAsync<BlueskySession>(_sessionKey);

    return session;
  }
}