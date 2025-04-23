using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FamilyCalender.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CategoryColorOnEvent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CategoryColor",
                table: "Events");

            migrationBuilder.AddColumn<int>(
                name: "EventCategoryColor",
                table: "Events",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EventCategoryColor",
                table: "Events");

            migrationBuilder.AddColumn<string>(
                name: "CategoryColor",
                table: "Events",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
