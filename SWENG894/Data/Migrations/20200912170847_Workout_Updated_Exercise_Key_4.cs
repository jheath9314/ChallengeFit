using Microsoft.EntityFrameworkCore.Migrations;

namespace SWENG894.Data.Migrations
{
    public partial class Workout_Updated_Exercise_Key_4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Exercise_Workouts_WrkOtId",
                table: "Exercise");

            migrationBuilder.DropIndex(
                name: "IX_Exercise_WrkOtId",
                table: "Exercise");

            migrationBuilder.DropColumn(
                name: "WrkOtId",
                table: "Exercise");

            migrationBuilder.AddColumn<int>(
                name: "WorkoutId",
                table: "Exercise",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Exercise_WorkoutId",
                table: "Exercise",
                column: "WorkoutId");

            migrationBuilder.AddForeignKey(
                name: "FK_Exercise_Workouts_WorkoutId",
                table: "Exercise",
                column: "WorkoutId",
                principalTable: "Workouts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Exercise_Workouts_WorkoutId",
                table: "Exercise");

            migrationBuilder.DropIndex(
                name: "IX_Exercise_WorkoutId",
                table: "Exercise");

            migrationBuilder.DropColumn(
                name: "WorkoutId",
                table: "Exercise");

            migrationBuilder.AddColumn<int>(
                name: "WrkOtId",
                table: "Exercise",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Exercise_WrkOtId",
                table: "Exercise",
                column: "WrkOtId");

            migrationBuilder.AddForeignKey(
                name: "FK_Exercise_Workouts_WrkOtId",
                table: "Exercise",
                column: "WrkOtId",
                principalTable: "Workouts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
