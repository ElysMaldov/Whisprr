using System.Text.Json.Serialization;

namespace Whisprr.MastodonService.Models.Dto;

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
