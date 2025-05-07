using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MinefieldServer.Models
{
    public class GameSession
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int PlayerId { get; set; }

        [ForeignKey(nameof(PlayerId))]
        public Player Player { get; set; } = null!;

        [Required]
        public int Bet { get; set; }

        [Required]
        public bool UseTestCoins { get; set; }

        [Required]
        public string StepResultsJson { get; set; } = null!;

        [Required]
        public bool IsActive { get; set; } = true;

        // Заміна DateTime на DateTimeOffset
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    }
}