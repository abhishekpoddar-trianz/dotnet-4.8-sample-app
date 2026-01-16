using Xunit;
using Microsoft.Extensions.DependencyInjection;
using DescopeSampleApp.Application.Extensions;
using DescopeSampleApp.Domain.Interfaces.Services;
using DescopeSampleApp.Application.Services;

namespace DescopeSampleApp.Application.Tests;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddApplicationServices_ShouldRegisterUserService()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddHttpClient();

        // Act
        services.AddApplicationServices();
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var userService = serviceProvider.GetService<IUserService>();
        Assert.NotNull(userService);
        Assert.IsType<UserService>(userService);
    }

    [Fact]
    public void AddApplicationServices_ShouldRegisterTokenValidationService()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddHttpClient();

        // Act
        services.AddApplicationServices();
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var tokenService = serviceProvider.GetService<ITokenValidationService>();
        Assert.NotNull(tokenService);
        Assert.IsType<TokenValidationService>(tokenService);
    }

    [Fact]
    public void AddApplicationServices_ShouldReturnServiceCollection()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var result = services.AddApplicationServices();

        // Assert
        Assert.NotNull(result);
        Assert.Same(services, result);
    }

    [Fact]
    public void AddApplicationServices_ShouldRegisterServicesAsScoped()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddHttpClient();

        // Act
        services.AddApplicationServices();

        // Assert
        var userServiceDescriptor = services.FirstOrDefault(s => s.ServiceType == typeof(IUserService));
        Assert.NotNull(userServiceDescriptor);
        Assert.Equal(ServiceLifetime.Scoped, userServiceDescriptor.Lifetime);

        var tokenServiceDescriptor = services.FirstOrDefault(s => s.ServiceType == typeof(ITokenValidationService));
        Assert.NotNull(tokenServiceDescriptor);
        Assert.Equal(ServiceLifetime.Scoped, tokenServiceDescriptor.Lifetime);
    }

    [Fact]
    public void AddApplicationServices_WithEmptyServiceCollection_ShouldAddServices()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddApplicationServices();

        // Assert
        Assert.Contains(services, s => s.ServiceType == typeof(IUserService));
        Assert.Contains(services, s => s.ServiceType == typeof(ITokenValidationService));
    }

    [Fact]
    public void AddApplicationServices_CalledMultipleTimes_ShouldRegisterServicesMultipleTimes()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddApplicationServices();
        services.AddApplicationServices();

        // Assert
        var userServiceCount = services.Count(s => s.ServiceType == typeof(IUserService));
        Assert.Equal(2, userServiceCount);
    }

    [Fact]
    public void AddApplicationServices_ShouldNotThrowException()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act & Assert
        var exception = Record.Exception(() => services.AddApplicationServices());
        Assert.Null(exception);
    }
}
