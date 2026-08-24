namespace Api.GraphQL.DTOs;

public record GetCarsDto(
    [DefaultValue(50)] int Take = 50);
