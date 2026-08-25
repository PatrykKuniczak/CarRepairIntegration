using Domain.Entities;
using Domain.Specifications;

namespace Application.Rules;

// Strategy: every rule set supplies one Specification and has a name.
public interface ICarRuleSet
{
    string Name { get; }
    Specification<Car> Rules { get; }
}

// Factory keeps rule selection out of the pipeline implementation.
public sealed class CarRuleSetFactory(IEnumerable<ICarRuleSet> ruleSets)
{
    public ICarRuleSet? GetRuleSet(string name) =>
        ruleSets.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
}
