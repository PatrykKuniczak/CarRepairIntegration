using Application.Persistence;
using Application.Pipeline;

namespace Application.CQRS;

// Command side of CQRS: commands are the only application entry point that changes state.
public sealed record CreateCarRepairCommand(
    string Source,
    string Payload,
    string RuleSet,
    RepairInput Repair);

public sealed record CreateCarRepairResult(Guid CarId, Guid RepairId);

public sealed record EditCarRepairCommand(
    Guid Id,
    string Description,
    DateTime RepairDate,
    decimal Cost,
    string ServiceName);

public sealed record EditCarRepairResult(Guid RepairId);

public sealed class CarCommands(ICarWriteStore store, CreateCarRepairPipeline pipeline)
{
    public async Task<CreateCarRepairResult> CreateCarRepairAsync(
        CreateCarRepairCommand command,
        CancellationToken cancellationToken)
    {
        var context = new CreateCarRepairContext
        {
            Source = command.Source,
            Payload = command.Payload,
            RuleSet = command.RuleSet,
            Repair = command.Repair
        };

        await pipeline.RunAsync(context, cancellationToken);

        var car = context.Car ?? throw new InvalidOperationException("Pipeline did not create a car.");
        var repair = context.PreparedRepair
            ?? throw new InvalidOperationException("Pipeline did not prepare a repair.");

        await store.AddAsync(car, repair, cancellationToken);
        return new(car.Id, repair.Id);
    }

    public async Task<EditCarRepairResult> EditCarRepairAsync(
        EditCarRepairCommand command,
        CancellationToken cancellationToken)
    {
        await store.UpdateRepairAsync(command, cancellationToken);
        return new(command.Id);
    }
}
