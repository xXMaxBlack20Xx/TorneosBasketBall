// Models/Equipo.cs
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TorneosBasketBall.Models
{
    public class Equipo
    {
        public int EquipoID { get; set; }
        public required string NombreEquipo { get; set; }
        public int EntrenadorID { get; set; }
        public required Entrenador Entrenador { get; set; }
        public int TorneoID { get; set; }
        public required Torneo Torneo { get; set; }

        [InverseProperty(nameof(Partido.EquipoLocal))]
        public ICollection<Partido> PartidosLocal { get; set; } = new List<Partido>();

        [InverseProperty(nameof(Partido.EquipoVisitante))]
        public ICollection<Partido> PartidosVisita { get; set; } = new List<Partido>();

        public ICollection<Jugador> Jugadores { get; set; } = new List<Jugador>();
    }
}
