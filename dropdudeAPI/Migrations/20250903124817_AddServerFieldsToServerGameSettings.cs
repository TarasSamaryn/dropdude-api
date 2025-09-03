using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MinefieldServer.Migrations
{
    /// <inheritdoc />
    public partial class AddServerFieldsToServerGameSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FindRoomSeconds",
                table: "ServerGameSettings",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GameplayTimer",
                table: "ServerGameSettings",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MaxPlayersForRandomRoom",
                table: "ServerGameSettings",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MaxPlayersForRankedRoom",
                table: "ServerGameSettings",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int[]>(
                name: "MonthlySkins",
                table: "ServerGameSettings",
                type: "integer[]",
                nullable: false,
                defaultValue: new int[0]);

            migrationBuilder.AddColumn<int>(
                name: "SkinsAmount",
                table: "ServerGameSettings",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FindRoomSeconds",
                table: "ServerGameSettings");

            migrationBuilder.DropColumn(
                name: "GameplayTimer",
                table: "ServerGameSettings");

            migrationBuilder.DropColumn(
                name: "MaxPlayersForRandomRoom",
                table: "ServerGameSettings");

            migrationBuilder.DropColumn(
                name: "MaxPlayersForRankedRoom",
                table: "ServerGameSettings");

            migrationBuilder.DropColumn(
                name: "MonthlySkins",
                table: "ServerGameSettings");

            migrationBuilder.DropColumn(
                name: "SkinsAmount",
                table: "ServerGameSettings");
        }
    }
}
