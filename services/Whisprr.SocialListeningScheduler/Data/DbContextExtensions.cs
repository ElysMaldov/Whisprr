using Microsoft.EntityFrameworkCore;
using Whisprr.Contracts.Enums;

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
        npgsqlOptions.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName); // Sets migration in the root folder of this project

        // Map enums to PostgreSQL native enum types
        npgsqlOptions.MapEnum<TaskProgressStatus>();
      });
    });

    return builder;
  }
}
