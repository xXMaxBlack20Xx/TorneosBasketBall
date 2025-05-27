using System.ComponentModel.DataAnnotations;

namespace TorneosBasketBall.Models
{
    public class Administrador
    {
        [Key]
        public int AdministradorID { get; set; }

        [Required]
        [StringLength(50)]
        public required string Usuario { get; set; }

        [Required]
        [StringLength(255)]
        public required string Contrasenia { get; set; }
    }
}
