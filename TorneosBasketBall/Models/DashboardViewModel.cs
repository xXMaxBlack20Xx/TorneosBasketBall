using System.Collections.Generic;

namespace TorneosBasketBall.Models
{
    public class DashboardViewModel
    {
        public List<Equipo> AllEquipos { get; set; } = new();

        public List<Equipo> Equipos { get; set; } = new();

        public List<Jugador> Jugadores { get; set; } = new();

        public List<Partidos> Partidos { get; set; } = new();

        public List<EstadisticasEquipos> Estadisticas { get; set; } = new();
    }
}
