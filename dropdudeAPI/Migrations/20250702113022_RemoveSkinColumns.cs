using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MinefieldServer.Migrations
{
    /// <inheritdoc />
    public partial class RemoveSkinColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HeadsSkins",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "LegsSkins",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "MasksSkins",
                table: "Players");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HeadsSkins",
                table: "Players",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LegsSkins",
                table: "Players",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MasksSkins",
                table: "Players",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
