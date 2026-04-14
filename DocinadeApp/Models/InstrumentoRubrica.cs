using System.ComponentModel.DataAnnotations;

namespace RubricasApp.Web.Models
{
    public class InstrumentoRubrica
    {
        public int InstrumentoEvaluacionId { get; set; }
        public int RubricaId { get; set; }
        
        // Alias para compatibilidad con vistas legacy
        public int InstrumentoId 
        { 
            get => InstrumentoEvaluacionId; 
            set => InstrumentoEvaluacionId = value; 
        }
        
        [Required]
        public DateTime FechaAsignacion { get; set; } = DateTime.Now;
        
        public int? OrdenPresentacion { get; set; }
        
        public bool EsObligatorio { get; set; } = false;
        
        // Propiedad para compatibilidad con vistas legacy (peso/ponderación)
        public decimal Ponderacion { get; set; } = 0;
        
        // Navigation properties
        public virtual InstrumentoEvaluacion InstrumentoEvaluacion { get; set; } = null!;
        public virtual Rubrica Rubrica { get; set; } = null!;
    }
}