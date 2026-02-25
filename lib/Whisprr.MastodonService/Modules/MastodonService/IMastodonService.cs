using Whisprr.MastodonService.Models.Domain;

namespace Whisprr.MastodonService.Modules.MastodonService;

/// <summary>
/// Service for interacting with the Mastodon API.
/// </summary>
public interface IMastodonService
{
    /// <summary>
    /// Searches for statuses (posts/toots) matching the given query.
    /// </summary>
    /// <param name="query">The search query.</param>
    /// <param name="limit">Maximum number of results to return (default 20, max 40).</param>
    /// <param name="offset">Skip the first n results.</param>
    /// <param name="minId">Returns results immediately newer than this ID.</param>
    /// <param name="maxId">All results returned will be lesser than this ID.</param>
    /// <returns>A response containing the matching statuses.</returns>
    Task<SearchStatusesResponse> SearchStatuses(
        string query,
        int? limit = null,
        int? offset = null,
        string? minId = null,
        string? maxId = null
    );
}
