using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MinefieldServer.Migrations
{
    /// <inheritdoc />
    public partial class RemoveCoinsAddSkins : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RealCoins",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "TestCoins",
                table: "Players");

            migrationBuilder.AddColumn<string[]>(
                name: "AvailableSkins",
                table: "Players",
                type: "text[]",
                nullable: false,
                defaultValue: new string[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvailableSkins",
                table: "Players");

            migrationBuilder.AddColumn<int>(
                name: "RealCoins",
                table: "Players",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TestCoins",
                table: "Players",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
