using System.ComponentModel.DataAnnotations;

namespace DocinadeApp.ViewModels.Admin
{
    /// <summary>
    /// ViewModel para resetear/cambiar password de un usuario
    /// </summary>
    public class ResetPasswordViewModel
    {
        [Required]
        public string UserId { get; set; } = string.Empty;
        
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;
        
        [Display(Name = "Nombre Completo")]
        public string NombreCompleto { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "La nueva contraseña es requerida")]
        [StringLength(100, ErrorMessage = "La {0} debe tener al menos {2} y máximo {1} caracteres.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Nueva Contraseña")]
        public string NewPassword { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "La confirmación de contraseña es requerida")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirmar Nueva Contraseña")]
        [Compare("NewPassword", ErrorMessage = "La nueva contraseña y la confirmación no coinciden.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
