using Microsoft.EntityFrameworkCore.Migrations;

namespace SWENG894.Data.Migrations
{
    public partial class Workout_Updated_Exercise_Key_1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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
                name: "workoutId",
                table: "Exercise",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Exercise_workoutId",
                table: "Exercise",
                column: "workoutId");

            migrationBuilder.AddForeignKey(
                name: "FK_Exercise_Workouts_workoutId",
                table: "Exercise",
                column: "workoutId",
                principalTable: "Workouts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Exercise_Workouts_workoutId",
                table: "Exercise");

            migrationBuilder.DropIndex(
                name: "IX_Exercise_workoutId",
                table: "Exercise");

            migrationBuilder.DropColumn(
                name: "workoutId",
                table: "Exercise");

            migrationBuilder.AddColumn<int>(
                name: "exId",
                table: "Workouts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "exId",
                table: "Exercise",
                type: "int",
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
    }
}
