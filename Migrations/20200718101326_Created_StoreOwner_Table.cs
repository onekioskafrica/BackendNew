using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OK_OnBoarding.Migrations
{
    public partial class Created_StoreOwner_Table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Stores_StoreId_FirstName_LastName_PhoneNumber_StoreName_EmailAddress_ReferredBy_IsOneKioskContractAccepted_DateCreated_IsAct~",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "DateOfBirth",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "EmailAddress",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "Gender",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "IsOneKioskContractAccepted",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "MiddleName",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "PasswordSalt",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "ReferredBy",
                table: "Stores");

            migrationBuilder.AddColumn<string>(
                name: "StoreCreationReason",
                table: "Stores",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StoreEmailAddress",
                table: "Stores",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "StoreOwnerId",
                table: "Stores",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "StorePhoneNumber",
                table: "Stores",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "StoreOwner",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    FirstName = table.Column<string>(nullable: true),
                    MiddleName = table.Column<string>(nullable: true),
                    LastName = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    Gender = table.Column<string>(nullable: true),
                    DateOfBirth = table.Column<DateTime>(nullable: false),
                    ProfilePicUrl = table.Column<string>(nullable: true),
                    EmailAddress = table.Column<string>(nullable: true),
                    ReferredBy = table.Column<string>(nullable: true),
                    PasswordHash = table.Column<byte[]>(nullable: true),
                    PasswordSalt = table.Column<byte[]>(nullable: true),
                    IsOneKioskContractAccepted = table.Column<bool>(nullable: false),
                    IsVerified = table.Column<bool>(nullable: false),
                    IsFacebookRegistered = table.Column<bool>(nullable: false),
                    IsGoogleRegistered = table.Column<bool>(nullable: false),
                    LastLoginDate = table.Column<DateTime>(nullable: true),
                    DateRegistered = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoreOwner", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Stores_StoreOwnerId_StoreId_StoreName_DateCreated_IsActivated",
                table: "Stores",
                columns: new[] { "StoreOwnerId", "StoreId", "StoreName", "DateCreated", "IsActivated" });

            migrationBuilder.CreateIndex(
                name: "IX_StoreOwner_FirstName_LastName_PhoneNumber_DateOfBirth_EmailAddress_ReferredBy_IsVerified_IsFacebookRegistered_IsGoogleRegist~",
                table: "StoreOwner",
                columns: new[] { "FirstName", "LastName", "PhoneNumber", "DateOfBirth", "EmailAddress", "ReferredBy", "IsVerified", "IsFacebookRegistered", "IsGoogleRegistered" });

            migrationBuilder.AddForeignKey(
                name: "FK_Stores_StoreOwner_StoreOwnerId",
                table: "Stores",
                column: "StoreOwnerId",
                principalTable: "StoreOwner",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Stores_StoreOwner_StoreOwnerId",
                table: "Stores");

            migrationBuilder.DropTable(
                name: "StoreOwner");

            migrationBuilder.DropIndex(
                name: "IX_Stores_StoreOwnerId_StoreId_StoreName_DateCreated_IsActivated",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "StoreCreationReason",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "StoreEmailAddress",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "StoreOwnerId",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "StorePhoneNumber",
                table: "Stores");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfBirth",
                table: "Stores",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "EmailAddress",
                table: "Stores",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "Stores",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Gender",
                table: "Stores",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsOneKioskContractAccepted",
                table: "Stores",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "Stores",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MiddleName",
                table: "Stores",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "PasswordHash",
                table: "Stores",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "PasswordSalt",
                table: "Stores",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Stores",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReferredBy",
                table: "Stores",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Stores_StoreId_FirstName_LastName_PhoneNumber_StoreName_EmailAddress_ReferredBy_IsOneKioskContractAccepted_DateCreated_IsAct~",
                table: "Stores",
                columns: new[] { "StoreId", "FirstName", "LastName", "PhoneNumber", "StoreName", "EmailAddress", "ReferredBy", "IsOneKioskContractAccepted", "DateCreated", "IsActivated" });
        }
    }
}
