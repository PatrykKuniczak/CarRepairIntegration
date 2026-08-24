using Application.Pipeline;

namespace Application.Normalization;

// Strategy contract: every unit conversion is a small, replaceable strategy.
public interface IUnitConversionStrategy
{
    bool CanHandle(string unit);
    decimal ConvertToKm(decimal value);
}

public sealed class UnitNormalizer(IEnumerable<IUnitConversionStrategy> strategies)
{
    public IncomingCarData Normalize(IncomingCarData input)
    {
        if (input.EnginePowerUnit.Equals("km", StringComparison.OrdinalIgnoreCase))
            return input with { EnginePowerUnit = "km" };

        var strategy = strategies.FirstOrDefault(x => x.CanHandle(input.EnginePowerUnit))
            ?? throw new InvalidOperationException($"Unsupported engine unit: {input.EnginePowerUnit}");

        return input with
        {
            EnginePower = strategy.ConvertToKm(input.EnginePower),
            EnginePowerUnit = "km"
        };
    }
}
