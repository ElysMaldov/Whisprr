using StackExchange.Redis;
using Whisprr.Caching.Modules;

namespace Whisprr.SocialScouter.Modules.Caching;

/// <summary>
/// Extension methods for registering Redis caching service.
/// </summary>
public static class CachingExtensions
{
    /// <summary>
    /// Adds Redis (StackExchange.Redis) to the DI container.
    /// </summary>
    public static IHostApplicationBuilder AddCaching(this IHostApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("Redis") ?? "localhost:6379";

        builder.Services.AddSingleton<IConnectionMultiplexer>(
            ConnectionMultiplexer.Connect(connectionString));

        builder.Services.AddSingleton(
            sp => sp.GetRequiredService<IConnectionMultiplexer>().GetDatabase());

        builder.Services.AddSingleton<CachingService, RedisCachingService>();

        return builder;
    }
}
