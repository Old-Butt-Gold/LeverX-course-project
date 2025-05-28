using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EER.Persistence.EFCore.Configuration;

public class MigrationHistoryConfiguration : IEntityTypeConfiguration<MigrationHistory>
{
    public void Configure(EntityTypeBuilder<MigrationHistory> entity)
    {
        entity.HasKey(e => e.MigrationId).HasName("PK__Migratio__E5D3573B9FD6F605");

        entity.ToTable("MigrationHistory");

        entity.Property(e => e.MigrationId).HasMaxLength(150);
    }
}
