namespace Whisprr.AuthService.Infrastructure;

/// <summary>
/// Extension methods for Swagger/OpenAPI documentation.
/// </summary>
public static class SwaggerExtensions
{
    /// <summary>
    /// Adds API documentation services to the application.
    /// </summary>
    public static IHostApplicationBuilder AddApiDocumentation(this IHostApplicationBuilder builder)
    {
        builder.Services.AddOpenApi();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        return builder;
    }

    /// <summary>
    /// Uses API documentation middleware in the application pipeline.
    /// </summary>
    public static WebApplication UseApiDocumentation(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        return app;
    }
}
