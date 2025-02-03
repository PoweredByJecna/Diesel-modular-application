using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Diesel_modular_application.Migrations
{
    /// <inheritdoc />
    public partial class Log : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LokalityTable_TableZdroj_ZdrojId",
                schema: "Data",
                table: "LokalityTable");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TableZdroj",
                schema: "Identity",
                table: "TableZdroj");

            migrationBuilder.RenameTable(
                name: "TableZdroj",
                schema: "Identity",
                newName: "Zdrojs",
                newSchema: "Identity");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Zdrojs",
                schema: "Identity",
                table: "Zdrojs",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "LogS",
                schema: "Identity",
                columns: table => new
                {
                    IdLog = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TimeStamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EntityName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EntityId = table.Column<int>(type: "int", nullable: true),
                    LogMessage = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogS", x => x.IdLog);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_LokalityTable_Zdrojs_ZdrojId",
                schema: "Data",
                table: "LokalityTable",
                column: "ZdrojId",
                principalSchema: "Identity",
                principalTable: "Zdrojs",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LokalityTable_Zdrojs_ZdrojId",
                schema: "Data",
                table: "LokalityTable");

            migrationBuilder.DropTable(
                name: "LogS",
                schema: "Identity");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Zdrojs",
                schema: "Identity",
                table: "Zdrojs");

            migrationBuilder.RenameTable(
                name: "Zdrojs",
                schema: "Identity",
                newName: "TableZdroj",
                newSchema: "Identity");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TableZdroj",
                schema: "Identity",
                table: "TableZdroj",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LokalityTable_TableZdroj_ZdrojId",
                schema: "Data",
                table: "LokalityTable",
                column: "ZdrojId",
                principalSchema: "Identity",
                principalTable: "TableZdroj",
                principalColumn: "Id");
        }
    }
}
