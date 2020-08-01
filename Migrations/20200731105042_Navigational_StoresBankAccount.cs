using Microsoft.EntityFrameworkCore.Migrations;

namespace OK_OnBoarding.Migrations
{
    public partial class Navigational_StoresBankAccount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_StoresBusinessInformation_StoreId",
                table: "StoresBusinessInformation");

            migrationBuilder.CreateIndex(
                name: "IX_StoresBusinessInformation_StoreId",
                table: "StoresBusinessInformation",
                column: "StoreId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StoresBankAccounts_StoreId",
                table: "StoresBankAccounts",
                column: "StoreId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_StoresBusinessInformation_StoreId",
                table: "StoresBusinessInformation");

            migrationBuilder.DropIndex(
                name: "IX_StoresBankAccounts_StoreId",
                table: "StoresBankAccounts");

            migrationBuilder.CreateIndex(
                name: "IX_StoresBusinessInformation_StoreId",
                table: "StoresBusinessInformation",
                column: "StoreId");
        }
    }
}
