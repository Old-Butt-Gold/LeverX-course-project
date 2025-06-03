using EER.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EER.Persistence.EFCore.Configuration;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> entity)
    {
        entity.HasKey(e => e.Id).HasName("PK__RefreshT__3214EC07B1623BFC");

        entity.ToTable("RefreshToken", "Identity");

        entity.HasIndex(e => e.Token, "UQ_RefreshToken_Token").IsUnique();

        entity.Property(e => e.CreatedAt)
            .HasPrecision(2)
            .HasDefaultValueSql("(getutcdate())");
        entity.Property(e => e.ExpiresAt).HasPrecision(2);
        entity.Property(e => e.RevokedAt).HasPrecision(2);
        entity.Property(e => e.Token).HasMaxLength(88);

        entity.HasOne(d => d.User).WithMany(p => p.RefreshTokens)
            .HasForeignKey(d => d.UserId)
            .HasConstraintName("FK__RefreshTo__UserI__06CD04F7");
    }
}
