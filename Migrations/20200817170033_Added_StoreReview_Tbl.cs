using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OK_OnBoarding.Migrations
{
    public partial class Added_StoreReview_Tbl : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StoreReview_Stores_StoreId",
                table: "StoreReview");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StoreReview",
                table: "StoreReview");

            migrationBuilder.RenameTable(
                name: "StoreReview",
                newName: "StoreReviews");

            migrationBuilder.RenameIndex(
                name: "IX_StoreReview_StoreId",
                table: "StoreReviews",
                newName: "IX_StoreReviews_StoreId");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "StoreReviews",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CustomerId",
                table: "StoreReviews",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_StoreReviews",
                table: "StoreReviews",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_StoreReviews_CustomerId",
                table: "StoreReviews",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_StoreReviews_Title_Rating_IsPublished_CreatedAt_PublishedAt",
                table: "StoreReviews",
                columns: new[] { "Title", "Rating", "IsPublished", "CreatedAt", "PublishedAt" });

            migrationBuilder.AddForeignKey(
                name: "FK_StoreReviews_Customers_CustomerId",
                table: "StoreReviews",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "CustomerId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StoreReviews_Stores_StoreId",
                table: "StoreReviews",
                column: "StoreId",
                principalTable: "Stores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StoreReviews_Customers_CustomerId",
                table: "StoreReviews");

            migrationBuilder.DropForeignKey(
                name: "FK_StoreReviews_Stores_StoreId",
                table: "StoreReviews");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StoreReviews",
                table: "StoreReviews");

            migrationBuilder.DropIndex(
                name: "IX_StoreReviews_CustomerId",
                table: "StoreReviews");

            migrationBuilder.DropIndex(
                name: "IX_StoreReviews_Title_Rating_IsPublished_CreatedAt_PublishedAt",
                table: "StoreReviews");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "StoreReviews");

            migrationBuilder.RenameTable(
                name: "StoreReviews",
                newName: "StoreReview");

            migrationBuilder.RenameIndex(
                name: "IX_StoreReviews_StoreId",
                table: "StoreReview",
                newName: "IX_StoreReview_StoreId");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "StoreReview",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_StoreReview",
                table: "StoreReview",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StoreReview_Stores_StoreId",
                table: "StoreReview",
                column: "StoreId",
                principalTable: "Stores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
