using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DropDudeAPI.Models
{
    public class ServerGameSettings
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        // --- SERVER SETTINGS ---
        // Gameplay
        public int GameplayTimer { get; set; }
        public int MaxPlayersForRandomRoom { get; set; }
        public int MaxPlayersForRankedRoom { get; set; }

        // Server
        public int FindRoomSeconds { get; set; }

        // Skins
        public int SkinsAmount { get; set; }
        public int[] FreeSkins { get; set; } = Array.Empty<int>();
        public int[] MonthlySkins { get; set; } = Array.Empty<int>();
    }
}