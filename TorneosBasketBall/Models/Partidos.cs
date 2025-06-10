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
        [ForeignKey(nameof(EquipoLocalID))]
        public Equipo? EquipoLocal { get; set; }     // nullable

        [Required]
        public int EquipoVisitanteID { get; set; }
        [ForeignKey(nameof(EquipoVisitanteID))]
        public Equipo? EquipoVisitante { get; set; } // nullable

        [Required]
        public DateTime FechaHora { get; set; }

        [Required]
        [StringLength(15)]
        public string Estado { get; set; } = "Programado";

        public int? PuntuacionLocal { get; set; }
        public int? PuntuacionVisitante { get; set; }
    }

}
