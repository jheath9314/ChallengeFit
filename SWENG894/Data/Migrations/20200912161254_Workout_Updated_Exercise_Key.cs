using Microsoft.EntityFrameworkCore.Migrations;

namespace SWENG894.Data.Migrations
{
    public partial class Workout_Updated_Exercise_Key : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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
                name: "exId",
                table: "Workouts",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "exId",
                table: "Exercise",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Exercise_exId",
                table: "Exercise",
                column: "exId");

            migrationBuilder.AddForeignKey(
                name: "FK_Exercise_Workouts_exId",
                table: "Exercise",
                column: "exId",
                principalTable: "Workouts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Exercise_Workouts_exId",
                table: "Exercise");

            migrationBuilder.DropIndex(
                name: "IX_Exercise_exId",
                table: "Exercise");

            migrationBuilder.DropColumn(
                name: "exId",
                table: "Workouts");

            migrationBuilder.DropColumn(
                name: "exId",
                table: "Exercise");

            migrationBuilder.AddColumn<int>(
                name: "WorkoutId",
                table: "Exercise",
                type: "int",
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
    }
}
