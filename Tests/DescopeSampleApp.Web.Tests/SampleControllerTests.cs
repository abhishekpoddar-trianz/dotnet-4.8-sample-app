using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using DescopeSampleApp.Web.Controllers;
using DescopeSampleApp.Domain.Interfaces.Services;

namespace DescopeSampleApp.Web.Tests;

public class SampleControllerTests
{
    private readonly Mock<ITokenValidationService> _mockTokenValidationService;
    private readonly Mock<ILogger<SampleController>> _mockLogger;
    private readonly SampleController _controller;

    public SampleControllerTests()
    {
        _mockTokenValidationService = new Mock<ITokenValidationService>();
        _mockLogger = new Mock<ILogger<SampleController>>();
        _controller = new SampleController(_mockTokenValidationService.Object, _mockLogger.Object);
    }

    [Fact]
    public void Constructor_WithValidParameters_ShouldInitialize()
    {
        // Arrange & Act
        var controller = new SampleController(_mockTokenValidationService.Object, _mockLogger.Object);

        // Assert
        Assert.NotNull(controller);
    }

    [Fact]
    public void Constructor_WithNullTokenValidationService_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            new SampleController(null!, _mockLogger.Object));
    }

    [Fact]
    public void Constructor_WithNullLogger_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            new SampleController(_mockTokenValidationService.Object, null!));
    }

    [Fact]
    public async Task Get_WithValidToken_ShouldReturnOkResult()
    {
        // Arrange
        var token = "valid-token";
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
        _controller.Request.Headers.Authorization = $"Bearer {token}";

        _mockTokenValidationService
            .Setup(s => s.ValidateSessionAsync(token, It.IsAny<CancellationToken>()))
            .ReturnsAsync("payload");

        // Act
        var result = await _controller.Get();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
    }

    [Fact]
    public async Task Get_WithoutAuthorizationHeader_ShouldReturnUnauthorized()
    {
        // Arrange
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };

        // Act
        var result = await _controller.Get();

        // Assert
        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
        Assert.NotNull(unauthorizedResult.Value);
    }

    [Fact]
    public async Task Get_WithEmptyAuthorizationHeader_ShouldReturnUnauthorized()
    {
        // Arrange
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
        _controller.Request.Headers.Authorization = string.Empty;

        // Act
        var result = await _controller.Get();

        // Assert
        Assert.IsType<UnauthorizedObjectResult>(result);
    }

    [Fact]
    public async Task Get_WithInvalidAuthorizationHeaderFormat_ShouldReturnUnauthorized()
    {
        // Arrange
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
        _controller.Request.Headers.Authorization = "Invalid token";

        // Act
        var result = await _controller.Get();

        // Assert
        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
        Assert.NotNull(unauthorizedResult.Value);
    }

    [Fact]
    public async Task Get_WithBearerButNoToken_ShouldReturnUnauthorized()
    {
        // Arrange
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
        _controller.Request.Headers.Authorization = "Bearer ";

        // Act
        var result = await _controller.Get();

        // Assert
        Assert.IsType<UnauthorizedObjectResult>(result);
    }

    [Fact]
    public async Task Get_WithSecurityTokenValidationException_ShouldReturnUnauthorized()
    {
        // Arrange
        var token = "invalid-token";
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
        _controller.Request.Headers.Authorization = $"Bearer {token}";

        _mockTokenValidationService
            .Setup(s => s.ValidateSessionAsync(token, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new SecurityTokenValidationException("Invalid token"));

        // Act
        var result = await _controller.Get();

        // Assert
        Assert.IsType<UnauthorizedObjectResult>(result);
    }

    [Fact]
    public async Task Get_WithGenericException_ShouldReturn500()
    {
        // Arrange
        var token = "error-token";
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
        _controller.Request.Headers.Authorization = $"Bearer {token}";

        _mockTokenValidationService
            .Setup(s => s.ValidateSessionAsync(token, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Generic error"));

        // Act
        var result = await _controller.Get();

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusCodeResult.StatusCode);
    }

    [Fact]
    public async Task Get_WithValidToken_ShouldCallTokenValidationService()
    {
        // Arrange
        var token = "test-token";
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
        _controller.Request.Headers.Authorization = $"Bearer {token}";

        _mockTokenValidationService
            .Setup(s => s.ValidateSessionAsync(token, It.IsAny<CancellationToken>()))
            .ReturnsAsync("payload");

        // Act
        await _controller.Get();

        // Assert
        _mockTokenValidationService.Verify(
            s => s.ValidateSessionAsync(token, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Get_WithBearerLowercaseFormat_ShouldWork()
    {
        // Arrange
        var token = "test-token";
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
        _controller.Request.Headers.Authorization = $"bearer {token}";

        _mockTokenValidationService
            .Setup(s => s.ValidateSessionAsync(token, It.IsAny<CancellationToken>()))
            .ReturnsAsync("payload");

        // Act
        var result = await _controller.Get();

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task Get_WithTokenWithWhitespace_ShouldTrimAndValidate()
    {
        // Arrange
        var token = "  test-token  ";
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
        _controller.Request.Headers.Authorization = $"Bearer {token}";

        _mockTokenValidationService
            .Setup(s => s.ValidateSessionAsync(token.Trim(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("payload");

        // Act
        var result = await _controller.Get();

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task Get_WithValidToken_ShouldReturnExpectedMessage()
    {
        // Arrange
        var token = "valid-token";
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
        _controller.Request.Headers.Authorization = $"Bearer {token}";

        _mockTokenValidationService
            .Setup(s => s.ValidateSessionAsync(token, It.IsAny<CancellationToken>()))
            .ReturnsAsync("payload");

        // Act
        var result = await _controller.Get();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var value = okResult.Value;
        Assert.NotNull(value);
    }
}
