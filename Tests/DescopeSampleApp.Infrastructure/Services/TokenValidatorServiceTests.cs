using Xunit;
using Microsoft.Extensions.Logging;
using DescopeSampleApp.Infrastructure.Services;
using Moq;
using Moq.Protected;
using System;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace DescopeSampleApp.Infrastructure.Services.Tests
{
    public class TokenValidatorServiceTests
    {
        private readonly Mock<IHttpClientFactory> _mockHttpClientFactory;
        private readonly Mock<ILogger<TokenValidatorService>> _mockLogger;
        private readonly string _projectId;

        public TokenValidatorServiceTests()
        {
            _mockHttpClientFactory = new Mock<IHttpClientFactory>();
            _mockLogger = new Mock<ILogger<TokenValidatorService>>();
            _projectId = "test-project-id";
        }

        [Fact]
        public void Constructor_WithValidParameters_ShouldInitialize()
        {
            // Arrange & Act
            var service = new TokenValidatorService(
                _mockHttpClientFactory.Object,
                _mockLogger.Object,
                _projectId);

            // Assert
            Assert.NotNull(service);
        }

        [Fact]
        public void Constructor_WithNullHttpClientFactory_ShouldNotThrow()
        {
            // Arrange & Act
            var service = new TokenValidatorService(null!, _mockLogger.Object, _projectId);

            // Assert
            Assert.NotNull(service);
        }

        [Fact]
        public void Constructor_WithNullLogger_ShouldNotThrow()
        {
            // Arrange & Act
            var service = new TokenValidatorService(_mockHttpClientFactory.Object, null!, _projectId);

            // Assert
            Assert.NotNull(service);
        }

        [Fact]
        public void Constructor_WithNullProjectId_ShouldInitialize()
        {
            // Arrange & Act
            var service = new TokenValidatorService(
                _mockHttpClientFactory.Object,
                _mockLogger.Object,
                null!);

            // Assert
            Assert.NotNull(service);
        }

        [Fact]
        public void Constructor_WithEmptyProjectId_ShouldInitialize()
        {
            // Arrange & Act
            var service = new TokenValidatorService(
                _mockHttpClientFactory.Object,
                _mockLogger.Object,
                string.Empty);

            // Assert
            Assert.NotNull(service);
        }

        [Fact]
        public async Task ValidateSessionAsync_WithInvalidToken_ShouldThrowException()
        {
            // Arrange
            var mockHandler = new Mock<HttpMessageHandler>();
            mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("{\"keys\":[]}")
                });

            var httpClient = new HttpClient(mockHandler.Object);
            _mockHttpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>()))
                .Returns(httpClient);

            var service = new TokenValidatorService(
                _mockHttpClientFactory.Object,
                _mockLogger.Object,
                _projectId);

            // Act & Assert
            await Assert.ThrowsAsync<DescopeSampleApp.Infrastructure.Services.SecurityException>(() =>
                service.ValidateSessionAsync("invalid-token", CancellationToken.None));
        }

        [Fact]
        public async Task ValidateSessionAsync_WithNullToken_ShouldThrowException()
        {
            // Arrange
            var mockHandler = new Mock<HttpMessageHandler>();
            mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("{\"keys\":[]}")
                });

            var httpClient = new HttpClient(mockHandler.Object);
            _mockHttpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>()))
                .Returns(httpClient);

            var service = new TokenValidatorService(
                _mockHttpClientFactory.Object,
                _mockLogger.Object,
                _projectId);

            // Act & Assert
            await Assert.ThrowsAsync<DescopeSampleApp.Infrastructure.Services.SecurityException>(() =>
                service.ValidateSessionAsync(null!, CancellationToken.None));
        }

        [Fact]
        public async Task ValidateSessionAsync_WithEmptyToken_ShouldThrowException()
        {
            // Arrange
            var mockHandler = new Mock<HttpMessageHandler>();
            mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("{\"keys\":[]}")
                });

            var httpClient = new HttpClient(mockHandler.Object);
            _mockHttpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>()))
                .Returns(httpClient);

            var service = new TokenValidatorService(
                _mockHttpClientFactory.Object,
                _mockLogger.Object,
                _projectId);

            // Act & Assert
            await Assert.ThrowsAsync<DescopeSampleApp.Infrastructure.Services.SecurityException>(() =>
                service.ValidateSessionAsync(string.Empty, CancellationToken.None));
        }

        [Fact]
        public async Task ValidateSessionAsync_WhenHttpClientThrowsException_ShouldThrow()
        {
            // Arrange
            var mockHandler = new Mock<HttpMessageHandler>();
            mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(new HttpRequestException("Network error"));

            var httpClient = new HttpClient(mockHandler.Object);
            _mockHttpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>()))
                .Returns(httpClient);

            var service = new TokenValidatorService(
                _mockHttpClientFactory.Object,
                _mockLogger.Object,
                _projectId);

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() =>
                service.ValidateSessionAsync("some-token", CancellationToken.None));
        }

        [Fact]
        public async Task ValidateSessionAsync_WithCancellationToken_ShouldPassToHttpClient()
        {
            // Arrange
            var cts = new CancellationTokenSource();
            cts.Cancel();
            var cancellationToken = cts.Token;

            var mockHandler = new Mock<HttpMessageHandler>();
            mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(new TaskCanceledException());

            var httpClient = new HttpClient(mockHandler.Object);
            _mockHttpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>()))
                .Returns(httpClient);

            var service = new TokenValidatorService(
                _mockHttpClientFactory.Object,
                _mockLogger.Object,
                _projectId);

            // Act & Assert
            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                service.ValidateSessionAsync("token", cancellationToken));
        }

        [Fact]
        public void VerifyTokenExpiration_WithInvalidToken_ShouldReturnFalse()
        {
            // Arrange
            var service = new TokenValidatorService(
                _mockHttpClientFactory.Object,
                _mockLogger.Object,
                _projectId);

            // Act
            var result = service.VerifyTokenExpiration("invalid-token");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void VerifyTokenExpiration_WithNullToken_ShouldReturnFalse()
        {
            // Arrange
            var service = new TokenValidatorService(
                _mockHttpClientFactory.Object,
                _mockLogger.Object,
                _projectId);

            // Act
            var result = service.VerifyTokenExpiration(null!);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void VerifyTokenExpiration_WithEmptyToken_ShouldReturnFalse()
        {
            // Arrange
            var service = new TokenValidatorService(
                _mockHttpClientFactory.Object,
                _mockLogger.Object,
                _projectId);

            // Act
            var result = service.VerifyTokenExpiration(string.Empty);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void VerifyTokenExpiration_WhenExceptionThrown_ShouldReturnFalse()
        {
            // Arrange
            var service = new TokenValidatorService(
                _mockHttpClientFactory.Object,
                _mockLogger.Object,
                _projectId);

            // Act
            var result = service.VerifyTokenExpiration("malformed-token");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void VerifyTokenExpiration_WhenExceptionThrown_ShouldLogError()
        {
            // Arrange
            var service = new TokenValidatorService(
                _mockHttpClientFactory.Object,
                _mockLogger.Object,
                _projectId);

            // Act
            service.VerifyTokenExpiration("malformed-token");

            // Assert
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Error verifying token expiration")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task ValidateSessionAsync_WhenExceptionThrown_ShouldLogError()
        {
            // Arrange
            var mockHandler = new Mock<HttpMessageHandler>();
            mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(new HttpRequestException("Network error"));

            var httpClient = new HttpClient(mockHandler.Object);
            _mockHttpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>()))
                .Returns(httpClient);

            var service = new TokenValidatorService(
                _mockHttpClientFactory.Object,
                _mockLogger.Object,
                _projectId);

            // Act
            try
            {
                await service.ValidateSessionAsync("token", CancellationToken.None);
            }
            catch
            {
                // Expected exception
            }

            // Assert
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Error")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.AtLeastOnce);
        }
    }

    public class SecurityExceptionTests
    {
        [Fact]
        public void Constructor_WithMessage_ShouldInitialize()
        {
            // Arrange
            var message = "Test security error";

            // Act
            var exception = new SecurityException(message);

            // Assert
            Assert.NotNull(exception);
            Assert.Equal(message, exception.Message);
        }

        [Fact]
        public void Constructor_WithEmptyMessage_ShouldInitialize()
        {
            // Arrange
            var message = string.Empty;

            // Act
            var exception = new SecurityException(message);

            // Assert
            Assert.NotNull(exception);
            Assert.Equal(message, exception.Message);
        }

        [Fact]
        public void Constructor_WithNullMessage_ShouldInitialize()
        {
            // Arrange
            string? message = null;

            // Act
            var exception = new SecurityException(message!);

            // Assert
            Assert.NotNull(exception);
        }

        [Fact]
        public void SecurityException_ShouldInheritFromException()
        {
            // Arrange
            var exception = new SecurityException("Test");

            // Act & Assert
            Assert.IsAssignableFrom<Exception>(exception);
        }

        [Fact]
        public void SecurityException_ShouldBeThrowable()
        {
            // Arrange
            var message = "Test throw";

            // Act & Assert
            try
            {
                throw new DescopeSampleApp.Infrastructure.Services.SecurityException(message);
            }
            catch (DescopeSampleApp.Infrastructure.Services.SecurityException ex)
            {
                Assert.Equal(message, ex.Message);
            }
        }
    }
}
