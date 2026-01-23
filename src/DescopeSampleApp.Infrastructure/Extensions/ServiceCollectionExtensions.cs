using DescopeSampleApp.Domain.Interfaces.Services;
using DescopeSampleApp.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DescopeSampleApp.Infrastructure.Extensions;

/// <summary>
/// Extension methods for registering infrastructure services
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers infrastructure layer services
    /// </summary>
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Register HttpClient
        services.AddHttpClient();

        // Register token validation service
        var projectId = configuration["Descope:ProjectId"] ?? "P2dI0leWLEC45BDmfxeOCSSOWiCt";
        services.AddScoped<ITokenValidationService>(sp =>
        {
            var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
            var logger = sp.GetRequiredService<ILogger<TokenValidationService>>();
            return new TokenValidationService(httpClientFactory, logger, projectId);
        });

        return services;
    }
}
