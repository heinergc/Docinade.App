using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DocinadeApp.ViewModels
{
    public class InstrumentoRubricaViewModel
    {
        [Required(ErrorMessage = "Se requiere seleccionar un instrumento de evaluación.")]
        [Range(1, int.MaxValue, ErrorMessage = "Se requiere seleccionar un instrumento de evaluación válido.")]
        [Display(Name = "Instrumento de Evaluación")]
        public int InstrumentoId { get; set; }

        [Required(ErrorMessage = "Se requiere seleccionar una rúbrica.")]
        [Range(1, int.MaxValue, ErrorMessage = "Se requiere seleccionar una rúbrica válida.")]
        [Display(Name = "Rúbrica")]
        public int RubricaId { get; set; }

        [Range(0, 100, ErrorMessage = "La ponderación debe estar entre 0 y 100.")]
        [Display(Name = "Ponderación (%)")]
        public decimal Ponderacion { get; set; } = 0;

        [Display(Name = "Materia")]
        public int? MateriaId { get; set; }

        // Propiedades para mostrar información de materias
        public List<SelectListItem> MateriasDisponibles { get; set; } = new List<SelectListItem>();
        
        // Para mostrar información en las vistas
        public string? MateriaNombre { get; set; }
        public string? InstrumentoNombre { get; set; }
        public string? RubricaNombre { get; set; }
    }
}