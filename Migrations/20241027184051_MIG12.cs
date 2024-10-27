using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Diesel_modular_application.Migrations
{
    /// <inheritdoc />
    public partial class MIG12 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
          migrationBuilder.DropForeignKey(
                name: "FK_TableRegiony_TableFirma_FirmaID",
                schema: "Data",
                table: "TableRegiony");

            migrationBuilder.DropIndex(
                name: "IX_TableRegiony_FirmaID",
                schema: "Data",
                table: "TableRegiony");

            migrationBuilder.DropColumn(
                name: "FirmaID",
                schema: "Data",
                table: "TableRegiony");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            
        }
    }
}
