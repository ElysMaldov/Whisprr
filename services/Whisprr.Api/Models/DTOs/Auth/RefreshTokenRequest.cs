using System.ComponentModel.DataAnnotations;

namespace Whisprr.Api.Models.DTOs.Auth;

/// <summary>
/// Request to refresh an access token using a refresh token.
/// </summary>
public class RefreshTokenRequest
{
    [Required]
    public required string RefreshToken { get; set; }
}
