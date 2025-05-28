using EER.Domain.Entities;
using EER.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EER.Persistence.EFCore.Configuration;

public class RentalConfiguration : IEntityTypeConfiguration<Rental>
{
    public void Configure(EntityTypeBuilder<Rental> entity)
    {
        entity.HasKey(e => e.Id).HasName("PK__Rental__3214EC074B5974FE");

        entity.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        entity.ToTable("Rental", "Supplies", tb => tb.HasTrigger("TRG_RentalStatus_AfterUpdate"));

        entity.HasIndex(e => e.CustomerId, "FK_Rental_CustomerId");

        entity.HasIndex(e => new { e.OwnerId, e.Status, e.CreatedAt }, "FK_Rental_OwnerId_Status_CreatedAt");

        entity.HasIndex(e => new { e.StartDate, e.EndDate }, "IX_Rental_Dates");

        entity.Property(e => e.CreatedAt)
            .HasPrecision(2)
            .HasDefaultValueSql("(getutcdate())");
        entity.Property(e => e.EndDate).HasPrecision(0);
        entity.Property(e => e.StartDate).HasPrecision(0);

        entity.Property(e => e.Status)
            .HasConversion(
                v => v.ToString(),            // enum → in DB (string)
                v => Enum.Parse<RentalStatus>(v))    // DB (string) → enum
            .HasMaxLength(255)
            .HasDefaultValue(RentalStatus.Pending);

        entity.Property(e => e.TotalPrice).HasColumnType("decimal(9, 2)");
        entity.Property(e => e.UpdatedAt)
            .HasPrecision(2)
            .HasDefaultValueSql("(getutcdate())");

        entity.HasOne(d => d.Customer).WithMany(p => p.RentalCustomers)
            .HasForeignKey(d => d.CustomerId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK__Rental__Customer__75A278F5");

        entity.HasOne(d => d.Owner).WithMany(p => p.RentalOwners)
            .HasForeignKey(d => d.OwnerId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK__Rental__OwnerId__76969D2E");
    }
}
