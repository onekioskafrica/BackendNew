using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OK_OnBoarding.Migrations
{
    public partial class Added_Latitude_Longitude_Store_Tbl : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Orders_Status_SubTotal_Tax_Shipping_Total_Promo_Discount_GrandTotal_FirstName_LastName_Mobile_Email_City_State_CreatedAt",
                table: "Orders");

            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "StoresBusinessInformation",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "StoresBusinessInformation",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<DateTime>(
                name: "PaymentDate",
                table: "Payments",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "PaymentReference",
                table: "Payments",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentStatus",
                table: "Payments",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SessionId",
                table: "Orders",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_SessionId_Status_SubTotal_Tax_Shipping_Total_Promo_Discount_GrandTotal_FirstName_LastName_Mobile_Email_City_State_Cre~",
                table: "Orders",
                columns: new[] { "SessionId", "Status", "SubTotal", "Tax", "Shipping", "Total", "Promo", "Discount", "GrandTotal", "FirstName", "LastName", "Mobile", "Email", "City", "State", "CreatedAt" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Orders_SessionId_Status_SubTotal_Tax_Shipping_Total_Promo_Discount_GrandTotal_FirstName_LastName_Mobile_Email_City_State_Cre~",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "StoresBusinessInformation");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "StoresBusinessInformation");

            migrationBuilder.DropColumn(
                name: "PaymentDate",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "PaymentReference",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "PaymentStatus",
                table: "Payments");

            migrationBuilder.AlterColumn<string>(
                name: "SessionId",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_Status_SubTotal_Tax_Shipping_Total_Promo_Discount_GrandTotal_FirstName_LastName_Mobile_Email_City_State_CreatedAt",
                table: "Orders",
                columns: new[] { "Status", "SubTotal", "Tax", "Shipping", "Total", "Promo", "Discount", "GrandTotal", "FirstName", "LastName", "Mobile", "Email", "City", "State", "CreatedAt" });
        }
    }
}
