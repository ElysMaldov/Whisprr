using System.Text.Json.Serialization;

namespace Whisprr.MastodonService.Models.Dto;

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
