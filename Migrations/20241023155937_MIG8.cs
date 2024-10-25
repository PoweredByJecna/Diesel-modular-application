using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Diesel_modular_application.Migrations
{
    /// <inheritdoc />
    public partial class MIG8 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
                  
        migrationBuilder.AddForeignKey(
                    name: "FK_OdstavkyTable_LokalityTable_LokalitaId",
                    schema: "Data",
                    table: "OdstavkyTable",
                    column: "LokalitaId",
                    principalSchema: "Data",
                    principalTable: "LokalityTable",
                    principalColumn: "Id");
            
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
                 migrationBuilder.DropForeignKey(
                name: "FK_OdstavkyTable_LokalityTable_LokalitaId",
                schema: "Data",
                table: "OdstavkyTable");
        }
    }
}
