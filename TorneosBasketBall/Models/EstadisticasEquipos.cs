using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TorneosBasketBall.Models
{
    public class EstadisticasEquipos
    {
        [Key]
        [ForeignKey("Equipo")]
        public int EquipoID { get; set; } // This is the PK and FK, NOT auto-incremented by DB

        public int PartidosJugados { get; set; }
        public int Ganados { get; set; }
        public int Perdidos { get; set; }
        public int PuntosFavor { get; set; }
        public int PuntosContra { get; set; }

        public required Equipo Equipo { get; set; }
    }
}