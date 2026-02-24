using Hangfire;
using Hangfire.Dashboard;

namespace Whisprr.SocialListeningScheduler.Modules.HangfireWorker;

public static class HangfireWorkerExtensions
{
  public static IHostApplicationBuilder AddHangfireWorker(this IHostApplicationBuilder builder)
  {
    builder.Services.AddHangfire(config => config.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UseInMemoryStorage());

    builder.Services.AddHangfireServer();
    builder.Services.AddSingleton<HangfireWorker>();

    return builder;
  }

  public static IApplicationBuilder UseHangfireWorker(this IApplicationBuilder app)
  {
    app.UseHangfireDashboard("/hangfire");

    using var scope = app.ApplicationServices.CreateScope();
    var recurringJobManager = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();

    // TODO finish handling this
    recurringJobManager.AddOrUpdate<HangfireWorker>(
        "empty-job",
        worker => worker.Execute(),
        "*/10 * * * * *");

    return app;
  }
}
