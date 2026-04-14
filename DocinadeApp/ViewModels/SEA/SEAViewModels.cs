using RubricasApp.Web.Models.SEA;
using System.ComponentModel.DataAnnotations;

namespace RubricasApp.Web.ViewModels.SEA
{
    /// <summary>
    /// ViewModel principal para el Reporte SEA por periodo
    /// Diseñado para cumplir con especificaciones MEP
    /// </summary>
    public class SEAReporteViewModel
    {
        public int? MateriaId { get; set; }
        public int? PeriodoAcademicoId { get; set; }
        
        [Display(Name = "Materia")]
        public string NombreMateria { get; set; } = "Todas las materias";
        
        [Display(Name = "Periodo Académico")]
        public string NombrePeriodo { get; set; } = "Sin periodo";
        
        public DateTime FechaGeneracion { get; set; } = DateTime.Now;
        
        /// <summary>
        /// Listado de estudiantes con sus calificaciones y asistencia
        /// </summary>
        public List<SEAEstudianteReporte> Estudiantes { get; set; } = new();
        
        /// <summary>
        /// Estudiantes regulares (sin ACS)
        /// </summary>
        public List<SEAEstudianteReporte> EstudiantesRegulares => 
            Estudiantes.Where(e => !e.RequiereACS).ToList();
        
        /// <summary>
        /// Estudiantes con Adecuación Curricular Significativa
        /// </summary>
        public List<SEAEstudianteReporte> EstudiantesACS => 
            Estudiantes.Where(e => e.RequiereACS).ToList();
        
        public bool TieneEstudiantesACS => EstudiantesACS.Any();
        
        /// <summary>
        /// Estadísticas generales del reporte
        /// </summary>
        public SEAEstadisticasGenerales Estadisticas { get; set; } = new();
        
        /// <summary>
        /// Componentes SEA configurados para esta materia
        /// </summary>
        public List<string> ComponentesActivos { get; set; } = new();
        
        /// <summary>
        /// Indica si las ponderaciones suman 100%
        /// </summary>
        public bool PonderacionesValidas { get; set; }
        
        public decimal PonderacionTotal { get; set; }
        
        /// <summary>
        /// Opción de redondeo aplicada al reporte
        /// </summary>
        public OpcionRedondeoSEA OpcionRedondeo { get; set; } = OpcionRedondeoSEA.SinDecimales;
    }

    /// <summary>
    /// Información de un estudiante en el reporte SEA
    /// Incluye calificaciones por componente y asistencia
    /// </summary>
    public class SEAEstudianteReporte
    {
        public int EstudianteId { get; set; }
        
        [Display(Name = "Número de Identificación")]
        public string NumeroIdentificacion { get; set; } = string.Empty;
        
        [Display(Name = "Nombre Completo")]
        public string NombreCompleto { get; set; } = string.Empty;
        
        public string Nombre { get; set; } = string.Empty;
        public string Apellidos { get; set; } = string.Empty;
        
        public string CorreoElectronico { get; set; } = string.Empty;
        
        /// <summary>
        /// Indica si el estudiante requiere ACS
        /// </summary>
        public bool RequiereACS { get; set; }
        
        public string TipoAdecuacion { get; set; } = "NoPresenta";
        
        // Componentes SEA
        [Display(Name = "Trabajo Cotidiano")]
        public decimal? TrabajoCotidiano { get; set; }
        
        [Display(Name = "Tareas")]
        public decimal? Tareas { get; set; }
        
        [Display(Name = "Pruebas")]
        public decimal? Pruebas { get; set; }
        
        [Display(Name = "Asistencia")]
        public decimal? Asistencia { get; set; }
        
        [Display(Name = "Proyecto")]
        public decimal? Proyecto { get; set; }
        
        /// <summary>
        /// Nota final ponderada (suma de componentes)
        /// </summary>
        [Display(Name = "Nota Final")]
        public decimal? NotaFinal { get; set; }
        
        /// <summary>
        /// Estado general del estudiante
        /// </summary>
        public string EstadoGeneral { get; set; } = EstadosEstudianteSEA.SIN_DATOS;
        
        public bool Aprobado => NotaFinal.HasValue && NotaFinal.Value >= 70;
        
