using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace RubricasApp.Web.Models.Identity
{
    /// <summary>
    /// Usuario del sistema con propiedades extendidas para el contexto académico
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(100)]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [Display(Name = "Apellidos")]
        public string Apellidos { get; set; } = string.Empty;

        [Display(Name = "Usuario Activo")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "Fecha de Creación")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [Display(Name = "Última Fecha de Acceso")]
        public DateTime? LastLoginDate { get; set; }

        // Alias para compatibilidad
        [Display(Name = "Apellido")]
        public string Apellido => Apellidos;

        [StringLength(20)]
        [Display(Name = "Número de Identificación")]
        public string? NumeroIdentificacion { get; set; }

        [StringLength(100)]
        [Display(Name = "Institución")]
        public string? Institucion { get; set; }

        [StringLength(50)]
        [Display(Name = "Departamento")]
        public string? Departamento { get; set; }

        [Display(Name = "Fecha de Registro")]
        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        [Display(Name = "Último Acceso")]
        public DateTime? UltimoAcceso { get; set; }

        [Display(Name = "Usuario Activo")]
        public bool Activo { get; set; } = true;

        [StringLength(500)]
        [Display(Name = "Observaciones")]
        public string? Observaciones { get; set; }

        // Propiedades calculadas
        [Display(Name = "Nombre Completo")]
        public string NombreCompleto => $"{Nombre} {Apellidos}";

        [Display(Name = "Iniciales")]
        public string Iniciales 
        { 
            get 
            {
                var inicialNombre = !string.IsNullOrEmpty(Nombre) ? Nombre[0].ToString().ToUpper() : "";
                var inicialApellido = !string.IsNullOrEmpty(Apellidos) ? Apellidos[0].ToString().ToUpper() : "";
                return $"{inicialNombre}{inicialApellido}";
            } 
        }

        // Relaciones de navegación
        public virtual ICollection<Rubrica> RubricasCreadas { get; set; } = new List<Rubrica>();
        public virtual ICollection<Evaluacion> EvaluacionesRealizadas { get; set; } = new List<Evaluacion>();
    }
}