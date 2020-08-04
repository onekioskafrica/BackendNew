using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OK_OnBoarding.Migrations
{
    public partial class Added_DeliverymanId_AdminActivityLog : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DeliverymanId",
                table: "AdminActivityLogs",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AdminActivityLogs_DeliverymanId",
                table: "AdminActivityLogs",
                column: "DeliverymanId");

            migrationBuilder.AddForeignKey(
                name: "FK_AdminActivityLogs_DeliveryMen_DeliverymanId",
                table: "AdminActivityLogs",
                column: "DeliverymanId",
                principalTable: "DeliveryMen",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AdminActivityLogs_DeliveryMen_DeliverymanId",
                table: "AdminActivityLogs");

            migrationBuilder.DropIndex(
                name: "IX_AdminActivityLogs_DeliverymanId",
                table: "AdminActivityLogs");

            migrationBuilder.DropColumn(
                name: "DeliverymanId",
                table: "AdminActivityLogs");
        }
    }
}
