namespace Whisprr.Api.Models.DTOs.Auth;

/// <summary>
/// Response containing authentication tokens and user info.
/// </summary>
public class AuthResponse
{
    public Guid UserId { get; set; }
    public required string Email { get; set; }
    public string? DisplayName { get; set; }
    public required string Token { get; set; }
    public required string RefreshToken { get; set; }
}
