using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using DescopeSampleApp.Web.Controllers;

namespace DescopeSampleApp.Web.Tests;

public class HomeControllerTests
{
    private readonly Mock<ILogger<HomeController>> _mockLogger;
    private readonly HomeController _controller;

    public HomeControllerTests()
    {
        _mockLogger = new Mock<ILogger<HomeController>>();
        _controller = new HomeController(_mockLogger.Object);
    }

    [Fact]
    public void Constructor_WithValidLogger_ShouldInitialize()
    {
        // Arrange & Act
        var controller = new HomeController(_mockLogger.Object);

        // Assert
        Assert.NotNull(controller);
    }

    [Fact]
    public void Constructor_WithNullLogger_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new HomeController(null!));
    }

    [Fact]
    public void Index_ShouldReturnViewResult()
    {
        // Act
        var result = _controller.Index();

        // Assert
        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public void Index_ShouldSetViewDataTitle()
    {
        // Act
        var result = _controller.Index() as ViewResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Home Page", _controller.ViewData["Title"]);
    }

    [Fact]
    public void Index_ShouldReturnDefaultView()
    {
        // Act
        var result = _controller.Index() as ViewResult;

        // Assert
        Assert.NotNull(result);
        Assert.Null(result.ViewName);
    }

    [Fact]
    public void Error_ShouldReturnViewResult()
    {
        // Act
        var result = _controller.Error();

        // Assert
        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public void Error_ShouldReturnDefaultView()
    {
        // Act
        var result = _controller.Error() as ViewResult;

        // Assert
        Assert.NotNull(result);
        Assert.Null(result.ViewName);
    }
}
