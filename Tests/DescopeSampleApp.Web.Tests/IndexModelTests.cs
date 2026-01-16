using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using DescopeSampleApp.Web.Pages;

namespace DescopeSampleApp.Web.Tests;

public class IndexModelTests
{
    private readonly Mock<ILogger<IndexModel>> _mockLogger;
    private readonly IndexModel _pageModel;

    public IndexModelTests()
    {
        _mockLogger = new Mock<ILogger<IndexModel>>();
        _pageModel = new IndexModel(_mockLogger.Object);
    }

    [Fact]
    public void Constructor_WithValidLogger_ShouldInitialize()
    {
        // Arrange & Act
        var model = new IndexModel(_mockLogger.Object);

        // Assert
        Assert.NotNull(model);
    }

    [Fact]
    public void Constructor_WithNullLogger_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new IndexModel(null!));
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
