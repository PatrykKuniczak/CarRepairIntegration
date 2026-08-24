using System.Text.Json.Serialization;

namespace Infrastructure.Adapters.ServiceA;

public record ServiceADto(
    [property: JsonPropertyName("brand")] string Brand,
    [property: JsonPropertyName("model")] string Model,
    [property: JsonPropertyName("powerKw")] decimal PowerKw,
    [property: JsonPropertyName("color")] string Color);
