using Domain.Domain.Constants;

namespace webApi;

public static class CorsConfigurationExtension
{
    public const string FrontendPolicyName =  CorsConfigurationConstans.FrontendPolicyName;

    public static IServiceCollection AddConfiguredCors(this IServiceCollection services, IConfiguration configuration)
    {
        var allowedOrigins = configuration
            .GetSection(CorsConfigurationConstans.CorsAllowedOriginsSection)
            .Get<string[]>()
            ?.Where(value => !string.IsNullOrWhiteSpace(value))
            .Select(value => value.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray()
            ?? [];

        if (allowedOrigins.Length == 0) throw new InvalidOperationException("Cors:AllowedOrigins configuration is required.");

        services.AddCors(options =>
        {
            options.AddPolicy(FrontendPolicyName, policy =>
            {
                policy
                    .WithOrigins(allowedOrigins)
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });

        return services;
    }

    private static string[] GetAllowedOrigins(IConfiguration configuration)
    {
        var fromSection = configuration
            .GetSection("Cors:AllowedOrigins")
            .Get<string[]>();

        if (fromSection is { Length: > 0 })
            return NormalizeOrigins(fromSection);

        var fromSingleVariable = configuration["CORS_ALLOWED_ORIGINS"];
        if (string.IsNullOrWhiteSpace(fromSingleVariable))
            return [];

        var split = fromSingleVariable.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        return NormalizeOrigins(split);
    }

    private static string[] NormalizeOrigins(IEnumerable<string> values)
        => values
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Select(value => value.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
}
