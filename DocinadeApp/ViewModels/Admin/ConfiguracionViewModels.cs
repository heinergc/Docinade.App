using System.ComponentModel.DataAnnotations;
using RubricasApp.Web.Models;

namespace RubricasApp.Web.ViewModels.Admin
{
    /// <summary>
    /// ViewModel base para las páginas de administración
    /// </summary>
    public class AdminBase2ViewModel
    {
        public string Titulo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public DateTime FechaActual { get; set; } = DateTime.Now;
        public string UsuarioActual { get; set; } = string.Empty;
    }

    /// <summary>
    /// ViewModel principal para la página de configuración
    /// </summary>
    public class ConfiguracionIndexViewModel : AdminBaseViewModel
    {
        public SystemInfoViewModel SystemInfo { get; set; } = new();
        public GeneralSettingsViewModel GeneralSettings { get; set; } = new();
        public SystemHealthViewModel SystemHealth { get; set; } = new();
        public QuickStatsViewModel QuickStats { get; set; } = new();
        
        // Configuraciones de registro de usuarios
        public List<ConfiguracionSistema> Configuraciones { get; set; } = new();
        public ModoRegistroUsuarios ModoRegistroActual { get; set; }
        public string MensajeRegistroCerrado { get; set; } = string.Empty;
        
        // Configuración de empadronamiento público
        public bool EmpadronamientoPublicoHabilitado { get; set; }
    }

    /// <summary>
    /// Información del sistema
    /// </summary>
    public class SystemInfoViewModel
    {
        public string NombreAplicacion { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public string EntornoEjecucion { get; set; } = string.Empty;
        public DateTime FechaInicioSistema { get; set; }
        public DateTime UltimaActualizacion { get; set; }
        public string VersionFramework { get; set; } = string.Empty;
        public string ServidorBase { get; set; } = string.Empty;
        public string BaseDatos { get; set; } = string.Empty;
    }

    /// <summary>
    /// Configuraciones generales del sistema
    /// </summary>
    public class GeneralSettingsViewModel : AdminBaseViewModel
    {
        // Información del Sistema
        [Required(ErrorMessage = "El nombre de la institución es requerido")]
        [StringLength(200, ErrorMessage = "El nombre no puede exceder 200 caracteres")]
        public string NombreInstitucion { get; set; } = string.Empty;

        [Url(ErrorMessage = "Debe ser una URL válida")]
        public string SitioWeb { get; set; } = string.Empty;

        [Url(ErrorMessage = "Debe ser una URL válida")]
        public string LogoInstitucion { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "La descripción no puede exceder 1000 caracteres")]
        public string Descripcion { get; set; } = string.Empty;

        // Información de Contacto
        [Required(ErrorMessage = "El email de soporte es requerido")]
        [EmailAddress(ErrorMessage = "Debe ser un email válido")]
        public string EmailSoporte { get; set; } = string.Empty;

        public string TelefonoSoporte { get; set; } = string.Empty;
        public string DireccionFisica { get; set; } = string.Empty;
        public string HorarioAtencion { get; set; } = string.Empty;

        // Configuración de Sesiones
        [Required]
        [Range(5, 480, ErrorMessage = "Debe estar entre 5 y 480 minutos")]
        public int TiempoExpiracionSesion { get; set; } = 30;

        public bool RecordarSesion { get; set; } = true;

        [Required]
        [Range(3, 10, ErrorMessage = "Debe estar entre 3 y 10 intentos")]
        public int MaximoIntentosFallidos { get; set; } = 5;

        // Configuración de Archivos
        [Required]
        [Range(1, 100, ErrorMessage = "Debe estar entre 1 y 100 MB")]
        public int TamanoMaximoArchivo { get; set; } = 10;

        [Required]
        public string TiposArchivosPermitidos { get; set; } = ".jpg,.png,.pdf,.docx,.xlsx";

        [Required]
        public string RutaAlmacenamiento { get; set; } = "~/uploads/";

        // Configuración de Notificaciones
        public bool NotificacionesEmail { get; set; } = true;
        public bool NotificacionesSistema { get; set; } = true;
        public string FrecuenciaNotificaciones { get; set; } = "Diaria";

        // Configuración de Idioma y Formato
        [Required]
        public string IdiomaDefecto { get; set; } = "es-CR";

        [Required]
        public string ZonaHoraria { get; set; } = "America/Costa_Rica";

        [Required]
        public string FormatoFecha { get; set; } = "dd/MM/yyyy";

        [Required]
        public string FormatoHora { get; set; } = "HH:mm";

        // Propiedades heredadas para compatibilidad
        public bool MantenimientoActivo { get; set; }
        public bool RegistroUsuariosAbierto { get; set; }
        public bool NotificacionesActivas { get; set; }
        public bool BackupAutomaticoActivo { get; set; }
    }

