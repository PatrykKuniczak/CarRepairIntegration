using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

#nullable disable

namespace Infrastructure.Persistence.Migrations;

[DbContext(typeof(AppDbContext))]
partial class AppDbContextModelSnapshot : ModelSnapshot
{
    protected override void BuildModel(ModelBuilder modelBuilder)
    {
        modelBuilder.HasAnnotation("ProductVersion", "10.0.11");

        modelBuilder.Entity("Domain.Entities.Car", b =>
        {
            b.Property<Guid>("Id").HasColumnType("TEXT");
            b.Property<string>("Brand").IsRequired().HasMaxLength(100).HasColumnType("TEXT");
            b.Property<string>("Model").IsRequired().HasMaxLength(100).HasColumnType("TEXT");
            b.Property<decimal>("EnginePower").HasPrecision(18, 2).HasColumnType("TEXT");
            b.Property<string>("EnginePowerUnit").IsRequired().HasMaxLength(10).HasColumnType("TEXT");
            b.Property<string>("Color").IsRequired().HasMaxLength(50).HasColumnType("TEXT");
            b.HasKey("Id");
            b.ToTable("Cars");
        });

        modelBuilder.Entity("Domain.Entities.CarRepair", b =>
        {
            b.Property<Guid>("Id").HasColumnType("TEXT");
            b.Property<Guid>("CarId").HasColumnType("TEXT");
            b.Property<string>("Description").IsRequired().HasMaxLength(500).HasColumnType("TEXT");
            b.Property<DateTime>("RepairDate").HasColumnType("TEXT");
            b.Property<decimal>("Cost").HasPrecision(18, 2).HasColumnType("TEXT");
            b.Property<string>("ServiceName").IsRequired().HasMaxLength(200).HasColumnType("TEXT");
            b.HasKey("Id");
            b.HasIndex("CarId");
            b.ToTable("CarRepairs");
        });

        modelBuilder.Entity("Domain.Entities.CarRepair", b =>
        {
            b.HasOne("Domain.Entities.Car", null)
                .WithMany()
                .HasForeignKey("CarId")
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
        });
    }
}
