using EER.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EER.Persistence.EFCore.Configuration;

public class OfficeConfiguration : IEntityTypeConfiguration<Office>
{
    public void Configure(EntityTypeBuilder<Office> entity)
    {
        entity.HasKey(e => e.Id).HasName("PK__Office__3214EC07B33694A9");

        entity.Property(e => e.Id)
            .ValueGeneratedOnAdd()
            .Metadata.SetBeforeSaveBehavior(PropertySaveBehavior.Ignore);

        entity.ToTable("Office", "Identity");

        entity.HasIndex(e => e.OwnerId, "FK_Office_OwnerId");

        entity.HasIndex(e => new { e.City, e.Country }, "IX_Office_City_Country");

        entity.Property(e => e.Address).HasMaxLength(150);
        entity.Property(e => e.City).HasMaxLength(100);
        entity.Property(e => e.Country).HasMaxLength(100);
        entity.Property(e => e.CreatedAt)
            .HasPrecision(2)
            .HasDefaultValueSql("(getutcdate())");
        entity.Property(e => e.IsActive).HasDefaultValue(true);
        entity.Property(e => e.UpdatedAt)
            .HasPrecision(2)
            .HasDefaultValueSql("(getutcdate())");

        entity.HasOne(d => d.Owner).WithMany(p => p.Offices)
            .HasForeignKey(d => d.OwnerId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK__Office__OwnerId__6E01572D");
    }
}
