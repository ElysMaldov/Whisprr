namespace Whisprr.MastodonService.Models.Domain;

/// <summary>
/// Represents a user account on Mastodon.
/// </summary>
public sealed record MastodonAccount
{
    /// <summary>
    /// The account id.
    /// </summary>
    public required string Id { get; init; }

    /// <summary>
    /// The username of the account, not including domain.
    /// </summary>
    public required string Username { get; init; }

    /// <summary>
    /// The WebFinger account URI. Equal to username for local users, or username@domain for remote users.
    /// </summary>
    public required string Acct { get; init; }

    /// <summary>
    /// The profile's display name.
    /// </summary>
    public required string DisplayName { get; init; }

    /// <summary>
    /// The location of the user's profile page (web interface URL).
    /// </summary>
    public required string Url { get; init; }

    /// <summary>
    /// An image icon that is shown next to statuses and in the profile.
    /// </summary>
    public required string Avatar { get; init; }

    /// <summary>
    /// The profile's bio or description.
    /// </summary>
    public required string Note { get; init; }
}
