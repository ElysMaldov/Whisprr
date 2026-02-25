namespace Whisprr.MastodonService.Models.Domain;

/// <summary>
/// Represents a status (post/toot) on Mastodon.
/// </summary>
public sealed record MastodonStatus
{
    /// <summary>
    /// ID of the status in the database.
    /// </summary>
    public required string Id { get; init; }

    /// <summary>
    /// The date when this status was created.
    /// </summary>
    public required DateTimeOffset CreatedAt { get; init; }

    /// <summary>
    /// HTML-encoded status content.
    /// </summary>
    public required string Content { get; init; }

    /// <summary>
    /// A link to the status's HTML representation.
    /// </summary>
    public required string Url { get; init; }

    /// <summary>
    /// URI of the status used for federation.
    /// </summary>
    public required string Uri { get; init; }

    /// <summary>
    /// The account that authored this status.
    /// </summary>
    public required MastodonAccount Account { get; init; }

    /// <summary>
    /// Visibility of this status (public, unlisted, private, direct).
    /// </summary>
    public required string Visibility { get; init; }

    /// <summary>
    /// Is this status marked as sensitive content?
    /// </summary>
    public required bool Sensitive { get; init; }

    /// <summary>
    /// Subject or summary line, below which status content is collapsed until expanded.
    /// </summary>
    public required string SpoilerText { get; init; }
}
