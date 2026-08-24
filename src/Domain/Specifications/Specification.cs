namespace Domain.Specifications;

public sealed class Specification<T>(Predicate<T> predicate)
{
    public bool IsSatisfiedBy(T value) => predicate(value);

    public Specification<T> And(Specification<T> other) =>
        new(value => IsSatisfiedBy(value) && other.IsSatisfiedBy(value));

    public Specification<T> Or(Specification<T> other) =>
        new(value => IsSatisfiedBy(value) || other.IsSatisfiedBy(value));

    public Specification<T> Not() =>
        new(value => !IsSatisfiedBy(value));
}

// A small, meaningful DSL for composing business rules in normal C#.
public static class RuleBuilder
{
    public static Specification<T> If<T>(Predicate<T> rule) =>
        new(rule);
}
