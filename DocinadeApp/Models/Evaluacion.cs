using System.ComponentModel.DataAnnotations;
using DocinadeApp.Models.Identity;

namespace DocinadeApp.Models
{
    public class Evaluacion
    {
        public int IdEvaluacion { get; set; }
        
        public int IdEstudiante { get; set; }
        
        public int IdRubrica { get; set; }
        
        [Display(Name = "Fecha de Evaluación")]
        public DateTime FechaEvaluacion { get; set; } = DateTime.Now;
        
        [Display(Name = "Total de Puntos")]
        public decimal? TotalPuntos { get; set; }
        
        [StringLength(3000, ErrorMessage = "Las observaciones no pueden exceder 3000 caracteres")]
        [Display(Name = "Observaciones")]
        public string? Observaciones { get; set; }
        
        [StringLength(20, ErrorMessage = "El estado no puede exceder 20 caracteres")]
        [Display(Name = "Estado")]
        public string Estado { get; set; } = "BORRADOR"; // BORRADOR, COMPLETADA, FINALIZADA

        // Relación con el usuario que realizó la evaluación
        [Display(Name = "Evaluado por")]
        public string? EvaluadoPorId { get; set; }

        [Display(Name = "Fecha de finalización")]
        public DateTime? FechaFinalizacion { get; set; }

        [Display(Name = "Tiempo de evaluación (minutos)")]
        public int? TiempoEvaluacionMinutos { get; set; }
        
        // Navigation properties
        public virtual ApplicationUser? EvaluadoPor { get; set; }
        public virtual Estudiante Estudiante { get; set; } = null!;
        public virtual Rubrica Rubrica { get; set; } = null!;
        public virtual ICollection<DetalleEvaluacion> DetallesEvaluacion { get; set; } = new List<DetalleEvaluacion>();
    }
}