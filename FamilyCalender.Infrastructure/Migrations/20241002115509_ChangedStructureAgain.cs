using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FamilyCalender.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangedStructureAgain : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CalendarAccesses_Calendars_CalendarId",
                table: "CalendarAccesses");

            migrationBuilder.DropForeignKey(
                name: "FK_Events_Members_MemberId",
                table: "Events");

            migrationBuilder.DropForeignKey(
                name: "FK_Members_Calendars_CalendarId",
                table: "Members");

            migrationBuilder.AlterColumn<int>(
                name: "CalendarId",
                table: "Members",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "MemberId",
                table: "Events",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "CalendarAccesses",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CalendarId",
                table: "CalendarAccesses",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CalendarAccesses_Calendars_CalendarId",
                table: "CalendarAccesses",
                column: "CalendarId",
                principalTable: "Calendars",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Members_MemberId",
                table: "Events",
                column: "MemberId",
                principalTable: "Members",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Members_Calendars_CalendarId",
                table: "Members",
                column: "CalendarId",
                principalTable: "Calendars",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CalendarAccesses_Calendars_CalendarId",
                table: "CalendarAccesses");

            migrationBuilder.DropForeignKey(
                name: "FK_Events_Members_MemberId",
                table: "Events");

            migrationBuilder.DropForeignKey(
                name: "FK_Members_Calendars_CalendarId",
                table: "Members");

            migrationBuilder.AlterColumn<int>(
                name: "CalendarId",
                table: "Members",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<int>(
                name: "MemberId",
                table: "Events",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "CalendarAccesses",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<int>(
                name: "CalendarId",
                table: "CalendarAccesses",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddForeignKey(
                name: "FK_CalendarAccesses_Calendars_CalendarId",
                table: "CalendarAccesses",
                column: "CalendarId",
                principalTable: "Calendars",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Members_MemberId",
                table: "Events",
                column: "MemberId",
                principalTable: "Members",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Members_Calendars_CalendarId",
                table: "Members",
                column: "CalendarId",
                principalTable: "Calendars",
                principalColumn: "Id");
        }
    }
}
