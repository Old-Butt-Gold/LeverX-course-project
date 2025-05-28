using EER.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EER.Persistence.EFCore.Configuration;

public class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> entity)
    {
        entity.HasKey(e => new { e.CustomerId, e.EquipmentId }).HasName("PK__Review__27EA109FBD41FCEF");

        entity.ToTable("Review", "Critique", tb =>
        {
            tb.HasTrigger("TRG_Review_AfterDelete");
            tb.HasTrigger("TRG_Review_AfterInsert");
            tb.HasTrigger("TRG_Review_AfterUpdate");
        });

        entity.HasIndex(e => e.CreatedAt, "IX_Review_CreatedAt");

        entity.HasIndex(e => e.Rating, "IX_Review_Rating");

        entity.Property(e => e.Comment).HasMaxLength(1000);
        entity.Property(e => e.CreatedAt)
            .HasPrecision(2)
            .HasDefaultValueSql("(getutcdate())");
        entity.Property(e => e.UpdatedAt)
            .HasPrecision(2)
            .HasDefaultValueSql("(getutcdate())");

        entity.HasOne(d => d.Customer).WithMany(p => p.Reviews)
            .HasForeignKey(d => d.CustomerId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK__Review__Customer__797309D9");

        entity.HasOne(d => d.Equipment).WithMany(p => p.Reviews)
            .HasForeignKey(d => d.EquipmentId)
            .HasConstraintName("FK__Review__Equipmen__7A672E12");
    }
}
