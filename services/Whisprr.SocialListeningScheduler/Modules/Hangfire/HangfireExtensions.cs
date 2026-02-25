using Hangfire;
using Whisprr.SocialListeningScheduler.Modules.SocialListeningTaskPublisher;

namespace Whisprr.SocialListeningScheduler.Modules.Hangfire;

internal static class HangfireExtensions
{
  public static IHostApplicationBuilder AddHangfire(this IHostApplicationBuilder builder)
  {
    builder.Services.AddHangfire(config => config.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UseInMemoryStorage());

    builder.Services.AddHangfireServer();

    return builder;
  }

  public static IApplicationBuilder UseHangfire(this IApplicationBuilder app)
  {
    app.UseHangfireDashboard("/hangfire");

    using var scope = app.ApplicationServices.CreateScope();
    var recurringJobManager = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();

    // name the ids like the main worker method for easier maintenance
    recurringJobManager.AddOrUpdate<ISocialListeningTaskPublisher>(
        nameof(ISocialListeningTaskPublisher.ArrangeAndPublishTasks),
        worker => worker.ArrangeAndPublishTasks(),
        "0 */15 * * * *");

    return app;
  }
}
