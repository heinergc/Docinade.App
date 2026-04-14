namespace RubricasApp.Web.ViewModels.Reportes
{
    /// <summary>
    /// ViewModel principal para el reporte en árbol del sistema de evaluaciones
    /// </summary>
    public class ReporteArbolViewModel
    {
        public ResumenGeneral ResumenGeneral { get; set; } = new();
        public List<MateriaNode> Materias { get; set; } = new();
        public DateTime FechaGeneracion { get; set; } = DateTime.Now;
        public FiltrosReporte FiltrosAplicados { get; set; } = new();
        public int TotalNodos { get; set; }
    }

    /// <summary>
    /// Resumen estadístico general del sistema
    /// </summary>
    public class ResumenGeneral
    {
        public int TotalMaterias { get; set; }
        public int TotalPeriodos { get; set; }
        public int TotalInstrumentos { get; set; }
        public int TotalRubricas { get; set; }
        public int TotalEvaluaciones { get; set; }
        public int TotalEstudiantes { get; set; }
        public decimal PorcentajeCompletitudGeneral { get; set; }
        public decimal PromedioGeneralSistema { get; set; }
    }

    /// <summary>
    /// Nodo de materia en el árbol jerárquico
    /// </summary>
    public class MateriaNode
    {
        public int MateriaId { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public List<PeriodoNode> Periodos { get; set; } = new();
        
        // Estadísticas agregadas de la materia
        public int TotalPeriodos { get; set; }
        public int TotalEstudiantes { get; set; }
        public int TotalInstrumentos { get; set; }
        public int TotalRubricas { get; set; }
        public int TotalEvaluaciones { get; set; }
        public decimal PorcentajeCompletitud { get; set; }
        public decimal PromedioMateria { get; set; }
    }

    /// <summary>
    /// Nodo de período académico en el árbol jerárquico
    /// </summary>
    public class PeriodoNode
    {
        public int PeriodoId { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string PeriodoCompleto { get; set; } = string.Empty;
        public int CantidadEstudiantes { get; set; }
        public List<InstrumentoNode> Instrumentos { get; set; } = new();
        
        // Estadísticas del período
        public int TotalInstrumentos { get; set; }
        public int TotalRubricas { get; set; }
        public int TotalEvaluaciones { get; set; }
        public decimal PorcentajeCompletitud { get; set; }
        public decimal PromedioPeriodo { get; set; }
        public int EvaluacionesEsperadas { get; set; }
        public int EvaluacionesRealizadas { get; set; }
    }

    /// <summary>
    /// Nodo de instrumento en el árbol jerárquico
    /// </summary>
    public class InstrumentoNode
    {
        public int InstrumentoId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public int Orden { get; set; }
        public List<RubricaNode> Rubricas { get; set; } = new();
        
        // Estadísticas del instrumento
        public int TotalRubricas { get; set; }
        public int TotalEvaluaciones { get; set; }
        public decimal PonderacionTotal { get; set; }
        public decimal PromedioInstrumento { get; set; }
        public string TipoCuaderno { get; set; } = string.Empty; // DINAMICO, CUADERNO_CONFIGURADO
    }

    /// <summary>
    /// Nodo de rúbrica en el árbol jerárquico
    /// </summary>
    public class RubricaNode
    {
        public int RubricaId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Titulo { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public decimal Ponderacion { get; set; }
        public int CantidadEvaluaciones { get; set; }
        
        // Estadísticas de la rúbrica
        public int EvaluacionesFinalizadas { get; set; }
        public int EvaluacionesBorrador { get; set; }
        public decimal PorcentajeAvance { get; set; }
        public decimal? PromedioCalificacion { get; set; }
        public decimal? NotaMaxima { get; set; }
        public decimal? NotaMinima { get; set; }
        public int EstudiantesEvaluados { get; set; }
        public int EstudiantesPendientes { get; set; }
    }

    /// <summary>
    /// Filtros para el reporte
    /// </summary>
    public class FiltrosReporte
    {
        public int? MateriaId { get; set; }
        public int? PeriodoId { get; set; }
        public int? InstrumentoId { get; set; }
        public bool IncluirEstadisticas { get; set; } = true;
        public bool SoloActivos { get; set; } = true;
        public bool IncluirBorradores { get; set; } = false;
        public DateTime? FechaDesde { get; set; }
        public DateTime? FechaHasta { get; set; }
        public string? EstadoEvaluacion { get; set; } // FINALIZADA, BORRADOR, TODAS
    }

    /// <summary>
    /// Datos sin procesar de la consulta jerárquica
    /// </summary>
    public class DatoJerarquico
    {
        // Nivel Materia
        public int MateriaId { get; set; }
        public string CodigoMateria { get; set; } = string.Empty;
        public string NombreMateria { get; set; } = string.Empty;
        public string EstadoMateria { get; set; } = string.Empty;
        
        // Nivel Período
        public int? PeriodoId { get; set; }
        public string? CodigoPeriodo { get; set; }
        public string? NombrePeriodo { get; set; }
        public string? PeriodoCompleto { get; set; }
        public int CantidadEstudiantes { get; set; }
        
        // Nivel Instrumento
        public int? InstrumentoId { get; set; }
        public string? NombreInstrumento { get; set; }
        public string? DescripcionInstrumento { get; set; }
        public int OrdenInstrumento { get; set; }
        
        // Nivel Rúbrica
        public int? RubricaId { get; set; }
        public string? NombreRubrica { get; set; }
        public string? TituloRubrica { get; set; }
        public string? EstadoRubrica { get; set; }
        public decimal Ponderacion { get; set; }
        
        // Estadísticas de evaluaciones
        public int CantidadEvaluaciones { get; set; }
        public int EvaluacionesFinalizadas { get; set; }
        public int EvaluacionesBorrador { get; set; }
        public decimal? PromedioCalificacion { get; set; }
        public decimal? NotaMaxima { get; set; }
        public decimal? NotaMinima { get; set; }
        
        // Metadatos
        public string TipoCuaderno { get; set; } = string.Empty;
        
        // Totales del sistema (para resumen)
        public int TotalMaterias { get; set; }
        public int TotalPeriodos { get; set; }
        public int TotalInstrumentos { get; set; }
        public int TotalRubricas { get; set; }
        public int TotalEvaluacionesSistema { get; set; }
        public int TotalEstudiantesSistema { get; set; }
    }
}