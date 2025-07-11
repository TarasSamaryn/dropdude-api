using System.Threading.Tasks;
using DropDudeAPI.Data;
using DropDudeAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DropDudeAPI.Controllers
{
    [ApiController]
    [Route("settings")]
    public class SettingsController : ControllerBase
    {
        private readonly AppDbContext _db;

        public SettingsController(AppDbContext db)
        {
            _db = db;
        }

        // GET /settings
        [HttpGet]
        public async Task<IActionResult> GetSettings()
        {
            var settings = await _db.GameSettings.FirstOrDefaultAsync();
            if (settings == null)
            {
                // Create defaults if not exist
                settings = new GameSettings
                {
                    GameplayTimer            = 300,
                    MaxPlayersForRandomRoom  = 6,
                    MaxPlayersForRankedRoom  = 6,
                    FindRoomSeconds          = 30,
                    SkinsAmount              = 41,
                    FreeSkins                = new[] { 0, 6, 7 },
                    MonthlySkins             = new[] { 1, 2, 8 },
                    BatKickForce             = 600,
                    LaserKickForce           = 400,
                    RevolverKickForce        = 500,
                    FistKickForce            = 300,
                    BatDamage                = 50,
                    LaserDamage              = 35,
                    RevolverDamage           = 25,
                    FistDamage               = 34,
                    BatKickFlyAmount         = 2,
                    LaserKickFlyAmount       = 2,
                    RevolverKickFlyAmount    = 2,
                    FistKickFlyAmount        = 3,
                    RespawnDelayX            = 20,
                    RespawnDelayY            = 40,
                    LaserBulletAmount        = 6,
                    RevolverBulletAmount     = 5,
                    BubbleBulletAmount       = 3,
                    LaserBulletSpeed         = 50,
                    RevolverBulletSpeed      = 35,
                    BubbleBulletSpeed        = 5,
                    FistsAttackSpeed         = 0.35,
                    BatAttackSpeed           = 0.5,
                    PlayerSpeed              = 7,
                    PlayerJumpPower          = 7,
                    PlayerGravity            = 10,
                    SkeletonTurnSpeed        = 500,
                    FocusSmoothSpeed         = 5,
                    CameraDistance           = 4,
                    CameraXSpeed             = 120,
                    CameraYSpeed             = 120,
                    MouseSpeed               = 0.02,
                    MouseSpeedMobile         = 0.002,
                    RotateSpeed              = 2
                };
                _db.GameSettings.Add(settings);
                await _db.SaveChangesAsync();
            }
            return Ok(settings);
        }

        // PUT /settings  (Admin only)
        [HttpPut]
        [Authorize(Policy = "RequireAdmin")]
        public async Task<IActionResult> UpdateSettings([FromBody] GameSettings input)
        {
            var settings = await _db.GameSettings.FirstOrDefaultAsync();
            if (settings == null)
                return NotFound(new { error = "Settings not found" });

            // Update all fields
            settings.GameplayTimer            = input.GameplayTimer;
            settings.MaxPlayersForRandomRoom  = input.MaxPlayersForRandomRoom;
            settings.MaxPlayersForRankedRoom  = input.MaxPlayersForRankedRoom;
            settings.FindRoomSeconds          = input.FindRoomSeconds;

            settings.SkinsAmount              = input.SkinsAmount;
            settings.FreeSkins                = input.FreeSkins;
            settings.MonthlySkins             = input.MonthlySkins;

            settings.BatKickForce             = input.BatKickForce;
            settings.LaserKickForce           = input.LaserKickForce;
            settings.RevolverKickForce        = input.RevolverKickForce;
            settings.FistKickForce            = input.FistKickForce;

            settings.BatDamage                = input.BatDamage;
            settings.LaserDamage              = input.LaserDamage;
            settings.RevolverDamage           = input.RevolverDamage;
            settings.FistDamage               = input.FistDamage;

            settings.BatKickFlyAmount         = input.BatKickFlyAmount;
            settings.LaserKickFlyAmount       = input.LaserKickFlyAmount;
            settings.RevolverKickFlyAmount    = input.RevolverKickFlyAmount;
            settings.FistKickFlyAmount        = input.FistKickFlyAmount;

            settings.RespawnDelayX            = input.RespawnDelayX;
            settings.RespawnDelayY            = input.RespawnDelayY;
            settings.LaserBulletAmount        = input.LaserBulletAmount;
            settings.RevolverBulletAmount     = input.RevolverBulletAmount;
            settings.BubbleBulletAmount       = input.BubbleBulletAmount;

            settings.LaserBulletSpeed         = input.LaserBulletSpeed;
            settings.RevolverBulletSpeed      = input.RevolverBulletSpeed;
            settings.BubbleBulletSpeed        = input.BubbleBulletSpeed;

            settings.FistsAttackSpeed         = input.FistsAttackSpeed;
            settings.BatAttackSpeed           = input.BatAttackSpeed;

            settings.PlayerSpeed              = input.PlayerSpeed;
            settings.PlayerJumpPower          = input.PlayerJumpPower;
            settings.PlayerGravity            = input.PlayerGravity;
            settings.SkeletonTurnSpeed        = input.SkeletonTurnSpeed;

            settings.FocusSmoothSpeed         = input.FocusSmoothSpeed;
            settings.CameraDistance           = input.CameraDistance;
            settings.CameraXSpeed             = input.CameraXSpeed;
            settings.CameraYSpeed             = input.CameraYSpeed;
            settings.MouseSpeed               = input.MouseSpeed;
            settings.MouseSpeedMobile         = input.MouseSpeedMobile;
            settings.RotateSpeed              = input.RotateSpeed;

            await _db.SaveChangesAsync();
            return Ok(settings);
        }
    }
}
