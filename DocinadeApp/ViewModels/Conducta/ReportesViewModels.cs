using System.ComponentModel.DataAnnotations;

namespace DocinadeApp.ViewModels.Conducta
{
    // ========================
    // HISTORIAL DE CONDUCTA POR ESTUDIANTE
    // ========================

    public class HistorialConductaEstudianteViewModel
    {
        public int IdEstudiante { get; set; }
        public string NombreEstudiante { get; set; } = string.Empty;
        public string NumeroIdentificacion { get; set; } = string.Empty;
        public string GrupoActual { get; set; } = string.Empty;
        public string ProfesorGuiaActual { get; set; } = string.Empty;
        public int TotalBoletasHistorico { get; set; }
        public decimal TotalRebajosHistorico { get; set; }
        public decimal PromedioHistorico { get; set; }
        public List<NotaConductaPorPeriodoViewModel> NotasPorPeriodo { get; set; } = new();
    }

    public class NotaConductaPorPeriodoViewModel
    {
        public int IdPeriodo { get; set; }
        public string NombrePeriodo { get; set; } = string.Empty;
        public decimal NotaFinal { get; set; }
        public decimal TotalRebajos { get; set; }
        public int CantidadBoletas { get; set; }
        public string Estado { get; set; } = string.Empty;
        public bool TienePrograma { get; set; }
        public string? EstadoPrograma { get; set; }
        public bool TieneDecision { get; set; }
        public string? DecisionAplicada { get; set; }
        public List<BoletaResumenViewModel> Boletas { get; set; } = new();
    }

