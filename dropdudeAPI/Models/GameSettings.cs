using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DropDudeAPI.Models
{
    public class GameSettings
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        // Gameplay
        public int GameplayTimer { get; set; }
        public int MaxPlayersForRandomRoom { get; set; }
        public int MaxPlayersForRankedRoom { get; set; }

        // Server
        public int FindRoomSeconds { get; set; }

        // Skins
        public int SkinsAmount { get; set; }
        public int[] FreeSkins { get; set; } = new int[0];
        public int[] MonthlySkins { get; set; } = new int[0];

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
}
