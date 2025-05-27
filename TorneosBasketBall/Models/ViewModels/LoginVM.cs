using System.ComponentModel.DataAnnotations;

namespace TorneosBasketBall.Models.ViewModels
{
    public class LoginVM
    {
        [Required]
        [Display(Name = "Usuario")]
        public required string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public required string Password { get; set; }

        [Display(Name = "Recordarme")]
        public bool RememberMe { get; set; }

        // Para redirigir al origen tras login
        public string? ReturnUrl { get; set; }
    }
}