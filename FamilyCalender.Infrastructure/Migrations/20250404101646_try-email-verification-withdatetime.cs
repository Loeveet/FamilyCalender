using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FamilyCalender.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class tryemailverificationwithdatetime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "VerificationDateUtc",
                table: "Users",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VerificationDateUtc",
                table: "Users");
        }
    }
}
