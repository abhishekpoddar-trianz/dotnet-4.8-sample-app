using Xunit;
using DescopeSampleApp.Domain.Entities;

namespace DescopeSampleApp.Domain.Tests;

public class UserTests
{
    [Fact]
    public void User_Constructor_ShouldInitializeWithDefaultValues()
    {
        // Arrange & Act
        var user = new User();

        // Assert
        Assert.Equal(0, user.Id);
        Assert.Equal(string.Empty, user.Name);
        Assert.Null(user.Description);
        Assert.Equal(default(DateTime), user.CreatedDate);
        Assert.Null(user.ModifiedDate);
        Assert.False(user.IsActive);
        Assert.Equal(string.Empty, user.CreatedBy);
        Assert.Null(user.ModifiedBy);
        Assert.Equal(string.Empty, user.Email);
        Assert.Null(user.DescopeUserId);
    }

    [Fact]
    public void User_SetId_ShouldUpdateValue()
    {
        // Arrange
        var user = new User();

        // Act
        user.Id = 123;

        // Assert
        Assert.Equal(123, user.Id);
    }

    [Fact]
    public void User_SetName_ShouldUpdateValue()
    {
        // Arrange
        var user = new User();

        // Act
        user.Name = "John Doe";

        // Assert
        Assert.Equal("John Doe", user.Name);
    }

    [Fact]
    public void User_SetDescription_ShouldUpdateValue()
    {
        // Arrange
        var user = new User();

        // Act
        user.Description = "Test description";

        // Assert
        Assert.Equal("Test description", user.Description);
    }

    [Fact]
    public void User_SetDescription_ToNull_ShouldBeNull()
    {
        // Arrange
        var user = new User { Description = "Initial" };

        // Act
        user.Description = null;

        // Assert
        Assert.Null(user.Description);
    }

    [Fact]
    public void User_SetCreatedDate_ShouldUpdateValue()
    {
        // Arrange
        var user = new User();
        var date = new DateTime(2024, 1, 1);

        // Act
        user.CreatedDate = date;

        // Assert
        Assert.Equal(date, user.CreatedDate);
    }

    [Fact]
    public void User_SetModifiedDate_ShouldUpdateValue()
    {
        // Arrange
        var user = new User();
        var date = new DateTime(2024, 6, 15);

        // Act
        user.ModifiedDate = date;

        // Assert
        Assert.Equal(date, user.ModifiedDate);
    }

    [Fact]
    public void User_SetModifiedDate_ToNull_ShouldBeNull()
    {
        // Arrange
        var user = new User { ModifiedDate = DateTime.Now };

        // Act
        user.ModifiedDate = null;

        // Assert
        Assert.Null(user.ModifiedDate);
    }

    [Fact]
    public void User_SetIsActive_ToTrue_ShouldUpdateValue()
    {
        // Arrange
        var user = new User();

        // Act
        user.IsActive = true;

        // Assert
        Assert.True(user.IsActive);
    }

    [Fact]
    public void User_SetIsActive_ToFalse_ShouldUpdateValue()
    {
        // Arrange
        var user = new User { IsActive = true };

        // Act
        user.IsActive = false;

        // Assert
        Assert.False(user.IsActive);
    }

    [Fact]
    public void User_SetCreatedBy_ShouldUpdateValue()
    {
        // Arrange
        var user = new User();

        // Act
        user.CreatedBy = "Admin";

        // Assert
        Assert.Equal("Admin", user.CreatedBy);
    }

    [Fact]
    public void User_SetModifiedBy_ShouldUpdateValue()
    {
        // Arrange
        var user = new User();

        // Act
        user.ModifiedBy = "Moderator";

        // Assert
        Assert.Equal("Moderator", user.ModifiedBy);
    }

    [Fact]
    public void User_SetModifiedBy_ToNull_ShouldBeNull()
    {
        // Arrange
        var user = new User { ModifiedBy = "Admin" };

        // Act
        user.ModifiedBy = null;

        // Assert
        Assert.Null(user.ModifiedBy);
    }

    [Fact]
    public void User_SetEmail_ShouldUpdateValue()
    {
        // Arrange
        var user = new User();

        // Act
        user.Email = "test@example.com";

        // Assert
        Assert.Equal("test@example.com", user.Email);
    }

    [Fact]
    public void User_SetDescopeUserId_ShouldUpdateValue()
    {
        // Arrange
        var user = new User();

        // Act
        user.DescopeUserId = "descope123";

        // Assert
        Assert.Equal("descope123", user.DescopeUserId);
    }

    [Fact]
    public void User_SetDescopeUserId_ToNull_ShouldBeNull()
    {
        // Arrange
        var user = new User { DescopeUserId = "descope123" };

        // Act
        user.DescopeUserId = null;

        // Assert
        Assert.Null(user.DescopeUserId);
    }

    [Fact]
    public void User_SetAllProperties_ShouldRetainValues()
    {
        // Arrange
        var user = new User();
        var createdDate = new DateTime(2024, 1, 1);
        var modifiedDate = new DateTime(2024, 6, 15);

        // Act
        user.Id = 99;
        user.Name = "Jane Smith";
        user.Description = "Test user";
        user.CreatedDate = createdDate;
        user.ModifiedDate = modifiedDate;
        user.IsActive = true;
        user.CreatedBy = "System";
        user.ModifiedBy = "Admin";
        user.Email = "jane@example.com";
        user.DescopeUserId = "descope456";

        // Assert
        Assert.Equal(99, user.Id);
        Assert.Equal("Jane Smith", user.Name);
        Assert.Equal("Test user", user.Description);
        Assert.Equal(createdDate, user.CreatedDate);
        Assert.Equal(modifiedDate, user.ModifiedDate);
        Assert.True(user.IsActive);
        Assert.Equal("System", user.CreatedBy);
        Assert.Equal("Admin", user.ModifiedBy);
        Assert.Equal("jane@example.com", user.Email);
        Assert.Equal("descope456", user.DescopeUserId);
    }
}
