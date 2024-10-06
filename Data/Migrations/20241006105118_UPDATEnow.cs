using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Diesel_modular_application.Data.Migrations
{
    /// <inheritdoc />
    public partial class UPDATEnow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TableRegiony",
                schema: "Data",
                columns: table => new
                {
                    IdRegion = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NazevRegionu = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TableRegiony", x => x.IdRegion);
                });

            migrationBuilder.CreateTable(
                name: "TableFirma",
                schema: "Data",
                columns: table => new
                {
                    IDFirmy = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NázevFirmy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RegionId = table.Column<int>(type: "int", nullable: false),
                    RegionyIdRegion = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TableFirma", x => x.IDFirmy);
                    table.ForeignKey(
                        name: "FK_TableFirma_TableRegiony_RegionyIdRegion",
                        column: x => x.RegionyIdRegion,
                        principalSchema: "Data",
                        principalTable: "TableRegiony",
                        principalColumn: "IdRegion",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "TableTechnik",
                schema: "Data",
                columns: table => new
                {
                    IdTechnika = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Jmeno = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FirmaId = table.Column<int>(type: "int", nullable: false),
                    RegionId = table.Column<int>(type: "int", nullable: false),
                    RegionyIdRegion = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TableTechnik", x => x.IdTechnika);
                    table.ForeignKey(
                        name: "FK_TableTechnik_TableFirma_FirmaId",
                        column: x => x.FirmaId,
                        principalSchema: "Data",
                        principalTable: "TableFirma",
                        principalColumn: "IDFirmy",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_TableTechnik_TableRegiony_RegionyIdRegion",
                        column: x => x.RegionyIdRegion,
                        principalSchema: "Data",
                        principalTable: "TableRegiony",
                        principalColumn: "IdRegion",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "TableDieslovani",
                schema: "Data",
                columns: table => new
                {
                    IdDieslovani = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Vstup = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Odchod = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IDodstavky = table.Column<int>(type: "int", nullable: false),
                    FirmaId = table.Column<int>(type: "int", nullable: false),
                    IdTechnik = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TableDieslovani", x => x.IdDieslovani);
                    table.ForeignKey(
                        name: "FK_TableDieslovani_OdstavkyTable_IDodstavky",
                        column: x => x.IDodstavky,
                        principalSchema: "Data",
                        principalTable: "OdstavkyTable",
                        principalColumn: "IdOdstavky",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_TableDieslovani_TableFirma_FirmaId",
                        column: x => x.FirmaId,
                        principalSchema: "Data",
                        principalTable: "TableFirma",
                        principalColumn: "IDFirmy",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_TableDieslovani_TableTechnik_IdTechnik",
                        column: x => x.IdTechnik,
                        principalSchema: "Data",
                        principalTable: "TableTechnik",
                        principalColumn: "IdTechnika",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "TablePohotovosti",
                schema: "Data",
                columns: table => new
                {
                    IdPohotovst = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Začátek = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Konec = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdTechnik = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TablePohotovosti", x => x.IdPohotovst);
                    table.ForeignKey(
                        name: "FK_TablePohotovosti_TableTechnik_IdTechnik",
                        column: x => x.IdTechnik,
                        principalSchema: "Data",
                        principalTable: "TableTechnik",
                        principalColumn: "IdTechnika",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TableDieslovani_FirmaId",
                schema: "Data",
                table: "TableDieslovani",
                column: "FirmaId");

            migrationBuilder.CreateIndex(
                name: "IX_TableDieslovani_IDodstavky",
                schema: "Data",
                table: "TableDieslovani",
                column: "IDodstavky");

            migrationBuilder.CreateIndex(
                name: "IX_TableDieslovani_IdTechnik",
                schema: "Data",
                table: "TableDieslovani",
                column: "IdTechnik");

            migrationBuilder.CreateIndex(
                name: "IX_TableFirma_RegionyIdRegion",
                schema: "Data",
                table: "TableFirma",
                column: "RegionyIdRegion");

            migrationBuilder.CreateIndex(
                name: "IX_TablePohotovosti_IdTechnik",
                schema: "Data",
                table: "TablePohotovosti",
                column: "IdTechnik");

            migrationBuilder.CreateIndex(
                name: "IX_TableTechnik_FirmaId",
                schema: "Data",
                table: "TableTechnik",
                column: "FirmaId");

            migrationBuilder.CreateIndex(
                name: "IX_TableTechnik_RegionyIdRegion",
                schema: "Data",
                table: "TableTechnik",
                column: "RegionyIdRegion");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TableDieslovani",
                schema: "Data");

            migrationBuilder.DropTable(
                name: "TablePohotovosti",
                schema: "Data");

            migrationBuilder.DropTable(
                name: "TableTechnik",
                schema: "Data");

            migrationBuilder.DropTable(
                name: "TableFirma",
                schema: "Data");

            migrationBuilder.DropTable(
                name: "TableRegiony",
                schema: "Data");
        }
    }
}
