using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RubricasApp.Web.Models.Identity;

namespace RubricasApp.Web.Models
{
    /// <summary>
    /// Programa de Acciones de Interés Institucional o Comunal
    /// Para estudiantes aplazados en conducta (Art. 132)
    /// Supervisado por el Comité de Evaluación
    /// </summary>
    [Table("ProgramasAccionesInstitucional")]
    public class ProgramaAccionesInstitucional
    {
        [Key]
        public int IdPrograma { get; set; }

        [Required]
        public int IdNotaConducta { get; set; }

        [Required]
        public int IdEstudiante { get; set; }

        [Required]
        public int IdPeriodo { get; set; }

        [Required]
        [StringLength(200)]
        public string TituloPrograma { get; set; } = string.Empty;

        [Required]
        [StringLength(2000)]
        public string Descripcion { get; set; } = string.Empty;

        [StringLength(2000)]
        public string? ObjetivosEspecificos { get; set; }

        [StringLength(2000)]
        public string? ActividadesARealizar { get; set; }

        [StringLength(2000)]
        public string? CompromisosEstudiante { get; set; }

        [StringLength(2000)]
        public string? CompromisosFamilia { get; set; }

        [StringLength(2000)]
        public string? CriteriosEvaluacion { get; set; }

        [Required]
        public DateTime FechaInicio { get; set; }

        [Required]
        public DateTime FechaFinPrevista { get; set; }

        public DateTime? FechaFinReal { get; set; }

        /// <summary>
        /// Responsable de supervisar (Comité de Evaluación)
        /// </summary>
        [Required]
        [StringLength(450)]
        public string ResponsableSupervisionId { get; set; } = string.Empty;

        [StringLength(2000)]
        public string? ObservacionesSupervision { get; set; }

        /// <summary>
        /// Estado: Pendiente, En Proceso, Completado, No Completado
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Estado { get; set; } = "Pendiente";

        /// <summary>
        /// Resultado: Satisfactorio, No Satisfactorio
        /// </summary>
        [StringLength(50)]
        public string? ResultadoFinal { get; set; }

        public DateTime? FechaVerificacion { get; set; }

        [StringLength(450)]
        public string? VerificadoPorId { get; set; }

        [StringLength(2000)]
        public string? ConclusionesComite { get; set; }

        /// <summary>
        /// Si el programa fue completado satisfactoriamente, se aprueba la conducta
        /// </summary>
        public bool AprobarConducta { get; set; } = false;

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        // Navegación
        [ForeignKey("IdNotaConducta")]
        public virtual NotaConducta NotaConducta { get; set; } = null!;

        [ForeignKey("IdEstudiante")]
        public virtual Estudiante Estudiante { get; set; } = null!;

        [ForeignKey("IdPeriodo")]
        public virtual PeriodoAcademico Periodo { get; set; } = null!;

        [ForeignKey("ResponsableSupervisionId")]
        public virtual ApplicationUser ResponsableSupervision { get; set; } = null!;

        [ForeignKey("VerificadoPorId")]
        public virtual ApplicationUser? VerificadoPor { get; set; }
    }
}
