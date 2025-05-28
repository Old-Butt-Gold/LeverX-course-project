using EER.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EER.Persistence.EFCore.Configuration;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> entity)
    {
        entity.HasKey(e => e.Id).HasName("PK__Category__3214EC07E13A5348");

        entity.ToTable("Category", "Supplies");

        entity.HasIndex(e => e.Slug, "UQ_Category_Slug").IsUnique();

        entity.Property(e => e.CreatedAt)
            .HasPrecision(2)
            .HasDefaultValueSql("(getutcdate())");
        entity.Property(e => e.Description).HasMaxLength(300);
        entity.Property(e => e.Name).HasMaxLength(100);
        entity.Property(e => e.Slug).HasMaxLength(100);
        entity.Property(e => e.UpdatedAt)
            .HasPrecision(2)
            .HasDefaultValueSql("(getutcdate())");
    }
}
