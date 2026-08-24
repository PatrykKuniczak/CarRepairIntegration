using HotChocolate.Types;

namespace Api.GraphQL.DTOs;

public record CreateCarRepairDto(
    [DefaultValue("Service_A")] string Source = "Service_A",
    [DefaultValue("{\"brand\":\"Audi\",\"model\":\"A4\",\"powerKw\":110,\"color\":\"Black\"}")] string Payload = "{\"brand\":\"Audi\",\"model\":\"A4\",\"powerKw\":110,\"color\":\"Black\"}",
    [DefaultValue("standard")] string RuleSet = "standard",
    RepairInputDto Repair = null!);

public record RepairInputDto(
    [DefaultValue("Wymiana rozrządu")] string Description = "Wymiana rozrządu",
    DateTime? RepairDate = null,
    [DefaultValue(1200.50)] decimal Cost = 1200.50m,
    [DefaultValue("ASO")] string ServiceName = "ASO");

public record EditCarRepairDto(
    Guid Id = default,
    [DefaultValue("Wymiana rozrządu")] string Description = "Wymiana rozrządu",
    DateTime? RepairDate = null,
    [DefaultValue(1200.50)] decimal Cost = 1200.50m,
    [DefaultValue("ASO")] string ServiceName = "ASO");
