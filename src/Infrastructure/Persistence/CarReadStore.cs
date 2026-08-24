using Application.DTOs;
using Application.Persistence;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Infrastructure.Persistence;

public sealed class CarReadStore(AppDbContext db) : ICarReadStore
{
    // Dapper uses the same DbConnection that belongs to EF Core.
    // Without an active EF transaction, Dapper manages opening/closing a closed connection itself.
    // With an active transaction, we pass EF's transaction to Dapper and EF keeps ownership of it.
    public async Task<IReadOnlyList<GetCar>> GetCarsAsync(int take, CancellationToken cancellationToken)
    {
        const string sql = """
                           SELECT Id, Brand, Model, EnginePower, Color
                           FROM Cars
                           ORDER BY Brand, Model
                           LIMIT @Take;
                           """;

        var command = new CommandDefinition(
            sql,
            new { Take = take },
            db.Database.CurrentTransaction?.GetDbTransaction(),
            cancellationToken: cancellationToken);

        var rows = await db.Database.GetDbConnection().QueryAsync<GetCar>(command);
        return rows.AsList();
    }

    public async Task<GetCar?> GetCarAsync(Guid id, CancellationToken cancellationToken)
    {
        const string sql = """
                           SELECT Id, Brand, Model, EnginePower, Color
                           FROM Cars
                           WHERE Id = @Id;
                           """;

        var command = new CommandDefinition(
            sql,
            new { Id = id },
            db.Database.CurrentTransaction?.GetDbTransaction(),
            cancellationToken: cancellationToken);

        return await db.Database.GetDbConnection().QuerySingleOrDefaultAsync<GetCar>(command);
    }
}