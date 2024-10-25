using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Diesel_modular_application.Migrations
{
    /// <inheritdoc />
    public partial class MIG11 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
         
           migrationBuilder.AddColumn<int>(
                name: "FirmaID",
                schema: "Data",
                table: "TableRegiony",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TableRegiony_FirmaID",
                schema: "Data",
                table: "TableRegiony",
                column: "FirmaID");

            migrationBuilder.AddForeignKey(
                name: "FK_TableRegiony_TableFirma_FirmaID",
                schema: "Data",
                table: "TableRegiony",
                column: "FirmaID",
                principalSchema: "Data",
                principalTable: "TableFirma",
                principalColumn: "IDFirmy");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
    }
}
