using Microsoft.EntityFrameworkCore.Migrations;

namespace SWENG894.Data.Migrations
{
    public partial class UpdateFriendRequestStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FriendRequestFlag",
                table: "FriendRequests");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "FriendRequests",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "FriendRequests");

            migrationBuilder.AddColumn<int>(
                name: "FriendRequestFlag",
                table: "FriendRequests",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
