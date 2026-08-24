namespace Application.DTOs;

// Lightweight read DTO. A list query does not need the full domain model.
public sealed record GetCar(
    Guid Id,
    string Brand,
    string Model,
    decimal EnginePower,
    string Color)
{
    public GetCar() : this(default, string.Empty, string.Empty, default, string.Empty) { }
}
