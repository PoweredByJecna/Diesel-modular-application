using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Diesel_modular_application.Migrations
{
    /// <inheritdoc />
    public partial class LOGZDROJ : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LokalityTable_Zdrojs_ZdrojId",
                schema: "Data",
                table: "LokalityTable");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Zdrojs",
                schema: "Identity",
                table: "Zdrojs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LogS",
                schema: "Identity",
                table: "LogS");

            migrationBuilder.RenameTable(
                name: "Zdrojs",
                schema: "Identity",
                newName: "TableZdroj",
                newSchema: "Data");

            migrationBuilder.RenameTable(
                name: "LogS",
                schema: "Identity",
                newName: "DebugModel",
                newSchema: "Data");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TableZdroj",
                schema: "Data",
                table: "TableZdroj",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DebugModel",
                schema: "Data",
                table: "DebugModel",
                column: "IdLog");

            migrationBuilder.AddForeignKey(
                name: "FK_LokalityTable_TableZdroj_ZdrojId",
                schema: "Data",
                table: "LokalityTable",
                column: "ZdrojId",
                principalSchema: "Data",
                principalTable: "TableZdroj",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LokalityTable_TableZdroj_ZdrojId",
                schema: "Data",
                table: "LokalityTable");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TableZdroj",
                schema: "Data",
                table: "TableZdroj");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DebugModel",
                schema: "Data",
                table: "DebugModel");

            migrationBuilder.RenameTable(
                name: "TableZdroj",
                schema: "Data",
                newName: "Zdrojs",
                newSchema: "Identity");

            migrationBuilder.RenameTable(
                name: "DebugModel",
                schema: "Data",
                newName: "LogS",
                newSchema: "Identity");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Zdrojs",
                schema: "Identity",
                table: "Zdrojs",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LogS",
                schema: "Identity",
                table: "LogS",
                column: "IdLog");

            migrationBuilder.AddForeignKey(
                name: "FK_LokalityTable_Zdrojs_ZdrojId",
                schema: "Data",
                table: "LokalityTable",
                column: "ZdrojId",
                principalSchema: "Identity",
                principalTable: "Zdrojs",
                principalColumn: "Id");
        }
    }
}