    public class BoletaResumenViewModel
    {
        public int IdBoleta { get; set; }
        public DateTime FechaEmision { get; set; }
        public DateTime FechaIncidente { get; set; }
        public string TipoFalta { get; set; } = string.Empty;
        public decimal Rebajo { get; set; }
        public int RebajoAplicado { get; set; }
        public string EmitidaPor { get; set; } = string.Empty;
        public string ReportadoPor { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public string DescripcionIncidente { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
    }

    // ========================
    // REPORTE GENERAL DE CONDUCTA
    // ========================

    public class ReporteGeneralConductaViewModel
    {
        public int IdPeriodo { get; set; }
        public string NombrePeriodo { get; set; } = string.Empty;
        public int? IdNivel { get; set; }
        public string? NombreNivel { get; set; }

        // Estadísticas globales
        public int TotalEstudiantes { get; set; }
        public int EstudiantesAprobados { get; set; }
        public int EstudiantesEnRiesgo { get; set; }
        public int EstudiantesAplazados { get; set; }
        public decimal PorcentajeAprobados { get; set; }
        public decimal PorcentajeEnRiesgo { get; set; }
        public decimal PorcentajeAplazados { get; set; }
        public int TotalBoletas { get; set; }
        public decimal PromedioGeneral { get; set; }

        // Detalles
        public List<EstadisticasGrupoConductaViewModel> EstadisticasPorGrupo { get; set; } = new();
        public List<BoletasPorTipoViewModel> BoletasPorTipo { get; set; } = new();
        public List<EstudianteTopViewModel> Top10MejorConducta { get; set; } = new();
        public List<EstudianteAtencionViewModel> EstudiantesAtencionUrgente { get; set; } = new();
    }

    public class EstadisticasGrupoConductaViewModel
    {
        public string NombreGrupo { get; set; } = string.Empty;
        public string ProfesorGuia { get; set; } = string.Empty;
        public int TotalEstudiantes { get; set; }
        public int Aprobados { get; set; }
        public int EnRiesgo { get; set; }
        public int Aplazados { get; set; }
        public decimal PromedioGrupo { get; set; }
        public int TotalBoletas { get; set; }
    }

    public class BoletasPorTipoViewModel
    {
        public string TipoFalta { get; set; } = string.Empty;
        public int Cantidad { get; set; }
    }

    public class EstudianteTopViewModel
    {
        public string NombreEstudiante { get; set; } = string.Empty;
        public string Grupo { get; set; } = string.Empty;
        public decimal Nota { get; set; }
    }

    public class EstudianteAtencionViewModel
    {
        public int IdEstudiante { get; set; }
        public string NombreEstudiante { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty; // Alias
        public string NumeroIdentificacion { get; set; } = string.Empty;
        public string Grupo { get; set; } = string.Empty;
        public string NombreGrupo { get; set; } = string.Empty; // Alias
        public decimal Nota { get; set; }
        public decimal NotaFinal { get; set; } // Alias
        public string Estado { get; set; } = string.Empty;
        public int CantidadBoletas { get; set; }
        public bool TienePrograma { get; set; }
        public bool TieneDecision { get; set; }
    }

    // ========================
    // REPORTES PDF - PROGRAMAS Y DECISIONES
    // ========================

    /// <summary>
    /// ViewModel para reporte individual de Programa de Acciones
    /// </summary>
    public class ReporteProgramaAccionesViewModel
    {
        public int IdPrograma { get; set; }
        public string NumeroPrograma { get; set; } = string.Empty;
        public DateTime FechaElaboracion { get; set; }
        
        // Datos del estudiante
        public string NombreEstudiante { get; set; } = string.Empty;
        public string CarnetEstudiante { get; set; } = string.Empty;
        public string GradoSeccion { get; set; } = string.Empty;
        public string PeriodoAcademico { get; set; } = string.Empty;
        
        // Contenido del programa
        public string DescripcionFaltas { get; set; } = string.Empty;
        public string AccionesFormativas { get; set; } = string.Empty;
        public string CompromisosEstudiante { get; set; } = string.Empty;
        public string ApoyoPadres { get; set; } = string.Empty;
        public string SeguimientoEvaluacion { get; set; } = string.Empty;
        public string? Observaciones { get; set; }
        
        // Autorización
        public string AutorizadoPor { get; set; } = string.Empty;
        public DateTime? FechaAutorizacion { get; set; }
        public string Estado { get; set; } = string.Empty;
        
        // Boletas asociadas
        public List<BoletaResumenViewModel> Boletas { get; set; } = new();
        
        // Datos institucionales
        public string NombreInstitucion { get; set; } = "Colegio Técnico Profesional de General Viejo";
        public DateTime FechaGeneracion { get; set; } = DateTime.Now;
    }

    /// <summary>
    /// ViewModel para reporte individual de Decisión Profesional
    /// </summary>
    public class ReporteDecisionProfesionalViewModel
    {
        public int IdDecision { get; set; }
        public string NumeroActa { get; set; } = string.Empty;
        public DateTime FechaReunion { get; set; }
        
        // Datos del estudiante
        public string NombreEstudiante { get; set; } = string.Empty;
        public string CarnetEstudiante { get; set; } = string.Empty;
        public string GradoSeccion { get; set; } = string.Empty;
        public string PeriodoAcademico { get; set; } = string.Empty;
        
        // Contenido de la decisión
        public string AntecedentesExpuestos { get; set; } = string.Empty;
        public string? VersionEstudiante { get; set; }
        public string? VersionPadres { get; set; }
        public string AnalisisComite { get; set; } = string.Empty;
        public string DecisionTomada { get; set; } = string.Empty;
        public string? AccionesSeguimiento { get; set; }
        public string? ObservacionesComite { get; set; }
        
        // Decisión
        public string TomaDecisionPor { get; set; } = string.Empty;
        public DateTime FechaDecision { get; set; }
        public string Estado { get; set; } = string.Empty;
        
        // Boletas asociadas
        public List<BoletaResumenViewModel> Boletas { get; set; } = new();
        
        // Datos institucionales
        public string NombreInstitucion { get; set; } = "Colegio Técnico Profesional de General Viejo";
        public DateTime FechaGeneracion { get; set; } = DateTime.Now;
    }

    /// <summary>
    /// ViewModel para reporte masivo de Programas de Acciones
    /// </summary>
    public class ReporteMasivoProgramasViewModel
    {
        public DateTime FechaGeneracion { get; set; }
        public string PeriodoAcademico { get; set; } = string.Empty;
        public string EstadoFiltro { get; set; } = string.Empty;
        public int TotalRegistros { get; set; }
        
        public List<ProgramaResumenViewModel> Programas { get; set; } = new();
        
        public string NombreInstitucion { get; set; } = "Colegio Técnico Profesional de General Viejo";
    }

    /// <summary>
    /// ViewModel para reporte masivo de Decisiones Profesionales
    /// </summary>
    public class ReporteMasivoDecisionesViewModel
    {
        public DateTime FechaGeneracion { get; set; }
        public string PeriodoAcademico { get; set; } = string.Empty;
        public string EstadoFiltro { get; set; } = string.Empty;
        public int TotalRegistros { get; set; }
        
        public List<DecisionResumenViewModel> Decisiones { get; set; } = new();
        
        public string NombreInstitucion { get; set; } = "Colegio Técnico Profesional de General Viejo";
    }

    /// <summary>
    /// ViewModel de resumen para Programa de Acciones en reportes masivos
    /// </summary>
    public class ProgramaResumenViewModel
    {
        public string NumeroPrograma { get; set; } = string.Empty;
        public DateTime FechaElaboracion { get; set; }
        public string NombreEstudiante { get; set; } = string.Empty;
        public string Carnet { get; set; } = string.Empty;
        public string GradoSeccion { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public string AutorizadoPor { get; set; } = string.Empty;
        public DateTime? FechaAutorizacion { get; set; }
    }

    /// <summary>
    /// ViewModel de resumen para Decisión Profesional en reportes masivos
    /// </summary>
    public class DecisionResumenViewModel
    {
        public string NumeroActa { get; set; } = string.Empty;
        public DateTime FechaReunion { get; set; }
        public string NombreEstudiante { get; set; } = string.Empty;
        public string Carnet { get; set; } = string.Empty;
        public string GradoSeccion { get; set; } = string.Empty;
        public string DecisionTomada { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public string TomaDecisionPor { get; set; } = string.Empty;
        public DateTime FechaDecision { get; set; }
    }
}
