using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddTestApplicationAndConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TestApplicationId",
                table: "TestScenarios",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "TestApplications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    PackageName = table.Column<string>(type: "text", nullable: false),
                    Version = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestApplications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TestConfigurations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TestScenarioId = table.Column<int>(type: "integer", nullable: false),
                    DeviceName = table.Column<string>(type: "text", nullable: false),
                    PlatformVersion = table.Column<string>(type: "text", nullable: false),
                    AdditionalCapabilities = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestConfigurations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestConfigurations_TestScenarios_TestScenarioId",
                        column: x => x.TestScenarioId,
                        principalTable: "TestScenarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TestScenarios_TestApplicationId",
                table: "TestScenarios",
                column: "TestApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_TestConfigurations_TestScenarioId",
                table: "TestConfigurations",
                column: "TestScenarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_TestScenarios_TestApplications_TestApplicationId",
                table: "TestScenarios",
                column: "TestApplicationId",
                principalTable: "TestApplications",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TestScenarios_TestApplications_TestApplicationId",
                table: "TestScenarios");

            migrationBuilder.DropTable(
                name: "TestApplications");

            migrationBuilder.DropTable(
                name: "TestConfigurations");

            migrationBuilder.DropIndex(
                name: "IX_TestScenarios_TestApplicationId",
                table: "TestScenarios");

            migrationBuilder.DropColumn(
                name: "TestApplicationId",
                table: "TestScenarios");
        }
    }
}
