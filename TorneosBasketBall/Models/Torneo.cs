// Models/Torneo.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TorneosBasketBall.Models
{
    public class Torneo
    {
        [Key]
        public int TorneoID { get; set; }

        [Required]
        public required string Nombre { get; set; }

        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }

        public ICollection<Equipo> Equipos { get; set; } = new List<Equipo>();
        public ICollection<Partido> Partidos { get; set; } = new List<Partido>();
    }
}
