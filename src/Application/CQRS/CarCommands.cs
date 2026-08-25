using Application.Persistence;
using Application.Pipeline;
using HotChocolate;

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

        if (context.Car is null || context.PreparedRepair is null)
            throw new GraphQLException("Pipeline failed to create a car or prepare a repair.");

        await store.AddAsync(context.Car, context.PreparedRepair, cancellationToken);
        return new CreateCarRepairResult(context.Car.Id, context.PreparedRepair.Id);
    }

    public async Task<EditCarRepairResult> EditCarRepairAsync(
        EditCarRepairCommand command,
        CancellationToken cancellationToken)
    {
        if (command.Id == Guid.Empty)
            throw new GraphQLException("Repair Id is required.");

        var updated = await store.UpdateRepairAsync(command, cancellationToken);
        if (!updated)
            throw new GraphQLException($"Repair '{command.Id}' was not found.");

        return new EditCarRepairResult(command.Id);
    }
}
