using Xunit;
using Microsoft.EntityFrameworkCore;
using DescopeSampleApp.Infrastructure.Data;
using DescopeSampleApp.Infrastructure.Data.Configurations;
using DescopeSampleApp.Domain.Entities;

namespace DescopeSampleApp.Infrastructure.Tests;

public class UserConfigurationTests
{
    private DbContextOptions<ApplicationDbContext> CreateOptions()
    {
        return new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    [Fact]
    public void Configure_ShouldSetTableName()
    {
        // Arrange
        var options = CreateOptions();
        using var context = new ApplicationDbContext(options);

        // Act
        var entityType = context.Model.FindEntityType(typeof(User));

        // Assert
        Assert.NotNull(entityType);
        Assert.Equal("Users", entityType.GetTableName());
    }

    [Fact]
    public void Configure_ShouldSetPrimaryKey()
    {
        // Arrange
        var options = CreateOptions();
        using var context = new ApplicationDbContext(options);

        // Act
        var entityType = context.Model.FindEntityType(typeof(User));
        var primaryKey = entityType?.FindPrimaryKey();

        // Assert
        Assert.NotNull(primaryKey);
        Assert.Single(primaryKey.Properties);
        Assert.Equal("Id", primaryKey.Properties.First().Name);
    }

    [Fact]
    public void Configure_NameProperty_ShouldBeRequired()
    {
        // Arrange
        var options = CreateOptions();
        using var context = new ApplicationDbContext(options);

        // Act
        var entityType = context.Model.FindEntityType(typeof(User));
        var nameProperty = entityType?.FindProperty("Name");

        // Assert
        Assert.NotNull(nameProperty);
        Assert.False(nameProperty.IsNullable);
    }

    [Fact]
    public void Configure_NameProperty_ShouldHaveMaxLength()
    {
        // Arrange
        var options = CreateOptions();
        using var context = new ApplicationDbContext(options);

        // Act
        var entityType = context.Model.FindEntityType(typeof(User));
        var nameProperty = entityType?.FindProperty("Name");

        // Assert
        Assert.NotNull(nameProperty);
        Assert.Equal(200, nameProperty.GetMaxLength());
    }

    [Fact]
    public void Configure_DescriptionProperty_ShouldHaveMaxLength()
    {
        // Arrange
        var options = CreateOptions();
        using var context = new ApplicationDbContext(options);

        // Act
        var entityType = context.Model.FindEntityType(typeof(User));
        var descriptionProperty = entityType?.FindProperty("Description");

        // Assert
        Assert.NotNull(descriptionProperty);
        Assert.Equal(500, descriptionProperty.GetMaxLength());
    }

    [Fact]
    public void Configure_EmailProperty_ShouldBeRequired()
    {
        // Arrange
        var options = CreateOptions();
        using var context = new ApplicationDbContext(options);

        // Act
        var entityType = context.Model.FindEntityType(typeof(User));
        var emailProperty = entityType?.FindProperty("Email");

        // Assert
        Assert.NotNull(emailProperty);
        Assert.False(emailProperty.IsNullable);
    }

    [Fact]
    public void Configure_EmailProperty_ShouldHaveMaxLength()
    {
        // Arrange
        var options = CreateOptions();
        using var context = new ApplicationDbContext(options);

        // Act
        var entityType = context.Model.FindEntityType(typeof(User));
        var emailProperty = entityType?.FindProperty("Email");

        // Assert
        Assert.NotNull(emailProperty);
        Assert.Equal(200, emailProperty.GetMaxLength());
    }

    [Fact]
    public void Configure_EmailProperty_ShouldHaveUniqueIndex()
    {
        // Arrange
        var options = CreateOptions();
        using var context = new ApplicationDbContext(options);

        // Act
        var entityType = context.Model.FindEntityType(typeof(User));
        var emailIndex = entityType?.GetIndexes().FirstOrDefault(i => i.Properties.Any(p => p.Name == "Email"));

        // Assert
        Assert.NotNull(emailIndex);
        Assert.True(emailIndex.IsUnique);
    }

    [Fact]
    public void Configure_CreatedByProperty_ShouldBeRequired()
    {
        // Arrange
        var options = CreateOptions();
        using var context = new ApplicationDbContext(options);

        // Act
        var entityType = context.Model.FindEntityType(typeof(User));
        var createdByProperty = entityType?.FindProperty("CreatedBy");

        // Assert
        Assert.NotNull(createdByProperty);
        Assert.False(createdByProperty.IsNullable);
    }

    [Fact]
    public void Configure_CreatedByProperty_ShouldHaveMaxLength()
    {
        // Arrange
        var options = CreateOptions();
        using var context = new ApplicationDbContext(options);

        // Act
        var entityType = context.Model.FindEntityType(typeof(User));
        var createdByProperty = entityType?.FindProperty("CreatedBy");

        // Assert
        Assert.NotNull(createdByProperty);
        Assert.Equal(200, createdByProperty.GetMaxLength());
    }

    [Fact]
    public void Configure_ModifiedByProperty_ShouldHaveMaxLength()
    {
        // Arrange
        var options = CreateOptions();
        using var context = new ApplicationDbContext(options);

        // Act
        var entityType = context.Model.FindEntityType(typeof(User));
        var modifiedByProperty = entityType?.FindProperty("ModifiedBy");

        // Assert
        Assert.NotNull(modifiedByProperty);
        Assert.Equal(200, modifiedByProperty.GetMaxLength());
    }

    [Fact]
    public void Configure_DescopeUserIdProperty_ShouldHaveMaxLength()
    {
        // Arrange
        var options = CreateOptions();
        using var context = new ApplicationDbContext(options);

        // Act
        var entityType = context.Model.FindEntityType(typeof(User));
        var descopeUserIdProperty = entityType?.FindProperty("DescopeUserId");

        // Assert
        Assert.NotNull(descopeUserIdProperty);
        Assert.Equal(200, descopeUserIdProperty.GetMaxLength());
    }

    [Fact]
    public void UserConfiguration_ShouldImplementIEntityTypeConfiguration()
    {
        // Arrange
        var configuration = new UserConfiguration();

        // Assert
        Assert.IsAssignableFrom<IEntityTypeConfiguration<User>>(configuration);
    }

    [Fact]
    public void Configure_ShouldNotThrowException()
    {
        // Arrange
        var options = CreateOptions();

        // Act & Assert
        var exception = Record.Exception(() => new ApplicationDbContext(options));
        Assert.Null(exception);
    }
}
