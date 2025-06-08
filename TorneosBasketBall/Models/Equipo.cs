// Models/Equipo.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TorneosBasketBall.Models
{
    public class Equipo
    {
        [Key]
        public int EquipoID { get; set; }

        [Required]
        [StringLength(100)]
        public required string NombreEquipo { get; set; }

        // Estas columnas son opcionales según tu diseño (Allow Nulls marcado)
        public int? EntrenadorID { get; set; }

        [StringLength(100)]
        public string? NombreEntrenador { get; set; }

        public int? PartidoID { get; set; }
    }
}