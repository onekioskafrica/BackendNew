using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OK_OnBoarding.Migrations
{
    public partial class Modified_AdminActivityLogs_Tbl_StoreReview : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "StoreReviewId",
                table: "AdminActivityLogs",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AdminActivityLogs_StoreReviewId",
                table: "AdminActivityLogs",
                column: "StoreReviewId");

            migrationBuilder.AddForeignKey(
                name: "FK_AdminActivityLogs_StoreReviews_StoreReviewId",
                table: "AdminActivityLogs",
                column: "StoreReviewId",
                principalTable: "StoreReviews",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AdminActivityLogs_StoreReviews_StoreReviewId",
                table: "AdminActivityLogs");

            migrationBuilder.DropIndex(
                name: "IX_AdminActivityLogs_StoreReviewId",
                table: "AdminActivityLogs");

            migrationBuilder.DropColumn(
                name: "StoreReviewId",
                table: "AdminActivityLogs");
        }
    }
}
