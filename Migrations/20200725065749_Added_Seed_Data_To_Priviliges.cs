using Microsoft.EntityFrameworkCore.Migrations;

namespace OK_OnBoarding.Migrations
{
    public partial class Added_Seed_Data_To_Priviliges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Priviliges",
                columns: new[] { "Id", "Action" },
                values: new object[,]
                {
                    { 1, "Create Other Admin" },
                    { 2, "Deactivate Other Admin" },
                    { 3, "Create Blogpost" },
                    { 4, "Publish Blogpost" },
                    { 5, "Deactivate Blogpost" },
                    { 6, "Approve Store Creation" },
                    { 7, "Deactivate Store" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Priviliges",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Priviliges",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Priviliges",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Priviliges",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Priviliges",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Priviliges",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Priviliges",
                keyColumn: "Id",
                keyValue: 7);
        }
    }
}
