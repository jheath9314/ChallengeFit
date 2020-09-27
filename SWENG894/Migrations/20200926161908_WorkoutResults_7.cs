using Microsoft.EntityFrameworkCore.Migrations;

namespace SWENG894.Migrations
{
    public partial class WorkoutResults_7 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkoutResults_AspNetUsers_UserId1",
                table: "WorkoutResults");

            migrationBuilder.DropIndex(
                name: "IX_WorkoutResults_UserId1",
                table: "WorkoutResults");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "WorkoutResults");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "WorkoutResults",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkoutResults_AspNetUsers_UserId",
                table: "WorkoutResults",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkoutResults_AspNetUsers_UserId",
                table: "WorkoutResults");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "WorkoutResults",
                type: "int",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AddColumn<string>(
                name: "UserId1",
                table: "WorkoutResults",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutResults_UserId1",
                table: "WorkoutResults",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkoutResults_AspNetUsers_UserId1",
                table: "WorkoutResults",
                column: "UserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
