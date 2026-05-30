using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TreeTerritory.Repository.Migrations
{
    /// <inheritdoc />
    public partial class ComplexZipCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DefaultZipCode",
                table: "Territories",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DefaultZipCode",
                table: "Territories");
        }
    }
}
