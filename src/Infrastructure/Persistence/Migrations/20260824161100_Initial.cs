using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations;

[DbContext(typeof(AppDbContext))]
[Migration("20260824161100_Initial")]
public partial class Initial : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Cars",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "TEXT", nullable: false),
                Brand = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                Model = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                EnginePower = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                EnginePowerUnit = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                Color = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Cars", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "CarRepairs",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "TEXT", nullable: false),
                CarId = table.Column<Guid>(type: "TEXT", nullable: false),
                Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                RepairDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                Cost = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                ServiceName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_CarRepairs", x => x.Id);
                table.ForeignKey(
                    name: "FK_CarRepairs_Cars_CarId",
                    column: x => x.CarId,
                    principalTable: "Cars",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_CarRepairs_CarId",
            table: "CarRepairs",
            column: "CarId");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "CarRepairs");
        migrationBuilder.DropTable(name: "Cars");
    }
}
