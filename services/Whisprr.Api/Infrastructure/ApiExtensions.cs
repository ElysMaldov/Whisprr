using Whisprr.Api.Services;

namespace Whisprr.Api.Infrastructure;

public static class ApiExtensions
{
    public static IHostApplicationBuilder AddApiServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddControllers();
        builder.Services.AddAuthorization();

        // Register application services
        builder.Services.AddScoped<ISocialTopicService, SocialTopicService>();
        builder.Services.AddScoped<ISocialListeningTaskService, SocialListeningTaskService>();
        builder.Services.AddScoped<ISocialInfoService, SocialInfoService>();

        return builder;
    }

    public static WebApplication UseApiServices(this WebApplication app)
    {
        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();

        return app;
    }
}
