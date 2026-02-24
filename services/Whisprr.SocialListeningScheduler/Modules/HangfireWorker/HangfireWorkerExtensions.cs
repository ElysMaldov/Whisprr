using Hangfire;

namespace Whisprr.SocialListeningScheduler.Modules.HangfireWorker;

public static class HangfireWorkerExtensions
{
  public static IServiceCollection AddHangfireWorker(this IServiceCollection services)
  {
    services.AddHangfire(config => config.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UseInMemoryStorage());

    services.AddHangfireServer();
    services.AddSingleton<HangfireWorker>();

    return services;
  }

  public static IHost UseHangfireWorker(this IHost host)
  {
    using var scope = host.Services.CreateScope();
    var recurringJobManager = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();

    recurringJobManager.AddOrUpdate<HangfireWorker>(
        "empty-job",
        worker => worker.Execute(),
        "*/5 * * * * *");

    return host;
  }
}
