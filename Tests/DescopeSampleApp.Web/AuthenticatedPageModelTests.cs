using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using DescopeSampleApp.Web.Pages;

namespace DescopeSampleApp.Web.Tests;

public class AuthenticatedPageModelTests
{
    private readonly Mock<ILogger<AuthenticatedPageModel>> _mockLogger;
    private readonly AuthenticatedPageModel _pageModel;

    public AuthenticatedPageModelTests()
    {
        _mockLogger = new Mock<ILogger<AuthenticatedPageModel>>();
        _pageModel = new AuthenticatedPageModel(_mockLogger.Object);
    }

    [Fact]
    public void Constructor_WithValidLogger_ShouldInitialize()
    {
        // Arrange & Act
        var model = new AuthenticatedPageModel(_mockLogger.Object);

        // Assert
        Assert.NotNull(model);
    }

    [Fact]
    public void Constructor_WithNullLogger_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new AuthenticatedPageModel(null!));
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
