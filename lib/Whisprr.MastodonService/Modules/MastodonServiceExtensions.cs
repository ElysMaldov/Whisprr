using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Whisprr.MastodonService.Modules.MastodonService;
using System.Threading.RateLimiting;
using Polly;

namespace Whisprr.MastodonService;

/// <summary>
/// Extension methods for registering Mastodon services.
/// </summary>
public static class MastodonServiceExtensions
{
    /// <summary>
    /// Adds Mastodon service infrastructure including HttpClient.
    /// No authentication is required for public search operations.
    /// </summary>
    public static IHostApplicationBuilder AddMastodonServices(this IHostApplicationBuilder builder)
    {
        var baseUrl = builder.Configuration["Mastodon:BaseUrl"] ?? "https://mastodon.social";

        // MastodonService - no auth handlers needed for public search
        builder.Services.AddHttpClient<IMastodonService, Modules.MastodonService.MastodonService>(client =>
        {
            client.BaseAddress = new Uri(baseUrl);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        })
        .AddResilienceHandler("mastodon-limit", builder =>
        {
            // Mastodon typically has rate limits of 300 requests per 5 minutes
            // per user/IP for unauthenticated requests
            builder.AddRateLimiter(new FixedWindowRateLimiter(new FixedWindowRateLimiterOptions
            {
                PermitLimit = 60,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 10,
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst
            }));
        });

        return builder;
    }
}
