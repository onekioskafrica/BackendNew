using Microsoft.EntityFrameworkCore.Migrations;

namespace OK_OnBoarding.Migrations
{
    public partial class Created_StoreOwner_Table_DbSet : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Stores_StoreOwner_StoreOwnerId",
                table: "Stores");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StoreOwner",
                table: "StoreOwner");

            migrationBuilder.RenameTable(
                name: "StoreOwner",
                newName: "StoreOwners");

            migrationBuilder.RenameIndex(
                name: "IX_StoreOwner_FirstName_LastName_PhoneNumber_DateOfBirth_EmailAddress_ReferredBy_IsVerified_IsFacebookRegistered_IsGoogleRegist~",
                table: "StoreOwners",
                newName: "IX_StoreOwners_FirstName_LastName_PhoneNumber_DateOfBirth_EmailAddress_ReferredBy_IsVerified_IsFacebookRegistered_IsGoogleRegis~");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StoreOwners",
                table: "StoreOwners",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Stores_StoreOwners_StoreOwnerId",
                table: "Stores",
                column: "StoreOwnerId",
                principalTable: "StoreOwners",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Stores_StoreOwners_StoreOwnerId",
                table: "Stores");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StoreOwners",
                table: "StoreOwners");

            migrationBuilder.RenameTable(
                name: "StoreOwners",
                newName: "StoreOwner");

            migrationBuilder.RenameIndex(
                name: "IX_StoreOwners_FirstName_LastName_PhoneNumber_DateOfBirth_EmailAddress_ReferredBy_IsVerified_IsFacebookRegistered_IsGoogleRegis~",
                table: "StoreOwner",
                newName: "IX_StoreOwner_FirstName_LastName_PhoneNumber_DateOfBirth_EmailAddress_ReferredBy_IsVerified_IsFacebookRegistered_IsGoogleRegist~");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StoreOwner",
                table: "StoreOwner",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Stores_StoreOwner_StoreOwnerId",
                table: "Stores",
                column: "StoreOwnerId",
                principalTable: "StoreOwner",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
