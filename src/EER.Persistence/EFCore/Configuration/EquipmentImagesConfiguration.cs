using EER.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EER.Persistence.EFCore.Configuration;

public class EquipmentImagesConfiguration : IEntityTypeConfiguration<EquipmentImages>
{
    public void Configure(EntityTypeBuilder<EquipmentImages> entity)
    {
        entity.HasKey(e => e.Id).HasName("PK__Equipmen__3214EC075365B074");

        entity.ToTable("EquipmentImages", "Supplies");

        entity.HasIndex(e => new { e.EquipmentId, e.DisplayOrder }, "UQ_EquipmentImages_Order").IsUnique();

        entity.Property(e => e.CreatedAt)
            .HasPrecision(0)
            .HasDefaultValueSql("(getutcdate())");
        entity.Property(e => e.ImageUrl).HasMaxLength(500);
        entity.Property(e => e.UpdatedAt)
            .HasPrecision(2)
            .HasDefaultValueSql("(getutcdate())");

        entity.HasOne(d => d.Equipment).WithMany(p => p.EquipmentImages)
            .HasForeignKey(d => d.EquipmentId)
            .HasConstraintName("FK__Equipment__Equip__74AE54BC");
    }
}
