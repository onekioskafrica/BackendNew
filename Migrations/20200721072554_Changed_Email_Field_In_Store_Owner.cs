using Microsoft.EntityFrameworkCore.Migrations;

namespace OK_OnBoarding.Migrations
{
    public partial class Changed_Email_Field_In_Store_Owner : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_StoreOwners_FirstName_LastName_PhoneNumber_DateOfBirth_EmailAddress_ReferredBy_IsVerified_IsFacebookRegistered_IsGoogleRegis~",
                table: "StoreOwners");

            migrationBuilder.DropColumn(
                name: "EmailAddress",
                table: "StoreOwners");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "StoreOwners",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StoreOwners_FirstName_LastName_PhoneNumber_DateOfBirth_Email_ReferredBy_IsVerified_IsFacebookRegistered_IsGoogleRegistered",
                table: "StoreOwners",
                columns: new[] { "FirstName", "LastName", "PhoneNumber", "DateOfBirth", "Email", "ReferredBy", "IsVerified", "IsFacebookRegistered", "IsGoogleRegistered" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_StoreOwners_FirstName_LastName_PhoneNumber_DateOfBirth_Email_ReferredBy_IsVerified_IsFacebookRegistered_IsGoogleRegistered",
                table: "StoreOwners");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "StoreOwners");

            migrationBuilder.AddColumn<string>(
                name: "EmailAddress",
                table: "StoreOwners",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StoreOwners_FirstName_LastName_PhoneNumber_DateOfBirth_EmailAddress_ReferredBy_IsVerified_IsFacebookRegistered_IsGoogleRegis~",
                table: "StoreOwners",
                columns: new[] { "FirstName", "LastName", "PhoneNumber", "DateOfBirth", "EmailAddress", "ReferredBy", "IsVerified", "IsFacebookRegistered", "IsGoogleRegistered" });
        }
    }
}
