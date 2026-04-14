using System.ComponentModel.DataAnnotations;
using RubricasApp.Web.Models.Academic;

namespace RubricasApp.Web.Models
{
    public class PeriodoAcademico
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El año es obligatorio.")]
        [Range(2020, 2100, ErrorMessage = "El año debe estar entre 2020 y 2100.")]
        [Display(Name = "Año")]
        public int Anio { get; set; }

        [Required(ErrorMessage = "El ciclo es obligatorio.")]
        [StringLength(10, ErrorMessage = "El ciclo no puede tener más de 10 caracteres.")]
        [Display(Name = "Ciclo")]
        public string Ciclo { get; set; } = string.Empty; // I, II, Verano, etc.

        [Required(ErrorMessage = "La fecha de inicio es obligatoria.")]
        [Display(Name = "Fecha de Inicio")]
        public DateTime FechaInicio { get; set; }

        [Required(ErrorMessage = "La fecha de fin es obligatoria.")]
        [Display(Name = "Fecha de Fin")]
        public DateTime FechaFin { get; set; }

        [Display(Name = "Activo")]
        public bool Activo { get; set; } = false;

        // Propiedades heredadas existentes para mantener compatibilidad
        [StringLength(10)]
        public string Codigo { get; set; } = string.Empty;

        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        public TipoPeriodo Tipo { get; set; }
        public int NumeroPeriodo { get; set; }

        // Propiedades calculadas
        public string NombreCompleto => $"{Anio} - {Ciclo}";

        // Propiedades de auditoría que pueden estar siendo usadas
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public DateTime? FechaModificacion { get; set; }
        public string? Estado { get; set; } = "Activo";

        // Navigation properties
        public virtual ICollection<Estudiante> Estudiantes { get; set; } = new List<Estudiante>();
        public virtual ICollection<MateriaPeriodo> Ofertas { get; set; } = new List<MateriaPeriodo>();
    }
}