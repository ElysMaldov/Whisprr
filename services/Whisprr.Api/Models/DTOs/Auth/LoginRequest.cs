using System.ComponentModel.DataAnnotations;

namespace Whisprr.Api.Models.DTOs.Auth;

/// <summary>
/// Request to login an existing user.
/// </summary>
public class LoginRequest
{
    [Required]
    [EmailAddress]
    public required string Email { get; set; }

    [Required]
    public required string Password { get; set; }
}
