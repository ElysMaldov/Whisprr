using MassTransit;
using Whisprr.BlueskyService.Modules.BlueskyService;
using Whisprr.Contracts.Enums;
using Whisprr.SocialScouter.Models;

namespace Whisprr.SocialScouter.Modules.SocialListener.Bluesky;

/// <summary>
/// Bluesky-specific implementation of the social listener.
/// Searches Bluesky posts using keywords from the listening task.
/// </summary>
internal class BlueskySocialListener(
    ILogger<BlueskySocialListener> logger,
    IBus bus,
    IBlueskyService blueskyService) : SocialListener<BlueskySocialListener>(logger, bus)
{
  /// <summary>
  /// This listener handles Bluesky platform tasks.
  /// </summary>
  public override PlatformType SupportedPlatform => PlatformType.Bluesky;

  protected override async Task<SocialInfo[]> PerformSearch(SocialListeningTask task)
  {
    var query = task.Query;

    var blueskyPosts = (await blueskyService.SearchPosts(q: query)).Posts;
    SocialInfo[] mappedPosts = blueskyPosts.Select(p => p.ToSocialInfo()).ToArray();

    return mappedPosts;
  }
}
