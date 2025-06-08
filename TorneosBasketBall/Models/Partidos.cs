using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TorneosBasketBall.Models
{
    public class Partidos
    {
        [Key]
        public int PartidoID { get; set; }

        [Required]
        public int EquipoLocalID { get; set; }
        // Puedes agregar navegación si quieres: 
        // [ForeignKey(nameof(EquipoLocalID))]
        // public Equipo EquipoLocal { get; set; }

        [Required]
        public int EquipoVisitanteID { get; set; }
        // [ForeignKey(nameof(EquipoVisitanteID))]
        // public Equipo EquipoVisitante { get; set; }

        [Required]
        public DateTime FechaHora { get; set; }

        [Required]
        [StringLength(15)]
        public required string Estado { get; set; }

        public int? PuntuacionLocal { get; set; }
        public int? PuntuacionVisitante { get; set; }
    }
}
