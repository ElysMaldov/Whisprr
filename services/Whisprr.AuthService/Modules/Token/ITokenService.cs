using Whisprr.AuthService.Models.Domain;

namespace Whisprr.AuthService.Modules.Token;

/// <summary>
/// Service for generating and validating JWT tokens.
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Generates a JWT token for the specified user.
    /// </summary>
    string GenerateJwtToken(User user);

    /// <summary>
    /// Generates a secure random refresh token.
    /// </summary>
    string GenerateRefreshToken();

    /// <summary>
    /// Validates a JWT token and returns the user ID if valid.
    /// </summary>
    Guid? ValidateToken(string token);
}
