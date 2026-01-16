using Xunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using DescopeSampleApp.Infrastructure.Extensions;
using DescopeSampleApp.Domain.Interfaces.Repositories;
using DescopeSampleApp.Infrastructure.Repositories;
using DescopeSampleApp.Infrastructure.Data;

namespace DescopeSampleApp.Infrastructure.Tests;

public class ServiceCollectionExtensionsTests
{
    private IConfiguration CreateConfiguration()
    {
        var configurationBuilder = new ConfigurationBuilder();
        configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["ConnectionStrings:DefaultConnection"] = "Host=localhost;Database=testdb;Username=test;Password=test",
            ["Logging:EnableSensitiveDataLogging"] = "false"
        });
        return configurationBuilder.Build();
    }

    [Fact]
    public void AddInfrastructureServices_ShouldRegisterApplicationDbContext()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        var configuration = CreateConfiguration();

        // Act
        services.AddInfrastructureServices(configuration);
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var dbContext = serviceProvider.GetService<ApplicationDbContext>();
        Assert.NotNull(dbContext);
    }

    [Fact]
    public void AddInfrastructureServices_ShouldRegisterUserRepository()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        var configuration = CreateConfiguration();

        // Act
        services.AddInfrastructureServices(configuration);
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var userRepository = serviceProvider.GetService<IUserRepository>();
        Assert.NotNull(userRepository);
        Assert.IsType<UserRepository>(userRepository);
    }

    [Fact]
    public void AddInfrastructureServices_ShouldReturnServiceCollection()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = CreateConfiguration();

        // Act
        var result = services.AddInfrastructureServices(configuration);

        // Assert
        Assert.NotNull(result);
        Assert.Same(services, result);
    }

    [Fact]
    public void AddInfrastructureServices_ShouldRegisterRepositoryAsScoped()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = CreateConfiguration();

        // Act
        services.AddInfrastructureServices(configuration);

        // Assert
        var descriptor = services.FirstOrDefault(s => s.ServiceType == typeof(IUserRepository));
        Assert.NotNull(descriptor);
        Assert.Equal(ServiceLifetime.Scoped, descriptor.Lifetime);
    }

    [Fact]
    public void AddInfrastructureServices_ShouldRegisterDbContextAsScoped()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = CreateConfiguration();

        // Act
        services.AddInfrastructureServices(configuration);

        // Assert
        var descriptor = services.FirstOrDefault(s => s.ServiceType == typeof(ApplicationDbContext));
        Assert.NotNull(descriptor);
        Assert.Equal(ServiceLifetime.Scoped, descriptor.Lifetime);
    }

    [Fact]
    public void AddInfrastructureServices_WithEmptyServiceCollection_ShouldAddServices()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = CreateConfiguration();

        // Act
        services.AddInfrastructureServices(configuration);

        // Assert
        Assert.Contains(services, s => s.ServiceType == typeof(ApplicationDbContext));
        Assert.Contains(services, s => s.ServiceType == typeof(IUserRepository));
    }

    [Fact]
    public void AddInfrastructureServices_ShouldNotThrowException()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = CreateConfiguration();

        // Act & Assert
        var exception = Record.Exception(() => services.AddInfrastructureServices(configuration));
        Assert.Null(exception);
    }
}
