using EER.Domain.Entities;
using EER.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EER.Persistence.EFCore.Configuration;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> entity)
    {
        entity.HasKey(e => e.Id).HasName("PK__User__3214EC07CD827C32");

        entity.ToTable("User", "Identity");

        entity.HasIndex(e => e.Email, "UQ_User_Email").IsUnique();

        entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
        entity.Property(e => e.CreatedAt)
            .HasPrecision(2)
            .HasDefaultValueSql("(getutcdate())");
        entity.Property(e => e.Email).HasMaxLength(150);
        entity.Property(e => e.FullName).HasMaxLength(150);
        entity.Property(e => e.PasswordHash).HasMaxLength(64);
        entity.Property(e => e.UpdatedAt)
            .HasPrecision(2)
            .HasDefaultValueSql("(getutcdate())");

        entity.Property(e => e.UserRole)
            .HasConversion(
                v => v.ToString(),            // enum → in DB (string)
                v => Enum.Parse<Role>(v))    // DB (string) → enum
            .HasMaxLength(255);
    }
}
