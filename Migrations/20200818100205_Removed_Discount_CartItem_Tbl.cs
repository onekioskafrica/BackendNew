using Microsoft.EntityFrameworkCore.Migrations;

namespace OK_OnBoarding.Migrations
{
    public partial class Removed_Discount_CartItem_Tbl : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Carts_CustomerId_Status_Mobile_Email_CreatedAt",
                table: "Carts");

            migrationBuilder.DropColumn(
                name: "Discount",
                table: "CartItems");

            migrationBuilder.AlterColumn<string>(
                name: "SessionId",
                table: "Carts",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Carts_CustomerId_SessionId_Status_Mobile_Email_CreatedAt",
                table: "Carts",
                columns: new[] { "CustomerId", "SessionId", "Status", "Mobile", "Email", "CreatedAt" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Carts_CustomerId_SessionId_Status_Mobile_Email_CreatedAt",
                table: "Carts");

            migrationBuilder.AlterColumn<string>(
                name: "SessionId",
                table: "Carts",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Discount",
                table: "CartItems",
                type: "decimal(18, 2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_Carts_CustomerId_Status_Mobile_Email_CreatedAt",
                table: "Carts",
                columns: new[] { "CustomerId", "Status", "Mobile", "Email", "CreatedAt" });
        }
    }
}
