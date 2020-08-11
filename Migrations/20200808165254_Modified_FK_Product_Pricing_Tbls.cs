using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OK_OnBoarding.Migrations
{
    public partial class Modified_FK_Product_Pricing_Tbls : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductPricing_Stores_StoreId",
                table: "ProductPricing");

            migrationBuilder.DropIndex(
                name: "IX_ProductPricing_ProductId",
                table: "ProductPricing");

            migrationBuilder.DropIndex(
                name: "IX_ProductPricing_StoreId",
                table: "ProductPricing");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "ProductPricing");

            migrationBuilder.DropColumn(
                name: "StoreId",
                table: "ProductPricing");

            migrationBuilder.DropColumn(
                name: "Variation",
                table: "ProductPricing");

            migrationBuilder.AlterColumn<DateTime>(
                name: "SaleStartDate",
                table: "ProductPricing",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "SaleEndDate",
                table: "ProductPricing",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.CreateIndex(
                name: "IX_ProductPricing_ProductId",
                table: "ProductPricing",
                column: "ProductId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProductPricing_ProductId",
                table: "ProductPricing");

            migrationBuilder.AlterColumn<DateTime>(
                name: "SaleStartDate",
                table: "ProductPricing",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "SaleEndDate",
                table: "ProductPricing",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "ProductPricing",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "StoreId",
                table: "ProductPricing",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Variation",
                table: "ProductPricing",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductPricing_ProductId",
                table: "ProductPricing",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductPricing_StoreId",
                table: "ProductPricing",
                column: "StoreId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductPricing_Stores_StoreId",
                table: "ProductPricing",
                column: "StoreId",
                principalTable: "Stores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
