using System.ComponentModel.DataAnnotations;

namespace Whisprr.AuthService.Models.DTOs;

/// <summary>
/// Request to refresh an access token using a refresh token.
/// </summary>
public class RefreshTokenRequest
{
    [Required]
    public required string RefreshToken { get; set; }
}
