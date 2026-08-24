namespace Domain.Entities;

public record Car
{
    public Guid Id { get; init; }
    public string Brand { get; init; } = "";
    public string Model { get; init; } = "";
    public decimal EnginePower { get; init; }
    public string EnginePowerUnit { get; init; } = "km";
    public string Color { get; init; } = "";
}
