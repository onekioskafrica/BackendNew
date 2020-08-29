using Microsoft.EntityFrameworkCore.Migrations;

namespace OK_OnBoarding.Migrations
{
    public partial class Added_Columns_Payment_Tbl : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Payments_GrandTotal",
                table: "Payments");

            migrationBuilder.AddColumn<decimal>(
                name: "AmountPaidToOneKiosk",
                table: "Payments",
                type: "decimal(18, 2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "AmountPaidToStore",
                table: "Payments",
                type: "decimal(18, 2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "IsSettled",
                table: "Payments",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Payments_GrandTotal_IsSettled_AmountPaidToStore_AmountPaidToOneKiosk",
                table: "Payments",
                columns: new[] { "GrandTotal", "IsSettled", "AmountPaidToStore", "AmountPaidToOneKiosk" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Payments_GrandTotal_IsSettled_AmountPaidToStore_AmountPaidToOneKiosk",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "AmountPaidToOneKiosk",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "AmountPaidToStore",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "IsSettled",
                table: "Payments");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_GrandTotal",
                table: "Payments",
                column: "GrandTotal");
        }
    }
}
