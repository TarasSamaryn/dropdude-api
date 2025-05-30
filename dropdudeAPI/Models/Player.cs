using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MinefieldServer.Models
{
    public class Player
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public bool IsAdmin { get; set; } = false;

        [Required]
        public string Username { get; set; } = null!;

        [Required]
        public string PasswordHash { get; set; } = null!;

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public string LastSelectedSkin { get; set; } = null!;

        // Заміна масивів на рядки
        public string HeadsSkins   { get; set; } = string.Empty;
        public string BodiesSkins  { get; set; } = string.Empty;
        public string LegsSkins    { get; set; } = string.Empty;
        public string MasksSkins   { get; set; } = string.Empty;

        public int MonthlyWins { get; set; } = 0;
    }
}