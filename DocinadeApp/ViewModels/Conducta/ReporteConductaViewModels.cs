namespace RubricasApp.Web.ViewModels.Conducta
{
    /// <summary>
    /// ViewModel para reportes de conducta
    /// </summary>
    public class ReporteConductaViewModel
    {
        public string TituloReporte { get; set; } = string.Empty;
        public string? Periodo { get; set; }
        public string? Grupo { get; set; }
        public string? Docente { get; set; }
        public DateTime FechaGeneracion { get; set; } = DateTime.Now;
    }

    /// <summary>
    /// Reporte: Historial de boletas por estudiante
    /// </summary>
    public class HistorialBoletasEstudianteViewModel : ReporteConductaViewModel
    {
        public string NombreEstudiante { get; set; } = string.Empty;
        public string NumeroId { get; set; } = string.Empty;
        public List<BoletaConductaListViewModel> Boletas { get; set; } = new();
        public decimal NotaFinalConducta { get; set; }
        public string EstadoConducta { get; set; } = string.Empty;
    }

    /// <summary>
    /// Reporte: Registro de faltas por grupo
    /// </summary>
    public class RegistroFaltasGrupoViewModel : ReporteConductaViewModel
    {
        public string NombreGrupo { get; set; } = string.Empty;
        public int TotalEstudiantes { get; set; }
        public int TotalBoletas { get; set; }
        public Dictionary<string, int> FaltasPorTipo { get; set; } = new();
        public List<BoletaConductaListViewModel> Boletas { get; set; } = new();
    }

    /// <summary>
    /// Reporte: Estudiantes en riesgo y aplazados
    /// </summary>
    public class ReporteEstudiantesRiesgoViewModel : ReporteConductaViewModel
    {
        public List<EstudianteRiesgoItem> EstudiantesRiesgo { get; set; } = new();
        public List<EstudianteRiesgoItem> EstudiantesAplazados { get; set; } = new();
        public int TotalRiesgo { get; set; }
        public int TotalAplazados { get; set; }
    }

    /// <summary>
    /// Reporte: Seguimiento de programas de acciones
    /// </summary>
    public class SeguimientoProgramasViewModel : ReporteConductaViewModel
    {
        public List<ProgramaAccionesListViewModel> ProgramasPendientes { get; set; } = new();
        public List<ProgramaAccionesListViewModel> ProgramasEnProceso { get; set; } = new();
        public List<ProgramaAccionesListViewModel> ProgramasCompletados { get; set; } = new();
        public int TotalProgramas { get; set; }
        public int ProgramasCompletadosSatisfactoriamente { get; set; }
    }

    /// <summary>
    /// Dashboard de conducta - resumen general
    /// </summary>
    public class DashboardConductaViewModel
    {
        public int IdPeriodo { get; set; }
        public int? IdGrupo { get; set; }
        public string Periodo { get; set; } = string.Empty;
        
        // Estadísticas generales
        public int TotalEstudiantes { get; set; }
        public int TotalBoletas { get; set; }
        public int TotalBoletasEmitidas { get; set; } // Alias
        public int EstudiantesConBoletas { get; set; }
        public int EstudiantesSinBoletas { get; set; }
        public decimal TotalPuntosRebajados { get; set; }
        public decimal PromedioGeneralConducta { get; set; }
        
        // Distribución por estado
        public int EstudiantesAprobados { get; set; }
        public decimal PorcentajeAprobados { get; set; }
        public int EstudiantesEnRiesgo { get; set; }
        public int EstudiantesRiesgo { get; set; } // Alias
        public decimal PorcentajeEnRiesgo { get; set; }
        public int EstudiantesAplazados { get; set; }
        public decimal PorcentajeAplazados { get; set; }
        
        // Distribución por tipo de falta
        public Dictionary<string, int> BoletasPorTipoFalta { get; set; } = new();
        
        // Programas y decisiones
        public int ProgramasAccionesActivos { get; set; }
        public int DecisionesProfesionalesAplicadas { get; set; }
        
        // Estudiantes que requieren atención
        public List<EstudianteAtencionViewModel> EstudiantesRequierenAtencion { get; set; } = new();
        
        // Últimas boletas registradas
        public List<BoletaConductaListViewModel> UltimasBoletas { get; set; } = new();
    }
}

