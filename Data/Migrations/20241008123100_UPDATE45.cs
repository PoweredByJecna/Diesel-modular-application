using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Diesel_modular_application.Data.Migrations
{
    /// <inheritdoc />
    public partial class UPDATE45 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                schema: "Data",
                table: "TableTechnik",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UsersId",
                schema: "Data",
                table: "TableTechnik",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TableTechnik_UsersId",
                schema: "Data",
                table: "TableTechnik",
                column: "UsersId");

            migrationBuilder.AddForeignKey(
                name: "FK_TableTechnik_User_UsersId",
                schema: "Data",
                table: "TableTechnik",
                column: "UsersId",
                principalSchema: "Identity",
                principalTable: "User",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TableTechnik_User_UsersId",
                schema: "Data",
                table: "TableTechnik");

            migrationBuilder.DropIndex(
                name: "IX_TableTechnik_UsersId",
                schema: "Data",
                table: "TableTechnik");

            migrationBuilder.DropColumn(
                name: "UserId",
                schema: "Data",
                table: "TableTechnik");

            migrationBuilder.DropColumn(
                name: "UsersId",
                schema: "Data",
                table: "TableTechnik");
        }
    }
}
