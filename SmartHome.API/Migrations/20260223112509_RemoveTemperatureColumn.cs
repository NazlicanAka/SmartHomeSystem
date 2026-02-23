using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartHome.API.Migrations
{
    /// <inheritdoc />
    public partial class RemoveTemperatureColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Temperature",
                table: "Devices");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Temperature",
                table: "Devices",
                type: "REAL",
                nullable: true);
        }
    }
}
