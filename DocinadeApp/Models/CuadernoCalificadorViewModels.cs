using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace RubricasApp.Web.Models
{
    public class CuadernoCalificadorViewModel
    {
        // Propiedades básicas
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Materia { get; set; } = string.Empty;
        public string PeriodoAcademico { get; set; } = string.Empty;
        public DateTime? FechaCierre { get; set; }
        
        // Información del encabezado
        public int CuadernoId { get; set; }
        public string NombreCuaderno { get; set; } = string.Empty;
        public string NombreMateria { get; set; } = string.Empty;
        public string NombrePeriodoAcademico { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; }
        public string Estado { get; set; } = string.Empty;
        
        // Estadísticas para las tarjetas
        public int TotalInstrumentos { get; set; }
        public decimal PonderacionTotal { get; set; }
        public int EstudiantesEvaluados { get; set; }
        public decimal PromedioGeneral { get; set; }
        
        // Información de instrumentos y ponderaciones
        public List<InstrumentoCalificacionInfo> Instrumentos { get; set; } = new();
        
        // Calificaciones de estudiantes
        public List<CalificacionEstudiante> CalificacionesEstudiantes { get; set; } = new();
        
        // Estadísticas
        public EstadisticasCuaderno Estadisticas { get; set; } = new();
        
        // Validaciones
        public bool PonderacionesValidas => Instrumentos.Sum(i => i.PonderacionPorcentaje) == 100m;
        public bool TieneCalificacionesCompletas => CalificacionesEstudiantes.All(c => c.Estado == "COMPLETO");
    }

    public class InstrumentoCalificacionInfo
    {
        public int CuadernoInstrumentoId { get; set; }
        public int RubricaId { get; set; }
        public string NombreInstrumento { get; set; } = string.Empty;
        public string NombreRubrica { get; set; } = string.Empty;
        public string DescripcionRubrica { get; set; } = string.Empty;
        public decimal PonderacionPorcentaje { get; set; }
        public int Orden { get; set; }
        public bool EsObligatorio { get; set; }
        public int TotalEvaluaciones { get; set; }
        public int EvaluacionesCompletas { get; set; }
    }

    public class CalificacionEstudiante
    {
        public int EstudianteId { get; set; }
        public string NombreCompleto { get; set; } = string.Empty;
        public string NumeroId { get; set; } = string.Empty;
        public Dictionary<int, decimal?> CalificacionesPorInstrumento { get; set; } = new();
        public decimal? TotalPonderado { get; set; }
        public string Estado { get; set; } = "PENDIENTE"; // PENDIENTE, COMPLETO, PARCIAL
    }

    public class EstadisticasCuaderno
    {
        public int TotalEstudiantes { get; set; }
        public int EstudiantesEvaluados { get; set; }
        public int EstudiantesPendientes { get; set; }
        public decimal PromedioGeneral { get; set; }
        public decimal NotaMaxima { get; set; }
        public decimal NotaMinima { get; set; }
        public Dictionary<string, int> DistribucionNotas { get; set; } = new();
    }

    public class CrearCuadernoViewModel
    {
        [Required]
        [Display(Name = "Nombre del Cuaderno")]
        public string Nombre { get; set; } = string.Empty;
        
        [Required]
        [Display(Name = "Materia")]
        public int MateriaId { get; set; }
        
        [Required]
        [Display(Name = "Período Académico")]
        public int PeriodoAcademicoId { get; set; }
        
        public List<SelectListItem> Materias { get; set; } = new();
        public List<SelectListItem> PeriodosAcademicos { get; set; } = new();
    }

    public class ConfigurarInstrumentosViewModel
    {
    public int CuadernoId { get; set; }
    public string NombreCuaderno { get; set; } = string.Empty;
    public string NombreMateria { get; set; } = string.Empty;
    
    // Propiedades faltantes para las vistas
        public string Materia { get; set; } = string.Empty;
        public string PeriodoAcademico { get; set; } = string.Empty;
        public List<InstrumentoConfiguracion> InstrumentosSeleccionados { get; set; } = new();
        
        public List<InstrumentoConfiguracion> Instrumentos { get; set; } = new();        
        public List<SelectListItem> RubricasDisponibles { get; set; } = new();
    }

    public class InstrumentoConfiguracion
    {
        public int Id { get; set; }
        public int RubricaId { get; set; }
        public string NombreRubrica { get; set; } = string.Empty;
        public decimal PonderacionPorcentaje { get; set; }
        public int Orden { get; set; }
        public bool EsObligatorio { get; set; } = true;
        public bool Eliminar { get; set; } = false;
    }
}