using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TorneosBasketBall.Models
{
    [Table("Administrador")] // Move the Table attribute to the class declaration  
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
