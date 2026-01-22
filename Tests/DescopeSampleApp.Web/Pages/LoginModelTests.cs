using Xunit;
using Microsoft.Extensions.Configuration;
using DescopeSampleApp.Web.Pages;
using System.Collections.Generic;

namespace DescopeSampleApp.Web.Pages.Tests
{
    public class LoginModelTests
    {
        [Fact]
        public void Constructor_WithConfiguration_ShouldInitialize()
        {
            // Arrange
            var configuration = new ConfigurationBuilder().Build();

            // Act
            var loginModel = new LoginModel(configuration);

            // Assert
            Assert.NotNull(loginModel);
        }

        [Fact]
        public void Constructor_WithNullConfiguration_ShouldNotThrow()
        {
            // Arrange & Act
            var loginModel = new LoginModel(null!);

            // Assert
            Assert.NotNull(loginModel);
        }

        [Fact]
        public void OnGet_WithProjectIdInConfiguration_ShouldSetProjectId()
        {
            // Arrange
            var inMemorySettings = new Dictionary<string, string>
            {
                {"Descope:ProjectId", "TestProjectId123"}
            };
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings!)
                .Build();
            var loginModel = new LoginModel(configuration);

            // Act
            loginModel.OnGet();

            // Assert
            Assert.Equal("TestProjectId123", loginModel.ProjectId);
        }

        [Fact]
        public void OnGet_WithoutProjectIdInConfiguration_ShouldSetDefaultProjectId()
        {
            // Arrange
            var configuration = new ConfigurationBuilder().Build();
            var loginModel = new LoginModel(configuration);

            // Act
            loginModel.OnGet();

            // Assert
            Assert.Equal("P2dI0leWLEC45BDmfxeOCSSOWiCt", loginModel.ProjectId);
        }

        [Fact]
        public void ProjectId_ShouldInitializeAsEmpty()
        {
            // Arrange
            var configuration = new ConfigurationBuilder().Build();

            // Act
            var loginModel = new LoginModel(configuration);

            // Assert
            Assert.Equal(string.Empty, loginModel.ProjectId);
        }

        [Fact]
        public void OnGet_CalledMultipleTimes_ShouldReturnSameProjectId()
        {
            // Arrange
            var inMemorySettings = new Dictionary<string, string>
            {
                {"Descope:ProjectId", "ConsistentId"}
            };
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings!)
                .Build();
            var loginModel = new LoginModel(configuration);

            // Act
            loginModel.OnGet();
            var firstProjectId = loginModel.ProjectId;
            loginModel.OnGet();
            var secondProjectId = loginModel.ProjectId;

            // Assert
            Assert.Equal(firstProjectId, secondProjectId);
        }
    }
}
