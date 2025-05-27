// Models/Partido.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TorneosBasketBall.Models
{
    public class Partido
    {
        [Key]
        public int PartidoID { get; set; }

        public int TorneoID { get; set; }
        public required Torneo Torneo { get; set; }

        public int EquipoLocalID { get; set; }
        [ForeignKey(nameof(EquipoLocalID))]
        [InverseProperty(nameof(Equipo.PartidosLocal))]
        public required Equipo EquipoLocal { get; set; }

        public int EquipoVisitanteID { get; set; }
        [ForeignKey(nameof(EquipoVisitanteID))]
        [InverseProperty(nameof(Equipo.PartidosVisita))]
        public required Equipo EquipoVisitante { get; set; }

        public int CanchaID { get; set; }
        public required Cancha Cancha { get; set; }

        public DateTime FechaHora { get; set; }
        public required string Estado { get; set; }

        public int? PuntuacionLocal { get; set; }
        public int? PuntuacionVisitante { get; set; }
    }
}