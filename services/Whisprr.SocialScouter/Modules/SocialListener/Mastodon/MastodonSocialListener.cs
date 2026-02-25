using MassTransit;
using Whisprr.Contracts.Enums;
using Whisprr.MastodonService.Modules.MastodonService;
using Whisprr.SocialScouter.Models;

namespace Whisprr.SocialScouter.Modules.SocialListener.Mastodon;

/// <summary>
/// Mastodon-specific implementation of the social listener.
/// Searches Mastodon posts using keywords from the listening task.
/// </summary>
internal class MastodonSocialListener(
    ILogger<MastodonSocialListener> logger,
    IBus bus,
    IMastodonService mastodonService) : SocialListener<MastodonSocialListener>(logger, bus)
{
    /// <summary>
    /// This listener handles Mastodon platform tasks.
    /// </summary>
    public override PlatformType SupportedPlatform => PlatformType.Mastodon;

    protected override async Task<SocialInfo[]> PerformSearch(SocialListeningTask task)
    {
        var query = task.Query;

        var searchResponse = await mastodonService.SearchStatuses(
            query: query,
            limit: 40  // Maximum allowed by Mastodon API
        );

        SocialInfo[] mappedPosts = searchResponse.Statuses
            .Select(s => s.ToSocialInfo())
            .ToArray();

        return mappedPosts;
    }
}
