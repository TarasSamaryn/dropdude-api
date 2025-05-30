using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MinefieldServer.Models
{
    public class GameResult
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey(nameof(Player))]
        public int PlayerId { get; set; }

        // Час перемоги
        public DateTimeOffset OccurredAt { get; set; } = DateTimeOffset.UtcNow;

        // Навігаційне поле
        public Player Player { get; set; } = null!;
    }
}