using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TorneosBasketBall.Models
{
    public class EstadisticasEquipos
    {
        [Key]
        [ForeignKey("Equipo")]
        public int EquipoID { get; set; }

        public int PartidosJugados { get; set; }
        public int Ganados { get; set; }
        public int Perdidos { get; set; }
        public int PuntosFavor { get; set; }
        public int PuntosContra { get; set; }

        // Relación de navegación (opcional)
        public required Equipo Equipo { get; set; }
    }
}
