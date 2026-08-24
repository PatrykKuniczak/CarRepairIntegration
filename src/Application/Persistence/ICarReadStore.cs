using Application.DTOs;

namespace Application.Persistence;

// Application owns the port; Infrastructure supplies the Dapper implementation.
public interface ICarReadStore
{
    Task<IReadOnlyList<GetCar>> GetCarsAsync(int take, CancellationToken cancellationToken);
    Task<GetCar?> GetCarAsync(Guid id, CancellationToken cancellationToken);
}
