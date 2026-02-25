using Microsoft.AspNetCore.WebUtilities;
using Whisprr.MastodonService.Models.Domain;
using Whisprr.MastodonService.Models.Dto;

namespace Whisprr.MastodonService.Modules.MastodonService;

/// <summary>
/// Implementation of the Mastodon API service.
/// No authentication is required for public search operations.
/// </summary>
/// <param name="httpClient">The HTTP client configured with the Mastodon instance base URL.</param>
public sealed class MastodonService(HttpClient httpClient) : IMastodonService
{
    public async Task<SearchStatusesResponse> SearchStatuses(
        string query,
        int? limit = null,
        int? offset = null,
        string? minId = null,
        string? maxId = null
    )
    {
        const string endpoint = "/api/v2/search";

        Dictionary<string, string?> queryParams = new()
        {
            ["q"] = query,
            ["type"] = "statuses",
            ["limit"] = limit?.ToString(),
            ["offset"] = offset?.ToString(),
            ["min_id"] = minId,
            ["max_id"] = maxId
        };

        var uri = QueryHelpers.AddQueryString(endpoint, queryParams);

        var response = await httpClient.GetAsync(uri);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var dto = SearchResponseDto.FromJson(json);

        return dto.ToDomain();
    }
}
