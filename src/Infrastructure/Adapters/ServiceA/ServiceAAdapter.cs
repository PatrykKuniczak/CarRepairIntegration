using System.Text.Json;
using Application.Adapters;
using Application.Pipeline;

namespace Infrastructure.Adapters.ServiceA;

public sealed class ServiceAAdapter : ICarAdapter
{
    public bool CanHandle(string source) =>
        source.Equals("Service_A", StringComparison.OrdinalIgnoreCase);

    public bool TryAdapt(string payload, out IncomingCarData? data, out string? error)
    {
        ServiceADto? dto;
        try
        {
            dto = JsonSerializer.Deserialize<ServiceADto>(payload, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
        catch (JsonException ex)
        {
            data = null;
            error = $"Invalid Service_A JSON format: {ex.Message}";
            return false;
        }

        if (dto is null)
        {
            data = null;
            error = "Invalid Service_A payload: JSON deserialized to null.";
            return false;
        }

        if (string.IsNullOrWhiteSpace(dto.Brand) || string.IsNullOrWhiteSpace(dto.Model))
        {
            data = null;
            error = "Invalid Service_A payload: Brand and Model are required.";
            return false;
        }

        data = new IncomingCarData(
            dto.Brand,
            dto.Model,
            dto.PowerKw,
            "kW",
            dto.Color);
        error = null;
        return true;
    }
}
