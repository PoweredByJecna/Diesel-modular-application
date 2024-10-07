using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Diesel_modular_application.Data.Migrations
{
    /// <inheritdoc />
    public partial class MIG9 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TableDieslovani_TableTechnik_IdTechnik",
                schema: "Data",
                table: "TableDieslovani");

            migrationBuilder.AddForeignKey(
                name: "FK_TableDieslovani_TableTechnik_IdTechnik",
                schema: "Data",
                table: "TableDieslovani",
                column: "IdTechnik",
                principalSchema: "Data",
                principalTable: "TableTechnik",
                principalColumn: "IdTechnika",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TableDieslovani_TableTechnik_IdTechnik",
                schema: "Data",
                table: "TableDieslovani");

            migrationBuilder.AddForeignKey(
                name: "FK_TableDieslovani_TableTechnik_IdTechnik",
                schema: "Data",
                table: "TableDieslovani",
                column: "IdTechnik",
                principalSchema: "Data",
                principalTable: "TableTechnik",
                principalColumn: "IdTechnika");
        }
    }
}
