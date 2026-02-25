using Whisprr.MastodonService.Models.Domain;
using Whisprr.MastodonService.Models.Dto;

namespace Whisprr.MastodonService.Modules.MastodonService;

/// <summary>
/// Extension methods for mapping Mastodon DTOs to domain models.
/// </summary>
internal static class SearchResponseMapper
{
    /// <summary>
    /// Converts the search response DTO to a domain model.
    /// </summary>
    public static SearchStatusesResponse ToDomain(this SearchResponseDto dto)
    {
        return new SearchStatusesResponse
        {
            Statuses = dto.Statuses.Select(s => s.ToDomain()).ToList()
        };
    }

    /// <summary>
    /// Converts a status DTO to a domain model.
    /// </summary>
    private static MastodonStatus ToDomain(this StatusDto dto)
    {
        return new MastodonStatus
        {
            Id = dto.Id,
            CreatedAt = dto.CreatedAt,
            Content = dto.Content,
            Url = dto.Url ?? dto.Uri,
            Uri = dto.Uri,
            Account = dto.Account.ToDomain(),
            Visibility = dto.Visibility,
            Sensitive = dto.Sensitive,
            SpoilerText = dto.SpoilerText
        };
    }

    /// <summary>
    /// Converts an account DTO to a domain model.
    /// </summary>
    private static MastodonAccount ToDomain(this AccountDto dto)
    {
        return new MastodonAccount
        {
            Id = dto.Id,
            Username = dto.Username,
            Acct = dto.Acct,
            DisplayName = dto.DisplayName,
            Url = dto.Url,
            Avatar = dto.Avatar,
            Note = dto.Note
        };
    }
}
