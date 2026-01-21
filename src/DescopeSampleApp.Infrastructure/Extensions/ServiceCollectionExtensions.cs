using DescopeSampleApp.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DescopeSampleApp.Infrastructure.Extensions;

/// <summary>
/// Extension methods for configuring infrastructure services
/// </summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        string descopeProjectId)
    {
        services.AddHttpClient();

        services.AddScoped<TokenValidationService>(sp =>
        {
            var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
            var logger = sp.GetRequiredService<Microsoft.Extensions.Logging.ILogger<TokenValidationService>>();
            return new TokenValidationService(httpClientFactory, logger, descopeProjectId);
        });

        return services;
    }
}
