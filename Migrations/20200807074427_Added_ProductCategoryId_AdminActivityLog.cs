using Microsoft.EntityFrameworkCore.Migrations;

namespace OK_OnBoarding.Migrations
{
    public partial class Added_ProductCategoryId_AdminActivityLog : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProductCategoryId",
                table: "AdminActivityLogs",
                nullable: true);

            migrationBuilder.InsertData(
                table: "Priviliges",
                columns: new[] { "Id", "Action" },
                values: new object[] { 8, "Create Product Category" });

            migrationBuilder.CreateIndex(
                name: "IX_AdminActivityLogs_ProductCategoryId",
                table: "AdminActivityLogs",
                column: "ProductCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_AdminActivityLogs_Categories_ProductCategoryId",
                table: "AdminActivityLogs",
                column: "ProductCategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AdminActivityLogs_Categories_ProductCategoryId",
                table: "AdminActivityLogs");

            migrationBuilder.DropIndex(
                name: "IX_AdminActivityLogs_ProductCategoryId",
                table: "AdminActivityLogs");

            migrationBuilder.DeleteData(
                table: "Priviliges",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DropColumn(
                name: "ProductCategoryId",
                table: "AdminActivityLogs");
        }
    }
}
