using Microsoft.EntityFrameworkCore.Migrations;

namespace AlertMe.Data.Migrations
{
    public partial class Reverted : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Alerts_AspNetUsers_UserId",
                table: "Alerts");

            migrationBuilder.DropIndex(
                name: "IX_Alerts_UserId",
                table: "Alerts");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Alerts");

            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                table: "Alerts",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Alerts_CreatedById",
                table: "Alerts",
                column: "CreatedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Alerts_AspNetUsers_CreatedById",
                table: "Alerts",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Alerts_AspNetUsers_CreatedById",
                table: "Alerts");

            migrationBuilder.DropIndex(
                name: "IX_Alerts_CreatedById",
                table: "Alerts");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "Alerts");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Alerts",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Alerts_UserId",
                table: "Alerts",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Alerts_AspNetUsers_UserId",
                table: "Alerts",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
