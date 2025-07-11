using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MinefieldServer.Migrations
{
    /// <inheritdoc />
    public partial class AddGameSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GameSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GameplayTimer = table.Column<int>(type: "integer", nullable: false),
                    MaxPlayersForRandomRoom = table.Column<int>(type: "integer", nullable: false),
                    MaxPlayersForRankedRoom = table.Column<int>(type: "integer", nullable: false),
                    FindRoomSeconds = table.Column<int>(type: "integer", nullable: false),
                    SkinsAmount = table.Column<int>(type: "integer", nullable: false),
                    FreeSkins = table.Column<int[]>(type: "integer[]", nullable: false),
                    MonthlySkins = table.Column<int[]>(type: "integer[]", nullable: false),
                    BatKickForce = table.Column<int>(type: "integer", nullable: false),
                    LaserKickForce = table.Column<int>(type: "integer", nullable: false),
                    RevolverKickForce = table.Column<int>(type: "integer", nullable: false),
                    FistKickForce = table.Column<int>(type: "integer", nullable: false),
                    BatDamage = table.Column<int>(type: "integer", nullable: false),
                    LaserDamage = table.Column<int>(type: "integer", nullable: false),
                    RevolverDamage = table.Column<int>(type: "integer", nullable: false),
                    FistDamage = table.Column<int>(type: "integer", nullable: false),
                    BatKickFlyAmount = table.Column<int>(type: "integer", nullable: false),
                    LaserKickFlyAmount = table.Column<int>(type: "integer", nullable: false),
                    RevolverKickFlyAmount = table.Column<int>(type: "integer", nullable: false),
                    FistKickFlyAmount = table.Column<int>(type: "integer", nullable: false),
                    RespawnDelayX = table.Column<int>(type: "integer", nullable: false),
                    RespawnDelayY = table.Column<int>(type: "integer", nullable: false),
                    LaserBulletAmount = table.Column<int>(type: "integer", nullable: false),
                    RevolverBulletAmount = table.Column<int>(type: "integer", nullable: false),
                    BubbleBulletAmount = table.Column<int>(type: "integer", nullable: false),
                    LaserBulletSpeed = table.Column<int>(type: "integer", nullable: false),
                    RevolverBulletSpeed = table.Column<int>(type: "integer", nullable: false),
                    BubbleBulletSpeed = table.Column<int>(type: "integer", nullable: false),
                    FistsAttackSpeed = table.Column<double>(type: "double precision", nullable: false),
                    BatAttackSpeed = table.Column<double>(type: "double precision", nullable: false),
                    PlayerSpeed = table.Column<int>(type: "integer", nullable: false),
                    PlayerJumpPower = table.Column<int>(type: "integer", nullable: false),
                    PlayerGravity = table.Column<int>(type: "integer", nullable: false),
                    SkeletonTurnSpeed = table.Column<int>(type: "integer", nullable: false),
                    FocusSmoothSpeed = table.Column<double>(type: "double precision", nullable: false),
                    CameraDistance = table.Column<double>(type: "double precision", nullable: false),
                    CameraXSpeed = table.Column<double>(type: "double precision", nullable: false),
                    CameraYSpeed = table.Column<double>(type: "double precision", nullable: false),
                    MouseSpeed = table.Column<double>(type: "double precision", nullable: false),
                    MouseSpeedMobile = table.Column<double>(type: "double precision", nullable: false),
                    RotateSpeed = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameSettings", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GameSettings");
        }
    }
}
