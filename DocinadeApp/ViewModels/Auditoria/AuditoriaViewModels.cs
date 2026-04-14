using DocinadeApp.Models;

namespace DocinadeApp.ViewModels.Auditoria
{
    /// <summary>
    /// ViewModel para mostrar una operación de auditoría
    /// </summary>
    public class AuditoriaViewModel
    {
        public int Id { get; set; }
        public string TipoOperacion { get; set; } = string.Empty;
        public string TablaAfectada { get; set; } = string.Empty;
        public int RegistroId { get; set; }
        public string? Motivo { get; set; }
        public string UsuarioId { get; set; } = string.Empty;
        public string? UsuarioNombre { get; set; }
        public DateTime FechaOperacion { get; set; }
        public string? DireccionIP { get; set; }
        public string? UserAgent { get; set; }
        public string? DatosAnteriores { get; set; }
        public string? DatosNuevos { get; set; }
        public bool Exitosa { get; set; }
        public string? MensajeError { get; set; }
    }

    /// <summary>
    /// ViewModel para la página principal de auditoría
    /// </summary>
    public class AuditoriaIndexViewModel
    {
        public List<AuditoriaViewModel> Operaciones { get; set; } = new List<AuditoriaViewModel>();
        public int? FiltroTipoOperacion { get; set; }
        public string? FiltroUsuario { get; set; }
        public DateTime? FiltroFechaDesde { get; set; }
        public DateTime? FiltroFechaHasta { get; set; }
        public int PaginaActual { get; set; } = 1;
        public int TamanioPagina { get; set; } = 20;
        public string? TituloEspecial { get; set; }
    }

    /// <summary>
    /// ViewModel para mostrar el historial de auditoría
    /// </summary>
    public class HistorialAuditoriaViewModel
    {
        public string TablaAfectada { get; set; } = string.Empty;
        public int RegistroId { get; set; }
        public string TituloRegistro { get; set; } = string.Empty;
        public List<AuditoriaOperacionViewModel> Operaciones { get; set; } = new List<AuditoriaOperacionViewModel>();
        public int TotalOperaciones { get; set; }
        public DateTime? FechaDesde { get; set; }
        public DateTime? FechaHasta { get; set; }
    }

    /// <summary>
    /// ViewModel para mostrar una operación de auditoría individual
    /// </summary>
    public class AuditoriaOperacionViewModel
    {
        public int Id { get; set; }
        public string TipoOperacion { get; set; } = string.Empty;
        public string TipoOperacionDisplay { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string? Motivo { get; set; }
        public DateTime FechaOperacion { get; set; }
        public string UsuarioNombre { get; set; } = string.Empty;
        public string UsuarioEmail { get; set; } = string.Empty;
        public bool OperacionExitosa { get; set; }
        public string? MensajeError { get; set; }
        public string? DireccionIP { get; set; }
        public string? UserAgent { get; set; }
        public bool TieneDatosAnteriores { get; set; }
        public bool TieneDatosNuevos { get; set; }
        public string? DatosAnteriores { get; set; }
        public string? DatosNuevos { get; set; }
        public string IconoOperacion { get; set; } = string.Empty;
        public string ColorOperacion { get; set; } = string.Empty;
    }

    /// <summary>
    /// ViewModel para filtros de auditoría
    /// </summary>
    public class FiltrosAuditoriaViewModel
    {
        public string? TipoOperacion { get; set; }
        public DateTime? FechaDesde { get; set; }
        public DateTime? FechaHasta { get; set; }
        public string? UsuarioId { get; set; }
        public bool? SoloExitosas { get; set; }
        public string? TablaAfectada { get; set; }
        public int? RegistroId { get; set; }
    }
}
