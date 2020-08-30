using Microsoft.EntityFrameworkCore.Migrations;

namespace OK_OnBoarding.Migrations
{
    public partial class Modified_Payment_Tbl : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Payments_SessionId_GrandTotal_IsSettled_AmountPaidToStore_AmountPaidToOneKiosk",
                table: "Payments");

            migrationBuilder.RenameColumn(
                name: "GrandTotal",
                table: "Payments",
                newName: "Total");

            migrationBuilder.AddColumn<int>(
                name: "CartItemId",
                table: "Payments",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "StoreDiscountOnPrice",
                table: "Payments",
                type: "decimal(18, 2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "StoreDiscountOnShipping",
                table: "Payments",
                type: "decimal(18, 2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_Payments_CartItemId_SessionId_Total_IsSettled_AmountPaidToStore_AmountPaidToOneKiosk",
                table: "Payments",
                columns: new[] { "CartItemId", "SessionId", "Total", "IsSettled", "AmountPaidToStore", "AmountPaidToOneKiosk" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Payments_CartItemId_SessionId_Total_IsSettled_AmountPaidToStore_AmountPaidToOneKiosk",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "CartItemId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "StoreDiscountOnPrice",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "StoreDiscountOnShipping",
                table: "Payments");

            migrationBuilder.RenameColumn(
                name: "Total",
                table: "Payments",
                newName: "GrandTotal");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_SessionId_GrandTotal_IsSettled_AmountPaidToStore_AmountPaidToOneKiosk",
                table: "Payments",
                columns: new[] { "SessionId", "GrandTotal", "IsSettled", "AmountPaidToStore", "AmountPaidToOneKiosk" });
        }
    }
}
