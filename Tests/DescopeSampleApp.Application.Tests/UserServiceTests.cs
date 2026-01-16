using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using DescopeSampleApp.Application.Services;
using DescopeSampleApp.Domain.Entities;
using DescopeSampleApp.Domain.Interfaces.Repositories;

namespace DescopeSampleApp.Application.Tests;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<ILogger<UserService>> _mockLogger;
    private readonly UserService _userService;

    public UserServiceTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockLogger = new Mock<ILogger<UserService>>();
        _userService = new UserService(_mockUserRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public void Constructor_WithValidParameters_ShouldInitialize()
    {
        // Arrange & Act
        var service = new UserService(_mockUserRepository.Object, _mockLogger.Object);

        // Assert
        Assert.NotNull(service);
    }

    [Fact]
    public void Constructor_WithNullRepository_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            new UserService(null!, _mockLogger.Object));
    }

    [Fact]
    public void Constructor_WithNullLogger_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            new UserService(_mockUserRepository.Object, null!));
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllUsers()
    {
        // Arrange
        var users = new List<User>
        {
            new User { Id = 1, Name = "User1", Email = "user1@test.com" },
            new User { Id = 2, Name = "User2", Email = "user2@test.com" }
        };
        _mockUserRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(users);

        // Act
        var result = await _userService.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        _mockUserRepository.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_WithEmptyRepository_ShouldReturnEmptyList()
    {
        // Arrange
        _mockUserRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<User>());

        // Act
        var result = await _userService.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllAsync_WhenRepositoryThrows_ShouldPropagateException()
    {
        // Arrange
        _mockUserRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _userService.GetAllAsync());
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ShouldReturnUser()
    {
        // Arrange
        var user = new User { Id = 1, Name = "Test User", Email = "test@test.com" };
        _mockUserRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        var result = await _userService.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Test User", result.Name);
        _mockUserRepository.Verify(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        _mockUserRepository.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _userService.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_WhenRepositoryThrows_ShouldPropagateException()
    {
        // Arrange
        _mockUserRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _userService.GetByIdAsync(1));
    }

    [Fact]
    public async Task CreateAsync_WithValidUser_ShouldSetCreatedDateAndIsActive()
    {
        // Arrange
        var user = new User { Name = "New User", Email = "new@test.com" };
        _mockUserRepository.Setup(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User u, CancellationToken ct) => u);

        // Act
        var result = await _userService.CreateAsync(user);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsActive);
        Assert.NotEqual(default(DateTime), result.CreatedDate);
        _mockUserRepository.Verify(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WhenRepositoryThrows_ShouldPropagateException()
    {
        // Arrange
        var user = new User { Name = "New User", Email = "new@test.com" };
        _mockUserRepository.Setup(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _userService.CreateAsync(user));
    }

    [Fact]
    public async Task UpdateAsync_WithExistingUser_ShouldUpdateAndSetModifiedDate()
    {
        // Arrange
        var existingUser = new User
        {
            Id = 1,
            Name = "Original",
            CreatedDate = DateTime.UtcNow.AddDays(-10),
            CreatedBy = "System"
        };
        var updatedUser = new User { Name = "Updated", Email = "updated@test.com" };

        _mockUserRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingUser);
        _mockUserRepository.Setup(r => r.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _userService.UpdateAsync(1, updatedUser);

        // Assert
        Assert.Equal(1, updatedUser.Id);
        Assert.NotNull(updatedUser.ModifiedDate);
        Assert.Equal(existingUser.CreatedDate, updatedUser.CreatedDate);
        Assert.Equal(existingUser.CreatedBy, updatedUser.CreatedBy);
        _mockUserRepository.Verify(r => r.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WithNonExistingUser_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        var user = new User { Name = "Updated", Email = "updated@test.com" };
        _mockUserRepository.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _userService.UpdateAsync(999, user));
    }

    [Fact]
    public async Task UpdateAsync_WhenRepositoryThrows_ShouldPropagateException()
    {
        // Arrange
        var user = new User { Name = "Updated", Email = "updated@test.com" };
        _mockUserRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _userService.UpdateAsync(1, user));
    }

    [Fact]
    public async Task DeleteAsync_WithExistingUser_ShouldDeleteUser()
    {
        // Arrange
        _mockUserRepository.Setup(r => r.ExistsAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _mockUserRepository.Setup(r => r.DeleteAsync(1, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _userService.DeleteAsync(1);

        // Assert
        _mockUserRepository.Verify(r => r.ExistsAsync(1, It.IsAny<CancellationToken>()), Times.Once);
        _mockUserRepository.Verify(r => r.DeleteAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WithNonExistingUser_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        _mockUserRepository.Setup(r => r.ExistsAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _userService.DeleteAsync(999));
    }

    [Fact]
    public async Task DeleteAsync_WhenRepositoryThrows_ShouldPropagateException()
    {
        // Arrange
        _mockUserRepository.Setup(r => r.ExistsAsync(1, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _userService.DeleteAsync(1));
    }

    [Fact]
    public async Task SearchAsync_WithValidSearchTerm_ShouldReturnMatchingUsers()
    {
        // Arrange
        var users = new List<User>
        {
            new User { Id = 1, Name = "John Doe", Email = "john@test.com" },
            new User { Id = 2, Name = "Jane Doe", Email = "jane@test.com" }
        };
        _mockUserRepository.Setup(r => r.SearchAsync("Doe", It.IsAny<CancellationToken>()))
            .ReturnsAsync(users);

        // Act
        var result = await _userService.SearchAsync("Doe");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        _mockUserRepository.Verify(r => r.SearchAsync("Doe", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SearchAsync_WithNoMatches_ShouldReturnEmptyList()
    {
        // Arrange
        _mockUserRepository.Setup(r => r.SearchAsync("NoMatch", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<User>());

        // Act
        var result = await _userService.SearchAsync("NoMatch");

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task SearchAsync_WhenRepositoryThrows_ShouldPropagateException()
    {
        // Arrange
        _mockUserRepository.Setup(r => r.SearchAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _userService.SearchAsync("test"));
    }

    [Fact]
    public async Task SearchAsync_WithEmptySearchTerm_ShouldCallRepository()
    {
        // Arrange
        _mockUserRepository.Setup(r => r.SearchAsync(string.Empty, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<User>());

        // Act
        var result = await _userService.SearchAsync(string.Empty);

        // Assert
        Assert.NotNull(result);
        _mockUserRepository.Verify(r => r.SearchAsync(string.Empty, It.IsAny<CancellationToken>()), Times.Once);
    }
}
