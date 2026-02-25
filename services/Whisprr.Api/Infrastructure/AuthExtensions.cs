using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Whisprr.Api.Auth.CurrentUser;
using Whisprr.Api.Auth.Proxy;

namespace Whisprr.Api.Infrastructure;

public static class AuthExtensions
{
    public static IHostApplicationBuilder AddAuthenticationServices(this IHostApplicationBuilder builder)
    {
        var configuration = builder.Configuration;
        var authServiceUrl = configuration["AuthService:BaseUrl"] ?? "http://localhost:5001";
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
            options.Authority = authServiceUrl;
            options.RequireHttpsMetadata = !builder.Environment.IsDevelopment(); // Set to true in production/staging
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

            // For SignalR WebSocket auth
            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = async context =>
                {
                    var accessToken = context.Request.Query["access_token"];
                    if (!string.IsNullOrEmpty(accessToken))
                    {
                        context.Token = accessToken;
                    }
                }
            };
        });

        builder.Services.AddAuthorization();

        // Register auth services
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddScoped<ICurrentUserProvider, CurrentUserProvider>();
        builder.Services.AddScoped<IAuthProxy, AuthProxy>();

        // Configure HTTP client for Auth Service
        builder.Services.AddHttpClient("AuthService", client =>
        {
            client.BaseAddress = new Uri(authServiceUrl);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        });

        return builder;
    }

    public static WebApplication UseAuthenticationServices(this WebApplication app)
    {
        app.UseAuthentication();
        app.UseAuthorization();
        return app;
    }
}
