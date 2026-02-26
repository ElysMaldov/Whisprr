using Microsoft.EntityFrameworkCore;
using Whisprr.AuthService.Data;
using Whisprr.AuthService.Modules.MessageBroker;
using Whisprr.AuthService.Modules.Token;
using Whisprr.AuthService.Models.Domain;
using Whisprr.AuthService.Models.DTOs;

namespace Whisprr.AuthService.Modules.Auth;

/// <summary>
/// Implementation of authentication service.
/// </summary>
public partial class AuthService(
    AuthDbContext dbContext,
    ITokenService tokenService,
    IUserEventPublisher eventPublisher,
    ILogger<AuthService> logger) : IAuthService
{
    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        // Check if email exists
        if (await dbContext.Users.AnyAsync(u => u.Email == request.Email, cancellationToken))
            throw new InvalidOperationException("Email already registered");

        // Hash password
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            PasswordHash = passwordHash,
            DisplayName = request.DisplayName
        };

        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync(cancellationToken);

        // Generate tokens
        var token = tokenService.GenerateJwtToken(user);
        var refreshToken = tokenService.GenerateRefreshToken();

        // Save refresh token and update last login
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
        user.LastLoginAt = DateTime.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);

        // Publish event
        await eventPublisher.PublishUserCreatedAsync(user, cancellationToken);

        LogUserRegistered(logger, user.Id, user.Email);

        return new AuthResponse
        {
            UserId = user.Id,
            Email = user.Email,
            DisplayName = user.DisplayName,
            Token = token,
            RefreshToken = refreshToken
        };
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var user = await dbContext.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email && u.IsActive, cancellationToken);

        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid credentials");

        // Update last login
        user.LastLoginAt = DateTime.UtcNow;

        // Generate tokens
        var token = tokenService.GenerateJwtToken(user);
        var refreshToken = tokenService.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);

        await dbContext.SaveChangesAsync(cancellationToken);

        // Publish event
        await eventPublisher.PublishUserLoggedInAsync(user, cancellationToken);

        LogUserLoggedIn(logger, user.Id, user.Email);

        return new AuthResponse
        {
            UserId = user.Id,
            Email = user.Email,
            DisplayName = user.DisplayName,
            Token = token,
            RefreshToken = refreshToken
        };
    }

    public async Task<AuthResult> ValidateTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        var userId = tokenService.ValidateToken(token);

        if (userId == null)
        {
            return new AuthResult { IsValid = false, ErrorMessage = "Invalid token" };
        }

        var user = await dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == userId && u.IsActive, cancellationToken);

        if (user == null)
        {
            return new AuthResult { IsValid = false, ErrorMessage = "User not found or inactive" };
        }

        return new AuthResult
        {
            IsValid = true,
            UserId = user.Id,
            Email = user.Email
        };
    }

    public async Task<UserResponse?> GetUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user == null)
            return null;

        return new UserResponse
        {
            Id = user.Id,
            Email = user.Email,
            DisplayName = user.DisplayName,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt,
            LastLoginAt = user.LastLoginAt
        };
    }

    public async Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default)
    {
        var user = await dbContext.Users
            .FirstOrDefaultAsync(u => u.RefreshToken == request.RefreshToken && u.IsActive, cancellationToken);

        if (user == null || user.RefreshTokenExpiry <= DateTime.UtcNow)
        {
            throw new UnauthorizedAccessException("Invalid or expired refresh token");
        }

        // Generate new tokens
        var newToken = tokenService.GenerateJwtToken(user);
        var newRefreshToken = tokenService.GenerateRefreshToken();

        // Update refresh token
        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);

        await dbContext.SaveChangesAsync(cancellationToken);

        LogTokenRefreshed(logger, user.Id, user.Email);

        return new AuthResponse
        {
            UserId = user.Id,
            Email = user.Email,
            DisplayName = user.DisplayName,
            Token = newToken,
            RefreshToken = newRefreshToken
        };
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "User registered: {UserId} ({Email})")]
    static partial void LogUserRegistered(ILogger<AuthService> logger, Guid userId, string email);

    [LoggerMessage(Level = LogLevel.Information, Message = "User logged in: {UserId} ({Email})")]
    static partial void LogUserLoggedIn(ILogger<AuthService> logger, Guid userId, string email);

    [LoggerMessage(Level = LogLevel.Information, Message = "Token refreshed for user: {UserId} ({Email})")]
    static partial void LogTokenRefreshed(ILogger<AuthService> logger, Guid userId, string email);
}
