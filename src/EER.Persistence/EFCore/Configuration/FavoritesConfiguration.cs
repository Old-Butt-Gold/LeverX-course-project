using EER.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EER.Persistence.EFCore.Configuration;

public class FavoritesConfiguration : IEntityTypeConfiguration<Favorites>
{
    public void Configure(EntityTypeBuilder<Favorites> entity)
    {
        entity.HasKey(e => new { e.UserId, e.EquipmentId }).HasName("PK__Favorite__94CCB80B68512D83");

        entity.ToTable("Favorites", "Identity");

        entity.HasIndex(e => e.CreatedAt, "IX_Favorites_CreatedAt");

        entity.Property(e => e.CreatedAt)
            .HasPrecision(2)
            .HasDefaultValueSql("(getutcdate())");

        entity.HasOne(d => d.Equipment).WithMany(p => p.Favorites)
            .HasForeignKey(d => d.EquipmentId)
            .HasConstraintName("FK__Favorites__Equip__6EF57B66");

        entity.HasOne(d => d.User).WithMany(p => p.Favorites)
            .HasForeignKey(d => d.UserId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK__Favorites__UserI__6FE99F9F");
    }
}
