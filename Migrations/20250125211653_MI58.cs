using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Diesel_modular_application.Migrations
{
    /// <inheritdoc />
    public partial class MI58 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TableDieslovani_TableFirma_FirmaId",
                schema: "Data",
                table: "TableDieslovani");

            migrationBuilder.DropIndex(
                name: "IX_TableDieslovani_FirmaId",
                schema: "Data",
                table: "TableDieslovani");

            migrationBuilder.DropColumn(
                name: "FirmaId",
                schema: "Data",
                table: "TableDieslovani");

            migrationBuilder.RenameColumn(
                name: "Začátek",
                schema: "Data",
                table: "TablePohotovosti",
                newName: "Zacatek");

            migrationBuilder.RenameColumn(
                name: "NázevFirmy",
                schema: "Data",
                table: "TableFirma",
                newName: "NazevFirmy");

            migrationBuilder.AddColumn<int>(
                name: "TableFirmaIDFirmy",
                schema: "Data",
                table: "TableDieslovani",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Baterie",
                schema: "Data",
                table: "LokalityTable",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_TableDieslovani_TableFirmaIDFirmy",
                schema: "Data",
                table: "TableDieslovani",
                column: "TableFirmaIDFirmy");

            migrationBuilder.AddForeignKey(
                name: "FK_TableDieslovani_TableFirma_TableFirmaIDFirmy",
                schema: "Data",
                table: "TableDieslovani",
                column: "TableFirmaIDFirmy",
                principalSchema: "Data",
                principalTable: "TableFirma",
                principalColumn: "IDFirmy");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TableDieslovani_TableFirma_TableFirmaIDFirmy",
                schema: "Data",
                table: "TableDieslovani");

            migrationBuilder.DropIndex(
                name: "IX_TableDieslovani_TableFirmaIDFirmy",
                schema: "Data",
                table: "TableDieslovani");

            migrationBuilder.DropColumn(
                name: "TableFirmaIDFirmy",
                schema: "Data",
                table: "TableDieslovani");

            migrationBuilder.RenameColumn(
                name: "Zacatek",
                schema: "Data",
                table: "TablePohotovosti",
                newName: "Začátek");

            migrationBuilder.RenameColumn(
                name: "NazevFirmy",
                schema: "Data",
                table: "TableFirma",
                newName: "NázevFirmy");

            migrationBuilder.AddColumn<int>(
                name: "FirmaId",
                schema: "Data",
                table: "TableDieslovani",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Baterie",
                schema: "Data",
                table: "LokalityTable",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_TableDieslovani_FirmaId",
                schema: "Data",
                table: "TableDieslovani",
                column: "FirmaId");

            migrationBuilder.AddForeignKey(
                name: "FK_TableDieslovani_TableFirma_FirmaId",
                schema: "Data",
                table: "TableDieslovani",
                column: "FirmaId",
                principalSchema: "Data",
                principalTable: "TableFirma",
                principalColumn: "IDFirmy",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
