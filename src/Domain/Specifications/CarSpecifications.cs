using Domain.Entities;

namespace Domain.Specifications;

public static class CarSpecifications
{
    public static Predicate<Car> HasIdentity() =>
        car => !string.IsNullOrWhiteSpace(car.Brand) && !string.IsNullOrWhiteSpace(car.Model);

    public static Predicate<Car> EnginePowerIsPositive() =>
        car => car.EnginePower > 0;

    public static Predicate<Car> EngineUnitIsKm() =>
        car => car.EnginePowerUnit.Equals("km", StringComparison.OrdinalIgnoreCase);

    public static Predicate<Car> EngineIsSport() =>
        car => car.EnginePower >= 200;

    public static Predicate<Car> IsBlack() =>
        car => car.Color.Equals("black", StringComparison.OrdinalIgnoreCase);

    public static Predicate<Car> IsWhite() =>
        car => car.Color.Equals("white", StringComparison.OrdinalIgnoreCase);

    public static Specification<Car> Standard() =>
        RuleBuilder.If(EnginePowerIsPositive())
            .And(RuleBuilder.If(EngineUnitIsKm()))
            .And(RuleBuilder.If(HasIdentity()))
            .And(RuleBuilder.If(IsBlack()).Or(RuleBuilder.If(IsWhite())));

    public static Specification<Car> Sport() =>
        RuleBuilder.If(EngineIsSport())
            .And(RuleBuilder.If(EngineUnitIsKm()))
            .And(RuleBuilder.If(HasIdentity()))
            // A sport rule also demonstrates NOT: a white car is rejected here.
            .And(RuleBuilder.If(IsWhite()).Not());
}
