namespace Whisprr.Api.Models.DTOs.Auth;

/// <summary>
/// User information from Auth Service.
/// </summary>
public class UserInfo
{
    public Guid Id { get; set; }
    public required string Email { get; set; }
    public string? DisplayName { get; set; }
    public bool IsActive { get; set; }
}
