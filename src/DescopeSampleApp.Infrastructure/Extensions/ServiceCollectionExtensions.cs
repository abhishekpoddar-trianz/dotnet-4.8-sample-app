using DescopeSampleApp.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DescopeSampleApp.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        string descopeProjectId)
    {
        services.AddHttpClient();

        services.AddSingleton<ITokenValidatorService>(sp =>
        {
            var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
            var logger = sp.GetRequiredService<ILogger<TokenValidatorService>>();
            return new TokenValidatorService(httpClientFactory, logger, descopeProjectId);
        });

        return services;
    }
}
