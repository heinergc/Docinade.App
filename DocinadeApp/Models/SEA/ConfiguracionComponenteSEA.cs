using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RubricasApp.Web.Models.SEA
{
    /// <summary>
    /// Configuración que mapea InstrumentosEvaluacion a componentes del sistema SEA del MEP
    /// Permite flexibilidad por materia para adaptar instrumentos a requerimientos MEP
    /// </summary>
    public class ConfiguracionComponenteSEA
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int MateriaId { get; set; }

        [Required]
        public int InstrumentoEvaluacionId { get; set; }

        /// <summary>
        /// Componente SEA MEP: TRABAJO_COTIDIANO, TAREAS, PRUEBAS, PROYECTO
        /// ASISTENCIA se calcula independientemente desde tabla Asistencias
        /// </summary>
        [Required]
        [StringLength(50)]
        public string ComponenteSEA { get; set; } = string.Empty;

        /// <summary>
        /// Porcentaje asignado a este componente en la nota final (0-100)
        /// La suma de todos los componentes de una materia debe ser 100%
        /// </summary>
        [Range(0, 100)]
        public decimal Porcentaje { get; set; }

        public bool Activo { get; set; } = true;

        public DateTime FechaConfiguracion { get; set; } = DateTime.Now;

        [StringLength(256)]
        public string? UsuarioConfiguracion { get; set; }

        // Navigation properties
        [ForeignKey(nameof(MateriaId))]
        public virtual Materia Materia { get; set; } = null!;

        [ForeignKey(nameof(InstrumentoEvaluacionId))]
        public virtual InstrumentoEvaluacion InstrumentoEvaluacion { get; set; } = null!;
    }
}
