using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace Whisprr.AuthService.Infrastructure;

public static class ApiExtensions
{
    public static IHostApplicationBuilder AddApiServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddControllers(options =>
        {
            options.Conventions.Add(new RouteTokenTransformerConvention(new KebabCaseParameterTransformer()));
        });

        return builder;
    }

    class KebabCaseParameterTransformer : IOutboundParameterTransformer
    {
        public string? TransformOutbound(object? value)
        {
            if (value == null) return null;

            // Uses regex to find the capital letters and insert a hyphen
            // Transforming "AuthService" into "auth-service"
            return System.Text.RegularExpressions.Regex.Replace(
                value.ToString()!,
                "([a-z0-9])([A-Z])",
                "$1-$2").ToLowerInvariant();
        }
    }
}
