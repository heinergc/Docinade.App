using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DocinadeApp.Models
{
    /// <summary>
    /// Representa la relaci�n entre un instrumento de evaluaci�n y una materia
    /// </summary>
    public class InstrumentoMateria
    {
        // ?? NOTA: Esta entidad usa clave compuesta (InstrumentoEvaluacionId, MateriaId)
        // No necesita una propiedad Id individual
        
        [Required]
        public int InstrumentoEvaluacionId { get; set; }
        
        [Required]
        public int MateriaId { get; set; }
        
        // Alias para compatibilidad con vistas legacy
        [NotMapped]
        public int InstrumentoId 
        { 
            get => InstrumentoEvaluacionId; 
            set => InstrumentoEvaluacionId = value; 
        }
        
        // Propiedad para compatibilidad con vistas legacy
        public int PeriodoAcademicoId { get; set; }
        
        [Required]
        public DateTime FechaAsignacion { get; set; } = DateTime.Now;
        
        public int? OrdenPresentacion { get; set; }
        
        public bool EsObligatorio { get; set; } = false;
        
        // Propiedad para observaciones adicionales
        public string? Observaciones { get; set; }
        
        // Navigation properties
        [ForeignKey(nameof(InstrumentoEvaluacionId))]
        public virtual InstrumentoEvaluacion InstrumentoEvaluacion { get; set; } = null!;
        
        [ForeignKey(nameof(MateriaId))]
        public virtual Materia Materia { get; set; } = null!;
        
        [ForeignKey(nameof(PeriodoAcademicoId))]
        public virtual PeriodoAcademico? PeriodoAcademico { get; set; }
    }
}