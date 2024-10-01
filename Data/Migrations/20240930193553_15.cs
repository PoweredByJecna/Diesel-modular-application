using System;
using Diesel_modular_application.Models;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Diesel_modular_application.Data.Migrations
{
    /// <inheritdoc />
    public partial class _15 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Odstavky",
                schema: "Data",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    IdOdstavky = table.Column<int>(type: "int", nullable: false),
                    Distributor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Od = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Do = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Popis = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Odstavky", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Odstavky_Lokality_Id",
                        column: x => x.Id,
                        principalSchema: "Data",
                        principalTable: "Lokality",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Odstavky",
                schema: "Data");
        }
    }
}
