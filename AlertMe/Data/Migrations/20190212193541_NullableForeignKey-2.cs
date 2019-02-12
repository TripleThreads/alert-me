using Microsoft.EntityFrameworkCore.Migrations;

namespace AlertMe.Data.Migrations
{
    public partial class NullableForeignKey2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Follows_Alerts_UnSeenAlertId",
                table: "Follows");

            migrationBuilder.AlterColumn<int>(
                name: "UnSeenAlertId",
                table: "Follows",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_Follows_Alerts_UnSeenAlertId",
                table: "Follows",
                column: "UnSeenAlertId",
                principalTable: "Alerts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Follows_Alerts_UnSeenAlertId",
                table: "Follows");

            migrationBuilder.AlterColumn<int>(
                name: "UnSeenAlertId",
                table: "Follows",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Follows_Alerts_UnSeenAlertId",
                table: "Follows",
                column: "UnSeenAlertId",
                principalTable: "Alerts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
