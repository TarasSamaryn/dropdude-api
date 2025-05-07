using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MinefieldServer.Models
{
    public class Player
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// Чи є адміністративним користувачем
        /// </summary>
        public bool IsAdmin { get; set; } = false;

        [Required]
        public string Username { get; set; } = null!;

        [Required]
        public string PasswordHash { get; set; } = null!;

        // Заміна DateTime на DateTimeOffset
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

        public int TestCoins { get; set; } = 5000;
        public int RealCoins { get; set; } = 5000;
    }
}