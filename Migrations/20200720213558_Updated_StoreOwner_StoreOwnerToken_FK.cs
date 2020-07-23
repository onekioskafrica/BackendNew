using Microsoft.EntityFrameworkCore.Migrations;

namespace OK_OnBoarding.Migrations
{
    public partial class Updated_StoreOwner_StoreOwnerToken_FK : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StoreOwnerTokens_Stores_StoreOwnerId",
                table: "StoreOwnerTokens");

            migrationBuilder.AddForeignKey(
                name: "FK_StoreOwnerTokens_StoreOwners_StoreOwnerId",
                table: "StoreOwnerTokens",
                column: "StoreOwnerId",
                principalTable: "StoreOwners",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StoreOwnerTokens_StoreOwners_StoreOwnerId",
                table: "StoreOwnerTokens");

            migrationBuilder.AddForeignKey(
                name: "FK_StoreOwnerTokens_Stores_StoreOwnerId",
                table: "StoreOwnerTokens",
                column: "StoreOwnerId",
                principalTable: "Stores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
