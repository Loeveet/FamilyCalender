using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FamilyCalender.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeEventEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "End",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "Start",
                table: "Events");

            migrationBuilder.AddColumn<string>(
                name: "EventDates",
                table: "Events",
                type: "TEXT",
                nullable: false,
                defaultValue: "[]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EventDates",
                table: "Events");

            migrationBuilder.AddColumn<DateTime>(
                name: "End",
                table: "Events",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Start",
                table: "Events",
                type: "TEXT",
                nullable: true);
        }
    }
}
