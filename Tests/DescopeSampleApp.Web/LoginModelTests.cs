using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using DescopeSampleApp.Web.Pages;

namespace DescopeSampleApp.Web.Tests;

public class LoginModelTests
{
    private readonly Mock<ILogger<LoginModel>> _mockLogger;
    private readonly LoginModel _pageModel;

    public LoginModelTests()
    {
        _mockLogger = new Mock<ILogger<LoginModel>>();
        _pageModel = new LoginModel(_mockLogger.Object);
    }

    [Fact]
    public void Constructor_WithValidLogger_ShouldInitialize()
    {
        // Arrange & Act
        var model = new LoginModel(_mockLogger.Object);

        // Assert
        Assert.NotNull(model);
    }

    [Fact]
    public void Constructor_WithNullLogger_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new LoginModel(null!));
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
