using Api.GraphQL;
using Infrastructure;

namespace Api;

public static class AppBuilder
{
    public static WebApplicationBuilder Build(string[]? args = null)
    {
        var builder = WebApplication.CreateBuilder(args ?? []);
        builder.Services.AddInfrastructure(builder.Configuration);

        builder.Services
            .AddGraphQLServer()
            .ModifyRequestOptions(opt => opt.IncludeExceptionDetails = true)
            .AddQueryType<CarQuery>()
            .AddMutationType<CarMutation>();

        return builder;
    }
}
