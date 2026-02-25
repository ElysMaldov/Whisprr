using Whisprr.Api.Models.DTOs.Auth;

namespace Whisprr.Api.Auth.Proxy;

/// <summary>
/// Proxy for communicating with the Auth Service.
/// </summary>
public interface IAuthProxy
{
    /// <summary>
    /// Validate a JWT token with the Auth Service.
    /// </summary>
    Task<AuthResult> ValidateTokenAsync(string token, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get user information from the Auth Service.
    /// </summary>
    Task<UserInfo?> GetUserInfoAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Login a user via the Auth Service.
    /// </summary>
    Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Register a new user via the Auth Service.
    /// </summary>
    Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);
}
