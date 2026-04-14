using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DocinadeApp.Models
{
    /// <summary>
    /// Configuración específica de ACS para un estudiante en un instrumento de evaluación particular.
    /// Permite personalizar criterios, ponderaciones y exenciones según las necesidades educativas.
    /// </summary>
    public class EstudianteInstrumentoACS
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [Display(Name = "Estudiante")]
        public int EstudianteId { get; set; }
        
        [Required]
        [Display(Name = "Instrumento de Evaluación")]
        public int InstrumentoEvaluacionId { get; set; }
        
        [Required]
        [Display(Name = "Período Académico")]
        public int PeriodoAcademicoId { get; set; }
        
        /// <summary>
        /// ID de rúbrica modificada/adaptada para este estudiante.
        /// Si es null, usa la rúbrica original del grupo.
        /// </summary>
        [Display(Name = "Rúbrica Modificada")]
        public int? RubricaModificadaId { get; set; }
        
        /// <summary>
        /// Ponderación personalizada para este instrumento (0.01-100.00).
        /// Si es null, usa la ponderación del grupo.
        /// </summary>
        [Display(Name = "Ponderación Personalizada (%)")]
        [Range(0.01, 100.00)]
        [Column(TypeName = "decimal(5,2)")]
        public decimal? PonderacionPersonalizadaPorcentaje { get; set; }
        
        /// <summary>
        /// Indica si el estudiante está exento de este instrumento.
        /// </summary>
        [Display(Name = "Exento")]
        public bool Exento { get; set; } = false;
        
        /// <summary>
        /// Motivo de la exención (si aplica).
        /// </summary>
        [Display(Name = "Motivo de Exención")]
        [StringLength(500)]
        public string? MotivoExencion { get; set; }
        
        /// <summary>
        /// Criterios de evaluación adaptados (texto descriptivo).
        /// </summary>
        [Display(Name = "Criterios Adaptados")]
        [StringLength(1000)]
        public string? CriteriosAdaptados { get; set; }
        
        /// <summary>
        /// Observaciones adicionales sobre la adaptación.
        /// </summary>
        [Display(Name = "Observaciones")]
        [StringLength(1000)]
        public string? Observaciones { get; set; }
        
        [Display(Name = "Fecha de Creación")]
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        
        [Display(Name = "Fecha de Modificación")]
        public DateTime? FechaModificacion { get; set; }
        
        [Display(Name = "Usuario que Creó")]
        [StringLength(256)]
        public string? UsuarioCreacion { get; set; }
        
        [Display(Name = "Usuario que Modificó")]
        [StringLength(256)]
        public string? UsuarioModificacion { get; set; }
        
        // Navigation properties
        [ForeignKey("EstudianteId")]
        public virtual Estudiante Estudiante { get; set; } = null!;
        
        [ForeignKey("InstrumentoEvaluacionId")]
        public virtual InstrumentoEvaluacion Instrumento { get; set; } = null!;
        
        [ForeignKey("PeriodoAcademicoId")]
        public virtual PeriodoAcademico Periodo { get; set; } = null!;
        
        [ForeignKey("RubricaModificadaId")]
        public virtual Rubrica? RubricaModificada { get; set; }
    }
}
