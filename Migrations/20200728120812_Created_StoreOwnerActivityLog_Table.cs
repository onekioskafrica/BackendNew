using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OK_OnBoarding.Migrations
{
    public partial class Created_StoreOwnerActivityLog_Table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsClosed",
                table: "Stores",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "StoreOwnerActivityLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    StoreOwnerId = table.Column<Guid>(nullable: false),
                    StoreId = table.Column<Guid>(nullable: true),
                    Action = table.Column<string>(nullable: true),
                    DateOfAction = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoreOwnerActivityLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoreOwnerActivityLogs_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StoreOwnerActivityLogs_StoreOwners_StoreOwnerId",
                        column: x => x.StoreOwnerId,
                        principalTable: "StoreOwners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StoreOwnerActivityLogs_DateOfAction",
                table: "StoreOwnerActivityLogs",
                column: "DateOfAction");

            migrationBuilder.CreateIndex(
                name: "IX_StoreOwnerActivityLogs_StoreId",
                table: "StoreOwnerActivityLogs",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_StoreOwnerActivityLogs_StoreOwnerId",
                table: "StoreOwnerActivityLogs",
                column: "StoreOwnerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StoreOwnerActivityLogs");

            migrationBuilder.DropColumn(
                name: "IsClosed",
                table: "Stores");
        }
    }
}
