using Api.GraphQL.DTOs;
using Application.CQRS;
using Application.DTOs;

namespace Api.GraphQL;

public sealed class CarQuery
{
    // GraphQL -> CQRS Query -> Dapper -> lightweight DTO.
    public Task<IReadOnlyList<GetCar>> GetCars(
        GetCarsDto input,
        CarQueries queries,
        CancellationToken cancellationToken)
    {
        return queries.GetCarsAsync(new GetCarsQuery(input.Take), cancellationToken);
    }

    public Task<GetCar?> GetCar(
        Guid id,
        CarQueries queries,
        CancellationToken cancellationToken)
    {
        return queries.GetCarAsync(new GetCarQuery(id), cancellationToken);
    }
}