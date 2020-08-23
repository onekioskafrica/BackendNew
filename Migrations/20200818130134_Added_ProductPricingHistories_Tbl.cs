using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OK_OnBoarding.Migrations
{
    public partial class Added_ProductPricingHistories_Tbl : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProductPricingHistories",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StoreOwnerId = table.Column<Guid>(nullable: false),
                    ProductId = table.Column<Guid>(nullable: false),
                    OldPrice = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    OldSalePrice = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    OldSaleStartDate = table.Column<DateTime>(nullable: true),
                    OldSaleEndDate = table.Column<DateTime>(nullable: true),
                    DateOfAction = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductPricingHistories", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductPricingHistories_StoreOwnerId_ProductId_DateOfAction",
                table: "ProductPricingHistories",
                columns: new[] { "StoreOwnerId", "ProductId", "DateOfAction" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductPricingHistories");
        }
    }
}
