using System.Data;
using System.Globalization;
using Application.Adapters;
using Application.CQRS;
using Application.Normalization;
using Application.Persistence;
using Application.Pipeline;
using Application.Rules;
using Dapper;
using Infrastructure.Adapters.ServiceA;
using Infrastructure.Adapters.ServiceB;
using Infrastructure.Persistence;
using Infrastructure.Rules;
using Infrastructure.Seeding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    static DependencyInjection()
    {
        SqlMapper.AddTypeHandler(new GuidTypeHandler());
        SqlMapper.AddTypeHandler(new DecimalTypeHandler());
    }

    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(configuration.GetConnectionString("Default")));

        services.AddScoped<ICarReadStore, CarReadStore>();
        services.AddScoped<ICarWriteStore, CarWriteStore>();
        services.AddScoped<CarQueries>();
        services.AddScoped<CarCommands>();

        // Adapter + Factory: the pipeline selects a source at runtime.
        services.AddScoped<ICarAdapter, ServiceAAdapter>();
        services.AddScoped<ICarAdapter, ServiceBAdapter>();
        services.AddScoped<CarAdapterFactory>();

        // Strategy: unit-specific conversion is isolated from normalization.
        services.AddScoped<IUnitConversionStrategy, KwToKmStrategy>();
        services.AddScoped<UnitNormalizer>();

        // Strategy + Factory: rule definitions can grow independently.
        services.AddScoped<ICarRuleSet, StandardCarRuleSet>();
        services.AddScoped<ICarRuleSet, SportCarRuleSet>();
        services.AddScoped<CarRuleSetFactory>();

        // Pipe & Filter: registration order is the workflow order.
        services.AddScoped<IImportFilter, ReceiveFilter>();
        services.AddScoped<IImportFilter, AdaptFilter>();
        services.AddScoped<IImportFilter, NormalizeFilter>();
        services.AddScoped<IImportFilter, EvaluateFilter>();
        services.AddScoped<IImportFilter, PrepareFilter>();
        services.AddScoped<ImportPipeline>();

        services.AddScoped<DbSeeder>();
        return services;
    }

    extension(IServiceProvider services)
    {
        public async Task ApplyMigrationsAsync()
        {
            using var scope = services.CreateScope();
            await scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.MigrateAsync();
        }

        public async Task SeedAsync()
        {
            using var scope = services.CreateScope();
            await scope.ServiceProvider.GetRequiredService<DbSeeder>().SeedAsync();
        }
    }
}

internal sealed class GuidTypeHandler : SqlMapper.TypeHandler<Guid>
{
    public override void SetValue(IDbDataParameter parameter, Guid value) => parameter.Value = value.ToString();
    public override Guid Parse(object value) => value is Guid guid ? guid : Guid.Parse(value.ToString()!);
}

internal sealed class DecimalTypeHandler : SqlMapper.TypeHandler<decimal>
{
    public override void SetValue(IDbDataParameter parameter, decimal value) => parameter.Value = value;
    public override decimal Parse(object value) => value is decimal d ? d : Convert.ToDecimal(value, CultureInfo.InvariantCulture);
}