using Microsoft.EntityFrameworkCore.Migrations;

namespace SWENG894.Data.Migrations
{
    public partial class Workout_Updated_Exercise_Key_6 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Exercise_Workouts_WorkoutId",
                table: "Exercise");

            migrationBuilder.AlterColumn<int>(
                name: "WorkoutId",
                table: "Exercise",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Exercise_Workouts_WorkoutId",
                table: "Exercise",
                column: "WorkoutId",
                principalTable: "Workouts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Exercise_Workouts_WorkoutId",
                table: "Exercise");

            migrationBuilder.AlterColumn<int>(
                name: "WorkoutId",
                table: "Exercise",
                type: "int",
                nullable: true,
                oldClrType: typeof(int));

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
