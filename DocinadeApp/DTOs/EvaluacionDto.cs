using System.ComponentModel.DataAnnotations;

namespace DocinadeApp.DTOs
{
    /// <summary>Datos de evaluación para listados y reportes.</summary>
    public record EvaluacionDto(
        int IdEvaluacion,
        int IdEstudiante,
        string EstudianteNombre,
        int IdRubrica,
        string RubricaNombre,
        DateTime FechaEvaluacion,
        decimal? TotalPuntos,
        string Estado,
        string? Observaciones
    );

    /// <summary>Datos para crear una evaluación.</summary>
    public class EvaluacionCreateDto
    {
        [Required(ErrorMessage = "Debe seleccionar un estudiante.")]
        public int IdEstudiante { get; set; }

        [Required(ErrorMessage = "Debe seleccionar una rúbrica.")]
        public int IdRubrica { get; set; }

        public DateTime FechaEvaluacion { get; set; } = DateTime.Now;

        [StringLength(3000)]
        public string? Observaciones { get; set; }

        public string Estado { get; set; } = "BORRADOR";
    }
}
