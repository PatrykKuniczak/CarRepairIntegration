using Application.Adapters;
using Application.Normalization;
using Application.Rules;
using Domain.Entities;

namespace Application.Pipeline;

public sealed class ReceiveFilter : IImportFilter
{
    public Task ApplyAsync(ImportContext context, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(context.Source))
            throw new InvalidOperationException("Source is required.");

        if (string.IsNullOrWhiteSpace(context.Payload))
            throw new InvalidOperationException("Payload is required.");

        return Task.CompletedTask;
    }
}

public sealed class AdaptFilter(CarAdapterFactory factory) : IImportFilter
{
    public Task ApplyAsync(ImportContext context, CancellationToken cancellationToken)
    {
        context.Input = factory.Create(context.Source).Adapt(context.Payload);
        return Task.CompletedTask;
    }
}

public sealed class NormalizeFilter(UnitNormalizer normalizer) : IImportFilter
{
    public Task ApplyAsync(ImportContext context, CancellationToken cancellationToken)
    {
        context.Input = normalizer.Normalize(
            context.Input ?? throw new InvalidOperationException("No adapted input."));

        return Task.CompletedTask;
    }
}

public sealed class EvaluateFilter(CarRuleSetFactory factory) : IImportFilter
{
    public Task ApplyAsync(ImportContext context, CancellationToken cancellationToken)
    {
        var input = context.Input
            ?? throw new InvalidOperationException("No normalized input.");

        var car = new Car
        {
            Id = Guid.NewGuid(),
            Brand = input.Brand,
            Model = input.Model,
            EnginePower = input.EnginePower,
            EnginePowerUnit = input.EnginePowerUnit,
            Color = input.Color
        };

        var rules = factory.Create(context.RuleSet).Rules;
        if (!rules.IsSatisfiedBy(car))
            throw new InvalidOperationException($"Car does not satisfy rule set '{context.RuleSet}'.");

        context.Car = car;
        return Task.CompletedTask;
    }
}

public sealed class PrepareFilter : IImportFilter
{
    public Task ApplyAsync(ImportContext context, CancellationToken cancellationToken)
    {
        var car = context.Car
            ?? throw new InvalidOperationException("No evaluated car.");

        context.PreparedRepair = new CarRepair
        {
            Id = Guid.NewGuid(),
            CarId = car.Id,
            Description = context.Repair.Description,
            RepairDate = context.Repair.RepairDate,
            Cost = context.Repair.Cost,
            ServiceName = context.Repair.ServiceName
        };

        return Task.CompletedTask;
    }
}
