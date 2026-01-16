using Xunit;
using Moq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using DescopeSampleApp.Web.Pages;
using System.Diagnostics;

namespace DescopeSampleApp.Web.Tests;

public class ErrorModelTests
{
    private readonly Mock<ILogger<ErrorModel>> _mockLogger;
    private readonly ErrorModel _pageModel;

    public ErrorModelTests()
    {
        _mockLogger = new Mock<ILogger<ErrorModel>>();
        _pageModel = new ErrorModel(_mockLogger.Object);
    }

    [Fact]
    public void Constructor_WithValidLogger_ShouldInitialize()
    {
        // Arrange & Act
        var model = new ErrorModel(_mockLogger.Object);

        // Assert
        Assert.NotNull(model);
    }

    [Fact]
    public void RequestId_DefaultValue_ShouldBeNull()
    {
        // Assert
        Assert.Null(_pageModel.RequestId);
    }

    [Fact]
    public void RequestId_WhenSet_ShouldReturnValue()
    {
        // Arrange
        var requestId = "test-request-id";

        // Act
        _pageModel.RequestId = requestId;

        // Assert
        Assert.Equal(requestId, _pageModel.RequestId);
    }

    [Fact]
    public void ShowRequestId_WhenRequestIdIsNull_ShouldReturnFalse()
    {
        // Arrange
        _pageModel.RequestId = null;

        // Act
        var result = _pageModel.ShowRequestId;

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void ShowRequestId_WhenRequestIdIsEmpty_ShouldReturnFalse()
    {
        // Arrange
        _pageModel.RequestId = string.Empty;

        // Act
        var result = _pageModel.ShowRequestId;

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void ShowRequestId_WhenRequestIdHasValue_ShouldReturnTrue()
    {
        // Arrange
        _pageModel.RequestId = "test-request-id";

        // Act
        var result = _pageModel.ShowRequestId;

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void OnGet_ShouldSetRequestIdFromHttpContext()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        httpContext.TraceIdentifier = "trace-123";
        _pageModel.PageContext = new PageContext
        {
            HttpContext = httpContext
        };

        // Act
        _pageModel.OnGet();

        // Assert
        Assert.NotNull(_pageModel.RequestId);
        Assert.Equal("trace-123", _pageModel.RequestId);
    }

    [Fact]
    public void OnGet_WithoutHttpContext_ShouldNotThrow()
    {
        // Act & Assert
        var exception = Record.Exception(() => _pageModel.OnGet());
        Assert.Null(exception);
    }

    [Fact]
    public void OnGet_ShouldComplete()
    {
        // Arrange
        _pageModel.PageContext = new PageContext
        {
            HttpContext = new DefaultHttpContext()
        };

        // Act
        _pageModel.OnGet();

        // Assert - method completes without exception
        Assert.True(true);
    }

    [Fact]
    public void ShowRequestId_WithWhitespaceRequestId_ShouldReturnFalse()
    {
        // Arrange
        _pageModel.RequestId = "   ";

        // Act
        var result = _pageModel.ShowRequestId;

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void RequestId_CanBeSetToNull()
    {
        // Arrange
        _pageModel.RequestId = "test";

        // Act
        _pageModel.RequestId = null;

        // Assert
        Assert.Null(_pageModel.RequestId);
    }
}
