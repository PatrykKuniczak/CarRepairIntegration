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
    public bool TryNormalize(IncomingCarData input, out IncomingCarData? normalized, out string? error)
    {
        if (input.EnginePowerUnit.Equals("km", StringComparison.OrdinalIgnoreCase))
        {
            normalized = input with { EnginePowerUnit = "km" };
            error = null;
            return true;
        }

        var strategy = strategies.FirstOrDefault(x => x.CanHandle(input.EnginePowerUnit));
        if (strategy is null)
        {
            normalized = null;
            error = $"Unsupported engine power unit: '{input.EnginePowerUnit}'.";
            return false;
        }

        normalized = input with
        {
            EnginePower = strategy.ConvertToKm(input.EnginePower),
            EnginePowerUnit = "km"
        };
        error = null;
        return true;
    }
}
