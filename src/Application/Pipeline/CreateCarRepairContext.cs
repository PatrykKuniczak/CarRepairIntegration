using System.ComponentModel.DataAnnotations;
using Domain.Entities;

namespace Application.Pipeline;

// One context travels through the Pipe & Filter stages.
// A filter changes only the part of the context it is responsible for.
public sealed class CreateCarRepairContext
{
    public required string Source { get; init; }
    public required string Payload { get; init; }
    public string RuleSet { get; init; } = "standard";
    public required RepairInput Repair { get; init; }

    public IncomingCarData? Input { get; set; }
    public Car? Car { get; set; }
    public CarRepair? PreparedRepair { get; set; }
    public Guid RepairId { get; set; }
}

public sealed record IncomingCarData(
    string Brand,
    string Model,
    decimal EnginePower,
    string EnginePowerUnit,
    string Color);

public sealed record RepairInput(
    [property: Required] [property: MinLength(1)] string Description,
    [property: Required] DateTime RepairDate,
    [property: Range(0.01, 1000000)] decimal Cost,
    [property: Required] [property: MinLength(1)] string ServiceName);

public interface ICreateCarRepairFilter
{
    Task ApplyAsync(CreateCarRepairContext context, CancellationToken cancellationToken);
}
