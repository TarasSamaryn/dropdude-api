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

        // DTO лише для КЛІЄНТСЬКИХ полів
        public class ClientGameSettingsDto
        {
            // Kick forces
            public int BatKickForce { get; set; }
            public int LaserKickForce { get; set; }
            public int RevolverKickForce { get; set; }
            public int FistKickForce { get; set; }

            // Damage
            public int BatDamage { get; set; }
            public int LaserDamage { get; set; }
            public int RevolverDamage { get; set; }
            public int FistDamage { get; set; }

            // Fly amount
            public int BatKickFlyAmount { get; set; }
            public int LaserKickFlyAmount { get; set; }
            public int RevolverKickFlyAmount { get; set; }
            public int FistKickFlyAmount { get; set; }

            // Weapons
            public int RespawnDelayX { get; set; }
            public int RespawnDelayY { get; set; }
            public int LaserBulletAmount { get; set; }
            public int RevolverBulletAmount { get; set; }
            public int BubbleBulletAmount { get; set; }

            // Bullet speed
            public int LaserBulletSpeed { get; set; }
            public int RevolverBulletSpeed { get; set; }
            public int BubbleBulletSpeed { get; set; }

            // Attack speed
            public double FistsAttackSpeed { get; set; }
            public double BatAttackSpeed { get; set; }

            // Player
            public int PlayerSpeed { get; set; }
            public int PlayerJumpPower { get; set; }
            public int PlayerGravity { get; set; }
            public int SkeletonTurnSpeed { get; set; }

            // Player camera
            public double FocusSmoothSpeed { get; set; }
            public double CameraDistance { get; set; }
            public double CameraXSpeed { get; set; }
            public double CameraYSpeed { get; set; }
            public double MouseSpeed { get; set; }
            public double MouseSpeedMobile { get; set; }
            public double RotateSpeed { get; set; }
        }

        private static ClientGameSettingsDto MapToClientDto(ClientGameSettings s) => new ClientGameSettingsDto
        {
            BatKickForce          = s.BatKickForce,
            LaserKickForce        = s.LaserKickForce,
            RevolverKickForce     = s.RevolverKickForce,
            FistKickForce         = s.FistKickForce,

            BatDamage             = s.BatDamage,
            LaserDamage           = s.LaserDamage,
            RevolverDamage        = s.RevolverDamage,
            FistDamage            = s.FistDamage,

            BatKickFlyAmount      = s.BatKickFlyAmount,
            LaserKickFlyAmount    = s.LaserKickFlyAmount,
            RevolverKickFlyAmount = s.RevolverKickFlyAmount,
            FistKickFlyAmount     = s.FistKickFlyAmount,

            RespawnDelayX         = s.RespawnDelayX,
            RespawnDelayY         = s.RespawnDelayY,
            LaserBulletAmount     = s.LaserBulletAmount,
            RevolverBulletAmount  = s.RevolverBulletAmount,
            BubbleBulletAmount    = s.BubbleBulletAmount,

            LaserBulletSpeed      = s.LaserBulletSpeed,
            RevolverBulletSpeed   = s.RevolverBulletSpeed,
            BubbleBulletSpeed     = s.BubbleBulletSpeed,

            FistsAttackSpeed      = s.FistsAttackSpeed,
            BatAttackSpeed        = s.BatAttackSpeed,

            PlayerSpeed           = s.PlayerSpeed,
            PlayerJumpPower       = s.PlayerJumpPower,
            PlayerGravity         = s.PlayerGravity,
            SkeletonTurnSpeed     = s.SkeletonTurnSpeed,

            FocusSmoothSpeed      = s.FocusSmoothSpeed,
            CameraDistance        = s.CameraDistance,
            CameraXSpeed          = s.CameraXSpeed,
            CameraYSpeed          = s.CameraYSpeed,
            MouseSpeed            = s.MouseSpeed,
            MouseSpeedMobile      = s.MouseSpeedMobile,
            RotateSpeed           = s.RotateSpeed
        };

        private static void ApplyFromClientDto(ClientGameSettings s, ClientGameSettingsDto input)
        {
            s.BatKickForce          = input.BatKickForce;
            s.LaserKickForce        = input.LaserKickForce;
            s.RevolverKickForce     = input.RevolverKickForce;
            s.FistKickForce         = input.FistKickForce;

            s.BatDamage             = input.BatDamage;
            s.LaserDamage           = input.LaserDamage;
            s.RevolverDamage        = input.RevolverDamage;
            s.FistDamage            = input.FistDamage;

            s.BatKickFlyAmount      = input.BatKickFlyAmount;
            s.LaserKickFlyAmount    = input.LaserKickFlyAmount;
            s.RevolverKickFlyAmount = input.RevolverKickFlyAmount;
            s.FistKickFlyAmount     = input.FistKickFlyAmount;

            s.RespawnDelayX         = input.RespawnDelayX;
            s.RespawnDelayY         = input.RespawnDelayY;
            s.LaserBulletAmount     = input.LaserBulletAmount;
            s.RevolverBulletAmount  = input.RevolverBulletAmount;
            s.BubbleBulletAmount    = input.BubbleBulletAmount;

            s.LaserBulletSpeed      = input.LaserBulletSpeed;
            s.RevolverBulletSpeed   = input.RevolverBulletSpeed;
            s.BubbleBulletSpeed     = input.BubbleBulletSpeed;

            s.FistsAttackSpeed      = input.FistsAttackSpeed;
            s.BatAttackSpeed        = input.BatAttackSpeed;

            s.PlayerSpeed           = input.PlayerSpeed;
            s.PlayerJumpPower       = input.PlayerJumpPower;
            s.PlayerGravity         = input.PlayerGravity;
            s.SkeletonTurnSpeed     = input.SkeletonTurnSpeed;

            s.FocusSmoothSpeed      = input.FocusSmoothSpeed;
            s.CameraDistance        = input.CameraDistance;
            s.CameraXSpeed          = input.CameraXSpeed;
            s.CameraYSpeed          = input.CameraYSpeed;
            s.MouseSpeed            = input.MouseSpeed;
            s.MouseSpeedMobile      = input.MouseSpeedMobile;
            s.RotateSpeed           = input.RotateSpeed;
        }

        // GET /settings  → працює з новою таблицею ClientGameSettings
        [HttpGet]
        public async Task<IActionResult> GetSettings()
        {
            ClientGameSettings? s = await _db.ClientGameSettings.FirstOrDefaultAsync();
            if (s == null)
            {
                s = new ClientGameSettings
                {
                    // Kick forces
                    BatKickForce = 600,
                    LaserKickForce = 400,
                    RevolverKickForce = 500,
                    FistKickForce = 300,

                    // Damage
                    BatDamage = 50,
                    LaserDamage = 35,
                    RevolverDamage = 25,
                    FistDamage = 34,

                    // Fly amount
                    BatKickFlyAmount = 2,
                    LaserKickFlyAmount = 2,
                    RevolverKickFlyAmount = 2,
                    FistKickFlyAmount = 3,

                    // Weapons
                    RespawnDelayX = 20,
                    RespawnDelayY = 40,
                    LaserBulletAmount = 6,
                    RevolverBulletAmount = 5,
                    BubbleBulletAmount = 3,

                    // Bullet speed
                    LaserBulletSpeed = 50,
                    RevolverBulletSpeed = 35,
                    BubbleBulletSpeed = 5,

                    // Attack speed
                    FistsAttackSpeed = 0.35,
                    BatAttackSpeed = 0.5,

                    // Player
                    PlayerSpeed = 7,
                    PlayerJumpPower = 7,
                    PlayerGravity = 10,
                    SkeletonTurnSpeed = 500,

                    // Player camera
                    FocusSmoothSpeed = 5,
                    CameraDistance = 4,
                    CameraXSpeed = 120,
                    CameraYSpeed = 120,
                    MouseSpeed = 0.02,
                    MouseSpeedMobile = 0.002,
                    RotateSpeed = 2
                };
                _db.ClientGameSettings.Add(s);
                await _db.SaveChangesAsync();
            }

            return Ok(MapToClientDto(s));
        }

        // PUT /settings (Admin only) → працює з ClientGameSettings
        [HttpPut]
        [Authorize(Policy = "RequireAdmin")]
        public async Task<IActionResult> UpdateSettings([FromBody] ClientGameSettingsDto input)
        {
            var s = await _db.ClientGameSettings.FirstOrDefaultAsync();
            if (s == null)
                return NotFound(new { error = "Client settings not found" });

            ApplyFromClientDto(s, input);
            await _db.SaveChangesAsync();

            return Ok(MapToClientDto(s));
        }
    }
}
