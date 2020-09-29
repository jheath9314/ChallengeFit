using Microsoft.EntityFrameworkCore.Migrations;

namespace SWENG894.Migrations
{
    public partial class workout_migration_30 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "temp",
                table: "WorkoutResults");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "temp",
                table: "WorkoutResults",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
