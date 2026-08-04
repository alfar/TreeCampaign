using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TreeTerritory.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class EvenOddHouseNumberRanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StartHouseNumber",
                table: "StreetSections",
                newName: "OddStartHouseNumber");

            migrationBuilder.RenameColumn(
                name: "EndHouseNumber",
                table: "StreetSections",
                newName: "OddEndHouseNumber");

            migrationBuilder.AddColumn<string>(
                name: "EvenEndHouseNumber",
                table: "StreetSections",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EvenStartHouseNumber",
                table: "StreetSections",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EvenEndHouseNumber",
                table: "StreetSections");

            migrationBuilder.DropColumn(
                name: "EvenStartHouseNumber",
                table: "StreetSections");

            migrationBuilder.RenameColumn(
                name: "OddStartHouseNumber",
                table: "StreetSections",
                newName: "StartHouseNumber");

            migrationBuilder.RenameColumn(
                name: "OddEndHouseNumber",
                table: "StreetSections",
                newName: "EndHouseNumber");
        }
    }
}
