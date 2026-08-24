namespace Infrastructure.Adapters.ServiceB;

public record ServiceBDto(
    string CarBrand,
    string CarModel,
    decimal EnginePower,
    string EnginePowerUnit,
    string Color);
