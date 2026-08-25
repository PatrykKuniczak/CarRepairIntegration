using System.ComponentModel.DataAnnotations;
using HotChocolate.Types;

namespace Api.GraphQL.DTOs;

public record CreateCarRepairDto(
    [property: Required] [property: MinLength(1)] [DefaultValue("Service_A")] string Source = "Service_A",
    [property: Required] [property: MinLength(1)] [DefaultValue("{\"brand\":\"Audi\",\"model\":\"A4\",\"powerKw\":110,\"color\":\"Black\"}")] string Payload = "{\"brand\":\"Audi\",\"model\":\"A4\",\"powerKw\":110,\"color\":\"Black\"}",
    [property: Required] [property: MinLength(1)] [DefaultValue("standard")] string RuleSet = "standard",
    [property: Required] RepairInputDto Repair = null!);

public record RepairInputDto(
    [property: Required] [property: MinLength(1)] [DefaultValue("Wymiana rozrządu")] string Description = "Wymiana rozrządu",
    [property: Required] DateTime? RepairDate = null,
    [property: Required] [property: Range(0.01, 1000000)] [DefaultValue(1200.50)] decimal Cost = 1200.50m,
    [property: Required] [property: MinLength(1)] [DefaultValue("ASO")] string ServiceName = "ASO");

public record EditCarRepairDto(
    [property: Required] Guid Id = default,
    [property: Required] [property: MinLength(1)] [DefaultValue("Wymiana rozrządu")] string Description = "Wymiana rozrządu",
    [property: Required] DateTime? RepairDate = null,
    [property: Required] [property: Range(0.01, 1000000)] [DefaultValue(1200.50)] decimal Cost = 1200.50m,
    [property: Required] [property: MinLength(1)] [DefaultValue("ASO")] string ServiceName = "ASO");