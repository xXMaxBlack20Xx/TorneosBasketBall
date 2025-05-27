// Models/Jugador.cs
using System.ComponentModel.DataAnnotations;

namespace TorneosBasketBall.Models
{
    public class Jugador
    {
        [Key]
        public int JugadorID { get; set; }

        [Required]
        public required string NombreCompleto { get; set; }

        public int Edad { get; set; }
        public required string Posicion { get; set; }
        public int NumeroCamiseta { get; set; }
        public bool Estado { get; set; }

        // FK → Equipo
        public int EquipoID { get; set; }
        public required Equipo Equipo { get; set; }
    }
}
