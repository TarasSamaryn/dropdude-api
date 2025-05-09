using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MinefieldServer.Migrations
{
    /// <inheritdoc />
    public partial class Migration1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvailableSkins",
                table: "Players");

            migrationBuilder.AddColumn<int[]>(
                name: "BodiesSkins",
                table: "Players",
                type: "integer[]",
                nullable: false,
                defaultValue: new int[0]);

            migrationBuilder.AddColumn<int[]>(
                name: "HeadsSkins",
                table: "Players",
                type: "integer[]",
                nullable: false,
                defaultValue: new int[0]);

            migrationBuilder.AddColumn<int[]>(
                name: "LegsSkins",
                table: "Players",
                type: "integer[]",
                nullable: false,
                defaultValue: new int[0]);

            migrationBuilder.AddColumn<int[]>(
                name: "MasksSkins",
                table: "Players",
                type: "integer[]",
                nullable: false,
                defaultValue: new int[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BodiesSkins",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "HeadsSkins",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "LegsSkins",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "MasksSkins",
                table: "Players");

            migrationBuilder.AddColumn<string[]>(
                name: "AvailableSkins",
                table: "Players",
                type: "text[]",
                nullable: false,
                defaultValue: new string[0]);
        }
    }
}
