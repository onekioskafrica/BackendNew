using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OK_OnBoarding.Migrations
{
    public partial class Modified_FK_Product_Category_Tbls : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductCategories_Products_ProductId1",
                table: "ProductCategories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductCategories",
                table: "ProductCategories");

            migrationBuilder.DropIndex(
                name: "IX_ProductCategories_ProductId1",
                table: "ProductCategories");

            migrationBuilder.DropColumn(
                name: "ProductId1",
                table: "ProductCategories");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "ProductCategories",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductCategories",
                table: "ProductCategories",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductCategories_Products_ProductId",
                table: "ProductCategories",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductCategories_Products_ProductId",
                table: "ProductCategories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductCategories",
                table: "ProductCategories");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "ProductCategories");

            migrationBuilder.AddColumn<Guid>(
                name: "ProductId1",
                table: "ProductCategories",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductCategories",
                table: "ProductCategories",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductCategories_ProductId1",
                table: "ProductCategories",
                column: "ProductId1");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductCategories_Products_ProductId1",
                table: "ProductCategories",
                column: "ProductId1",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
