using DescopeSampleApp.Application.Services;
using DescopeSampleApp.Domain.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DescopeSampleApp.Application.Extensions;

/// <summary>
/// Extension methods for registering application services
/// </summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Register services
        services.AddScoped<IUserService, UserService>();

        return services;
    }
}
