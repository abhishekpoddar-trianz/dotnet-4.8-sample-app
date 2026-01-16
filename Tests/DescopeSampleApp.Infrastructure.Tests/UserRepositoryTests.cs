using Xunit;
using Moq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using DescopeSampleApp.Infrastructure.Repositories;
using DescopeSampleApp.Infrastructure.Data;
using DescopeSampleApp.Domain.Entities;

namespace DescopeSampleApp.Infrastructure.Tests;

public class UserRepositoryTests
{
    private readonly Mock<ILogger<UserRepository>> _mockLogger;
    private readonly DbContextOptions<ApplicationDbContext> _options;

    public UserRepositoryTests()
    {
        _mockLogger = new Mock<ILogger<UserRepository>>();
        _options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    [Fact]
    public void Constructor_WithValidParameters_ShouldInitialize()
    {
        // Arrange
        using var context = new ApplicationDbContext(_options);

        // Act
        var repository = new UserRepository(context, _mockLogger.Object);

        // Assert
        Assert.NotNull(repository);
    }

    [Fact]
    public void Constructor_WithNullContext_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            new UserRepository(null!, _mockLogger.Object));
    }

    [Fact]
    public void Constructor_WithNullLogger_ShouldThrowArgumentNullException()
    {
        // Arrange
        using var context = new ApplicationDbContext(_options);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            new UserRepository(context, null!));
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnOnlyActiveUsers()
    {
        // Arrange
        using var context = new ApplicationDbContext(_options);
        var repository = new UserRepository(context, _mockLogger.Object);

        context.Users.AddRange(
            new User { Id = 1, Name = "User1", Email = "user1@test.com", IsActive = true },
            new User { Id = 2, Name = "User2", Email = "user2@test.com", IsActive = false },
            new User { Id = 3, Name = "User3", Email = "user3@test.com", IsActive = true }
        );
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.All(result, u => Assert.True(u.IsActive));
    }

    [Fact]
    public async Task GetAllAsync_WithEmptyDatabase_ShouldReturnEmptyList()
    {
        // Arrange
        using var context = new ApplicationDbContext(_options);
        var repository = new UserRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ShouldReturnUser()
    {
        // Arrange
        using var context = new ApplicationDbContext(_options);
        var repository = new UserRepository(context, _mockLogger.Object);

        var user = new User { Id = 1, Name = "Test User", Email = "test@test.com", IsActive = true };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Test User", result.Name);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        using var context = new ApplicationDbContext(_options);
        var repository = new UserRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_WithInactiveUser_ShouldReturnNull()
    {
        // Arrange
        using var context = new ApplicationDbContext(_options);
        var repository = new UserRepository(context, _mockLogger.Object);

        var user = new User { Id = 1, Name = "Inactive User", Email = "inactive@test.com", IsActive = false };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetByIdAsync(1);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task AddAsync_WithValidUser_ShouldAddUserToDatabase()
    {
        // Arrange
        using var context = new ApplicationDbContext(_options);
        var repository = new UserRepository(context, _mockLogger.Object);

        var user = new User { Name = "New User", Email = "new@test.com", IsActive = true };

        // Act
        var result = await repository.AddAsync(user);

        // Assert
        Assert.NotNull(result);
        Assert.NotEqual(0, result.Id);
        Assert.Equal("New User", result.Name);

        var savedUser = await context.Users.FindAsync(result.Id);
        Assert.NotNull(savedUser);
    }

    [Fact]
    public async Task UpdateAsync_WithValidUser_ShouldUpdateUserInDatabase()
    {
        // Arrange
        using var context = new ApplicationDbContext(_options);
        var repository = new UserRepository(context, _mockLogger.Object);

        var user = new User { Name = "Original Name", Email = "original@test.com", IsActive = true };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        // Act
        user.Name = "Updated Name";
        await repository.UpdateAsync(user);

        // Assert
        var updatedUser = await context.Users.FindAsync(user.Id);
        Assert.NotNull(updatedUser);
        Assert.Equal("Updated Name", updatedUser.Name);
    }

    [Fact]
    public async Task DeleteAsync_WithValidId_ShouldSetUserAsInactive()
    {
        // Arrange
        using var context = new ApplicationDbContext(_options);
        var repository = new UserRepository(context, _mockLogger.Object);

        var user = new User { Name = "User to Delete", Email = "delete@test.com", IsActive = true };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        // Act
        await repository.DeleteAsync(user.Id);

        // Assert
        var deletedUser = await context.Users.FindAsync(user.Id);
        Assert.NotNull(deletedUser);
        Assert.False(deletedUser.IsActive);
        Assert.NotNull(deletedUser.ModifiedDate);
    }

    [Fact]
    public async Task DeleteAsync_WithInvalidId_ShouldNotThrow()
    {
        // Arrange
        using var context = new ApplicationDbContext(_options);
        var repository = new UserRepository(context, _mockLogger.Object);

        // Act & Assert
        var exception = await Record.ExceptionAsync(() => repository.DeleteAsync(999));
        Assert.Null(exception);
    }

    [Fact]
    public async Task ExistsAsync_WithExistingActiveUser_ShouldReturnTrue()
    {
        // Arrange
        using var context = new ApplicationDbContext(_options);
        var repository = new UserRepository(context, _mockLogger.Object);

        var user = new User { Name = "Existing User", Email = "exists@test.com", IsActive = true };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.ExistsAsync(user.Id);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task ExistsAsync_WithNonExistingUser_ShouldReturnFalse()
    {
        // Arrange
        using var context = new ApplicationDbContext(_options);
        var repository = new UserRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.ExistsAsync(999);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task ExistsAsync_WithInactiveUser_ShouldReturnFalse()
    {
        // Arrange
        using var context = new ApplicationDbContext(_options);
        var repository = new UserRepository(context, _mockLogger.Object);

        var user = new User { Name = "Inactive User", Email = "inactive@test.com", IsActive = false };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.ExistsAsync(user.Id);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task SearchAsync_ShouldReturnMatchingUsersByName()
    {
        // Arrange
        using var context = new ApplicationDbContext(_options);
        var repository = new UserRepository(context, _mockLogger.Object);

        context.Users.AddRange(
            new User { Name = "John Doe", Email = "john@test.com", IsActive = true },
            new User { Name = "Jane Doe", Email = "jane@test.com", IsActive = true },
            new User { Name = "Bob Smith", Email = "bob@test.com", IsActive = true }
        );
        await context.SaveChangesAsync();

        // Act
        var result = await repository.SearchAsync("Doe");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task SearchAsync_ShouldReturnMatchingUsersByEmail()
    {
        // Arrange
        using var context = new ApplicationDbContext(_options);
        var repository = new UserRepository(context, _mockLogger.Object);

        context.Users.AddRange(
            new User { Name = "User1", Email = "admin@test.com", IsActive = true },
            new User { Name = "User2", Email = "user@example.com", IsActive = true }
        );
        await context.SaveChangesAsync();

        // Act
        var result = await repository.SearchAsync("admin");

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
    }

    [Fact]
    public async Task SearchAsync_ShouldReturnMatchingUsersByDescription()
    {
        // Arrange
        using var context = new ApplicationDbContext(_options);
        var repository = new UserRepository(context, _mockLogger.Object);

        context.Users.AddRange(
            new User { Name = "User1", Email = "user1@test.com", Description = "Manager", IsActive = true },
            new User { Name = "User2", Email = "user2@test.com", Description = "Employee", IsActive = true }
        );
        await context.SaveChangesAsync();

        // Act
        var result = await repository.SearchAsync("Manager");

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
    }

    [Fact]
    public async Task SearchAsync_WithNoMatches_ShouldReturnEmptyList()
    {
        // Arrange
        using var context = new ApplicationDbContext(_options);
        var repository = new UserRepository(context, _mockLogger.Object);

        context.Users.Add(new User { Name = "User1", Email = "user1@test.com", IsActive = true });
        await context.SaveChangesAsync();

        // Act
        var result = await repository.SearchAsync("NoMatch");

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task SearchAsync_ShouldNotReturnInactiveUsers()
    {
        // Arrange
        using var context = new ApplicationDbContext(_options);
        var repository = new UserRepository(context, _mockLogger.Object);

        context.Users.AddRange(
            new User { Name = "John Doe", Email = "john@test.com", IsActive = true },
            new User { Name = "Jane Doe", Email = "jane@test.com", IsActive = false }
        );
        await context.SaveChangesAsync();

        // Act
        var result = await repository.SearchAsync("Doe");

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
    }

    [Fact]
    public async Task GetByEmailAsync_WithValidEmail_ShouldReturnUser()
    {
        // Arrange
        using var context = new ApplicationDbContext(_options);
        var repository = new UserRepository(context, _mockLogger.Object);

        var user = new User { Name = "Test User", Email = "test@test.com", IsActive = true };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetByEmailAsync("test@test.com");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("test@test.com", result.Email);
    }

    [Fact]
    public async Task GetByEmailAsync_WithInvalidEmail_ShouldReturnNull()
    {
        // Arrange
        using var context = new ApplicationDbContext(_options);
        var repository = new UserRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.GetByEmailAsync("nonexistent@test.com");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByEmailAsync_WithInactiveUser_ShouldReturnNull()
    {
        // Arrange
        using var context = new ApplicationDbContext(_options);
        var repository = new UserRepository(context, _mockLogger.Object);

        var user = new User { Name = "Inactive User", Email = "inactive@test.com", IsActive = false };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetByEmailAsync("inactive@test.com");

        // Assert
        Assert.Null(result);
    }
}
