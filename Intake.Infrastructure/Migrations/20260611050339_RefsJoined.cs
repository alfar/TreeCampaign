using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Intake.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RefsJoined : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WashedOrder_NeighborhoodId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "WashedOrder_StreetSectionId",
                table: "Orders");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "WashedOrder_NeighborhoodId",
                table: "Orders",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "WashedOrder_StreetSectionId",
                table: "Orders",
                type: "TEXT",
                nullable: true);
        }
    }
}
