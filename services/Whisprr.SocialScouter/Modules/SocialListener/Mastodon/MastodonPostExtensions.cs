using System.Text.RegularExpressions;
using Whisprr.MastodonService.Models.Domain;
using Whisprr.SocialScouter.Models;

namespace Whisprr.SocialScouter.Modules.SocialListener.Mastodon;

/// <summary>
/// Extension methods for converting Mastodon domain models to SocialInfo entities.
/// </summary>
internal static class MastodonPostExtensions
{
    /// <summary>
    /// Converts a MastodonStatus to a SocialInfo entity.
    /// </summary>
    internal static SocialInfo ToSocialInfo(this MastodonStatus mastodonStatus)
    {
        // Strip HTML tags from the content for cleaner text
        var plainText = StripHtml(mastodonStatus.Content);

        return new SocialInfo()
        {
            OriginalUrl = mastodonStatus.Url,
            Content = plainText,
            OriginalId = mastodonStatus.Id,
            Platform = Contracts.Enums.PlatformType.Mastodon,
            CreatedAt = mastodonStatus.CreatedAt,
            Title = mastodonStatus.SpoilerText switch
            {
                { Length: > 0 } spoiler => spoiler,
                _ => null
            }
        };
    }

    /// <summary>
    /// Removes HTML tags from the given HTML string.
    /// </summary>
    private static string StripHtml(string html)
    {
        if (string.IsNullOrWhiteSpace(html))
            return string.Empty;

        // First, replace <br>, <p> tags with newlines for better readability
        var withNewlines = html
            .Replace("<br>", "\n")
            .Replace("<br/>", "\n")
            .Replace("<br />", "\n")
            .Replace("</p>", "\n")
            .Replace("<p>", "");

        // Then strip all remaining HTML tags
        var stripped = Regex.Replace(withNewlines, "<[^>]+>", string.Empty);

        // Decode HTML entities
        var decoded = System.Net.WebUtility.HtmlDecode(stripped);

        // Trim and normalize whitespace
        var normalized = Regex.Replace(decoded, @"\s+", " ").Trim();

        return normalized;
    }
}
