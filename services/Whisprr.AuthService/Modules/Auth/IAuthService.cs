using Whisprr.AuthService.Models.DTOs;

namespace Whisprr.AuthService.Modules.Auth;

/// <summary>
/// Service for handling authentication operations.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Registers a new user.
    /// </summary>
    Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Authenticates a user and returns tokens.
    /// </summary>
    Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates a JWT token.
    /// </summary>
    Task<AuthResult> ValidateTokenAsync(string token, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets user information by ID.
    /// </summary>
    Task<UserResponse?> GetUserAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Refreshes an access token using a valid refresh token.
    /// </summary>
    Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default);
}
