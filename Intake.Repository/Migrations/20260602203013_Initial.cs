using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Intake.Repository.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    CampaignId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Amount = table.Column<decimal>(type: "TEXT", nullable: false),
                    OrderDate = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    Message = table.Column<string>(type: "TEXT", nullable: false),
                    OrderType = table.Column<string>(type: "TEXT", maxLength: 13, nullable: false),
                    SenderName = table.Column<string>(type: "TEXT", nullable: false),
                    SenderPhoneNumber = table.Column<string>(type: "TEXT", nullable: false),
                    StreetId = table.Column<Guid>(type: "TEXT", nullable: true),
                    StreetSectionId = table.Column<Guid>(type: "TEXT", nullable: true),
                    NeighborhoodId = table.Column<Guid>(type: "TEXT", nullable: true),
                    WashedHouseNumber = table.Column<string>(type: "TEXT", nullable: true),
                    WashedStreet = table.Column<string>(type: "TEXT", nullable: true),
                    WashedZipCode = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Orders");
        }
    }
}
