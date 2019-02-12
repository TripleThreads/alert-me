using Microsoft.EntityFrameworkCore.Migrations;

namespace AlertMe.Data.Migrations
{
    public partial class Reverted2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Alerts_AspNetUsers_CreatedById",
                table: "Alerts");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedById",
                table: "Alerts",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AddForeignKey(
                name: "FK_Alerts_AspNetUsers_CreatedById",
                table: "Alerts",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Alerts_AspNetUsers_CreatedById",
                table: "Alerts");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedById",
                table: "Alerts",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Alerts_AspNetUsers_CreatedById",
                table: "Alerts",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
