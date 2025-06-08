using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TorneosBasketBall.Models
{
    public class Jugador
    {
        [Key]
        public int JugadorID { get; set; }

        [Required]
        [StringLength(100)]
        public required string NombreCompleto { get; set; }

        public int Edad { get; set; }

        [Required]
        [StringLength(20)]
        public required string Posicion { get; set; }

        public int NumeroCamiseta { get; set; }

        public bool Estado { get; set; }

        // FK → Equipo
        [ForeignKey("Equipo")]
        public int EquipoID { get; set; }
        public required Equipo Equipo { get; set; }
    }
}