    /// <summary>
    /// Estado de salud del sistema
    /// </summary>
    public class SystemHealthViewModel
    {
        public string EstadoGeneral { get; set; } = string.Empty;
        public bool BaseDatosOnline { get; set; }
        public string EspacioDiscoDisponible { get; set; } = string.Empty;
        public string MemoriaDisponible { get; set; } = string.Empty;
        public DateTime? UltimBackup { get; set; }
    }

    /// <summary>
    /// Estadísticas rápidas del sistema
    /// </summary>
    public class QuickStatsViewModel
    {
        public int TotalUsuarios { get; set; }
        public int UsuariosActivos { get; set; }
        public int TotalRoles { get; set; }
        public int TotalRubricas { get; set; }
        public int EvaluacionesHoy { get; set; }
    }

    /// <summary>
    /// ViewModel para configuración de seguridad avanzada
    /// </summary>
    public class SecurityViewModel : AdminBaseViewModel
    {
        // Políticas de Contraseñas
        [Required]
        [Range(6, 32, ErrorMessage = "La longitud mínima debe estar entre 6 y 32 caracteres")]
        public int LongitudMinimaPassword { get; set; } = 8;

        public bool RequiereLetrasMayusculas { get; set; } = true;
        public bool RequiereLetrasMinusculas { get; set; } = true;
        public bool RequiereNumeros { get; set; } = true;
        public bool RequiereCaracteresEspeciales { get; set; }

        [Range(0, 365, ErrorMessage = "Los días de expiración deben estar entre 0 y 365")]
        public int DiasExpiracionPassword { get; set; } = 90;

        [Range(0, 12, ErrorMessage = "El historial de contraseñas debe estar entre 0 y 12")]
        public int HistorialPasswords { get; set; } = 5;

        // Bloqueo de Cuentas
        public bool BloqueoAutomatico { get; set; } = true;

        [Range(3, 10, ErrorMessage = "Los máximos intentos deben estar entre 3 y 10")]
        public int MaximosIntentos { get; set; } = 5;

        [Range(5, 1440, ErrorMessage = "El tiempo de bloqueo debe estar entre 5 y 1440 minutos")]
        public int TiempoBloqueo { get; set; } = 30;

        public bool NotificarBloqueos { get; set; } = true;
        public bool BloqueoTemporal { get; set; } = true;

        // Autenticación de Dos Factores
        public bool Habilitar2FA { get; set; }
        public bool Requerir2FAAdmins { get; set; } = true;

        [Range(1, 15, ErrorMessage = "El tiempo de validez del código debe estar entre 1 y 15 minutos")]
        public int TiempoValidezCodigo { get; set; } = 5;

        // Configuración de Sesiones
        [Range(5, 480, ErrorMessage = "El tiempo de inactividad debe estar entre 5 y 480 minutos")]
        public int TiempoInactividadSesion { get; set; } = 30;

        [Range(1, 10, ErrorMessage = "El máximo de sesiones simultáneas debe estar entre 1 y 10")]
        public int MaximoSesionesSimultaneas { get; set; } = 3;

        public bool CerrarSesionesInactivas { get; set; } = true;
        public bool LoguearCambiosSesion { get; set; } = true;

        // Auditoría y Logs
        public bool AuditoriaHabilitada { get; set; } = true;
        public bool LoguearAccesos { get; set; } = true;
        public bool LoguearCambiosDatos { get; set; } = true;

        [Range(30, 3650, ErrorMessage = "La retención de logs debe estar entre 30 y 3650 días")]
        public int RetencionLogs { get; set; } = 365;

        public bool NotificarEventosSeguridad { get; set; } = true;

        // Configuraciones Adicionales
        public bool ValidarIPAcceso { get; set; }
        public bool ForzarHTTPS { get; set; } = true;
        public bool ValidarDispositivo { get; set; }
        public bool CaptchaLogin { get; set; }
        public string NivelSeguridadMinimo { get; set; } = "Medio";
    }

    /// <summary>
    /// ViewModel para logs del sistema
    /// </summary>
    public class LogsViewModel : AdminBase2ViewModel
    {
        public List<LogFileViewModel> LogFiles { get; set; } = new();
        public List<LogEntryViewModel> RecentLogs { get; set; } = new();
        public List<LogEntryViewModel> LogEntries { get; set; } = new();
        public string SelectedLogFile { get; set; } = string.Empty;
        public string LogLevel { get; set; } = string.Empty;
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        
        // Filtros
        public string FiltroNivel { get; set; } = string.Empty;
        public DateTime? FechaDesde { get; set; }
        public DateTime? FechaHasta { get; set; }
        public string FiltroUsuario { get; set; } = string.Empty;
        public string FiltroBusqueda { get; set; } = string.Empty;
        
        // Contadores
        public LogCountsViewModel LogCounts { get; set; } = new();
    }

