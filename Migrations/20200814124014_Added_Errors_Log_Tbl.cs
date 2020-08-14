using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OK_OnBoarding.Migrations
{
    public partial class Added_Errors_Log_Tbl : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ProductId",
                table: "StoreOwnerActivityLogs",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ProductId",
                table: "AdminActivityLogs",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ErrorLogs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateOfLog = table.Column<DateTime>(nullable: false),
                    Message = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ErrorLogs", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StoreOwnerActivityLogs_ProductId",
                table: "StoreOwnerActivityLogs",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_AdminActivityLogs_ProductId",
                table: "AdminActivityLogs",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_AdminActivityLogs_Products_ProductId",
                table: "AdminActivityLogs",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StoreOwnerActivityLogs_Products_ProductId",
                table: "StoreOwnerActivityLogs",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AdminActivityLogs_Products_ProductId",
                table: "AdminActivityLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_StoreOwnerActivityLogs_Products_ProductId",
                table: "StoreOwnerActivityLogs");

            migrationBuilder.DropTable(
                name: "ErrorLogs");

            migrationBuilder.DropIndex(
                name: "IX_StoreOwnerActivityLogs_ProductId",
                table: "StoreOwnerActivityLogs");

            migrationBuilder.DropIndex(
                name: "IX_AdminActivityLogs_ProductId",
                table: "AdminActivityLogs");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "StoreOwnerActivityLogs");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "AdminActivityLogs");
        }
    }
}
