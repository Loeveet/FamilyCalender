using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FamilyCalender.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class TextOnEvent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Text",
                table: "Events",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Text",
                table: "Events");
        }
    }
}
