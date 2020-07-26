using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OK_OnBoarding.Migrations
{
    public partial class Removed_AdminAction_Tbl : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AdminActivityLogs_Admins_AdminId",
                table: "AdminActivityLogs");

            migrationBuilder.DropTable(
                name: "AdminActions");

            migrationBuilder.DropColumn(
                name: "ActionCarriedOut",
                table: "AdminActivityLogs");

            migrationBuilder.AlterColumn<Guid>(
                name: "StoreId",
                table: "AdminActivityLogs",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateOfAction",
                table: "AdminActivityLogs",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<Guid>(
                name: "AdminId",
                table: "AdminActivityLogs",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<string>(
                name: "Action",
                table: "AdminActivityLogs",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AdminActivityLogs_StoreId",
                table: "AdminActivityLogs",
                column: "StoreId");

            migrationBuilder.AddForeignKey(
                name: "FK_AdminActivityLogs_Admins_AdminId",
                table: "AdminActivityLogs",
                column: "AdminId",
                principalTable: "Admins",
                principalColumn: "AdminId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AdminActivityLogs_Stores_StoreId",
                table: "AdminActivityLogs",
                column: "StoreId",
                principalTable: "Stores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AdminActivityLogs_Admins_AdminId",
                table: "AdminActivityLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_AdminActivityLogs_Stores_StoreId",
                table: "AdminActivityLogs");

            migrationBuilder.DropIndex(
                name: "IX_AdminActivityLogs_StoreId",
                table: "AdminActivityLogs");

            migrationBuilder.DropColumn(
                name: "Action",
                table: "AdminActivityLogs");

            migrationBuilder.AlterColumn<Guid>(
                name: "StoreId",
                table: "AdminActivityLogs",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateOfAction",
                table: "AdminActivityLogs",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "AdminId",
                table: "AdminActivityLogs",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ActionCarriedOut",
                table: "AdminActivityLogs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "AdminActions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Activity = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminActions", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_AdminActivityLogs_Admins_AdminId",
                table: "AdminActivityLogs",
                column: "AdminId",
                principalTable: "Admins",
                principalColumn: "AdminId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
