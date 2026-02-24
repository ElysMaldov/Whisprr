using Hangfire;
using Hangfire.Dashboard;

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

  public static IApplicationBuilder UseHangfireWorker(this IApplicationBuilder app)
  {
    app.UseHangfireDashboard("/hangfire", new DashboardOptions
    {
      IsReadOnlyFunc = (DashboardContext dashboardContext) =>
      {
        var context = dashboardContext.GetHttpContext();
        return !context.User.IsInRole("Admin");
      }
    });

    using var scope = app.ApplicationServices.CreateScope();
    var recurringJobManager = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();

    recurringJobManager.AddOrUpdate<HangfireWorker>(
        "empty-job",
        worker => worker.Execute(),
        "*/10 * * * * *");

    return app;
  }
}
