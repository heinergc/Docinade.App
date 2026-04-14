namespace DocinadeApp.ViewModels.Conducta
{
    /// <summary>
    /// ViewModel para registrar una nueva boleta de conducta
    /// </summary>
    public class RegistrarBoletaConductaViewModel
    {
        public int IdEstudiante { get; set; }
        public string NombreEstudiante { get; set; } = string.Empty;
        public string NumeroId { get; set; } = string.Empty;
        
        public int IdTipoFalta { get; set; }
        public string NombreTipoFalta { get; set; } = string.Empty;
        
        public int IdPeriodo { get; set; }
        public string NombrePeriodo { get; set; } = string.Empty;
        
        public int RebajoAplicado { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        
        // Campos adicionales del modelo
        public string? LugarIncidente { get; set; }
        public string? Testigos { get; set; }
        public string? AccionInmediata { get; set; }
        public string? Observaciones { get; set; }
        
        // Para archivo de evidencia
        public IFormFile? ArchivoEvidencia { get; set; }
        
        // Información del tipo de falta seleccionado
        public string? DefinicionFalta { get; set; }
        public string? EjemplosFalta { get; set; }
        public string? AccionCorrectiva { get; set; }
        public int RebajoMinimo { get; set; }
        public int RebajoMaximo { get; set; }
    }

    /// <summary>
    /// ViewModel para mostrar el listado de boletas
    /// </summary>
    public class BoletaConductaListViewModel
    {
        public int IdBoleta { get; set; }
        public int IdEstudiante { get; set; }
        public string NombreEstudiante { get; set; } = string.Empty;
        public string NumeroId { get; set; } = string.Empty;
        public string TipoFalta { get; set; } = string.Empty;
        public int RebajoAplicado { get; set; }
        public int Rebajo { get; set; } // Alias
        public string Descripcion { get; set; } = string.Empty;
        public string DocenteEmisor { get; set; } = string.Empty;
        public string EmitidaPor { get; set; } = string.Empty; // Alias
        public DateTime FechaEmision { get; set; }
        public DateTime? FechaNotificacion { get; set; }
        public string Estado { get; set; } = string.Empty;
        public bool NotificacionEnviada { get; set; }
        public string? ProfesorGuia { get; set; }
    }

    /// <summary>
    /// ViewModel para el detalle de una boleta
    /// </summary>
    public class BoletaConductaDetalleViewModel
    {
        public int IdBoleta { get; set; }
        public int IdEstudiante { get; set; }
        public string NombreEstudiante { get; set; } = string.Empty;
        public string NumeroId { get; set; } = string.Empty;
        public string NumeroIdEstudiante { get; set; } = string.Empty; // Alias para vistas
        public string Grupo { get; set; } = string.Empty;
        
        public string TipoFalta { get; set; } = string.Empty;
        public string DefinicionFalta { get; set; } = string.Empty;
        public string? EjemplosFalta { get; set; }
        public string? AccionCorrectiva { get; set; }
        public int RebajoMinimo { get; set; }
        public int RebajoMaximo { get; set; }
        public int RebajoAplicado { get; set; }
        
        public string Descripcion { get; set; } = string.Empty;
        public string? LugarIncidente { get; set; }
        public string? Testigos { get; set; }
        public string? AccionInmediata { get; set; }
        public string? Observaciones { get; set; }
        public string? RutaEvidencia { get; set; }
        public string? ArchivoEvidencia { get; set; } // Alias para vistas
        
        public string DocenteEmisor { get; set; } = string.Empty;
        public string EmailDocenteEmisor { get; set; } = string.Empty;
        public DateTime FechaEmision { get; set; }
        
        public string? ProfesorGuia { get; set; }
        public string? EmailProfesorGuia { get; set; }
        public bool NotificacionEnviada { get; set; }
        public DateTime? FechaNotificacion { get; set; }
        public string? ObservacionesProfesorGuia { get; set; }
        
        public string Estado { get; set; } = string.Empty;
        public string? MotivoAnulacion { get; set; }
        public DateTime? FechaAnulacion { get; set; }
        public string? AnuladaPor { get; set; }
        
        public string Periodo { get; set; } = string.Empty;
    }
}
