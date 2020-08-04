using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OK_OnBoarding.Migrations
{
    public partial class Modified_deliveryman_tbl : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AccountName",
                table: "DeliveryMen",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AccountNumber",
                table: "DeliveryMen",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Bank",
                table: "DeliveryMen",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CompanyId",
                table: "DeliveryMen",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GovernmentIssuedIDBack",
                table: "DeliveryMen",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GovernmentIssuedIDFront",
                table: "DeliveryMen",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "InternetAccess",
                table: "DeliveryMen",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsCompanyDriver",
                table: "DeliveryMen",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "MeansOfTransport",
                table: "DeliveryMen",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PassportUrl",
                table: "DeliveryMen",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneTypeUsed",
                table: "DeliveryMen",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UtilityBillUrl",
                table: "DeliveryMen",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccountName",
                table: "DeliveryMen");

            migrationBuilder.DropColumn(
                name: "AccountNumber",
                table: "DeliveryMen");

            migrationBuilder.DropColumn(
                name: "Bank",
                table: "DeliveryMen");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "DeliveryMen");

            migrationBuilder.DropColumn(
                name: "GovernmentIssuedIDBack",
                table: "DeliveryMen");

            migrationBuilder.DropColumn(
                name: "GovernmentIssuedIDFront",
                table: "DeliveryMen");

            migrationBuilder.DropColumn(
                name: "InternetAccess",
                table: "DeliveryMen");

            migrationBuilder.DropColumn(
                name: "IsCompanyDriver",
                table: "DeliveryMen");

            migrationBuilder.DropColumn(
                name: "MeansOfTransport",
                table: "DeliveryMen");

            migrationBuilder.DropColumn(
                name: "PassportUrl",
                table: "DeliveryMen");

            migrationBuilder.DropColumn(
                name: "PhoneTypeUsed",
                table: "DeliveryMen");

            migrationBuilder.DropColumn(
                name: "UtilityBillUrl",
                table: "DeliveryMen");
        }
    }
}
