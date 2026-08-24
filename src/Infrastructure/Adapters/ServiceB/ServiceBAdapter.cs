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

    public IncomingCarData Adapt(string payload)
    {
        var values = ReadValues(payload);

        var dto = new ServiceBDto(
            values["CAR_BRAND"],
            values["CAR_MODEL"],
            decimal.Parse(values["ENGINE_POWER"], CultureInfo.InvariantCulture),
            values["ENGINE_POWER_UNIT"],
            values["COLOR"]);

        return new IncomingCarData(
            dto.CarBrand,
            dto.CarModel,
            dto.EnginePower,
            dto.EnginePowerUnit,
            dto.Color);
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