    /// <summary>
    /// Contadores de logs por nivel
    /// </summary>
    public class LogCountsViewModel
    {
        public int Information { get; set; }
        public int Warning { get; set; }
        public int Error { get; set; }
        public int Critical { get; set; }
        public int Debug { get; set; }
        public int Audit { get; set; }
    }

    /// <summary>
    /// Archivo de log
    /// </summary>
    public class LogFileViewModel
    {
        public string Nombre { get; set; } = string.Empty;
        public string Tamaño { get; set; } = string.Empty;
        public DateTime FechaModificacion { get; set; }
        public string Ruta { get; set; } = string.Empty;
    }

    /// <summary>
    /// Entrada de log
    /// </summary>
    public class LogEntryViewModel
    {
        public string Id { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public string Level { get; set; } = string.Empty;
        public string Mensaje { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Exception { get; set; } = string.Empty;
        public string Usuario { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public string DetalleError { get; set; } = string.Empty;
        public string StackTrace { get; set; } = string.Empty;
        public string DireccionIP { get; set; } = string.Empty;
        public string UserAgent { get; set; } = string.Empty;
    }

    /// <summary>
    /// ViewModel para health checks
    /// </summary>
    public class HealthCheckViewModel
    {
        public string Name { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public TimeSpan? Duration { get; set; }
    }

    /// <summary>
    /// ViewModel para backup y restauración
    /// </summary>
    public class BackupViewModel : AdminBaseViewModel
    {
        public List<BackupFileViewModel> BackupFiles { get; set; } = new();
        public bool BackupAutomaticoActivo { get; set; }
        public int FrecuenciaBackupHoras { get; set; }
        public DateTime? ProximoBackup { get; set; }
        public long EspacioDisponible { get; set; }
        public long EspacioRequerido { get; set; }
    }

    /// <summary>
    /// Archivo de backup
    /// </summary>
    public class BackupFileViewModel
    {
        public string Nombre { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; }
        public long Tamaño { get; set; }
        public string TamañoFormateado { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
    }

    /// <summary>
    /// ViewModel para mantenimiento del sistema
    /// </summary>
    public class MantenimientoViewModel : AdminBaseViewModel
    {
        public bool ModoMantenimientoActivo { get; set; }
        public DateTime? InicioMantenimiento { get; set; }
        public DateTime? FinEstimadoMantenimiento { get; set; }
        public string MensajeMantenimiento { get; set; } = string.Empty;
        public List<TareaMantenimientoViewModel> TareasProgramadas { get; set; } = new();
    }

    /// <summary>
    /// Tarea de mantenimiento
    /// </summary>
    public class TareaMantenimientoViewModel
    {
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public DateTime FechaProgramada { get; set; }
        public string Estado { get; set; } = string.Empty;
        public int Progreso { get; set; }
        public TimeSpan? DuracionEstimada { get; set; }
    }

    /// <summary>
    /// ViewModel para configuración del modo de registro de usuarios
    /// </summary>
    public class ConfiguracionModoRegistroViewModel
    {
        [Required(ErrorMessage = "Debe seleccionar un modo de registro")]
        public ModoRegistroUsuarios ModoRegistro { get; set; }

        [StringLength(500, ErrorMessage = "El mensaje no puede exceder 500 caracteres")]
        public string? MensajePersonalizado { get; set; }
    }

    /// <summary>
    /// ViewModel para configuración de email
    /// </summary>
    public class EmailConfigurationViewModel : AdminBaseViewModel
    {
        [Display(Name = "Servidor SMTP")]
        [Required(ErrorMessage = "El servidor SMTP es requerido")]
        [StringLength(255, ErrorMessage = "No puede tener más de 255 caracteres")]
        public string SmtpServer { get; set; } = string.Empty;

        [Display(Name = "Puerto SMTP")]
        [Range(1, 65535, ErrorMessage = "Debe estar entre 1 y 65535")]
        public int SmtpPort { get; set; } = 587;

        [Display(Name = "Habilitar SSL")]
        public bool EnableSsl { get; set; } = true;

        [Display(Name = "Usuario SMTP")]
        [StringLength(255, ErrorMessage = "No puede tener más de 255 caracteres")]
        public string? SmtpUsername { get; set; }

        [Display(Name = "Contraseña SMTP")]
        [DataType(DataType.Password)]
        [StringLength(255, ErrorMessage = "No puede tener más de 255 caracteres")]
        public string? SmtpPassword { get; set; }

        [Display(Name = "Email remitente")]
        [Required(ErrorMessage = "El email remitente es requerido")]
        [EmailAddress(ErrorMessage = "Formato de email inválido")]
        public string FromEmail { get; set; } = string.Empty;

        [Display(Name = "Nombre del remitente")]
        [StringLength(100, ErrorMessage = "No puede tener más de 100 caracteres")]
        public string? FromName { get; set; }

        [Display(Name = "Habilitar envío de emails")]
        public bool EnableEmailSending { get; set; } = true;
    }
}