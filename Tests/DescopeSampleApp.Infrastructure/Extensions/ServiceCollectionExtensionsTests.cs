using Xunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using DescopeSampleApp.Infrastructure.Extensions;
using DescopeSampleApp.Infrastructure.Services;
using System;

namespace DescopeSampleApp.Infrastructure.Extensions.Tests
{
    public class ServiceCollectionExtensionsTests
    {
        [Fact]
        public void AddInfrastructureServices_WithValidProjectId_ShouldRegisterServices()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddLogging();
            var projectId = "test-project-id";

            // Act
            services.AddInfrastructureServices(projectId);
            var serviceProvider = services.BuildServiceProvider();

            // Assert
            var tokenValidator = serviceProvider.GetService<ITokenValidatorService>();
            Assert.NotNull(tokenValidator);
        }

        [Fact]
        public void AddInfrastructureServices_ShouldRegisterHttpClient()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddLogging();
            var projectId = "test-project-id";

            // Act
            services.AddInfrastructureServices(projectId);
            var serviceProvider = services.BuildServiceProvider();

            // Assert
            var httpClientFactory = serviceProvider.GetService<IHttpClientFactory>();
            Assert.NotNull(httpClientFactory);
        }

        [Fact]
        public void AddInfrastructureServices_ShouldRegisterTokenValidatorAsSingleton()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddLogging();
            var projectId = "test-project-id";

            // Act
            services.AddInfrastructureServices(projectId);
            var serviceProvider = services.BuildServiceProvider();

            // Assert
            var tokenValidator1 = serviceProvider.GetService<ITokenValidatorService>();
            var tokenValidator2 = serviceProvider.GetService<ITokenValidatorService>();
            Assert.Same(tokenValidator1, tokenValidator2);
        }

        [Fact]
        public void AddInfrastructureServices_WithEmptyProjectId_ShouldRegisterServices()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddLogging();
            var projectId = string.Empty;

            // Act
            services.AddInfrastructureServices(projectId);
            var serviceProvider = services.BuildServiceProvider();

            // Assert
            var tokenValidator = serviceProvider.GetService<ITokenValidatorService>();
            Assert.NotNull(tokenValidator);
        }

        [Fact]
        public void AddInfrastructureServices_ShouldReturnServiceCollection()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddLogging();
            var projectId = "test-project-id";

            // Act
            var result = services.AddInfrastructureServices(projectId);

            // Assert
            Assert.Same(services, result);
        }

        [Fact]
        public void AddInfrastructureServices_CalledMultipleTimes_ShouldNotThrow()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddLogging();
            var projectId = "test-project-id";

            // Act
            services.AddInfrastructureServices(projectId);
            services.AddInfrastructureServices(projectId);
            var serviceProvider = services.BuildServiceProvider();

            // Assert
            var tokenValidator = serviceProvider.GetService<ITokenValidatorService>();
            Assert.NotNull(tokenValidator);
        }

        [Fact]
        public void AddInfrastructureServices_WithNullServiceCollection_ShouldThrow()
        {
            // Arrange
            IServiceCollection? services = null;
            var projectId = "test-project-id";

            // Act & Assert
            try
            {
                services!.AddInfrastructureServices(projectId);
                Assert.Fail("Expected exception was not thrown");
            }
            catch (ArgumentNullException)
            {
                Assert.True(true);
            }
        }

        [Fact]
        public void AddInfrastructureServices_WithNullProjectId_ShouldRegisterServices()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddLogging();
            string? projectId = null;

            // Act
            services.AddInfrastructureServices(projectId!);
            var serviceProvider = services.BuildServiceProvider();

            // Assert
            var tokenValidator = serviceProvider.GetService<ITokenValidatorService>();
            Assert.NotNull(tokenValidator);
        }

        [Fact]
        public void AddInfrastructureServices_WithDifferentProjectIds_ShouldRegisterDifferentServices()
        {
            // Arrange
            var services1 = new ServiceCollection();
            services1.AddLogging();
            var projectId1 = "project-id-1";

            var services2 = new ServiceCollection();
            services2.AddLogging();
            var projectId2 = "project-id-2";

            // Act
            services1.AddInfrastructureServices(projectId1);
            var serviceProvider1 = services1.BuildServiceProvider();

            services2.AddInfrastructureServices(projectId2);
            var serviceProvider2 = services2.BuildServiceProvider();

            // Assert
            var tokenValidator1 = serviceProvider1.GetService<ITokenValidatorService>();
            var tokenValidator2 = serviceProvider2.GetService<ITokenValidatorService>();
            Assert.NotNull(tokenValidator1);
            Assert.NotNull(tokenValidator2);
            Assert.NotSame(tokenValidator1, tokenValidator2);
        }

        [Fact]
        public void AddInfrastructureServices_ShouldResolveHttpClientFactoryInFactory()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddLogging();
            var projectId = "test-project-id";

            // Act
            services.AddInfrastructureServices(projectId);
            var serviceProvider = services.BuildServiceProvider();

            // Assert
            Assert.NotNull(serviceProvider.GetService<ITokenValidatorService>());
            Assert.NotNull(serviceProvider.GetService<IHttpClientFactory>());
        }

        [Fact]
        public void AddInfrastructureServices_ShouldResolveLoggerInFactory()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddLogging();
            var projectId = "test-project-id";

            // Act
            services.AddInfrastructureServices(projectId);
            var serviceProvider = services.BuildServiceProvider();

            // Assert
            Assert.NotNull(serviceProvider.GetService<ITokenValidatorService>());
            Assert.NotNull(serviceProvider.GetService<ILogger<TokenValidatorService>>());
        }
    }
}
