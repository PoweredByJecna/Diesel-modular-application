using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Diesel_modular_application.Data.Migrations
{
    /// <inheritdoc />
    public partial class MIG7 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TableFirma_TableRegiony_RegionyIdRegion",
                schema: "Data",
                table: "TableFirma");

            migrationBuilder.DropForeignKey(
                name: "FK_TableTechnik_TableRegiony_RegionyIdRegion",
                schema: "Data",
                table: "TableTechnik");

            migrationBuilder.DropIndex(
                name: "IX_TableTechnik_RegionyIdRegion",
                schema: "Data",
                table: "TableTechnik");

            migrationBuilder.DropIndex(
                name: "IX_TableFirma_RegionyIdRegion",
                schema: "Data",
                table: "TableFirma");

            migrationBuilder.DropColumn(
                name: "RegionId",
                schema: "Data",
                table: "TableTechnik");

            migrationBuilder.DropColumn(
                name: "RegionyIdRegion",
                schema: "Data",
                table: "TableTechnik");

            migrationBuilder.DropColumn(
                name: "RegionId",
                schema: "Data",
                table: "TableFirma");

            migrationBuilder.DropColumn(
                name: "RegionyIdRegion",
                schema: "Data",
                table: "TableFirma");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RegionId",
                schema: "Data",
                table: "TableTechnik",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RegionyIdRegion",
                schema: "Data",
                table: "TableTechnik",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RegionId",
                schema: "Data",
                table: "TableFirma",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RegionyIdRegion",
                schema: "Data",
                table: "TableFirma",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_TableTechnik_RegionyIdRegion",
                schema: "Data",
                table: "TableTechnik",
                column: "RegionyIdRegion");

            migrationBuilder.CreateIndex(
                name: "IX_TableFirma_RegionyIdRegion",
                schema: "Data",
                table: "TableFirma",
                column: "RegionyIdRegion");

            migrationBuilder.AddForeignKey(
                name: "FK_TableFirma_TableRegiony_RegionyIdRegion",
                schema: "Data",
                table: "TableFirma",
                column: "RegionyIdRegion",
                principalSchema: "Data",
                principalTable: "TableRegiony",
                principalColumn: "IdRegion",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TableTechnik_TableRegiony_RegionyIdRegion",
                schema: "Data",
                table: "TableTechnik",
                column: "RegionyIdRegion",
                principalSchema: "Data",
                principalTable: "TableRegiony",
                principalColumn: "IdRegion",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
