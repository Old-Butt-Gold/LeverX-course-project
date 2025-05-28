using EER.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EER.Persistence.EFCore.Configuration;

public class EquipmentConfiguration : IEntityTypeConfiguration<Equipment>
{
    public void Configure(EntityTypeBuilder<Equipment> entity)
    {
        entity.HasKey(e => e.Id).HasName("PK__Equipmen__3214EC0734AC969E");

        entity.Property(e => e.Id)
            .ValueGeneratedOnAdd()
            .Metadata.SetBeforeSaveBehavior(PropertySaveBehavior.Ignore);

        entity.ToTable("Equipment", "Supplies", tb =>
        {
            tb.HasTrigger("TRG_Equipment_AfterDelete");
            tb.HasTrigger("TRG_Equipment_AfterInsert");
            tb.HasTrigger("TRG_Equipment_AfterUpdate");
        });

        entity.HasIndex(e => e.CategoryId, "FK_Equipment_CategoryId");

        entity.HasIndex(e => e.OwnerId, "FK_Equipment_OwnerId");

        entity.HasIndex(e => e.Name, "IX_Equipment_Name");

        entity.Property(e => e.TotalReviews)
            .Metadata.SetBeforeSaveBehavior(PropertySaveBehavior.Ignore);

        entity.Property(e => e.AverageRating)
            .HasColumnType("decimal(3, 2)")
            .Metadata.SetBeforeSaveBehavior(PropertySaveBehavior.Ignore);

        entity.Property(e => e.CreatedAt)
            .HasPrecision(2)
            .HasDefaultValueSql("(getutcdate())");
        entity.Property(e => e.Description).HasMaxLength(3000);
        entity.Property(e => e.Name).HasMaxLength(100);
        entity.Property(e => e.PricePerDay).HasColumnType("decimal(8, 2)");
        entity.Property(e => e.UpdatedAt)
            .HasPrecision(2)
            .HasDefaultValueSql("(getutcdate())");

        entity.HasOne(d => d.Category).WithMany(p => p.Equipment)
            .HasForeignKey(d => d.CategoryId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK__Equipment__Categ__70DDC3D8");

        entity.HasOne(d => d.Owner).WithMany(p => p.Equipment)
            .HasForeignKey(d => d.OwnerId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK__Equipment__Owner__71D1E811");
    }
}
