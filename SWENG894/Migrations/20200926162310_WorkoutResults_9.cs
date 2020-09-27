using Microsoft.EntityFrameworkCore.Migrations;

namespace SWENG894.Migrations
{
    public partial class WorkoutResults_9 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "temp",
                table: "WorkoutResults",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "temp",
                table: "WorkoutResults");
        }
    }
}
