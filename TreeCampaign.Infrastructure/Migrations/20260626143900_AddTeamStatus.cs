using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TreeCampaign.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTeamStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte>(
                name: "Status",
                table: "Teams",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Teams");
        }
    }
}
