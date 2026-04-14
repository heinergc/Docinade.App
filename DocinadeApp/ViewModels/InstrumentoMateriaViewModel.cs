using System.ComponentModel.DataAnnotations;

namespace DocinadeApp.ViewModels
{
    public class InstrumentoMateriaViewModel
    {
        [Required(ErrorMessage = "El instrumento de evaluación es obligatorio.")]
        [Display(Name = "Instrumento de Evaluación")]
        public int InstrumentoId { get; set; }

        [Required(ErrorMessage = "La materia es obligatoria.")]
        [Display(Name = "Materia")]
        public int MateriaId { get; set; }

        [Required(ErrorMessage = "El período académico es obligatorio.")]
        [Display(Name = "Período Académico")]
        public int PeriodoAcademicoId { get; set; }

        // Para mostrar información en las vistas
        public string? InstrumentoNombre { get; set; }
        public string? MateriaNombre { get; set; }
        public string? MateriaDescripcion { get; set; }
        public bool? InstrumentoActivo { get; set; }
        public string? PeriodoAcademicoNombre { get; set; }
        public DateTime? FechaAsignacion { get; set; } = DateTime.Now;
    }
}