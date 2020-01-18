using Microsoft.EntityFrameworkCore.Migrations;

namespace AlertMe.Data.Migrations
{
    public partial class UserHasAlert : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                table: "Alerts",
                nullable: true);

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
                onDelete: ReferentialAction.Restrict);
        }
    }
}
