using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FamilyCalender.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UserNotificationSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserNotificationSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedUtc = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LastUpdatedUtc = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Endpoint = table.Column<string>(type: "TEXT", nullable: false),
                    P256dh = table.Column<string>(type: "TEXT", nullable: false),
                    Auth = table.Column<string>(type: "TEXT", nullable: false),
                    AllowNotifications = table.Column<bool>(type: "INTEGER", nullable: false),
                    AllowOnNewCalendarEvents = table.Column<bool>(type: "INTEGER", nullable: false),
                    AllowOnEditCalendarEvents = table.Column<bool>(type: "INTEGER", nullable: false),
                    AllowOnDeleteCalendarEvents = table.Column<bool>(type: "INTEGER", nullable: false),
                    AllowOnCalendarInviteAcceptEvents = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserNotificationSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserNotificationSettings_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserNotificationSettings_UserId",
                table: "UserNotificationSettings",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserNotificationSettings");
        }
    }
}
