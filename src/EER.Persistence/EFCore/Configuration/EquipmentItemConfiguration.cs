using EER.Domain.Entities;
using EER.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EER.Persistence.EFCore.Configuration;

public class EquipmentItemConfiguration : IEntityTypeConfiguration<EquipmentItem>
{
    public void Configure(EntityTypeBuilder<EquipmentItem> entity)
    {
        entity.HasKey(e => e.Id).HasName("PK__Equipmen__3214EC07DD244325");

        entity.Property(e => e.Id)
            .ValueGeneratedOnAdd()
            .Metadata.SetBeforeSaveBehavior(PropertySaveBehavior.Ignore);

        entity.ToTable("EquipmentItem", "Supplies");

        entity.HasIndex(e => e.OfficeId, "FK_EquipmentItem_OfficeId");

        entity.HasIndex(e => new { e.EquipmentId, e.ItemStatus }, "IX_EquipmentItem_EquipmentId_Status");

        entity.HasIndex(e => new { e.SerialNumber, e.EquipmentId }, "UQ_EquipmentItem_Serial").IsUnique();

        entity.Property(e => e.CreatedAt)
            .HasPrecision(2)
            .HasDefaultValueSql("(getutcdate())");

        entity.Property(e => e.ItemStatus)
            .HasConversion(
                v => v.ToString(),            // enum → in DB (string)
                v => Enum.Parse<ItemStatus>(v))    // DB (string) → enum
            .HasMaxLength(255)
            .HasDefaultValue(ItemStatus.Available);

        entity.Property(e => e.SerialNumber).HasMaxLength(100);
        entity.Property(e => e.UpdatedAt)
            .HasPrecision(2)
            .HasDefaultValueSql("(getutcdate())");

        entity.HasOne(d => d.Equipment).WithMany(p => p.EquipmentItems)
            .HasForeignKey(d => d.EquipmentId)
            .HasConstraintName("FK__Equipment__Equip__72C60C4A");

        entity.HasOne(d => d.Office).WithMany(p => p.EquipmentItems)
            .HasForeignKey(d => d.OfficeId)
            .OnDelete(DeleteBehavior.SetNull)
            .HasConstraintName("FK__Equipment__Offic__73BA3083");
    }
}
