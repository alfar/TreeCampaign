using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TreeCampaign.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SplitTeamKind : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsTrailerFull",
                table: "Teams",
                type: "INTEGER",
                nullable: true,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "TeamKind",
                table: "Teams",
                type: "TEXT",
                maxLength: 8,
                nullable: false,
                defaultValue: "Trailer");

            // Backfill: existing teams could report a full trailer, so they all become TrailerTeam.
            // Old Status=2 (TrailerFull) becomes IsTrailerFull=1 with Status reset to 0 (Active).
            migrationBuilder.Sql("UPDATE Teams SET TeamKind = 'Trailer';");
            migrationBuilder.Sql("UPDATE Teams SET IsTrailerFull = 1 WHERE Status = 2;");
            migrationBuilder.Sql("UPDATE Teams SET Status = 0 WHERE Status = 2;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsTrailerFull",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "TeamKind",
                table: "Teams");
        }
    }
}
