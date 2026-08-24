using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Seeding;

public sealed class DbSeeder(AppDbContext db)
{
    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        if (await db.Cars.AnyAsync(cancellationToken))
            return;

        var carId = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6");

        db.Cars.Add(new Car
        {
            Id = carId,
            Brand = "Audi",
            Model = "A3",
            EnginePower = 115,
            EnginePowerUnit = "km",
            Color = "red"
        });

        db.CarRepairs.Add(new CarRepair
        {
            Id = Guid.Parse("5fa85f64-5717-4562-b3fc-2c963f66afa6"),
            CarId = carId,
            Description = "Wymiana oleju i filtrów",
            RepairDate = new DateTime(2026, 8, 23, 12, 0, 0, DateTimeKind.Utc),
            Cost = 450,
            ServiceName = "AutoService"
        });

        await db.SaveChangesAsync(cancellationToken);
    }
}
