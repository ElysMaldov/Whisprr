using Whisprr.BlueskyService.Models.Domain;
using Whisprr.SocialScouter.Models;

namespace Whisprr.SocialScouter.Modules.SocialListener.Bluesky;

/// <summary>
/// Extension methods for converting Bluesky domain models to SocialInfo entities.
/// </summary>
internal static class BlueskyPostExtensions
{
    /// <summary>
    /// Converts a BlueskyPost to a SocialInfo entity.
    /// </summary>
    internal static SocialInfo ToSocialInfo(this BlueskyPost blueskyPost)
    {
        return new SocialInfo()
        {
            OriginalUrl = blueskyPost.Uri,
            Content = blueskyPost.Record.Text,
            OriginalId = blueskyPost.CId,
            Platform = Contracts.Enums.PlatformType.Bluesky
        };
    }
}
