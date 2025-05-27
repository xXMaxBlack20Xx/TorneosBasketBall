// Models/Cancha.cs
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TorneosBasketBall.Models
{
    public class Cancha
    {
        [Key]
        public int CanchaID { get; set; }

        [Required]
        public required string Nombre { get; set; }
        public required string Direccion { get; set; }

        public ICollection<Partido> Partidos { get; set; } = new List<Partido>();
    }
}
