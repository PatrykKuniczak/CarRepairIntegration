using Application.CQRS;
using Application.Persistence;
using Domain.Entities;

namespace Infrastructure.Persistence;

public sealed class CarWriteStore(AppDbContext db) : ICarWriteStore
{
    public async Task AddAsync(Car car, CarRepair repair, CancellationToken cancellationToken)
    {
        db.Cars.Add(car);
        db.CarRepairs.Add(repair);
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> UpdateRepairAsync(
        EditCarRepairCommand command,
        CancellationToken cancellationToken)
    {
        var repair = await db.CarRepairs.FindAsync([command.Id], cancellationToken);
        if (repair is null)
            return false;

        repair.Description = command.Description;
        repair.RepairDate = command.RepairDate;
        repair.Cost = command.Cost;
        repair.ServiceName = command.ServiceName;

        await db.SaveChangesAsync(cancellationToken);
        return true;
    }
}