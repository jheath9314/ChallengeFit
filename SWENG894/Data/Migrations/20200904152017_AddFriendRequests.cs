using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SWENG894.Data.Migrations
{
    public partial class AddFriendRequests : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FriendRequests",
                columns: table => new
                {
                    RequestedById = table.Column<string>(nullable: false),
                    RequestedForId = table.Column<string>(nullable: false),
                    RequestTime = table.Column<DateTime>(nullable: true),
                    BecameFriendsTime = table.Column<DateTime>(nullable: true),
                    FriendRequestFlag = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FriendRequests", x => new { x.RequestedById, x.RequestedForId });
                    table.ForeignKey(
                        name: "FK_FriendRequests_AspNetUsers_RequestedById",
                        column: x => x.RequestedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FriendRequests_AspNetUsers_RequestedForId",
                        column: x => x.RequestedForId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FriendRequests_RequestedForId",
                table: "FriendRequests",
                column: "RequestedForId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FriendRequests");
        }
    }
}
