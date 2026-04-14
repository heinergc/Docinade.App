using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RubricasApp.Web.Models.Identity;

namespace RubricasApp.Web.Models
{
    /// <summary>
    /// Modelo para registrar la asistencia de estudiantes por fecha y grupo
    /// </summary>
    public class Asistencia
    {
        [Key]
        public int AsistenciaId { get; set; }

        /// <summary>
        /// ID del estudiante
        /// </summary>
        [Required]
        public int EstudianteId { get; set; }

        /// <summary>
        /// ID del grupo
        /// </summary>
        [Required]
        public int GrupoId { get; set; }

        /// <summary>
        /// ID de la materia (clase específica)
        /// </summary>
        [Required]
        public int MateriaId { get; set; }

        /// <summary>
        /// ID de la lección/bloque específico (nuevo campo según especificación MEP)
        /// Nullable para compatibilidad con registros antiguos
        /// </summary>
        [Display(Name = "Lección/Bloque")]
        public int? IdLeccion { get; set; }

        /// <summary>
        /// Fecha del pase de lista
        /// </summary>
        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha")]
        public DateTime Fecha { get; set; }

        /// <summary>
        /// Estado de asistencia: P=Presente, A=Ausente, T=Tardanza, AJ=Ausencia Justificada, N=Neutro/Sin marcar
        /// </summary>
        [Required]
        [StringLength(2)]
        [Display(Name = "Estado de Asistencia")]
        public string Estado { get; set; } = "N";

        /// <summary>
        /// Justificación para ausencias o tardanzas
        /// </summary>
        [StringLength(500)]
        [Display(Name = "Justificación")]
        public string? Justificacion { get; set; }

        /// <summary>
        /// Observaciones adicionales
        /// </summary>
        [StringLength(1000)]
        [Display(Name = "Observaciones")]
        public string? Observaciones { get; set; }

        /// <summary>
        /// Hora de registro de la asistencia
        /// </summary>
        [Display(Name = "Hora de Registro")]
        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        /// <summary>
        /// Usuario que registró la asistencia
        /// </summary>
        [StringLength(450)]
        [Display(Name = "Registrado Por")]
        public string? RegistradoPorId { get; set; }

        /// <summary>
        /// Hora específica de llegada (para tardanzas)
        /// </summary>
        [Display(Name = "Hora de Llegada")]
        public TimeSpan? HoraLlegada { get; set; }

        /// <summary>
        /// Si la asistencia fue modificada posteriormente
        /// </summary>
        [Display(Name = "Modificada")]
        public bool EsModificacion { get; set; } = false;

        /// <summary>
        /// Fecha de la última modificación
        /// </summary>
        [Display(Name = "Última Modificación")]
        public DateTime? FechaModificacion { get; set; }

        /// <summary>
        /// Usuario que modificó la asistencia
        /// </summary>
        [StringLength(450)]
        public string? ModificadoPorId { get; set; }

        // Propiedades de navegación
        /// <summary>
        /// Estudiante al que pertenece el registro
        /// </summary>
        [ForeignKey("EstudianteId")]
        public virtual Estudiante Estudiante { get; set; } = null!;

        /// <summary>
        /// Grupo al que pertenece el registro
        /// </summary>
        [ForeignKey("GrupoId")]
        public virtual GrupoEstudiante Grupo { get; set; } = null!;

        /// <summary>
        /// Materia para la cual se está tomando asistencia
        /// </summary>
        [ForeignKey("MateriaId")]
        public virtual Materia Materia { get; set; } = null!;

        /// <summary>
        /// Lección/bloque específico (nuevo según especificación MEP)
        /// </summary>
        [ForeignKey("IdLeccion")]
        public virtual Leccion? Leccion { get; set; }

        /// <summary>
        /// Usuario que registró la asistencia
        /// </summary>
        [ForeignKey("RegistradoPorId")]
        public virtual ApplicationUser? RegistradoPor { get; set; }

        /// <summary>
        /// Usuario que modificó la asistencia
        /// </summary>
        [ForeignKey("ModificadoPorId")]
        public virtual ApplicationUser? ModificadoPor { get; set; }

        // Propiedades calculadas
        /// <summary>
        /// Indica si el estudiante está presente
        /// </summary>
        [NotMapped]
        public bool EstaPresente => Estado == "P";

        /// <summary>
        /// Indica si el estudiante está ausente (sin justificar)
        /// </summary>
        [NotMapped]
        public bool EstaAusente => Estado == "A";

        /// <summary>
        /// Indica si el estudiante llegó tarde
        /// </summary>
        [NotMapped]
        public bool LlegoTarde => Estado == "T";

        /// <summary>
        /// Indica si tiene ausencia justificada
        /// </summary>
        [NotMapped]
        public bool AusenciaJustificada => Estado == "AJ";

        /// <summary>
        /// Descripción del estado en formato legible
        /// </summary>
        [NotMapped]
        [Display(Name = "Estado")]
        public string EstadoDescripcion => Estado switch
        {
            "P" => "Presente",
            "A" => "Ausente",
            "T" => "Tardanza",
            "AJ" => "Ausencia Justificada",
            "N" => "Sin marcar",
            _ => "Desconocido"
        };

        /// <summary>
        /// Clase CSS para el estado (para styling)
        /// </summary>
        [NotMapped]
        public string EstadoCssClass => Estado switch
        {
            "P" => "success",
            "A" => "danger",
            "T" => "warning",
            "AJ" => "info",
            "N" => "secondary",
            _ => "secondary"
        };

        /// <summary>
        /// Icono FontAwesome para el estado
        /// </summary>
        [NotMapped]
        public string EstadoIcono => Estado switch
        {
            "P" => "fas fa-check-circle",
            "A" => "fas fa-times-circle",
            "T" => "fas fa-clock",
            "AJ" => "fas fa-file-alt",
            "N" => "fas fa-minus-circle",
            _ => "fas fa-question-circle"
        };
    }
}