using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Diesel_modular_application.Migrations
{
    /// <inheritdoc />
    public partial class MIG : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TableDieslovani_TechniS_IdTechnik",
                schema: "Data",
                table: "TableDieslovani");

            migrationBuilder.DropForeignKey(
                name: "FK_TablePohotovosti_TechniS_IdTechnik",
                schema: "Data",
                table: "TablePohotovosti");

            migrationBuilder.DropForeignKey(
                name: "FK_TechniS_TableFirma_FirmaId",
                schema: "Identity",
                table: "TechniS");

            migrationBuilder.DropForeignKey(
                name: "FK_TechniS_User_UserId",
                schema: "Identity",
                table: "TechniS");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TechniS",
                schema: "Identity",
                table: "TechniS");

            migrationBuilder.RenameTable(
                name: "TechniS",
                schema: "Identity",
                newName: "TableTechnici",
                newSchema: "Data");

            migrationBuilder.RenameIndex(
                name: "IX_TechniS_UserId",
                schema: "Data",
                table: "TableTechnici",
                newName: "IX_TableTechnici_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_TechniS_FirmaId",
                schema: "Data",
                table: "TableTechnici",
                newName: "IX_TableTechnici_FirmaId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TableTechnici",
                schema: "Data",
                table: "TableTechnici",
                column: "IdTechnika");

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
                name: "FK_TableTechnici_TableFirma_FirmaId",
                schema: "Data",
                table: "TableTechnici",
                column: "FirmaId",
                principalSchema: "Data",
                principalTable: "TableFirma",
                principalColumn: "IDFirmy",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TableTechnici_User_UserId",
                schema: "Data",
                table: "TableTechnici",
                column: "UserId",
                principalSchema: "Identity",
                principalTable: "User",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TableDieslovani_TableTechnici_IdTechnik",
                schema: "Data",
                table: "TableDieslovani");

            migrationBuilder.DropForeignKey(
                name: "FK_TablePohotovosti_TableTechnici_IdTechnik",
                schema: "Data",
                table: "TablePohotovosti");

            migrationBuilder.DropForeignKey(
                name: "FK_TableTechnici_TableFirma_FirmaId",
                schema: "Data",
                table: "TableTechnici");

            migrationBuilder.DropForeignKey(
                name: "FK_TableTechnici_User_UserId",
                schema: "Data",
                table: "TableTechnici");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TableTechnici",
                schema: "Data",
                table: "TableTechnici");

            migrationBuilder.RenameTable(
                name: "TableTechnici",
                schema: "Data",
                newName: "TechniS",
                newSchema: "Identity");

            migrationBuilder.RenameIndex(
                name: "IX_TableTechnici_UserId",
                schema: "Identity",
                table: "TechniS",
                newName: "IX_TechniS_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_TableTechnici_FirmaId",
                schema: "Identity",
                table: "TechniS",
                newName: "IX_TechniS_FirmaId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TechniS",
                schema: "Identity",
                table: "TechniS",
                column: "IdTechnika");

            migrationBuilder.AddForeignKey(
                name: "FK_TableDieslovani_TechniS_IdTechnik",
                schema: "Data",
                table: "TableDieslovani",
                column: "IdTechnik",
                principalSchema: "Identity",
                principalTable: "TechniS",
                principalColumn: "IdTechnika",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TablePohotovosti_TechniS_IdTechnik",
                schema: "Data",
                table: "TablePohotovosti",
                column: "IdTechnik",
                principalSchema: "Identity",
                principalTable: "TechniS",
                principalColumn: "IdTechnika",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TechniS_TableFirma_FirmaId",
                schema: "Identity",
                table: "TechniS",
                column: "FirmaId",
                principalSchema: "Data",
                principalTable: "TableFirma",
                principalColumn: "IDFirmy",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TechniS_User_UserId",
                schema: "Identity",
                table: "TechniS",
                column: "UserId",
                principalSchema: "Identity",
                principalTable: "User",
                principalColumn: "Id");
        }
    }
}
