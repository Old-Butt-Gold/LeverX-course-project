using EER.Domain.Entities;
using EER.Persistence.EFCore.Configuration;
using Microsoft.EntityFrameworkCore;

namespace EER.Persistence.EFCore;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Equipment> Equipment { get; set; }

    public virtual DbSet<EquipmentImages> EquipmentImages { get; set; }

    public virtual DbSet<EquipmentItem> EquipmentItems { get; set; }

    public virtual DbSet<Favorites> Favorites { get; set; }

    public virtual DbSet<MigrationHistory> MigrationHistories { get; set; }

    public virtual DbSet<Office> Offices { get; set; }

    public virtual DbSet<Rental> Rentals { get; set; }

    public virtual DbSet<RentalItem> RentalItems { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new CategoryConfiguration());
        modelBuilder.ApplyConfiguration(new EquipmentConfiguration());
        modelBuilder.ApplyConfiguration(new EquipmentImagesConfiguration());
        modelBuilder.ApplyConfiguration(new EquipmentItemConfiguration());
        modelBuilder.ApplyConfiguration(new FavoritesConfiguration());
        modelBuilder.ApplyConfiguration(new MigrationHistoryConfiguration());
        modelBuilder.ApplyConfiguration(new OfficeConfiguration());
        modelBuilder.ApplyConfiguration(new RentalConfiguration());
        modelBuilder.ApplyConfiguration(new RentalItemConfiguration());
        modelBuilder.ApplyConfiguration(new ReviewConfiguration());
        modelBuilder.ApplyConfiguration(new UserConfiguration());

    }
}
