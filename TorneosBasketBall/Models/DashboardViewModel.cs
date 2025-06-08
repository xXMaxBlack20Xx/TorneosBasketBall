namespace TorneosBasketBall.Models
{
    public class DashboardViewModel
    {
        public required List<Equipo> Equipos { get; set; }
        public required List<Jugador> Jugadores { get; set; }
        public required List<Partidos> Partidos { get; set; }
        public required List<EstadisticasEquipos> Estadisticas { get; set; }
    }

}