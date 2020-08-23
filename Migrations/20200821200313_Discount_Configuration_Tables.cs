using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OK_OnBoarding.Migrations
{
    public partial class Discount_Configuration_Tables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Orders_Status_SubTotal_ItemDiscount_Tax_Shipping_Total_Promo_Discount_GrandTotal_FirstName_LastName_Mobile_Email_City_State_~",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "ItemDiscount",
                table: "Orders",
                newName: "StoreOwnerShippingDiscount");

            migrationBuilder.AddColumn<Guid>(
                name: "DiscountId",
                table: "StoreOwnerActivityLogs",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsSalePriceSet",
                table: "ProductPricing",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "OnekioskPriceDiscount",
                table: "Orders",
                type: "decimal(18, 2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "OnekioskShippingDiscount",
                table: "Orders",
                type: "decimal(18, 2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PriceDiscount",
                table: "Orders",
                type: "decimal(18, 2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ShippingDiscount",
                table: "Orders",
                type: "decimal(18, 2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "StoreOwnerPriceDiscount",
                table: "Orders",
                type: "decimal(18, 2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<Guid>(
                name: "StoreId",
                table: "CartItems",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "DiscountId",
                table: "AdminActivityLogs",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Configurations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Key = table.Column<string>(nullable: true),
                    Value = table.Column<decimal>(type: "decimal(18, 2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Configurations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Coupons",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    IsStoreOwnerConfigured = table.Column<bool>(nullable: false),
                    StoreId = table.Column<Guid>(nullable: true),
                    IsForAllStoresOwnByAStoreOwner = table.Column<bool>(nullable: false),
                    StoreOwnerId = table.Column<Guid>(nullable: true),
                    Code = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    AdminId = table.Column<Guid>(nullable: true),
                    IsAdminConfigured = table.Column<bool>(nullable: false),
                    IsForCategory = table.Column<bool>(nullable: false),
                    CategoryId = table.Column<int>(nullable: false),
                    IsForProduct = table.Column<bool>(nullable: false),
                    ProductId = table.Column<Guid>(nullable: true),
                    IsForAllProducts = table.Column<bool>(nullable: false),
                    IsForShipping = table.Column<bool>(nullable: false),
                    IsForPrice = table.Column<bool>(nullable: false),
                    IsPercentageDiscount = table.Column<bool>(nullable: false),
                    PercentageDiscount = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    IsAmountDiscount = table.Column<bool>(nullable: false),
                    AmountDiscount = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    IsSetPrice = table.Column<bool>(nullable: false),
                    SetPrice = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    IsSlotSet = table.Column<bool>(nullable: false),
                    AllocatedSlot = table.Column<int>(nullable: false),
                    SlotUsed = table.Column<int>(nullable: false),
                    StartDate = table.Column<DateTime>(nullable: true),
                    EndDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Coupons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Coupons_Admins_AdminId",
                        column: x => x.AdminId,
                        principalTable: "Admins",
                        principalColumn: "AdminId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Coupons_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Coupons_StoreOwners_StoreOwnerId",
                        column: x => x.StoreOwnerId,
                        principalTable: "StoreOwners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                table: "Priviliges",
                keyColumn: "Id",
                keyValue: 2,
                column: "Action",
                value: "Activate/Deactivate Other Admin");

            migrationBuilder.UpdateData(
                table: "Priviliges",
                keyColumn: "Id",
                keyValue: 5,
                column: "Action",
                value: "Activate/Deactivate Blogpost");

            migrationBuilder.UpdateData(
                table: "Priviliges",
                keyColumn: "Id",
                keyValue: 7,
                column: "Action",
                value: "Activate/Deactivate Store");

            migrationBuilder.InsertData(
                table: "Priviliges",
                columns: new[] { "Id", "Action" },
                values: new object[,]
                {
                    { 10, "Set Discount Code" },
                    { 11, "Activate/Disactivate Discount Code" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Orders_Status_SubTotal_Tax_Shipping_Total_Promo_Discount_GrandTotal_FirstName_LastName_Mobile_Email_City_State_CreatedAt",
                table: "Orders",
                columns: new[] { "Status", "SubTotal", "Tax", "Shipping", "Total", "Promo", "Discount", "GrandTotal", "FirstName", "LastName", "Mobile", "Email", "City", "State", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_StoreId",
                table: "CartItems",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_AdminActivityLogs_DiscountId",
                table: "AdminActivityLogs",
                column: "DiscountId");

            migrationBuilder.CreateIndex(
                name: "IX_Configurations_Key",
                table: "Configurations",
                column: "Key");

            migrationBuilder.CreateIndex(
                name: "IX_Coupons_AdminId",
                table: "Coupons",
                column: "AdminId");

            migrationBuilder.CreateIndex(
                name: "IX_Coupons_StoreId",
                table: "Coupons",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_Coupons_StoreOwnerId",
                table: "Coupons",
                column: "StoreOwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Coupons_IsStoreOwnerConfigured_StoreId_IsForAllStoresOwnByAStoreOwner_StoreOwnerId_AdminId_IsAdminConfigured_Code_IsActive_I~",
                table: "Coupons",
                columns: new[] { "IsStoreOwnerConfigured", "StoreId", "IsForAllStoresOwnByAStoreOwner", "StoreOwnerId", "AdminId", "IsAdminConfigured", "Code", "IsActive", "IsForCategory", "CategoryId", "IsForProduct", "ProductId", "IsForShipping", "IsPercentageDiscount", "IsAmountDiscount", "IsSetPrice", "IsSlotSet", "StartDate", "EndDate" });

            migrationBuilder.AddForeignKey(
                name: "FK_AdminActivityLogs_Coupons_DiscountId",
                table: "AdminActivityLogs",
                column: "DiscountId",
                principalTable: "Coupons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CartItems_Stores_StoreId",
                table: "CartItems",
                column: "StoreId",
                principalTable: "Stores",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AdminActivityLogs_Coupons_DiscountId",
                table: "AdminActivityLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_CartItems_Stores_StoreId",
                table: "CartItems");

            migrationBuilder.DropTable(
                name: "Configurations");

            migrationBuilder.DropTable(
                name: "Coupons");

            migrationBuilder.DropIndex(
                name: "IX_Orders_Status_SubTotal_Tax_Shipping_Total_Promo_Discount_GrandTotal_FirstName_LastName_Mobile_Email_City_State_CreatedAt",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_CartItems_StoreId",
                table: "CartItems");

            migrationBuilder.DropIndex(
                name: "IX_AdminActivityLogs_DiscountId",
                table: "AdminActivityLogs");

            migrationBuilder.DeleteData(
                table: "Priviliges",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Priviliges",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DropColumn(
                name: "DiscountId",
                table: "StoreOwnerActivityLogs");

            migrationBuilder.DropColumn(
                name: "IsSalePriceSet",
                table: "ProductPricing");

            migrationBuilder.DropColumn(
                name: "OnekioskPriceDiscount",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "OnekioskShippingDiscount",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PriceDiscount",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ShippingDiscount",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "StoreOwnerPriceDiscount",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "StoreId",
                table: "CartItems");

            migrationBuilder.DropColumn(
                name: "DiscountId",
                table: "AdminActivityLogs");

            migrationBuilder.RenameColumn(
                name: "StoreOwnerShippingDiscount",
                table: "Orders",
                newName: "ItemDiscount");

            migrationBuilder.UpdateData(
                table: "Priviliges",
                keyColumn: "Id",
                keyValue: 2,
                column: "Action",
                value: "Deactivate Other Admin");

            migrationBuilder.UpdateData(
                table: "Priviliges",
                keyColumn: "Id",
                keyValue: 5,
                column: "Action",
                value: "Deactivate Blogpost");

            migrationBuilder.UpdateData(
                table: "Priviliges",
                keyColumn: "Id",
                keyValue: 7,
                column: "Action",
                value: "Deactivate Store");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_Status_SubTotal_ItemDiscount_Tax_Shipping_Total_Promo_Discount_GrandTotal_FirstName_LastName_Mobile_Email_City_State_~",
                table: "Orders",
                columns: new[] { "Status", "SubTotal", "ItemDiscount", "Tax", "Shipping", "Total", "Promo", "Discount", "GrandTotal", "FirstName", "LastName", "Mobile", "Email", "City", "State", "CreatedAt" });
        }
    }
}
