using Infrastructure;

namespace Api;

public static class App
{
    public static async Task<WebApplication> Build(WebApplicationBuilder builder)
    {
        var app = builder.Build();

        // GraphQL is the only HTTP endpoint. Everything below it lives in Application/Infrastructure.
        app.MapGraphQL();

        await app.Services.ApplyMigrationsAsync();
        await app.Services.SeedAsync();

        return app;
    }
}
