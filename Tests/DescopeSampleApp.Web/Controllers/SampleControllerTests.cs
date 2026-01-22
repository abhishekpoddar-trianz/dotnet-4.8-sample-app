using Xunit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using DescopeSampleApp.Web.Controllers;
using DescopeSampleApp.Infrastructure.Services;
using Moq;
using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using SecurityException = DescopeSampleApp.Infrastructure.Services.SecurityException;

namespace DescopeSampleApp.Web.Controllers.Tests
{
    public class SampleControllerTests
    {
        private readonly Mock<ITokenValidatorService> _mockTokenValidator;
        private readonly Mock<ILogger<SampleController>> _mockLogger;
        private readonly SampleController _controller;

        public SampleControllerTests()
        {
            _mockTokenValidator = new Mock<ITokenValidatorService>();
            _mockLogger = new Mock<ILogger<SampleController>>();
            _controller = new SampleController(_mockTokenValidator.Object, _mockLogger.Object);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
        }

        [Fact]
        public void Constructor_WithValidParameters_ShouldInitialize()
        {
            // Arrange & Act
            var controller = new SampleController(_mockTokenValidator.Object, _mockLogger.Object);

            // Assert
            Assert.NotNull(controller);
        }

        [Fact]
        public void Constructor_WithNullTokenValidator_ShouldNotThrow()
        {
            // Arrange, Act
            var controller = new SampleController(null!, _mockLogger.Object);

            // Assert
            Assert.NotNull(controller);
        }

        [Fact]
        public void Constructor_WithNullLogger_ShouldNotThrow()
        {
            // Arrange, Act
            var controller = new SampleController(_mockTokenValidator.Object, null!);

            // Assert
            Assert.NotNull(controller);
        }

        [Fact]
        public async Task Get_WithMissingAuthorizationHeader_ShouldReturnUnauthorized()
        {
            // Arrange
            _controller.ControllerContext.HttpContext.Request.Headers.Authorization = string.Empty;

            // Act
            var result = await _controller.Get(CancellationToken.None);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.NotNull(unauthorizedResult.Value);
        }

        [Fact]
        public async Task Get_WithInvalidAuthorizationHeaderFormat_ShouldReturnUnauthorized()
        {
            // Arrange
            _controller.ControllerContext.HttpContext.Request.Headers.Authorization = "InvalidFormat token";

            // Act
            var result = await _controller.Get(CancellationToken.None);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.NotNull(unauthorizedResult.Value);
        }

        [Fact]
        public async Task Get_WithEmptyToken_ShouldReturnUnauthorized()
        {
            // Arrange
            _controller.ControllerContext.HttpContext.Request.Headers.Authorization = "Bearer ";

            // Act
            var result = await _controller.Get(CancellationToken.None);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.NotNull(unauthorizedResult.Value);
        }

