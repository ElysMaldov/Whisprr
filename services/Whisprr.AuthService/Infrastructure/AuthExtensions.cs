using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
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
        var configuration = builder.Configuration;
        var jwtKey = configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured");
        var jwtIssuer = configuration["Jwt:Issuer"] ?? "whisprr-auth";
        var jwtAudience = configuration["Jwt:Audience"] ?? "whisprr-api";

        // Configure JWT Bearer authentication
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtIssuer,
                ValidAudience = jwtAudience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                ClockSkew = TimeSpan.Zero
            };
        });

        builder.Services.AddAuthorization();

        // Register Token and Auth services
        builder.Services.AddScoped<ITokenService, TokenService>();
        builder.Services.AddScoped<IAuthService, Modules.Auth.AuthService>();
        builder.Services.AddScoped<IUserEventPublisher, UserEventPublisher>();

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
