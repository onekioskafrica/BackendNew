using Microsoft.EntityFrameworkCore.Migrations;

namespace OK_OnBoarding.Migrations
{
    public partial class Remove_Order_ID_Payment_Tbl : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Orders_OrderId",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Payments_OrderId",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Payments_GrandTotal_IsSettled_AmountPaidToStore_AmountPaidToOneKiosk",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "Payments");

            migrationBuilder.AddColumn<string>(
                name: "SessionId",
                table: "Payments",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payments_SessionId_GrandTotal_IsSettled_AmountPaidToStore_AmountPaidToOneKiosk",
                table: "Payments",
                columns: new[] { "SessionId", "GrandTotal", "IsSettled", "AmountPaidToStore", "AmountPaidToOneKiosk" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Payments_SessionId_GrandTotal_IsSettled_AmountPaidToStore_AmountPaidToOneKiosk",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "SessionId",
                table: "Payments");

            migrationBuilder.AddColumn<int>(
                name: "OrderId",
                table: "Payments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Payments_OrderId",
                table: "Payments",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_GrandTotal_IsSettled_AmountPaidToStore_AmountPaidToOneKiosk",
                table: "Payments",
                columns: new[] { "GrandTotal", "IsSettled", "AmountPaidToStore", "AmountPaidToOneKiosk" });

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Orders_OrderId",
                table: "Payments",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
