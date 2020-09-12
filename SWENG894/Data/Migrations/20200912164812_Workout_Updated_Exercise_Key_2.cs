using Microsoft.EntityFrameworkCore.Migrations;

namespace SWENG894.Data.Migrations
{
    public partial class Workout_Updated_Exercise_Key_2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Exercise_Workouts_workoutId",
                table: "Exercise");

            migrationBuilder.DropIndex(
                name: "IX_Exercise_workoutId",
                table: "Exercise");

            migrationBuilder.DropColumn(
                name: "exercise",
                table: "Exercise");

            migrationBuilder.DropColumn(
                name: "workoutId",
                table: "Exercise");

            migrationBuilder.RenameColumn(
                name: "time",
                table: "Workouts",
                newName: "Time");

            migrationBuilder.RenameColumn(
                name: "notes",
                table: "Workouts",
                newName: "Notes");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "Workouts",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "reps",
                table: "Exercise",
                newName: "Reps");

            migrationBuilder.AddColumn<int>(
                name: "Exer",
                table: "Exercise",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "WrkOtId",
                table: "Exercise",
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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Exercise_Workouts_WrkOtId",
                table: "Exercise");

            migrationBuilder.DropIndex(
                name: "IX_Exercise_WrkOtId",
                table: "Exercise");

            migrationBuilder.DropColumn(
                name: "Exer",
                table: "Exercise");

            migrationBuilder.DropColumn(
                name: "WrkOtId",
                table: "Exercise");

            migrationBuilder.RenameColumn(
                name: "Time",
                table: "Workouts",
                newName: "time");

            migrationBuilder.RenameColumn(
                name: "Notes",
                table: "Workouts",
                newName: "notes");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Workouts",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Reps",
                table: "Exercise",
                newName: "reps");

            migrationBuilder.AddColumn<int>(
                name: "exercise",
                table: "Exercise",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "workoutId",
                table: "Exercise",
                type: "int",
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
    }
}
