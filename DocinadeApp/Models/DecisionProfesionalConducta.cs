using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DocinadeApp.Models.Identity;

namespace DocinadeApp.Models
{
    /// <summary>
    /// Registro de decisión profesional docente sobre conducta (Opción C)
    /// Permite al Comité de Evaluación resolver casos de estudiantes aplazados
    /// mediante criterio profesional fundamentado
    /// </summary>
    [Table("DecisionesProfesionalesConducta")]
    public class DecisionProfesionalConducta
    {
        [Key]
        public int IdDecision { get; set; }

        [Required]
        public int IdNotaConducta { get; set; }

        [Required]
        public int IdEstudiante { get; set; }

        [Required]
        public int IdPeriodo { get; set; }

        /// <summary>
        /// Justificación pedagógica obligatoria
        /// </summary>
        [Required]
        [StringLength(3000)]
        public string JustificacionPedagogica { get; set; } = string.Empty;

        [StringLength(2000)]
        public string? ConsideracionesAdicionales { get; set; }

        /// <summary>
        /// Decisión tomada: Mantener Aplazado, Asignar Aprobado
        /// </summary>
        [Required]
        [StringLength(50)]
        public string DecisionTomada { get; set; } = string.Empty; // "Mantener Aplazado", "Asignar Aprobado"

        /// <summary>
        /// Si se asigna aprobado, la nota se ajusta a la mínima
        /// </summary>
        public decimal? NotaAjustada { get; set; }

        [Required]
        [StringLength(450)]
        public string TomaDecisionPorId { get; set; } = string.Empty; // Coordinador/Director

        public DateTime FechaDecision { get; set; } = DateTime.Now;

        /// <summary>
        /// Número de acta del Comité de Evaluación
        /// </summary>
        [StringLength(50)]
        public string? NumeroActa { get; set; }

        public DateTime? FechaActa { get; set; }

        /// <summary>
        /// Miembros del Comité presentes
        /// </summary>
        [StringLength(1000)]
        public string? MiembrosComitePresentes { get; set; }

        [StringLength(2000)]
        public string? ObservacionesComite { get; set; }

        /// <summary>
        /// Registrado en expediente del estudiante
        /// </summary>
        [Required]
        public bool RegistradoEnExpediente { get; set; } = false;

        public DateTime? FechaRegistroExpediente { get; set; }

        // Navegación
        [ForeignKey("IdNotaConducta")]
        public virtual NotaConducta NotaConducta { get; set; } = null!;

        [ForeignKey("IdEstudiante")]
        public virtual Estudiante Estudiante { get; set; } = null!;

        [ForeignKey("IdPeriodo")]
        public virtual PeriodoAcademico Periodo { get; set; } = null!;

        [ForeignKey("TomaDecisionPorId")]
        public virtual ApplicationUser TomaDecisionPor { get; set; } = null!;
    }
}
