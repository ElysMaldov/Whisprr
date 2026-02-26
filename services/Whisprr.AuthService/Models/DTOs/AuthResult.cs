namespace Whisprr.AuthService.Models.DTOs;

/// <summary>
/// Result of token validation.
/// </summary>
public class AuthResult
{
    public bool IsValid { get; set; }
    public Guid? UserId { get; set; }
    public string? Email { get; set; }
    public string? ErrorMessage { get; set; }
}
