using System.ComponentModel.DataAnnotations;
using DocinadeApp.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DocinadeApp.ViewModels
{
    public class CrearEvaluacionViewModel
    {
        [Required(ErrorMessage = "Debe seleccionar un estudiante")]
        [Display(Name = "Estudiante")]
        public int IdEstudiante { get; set; }

        [Required(ErrorMessage = "Debe seleccionar una rúbrica")]
        [Display(Name = "Rúbrica")]
        public int IdRubrica { get; set; }

        [Required]
        [Display(Name = "Fecha de Evaluación")]
        public DateTime FechaEvaluacion { get; set; } = DateTime.Now;

        [StringLength(3000, ErrorMessage = "Las observaciones no pueden exceder 3000 caracteres")]
        [Display(Name = "Observaciones")]
        public string? Observaciones { get; set; }

        public List<DetalleEvaluacionViewModel> DetallesEvaluacion { get; set; } = new List<DetalleEvaluacionViewModel>();
    }

    public class EditarEvaluacionViewModel : CrearEvaluacionViewModel
    {
        public int IdEvaluacion { get; set; }
    }

    public class DetalleEvaluacionViewModel
    {
        [Required]
        public int IdItem { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un nivel de calificación")]
        public int IdNivel { get; set; }
    }

    public class ReporteEvaluacionesViewModel
    {
        public List<Evaluacion> Evaluaciones { get; set; } = new List<Evaluacion>();
        public int TotalEvaluaciones { get; set; }
        public decimal PromedioGeneral { get; set; }
        public decimal PuntajeMaximo { get; set; }
        public decimal PuntajeMinimo { get; set; }
    }

    // TODO: Filtros simplificados - ViewModel para filtros básicos (sin cascada)
    public class EvaluacionesFiltroViewModel
    {
        // Filtros básicos
        public int? EstudianteId { get; set; }
        public int? RubricaId { get; set; }
        public int? PeriodoId { get; set; }
        public bool ShowAll { get; set; } = false;

        // Listas para los dropdowns básicos
        public IEnumerable<SelectListItem> Estudiantes { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> Rubricas { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> Periodos { get; set; } = new List<SelectListItem>();

        // REMOVIDO: Propiedades de filtros en cascada que ya no se usan
        // public int? GrupoId { get; set; }
        // public int? MateriaId { get; set; }
        // public int? InstrumentoEvaluacionId { get; set; }
        // public IEnumerable<SelectListItem> Grupos { get; set; }
        // public IEnumerable<SelectListItem> Materias { get; set; }
        // public IEnumerable<SelectListItem> Instrumentos { get; set; }
    }
}