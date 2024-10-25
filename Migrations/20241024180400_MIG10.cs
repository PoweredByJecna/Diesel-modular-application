using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Diesel_modular_application.Migrations
{
    /// <inheritdoc />
    public partial class MIG10 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
