namespace Whisprr.Api.Auth.CurrentUser;

/// <summary>
/// Provides access to the current authenticated user's information.
/// This doesn't  use the domain <see cref="User"/> model since this gets the user data
/// from JWT.
/// </summary>
public interface ICurrentUserProvider
{
    /// <summary>
    /// The current user's ID. Throws if not authenticated.
    /// </summary>
    Guid UserId { get; }

    /// <summary>
    /// The current user's email. Throws if not authenticated.
    /// </summary>
    string Email { get; }

    /// <summary>
    /// Whether the current request is authenticated.
    /// </summary>
    bool IsAuthenticated { get; }

    /// <summary>
    /// Try to get the current user ID without throwing.
    /// </summary>
    bool TryGetUserId(out Guid userId);
}
