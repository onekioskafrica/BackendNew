using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OK_OnBoarding.Migrations
{
    public partial class Added_Password_Details_Deliveryman : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DeliveryMen",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    RiderId = table.Column<string>(nullable: true),
                    FirstName = table.Column<string>(nullable: true),
                    MiddleName = table.Column<string>(nullable: true),
                    LastName = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    Gender = table.Column<string>(nullable: true),
                    PasswordHash = table.Column<byte[]>(nullable: true),
                    PasswordSalt = table.Column<byte[]>(nullable: true),
                    DateOfBirth = table.Column<DateTime>(nullable: true),
                    Line1 = table.Column<string>(nullable: true),
                    Line2 = table.Column<string>(nullable: true),
                    City = table.Column<string>(nullable: true),
                    State = table.Column<string>(nullable: true),
                    Country = table.Column<string>(nullable: true),
                    IsVerified = table.Column<bool>(nullable: false),
                    IsEnabled = table.Column<bool>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    IsGoogleRegistered = table.Column<bool>(nullable: false),
                    IsFacebookRegistered = table.Column<bool>(nullable: false),
                    LastLoginDate = table.Column<DateTime>(nullable: true),
                    DateRegistered = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeliveryMen", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DeliverymenTokens",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TheToken = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    IsUsed = table.Column<bool>(nullable: false),
                    ExpiryDate = table.Column<DateTime>(nullable: false),
                    StatusOperation = table.Column<string>(nullable: true),
                    DeliverymanId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeliverymenTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeliverymenTokens_DeliveryMen_DeliverymanId",
                        column: x => x.DeliverymanId,
                        principalTable: "DeliveryMen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryMen_RiderId_FirstName_LastName_Email_PhoneNumber_DateOfBirth_State_IsVerified_IsEnabled_IsActive",
                table: "DeliveryMen",
                columns: new[] { "RiderId", "FirstName", "LastName", "Email", "PhoneNumber", "DateOfBirth", "State", "IsVerified", "IsEnabled", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_DeliverymenTokens_DeliverymanId",
                table: "DeliverymenTokens",
                column: "DeliverymanId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeliverymenTokens");

            migrationBuilder.DropTable(
                name: "DeliveryMen");
        }
    }
}
