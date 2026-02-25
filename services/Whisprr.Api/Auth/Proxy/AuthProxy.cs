using System.Net.Http.Json;
using Whisprr.Api.Models.DTOs.Auth;

namespace Whisprr.Api.Auth.Proxy;

public partial class AuthProxy(IHttpClientFactory factory, ILogger<AuthProxy> logger) : IAuthProxy
{
    private readonly HttpClient _httpClient = factory.CreateClient("AuthService");
    private readonly ILogger<AuthProxy> _logger = logger;

    public async Task<AuthResult> ValidateTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(
                "/api/auth/validate",
                new { Token = token },
                cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                return new AuthResult
                {
                    IsValid = false,
                    ErrorMessage = $"Auth service returned {(int)response.StatusCode}"
                };
            }

            var result = await response.Content.ReadFromJsonAsync<AuthResult>(cancellationToken);
            return result ?? new AuthResult { IsValid = false, ErrorMessage = "Invalid response" };
        }
        catch (Exception ex)
        {
            LogValidationError(_logger, ex.Message);
            return new AuthResult { IsValid = false, ErrorMessage = ex.Message };
        }
    }

    public async Task<UserInfo?> GetUserInfoAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync(
                $"/api/auth/users/{userId}",
                cancellationToken);

            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<UserInfo>(cancellationToken);
        }
        catch (Exception ex)
        {
            LogUserInfoError(_logger, userId, ex.Message);
            return null;
        }
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/auth/login", request, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<AuthResponse>(cancellationToken)
            ?? throw new InvalidOperationException("Invalid response from auth service");
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/auth/register", request, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<AuthResponse>(cancellationToken)
            ?? throw new InvalidOperationException("Invalid response from auth service");
    }

    [LoggerMessage(
        Level = LogLevel.Error,
        Message = "Token validation failed: {ErrorMessage}")]
    static partial void LogValidationError(ILogger<AuthProxy> logger, string errorMessage);

    [LoggerMessage(
        Level = LogLevel.Error,
        Message = "Failed to get user info for {UserId}: {ErrorMessage}")]
    static partial void LogUserInfoError(ILogger<AuthProxy> logger, Guid userId, string errorMessage);
}
