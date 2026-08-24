using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Car> Cars => Set<Car>();
    public DbSet<CarRepair> CarRepairs => Set<CarRepair>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Car>(car =>
        {
            car.HasKey(x => x.Id);
            car.Property(x => x.Brand).HasMaxLength(100).IsRequired();
            car.Property(x => x.Model).HasMaxLength(100).IsRequired();
            car.Property(x => x.EnginePower).HasPrecision(18, 2);
            car.Property(x => x.EnginePowerUnit).HasMaxLength(10).IsRequired();
            car.Property(x => x.Color).HasMaxLength(50).IsRequired();
        });

        modelBuilder.Entity<CarRepair>(repair =>
        {
            repair.HasKey(x => x.Id);
            repair.Property(x => x.Description).HasMaxLength(500).IsRequired();
            repair.Property(x => x.Cost).HasPrecision(18, 2);
            repair.Property(x => x.ServiceName).HasMaxLength(200).IsRequired();

            repair.HasOne<Car>()
                .WithMany()
                .HasForeignKey(x => x.CarId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
