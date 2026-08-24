using System.Text.Json;
using Application.Adapters;
using Application.Pipeline;

namespace Infrastructure.Adapters.ServiceA;

public sealed class ServiceAAdapter : ICarAdapter
{
    public bool CanHandle(string source) =>
        source.Equals("Service_A", StringComparison.OrdinalIgnoreCase);

    public IncomingCarData Adapt(string payload)
    {
        var dto = JsonSerializer.Deserialize<ServiceADto>(payload)
            ?? throw new InvalidOperationException("Invalid Service_A payload.");

        return new IncomingCarData(
            dto.Brand,
            dto.Model,
            dto.PowerKw,
            "kW",
            dto.Color);
    }
}
