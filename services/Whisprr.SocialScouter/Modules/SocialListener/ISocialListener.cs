using Whisprr.Contracts.Enums;
using Whisprr.SocialScouter.Models;

namespace Whisprr.SocialScouter.Modules.SocialListener;

/// <summary>
/// Interface for social media platform listeners.
/// Each implementation handles listening for a specific platform.
/// </summary>
internal interface ISocialListener
{
    /// <summary>
    /// The platform type this listener supports (e.g., Bluesky, Mastodon).
    /// </summary>
    PlatformType SupportedPlatform { get; }

    /// <summary>
    /// Searches the platform for content matching the task query.
    /// </summary>
    /// <param name="task">The listening task containing search criteria.</param>
    /// <returns>An array of social information found.</returns>
    Task<SocialInfo[]> Search(SocialListeningTask task);
}
