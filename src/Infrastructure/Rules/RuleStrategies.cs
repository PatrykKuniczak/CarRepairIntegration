using Application.Normalization;
using Application.Rules;
using Domain.Entities;
using Domain.Specifications;

namespace Infrastructure.Rules;

public sealed class KwToKmStrategy : IUnitConversionStrategy
{
    public bool CanHandle(string unit) =>
        unit.Equals("kw", StringComparison.OrdinalIgnoreCase);

    public decimal ConvertToKm(decimal value) => value * 1.3596216173m;
}

public sealed class StandardCarRuleSet : ICarRuleSet
{
    public string Name => "standard";
    public Specification<Car> Rules => CarSpecifications.Standard();
}

public sealed class SportCarRuleSet : ICarRuleSet
{
    public string Name => "sport";
    public Specification<Car> Rules => CarSpecifications.Sport();
}
