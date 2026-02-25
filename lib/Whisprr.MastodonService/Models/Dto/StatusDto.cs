using System.Text.Json.Serialization;

namespace Whisprr.MastodonService.Models.Dto;

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
