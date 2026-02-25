using Whisprr.AuthService.Modules.Auth;
using Whisprr.AuthService.Modules.MessageBroker;
using Whisprr.AuthService.Modules.Token;

namespace Whisprr.AuthService.Infrastructure;

/// <summary>
/// Extension methods for authentication service registration.
/// </summary>
public static class AuthExtensions
{
    /// <summary>
    /// Adds authentication services to the application.
    /// </summary>
    public static IHostApplicationBuilder AddAuthServices(this IHostApplicationBuilder builder)
    {
        // Register Token and Auth services
        builder.Services.AddScoped<ITokenService, TokenService>();
        builder.Services.AddScoped<IAuthService, Modules.Auth.AuthService>();
        builder.Services.AddSingleton<IUserEventPublisher, UserEventPublisher>();

        return builder;
    }

    /// <summary>
    /// Uses authentication middleware in the application pipeline.
    /// </summary>
    public static WebApplication UseAuthServices(this WebApplication app)
    {
        app.UseAuthentication();
        app.UseAuthorization();

        return app;
    }
}
