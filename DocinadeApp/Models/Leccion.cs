using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RubricasApp.Web.Models
{
    /// <summary>
    /// Representa un bloque/lección en el horario de un grupo para una materia específica
    /// Según especificación MEP para registro de asistencia por bloque
    /// </summary>
    public class Leccion
    {
        [Key]
        public int IdLeccion { get; set; }

        /// <summary>
        /// Grupo al que pertenece la lección
        /// </summary>
        [Required(ErrorMessage = "El grupo es requerido")]
        [Display(Name = "Grupo")]
        public int IdGrupo { get; set; }

        /// <summary>
        /// Materia que se imparte en esta lección
        /// </summary>
        [Required(ErrorMessage = "La materia es requerida")]
        [Display(Name = "Materia")]
        public int MateriaId { get; set; }

        /// <summary>
        /// Número del bloque en el horario del día (1, 2, 3, etc.)
        /// </summary>
        [Required(ErrorMessage = "El número de bloque es requerido")]
        [Range(1, 12, ErrorMessage = "El número de bloque debe estar entre 1 y 12")]
        [Display(Name = "Número de Bloque")]
        public int NumeroBloque { get; set; }

        /// <summary>
        /// Día de la semana (Monday, Tuesday, etc.)
        /// </summary>
        [Required(ErrorMessage = "El día de la semana es requerido")]
        [Display(Name = "Día de la Semana")]
        public DayOfWeek DiaSemana { get; set; }

        /// <summary>
        /// Hora de inicio del bloque
        /// </summary>
        [Required(ErrorMessage = "La hora de inicio es requerida")]
        [Display(Name = "Hora de Inicio")]
        public TimeSpan HoraInicio { get; set; }

        /// <summary>
        /// Hora de fin del bloque
        /// </summary>
        [Required(ErrorMessage = "La hora de fin es requerida")]
        [Display(Name = "Hora de Fin")]
        public TimeSpan HoraFin { get; set; }

        /// <summary>
        /// Indica si la lección está activa
        /// </summary>
        [Display(Name = "Activa")]
        public bool Activa { get; set; } = true;

        /// <summary>
        /// Observaciones sobre la lección
        /// </summary>
        [StringLength(500, ErrorMessage = "Las observaciones no pueden exceder 500 caracteres")]
        [Display(Name = "Observaciones")]
        public string? Observaciones { get; set; }

        /// <summary>
        /// Fecha de creación del registro
        /// </summary>
        [Display(Name = "Fecha de Creación")]
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        /// <summary>
        /// Fecha de última modificación
        /// </summary>
        [Display(Name = "Fecha de Modificación")]
        public DateTime? FechaModificacion { get; set; }

        // Propiedades de navegación
        /// <summary>
        /// Grupo al que pertenece esta lección
        /// </summary>
        [ForeignKey("IdGrupo")]
        public virtual GrupoEstudiante Grupo { get; set; } = null!;

        /// <summary>
        /// Materia que se imparte en esta lección
        /// </summary>
        [ForeignKey("MateriaId")]
        public virtual Materia Materia { get; set; } = null!;

        /// <summary>
        /// Registros de asistencia para esta lección
        /// </summary>
        public virtual ICollection<Asistencia> Asistencias { get; set; } = new List<Asistencia>();

        // Propiedades calculadas
        /// <summary>
        /// Duración de la lección en minutos
        /// </summary>
        [NotMapped]
        [Display(Name = "Duración (minutos)")]
        public int DuracionMinutos => (int)(HoraFin - HoraInicio).TotalMinutes;

        /// <summary>
        /// Nombre del día de la semana
        /// </summary>
        [NotMapped]
        [Display(Name = "Día")]
        public string NombreDia => DiaSemana switch
        {
            DayOfWeek.Monday => "Lunes",
            DayOfWeek.Tuesday => "Martes",
            DayOfWeek.Wednesday => "Miércoles",
            DayOfWeek.Thursday => "Jueves",
            DayOfWeek.Friday => "Viernes",
            DayOfWeek.Saturday => "Sábado",
            DayOfWeek.Sunday => "Domingo",
            _ => "Desconocido"
        };

        /// <summary>
        /// Formato de hora legible (ej: "08:00 - 09:40")
        /// </summary>
        [NotMapped]
        [Display(Name = "Horario")]
        public string HorarioFormateado => $"{HoraInicio:hh\\:mm} - {HoraFin:hh\\:mm}";

        /// <summary>
        /// Descripción completa de la lección
        /// </summary>
        [NotMapped]
        [Display(Name = "Descripción")]
        public string DescripcionCompleta => $"Bloque {NumeroBloque} - {NombreDia} {HorarioFormateado}";

        /// <summary>
        /// Validación personalizada para verificar que HoraFin sea mayor que HoraInicio
        /// </summary>
        [NotMapped]
        public bool EsHorarioValido => HoraFin > HoraInicio;

        /// <summary>
        /// Número de asistencias registradas para esta lección (solo para uso en vistas)
        /// </summary>
        [NotMapped]
        public int AsistenciasRegistradas { get; set; }
    }
}
