using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OK_OnBoarding.Migrations
{
    public partial class Modified_AdminActivityLog_tbl : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ProductReviewId",
                table: "AdminActivityLogs",
                nullable: true);

            migrationBuilder.InsertData(
                table: "Priviliges",
                columns: new[] { "Id", "Action" },
                values: new object[] { 9, "Publish Product Review" });

            migrationBuilder.CreateIndex(
                name: "IX_AdminActivityLogs_ProductReviewId",
                table: "AdminActivityLogs",
                column: "ProductReviewId");

            migrationBuilder.AddForeignKey(
                name: "FK_AdminActivityLogs_ProductReviews_ProductReviewId",
                table: "AdminActivityLogs",
                column: "ProductReviewId",
                principalTable: "ProductReviews",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AdminActivityLogs_ProductReviews_ProductReviewId",
                table: "AdminActivityLogs");

            migrationBuilder.DropIndex(
                name: "IX_AdminActivityLogs_ProductReviewId",
                table: "AdminActivityLogs");

            migrationBuilder.DeleteData(
                table: "Priviliges",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DropColumn(
                name: "ProductReviewId",
                table: "AdminActivityLogs");
        }
    }
}
