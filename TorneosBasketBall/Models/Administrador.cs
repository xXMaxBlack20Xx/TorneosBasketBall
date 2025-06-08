using System.ComponentModel.DataAnnotations;

namespace TorneosBasketBall.Models
{
    public class Administrador
    {
        [Key]
        public int AdministradorID { get; set; }

        [Required]
        [StringLength(50)]
        public string Usuario { get; set; }

        [Required]
        [StringLength(255)]
        public string Contrasenia { get; set; }
    }
}