        [Fact]
        public async Task Get_WithValidToken_ShouldReturnOk()
        {
            // Arrange
            var token = "valid-token-123";
            _controller.ControllerContext.HttpContext.Request.Headers.Authorization = $"Bearer {token}";

            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, "testuser")
            }, "TestAuth"));

            _mockTokenValidator
                .Setup(x => x.ValidateSessionAsync(token, It.IsAny<CancellationToken>()))
                .ReturnsAsync(claimsPrincipal);

            // Act
            var result = await _controller.Get(CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task Get_WithValidToken_ShouldCallTokenValidator()
        {
            // Arrange
            var token = "valid-token-456";
            _controller.ControllerContext.HttpContext.Request.Headers.Authorization = $"Bearer {token}";

            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity());
            _mockTokenValidator
                .Setup(x => x.ValidateSessionAsync(token, It.IsAny<CancellationToken>()))
                .ReturnsAsync(claimsPrincipal);

            // Act
            await _controller.Get(CancellationToken.None);

            // Assert
            _mockTokenValidator.Verify(
                x => x.ValidateSessionAsync(token, It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Get_WhenTokenValidationThrowsSecurityException_ShouldReturnUnauthorized()
        {
            // Arrange
            var token = "invalid-token";
            _controller.ControllerContext.HttpContext.Request.Headers.Authorization = $"Bearer {token}";

            _mockTokenValidator
                .Setup(x => x.ValidateSessionAsync(token, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new SecurityException("Invalid token"));

            // Act
            var result = await _controller.Get(CancellationToken.None);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.NotNull(unauthorizedResult.Value);
        }

        [Fact]
        public async Task Get_WhenUnexpectedExceptionOccurs_ShouldReturnInternalServerError()
        {
            // Arrange
            var token = "some-token";
            _controller.ControllerContext.HttpContext.Request.Headers.Authorization = $"Bearer {token}";

            _mockTokenValidator
                .Setup(x => x.ValidateSessionAsync(token, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Unexpected error"));

            // Act
            var result = await _controller.Get(CancellationToken.None);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }

        [Fact]
        public async Task Get_WithValidToken_ShouldLogInformation()
        {
            // Arrange
            var token = "valid-token-789";
            _controller.ControllerContext.HttpContext.Request.Headers.Authorization = $"Bearer {token}";

            var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, "testuser") }, "TestAuth");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            _mockTokenValidator
                .Setup(x => x.ValidateSessionAsync(token, It.IsAny<CancellationToken>()))
                .ReturnsAsync(claimsPrincipal);

            // Act
            await _controller.Get(CancellationToken.None);

            // Assert
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Token validated successfully")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task Get_WithMissingAuthorizationHeader_ShouldLogWarning()
        {
            // Arrange
            _controller.ControllerContext.HttpContext.Request.Headers.Authorization = string.Empty;

            // Act
            await _controller.Get(CancellationToken.None);

            // Assert
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Missing or invalid authorization header")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task Get_WithSecurityException_ShouldLogWarning()
        {
            // Arrange
            var token = "invalid-token";
            _controller.ControllerContext.HttpContext.Request.Headers.Authorization = $"Bearer {token}";

            _mockTokenValidator
                .Setup(x => x.ValidateSessionAsync(token, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new SecurityException("Token validation failed"));

            // Act
            await _controller.Get(CancellationToken.None);

            // Assert
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Token validation failed")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task Get_WithUnexpectedException_ShouldLogError()
        {
            // Arrange
            var token = "some-token";
            _controller.ControllerContext.HttpContext.Request.Headers.Authorization = $"Bearer {token}";

            _mockTokenValidator
                .Setup(x => x.ValidateSessionAsync(token, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Unexpected error"));

            // Act
            await _controller.Get(CancellationToken.None);

            // Assert
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Unexpected error in Sample API")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task Get_WithCancellationToken_ShouldPassToValidator()
        {
            // Arrange
            var token = "valid-token";
            var cts = new CancellationTokenSource();
            var cancellationToken = cts.Token;
            _controller.ControllerContext.HttpContext.Request.Headers.Authorization = $"Bearer {token}";

            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity());
            _mockTokenValidator
                .Setup(x => x.ValidateSessionAsync(token, cancellationToken))
                .ReturnsAsync(claimsPrincipal);

            // Act
            await _controller.Get(cancellationToken);

            // Assert
            _mockTokenValidator.Verify(
                x => x.ValidateSessionAsync(token, cancellationToken),
                Times.Once);
        }

        [Fact]
        public async Task Get_WithBearerTokenWithExtraSpaces_ShouldTrimAndValidate()
        {
            // Arrange
            var token = "token-with-spaces";
            _controller.ControllerContext.HttpContext.Request.Headers.Authorization = $"Bearer   {token}   ";

            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity());
            _mockTokenValidator
                .Setup(x => x.ValidateSessionAsync(token, It.IsAny<CancellationToken>()))
                .ReturnsAsync(claimsPrincipal);

            // Act
            var result = await _controller.Get(CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }
    }
}
