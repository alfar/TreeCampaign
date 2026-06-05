using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Intake.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class WashedOrderRefac : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WashedHouseNumber",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "WashedZipCode",
                table: "Orders",
                newName: "WashedOrder_StreetSectionId");

            migrationBuilder.RenameColumn(
                name: "WashedStreet",
                table: "Orders",
                newName: "WashedOrder_NeighborhoodId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "WashedOrder_StreetSectionId",
                table: "Orders",
                newName: "WashedZipCode");

            migrationBuilder.RenameColumn(
                name: "WashedOrder_NeighborhoodId",
                table: "Orders",
                newName: "WashedStreet");

            migrationBuilder.AddColumn<string>(
                name: "WashedHouseNumber",
                table: "Orders",
                type: "TEXT",
                nullable: true);
        }
    }
}
