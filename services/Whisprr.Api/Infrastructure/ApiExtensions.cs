using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Whisprr.Api.Hubs;
using Whisprr.Api.Services;

namespace Whisprr.Api.Infrastructure;

public static class ApiExtensions
{
    public static IHostApplicationBuilder AddApiServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddControllers(options =>
       {
           options.Conventions.Add(new RouteTokenTransformerConvention(new KebabCaseParameterTransformer()));
       });

        // Add SignalR for real-time updates
        builder.Services.AddSignalR();

        // Register application services
        builder.Services.AddScoped<ISocialTopicService, SocialTopicService>();
        builder.Services.AddScoped<ISocialListeningTaskService, SocialListeningTaskService>();
        builder.Services.AddScoped<ISocialInfoService, SocialInfoService>();
        builder.Services.AddScoped<INotificationService, NotificationService>();

        return builder;
    }

    public static WebApplication UseApiServices(this WebApplication app)
    {
        app.UseHttpsRedirection();
        app.MapControllers();

        // Map SignalR hub
        app.MapHub<SocialTopicHub>("/hubs/social");

        return app;
    }

    class KebabCaseParameterTransformer : IOutboundParameterTransformer
    {
        public string? TransformOutbound(object? value)
        {
            if (value == null) return null;

            // Uses regex to find the capital letters and insert a hyphen
            // Transforming "AuthService" into "auth-service"
            return System.Text.RegularExpressions.Regex.Replace(
                value.ToString()!,
                "([a-z0-9])([A-Z])",
                "$1-$2").ToLowerInvariant();
        }
    }
}
