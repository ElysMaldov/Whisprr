using System.Security.Claims;

namespace Whisprr.Api.Auth.CurrentUser;

public class CurrentUserProvider(IHttpContextAccessor httpContextAccessor) : ICurrentUserProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public Guid UserId => TryGetUserId(out var id)
          ? id
          : throw new UnauthorizedAccessException("User is not authenticated");

    public string Email => _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Email)?.Value
        ?? throw new UnauthorizedAccessException("User is not authenticated");

    public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;

    public bool TryGetUserId(out Guid userId)
    {
        userId = Guid.Empty;
        var user = _httpContextAccessor.HttpContext?.User;

        if (user?.Identity?.IsAuthenticated != true)
            return false;

        var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(userIdClaim, out userId);
    }
}
