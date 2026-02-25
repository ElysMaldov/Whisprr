namespace Whisprr.AuthService.Models.DTOs;

/// <summary>
/// User information response.
/// </summary>
public class UserResponse
{
    public Guid Id { get; set; }
    public required string Email { get; set; }
    public string? DisplayName { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
}
