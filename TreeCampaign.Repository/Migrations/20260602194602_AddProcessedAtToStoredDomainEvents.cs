using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TreeCampaign.Repository.Migrations
{
    /// <inheritdoc />
    public partial class AddProcessedAtToStoredDomainEvents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ProcessedAtUtc",
                table: "StoredDomainEvents",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProcessedAtUtc",
                table: "StoredDomainEvents");
        }
    }
}
