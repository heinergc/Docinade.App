using System.ComponentModel.DataAnnotations;

namespace RubricasApp.Web.Models
{
    /// <summary>
    /// Representa un ítem del slider dinámico del sistema
    /// </summary>
    public class SliderItem
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El título es obligatorio")]
        [StringLength(200, ErrorMessage = "El título no puede exceder 200 caracteres")]
        public string Titulo { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "El subtítulo no puede exceder 500 caracteres")]
        public string? Subtitulo { get; set; }

        [StringLength(500, ErrorMessage = "El enlace no puede exceder 500 caracteres")]
        [Url(ErrorMessage = "Debe ser una URL válida")]
        public string? EnlaceUrl { get; set; }

    [StringLength(100, ErrorMessage = "El texto del botón no puede exceder 100 caracteres")]
    public string? TextoBoton { get; set; }

    [StringLength(500)]
    public string ImagenUrl { get; set; } = string.Empty;        [Required]
        [Range(1, 999, ErrorMessage = "El orden debe estar entre 1 y 999")]
        public int Orden { get; set; } = 1;

        [Required]
        public bool Activo { get; set; } = true;

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        public DateTime? FechaModificacion { get; set; }

        [StringLength(450)]
        public string? UsuarioCreacionId { get; set; }

        [StringLength(450)]
        public string? UsuarioModificacionId { get; set; }

        // Propiedades de navegación
        public virtual Models.Identity.ApplicationUser? UsuarioCreacion { get; set; }
        public virtual Models.Identity.ApplicationUser? UsuarioModificacion { get; set; }
    }
}
