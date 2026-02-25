using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Whisprr.Api.Auth.Proxy;
using Whisprr.Api.Models.DTOs.Auth;

namespace Whisprr.Api.Controllers;

/// <summary>
/// Authentication controller that proxies requests to the Auth Service.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthProxy authProxy, ILogger<AuthController> logger) : ControllerBase
{
    /// <summary>
    /// Authenticates a user and returns tokens.
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await authProxy.LoginAsync(request, cancellationToken);
            return Ok(response);
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            logger.LogWarning("Login failed for email: {Email}", request.Email);
            return Unauthorized(new ProblemDetails
            {
                Title = "Authentication Failed",
                Detail = "Invalid credentials",
                Status = StatusCodes.Status401Unauthorized
            });
        }
    }

    /// <summary>
    /// Registers a new user.
    /// </summary>
    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await authProxy.RegisterAsync(request, cancellationToken);
            return Ok(response);
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Conflict)
        {
            logger.LogWarning("Registration failed for email: {Email}", request.Email);
            return Conflict(new ProblemDetails
            {
                Title = "Registration Failed",
                Detail = "Email already registered",
                Status = StatusCodes.Status409Conflict
            });
        }
    }

    /// <summary>
    /// Refreshes an access token using a valid refresh token.
    /// </summary>
    [HttpPost("refresh")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponse>> RefreshToken(RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await authProxy.RefreshTokenAsync(request, cancellationToken);
            return Ok(response);
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            logger.LogWarning("Token refresh failed");
            return Unauthorized(new ProblemDetails
            {
                Title = "Token Refresh Failed",
                Detail = "Invalid or expired refresh token",
                Status = StatusCodes.Status401Unauthorized
            });
        }
    }
}
