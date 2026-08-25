using System.ComponentModel.DataAnnotations;
using Application.DTOs;
using Application.Persistence;
using HotChocolate;

namespace Application.CQRS;

// Query side of CQRS: only reads data and returns lightweight DTOs.
public sealed record GetCarsQuery(
    [property: Required]
    [property: Range(1, int.MaxValue)]
    int Take = 50);

public sealed record GetCarQuery(Guid Id);

public sealed class CarQueries(ICarReadStore store)
{
    public Task<IReadOnlyList<GetCar>> GetCarsAsync(
        GetCarsQuery query,
        CancellationToken cancellationToken)
    {
        var validationContext = new ValidationContext(query);
        var validationResults = new List<ValidationResult>();

        if (!Validator.TryValidateObject(query, validationContext, validationResults, true))
        {
            var errorMessage = validationResults.First().ErrorMessage;
            throw new GraphQLException(errorMessage ?? "Query validation failed.");
        }

        return store.GetCarsAsync(query.Take, cancellationToken);
    }

    public Task<GetCar?> GetCarAsync(
        GetCarQuery query,
        CancellationToken cancellationToken)
    {
        return store.GetCarAsync(query.Id, cancellationToken);
    }
}