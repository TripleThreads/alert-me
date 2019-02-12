using Microsoft.EntityFrameworkCore.Migrations;

namespace AlertMe.Data.Migrations
{
    public partial class AlertMigrationAddedLevel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Critical",
                table: "Alerts",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FalseAlarm",
                table: "Alerts",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Warning",
                table: "Alerts",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Critical",
                table: "Alerts");

            migrationBuilder.DropColumn(
                name: "FalseAlarm",
                table: "Alerts");

            migrationBuilder.DropColumn(
                name: "Warning",
                table: "Alerts");
        }
    }
}
