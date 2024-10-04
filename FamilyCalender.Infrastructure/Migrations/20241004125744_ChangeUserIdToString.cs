using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FamilyCalender.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeUserIdToString : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CalendarAccesses_AspNetUsers_UserId1",
                table: "CalendarAccesses");

            migrationBuilder.DropForeignKey(
                name: "FK_Calendars_AspNetUsers_OwnerId1",
                table: "Calendars");

            migrationBuilder.DropIndex(
                name: "IX_Calendars_OwnerId1",
                table: "Calendars");

            migrationBuilder.DropIndex(
                name: "IX_CalendarAccesses_UserId1",
                table: "CalendarAccesses");

            migrationBuilder.DropColumn(
                name: "OwnerId1",
                table: "Calendars");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "CalendarAccesses");

            migrationBuilder.AlterColumn<string>(
                name: "OwnerId",
                table: "Calendars",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "CalendarAccesses",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.CreateIndex(
                name: "IX_Calendars_OwnerId",
                table: "Calendars",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_CalendarAccesses_UserId",
                table: "CalendarAccesses",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_CalendarAccesses_AspNetUsers_UserId",
                table: "CalendarAccesses",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Calendars_AspNetUsers_OwnerId",
                table: "Calendars",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CalendarAccesses_AspNetUsers_UserId",
                table: "CalendarAccesses");

            migrationBuilder.DropForeignKey(
                name: "FK_Calendars_AspNetUsers_OwnerId",
                table: "Calendars");

            migrationBuilder.DropIndex(
                name: "IX_Calendars_OwnerId",
                table: "Calendars");

            migrationBuilder.DropIndex(
                name: "IX_CalendarAccesses_UserId",
                table: "CalendarAccesses");

            migrationBuilder.AlterColumn<int>(
                name: "OwnerId",
                table: "Calendars",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OwnerId1",
                table: "Calendars",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "CalendarAccesses",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId1",
                table: "CalendarAccesses",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Calendars_OwnerId1",
                table: "Calendars",
                column: "OwnerId1");

            migrationBuilder.CreateIndex(
                name: "IX_CalendarAccesses_UserId1",
                table: "CalendarAccesses",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_CalendarAccesses_AspNetUsers_UserId1",
                table: "CalendarAccesses",
                column: "UserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Calendars_AspNetUsers_OwnerId1",
                table: "Calendars",
                column: "OwnerId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
