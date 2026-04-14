using DocinadeApp.Models;
using System.ComponentModel.DataAnnotations;

namespace DocinadeApp.ViewModels.Configuracion
{
    /// <summary>
    /// ViewModel para la página principal de configuración
    /// </summary>
    public class ConfiguracionIndexViewModel
    {
        public List<ConfiguracionSistema> Configuraciones { get; set; } = new();
        public ModoRegistroUsuarios ModoRegistroActual { get; set; }
        public string MensajeRegistroCerrado { get; set; } = string.Empty;
    }

    /// <summary>
    /// ViewModel para actualizar el modo de registro
    /// </summary>
    public class ConfiguracionModoRegistroViewModel
    {
        [Required(ErrorMessage = "El modo de registro es requerido")]
        [Display(Name = "Modo de Registro")]
        public ModoRegistroUsuarios ModoRegistro { get; set; }

        [Display(Name = "Mensaje Personalizado")]
        [StringLength(500, ErrorMessage = "El mensaje no puede exceder 500 caracteres")]
        public string? MensajePersonalizado { get; set; }
    }

    /// <summary>
    /// ViewModel para editar una configuración específica
    /// </summary>
    public class EditarConfiguracionViewModel
    {
        [Required]
        public string Clave { get; set; } = string.Empty;

        [Required(ErrorMessage = "El valor es requerido")]
        [Display(Name = "Valor")]
        public string Valor { get; set; } = string.Empty;

        [Display(Name = "Descripción")]
        [StringLength(500)]
        public string? Descripcion { get; set; }
    }
}