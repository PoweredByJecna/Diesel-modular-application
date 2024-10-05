using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Diesel_modular_application.Data.Migrations
{
    /// <inheritdoc />
    public partial class Updated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
          

            migrationBuilder.CreateTable(
                name: "LokalityTable",
                schema: "Data",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Lokalita = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Klasifikace = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Adresa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Baterie = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DA = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Zásuvka = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LokalityTable", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OdstavkyTable",
                schema: "Data",
                columns: table => new
                {
                    IdOdstavky = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Distributor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Firma = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Od = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Do = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Popis = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Vstup = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Odchod = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LokalitaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OdstavkyTable", x => x.IdOdstavky);
                    table.ForeignKey(
                        name: "FK_OdstavkyTable_LokalityTable_LokalitaId",
                        column: x => x.LokalitaId,
                        principalSchema: "Data",
                        principalTable: "LokalityTable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OdstavkyTable_LokalitaId",
                schema: "Data",
                table: "OdstavkyTable",
                column: "LokalitaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            
        }
    }
}
