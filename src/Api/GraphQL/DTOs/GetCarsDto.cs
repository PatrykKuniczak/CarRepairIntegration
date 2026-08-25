using System.ComponentModel.DataAnnotations;

namespace Api.GraphQL.DTOs;

public record GetCarsDto(
    [property: Required]
    [property: Range(1, int.MaxValue)]
    [DefaultValue(50)]
    int Take = 50);