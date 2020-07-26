using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OK_OnBoarding.Migrations
{
    public partial class Removed_SuperAdminActions_Tbl : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SuperAdminActions");

            migrationBuilder.DropIndex(
                name: "IX_SuperAdminActivityLogs_AdminId_ActionCarriedOutId_StoreId_DateOfAction",
                table: "SuperAdminActivityLogs");

            migrationBuilder.DropColumn(
                name: "ActionCarriedOutId",
                table: "SuperAdminActivityLogs");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateOfAction",
                table: "SuperAdminActivityLogs",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<string>(
                name: "Action",
                table: "SuperAdminActivityLogs",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SuperAdminActivityLogs_AdminId_StoreId_DateOfAction",
                table: "SuperAdminActivityLogs",
                columns: new[] { "AdminId", "StoreId", "DateOfAction" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SuperAdminActivityLogs_AdminId_StoreId_DateOfAction",
                table: "SuperAdminActivityLogs");

            migrationBuilder.DropColumn(
                name: "Action",
                table: "SuperAdminActivityLogs");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateOfAction",
                table: "SuperAdminActivityLogs",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ActionCarriedOutId",
                table: "SuperAdminActivityLogs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "SuperAdminActions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Activity = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SuperAdminActions", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SuperAdminActivityLogs_AdminId_ActionCarriedOutId_StoreId_DateOfAction",
                table: "SuperAdminActivityLogs",
                columns: new[] { "AdminId", "ActionCarriedOutId", "StoreId", "DateOfAction" });
        }
    }
}
