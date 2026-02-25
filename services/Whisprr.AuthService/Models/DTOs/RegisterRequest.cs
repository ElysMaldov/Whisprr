using System.ComponentModel.DataAnnotations;

namespace Whisprr.AuthService.Models.DTOs;

/// <summary>
/// Request to register a new user.
/// </summary>
public class RegisterRequest
{
    [Required]
    [EmailAddress]
    [MaxLength(256)]
    public required string Email { get; set; }

    [Required]
    [MinLength(8)]
    public required string Password { get; set; }

    [MaxLength(200)]
    public string? DisplayName { get; set; }
}
