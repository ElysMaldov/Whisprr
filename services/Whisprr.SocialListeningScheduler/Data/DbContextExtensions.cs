using Microsoft.EntityFrameworkCore;

namespace Whisprr.SocialListeningScheduler.Data;

public static class DbContextExtensions
{
  public static IHostApplicationBuilder AddAppDbContext(this IHostApplicationBuilder builder)
  {
    builder.Services.AddDbContext<AppDbContext>(options =>
    {
      var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
      options.UseNpgsql(connectionString, npgsqlOptions =>
      {
        npgsqlOptions.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName);
        npgsqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", "scheduler");
      });
    });

    return builder;
  }
}
