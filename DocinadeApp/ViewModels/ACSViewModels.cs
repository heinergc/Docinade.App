using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DocinadeApp.ViewModels
{
    /// <summary>
    /// Configuración específica de ACS para un instrumento de evaluación.
    /// Usado en EstudianteCalificacionInfo para personalizar criterios por estudiante.
    /// </summary>
    public class ConfiguracionACSInstrumento
    {
        public int InstrumentoId { get; set; }
        public bool Exento { get; set; }
        public int? RubricaModificadaId { get; set; }
        public string? NombreRubricaModificada { get; set; }
        public decimal? PonderacionPersonalizada { get; set; }
        public string? CriteriosAdaptados { get; set; }
        public string? Observaciones { get; set; }
    }

    /// <summary>
    /// ViewModel para configurar ACS de un estudiante específico
    /// </summary>
    public class EstudianteACSConfigViewModel
    {
        public int EstudianteId { get; set; }
        
        [Display(Name = "Estudiante")]
        public string NombreCompleto { get; set; } = string.Empty;
        
        [Display(Name = "Número de Identificación")]
        public string NumeroIdentificacion { get; set; } = string.Empty;
        
        [Display(Name = "Tipo de Adecuación")]
        public string TipoAdecuacion { get; set; } = "Significativa";
        
        [Display(Name = "Período Académico")]
        public int PeriodoAcademicoId { get; set; }
        
        [Display(Name = "Período")]
        public string NombrePeriodo { get; set; } = string.Empty;
        
        [Display(Name = "Grupo")]
        public string NombreGrupo { get; set; } = string.Empty;
        
        [Display(Name = "Materia")]
        public int? MateriaId { get; set; }
        
        public string? NombreMateria { get; set; }
        
        [Display(Name = "Detalles ACS")]
        [StringLength(500)]
        public string? DetallesACS { get; set; }
        
        [Display(Name = "Aplicar a Períodos Anteriores")]
        public bool AplicarPeriodosAnteriores { get; set; } = false;
        
        public bool EsPrimerPeriodo { get; set; } = true;
        
        /// <summary>
        /// Lista de instrumentos disponibles para configurar
        /// </summary>
        public List<InstrumentoACSConfiguracion> Instrumentos { get; set; } = new();
        
        // Para dropdowns
        public List<SelectListItem> PeriodosDisponibles { get; set; } = new();
        public List<SelectListItem> MateriasDisponibles { get; set; } = new();
    }

    /// <summary>
    /// Información de un instrumento para configuración ACS 123
    /// </summary>
    public class InstrumentoACSConfiguracion
    {
        public int InstrumentoId { get; set; }
        
        [Display(Name = "Instrumento")]
        public string Nombre { get; set; } = string.Empty;
        
        [Display(Name = "Ponderación Original (%)")]
        public decimal PonderacionOriginal { get; set; }
        
        [Display(Name = "Exento")]
        public bool Exento { get; set; } = false;
        
        [Display(Name = "Motivo Exención")]
        [StringLength(500)]
        public string? MotivoExencion { get; set; }
        
        [Display(Name = "Rúbrica Modificada")]
        public int? RubricaModificadaId { get; set; }
        
        [Display(Name = "Ponderación Personalizada (%)")]
        [Range(0.01, 100.00)]
        public decimal? PonderacionPersonalizada { get; set; }
        
        [Display(Name = "Criterios Adaptados")]
        [StringLength(1000)]
        public string? CriteriosAdaptados { get; set; }
        
        [Display(Name = "Observaciones")]
        [StringLength(1000)]
        public string? Observaciones { get; set; }
        
        /// <summary>
        /// Rúbricas disponibles para este instrumento
        /// </summary>
        public List<RubricaDisponibleInfo> RubricasDisponibles { get; set; } = new();
    }

    /// <summary>
    /// Información de rúbricas disponibles para selección
    /// </summary>
    public class RubricaDisponibleInfo
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public int TotalItems { get; set; }
        public bool EsModificadaACS { get; set; } = false;
    }

    /// <summary>
    /// ViewModel para formulario de edición de estudiante con opciones ACS
    /// </summary>
    public class EstudianteEditACSViewModel
    {
        public int EstudianteId { get; set; }
        
        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(100)]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Los apellidos son requeridos")]
        [StringLength(100)]
        [Display(Name = "Apellidos")]
        public string Apellidos { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "El número de identificación es requerido")]
        [StringLength(20)]
        [Display(Name = "Número de Identificación")]
        public string NumeroId { get; set; } = string.Empty;
        
        [EmailAddress]
        [StringLength(100)]
        [Display(Name = "Correo Electrónico")]
        public string? DireccionCorreo { get; set; }
        
        [Required]
        [StringLength(20)]
        [Display(Name = "Tipo de Adecuación")]
        public string TipoAdecuacion { get; set; } = "NoPresenta";
        
        [Display(Name = "Detalles ACS")]
        [StringLength(500)]
        public string? DetallesACS { get; set; }
        
        [Display(Name = "Aplicar ACS a Períodos Anteriores")]
        public bool AplicarACSPeriodosAnteriores { get; set; } = false;
        
        public bool EsPrimerPeriodo { get; set; } = true;
        
        public int PeriodoActualId { get; set; }
        
        public string TipoAdecuacionAnterior { get; set; } = "NoPresenta";
        
        // Opciones para dropdown
        public List<SelectListItem> TiposAdecuacion { get; set; } = new()
        {
            new SelectListItem { Value = "NoPresenta", Text = "No presenta" },
            new SelectListItem { Value = "Acceso", Text = "Adecuación de Acceso" },
            new SelectListItem { Value = "NoSignificativa", Text = "Adecuación No Significativa" },
            new SelectListItem { Value = "Significativa", Text = "Adecuación Curricular Significativa (ACS)" }
        };
    }

    /// <summary>
    /// ViewModel para listado de estudiantes con indicadores ACS
    /// </summary>
    public class EstudianteListACSViewModel
    {
        public int EstudianteId { get; set; }
        public string NumeroIdentificacion { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
        public string CorreoElectronico { get; set; } = string.Empty;
        public bool EstaActivo { get; set; }
        public string TipoAdecuacion { get; set; } = "NoPresenta";
        public bool RequiereACS { get; set; }
        public DateTime? FechaInicioACS { get; set; }
        public string? NombrePeriodoInicioACS { get; set; }
        
        public string TipoAdecuacionTexto =>TipoAdecuacion switch
        {
            "NoPresenta" => "No presenta",
            "Acceso" => "Acceso",
            "NoSignificativa" => "No Significativa",
            "Significativa" => "ACS",
            _ => TipoAdecuacion
        };
        
        public string IconoAdecuacion => TipoAdecuacion switch
        {
            "Significativa" => "fas fa-user-graduate",
            "NoSignificativa" => "fas fa-user-check",
            "Acceso" => "fas fa-universal-access",
            _ => "fas fa-user"
        };
        
        public string ColorBadge => TipoAdecuacion switch
        {
            "Significativa" => "warning",
            "NoSignificativa" => "info",
            "Acceso" => "secondary",
            _ => "light"
        };
    }

    /// <summary>
    /// ViewModel para reporte de estudiantes ACS
    /// </summary>
    public class ReporteEstudiantesACSViewModel
    {
        public string NombreInstitucion { get; set; } = string.Empty;
        public string PeriodoAcademico { get; set; } = string.Empty;
        public DateTime FechaGeneracion { get; set; } = DateTime.Now;
        
        public int TotalEstudiantes { get; set; }
        public int EstudiantesConACS { get; set; }
        public int EstudiantesConAdecuacionNoSignificativa { get; set; }
        public int EstudiantesConAdecuacionAcceso { get; set; }
        
        public decimal PorcentajeACS => TotalEstudiantes > 0 
            ? (decimal)EstudiantesConACS / TotalEstudiantes * 100 
            : 0;
        
        public List<EstudianteACSReporteInfo> Estudiantes { get; set; } = new();
        
        public Dictionary<string, int> DistribucionPorTipo { get; set; } = new();
    }

    /// <summary>
    /// Información detallada de estudiante ACS para reportes
    /// </summary>
    public class EstudianteACSReporteInfo
    {
        public string NumeroIdentificacion { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
        public string TipoAdecuacion { get; set; } = string.Empty;
        public DateTime? FechaInicio { get; set; }
        public string? Grupo { get; set; }
        public int CantidadInstrumentosExentos { get; set; }
        public int CantidadInstrumentosModificados { get; set; }
        public List<string> MateriasAfectadas { get; set; } = new();
    }
}
