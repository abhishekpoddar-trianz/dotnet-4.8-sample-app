using Xunit;
using Moq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using DescopeSampleApp.Application.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace DescopeSampleApp.Application.Tests;

public class TokenValidationServiceTests
{
    private readonly Mock<IHttpClientFactory> _mockHttpClientFactory;
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly Mock<ILogger<TokenValidationService>> _mockLogger;

    public TokenValidationServiceTests()
    {
        _mockHttpClientFactory = new Mock<IHttpClientFactory>();
        _mockConfiguration = new Mock<IConfiguration>();
        _mockLogger = new Mock<ILogger<TokenValidationService>>();
    }

    [Fact]
    public void Constructor_WithValidParameters_ShouldInitialize()
    {
        // Arrange
        _mockConfiguration.Setup(c => c["Descope:ProjectId"]).Returns("test-project-id");

        // Act
        var service = new TokenValidationService(
            _mockHttpClientFactory.Object,
            _mockConfiguration.Object,
            _mockLogger.Object);

        // Assert
        Assert.NotNull(service);
    }

    [Fact]
    public void Constructor_WithNullHttpClientFactory_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            new TokenValidationService(
                null!,
                _mockConfiguration.Object,
                _mockLogger.Object));
    }

    [Fact]
    public void Constructor_WithNullConfiguration_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            new TokenValidationService(
                _mockHttpClientFactory.Object,
                null!,
                _mockLogger.Object));
    }

    [Fact]
    public void Constructor_WithNullLogger_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            new TokenValidationService(
                _mockHttpClientFactory.Object,
                _mockConfiguration.Object,
                null!));
    }

    [Fact]
    public void VerifyTokenExpiration_WithValidNonExpiredToken_ShouldReturnTrue()
    {
        // Arrange
        _mockConfiguration.Setup(c => c["Descope:ProjectId"]).Returns("test-project-id");
        var service = new TokenValidationService(
            _mockHttpClientFactory.Object,
            _mockConfiguration.Object,
            _mockLogger.Object);

        var token = GenerateJwtToken(DateTime.UtcNow.AddHours(1));

        // Act
        var result = service.VerifyTokenExpiration(token);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void VerifyTokenExpiration_WithExpiredToken_ShouldReturnFalse()
    {
        // Arrange
        _mockConfiguration.Setup(c => c["Descope:ProjectId"]).Returns("test-project-id");
        var service = new TokenValidationService(
            _mockHttpClientFactory.Object,
            _mockConfiguration.Object,
            _mockLogger.Object);

        var token = GenerateJwtToken(DateTime.UtcNow.AddHours(-1));

        // Act
        var result = service.VerifyTokenExpiration(token);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void VerifyTokenExpiration_WithInvalidToken_ShouldReturnFalse()
    {
        // Arrange
        _mockConfiguration.Setup(c => c["Descope:ProjectId"]).Returns("test-project-id");
        var service = new TokenValidationService(
            _mockHttpClientFactory.Object,
            _mockConfiguration.Object,
            _mockLogger.Object);

        // Act
        var result = service.VerifyTokenExpiration("invalid-token");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void VerifyTokenExpiration_WithEmptyToken_ShouldReturnFalse()
    {
        // Arrange
        _mockConfiguration.Setup(c => c["Descope:ProjectId"]).Returns("test-project-id");
        var service = new TokenValidationService(
            _mockHttpClientFactory.Object,
            _mockConfiguration.Object,
            _mockLogger.Object);

        // Act
        var result = service.VerifyTokenExpiration(string.Empty);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task ValidateSessionAsync_WithInvalidToken_ShouldThrowException()
    {
        // Arrange
        _mockConfiguration.Setup(c => c["Descope:ProjectId"]).Returns("test-project-id");
        var service = new TokenValidationService(
            _mockHttpClientFactory.Object,
            _mockConfiguration.Object,
            _mockLogger.Object);

        // Act & Assert
        await Assert.ThrowsAnyAsync<Exception>(async () =>
            await service.ValidateSessionAsync("invalid-token"));
    }

    [Fact]
    public async Task ValidateSessionAsync_WithEmptyToken_ShouldThrowException()
    {
        // Arrange
        _mockConfiguration.Setup(c => c["Descope:ProjectId"]).Returns("test-project-id");
        var service = new TokenValidationService(
            _mockHttpClientFactory.Object,
            _mockConfiguration.Object,
            _mockLogger.Object);

        // Act & Assert
        await Assert.ThrowsAnyAsync<Exception>(async () =>
            await service.ValidateSessionAsync(string.Empty));
    }

    [Fact]
    public void Constructor_WithNoProjectIdInConfig_ShouldUseDefaultValue()
    {
        // Arrange
        _mockConfiguration.Setup(c => c["Descope:ProjectId"]).Returns((string?)null);

        // Act
        var service = new TokenValidationService(
            _mockHttpClientFactory.Object,
            _mockConfiguration.Object,
            _mockLogger.Object);

        // Assert
        Assert.NotNull(service);
    }

    [Fact]
    public void VerifyTokenExpiration_WithTokenExpiringExactlyNow_ShouldReturnFalse()
    {
        // Arrange
        _mockConfiguration.Setup(c => c["Descope:ProjectId"]).Returns("test-project-id");
        var service = new TokenValidationService(
            _mockHttpClientFactory.Object,
            _mockConfiguration.Object,
            _mockLogger.Object);

        var token = GenerateJwtToken(DateTime.UtcNow);

        // Act
        var result = service.VerifyTokenExpiration(token);

        // Assert
        Assert.False(result);
    }

    private string GenerateJwtToken(DateTime expiration)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("this-is-a-test-key-with-at-least-32-characters"));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, "test-user"),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: "test-issuer",
            audience: "test-audience",
            claims: claims,
            expires: expiration,
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
