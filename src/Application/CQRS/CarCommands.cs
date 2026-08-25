using System.ComponentModel.DataAnnotations;
using Application.Persistence;
using Application.Pipeline;
using HotChocolate;

namespace Application.CQRS;

// Command side of CQRS: commands are the only application entry point that changes state.
public sealed record CreateCarRepairCommand(
    [property: Required] [property: MinLength(1)] string Source,
    [property: Required] [property: MinLength(1)] string Payload,
    [property: Required] [property: MinLength(1)] string RuleSet,
    [property: Required] RepairInput Repair);

public sealed record CreateCarRepairResult(Guid CarId, Guid RepairId);

public sealed record EditCarRepairCommand(
    [property: Required] Guid Id,
    [property: Required] [property: MinLength(1)] string Description,
    [property: Required] DateTime RepairDate,
    [property: Range(0.01, 1000000)] decimal Cost,
    [property: Required] [property: MinLength(1)] string ServiceName);

public sealed record EditCarRepairResult(Guid RepairId);

public sealed class CarCommands(ICarWriteStore store, CreateCarRepairPipeline pipeline)
{
    public async Task<CreateCarRepairResult> CreateCarRepairAsync(
        CreateCarRepairCommand command,
        CancellationToken cancellationToken)
    {
        var validationContext = new ValidationContext(command);
        var validationResults = new List<ValidationResult>();

        if (!Validator.TryValidateObject(command, validationContext, validationResults, true))
        {
            var errorMessage = validationResults.First().ErrorMessage;
            throw new GraphQLException(errorMessage ?? "Command validation failed.");
        }

        {
            var repairContext = new ValidationContext(command.Repair);
            var repairResults = new List<ValidationResult>();

            if (!Validator.TryValidateObject(command.Repair, repairContext, repairResults, true))
            {
                var errorMessage = repairResults.First().ErrorMessage;
                throw new GraphQLException(errorMessage ?? "Repair validation failed.");
            }
        }

        var context = new CreateCarRepairContext
        {
            Source = command.Source,
            Payload = command.Payload,
            RuleSet = command.RuleSet,
            Repair = command.Repair
        };

        await pipeline.RunAsync(context, cancellationToken);

        await store.AddAsync(context.Car!, context.PreparedRepair!, cancellationToken);
        return new CreateCarRepairResult(context.Car!.Id, context.PreparedRepair!.Id);
    }

    public async Task<EditCarRepairResult> EditCarRepairAsync(
        EditCarRepairCommand command,
        CancellationToken cancellationToken)
    {
        var validationContext = new ValidationContext(command);
        var validationResults = new List<ValidationResult>();

        if (!Validator.TryValidateObject(command, validationContext, validationResults, true))
        {
            var errorMessage = validationResults.First().ErrorMessage;
            throw new GraphQLException(errorMessage ?? "Command validation failed.");
        }

        if (command.Id == Guid.Empty)
            throw new GraphQLException("Repair Id is required.");

        var updated = await store.UpdateRepairAsync(command, cancellationToken);
        return !updated ? throw new GraphQLException($"Repair '{command.Id}' was not found.") : new EditCarRepairResult(command.Id);
    }
}