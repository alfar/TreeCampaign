using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TreeCampaign.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCampaignDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "CollectionDate",
                table: "Campaigns",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LastOrderDateTime",
                table: "Campaigns",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PricePerTree",
                table: "Campaigns",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Campaigns",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CollectionDate",
                table: "Campaigns");

            migrationBuilder.DropColumn(
                name: "LastOrderDateTime",
                table: "Campaigns");

            migrationBuilder.DropColumn(
                name: "PricePerTree",
                table: "Campaigns");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Campaigns");
        }
    }
}
