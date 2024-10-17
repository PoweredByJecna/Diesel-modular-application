using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Diesel_modular_application.Migrations
{
    /// <inheritdoc />
    public partial class MIG1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OdstavkyTable_LokalityTable_LokalitaId",
                schema: "Data",
                table: "OdstavkyTable");

            migrationBuilder.DropForeignKey(
                name: "FK_RoleClaims_Role_RoleId",
                schema: "Identity",
                table: "RoleClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_TableDieslovani_OdstavkyTable_IDodstavky",
                schema: "Data",
                table: "TableDieslovani");

            migrationBuilder.DropForeignKey(
                name: "FK_TableDieslovani_TableFirma_FirmaId",
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
                name: "FK_TableTechnici_TableFirma_FirmaId",
                schema: "Data",
                table: "TableTechnici");

            migrationBuilder.DropForeignKey(
                name: "FK_TableTechnici_User_IdUser",
                schema: "Data",
                table: "TableTechnici");

            migrationBuilder.DropForeignKey(
                name: "FK_UserClaims_User_UserId",
                schema: "Identity",
                table: "UserClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_UserLogins_User_UserId",
                schema: "Identity",
                table: "UserLogins");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRoles_Role_RoleId",
                schema: "Identity",
                table: "UserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRoles_User_UserId",
                schema: "Identity",
                table: "UserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_UserTokens_User_UserId",
                schema: "Identity",
                table: "UserTokens");

            migrationBuilder.AddForeignKey(
                name: "FK_OdstavkyTable_LokalityTable_LokalitaId",
                schema: "Data",
                table: "OdstavkyTable",
                column: "LokalitaId",
                principalSchema: "Data",
                principalTable: "LokalityTable",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_RoleClaims_Role_RoleId",
                schema: "Identity",
                table: "RoleClaims",
                column: "RoleId",
                principalSchema: "Identity",
                principalTable: "Role",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_TableDieslovani_OdstavkyTable_IDodstavky",
                schema: "Data",
                table: "TableDieslovani",
                column: "IDodstavky",
                principalSchema: "Data",
                principalTable: "OdstavkyTable",
                principalColumn: "IdOdstavky",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_TableDieslovani_TableFirma_FirmaId",
                schema: "Data",
                table: "TableDieslovani",
                column: "FirmaId",
                principalSchema: "Data",
                principalTable: "TableFirma",
                principalColumn: "IDFirmy",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_TableDieslovani_TableTechnici_IdTechnik",
                schema: "Data",
                table: "TableDieslovani",
                column: "IdTechnik",
                principalSchema: "Data",
                principalTable: "TableTechnici",
                principalColumn: "IdTechnika",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_TablePohotovosti_TableTechnici_IdTechnik",
                schema: "Data",
                table: "TablePohotovosti",
                column: "IdTechnik",
                principalSchema: "Data",
                principalTable: "TableTechnici",
                principalColumn: "IdTechnika",
                onDelete: ReferentialAction.NoAction
);

            migrationBuilder.AddForeignKey(
                name: "FK_TablePohotovosti_User_IdUser",
                schema: "Data",
                table: "TablePohotovosti",
                column: "IdUser",
                principalSchema: "Identity",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction
);

            migrationBuilder.AddForeignKey(
                name: "FK_TableTechnici_TableFirma_FirmaId",
                schema: "Data",
                table: "TableTechnici",
                column: "FirmaId",
                principalSchema: "Data",
                principalTable: "TableFirma",
                principalColumn: "IDFirmy",
                onDelete: ReferentialAction.NoAction
);

            migrationBuilder.AddForeignKey(
                name: "FK_TableTechnici_User_IdUser",
                schema: "Data",
                table: "TableTechnici",
                column: "IdUser",
                principalSchema: "Identity",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction
);

            migrationBuilder.AddForeignKey(
                name: "FK_UserClaims_User_UserId",
                schema: "Identity",
                table: "UserClaims",
                column: "UserId",
                principalSchema: "Identity",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction
);

            migrationBuilder.AddForeignKey(
                name: "FK_UserLogins_User_UserId",
                schema: "Identity",
                table: "UserLogins",
                column: "UserId",
                principalSchema: "Identity",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction
);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRoles_Role_RoleId",
                schema: "Identity",
                table: "UserRoles",
                column: "RoleId",
                principalSchema: "Identity",
                principalTable: "Role",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction
);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRoles_User_UserId",
                schema: "Identity",
                table: "UserRoles",
                column: "UserId",
                principalSchema: "Identity",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction
);

            migrationBuilder.AddForeignKey(
                name: "FK_UserTokens_User_UserId",
                schema: "Identity",
                table: "UserTokens",
                column: "UserId",
                principalSchema: "Identity",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction
);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OdstavkyTable_LokalityTable_LokalitaId",
                schema: "Data",
                table: "OdstavkyTable");

            migrationBuilder.DropForeignKey(
                name: "FK_RoleClaims_Role_RoleId",
                schema: "Identity",
                table: "RoleClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_TableDieslovani_OdstavkyTable_IDodstavky",
                schema: "Data",
                table: "TableDieslovani");

            migrationBuilder.DropForeignKey(
                name: "FK_TableDieslovani_TableFirma_FirmaId",
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
                name: "FK_TableTechnici_TableFirma_FirmaId",
                schema: "Data",
                table: "TableTechnici");

            migrationBuilder.DropForeignKey(
                name: "FK_TableTechnici_User_IdUser",
                schema: "Data",
                table: "TableTechnici");

            migrationBuilder.DropForeignKey(
                name: "FK_UserClaims_User_UserId",
                schema: "Identity",
                table: "UserClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_UserLogins_User_UserId",
                schema: "Identity",
                table: "UserLogins");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRoles_Role_RoleId",
                schema: "Identity",
                table: "UserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRoles_User_UserId",
                schema: "Identity",
                table: "UserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_UserTokens_User_UserId",
                schema: "Identity",
                table: "UserTokens");

            migrationBuilder.AddForeignKey(
                name: "FK_OdstavkyTable_LokalityTable_LokalitaId",
                schema: "Data",
                table: "OdstavkyTable",
                column: "LokalitaId",
                principalSchema: "Data",
                principalTable: "LokalityTable",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RoleClaims_Role_RoleId",
                schema: "Identity",
                table: "RoleClaims",
                column: "RoleId",
                principalSchema: "Identity",
                principalTable: "Role",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TableDieslovani_OdstavkyTable_IDodstavky",
                schema: "Data",
                table: "TableDieslovani",
                column: "IDodstavky",
                principalSchema: "Data",
                principalTable: "OdstavkyTable",
                principalColumn: "IdOdstavky");

            migrationBuilder.AddForeignKey(
                name: "FK_TableDieslovani_TableFirma_FirmaId",
                schema: "Data",
                table: "TableDieslovani",
                column: "FirmaId",
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
                name: "FK_TableTechnici_TableFirma_FirmaId",
                schema: "Data",
                table: "TableTechnici",
                column: "FirmaId",
                principalSchema: "Data",
                principalTable: "TableFirma",
                principalColumn: "IDFirmy");

            migrationBuilder.AddForeignKey(
                name: "FK_TableTechnici_User_IdUser",
                schema: "Data",
                table: "TableTechnici",
                column: "IdUser",
                principalSchema: "Identity",
                principalTable: "User",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserClaims_User_UserId",
                schema: "Identity",
                table: "UserClaims",
                column: "UserId",
                principalSchema: "Identity",
                principalTable: "User",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserLogins_User_UserId",
                schema: "Identity",
                table: "UserLogins",
                column: "UserId",
                principalSchema: "Identity",
                principalTable: "User",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserRoles_Role_RoleId",
                schema: "Identity",
                table: "UserRoles",
                column: "RoleId",
                principalSchema: "Identity",
                principalTable: "Role",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserRoles_User_UserId",
                schema: "Identity",
                table: "UserRoles",
                column: "UserId",
                principalSchema: "Identity",
                principalTable: "User",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserTokens_User_UserId",
                schema: "Identity",
                table: "UserTokens",
                column: "UserId",
                principalSchema: "Identity",
                principalTable: "User",
                principalColumn: "Id");
        }
    }
}
