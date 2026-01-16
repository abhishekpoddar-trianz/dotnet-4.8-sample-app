using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using DescopeSampleApp.Web.Pages;

namespace DescopeSampleApp.Web.Tests;

public class PrivacyModelTests
{
    private readonly Mock<ILogger<PrivacyModel>> _mockLogger;
    private readonly PrivacyModel _pageModel;

    public PrivacyModelTests()
    {
        _mockLogger = new Mock<ILogger<PrivacyModel>>();
        _pageModel = new PrivacyModel(_mockLogger.Object);
    }

    [Fact]
    public void Constructor_WithValidLogger_ShouldInitialize()
    {
        // Arrange & Act
        var model = new PrivacyModel(_mockLogger.Object);

        // Assert
        Assert.NotNull(model);
    }

    [Fact]
    public void OnGet_ShouldNotThrow()
    {
        // Act & Assert
        var exception = Record.Exception(() => _pageModel.OnGet());
        Assert.Null(exception);
    }

    [Fact]
    public void OnGet_ShouldComplete()
    {
        // Act
        _pageModel.OnGet();

        // Assert - method completes without exception
        Assert.True(true);
    }
}
