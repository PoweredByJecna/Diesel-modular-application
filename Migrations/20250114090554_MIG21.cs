using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Diesel_modular_application.Migrations
{
    /// <inheritdoc />
    public partial class MIG21 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Odchod",
                schema: "Data",
                table: "OdstavkyTable");

            migrationBuilder.DropColumn(
                name: "Vstup",
                schema: "Data",
                table: "OdstavkyTable");

            migrationBuilder.DropColumn(
                name: "Zásuvka",
                schema: "Data",
                table: "LokalityTable");

            migrationBuilder.RenameColumn(
                name: "Prijemni",
                schema: "Data",
                table: "TableTechnici",
                newName: "Prijmeni");

            migrationBuilder.AlterColumn<bool>(
                name: "DA",
                schema: "Data",
                table: "LokalityTable",
                type: "bit",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<bool>(
                name: "Zasuvka",
                schema: "Data",
                table: "LokalityTable",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Zasuvka",
                schema: "Data",
                table: "LokalityTable");

            migrationBuilder.RenameColumn(
                name: "Prijmeni",
                schema: "Data",
                table: "TableTechnici",
                newName: "Prijemni");

            migrationBuilder.AddColumn<DateTime>(
                name: "Odchod",
                schema: "Data",
                table: "OdstavkyTable",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "Vstup",
                schema: "Data",
                table: "OdstavkyTable",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<string>(
                name: "DA",
                schema: "Data",
                table: "LokalityTable",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AddColumn<string>(
                name: "Zásuvka",
                schema: "Data",
                table: "LokalityTable",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
