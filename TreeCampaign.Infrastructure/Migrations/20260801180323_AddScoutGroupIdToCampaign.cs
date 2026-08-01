using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TreeCampaign.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddScoutGroupIdToCampaign : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ScoutGroupId",
                table: "Campaigns",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ScoutGroupId",
                table: "Campaigns");
        }
    }
}
