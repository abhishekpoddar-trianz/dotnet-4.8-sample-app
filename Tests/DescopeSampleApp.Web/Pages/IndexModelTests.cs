using Xunit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using DescopeSampleApp.Web.Pages;
using System.Collections.Generic;
using Moq;

namespace DescopeSampleApp.Web.Pages.Tests
{
    public class IndexModelTests
    {
        [Fact]
        public void Constructor_WithValidParameters_ShouldInitialize()
        {
            // Arrange
            var configuration = new ConfigurationBuilder().Build();
            var logger = new Mock<ILogger<IndexModel>>().Object;

            // Act
            var indexModel = new IndexModel(configuration, logger);

            // Assert
            Assert.NotNull(indexModel);
        }

        [Fact]
        public void Constructor_WithNullConfiguration_ShouldNotThrow()
        {
            // Arrange
            var logger = new Mock<ILogger<IndexModel>>().Object;

            // Act
            var indexModel = new IndexModel(null!, logger);

            // Assert
            Assert.NotNull(indexModel);
        }

        [Fact]
        public void Constructor_WithNullLogger_ShouldNotThrow()
        {
            // Arrange
            var configuration = new ConfigurationBuilder().Build();

            // Act
            var indexModel = new IndexModel(configuration, null!);

            // Assert
            Assert.NotNull(indexModel);
        }

        [Fact]
        public void OnGet_WithProjectIdInConfiguration_ShouldSetProjectId()
        {
            // Arrange
            var inMemorySettings = new Dictionary<string, string>
            {
                {"Descope:ProjectId", "TestProjectId456"}
            };
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings!)
                .Build();
            var logger = new Mock<ILogger<IndexModel>>().Object;
            var indexModel = new IndexModel(configuration, logger);

            // Act
            indexModel.OnGet();

            // Assert
            Assert.Equal("TestProjectId456", indexModel.ProjectId);
        }

        [Fact]
        public void OnGet_WithoutProjectIdInConfiguration_ShouldSetDefaultProjectId()
        {
            // Arrange
            var configuration = new ConfigurationBuilder().Build();
            var logger = new Mock<ILogger<IndexModel>>().Object;
            var indexModel = new IndexModel(configuration, logger);

            // Act
            indexModel.OnGet();

            // Assert
            Assert.Equal("P2dI0leWLEC45BDmfxeOCSSOWiCt", indexModel.ProjectId);
        }

        [Fact]
        public void ProjectId_ShouldInitializeAsEmpty()
        {
            // Arrange
            var configuration = new ConfigurationBuilder().Build();
            var logger = new Mock<ILogger<IndexModel>>().Object;

            // Act
            var indexModel = new IndexModel(configuration, logger);

            // Assert
            Assert.Equal(string.Empty, indexModel.ProjectId);
        }

        [Fact]
        public void OnGet_ShouldLogInformation()
        {
            // Arrange
            var configuration = new ConfigurationBuilder().Build();
            var mockLogger = new Mock<ILogger<IndexModel>>();
            var indexModel = new IndexModel(configuration, mockLogger.Object);

            // Act
            indexModel.OnGet();

            // Assert
            mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Index page loaded")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public void OnGet_CalledMultipleTimes_ShouldLogMultipleTimes()
        {
            // Arrange
            var configuration = new ConfigurationBuilder().Build();
            var mockLogger = new Mock<ILogger<IndexModel>>();
            var indexModel = new IndexModel(configuration, mockLogger.Object);

            // Act
            indexModel.OnGet();
            indexModel.OnGet();

            // Assert
            mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Exactly(2));
        }
    }
}
