using Application.Adapters;
using Application.Normalization;
using Application.Rules;
using Domain.Entities;
using HotChocolate;

namespace Application.Pipeline;

public sealed class ReceiveFilter : ICreateCarRepairFilter
{
    public Task ApplyAsync(CreateCarRepairContext context, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(context.Source))
            throw new GraphQLException("Source is required.");

        if (string.IsNullOrWhiteSpace(context.Payload))
            throw new GraphQLException("Payload is required.");

        return Task.CompletedTask;
    }
}

public sealed class AdaptFilter(CarAdapterFactory factory) : ICreateCarRepairFilter
{
    public Task ApplyAsync(CreateCarRepairContext context, CancellationToken cancellationToken)
    {
        var adapter = factory.GetAdapter(context.Source);
        if (adapter is null)
            throw new GraphQLException($"Unsupported source: '{context.Source}'.");

        if (!adapter.TryAdapt(context.Payload, out var input, out var error))
            throw new GraphQLException(error ?? $"Failed to adapt payload for source '{context.Source}'.");

        context.Input = input;
        return Task.CompletedTask;
    }
}

public sealed class NormalizeFilter(UnitNormalizer normalizer) : ICreateCarRepairFilter
{
    public Task ApplyAsync(CreateCarRepairContext context, CancellationToken cancellationToken)
    {
        if (context.Input is null)
            throw new GraphQLException("No adapted input.");

        if (!normalizer.TryNormalize(context.Input, out var normalized, out var error))
            throw new GraphQLException(error ?? "Failed to normalize engine power unit.");

        context.Input = normalized;
        return Task.CompletedTask;
    }
}

public sealed class EvaluateFilter(CarRuleSetFactory factory) : ICreateCarRepairFilter
{
    public Task ApplyAsync(CreateCarRepairContext context, CancellationToken cancellationToken)
    {
        var input = context.Input;
        if (input is null)
            throw new GraphQLException("No normalized input.");

        var car = new Car
        {
            Id = Guid.NewGuid(),
            Brand = input.Brand,
            Model = input.Model,
            EnginePower = input.EnginePower,
            EnginePowerUnit = input.EnginePowerUnit,
            Color = input.Color
        };

        var ruleSet = factory.GetRuleSet(context.RuleSet);
        if (ruleSet is null)
            throw new GraphQLException($"Unknown rule set: '{context.RuleSet}'.");

        var rules = ruleSet.Rules;
        if (!rules.IsSatisfiedBy(car))
            throw new GraphQLException($"Car does not satisfy rule set '{context.RuleSet}'.");

        context.Car = car;
        return Task.CompletedTask;
    }
}

public sealed class PrepareFilter : ICreateCarRepairFilter
{
    public Task ApplyAsync(CreateCarRepairContext context, CancellationToken cancellationToken)
    {
        var car = context.Car;
        if (car is null)
            throw new GraphQLException("No evaluated car.");

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
