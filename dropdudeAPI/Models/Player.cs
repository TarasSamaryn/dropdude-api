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
        
        public int[] HeadsSkins { get; set; } = Array.Empty<int>();
        public int[] BodiesSkins { get; set; } = Array.Empty<int>();
        public int[] LegsSkins { get; set; } = Array.Empty<int>();
        public int[] MasksSkins { get; set; } = Array.Empty<int>();
    }
}