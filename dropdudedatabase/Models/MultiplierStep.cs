using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MinefieldServer.Models
{
    public class MultiplierStep
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column(TypeName = "integer")]
        public int Id { get; set; }

        [Required]
        [Column(TypeName = "integer")]
        public int Step { get; set; }

        [Required]
        [Column(TypeName = "real")]
        public float Multiplier { get; set; }
    }
}