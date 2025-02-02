using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Diesel_modular_application.Migrations
{
    /// <inheritdoc />
    public partial class ZdrojTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TableDieslovani_TableFirma_TableFirmaIDFirmy",
                schema: "Data",
                table: "TableDieslovani");

            migrationBuilder.DropForeignKey(
                name: "FK_TableDieslovani_TableTechnici_IdTechnik",
                schema: "Data",
                table: "TableDieslovani");

            migrationBuilder.DropForeignKey(
                name: "FK_TablePohotovosti_TableTechnici_IdTechnik",
                schema: "Data",
                table: "TablePohotovosti");

            migrationBuilder.DropForeignKey(
                name: "FK_TablePohotovosti_User_IdUser",
                schema: "Data",
                table: "TablePohotovosti");

            migrationBuilder.DropForeignKey(
                name: "FK_TableTechnici_User_IdUser",
                schema: "Data",
                table: "TableTechnici");

            migrationBuilder.DropIndex(
                name: "IX_TableDieslovani_TableFirmaIDFirmy",
                schema: "Data",
                table: "TableDieslovani");

            migrationBuilder.DropColumn(
                name: "TableFirmaIDFirmy",
                schema: "Data",
                table: "TableDieslovani");

            migrationBuilder.AlterColumn<string>(
                name: "Prijmeni",
                schema: "Data",
                table: "TableTechnici",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Jmeno",
                schema: "Data",
                table: "TableTechnici",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "IdUser",
                schema: "Data",
                table: "TableTechnici",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "NazevRegionu",
                schema: "Data",
                table: "TableRegiony",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "IdUser",
                schema: "Data",
                table: "TablePohotovosti",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "IdTechnik",
                schema: "Data",
                table: "TablePohotovosti",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "NazevFirmy",
                schema: "Data",
                table: "TableFirma",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "IdTechnik",
                schema: "Data",
                table: "TableDieslovani",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Popis",
                schema: "Data",
                table: "OdstavkyTable",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Distributor",
                schema: "Data",
                table: "OdstavkyTable",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Lokalita",
                schema: "Data",
                table: "LokalityTable",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Klasifikace",
                schema: "Data",
                table: "LokalityTable",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Adresa",
                schema: "Data",
                table: "LokalityTable",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "ZdrojId",
                schema: "Data",
                table: "LokalityTable",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TableZdroj",
                schema: "Identity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nazev = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Odber = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TableZdroj", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LokalityTable_ZdrojId",
                schema: "Data",
                table: "LokalityTable",
                column: "ZdrojId");

            migrationBuilder.AddForeignKey(
                name: "FK_LokalityTable_TableZdroj_ZdrojId",
                schema: "Data",
                table: "LokalityTable",
                column: "ZdrojId",
                principalSchema: "Identity",
                principalTable: "TableZdroj",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TableDieslovani_TableTechnici_IdTechnik",
                schema: "Data",
                table: "TableDieslovani",
                column: "IdTechnik",
                principalSchema: "Data",
                principalTable: "TableTechnici",
                principalColumn: "IdTechnika");

            migrationBuilder.AddForeignKey(
                name: "FK_TablePohotovosti_TableTechnici_IdTechnik",
                schema: "Data",
                table: "TablePohotovosti",
                column: "IdTechnik",
                principalSchema: "Data",
                principalTable: "TableTechnici",
                principalColumn: "IdTechnika");

            migrationBuilder.AddForeignKey(
                name: "FK_TablePohotovosti_User_IdUser",
                schema: "Data",
                table: "TablePohotovosti",
                column: "IdUser",
                principalSchema: "Identity",
                principalTable: "User",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TableTechnici_User_IdUser",
                schema: "Data",
                table: "TableTechnici",
                column: "IdUser",
                principalSchema: "Identity",
                principalTable: "User",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LokalityTable_TableZdroj_ZdrojId",
                schema: "Data",
                table: "LokalityTable");

            migrationBuilder.DropForeignKey(
                name: "FK_TableDieslovani_TableTechnici_IdTechnik",
                schema: "Data",
                table: "TableDieslovani");

            migrationBuilder.DropForeignKey(
                name: "FK_TablePohotovosti_TableTechnici_IdTechnik",
                schema: "Data",
                table: "TablePohotovosti");

            migrationBuilder.DropForeignKey(
                name: "FK_TablePohotovosti_User_IdUser",
                schema: "Data",
                table: "TablePohotovosti");

            migrationBuilder.DropForeignKey(
                name: "FK_TableTechnici_User_IdUser",
                schema: "Data",
                table: "TableTechnici");

            migrationBuilder.DropTable(
                name: "TableZdroj",
                schema: "Identity");

            migrationBuilder.DropIndex(
                name: "IX_LokalityTable_ZdrojId",
                schema: "Data",
                table: "LokalityTable");

            migrationBuilder.DropColumn(
                name: "ZdrojId",
                schema: "Data",
                table: "LokalityTable");

            migrationBuilder.AlterColumn<string>(
                name: "Prijmeni",
                schema: "Data",
                table: "TableTechnici",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Jmeno",
                schema: "Data",
                table: "TableTechnici",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "IdUser",
                schema: "Data",
                table: "TableTechnici",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NazevRegionu",
                schema: "Data",
                table: "TableRegiony",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "IdUser",
                schema: "Data",
                table: "TablePohotovosti",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "IdTechnik",
                schema: "Data",
                table: "TablePohotovosti",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NazevFirmy",
                schema: "Data",
                table: "TableFirma",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "IdTechnik",
                schema: "Data",
                table: "TableDieslovani",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TableFirmaIDFirmy",
                schema: "Data",
                table: "TableDieslovani",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Popis",
                schema: "Data",
                table: "OdstavkyTable",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Distributor",
                schema: "Data",
                table: "OdstavkyTable",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Lokalita",
                schema: "Data",
                table: "LokalityTable",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Klasifikace",
                schema: "Data",
                table: "LokalityTable",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Adresa",
                schema: "Data",
                table: "LokalityTable",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

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

            migrationBuilder.AddForeignKey(
                name: "FK_TableDieslovani_TableTechnici_IdTechnik",
                schema: "Data",
                table: "TableDieslovani",
                column: "IdTechnik",
                principalSchema: "Data",
                principalTable: "TableTechnici",
                principalColumn: "IdTechnika",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TablePohotovosti_TableTechnici_IdTechnik",
                schema: "Data",
                table: "TablePohotovosti",
                column: "IdTechnik",
                principalSchema: "Data",
                principalTable: "TableTechnici",
                principalColumn: "IdTechnika",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TablePohotovosti_User_IdUser",
                schema: "Data",
                table: "TablePohotovosti",
                column: "IdUser",
                principalSchema: "Identity",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TableTechnici_User_IdUser",
                schema: "Data",
                table: "TableTechnici",
                column: "IdUser",
                principalSchema: "Identity",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
