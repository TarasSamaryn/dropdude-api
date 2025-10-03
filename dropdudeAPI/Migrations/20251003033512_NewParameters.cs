using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MinefieldServer.Migrations
{
    /// <inheritdoc />
    public partial class NewParameters : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MediumRatingMax",
                table: "ServerGameSettings",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MediumRatingMin",
                table: "ServerGameSettings",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RespawnTimeSeconds",
                table: "ClientGameSettings",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MediumRatingMax",
                table: "ServerGameSettings");

            migrationBuilder.DropColumn(
                name: "MediumRatingMin",
                table: "ServerGameSettings");

            migrationBuilder.DropColumn(
                name: "RespawnTimeSeconds",
                table: "ClientGameSettings");
        }
    }
}
