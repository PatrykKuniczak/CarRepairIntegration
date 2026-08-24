using Api.GraphQL.DTOs;
using Application.CQRS;
using Application.Pipeline;

namespace Api.GraphQL;

public sealed class CarMutation
{
    private static readonly DateTime DefaultRepairDate =
        DateTime.Parse("2026-08-23T14:30:00Z", null, System.Globalization.DateTimeStyles.AdjustToUniversal);

    // GraphQL -> CQRS Command -> Pipe & Filter -> EF Core.
    public Task<CreateCarRepairResult> CreateCarRepair(
        CreateCarRepairDto input,
        CarCommands commands,
        CancellationToken cancellationToken) =>
        commands.CreateCarRepairAsync(
            new CreateCarRepairCommand(
                input.Source,
                input.Payload,
                input.RuleSet,
                new RepairInput(
                    input.Repair.Description,
                    input.Repair.RepairDate ?? DefaultRepairDate,
                    input.Repair.Cost,
                    input.Repair.ServiceName)),
            cancellationToken);

    public Task<EditCarRepairResult> EditCarRepair(
        EditCarRepairDto input,
        CarCommands commands,
        CancellationToken cancellationToken) =>
        commands.EditCarRepairAsync(
            new EditCarRepairCommand(
                input.Id,
                input.Description,
                input.RepairDate ?? DefaultRepairDate,
                input.Cost,
                input.ServiceName),
            cancellationToken);
}
