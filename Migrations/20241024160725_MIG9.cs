using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Diesel_modular_application.Migrations
{
    /// <inheritdoc />
    public partial class MIG9 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RegionID",
                schema: "Data",
                table: "TableFirma",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TableFirma_RegionID",
                schema: "Data",
                table: "TableFirma",
                column: "RegionID");

            migrationBuilder.AddForeignKey(
                name: "FK_TableFirma_TableRegiony_RegionID",
                schema: "Data",
                table: "TableFirma",
                column: "RegionID",
                principalSchema: "Data",
                principalTable: "TableRegiony",
                principalColumn: "IdRegion");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TableFirma_TableRegiony_RegionID",
                schema: "Data",
                table: "TableFirma");

            migrationBuilder.DropIndex(
                name: "IX_TableFirma_RegionID",
                schema: "Data",
                table: "TableFirma");

            migrationBuilder.DropColumn(
                name: "RegionID",
                schema: "Data",
                table: "TableFirma");
        }
    }
}
