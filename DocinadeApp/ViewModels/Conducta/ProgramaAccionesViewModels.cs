namespace RubricasApp.Web.ViewModels.Conducta
{
    /// <summary>
    /// ViewModel para crear/editar un Programa de Acciones
    /// </summary>
    public class ProgramaAccionesViewModel
    {
        public int? IdPrograma { get; set; }
        public int IdNotaConducta { get; set; }
        public int IdEstudiante { get; set; }
        public string NombreEstudiante { get; set; } = string.Empty;
        public int IdPeriodo { get; set; }
        public string NombrePeriodo { get; set; } = string.Empty;
        
        public string TituloPrograma { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string? ObjetivosEspecificos { get; set; }
        public string? ActividadesARealizar { get; set; }
        
        public DateTime FechaInicio { get; set; } = DateTime.Now;
        public DateTime FechaFinPrevista { get; set; } = DateTime.Now.AddMonths(1);
        public DateTime? FechaFinReal { get; set; }
        
        public string ResponsableSupervisionId { get; set; } = string.Empty;
        public string? ResponsableSupervisionNombre { get; set; }
        
        public string? ObservacionesSupervision { get; set; }
        
        public string Estado { get; set; } = "Pendiente";
        public string? ResultadoFinal { get; set; }
        
        public DateTime? FechaVerificacion { get; set; }
        public string? VerificadoPorNombre { get; set; }
        public string? ConclusionesComite { get; set; }
        
        public bool AprobarConducta { get; set; }
    }

    /// <summary>
    /// ViewModel para el listado de programas de acciones
    /// </summary>
    public class ProgramaAccionesListViewModel
    {
        public int IdPrograma { get; set; }
        public string TituloPrograma { get; set; } = string.Empty;
        public string NombreEstudiante { get; set; } = string.Empty;
        public string NumeroId { get; set; } = string.Empty;
        public string NumeroIdentificacion { get; set; } = string.Empty; // Alias
        public string Periodo { get; set; } = string.Empty;
        public string NombrePeriodo { get; set; } = string.Empty; // Alias
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFinPrevista { get; set; }
        public DateTime? FechaFinReal { get; set; }
        public string Estado { get; set; } = string.Empty;
        public string? ResultadoFinal { get; set; }
        public string ResponsableSupervision { get; set; } = string.Empty;
        public string NombreSupervisor { get; set; } = string.Empty; // Alias
        public bool AprobarConducta { get; set; }
        public DateTime? FechaVerificacion { get; set; }
    }

    /// <summary>
    /// ViewModel para verificar completación del programa
    /// </summary>
    public class VerificarProgramaAccionesViewModel
    {
        public int IdPrograma { get; set; }
        public string TituloPrograma { get; set; } = string.Empty;
        public string NombreEstudiante { get; set; } = string.Empty;
        
        public DateTime FechaFinReal { get; set; } = DateTime.Now;
        public string ResultadoFinal { get; set; } = string.Empty; // Satisfactorio, No Satisfactorio
        public string ConclusionesComite { get; set; } = string.Empty;
        public bool AprobarConducta { get; set; }
        
        public string? NumeroActa { get; set; }
        public DateTime? FechaActa { get; set; }
        public string? MiembrosComitePresentes { get; set; }
    }

    /// <summary>
    /// ViewModel para detalles del programa
    /// </summary>
    public class ProgramaAccionesDetalleViewModel
    {
        public int IdPrograma { get; set; }
        public string TituloPrograma { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string? ObjetivosEspecificos { get; set; }
        public string? Objetivos { get; set; } // Alias
        public string? ActividadesARealizar { get; set; }
        public string? ActividadesPropuestas { get; set; } // Alias
        public string? MetasEsperadas { get; set; }
        public string? RecursosNecesarios { get; set; }
        public string? CompromisosEstudiante { get; set; }
        public string? CompromisosFamilia { get; set; }
        public string? CriteriosEvaluacion { get; set; }
        
        public int IdEstudiante { get; set; }
        public int IdPeriodo { get; set; }
        public string NombreEstudiante { get; set; } = string.Empty;
        public string NumeroIdentificacion { get; set; } = string.Empty;
        public string NombreGrupo { get; set; } = string.Empty;
        public string NombrePeriodo { get; set; } = string.Empty;
        public decimal NotaConducta { get; set; }
        
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFinPrevista { get; set; }
        public DateTime? FechaFinReal { get; set; }
        
        public string NombreResponsableSupervision { get; set; } = string.Empty;
        public string? NombreSupervisor { get; set; } // Alias
        public string? ObservacionesSupervision { get; set; }
        
        public string Estado { get; set; } = string.Empty;
        public string? Resultado { get; set; } // Alias de ResultadoFinal
        public DateTime FechaCreacion { get; set; }
        
        public bool Verificado { get; set; }
        public DateTime? FechaVerificacion { get; set; }
        public string? NombreVerificador { get; set; }
        public string? ResultadoFinal { get; set; }
        public string? ConclusionesComite { get; set; }
        public string? ObservacionesVerificacion { get; set; }
        public bool AprobarConducta { get; set; }
    }

    /// <summary>
    /// ViewModel para verificar programa
    /// </summary>
    public class VerificarProgramaViewModel
    {
        public int IdPrograma { get; set; }
        public string TituloPrograma { get; set; } = string.Empty;
        public string NombreEstudiante { get; set; } = string.Empty;
        public string NumeroIdentificacion { get; set; } = string.Empty;
        public string NombreGrupo { get; set; } = string.Empty;
        public string NombrePeriodo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string? NombreSupervisor { get; set; }
        public string? Objetivos { get; set; }
        public string? CriteriosEvaluacion { get; set; }
        
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFinPrevista { get; set; }
        
        // Campos de verificación
        public DateTime FechaFinReal { get; set; } = DateTime.Now;
        public DateTime FechaVerificacion { get; set; } = DateTime.Now;
        public string ResultadoFinal { get; set; } = string.Empty;
        public string ConclusionesComite { get; set; } = string.Empty;
        public string? ObservacionesVerificacion { get; set; }
        public bool AprobarConducta { get; set; }
    }
}

