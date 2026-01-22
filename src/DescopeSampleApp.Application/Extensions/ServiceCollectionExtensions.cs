using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace DescopeSampleApp.Application.Extensions;

/// <summary>
/// Extension methods for registering application services
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers all application layer services
    /// </summary>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Register AutoMapper
        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        // Register services here when needed
        // services.AddScoped<IYourService, YourService>();

        return services;
    }
}
