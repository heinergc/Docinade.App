namespace RubricasApp.Web.ViewModels.Conducta
{
    /// <summary>
    /// ViewModel para aplicar decisión profesional docente (Opción C)
    /// </summary>
    public class DecisionProfesionalViewModel
    {
        public int? IdDecision { get; set; }
        public int IdNotaConducta { get; set; }
        public int IdEstudiante { get; set; }
        public string NombreEstudiante { get; set; } = string.Empty;
        public string NumeroId { get; set; } = string.Empty;
        
        public int IdPeriodo { get; set; }
        public string NombrePeriodo { get; set; } = string.Empty;
        
        public decimal NotaActual { get; set; }
        public decimal NotaMinimaAprobacion { get; set; }
        
        public string JustificacionPedagogica { get; set; } = string.Empty;
        public string? ConsideracionesAdicionales { get; set; }
        
        public string DecisionTomada { get; set; } = string.Empty; // "Mantener Aplazado", "Asignar Aprobado"
        public decimal? NotaAjustada { get; set; }
        
        public string? NumeroActa { get; set; }
        public DateTime? FechaActa { get; set; } = DateTime.Now;
        public string? MiembrosComitePresentes { get; set; }
        public string? ObservacionesComite { get; set; }
        
        public string? TomaDecisionPorNombre { get; set; }
        public DateTime? FechaDecision { get; set; }
        
        public bool RegistradoEnExpediente { get; set; }
    }

    /// <summary>
    /// ViewModel para el listado de decisiones profesionales
    /// </summary>
    public class DecisionProfesionalListViewModel
    {
        public int IdDecision { get; set; }
        public string NombreEstudiante { get; set; } = string.Empty;
        public string NumeroId { get; set; } = string.Empty;
        public string Periodo { get; set; } = string.Empty;
        public string DecisionTomada { get; set; } = string.Empty;
        public decimal? NotaAjustada { get; set; }
        public string TomaDecisionPor { get; set; } = string.Empty;
        public DateTime FechaDecision { get; set; }
        public string? NumeroActa { get; set; }
        public bool RegistradoEnExpediente { get; set; }
    }
}
