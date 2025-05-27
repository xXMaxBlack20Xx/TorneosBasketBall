// Models/Entrenador.cs
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TorneosBasketBall.Models
{
    public class Entrenador
    {
        [Key]
        public int EntrenadorID { get; set; }

        [Required]
        public required string NombreCompleto { get; set; }

        public int Experiencia { get; set; }

        public ICollection<Equipo> Equipos { get; set; } = new List<Equipo>();
    }
}
