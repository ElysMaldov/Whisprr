namespace Whisprr.Api.Models.DTOs.Auth;

/// <summary>
/// Result of token validation from Auth Service.
/// </summary>
public class AuthResult
{
    public bool IsValid { get; set; }
    public Guid? UserId { get; set; }
    public string? Email { get; set; }
    public string? ErrorMessage { get; set; }
}
