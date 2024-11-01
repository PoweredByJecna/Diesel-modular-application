using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Diesel_modular_application.Migrations
{
    /// <inheritdoc />
    public partial class MIG16 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {


                 migrationBuilder.DropForeignKey(
                name: "FK_TableDieslovani_OdstavkyTable_IDodstavky",
                schema: "Data",
                table: "TableDieslovani");

            migrationBuilder.DropIndex(
                name: "IX_TableDieslovani_IDodstavky",
                schema: "Data",
                table: "TableDieslovani");

            migrationBuilder.DropColumn(
                name: "IDodstavky",
                schema: "Data",
                table: "TableDieslovani");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
