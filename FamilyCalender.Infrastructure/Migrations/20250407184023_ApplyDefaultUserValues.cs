using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FamilyCalender.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ApplyDefaultUserValues : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //Sqlite = DATETIME('now'), SQL = GetUtcDate()
            migrationBuilder.Sql($"Update Users Set IsVerified = 1, VerificationDateUtc = DATETIME('now')");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
