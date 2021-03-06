﻿using Microsoft.EntityFrameworkCore.Migrations;

namespace AlertMe.Data.Migrations
{
    public partial class AlertMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "Alerts",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "Alerts",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Alerts");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Alerts");
        }
    }
}
