using System.Text.RegularExpressions;
using Domain.Domain.Constants;

namespace webApi;

public static class CorsConfigurationExtension
{
    public const string FrontendPolicyName =  CorsConfigurationConstans.FrontendPolicyName;

    public static IServiceCollection AddConfiguredCors(this IServiceCollection services, IConfiguration configuration)
    {
        var allowedOrigins = GetAllowedOrigins(configuration);

        if (allowedOrigins.Length == 0) throw new InvalidOperationException("Cors:AllowedOrigins configuration is required.");

        var exactOrigins = allowedOrigins
            .Where(origin => !origin.Contains('*'))
            .ToArray();

        var wildcardMatchers = allowedOrigins
            .Where(origin => origin.Contains('*'))
            .Select(BuildWildcardRegex)
            .ToArray();

        services.AddCors(options =>
        {
            options.AddPolicy(FrontendPolicyName, policy =>
            {
                policy
                    .SetIsOriginAllowed(origin =>
                        exactOrigins.Contains(origin, StringComparer.OrdinalIgnoreCase)
                        || wildcardMatchers.Any(matcher => matcher.IsMatch(origin)))
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });

        return services;
    }

    private static Regex BuildWildcardRegex(string pattern)
    {
        var escaped = Regex.Escape(pattern).Replace("\\*", "[^.]*");
        return new Regex($"^{escaped}$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    }

    private static string[] GetAllowedOrigins(IConfiguration configuration)
    {
        var fromSection = configuration
            .GetSection(CorsConfigurationConstans.CorsAllowedOriginsSection)
            .Get<string[]>()
            ?? [];

        var fromSingleVariable = configuration["CORS_ALLOWED_ORIGINS"];
        var fromEnvironment = string.IsNullOrWhiteSpace(fromSingleVariable)
            ? []
            : fromSingleVariable.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        return NormalizeOrigins(fromSection.Concat(fromEnvironment));
    }

    private static string[] NormalizeOrigins(IEnumerable<string> values)
        => values
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Select(value => value.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
}
