using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Diesel_modular_application.Migrations
{
    /// <inheritdoc />
    public partial class MIG17 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IDodstavky",
                schema: "Data",
                table: "TableDieslovani",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TableDieslovani_IDodstavky",
                schema: "Data",
                table: "TableDieslovani",
                column: "IDodstavky");

            migrationBuilder.AddForeignKey(
                name: "FK_TableDieslovani_OdstavkyTable_IDodstavky",
                schema: "Data",
                table: "TableDieslovani",
                column: "IDodstavky",
                principalSchema: "Data",
                principalTable: "OdstavkyTable",
                principalColumn: "IdOdstavky");
               
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
