using Microsoft.EntityFrameworkCore;
using Whisprr.Api.Data;

namespace Whisprr.Api.Infrastructure;

public static class DatabaseExtensions
{
    public static IHostApplicationBuilder AddAppDbContext(this IHostApplicationBuilder builder)
    {
        builder.Services.AddDbContext<AppDbContext>(options =>
        {
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName);
                npgsqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", "public");
            });
        });

        return builder;
    }
}
