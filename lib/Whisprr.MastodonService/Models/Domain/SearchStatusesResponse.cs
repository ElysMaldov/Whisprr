namespace Whisprr.MastodonService.Models.Domain;

/// <summary>
/// Represents the results of a search for statuses.
/// </summary>
public sealed record SearchStatusesResponse
{
    /// <summary>
    /// Statuses which match the given query.
    /// </summary>
    public required IReadOnlyList<MastodonStatus> Statuses { get; init; }
}
