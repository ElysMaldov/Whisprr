using Whisprr.Api.Hubs;
using Whisprr.Api.Services;

namespace Whisprr.Api.Infrastructure;

public static class ApiExtensions
{
    public static IHostApplicationBuilder AddApiServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddControllers();
        
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
}
