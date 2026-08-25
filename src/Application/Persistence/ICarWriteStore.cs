using Application.CQRS;
using Domain.Entities;

namespace Application.Persistence;

// Separate write contract = CQRS + ISP. Infrastructure implements it with EF Core.
public interface ICarWriteStore
{
    Task AddAsync(Car car, CarRepair repair, CancellationToken cancellationToken);
    Task<bool> UpdateRepairAsync(EditCarRepairCommand command, CancellationToken cancellationToken);
}
