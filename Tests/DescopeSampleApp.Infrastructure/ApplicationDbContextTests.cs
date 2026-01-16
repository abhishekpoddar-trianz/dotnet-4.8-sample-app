using Xunit;
using Microsoft.EntityFrameworkCore;
using DescopeSampleApp.Infrastructure.Data;
using DescopeSampleApp.Domain.Entities;

namespace DescopeSampleApp.Infrastructure.Tests;

public class ApplicationDbContextTests
{
    private DbContextOptions<ApplicationDbContext> CreateOptions()
    {
        return new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    [Fact]
    public void Constructor_WithValidOptions_ShouldInitialize()
    {
        // Arrange
        var options = CreateOptions();

        // Act
        using var context = new ApplicationDbContext(options);

        // Assert
        Assert.NotNull(context);
    }

    [Fact]
    public void Users_DbSet_ShouldBeInitialized()
    {
        // Arrange
        var options = CreateOptions();

        // Act
        using var context = new ApplicationDbContext(options);

        // Assert
        Assert.NotNull(context.Users);
    }

    [Fact]
    public void Users_CanAddEntity()
    {
        // Arrange
        var options = CreateOptions();
        using var context = new ApplicationDbContext(options);
        var user = new User
        {
            Name = "Test User",
            Email = "test@test.com",
            IsActive = true,
            CreatedDate = DateTime.UtcNow,
            CreatedBy = "System"
        };

        // Act
        context.Users.Add(user);
        context.SaveChanges();

        // Assert
        Assert.NotEqual(0, user.Id);
    }

    [Fact]
    public void Users_CanRetrieveEntity()
    {
        // Arrange
        var options = CreateOptions();
        using (var context = new ApplicationDbContext(options))
        {
            var user = new User
            {
                Name = "Test User",
                Email = "test@test.com",
                IsActive = true,
                CreatedDate = DateTime.UtcNow,
                CreatedBy = "System"
            };
            context.Users.Add(user);
            context.SaveChanges();
        }

        // Act & Assert
        using (var context = new ApplicationDbContext(options))
        {
            var retrievedUser = context.Users.FirstOrDefault(u => u.Name == "Test User");
            Assert.NotNull(retrievedUser);
            Assert.Equal("test@test.com", retrievedUser.Email);
        }
    }

    [Fact]
    public void Users_CanUpdateEntity()
    {
        // Arrange
        var options = CreateOptions();
        int userId;
        using (var context = new ApplicationDbContext(options))
        {
            var user = new User
            {
                Name = "Test User",
                Email = "test@test.com",
                IsActive = true,
                CreatedDate = DateTime.UtcNow,
                CreatedBy = "System"
            };
            context.Users.Add(user);
            context.SaveChanges();
            userId = user.Id;
        }

        // Act
        using (var context = new ApplicationDbContext(options))
        {
            var user = context.Users.Find(userId);
            Assert.NotNull(user);
            user.Name = "Updated User";
            context.SaveChanges();
        }

        // Assert
        using (var context = new ApplicationDbContext(options))
        {
            var user = context.Users.Find(userId);
            Assert.NotNull(user);
            Assert.Equal("Updated User", user.Name);
        }
    }

    [Fact]
    public void Users_CanDeleteEntity()
    {
        // Arrange
        var options = CreateOptions();
        int userId;
        using (var context = new ApplicationDbContext(options))
        {
            var user = new User
            {
                Name = "Test User",
                Email = "test@test.com",
                IsActive = true,
                CreatedDate = DateTime.UtcNow,
                CreatedBy = "System"
            };
            context.Users.Add(user);
            context.SaveChanges();
            userId = user.Id;
        }

        // Act
        using (var context = new ApplicationDbContext(options))
        {
            var user = context.Users.Find(userId);
            Assert.NotNull(user);
            context.Users.Remove(user);
            context.SaveChanges();
        }

        // Assert
        using (var context = new ApplicationDbContext(options))
        {
            var user = context.Users.Find(userId);
            Assert.Null(user);
        }
    }

    [Fact]
    public async Task Users_CanAddEntityAsync()
    {
        // Arrange
        var options = CreateOptions();
        using var context = new ApplicationDbContext(options);
        var user = new User
        {
            Name = "Async Test User",
            Email = "async@test.com",
            IsActive = true,
            CreatedDate = DateTime.UtcNow,
            CreatedBy = "System"
        };

        // Act
        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();

        // Assert
        Assert.NotEqual(0, user.Id);
    }

    [Fact]
    public async Task Users_CanRetrieveMultipleEntities()
    {
        // Arrange
        var options = CreateOptions();
        using (var context = new ApplicationDbContext(options))
        {
            context.Users.AddRange(
                new User { Name = "User1", Email = "user1@test.com", IsActive = true, CreatedDate = DateTime.UtcNow, CreatedBy = "System" },
                new User { Name = "User2", Email = "user2@test.com", IsActive = true, CreatedDate = DateTime.UtcNow, CreatedBy = "System" },
                new User { Name = "User3", Email = "user3@test.com", IsActive = true, CreatedDate = DateTime.UtcNow, CreatedBy = "System" }
            );
            await context.SaveChangesAsync();
        }

        // Act
        using (var context = new ApplicationDbContext(options))
        {
            var users = await context.Users.ToListAsync();

            // Assert
            Assert.Equal(3, users.Count);
        }
    }

    [Fact]
    public void Users_CanQueryWithLinq()
    {
        // Arrange
        var options = CreateOptions();
        using (var context = new ApplicationDbContext(options))
        {
            context.Users.AddRange(
                new User { Name = "Active User", Email = "active@test.com", IsActive = true, CreatedDate = DateTime.UtcNow, CreatedBy = "System" },
                new User { Name = "Inactive User", Email = "inactive@test.com", IsActive = false, CreatedDate = DateTime.UtcNow, CreatedBy = "System" }
            );
            context.SaveChanges();
        }

        // Act
        using (var context = new ApplicationDbContext(options))
        {
            var activeUsers = context.Users.Where(u => u.IsActive).ToList();

            // Assert
            Assert.Single(activeUsers);
            Assert.Equal("Active User", activeUsers[0].Name);
        }
    }

    [Fact]
    public void Context_CanBeDisposed()
    {
        // Arrange
        var options = CreateOptions();
        var context = new ApplicationDbContext(options);

        // Act & Assert
        context.Dispose();
        Assert.True(true); // If we reach here, dispose worked
    }
}
