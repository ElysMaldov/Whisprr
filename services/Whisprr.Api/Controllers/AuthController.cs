using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Whisprr.Api.Auth.Proxy;
using Whisprr.Api.Data;
using Whisprr.Api.Models.Domain;
using Whisprr.Api.Models.DTOs.Auth;

namespace Whisprr.Api.Controllers;

/// <summary>
/// Authentication controller that proxies requests to the Auth Service
/// and syncs user data to the local database.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController(
    IAuthProxy authProxy,
    AppDbContext dbContext,
    ILogger<AuthController> logger) : ControllerBase
{
    /// <summary>
    /// Authenticates a user and returns tokens. Updates LastLoginAt in local DB.
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
            
            // Sync user to local DB
            await SyncUserAsync(response, cancellationToken);
            
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
    /// Registers a new user and saves to local DB.
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
            
            // Create user in local DB
            await CreateUserAsync(response, cancellationToken);
            
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

    /// <summary>
    /// Creates a new user in the local database after registration.
    /// </summary>
    private async Task CreateUserAsync(AuthResponse response, CancellationToken cancellationToken)
    {
        try
        {
            // Check if user already exists
            var existingUser = await dbContext.Users
                .FirstOrDefaultAsync(u => u.Id == response.UserId, cancellationToken);

            if (existingUser != null)
            {
                logger.LogInformation("User {UserId} already exists in local DB", response.UserId);
                return;
            }

            var user = new User
            {
                Id = response.UserId,
                Email = response.Email,
                DisplayName = response.DisplayName,
                ExternalAuthId = response.UserId.ToString(),
                CreatedAt = DateTime.UtcNow,
                LastLoginAt = DateTime.UtcNow,
                IsActive = true
            };

            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync(cancellationToken);

            logger.LogInformation("User {UserId} created in local DB", response.UserId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to create user {UserId} in local DB", response.UserId);
            // Don't throw - auth should still succeed even if local sync fails
        }
    }

    /// <summary>
    /// Syncs user data and updates LastLoginAt in the local database after login.
    /// </summary>
    private async Task SyncUserAsync(AuthResponse response, CancellationToken cancellationToken)
    {
        try
        {
            var user = await dbContext.Users
                .FirstOrDefaultAsync(u => u.Id == response.UserId, cancellationToken);

            if (user == null)
            {
                // User exists in AuthService but not in local DB - create them
                user = new User
                {
                    Id = response.UserId,
                    Email = response.Email,
                    DisplayName = response.DisplayName,
                    ExternalAuthId = response.UserId.ToString(),
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };
                dbContext.Users.Add(user);
                logger.LogInformation("User {UserId} created in local DB during login", response.UserId);
            }

            // Update LastLoginAt
            user.LastLoginAt = DateTime.UtcNow;
            
            // Update DisplayName if changed
            if (user.DisplayName != response.DisplayName)
            {
                user.DisplayName = response.DisplayName;
            }

            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to sync user {UserId} in local DB", response.UserId);
            // Don't throw - auth should still succeed even if local sync fails
        }
    }
}
