
using Whisprr.BlueskyService.Models.Domain;
using Whisprr.Caching.Interfaces;

namespace Whisprr.BlueskyService.Modules.BlueskySessionStore;

public class BlueskySessionStore(ICaching cache) : IBlueskySessionStore
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