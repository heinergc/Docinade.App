using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DocinadeApp.Models.Identity;

namespace DocinadeApp.Models
{
    /// <summary>
    /// Registro de boleta de conducta para un estudiante
    /// </summary>
    [Table("BoletasConducta")]
    public class BoletaConducta
    {
        [Key]
        public int IdBoleta { get; set; }

        [Required]
        public int IdEstudiante { get; set; }

        [Required]
        public int IdTipoFalta { get; set; }

        [Required]
        public int IdPeriodo { get; set; }

        /// <summary>
        /// Rebajo aplicado en esta boleta (entre el rango del tipo de falta)
        /// </summary>
        [Required]
        public int RebajoAplicado { get; set; }

        [Required]
        [StringLength(2000)]
        public string Descripcion { get; set; } = string.Empty;

        [StringLength(500)]
        public string? RutaEvidencia { get; set; } // Ruta al archivo adjunto

        [Required]
        [StringLength(450)]
        public string DocenteEmisorId { get; set; } = string.Empty;

        [Required]
        public DateTime FechaEmision { get; set; } = DateTime.Now;

        /// <summary>
        /// ID del profesor guía del estudiante (de la tabla Profesores)
        /// </summary>
        public int? ProfesorGuiaId { get; set; }

        public DateTime? FechaNotificacion { get; set; }

        [Required]
        public bool NotificacionEnviada { get; set; } = false;

        [StringLength(1000)]
        public string? ObservacionesProfesorGuia { get; set; }

        [Required]
        [StringLength(50)]
        public string Estado { get; set; } = "Activa"; // Activa, Anulada, Apelada

        [StringLength(1000)]
        public string? MotivoAnulacion { get; set; }

        public DateTime? FechaAnulacion { get; set; }

        [StringLength(450)]
        public string? AnuladaPorId { get; set; }

        // Navegación
        [ForeignKey("IdEstudiante")]
        public virtual Estudiante Estudiante { get; set; } = null!;

        [ForeignKey("IdTipoFalta")]
        public virtual TipoFalta TipoFalta { get; set; } = null!;

        [ForeignKey("IdPeriodo")]
        public virtual PeriodoAcademico Periodo { get; set; } = null!;

        [ForeignKey("DocenteEmisorId")]
        public virtual ApplicationUser DocenteEmisor { get; set; } = null!;

        [ForeignKey("ProfesorGuiaId")]
        public virtual Profesor? ProfesorGuia { get; set; }

        [ForeignKey("AnuladaPorId")]
        public virtual ApplicationUser? AnuladaPor { get; set; }
    }
}
