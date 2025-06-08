using Microsoft.EntityFrameworkCore;
using TorneosBasketBall.Models;

namespace TorneosBasketBall.Servicios
{
    public class ContextoBD : DbContext
    {
        // Constructor with options parameter  
        public ContextoBD(DbContextOptions<ContextoBD> options) : base(options)
        {
        }

        // Removed duplicate constructor to fix CS0111 error  

        public DbSet<Equipo> Equipos { get; set; }
        public DbSet<Jugador> Jugadores { get; set; }
        public DbSet<Partidos> Partidos { get; set; }
        public virtual DbSet<EstadisticasEquipos> EstadisticasEquipos { get; set; }

        //public DbSet<EstadisticaEquipo> EstadisticasEquipos { get; set; }
        public DbSet<Administrador> Administradores { get; set; }
    }
}