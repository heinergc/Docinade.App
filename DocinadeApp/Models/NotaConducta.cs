using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RubricasApp.Web.Models
{
    /// <summary>
    /// Nota de conducta acumulada por estudiante y período académico
    /// Conforme al Art. 132 del REA 40862-V21
    /// </summary>
    [Table("NotasConducta")]
    public class NotaConducta
    {
        [Key]
        public int IdNotaConducta { get; set; }

        [Required]
        public int IdEstudiante { get; set; }

        [Required]
        public int IdPeriodo { get; set; }

        /// <summary>
        /// Nota inicial: 100 puntos
        /// </summary>
        [Required]
        public decimal NotaInicial { get; set; } = 100;

        /// <summary>
        /// Total de rebajos acumulados en el período
        /// </summary>
        [Required]
        public decimal TotalRebajos { get; set; } = 0;

        /// <summary>
        /// Nota final = NotaInicial - TotalRebajos
        /// </summary>
        [Required]
        public decimal NotaFinal { get; set; } = 100;

        /// <summary>
        /// Estado: Aprobado, Riesgo, Aplazado
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Estado { get; set; } = "Aprobado";

        /// <summary>
        /// Si está aplazado, requiere programa de acciones
        /// </summary>
        public bool RequiereProgramaAcciones { get; set; } = false;

        /// <summary>
        /// Programa de acciones asignado
        /// </summary>
        public int? IdProgramaAcciones { get; set; }

        /// <summary>
        /// Si se aplicó decisión profesional docente (Opción C)
        /// </summary>
        public bool DecisionProfesionalAplicada { get; set; } = false;

        public int? IdDecisionProfesional { get; set; }

        public DateTime FechaCalculo { get; set; } = DateTime.Now;

        public DateTime? FechaUltimaActualizacion { get; set; }

        // Navegación
        [ForeignKey("IdEstudiante")]
        public virtual Estudiante Estudiante { get; set; } = null!;

        [ForeignKey("IdPeriodo")]
        public virtual PeriodoAcademico Periodo { get; set; } = null!;

        [ForeignKey("IdProgramaAcciones")]
        public virtual ProgramaAccionesInstitucional? ProgramaAcciones { get; set; }

        [ForeignKey("IdDecisionProfesional")]
        public virtual DecisionProfesionalConducta? DecisionProfesional { get; set; }
    }
}
