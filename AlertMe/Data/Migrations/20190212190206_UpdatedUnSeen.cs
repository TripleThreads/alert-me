using Microsoft.EntityFrameworkCore.Migrations;

namespace AlertMe.Data.Migrations
{
    public partial class UpdatedUnSeen : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UnSeenAlert",
                table: "Follows");

            migrationBuilder.AddColumn<int>(
                name: "UnSeenAlertId",
                table: "Follows",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Follows_UnSeenAlertId",
                table: "Follows",
                column: "UnSeenAlertId");

            migrationBuilder.AddForeignKey(
                name: "FK_Follows_Alerts_UnSeenAlertId",
                table: "Follows",
                column: "UnSeenAlertId",
                principalTable: "Alerts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Follows_Alerts_UnSeenAlertId",
                table: "Follows");

            migrationBuilder.DropIndex(
                name: "IX_Follows_UnSeenAlertId",
                table: "Follows");

            migrationBuilder.DropColumn(
                name: "UnSeenAlertId",
                table: "Follows");

            migrationBuilder.AddColumn<bool>(
                name: "UnSeenAlert",
                table: "Follows",
                nullable: false,
                defaultValue: false);
        }
    }
}
