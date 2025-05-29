using EER.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EER.Persistence.EFCore.Configuration;

public class RentalItemConfiguration : IEntityTypeConfiguration<RentalItem>
{
    public void Configure(EntityTypeBuilder<RentalItem> entity)
    {
        entity.HasKey(e => new { e.RentalId, e.EquipmentItemId }).HasName("PK__RentalIt__D6B293460B611CF7");

        entity.ToTable("RentalItem", "Supplies", tb => tb.HasTrigger("TRG_PreventDoubleBooking"));

        entity.HasIndex(e => e.EquipmentItemId, "FK_RentalItem_EquipmentItemId");

        entity.Property(e => e.ActualPrice).HasColumnType("decimal(8, 2)");
        entity.Property(e => e.CreatedAt)
            .HasPrecision(2)
            .HasDefaultValueSql("(getutcdate())");

        entity.HasOne(d => d.EquipmentItem).WithMany(p => p.RentalItems)
            .HasForeignKey(d => d.EquipmentItemId)
            .HasConstraintName("FK__RentalIte__Equip__787EE5A0");

        entity.HasOne(d => d.Rental).WithMany(p => p.RentalItems)
            .HasForeignKey(d => d.RentalId)
            .HasConstraintName("FK__RentalIte__Renta__778AC167");
    }
}
