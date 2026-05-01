using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddTestScenario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TestScenarioId",
                table: "TestRuns",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "TestScenarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    RobotFile = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestScenarios", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TestRuns_TestScenarioId",
                table: "TestRuns",
                column: "TestScenarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_TestRuns_TestScenarios_TestScenarioId",
                table: "TestRuns",
                column: "TestScenarioId",
                principalTable: "TestScenarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TestRuns_TestScenarios_TestScenarioId",
                table: "TestRuns");

            migrationBuilder.DropTable(
                name: "TestScenarios");

            migrationBuilder.DropIndex(
                name: "IX_TestRuns_TestScenarioId",
                table: "TestRuns");

            migrationBuilder.DropColumn(
                name: "TestScenarioId",
                table: "TestRuns");
        }
    }
}
