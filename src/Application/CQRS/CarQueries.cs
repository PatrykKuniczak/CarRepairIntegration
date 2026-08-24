using Application.DTOs;
using Application.Persistence;

namespace Application.CQRS;

// Query side of CQRS: only reads data and returns lightweight DTOs.
public sealed record GetCarsQuery(int Take = 50);
public sealed record GetCarQuery(Guid Id);

public sealed class CarQueries(ICarReadStore store)
{
    public Task<IReadOnlyList<GetCar>> GetCarsAsync(
        GetCarsQuery query,
        CancellationToken cancellationToken) =>
        store.GetCarsAsync(query.Take, cancellationToken);

    public Task<GetCar?> GetCarAsync(
        GetCarQuery query,
        CancellationToken cancellationToken) =>
        store.GetCarAsync(query.Id, cancellationToken);
}
