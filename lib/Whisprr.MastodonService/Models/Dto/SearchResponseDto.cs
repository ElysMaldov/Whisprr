using System.Text.Json;
using System.Text.Json.Serialization;

namespace Whisprr.MastodonService.Models.Dto;

/// <summary>
/// DTO representing the search response from Mastodon API.
/// </summary>
public sealed record SearchResponseDto
{
    /// <summary>
    /// Accounts which match the given query.
    /// </summary>
    [JsonPropertyName("accounts")]
    public List<AccountDto> Accounts { get; init; } = [];

    /// <summary>
    /// Statuses which match the given query.
    /// </summary>
    [JsonPropertyName("statuses")]
    public List<StatusDto> Statuses { get; init; } = [];

    /// <summary>
    /// Hashtags which match the given query.
    /// </summary>
    [JsonPropertyName("hashtags")]
    public List<HashtagDto> Hashtags { get; init; } = [];

    public static SearchResponseDto FromJson(string json)
    {
        return JsonSerializer.Deserialize<SearchResponseDto>(json, MastodonDtoContext.Default.SearchResponseDto)
            ?? throw new InvalidOperationException("Failed to deserialize search response");
    }
}
