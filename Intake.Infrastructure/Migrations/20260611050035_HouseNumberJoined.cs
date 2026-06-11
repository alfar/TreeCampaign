using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Intake.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class HouseNumberJoined : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WashedOrder_HouseNumber",
                table: "Orders");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "WashedOrder_HouseNumber",
                table: "Orders",
                type: "TEXT",
                nullable: true);
        }
    }
}
