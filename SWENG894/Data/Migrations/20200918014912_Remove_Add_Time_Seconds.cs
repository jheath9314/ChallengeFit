using Microsoft.EntityFrameworkCore.Migrations;

namespace SWENG894.Data.Migrations
{
    public partial class Remove_Add_Time_Seconds : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "timeSeconds",
                table: "Workouts");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "timeSeconds",
                table: "Workouts",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
