using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FamilyCalender.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDbSet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MemberCalendar_Calendars_CalendarId",
                table: "MemberCalendar");

            migrationBuilder.DropForeignKey(
                name: "FK_MemberCalendar_Events_EventId",
                table: "MemberCalendar");

            migrationBuilder.DropForeignKey(
                name: "FK_MemberCalendar_Members_MemberId",
                table: "MemberCalendar");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MemberCalendar",
                table: "MemberCalendar");

            migrationBuilder.RenameTable(
                name: "MemberCalendar",
                newName: "MemberCalendars");

            migrationBuilder.RenameIndex(
                name: "IX_MemberCalendar_MemberId",
                table: "MemberCalendars",
                newName: "IX_MemberCalendars_MemberId");

            migrationBuilder.RenameIndex(
                name: "IX_MemberCalendar_EventId",
                table: "MemberCalendars",
                newName: "IX_MemberCalendars_EventId");

            migrationBuilder.RenameIndex(
                name: "IX_MemberCalendar_CalendarId",
                table: "MemberCalendars",
                newName: "IX_MemberCalendars_CalendarId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MemberCalendars",
                table: "MemberCalendars",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "MemberEvents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MemberId = table.Column<int>(type: "INTEGER", nullable: false),
                    EventId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemberEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MemberEvents_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MemberEvents_Members_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MemberEvents_EventId",
                table: "MemberEvents",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_MemberEvents_MemberId",
                table: "MemberEvents",
                column: "MemberId");

            migrationBuilder.AddForeignKey(
                name: "FK_MemberCalendars_Calendars_CalendarId",
                table: "MemberCalendars",
                column: "CalendarId",
                principalTable: "Calendars",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MemberCalendars_Events_EventId",
                table: "MemberCalendars",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MemberCalendars_Members_MemberId",
                table: "MemberCalendars",
                column: "MemberId",
                principalTable: "Members",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MemberCalendars_Calendars_CalendarId",
                table: "MemberCalendars");

            migrationBuilder.DropForeignKey(
                name: "FK_MemberCalendars_Events_EventId",
                table: "MemberCalendars");

            migrationBuilder.DropForeignKey(
                name: "FK_MemberCalendars_Members_MemberId",
                table: "MemberCalendars");

            migrationBuilder.DropTable(
                name: "MemberEvents");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MemberCalendars",
                table: "MemberCalendars");

            migrationBuilder.RenameTable(
                name: "MemberCalendars",
                newName: "MemberCalendar");

            migrationBuilder.RenameIndex(
                name: "IX_MemberCalendars_MemberId",
                table: "MemberCalendar",
                newName: "IX_MemberCalendar_MemberId");

            migrationBuilder.RenameIndex(
                name: "IX_MemberCalendars_EventId",
                table: "MemberCalendar",
                newName: "IX_MemberCalendar_EventId");

            migrationBuilder.RenameIndex(
                name: "IX_MemberCalendars_CalendarId",
                table: "MemberCalendar",
                newName: "IX_MemberCalendar_CalendarId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MemberCalendar",
                table: "MemberCalendar",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MemberCalendar_Calendars_CalendarId",
                table: "MemberCalendar",
                column: "CalendarId",
                principalTable: "Calendars",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MemberCalendar_Events_EventId",
                table: "MemberCalendar",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MemberCalendar_Members_MemberId",
                table: "MemberCalendar",
                column: "MemberId",
                principalTable: "Members",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
