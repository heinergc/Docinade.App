using System.ComponentModel.DataAnnotations;

namespace RubricasApp.Web.ViewModels
{
    /// <summary>
    /// ViewModel para la página principal del sistema de asistencia
    /// </summary>
    public class AsistenciaIndexViewModel
    {
        public List<GrupoEstudiantesViewModel> Grupos { get; set; } = new();
        public List<PeriodoViewModel> Periodos { get; set; } = new();
        public int TotalGrupos { get; set; }
    }

    /// <summary>
    /// ViewModel para representar un período académico
    /// </summary>
    public class PeriodoViewModel
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
    }

    /// <summary>
    /// ViewModel para representar la información básica de un grupo de estudiantes para asistencia
    /// </summary>
    public class GrupoEstudiantesViewModel
    {
        public int Id { get; set; }
        
        [Required]
        [Display(Name = "Nombre del Grupo")]
        public string Nombre { get; set; } = string.Empty;
        
        [Display(Name = "Código del Grupo")]
        public string? Codigo { get; set; }
        
        [Display(Name = "Descripción")]
        public string? Descripcion { get; set; }
        
        [Display(Name = "Período Académico")]
        public string? PeriodoAcademico { get; set; }
        
        public int TotalEstudiantes { get; set; }
    }



    /// <summary>
    /// ViewModel principal para el pase de lista
    /// </summary>
    public class PaseListaViewModel
    {
        [Required]
        public GrupoEstudiantesViewModel Grupo { get; set; } = new();
        
        /// <summary>
        /// ID de la materia para el pase de lista
        /// </summary>
        [Required]
        [Display(Name = "Materia")]
        public int MateriaId { get; set; }
        
        [Required]
        [Display(Name = "Lista de Estudiantes")]
        public List<EstudianteAsistenciaViewModel> Estudiantes { get; set; } = new();
        
        [Display(Name = "Fecha del Pase de Lista")]
        [DataType(DataType.Date)]
        public DateTime Fecha { get; set; } = DateTime.Now.Date;
        
        [Display(Name = "Observaciones Generales")]
        public string? ObservacionesGenerales { get; set; }
        
        /// <summary>
        /// Total de estudiantes en la lista
        /// </summary>
        public int TotalEstudiantes => Estudiantes.Count;
        
        /// <summary>
        /// Número de estudiantes presentes
        /// </summary>
        public int EstudiantesPresentes => Estudiantes.Count(e => e.EsPresente);
        
        /// <summary>
        /// Número de estudiantes ausentes
        /// </summary>
        public int EstudiantesAusentes => Estudiantes.Count(e => e.EsAusente);
        
        /// <summary>
        /// Número de estudiantes que llegaron tarde
        /// </summary>
        public int EstudiantesTardanza => Estudiantes.Count(e => e.EsTarde);
        
        /// <summary>
        /// Número de ausencias justificadas
        /// </summary>
        public int AusenciasJustificadas => Estudiantes.Count(e => e.EsAusenciaJustificada);
        
        /// <summary>
        /// Número de ausencias sin justificar
        /// </summary>
        public int AusenciasSinJustificar => Estudiantes.Count(e => e.EsAusente);
        
        /// <summary>
        /// Total de ausencias (justificadas y sin justificar)
        /// </summary>
        public int TotalAusencias => EstudiantesAusentes + AusenciasJustificadas;
        
        /// <summary>
        /// Porcentaje de asistencia
        /// </summary>
        public double PorcentajeAsistencia => TotalEstudiantes > 0 
            ? Math.Round((double)EstudiantesPresentes / TotalEstudiantes * 100, 2) 
            : 0;
    }

    /// <summary>
    /// ViewModel para guardar la asistencia (usado en el POST del formulario)
    /// </summary>
    public class GuardarAsistenciaViewModel
    {
        [Required]
        public int GrupoId { get; set; }
        
        [Required]
        [DataType(DataType.Date)]
        public DateTime Fecha { get; set; }
        
        [Required]
        public List<EstudianteAsistenciaViewModel> Estudiantes { get; set; } = new();
        
        public string? ObservacionesGenerales { get; set; }
    }

    /// <summary>
    /// ViewModel para mostrar el resumen de asistencia
    /// </summary>
    public class ResumenAsistenciaViewModel
    {
        public GrupoEstudiantesViewModel Grupo { get; set; } = new();
        public DateTime Fecha { get; set; }
        public int TotalEstudiantes { get; set; }
        public int EstudiantesPresentes { get; set; }
        public int EstudiantesAusentes { get; set; }
        public int EstudiantesTardanza { get; set; }
        public int AusenciasJustificadas { get; set; }
        public double PorcentajeAsistencia { get; set; }
        public string? ObservacionesGenerales { get; set; }
        
        public List<EstudianteAsistenciaViewModel> DetalleEstudiantes { get; set; } = new();
    }

    /// <summary>
    /// ViewModel para cada estudiante en la lista de asistencia
    /// </summary>
    public class EstudianteAsistenciaViewModel
    {
        public int EstudianteId { get; set; }

        [Display(Name = "Nombre Completo")]
        public string NombreCompleto { get; set; } = string.Empty;

        [Display(Name = "Número de ID")]
        public string NumeroId { get; set; } = string.Empty;

        [Display(Name = "Estado de Asistencia")]
        public string? Estado { get; set; } // P, A, T, AJ, N (neutro)

        [Display(Name = "Justificación")]
        [StringLength(500)]
        public string? Justificacion { get; set; }

        // Para identificar si ya existe un registro
        public int? AsistenciaId { get; set; }

        // Propiedades calculadas para la vista
        public bool EsPresente => Estado == "P";
        public bool EsAusente => Estado == "A";
        public bool EsTarde => Estado == "T";
        public bool EsAusenciaJustificada => Estado == "AJ";
        public bool EsNeutro => string.IsNullOrEmpty(Estado) || Estado == "N";

        public string EstadoDescripcion => Estado switch
        {
            "P" => "Presente",
            "A" => "Ausente",
            "T" => "Tardanza",
            "AJ" => "Ausencia Justificada",
            "N" => "Sin registrar",
            _ => "Sin registrar"
        };

        public string EstadoCssClass => Estado switch
        {
            "P" => "badge-success",
            "A" => "badge-danger",
            "T" => "badge-warning",
            "AJ" => "badge-info",
            _ => "badge-secondary"
        };

        public string EstadoIcono => Estado switch
        {
            "P" => "fas fa-check",
            "A" => "fas fa-times",
            "T" => "fas fa-clock",
            "AJ" => "fas fa-exclamation-triangle",
            _ => "fas fa-question"
        };
    }
}