using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace RubricasApp.Web.ViewModels
{
    /// <summary>
    /// ViewModel para el cuaderno calificador din�mico basado en evaluaciones
    /// </summary>
    public class CuadernoCalificadorDinamicoViewModel
    {
        // Filtros aplicados
        public int? MateriaId { get; set; }
        public int? PeriodoAcademicoId { get; set; }
        public string NombreMateria { get; set; } = string.Empty;
        public string NombrePeriodoAcademico { get; set; } = string.Empty;
        
        // Informaci�n general
        public DateTime FechaGeneracion { get; set; } = DateTime.Now;
        public int TotalEstudiantes { get; set; }
        public int EstudiantesConEvaluaciones { get; set; }
        
        // Instrumentos de la materia con sus ponderaciones
        public List<InstrumentoEvaluacionInfo> Instrumentos { get; set; } = new();
        
        // Calificaciones de estudiantes - Separadas por tipo ACS
        public List<EstudianteCalificacionInfo> EstudiantesCalificaciones { get; set; } = new();
        
        // 🆕 SEPARACIÓN ACS: Lista de estudiantes regulares (sin ACS)
        public List<EstudianteCalificacionInfo> EstudiantesRegulares { get; set; } = new();
        
        // 🆕 SEPARACIÓN ACS: Lista de estudiantes con Adecuación Curricular Significativa
        public List<EstudianteCalificacionInfo> EstudiantesACS { get; set; } = new();
        
        // 🆕 Indicador si hay estudiantes ACS en el grupo
        public bool TieneEstudiantesACS => EstudiantesACS?.Any() ?? false;
        
        // Estad�sticas
        public EstadisticasGenerales Estadisticas { get; set; } = new();
        
        // Para los dropdowns de filtros
        public List<SelectListItem> MateriasDisponibles { get; set; } = new();
        public List<SelectListItem> PeriodosDisponibles { get; set; } = new();
        
        // Validaciones
        public bool PonderacionesValidas => Math.Abs(Instrumentos.Sum(i => i.PorcentajePonderacion) - 100m) < 0.01m;
        public decimal PonderacionTotal => Instrumentos.Sum(i => i.PorcentajePonderacion);
        public bool TieneDatos => EstudiantesCalificaciones.Any();
        public decimal NotaMinimaAprobacion { get; set; } = 70m; // Por defecto 70%
        
        // Diagnóstico de instrumentos sin configuración
        public List<string> InstrumentosSinRubricas { get; set; } = new();
        public bool TieneInstrumentosSinRubricas => InstrumentosSinRubricas.Any();
        public int TotalInstrumentosAsignados { get; set; }
    }

    /// <summary>
    /// Informaci�n de un instrumento de evaluaci�n con sus m�tricas
    /// </summary>
    public class InstrumentoEvaluacionInfo
    {
        public int InstrumentoId { get; set; }
        public string NombreInstrumento { get; set; } = string.Empty;
        public string DescripcionInstrumento { get; set; } = string.Empty;
        public decimal PorcentajePonderacion { get; set; }
        public int TotalRubricas { get; set; }
        public int TotalEvaluacionesCompletas { get; set; }
        public decimal NotaPromedioInstrumento { get; set; }
        public List<RubricaInstrumentoInfo> Rubricas { get; set; } = new();
        
        // Para el c�lculo del valor final del instrumento
        public decimal PuntajeMaximoPosible { get; set; }
    }

    /// <summary>
    /// Informaci�n de una r�brica dentro de un instrumento
    /// </summary>
    public class RubricaInstrumentoInfo
    {
        public int RubricaId { get; set; }
        public string NombreRubrica { get; set; } = string.Empty;
        public decimal PonderacionEnInstrumento { get; set; } // % que representa dentro del instrumento
        public int TotalEvaluaciones { get; set; }
        public decimal NotaPromedioRubrica { get; set; }
    }

    /// <summary>
    /// Informaci�n de calificaciones de un estudiante
    /// </summary>
    public class EstudianteCalificacionInfo
    {
        public int EstudianteId { get; set; }
        public string NumeroIdentificacion { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
        public string CorreoElectronico { get; set; } = string.Empty;
        
        // 🆕 Información ACS
        public bool RequiereACS { get; set; }
        public string TipoAdecuacion { get; set; } = "NoPresenta";
        public string? DetallesACS { get; set; }
        
        // 🆕 Configuraciones ACS por instrumento
        public Dictionary<int, ConfiguracionACSInstrumento> ConfigACSPorInstrumento { get; set; } = new();
        
        // Calificaciones por instrumento (clave: InstrumentoId, valor: nota del instrumento)
        public Dictionary<int, NotaInstrumentoEstudiante> NotasPorInstrumento { get; set; } = new();
        
        // Nota final ponderada
        public decimal? NotaFinalPonderada { get; set; }
        public string EstadoGeneral { get; set; } = "PENDIENTE"; // PENDIENTE, PARCIAL, COMPLETO
        public bool Aprobado { get; set; }
        public decimal PorcentajeCompletado { get; set; }
    }

    /// <summary>
    /// Detalle de la nota de un instrumento para un estudiante
    /// </summary>
    public class NotaInstrumentoEstudiante
    {
        public int InstrumentoId { get; set; }
        public decimal? NotaInstrumento { get; set; } // Nota promedio de todas las r�bricas del instrumento
        public decimal NotaConPonderacion { get; set; } // Nota * porcentaje del instrumento
        public int EvaluacionesCompletas { get; set; }
        public int TotalEvaluacionesEsperadas { get; set; }
        public Dictionary<int, decimal> NotasPorRubrica { get; set; } = new(); // RubricaId -> Nota
        public bool EstaCompleto => EvaluacionesCompletas == TotalEvaluacionesEsperadas && TotalEvaluacionesEsperadas > 0;
    }

    /// <summary>
    /// Estad�sticas generales del cuaderno
    /// </summary>
    public class EstadisticasGenerales
    {
        public decimal PromedioGeneral { get; set; }
        public decimal NotaMaxima { get; set; }
        public decimal NotaMinima { get; set; }
        public int EstudiantesAprobados { get; set; }
        public int EstudiantesReprobados { get; set; }
        public decimal PorcentajeAprobacion { get; set; }
        
        // Distribuci�n de notas
        public Dictionary<string, int> DistribucionNotas { get; set; } = new();
        
        // Estad�sticas por instrumento
        public Dictionary<int, EstadisticasInstrumento> EstadisticasPorInstrumento { get; set; } = new();
    }

    /// <summary>
    /// Estad�sticas espec�ficas de un instrumento
    /// </summary>
    public class EstadisticasInstrumento
    {
        public int InstrumentoId { get; set; }
        public string NombreInstrumento { get; set; } = string.Empty;
        public decimal PromedioInstrumento { get; set; }
        public decimal NotaMaximaInstrumento { get; set; }
        public decimal NotaMinimaInstrumento { get; set; }
        public int EstudiantesConNotaCompleta { get; set; }
        public int TotalEstudiantes { get; set; }
        public decimal PorcentajeCompletado { get; set; }
    }

    /// <summary>
    /// ViewModel para filtros del cuaderno calificador
    /// </summary>
    public class FiltrosCuadernoCalificadorViewModel
    {
        public int? MateriaId { get; set; }
        public int? PeriodoAcademicoId { get; set; }
        public List<SelectListItem> MateriasDisponibles { get; set; } = new();
        public List<SelectListItem> PeriodosDisponibles { get; set; } = new();
        public bool MostrarSoloEstudiantesConEvaluaciones { get; set; } = false;
        public decimal NotaMinimaAprobacion { get; set; } = 70m;
    }
}