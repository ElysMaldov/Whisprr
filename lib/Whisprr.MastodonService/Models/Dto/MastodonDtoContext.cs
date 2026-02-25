using System.Text.Json.Serialization;

namespace Whisprr.MastodonService.Models.Dto;

/// <summary>
/// JSON serialization context for Mastodon DTOs.
/// </summary>
[JsonSerializable(typeof(SearchResponseDto))]
[JsonSerializable(typeof(StatusDto))]
[JsonSerializable(typeof(AccountDto))]
[JsonSerializable(typeof(HashtagDto))]
internal partial class MastodonDtoContext : JsonSerializerContext
{
}
