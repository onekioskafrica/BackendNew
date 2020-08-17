using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OK_OnBoarding.Migrations
{
    public partial class Modified_Product_Review_Table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CustomerId",
                table: "ProductReviews",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "StoreReview",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    StoreId = table.Column<Guid>(nullable: false),
                    ParentId = table.Column<Guid>(nullable: false),
                    Title = table.Column<string>(nullable: true),
                    Rating = table.Column<int>(nullable: false),
                    IsPublished = table.Column<bool>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    PublishedAt = table.Column<DateTime>(nullable: false),
                    Content = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoreReview", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoreReview_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductReviews_CustomerId",
                table: "ProductReviews",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_StoreReview_StoreId",
                table: "StoreReview",
                column: "StoreId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductReviews_Customers_CustomerId",
                table: "ProductReviews",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "CustomerId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductReviews_Customers_CustomerId",
                table: "ProductReviews");

            migrationBuilder.DropTable(
                name: "StoreReview");

            migrationBuilder.DropIndex(
                name: "IX_ProductReviews_CustomerId",
                table: "ProductReviews");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "ProductReviews");
        }
    }
}
