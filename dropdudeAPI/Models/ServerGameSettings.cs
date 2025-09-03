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

        // Масив ID безкоштовних скінів (серверна правда)
        public int[] FreeSkins { get; set; } = Array.Empty<int>();
    }
}