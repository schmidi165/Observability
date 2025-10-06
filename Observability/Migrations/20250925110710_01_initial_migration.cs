using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Observability.Migrations
{
    /// <inheritdoc />
    public partial class _01_initial_migration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Weather",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    City = table.Column<string>(type: "text", nullable: false),
                    Temperature = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Weather", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Weather",
                columns: new[] { "Id", "City", "Temperature" },
                values: new object[,]
                {
                    { new Guid("0ee64386-08eb-42b6-8b56-5e07fb009ec0"), "Chicago", 20 },
                    { new Guid("47b4fd08-495d-4b88-9c15-d49e29d7d07a"), "Houston", 15 },
                    { new Guid("93719b20-bafd-4243-9cc7-8c4cc86046f2"), "San Antonio", 0 },
                    { new Guid("ab59ab63-3568-4b7a-a6a1-ca84cba2f677"), "Phoenix", 10 },
                    { new Guid("aef610f8-9364-4a63-8374-7598df451027"), "Philadelphia", 5 },
                    { new Guid("dda00da3-9379-497b-bd2f-bc2b479f561e"), "New York", 25 },
                    { new Guid("e38eedaf-092c-4376-a606-45ea14b41aab"), "Los Angeles", 30 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Weather");
        }
    }
}
