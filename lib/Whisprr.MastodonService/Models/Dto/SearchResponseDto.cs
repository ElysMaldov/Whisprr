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
        return JsonSerializer.Deserialize<SearchResponseDto>(json, JsonContext.Default.SearchResponseDto)
            ?? throw new InvalidOperationException("Failed to deserialize search response");
    }
}

/// <summary>
/// DTO representing a status in the search response.
/// </summary>
public sealed record StatusDto
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = string.Empty;

    [JsonPropertyName("created_at")]
    public DateTimeOffset CreatedAt { get; init; }

    [JsonPropertyName("content")]
    public string Content { get; init; } = string.Empty;

    [JsonPropertyName("url")]
    public string? Url { get; init; }

    [JsonPropertyName("uri")]
    public string Uri { get; init; } = string.Empty;

    [JsonPropertyName("account")]
    public AccountDto Account { get; init; } = new();

    [JsonPropertyName("visibility")]
    public string Visibility { get; init; } = "public";

    [JsonPropertyName("sensitive")]
    public bool Sensitive { get; init; }

    [JsonPropertyName("spoiler_text")]
    public string SpoilerText { get; init; } = string.Empty;
}

/// <summary>
/// DTO representing an account in the search response.
/// </summary>
public sealed record AccountDto
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = string.Empty;

    [JsonPropertyName("username")]
    public string Username { get; init; } = string.Empty;

    [JsonPropertyName("acct")]
    public string Acct { get; init; } = string.Empty;

    [JsonPropertyName("display_name")]
    public string DisplayName { get; init; } = string.Empty;

    [JsonPropertyName("url")]
    public string Url { get; init; } = string.Empty;

    [JsonPropertyName("avatar")]
    public string Avatar { get; init; } = string.Empty;

    [JsonPropertyName("note")]
    public string Note { get; init; } = string.Empty;
}

/// <summary>
/// DTO representing a hashtag in the search response.
/// </summary>
public sealed record HashtagDto
{
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    [JsonPropertyName("url")]
    public string Url { get; init; } = string.Empty;
}

/// <summary>
/// JSON serialization context for Mastodon DTOs.
/// </summary>
[JsonSerializable(typeof(SearchResponseDto))]
[JsonSerializable(typeof(StatusDto))]
[JsonSerializable(typeof(AccountDto))]
[JsonSerializable(typeof(HashtagDto))]
internal partial class JsonContext : JsonSerializerContext
{
}
