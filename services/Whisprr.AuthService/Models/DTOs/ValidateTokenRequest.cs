using System.ComponentModel.DataAnnotations;

namespace Whisprr.AuthService.Models.DTOs;

/// <summary>
/// Request to validate a JWT token.
/// </summary>
public class ValidateTokenRequest
{
    [Required]
    public required string Token { get; set; }
}
