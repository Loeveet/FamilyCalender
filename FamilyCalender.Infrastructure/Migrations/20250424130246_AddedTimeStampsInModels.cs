using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FamilyCalender.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedTimeStampsInModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedUtc",
                table: "Users",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastLoggedInUtc",
                table: "Users",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedUtc",
                table: "Events",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastEditedUtc",
                table: "Events",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedUtc",
                table: "Calendars",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastEditedUtc",
                table: "Calendars",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedUtc",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LastLoggedInUtc",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CreatedUtc",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "LastEditedUtc",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "CreatedUtc",
                table: "Calendars");

            migrationBuilder.DropColumn(
                name: "LastEditedUtc",
                table: "Calendars");
        }
    }
}
