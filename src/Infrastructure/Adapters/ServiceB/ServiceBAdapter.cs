using System.Globalization;
using Application.Adapters;
using Application.Pipeline;
using CsvHelper;
using CsvHelper.Configuration;

namespace Infrastructure.Adapters.ServiceB;

public sealed class ServiceBAdapter : ICarAdapter
{
    public bool CanHandle(string source) =>
        source.Equals("Service_B", StringComparison.OrdinalIgnoreCase);

    public bool TryAdapt(string payload, out IncomingCarData? data, out string? error)
    {
        Dictionary<string, string> values;
        try
        {
            values = ReadValues(payload);
        }
        catch (Exception ex)
        {
            data = null;
            error = $"Invalid Service_B CSV format: {ex.Message}";
            return false;
        }

        if (!values.TryGetValue("CAR_BRAND", out var brand) || string.IsNullOrWhiteSpace(brand))
        {
            data = null;
            error = "Invalid Service_B payload: CAR_BRAND is missing.";
            return false;
        }

        if (!values.TryGetValue("CAR_MODEL", out var model) || string.IsNullOrWhiteSpace(model))
        {
            data = null;
            error = "Invalid Service_B payload: CAR_MODEL is missing.";
            return false;
        }

        if (!values.TryGetValue("ENGINE_POWER", out var powerStr) || !decimal.TryParse(powerStr, CultureInfo.InvariantCulture, out var power))
        {
            data = null;
            error = "Invalid Service_B payload: ENGINE_POWER is invalid or missing.";
            return false;
        }

        if (!values.TryGetValue("ENGINE_POWER_UNIT", out var unit) || string.IsNullOrWhiteSpace(unit))
        {
            data = null;
            error = "Invalid Service_B payload: ENGINE_POWER_UNIT is missing.";
            return false;
        }

        values.TryGetValue("COLOR", out var color);

        data = new IncomingCarData(
            brand,
            model,
            power,
            unit,
            color ?? string.Empty);
        error = null;
        return true;
    }

    private static Dictionary<string, string> ReadValues(string payload)
    {
        using var reader = new StringReader(payload.Replace(";", "\n", StringComparison.Ordinal));
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = ":",
            HasHeaderRecord = false,
            BadDataFound = null,
            MissingFieldFound = null
        });

        var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        while (csv.Read())
        {
            var record = csv.Parser.Record;
            if (record is { Length: >= 2 })
                values[record[0].Trim()] = record[1].Trim();
        }

        return values;
    }
}
