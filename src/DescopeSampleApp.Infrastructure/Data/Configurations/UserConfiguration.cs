using DescopeSampleApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DescopeSampleApp.Infrastructure.Data.Configurations;

/// <summary>
/// Entity configuration for User entity
/// </summary>
public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(u => u.Description)
            .HasMaxLength(500);

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(u => u.CreatedBy)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(u => u.ModifiedBy)
            .HasMaxLength(200);

        builder.Property(u => u.DescopeUserId)
            .HasMaxLength(200);

        builder.Property(u => u.IsActive)
            .HasDefaultValue(true);

        builder.Property(u => u.CreatedDate)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.HasIndex(u => u.Email)
            .IsUnique();
    }
}