        /// <summary>
        /// Porcentaje de asistencia detallado
        /// </summary>
        public int TotalLecciones { get; set; }
        public int LeccionesPresente { get; set; }
        public int LeccionesAusente { get; set; }
        public int LeccionesTardanza { get; set; }
        public int LeccionesJustificadas { get; set; }
        
        public decimal PorcentajeAsistencia => TotalLecciones > 0 
            ? Math.Round((decimal)LeccionesPresente / TotalLecciones * 100, 2) 
            : 0;
        
        /// <summary>
        /// Advertencias de validación (ej: ID mal formateado)
        /// </summary>
        public List<string> AdvertenciasValidacion { get; set; } = new();
        
        public bool TieneAdvertencias => AdvertenciasValidacion.Any();
        
        /// <summary>
        /// Diccionario con notas por componente SEA
        /// </summary>
        public Dictionary<string, decimal?> NotasPorComponente { get; set; } = new();

        /// <summary>
        /// Porcentaje de completitud de evaluaciones
        /// </summary>
        public decimal PorcentajeCompletado { get; set; }
    }

    /// <summary>
    /// Información de un componente SEA individual
    /// </summary>
    public class SEAComponenteInfo
    {
        public string CodigoComponente { get; set; } = string.Empty;
        
        [Display(Name = "Componente")]
        public string NombreComponente { get; set; } = string.Empty;
        
        [Display(Name = "Porcentaje")]
        public decimal Porcentaje { get; set; }
        
        public int InstrumentoEvaluacionId { get; set; }
        public string NombreInstrumento { get; set; } = string.Empty;
        
        public bool EstaConfigurado { get; set; }
    }

    /// <summary>
    /// Estadísticas generales del reporte SEA
    /// </summary>
    public class SEAEstadisticasGenerales
    {
        public int TotalEstudiantes { get; set; }
        public int EstudiantesConDatos { get; set; }
        public int EstudiantesSinDatos { get; set; }
        
        [Display(Name = "Promedio General")]
        public decimal PromedioGeneral { get; set; }
        
        public decimal NotaMaxima { get; set; }
        public decimal NotaMinima { get; set; }
        
        public int EstudiantesAprobados { get; set; }
        public int EstudiantesReprobados { get; set; }
        
        [Display(Name = "% Aprobación")]
        public decimal PorcentajeAprobacion { get; set; }
        
        [Display(Name = "Asistencia Promedio")]
        public decimal AsistenciaPromedio { get; set; }
        
        /// <summary>
        /// Distribución de notas por rangos
        /// </summary>
        public Dictionary<string, int> DistribucionNotas { get; set; } = new()
        {
            ["90-100"] = 0,
            ["80-89"] = 0,
            ["70-79"] = 0,
            ["60-69"] = 0,
            ["< 60"] = 0
        };
        
        /// <summary>
        /// Promedios por componente SEA
        /// </summary>
        public Dictionary<string, decimal> PromediosPorComponente { get; set; } = new();
    }

    /// <summary>
    /// ViewModel para configuración de componentes SEA
    /// </summary>
    public class ConfiguracionSEAViewModel
    {
        public int MateriaId { get; set; }
        public string NombreMateria { get; set; } = string.Empty;
        
        /// <summary>
        /// Lista de instrumentos disponibles para mapear
        /// </summary>
        public List<InstrumentoDisponibleSEA> InstrumentosDisponibles { get; set; } = new();
        
        /// <summary>
        /// Configuraciones actuales
        /// </summary>
        public List<ConfiguracionComponenteItem> ConfiguracionesActuales { get; set; } = new();
        
        public decimal PorcentajeTotal => ConfiguracionesActuales.Sum(c => c.Porcentaje);
        public bool PonderacionValida => Math.Abs(PorcentajeTotal - 100) < 0.01m;
    }

    public class InstrumentoDisponibleSEA
    {
        public int InstrumentoId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int TotalRubricas { get; set; }
        public bool YaConfigurado { get; set; }
    }

    public class ConfiguracionComponenteItem
    {
        public int Id { get; set; }
        public int InstrumentoEvaluacionId { get; set; }
        public string NombreInstrumento { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Debe seleccionar un componente SEA")]
        public string ComponenteSEA { get; set; } = string.Empty;
        
        [Range(0, 100, ErrorMessage = "El porcentaje debe estar entre 0 y 100")]
        [Display(Name = "Porcentaje")]
        public decimal Porcentaje { get; set; }
        
        public bool Activo { get; set; } = true;
    }
}
