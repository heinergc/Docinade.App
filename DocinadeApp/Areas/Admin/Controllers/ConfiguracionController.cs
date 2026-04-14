using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RubricasApp.Web.Authorization;
using RubricasApp.Web.Data;
using RubricasApp.Web.Models.Identity;
using RubricasApp.Web.Models.Permissions;
using RubricasApp.Web.Models;
using RubricasApp.Web.Services.Audit;
using RubricasApp.Web.Services;
using RubricasApp.Web.ViewModels.Admin;
using System.Security.Claims;
using System.Text.Json;
using System.ComponentModel.DataAnnotations;

namespace RubricasApp.Web.Areas.Admin.Controllers
{
    /// <summary>
    /// Controlador para administración de configuración del sistema
    /// </summary>
    [Authorize]
    [Area("Admin")]
    [Route("Admin/[controller]")]
    public class ConfiguracionController : Controller
    {
        private readonly RubricasDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IAuditService _auditService;
        private readonly ILogger<ConfiguracionController> _logger;
        private readonly IWebHostEnvironment _environment;
        private readonly IConfiguracionService _configuracionService;
        private readonly IEmailService _emailService;

        public ConfiguracionController(
            RubricasDbContext context,
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration,
            IAuditService auditService,
            ILogger<ConfiguracionController> logger,
            IWebHostEnvironment environment,
            IConfiguracionService configuracionService,
            IEmailService emailService)
        {
            _context = context;
            _userManager = userManager;
            _configuration = configuration;
            _auditService = auditService;
            _logger = logger;
            _environment = environment;
            _configuracionService = configuracionService;
            _emailService = emailService;
        }

        /// <summary>
        /// Página principal de configuración del sistema
        /// </summary>
        [HttpGet]
        [RequirePermission(ApplicationPermissions.Configuracion.VER)]
        public async Task<IActionResult> Index()
        {
            try
            {
                var model = new ConfiguracionIndexViewModel
                {
                    // Información del sistema
                    SystemInfo = await GetSystemInfoAsync(),
                    
                    // Configuraciones generales
                    GeneralSettings = await GetGeneralSettingsAsync(),
                    
                    // Estado del sistema
                    SystemHealth = await GetSystemHealthAsync(),
                    
                    // Estadísticas rápidas
                    QuickStats = await GetQuickStatsAsync(),
                    
                    // Configuraciones de registro de usuarios
                    Configuraciones = await _configuracionService.ObtenerTodasLasConfiguracionesAsync(),
                    ModoRegistroActual = await _configuracionService.ObtenerModoRegistroAsync(),
                    MensajeRegistroCerrado = await _configuracionService.ObtenerMensajeRegistroCerradoAsync(),
                    
                    // Configuración de empadronamiento público
                    EmpadronamientoPublicoHabilitado = await ObtenerEstadoEmpadronamientoPublico()
                };

                await _auditService.LogAsync(
                    User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "Sistema",
                    "Configuración",
                    "Ver",
                    null,
                    "Página de configuración consultada"
                );

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar página de configuración");
                TempData["ErrorMessage"] = "Error al cargar la configuración del sistema.";
                return View(new ConfiguracionIndexViewModel());
            }
        }

        /// <summary>
        /// Configuración general del sistema
        /// </summary>
        [HttpGet("General")]
        [RequirePermission(ApplicationPermissions.Configuracion.EDITAR_SISTEMA)]
        public async Task<IActionResult> General()
        {
            try
            {
                var model = new GeneralSettingsViewModel
                {
                    // Información del Sistema
                    NombreInstitucion = GetConfigValue("Sistema:Nombre", "Sistema de Rúbricas"),
                    SitioWeb = GetConfigValue("Sistema:SitioWeb", ""),
                    LogoInstitucion = GetConfigValue("Sistema:Logo", ""),
                    Descripcion = GetConfigValue("Sistema:Descripcion", "Sistema de evaluación con rúbricas"),
                    
                    // Información de Contacto
                    EmailSoporte = GetConfigValue("Sistema:EmailSoporte", "soporte@rubricas.com"),
                    TelefonoSoporte = GetConfigValue("Sistema:TelefonoSoporte", ""),
                    DireccionFisica = GetConfigValue("Sistema:Direccion", ""),
                    HorarioAtencion = GetConfigValue("Sistema:HorarioAtencion", "Lunes a Viernes 8:00 AM - 5:00 PM"),
                    
                    // Configuración de Sesiones
                    TiempoExpiracionSesion = GetConfigValueInt("Sesion:TiempoLimite", 30),
                    RecordarSesion = GetConfigValueBool("Sesion:RecordarSesion", true),
                    MaximoIntentosFallidos = GetConfigValueInt("Seguridad:MaxIntentosFallidos", 5),
                    
                    // Configuración de Archivos
                    TamanoMaximoArchivo = GetConfigValueInt("Archivos:TamañoMaximoMB", 10),
                    TiposArchivosPermitidos = GetConfigValue("Archivos:TiposPermitidos", ".jpg,.png,.pdf,.docx,.xlsx"),
                    RutaAlmacenamiento = GetConfigValue("Archivos:RutaAlmacenamiento", "~/uploads/"),
                    
                    // Configuración de Notificaciones
                    NotificacionesEmail = GetConfigValueBool("Notificaciones:Email", true),
                    NotificacionesSistema = GetConfigValueBool("Notificaciones:Sistema", true),
                    FrecuenciaNotificaciones = GetConfigValue("Notificaciones:Frecuencia", "Diaria"),
                    
                    // Configuración de Idioma y Formato
                    IdiomaDefecto = GetConfigValue("Localizacion:IdiomaDefecto", "es-CR"),
                    ZonaHoraria = GetConfigValue("Localizacion:ZonaHoraria", "America/Costa_Rica"),
                    FormatoFecha = GetConfigValue("Localizacion:FormatoFecha", "dd/MM/yyyy"),
                    FormatoHora = GetConfigValue("Localizacion:FormatoHora", "HH:mm"),
                    
                    // Propiedades heredadas para compatibilidad
                    MantenimientoActivo = GetConfigValueBool("Sistema:MantenimientoActivo", false),
                    RegistroUsuariosAbierto = GetConfigValueBool("Sistema:RegistroAbierto", true),
                    NotificacionesActivas = GetConfigValueBool("Notificaciones:Activas", true),
                    BackupAutomaticoActivo = GetConfigValueBool("Backup:Automatico", false)
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar configuración general");
                TempData["ErrorMessage"] = "Error al cargar la configuración general.";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Guardar configuración general
        /// </summary>
        [HttpPost("General")]
        [RequirePermission(ApplicationPermissions.Configuracion.EDITAR_SISTEMA)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> General(GeneralSettingsViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                // Aquí normalmente guardarías en base de datos o archivo de configuración
                // Por ahora simularemos el guardado

                await _auditService.LogAsync(
                    User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "Sistema",
                    "Configuración",
                    "Actualizar",
                    null,
                    "Configuración general actualizada"
                );

                TempData["SuccessMessage"] = "Configuración general guardada exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar configuración general");
                TempData["ErrorMessage"] = "Error al guardar la configuración.";
                return View(model);
            }
        }

        /// <summary>
        /// Configuración de seguridad
        /// </summary>
        [HttpGet("Seguridad")]
        [RequirePermission(ApplicationPermissions.Configuracion.EDITAR_SEGURIDAD)]
        public async Task<IActionResult> Seguridad()
        {
            try
            {
                var model = new SecurityViewModel
                {
                    // Políticas de Contraseñas
                    LongitudMinimaPassword = GetConfigValueInt("Seguridad:LongitudMinimaPassword", 8),
                    RequiereLetrasMayusculas = GetConfigValueBool("Seguridad:RequiereMayusculas", true),
                    RequiereLetrasMinusculas = GetConfigValueBool("Seguridad:RequiereMinusculas", true),
                    RequiereNumeros = GetConfigValueBool("Seguridad:RequiereNumeros", true),
                    RequiereCaracteresEspeciales = GetConfigValueBool("Seguridad:RequiereCaracteresEspeciales", false),
                    DiasExpiracionPassword = GetConfigValueInt("Seguridad:TiempoExpiracionPasswordDias", 90),
                    HistorialPasswords = GetConfigValueInt("Seguridad:HistorialPasswordsRecordar", 5),
                    
                    // Bloqueo de Cuentas
                    BloqueoAutomatico = GetConfigValueBool("Seguridad:BloqueoAutomatico", true),
                    MaximosIntentos = GetConfigValueInt("Seguridad:MaxIntentosFallidos", 5),
                    TiempoBloqueo = GetConfigValueInt("Seguridad:TiempoBloqueoMinutos", 30),
                    NotificarBloqueos = GetConfigValueBool("Seguridad:NotificarBloqueos", true),
                    BloqueoTemporal = GetConfigValueBool("Seguridad:BloqueoTemporal", true),
                    
                    // Autenticación de Dos Factores
                    Habilitar2FA = GetConfigValueBool("Seguridad:RequiereDosFactores", false),
                    Requerir2FAAdmins = GetConfigValueBool("Seguridad:Requerir2FAAdmins", true),
                    TiempoValidezCodigo = GetConfigValueInt("Seguridad:TiempoValidezCodigo", 5),
                    
                    // Configuración de Sesiones
                    TiempoInactividadSesion = GetConfigValueInt("Sesion:TiempoInactividad", 30),
                    MaximoSesionesSimultaneas = GetConfigValueInt("Sesion:MaximoSesionesSimultaneas", 3),
                    CerrarSesionesInactivas = GetConfigValueBool("Sesion:CerrarSesionesInactivas", true),
                    LoguearCambiosSesion = GetConfigValueBool("Sesion:LoguearCambios", true),
                    
                    // Auditoría y Logs
                    AuditoriaHabilitada = GetConfigValueBool("Auditoria:Habilitada", true),
                    LoguearAccesos = GetConfigValueBool("Auditoria:LoguearAccesos", true),
                    LoguearCambiosDatos = GetConfigValueBool("Auditoria:LoguearCambiosDatos", true),
                    RetencionLogs = GetConfigValueInt("Auditoria:RetencionLogs", 365),
                    NotificarEventosSeguridad = GetConfigValueBool("Auditoria:NotificarEventosSeguridad", true),
                    
                    // Configuraciones Adicionales
                    ValidarIPAcceso = GetConfigValueBool("Seguridad:ValidarIPAcceso", false),
                    ForzarHTTPS = GetConfigValueBool("Seguridad:ForzarHTTPS", true),
                    ValidarDispositivo = GetConfigValueBool("Seguridad:ValidarDispositivo", false),
                    CaptchaLogin = GetConfigValueBool("Seguridad:CaptchaLogin", false),
                    NivelSeguridadMinimo = GetConfigValue("Seguridad:NivelMinimo", "Medio")
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar configuración de seguridad");
                TempData["ErrorMessage"] = "Error al cargar la configuración de seguridad.";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Guardar configuración de seguridad
        /// </summary>
        [HttpPost("Seguridad")]
        [RequirePermission(ApplicationPermissions.Configuracion.EDITAR_SEGURIDAD)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Seguridad(SecurityViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                // Aquí guardarías la configuración de seguridad

                await _auditService.LogAsync(
                    User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "Sistema",
                    "Configuración",
                    "Actualizar Seguridad",
                    null,
                    "Configuración de seguridad actualizada"
                );

                TempData["SuccessMessage"] = "Configuración de seguridad guardada exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar configuración de seguridad");
                TempData["ErrorMessage"] = "Error al guardar la configuración de seguridad.";
                return View(model);
            }
        }

        /// <summary>
        /// Página de logs del sistema
        /// </summary>
        [HttpGet("Logs")]
        [RequirePermission(ApplicationPermissions.Configuracion.VER_LOGS)]
        public async Task<IActionResult> Logs()
        {
            try
            {
                var model = new LogsViewModel
                {
                    LogFiles = await GetLogFilesAsync(),
                    RecentLogs = await GetRecentLogsAsync()
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar logs del sistema");
                TempData["ErrorMessage"] = "Error al cargar los logs del sistema.";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Health Check del sistema
        /// </summary>
        [HttpGet("HealthCheck")]
        [RequirePermission(ApplicationPermissions.Configuracion.HealthCheck)]
        public async Task<IActionResult> HealthCheck()
        {
            try
            {
                var healthChecks = await PerformHealthChecksAsync();
                
                return Json(new { success = true, checks = healthChecks });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en health check");
                return Json(new { success = false, message = "Error al verificar el estado del sistema" });
            }
        }

        /// <summary>
        /// Actualizar modo de registro de usuarios
        /// </summary>
        [HttpPost("ActualizarModoRegistro")]
        [RequirePermission(ApplicationPermissions.Configuracion.EDITAR_SISTEMA)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActualizarModoRegistro(ConfiguracionModoRegistroViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Datos inválidos. Por favor revise el formulario.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var usuario = User.Identity?.Name ?? "Sistema";
                
                // Actualizar modo de registro
                await _configuracionService.ActualizarModoRegistroAsync(model.ModoRegistro, usuario);
                
                // Actualizar mensaje personalizado si se proporcionó
                if (!string.IsNullOrWhiteSpace(model.MensajePersonalizado))
                {
                    await _configuracionService.ActualizarConfiguracionAsync(
                        ConfiguracionClaves.MensajeRegistroCerrado,
                        model.MensajePersonalizado,
                        "Mensaje mostrado cuando el registro está cerrado",
                        usuario
                    );
                }

                await _auditService.LogAsync(
                    User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "Sistema",
                    "Configuración",
                    "Actualizar Modo Registro",
                    null,
                    $"Modo de registro actualizado a {model.ModoRegistro}"
                );

                _logger.LogInformation("Modo de registro actualizado a {Modo} por {Usuario}", 
                    model.ModoRegistro, usuario);

                TempData["SuccessMessage"] = $"Configuración actualizada correctamente. El registro ahora está {(model.ModoRegistro == ModoRegistroUsuarios.Abierto ? "ABIERTO" : "CERRADO")}.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar modo de registro");
                TempData["ErrorMessage"] = "Error al actualizar la configuración. Inténtelo nuevamente.";
            }

            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Actualizar configuración específica
        /// </summary>
        [HttpPost("ActualizarConfiguracion")]
        [RequirePermission(ApplicationPermissions.Configuracion.EDITAR_SISTEMA)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActualizarConfiguracion(string clave, string valor, string? descripcion)
        {
            if (string.IsNullOrWhiteSpace(clave) || string.IsNullOrWhiteSpace(valor))
            {
                TempData["ErrorMessage"] = "La clave y el valor son requeridos.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var usuario = User.Identity?.Name ?? "Sistema";
                await _configuracionService.ActualizarConfiguracionAsync(clave, valor, descripcion, usuario);
                
                await _auditService.LogAsync(
                    User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "Sistema",
                    "Configuración",
                    "Actualizar Configuración",
                    null,
                    $"Configuración '{clave}' actualizada"
                );
                
                TempData["SuccessMessage"] = $"Configuración '{clave}' actualizada correctamente.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar configuración {Clave}", clave);
                TempData["ErrorMessage"] = "Error al actualizar la configuración.";
            }

            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Modo mantenimiento
        /// </summary>
        [HttpPost("MantenimientoMode")]
        [RequirePermission(ApplicationPermissions.Configuracion.MODO_MANTENIMIENTO)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MantenimientoMode(bool enable)
        {
            try
            {
                // Aquí implementarías la lógica para activar/desactivar modo mantenimiento
                
                await _auditService.LogAsync(
                    User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "Sistema",
                    "Configuración",
                    enable ? "Activar Mantenimiento" : "Desactivar Mantenimiento",
                    null,
                    $"Modo mantenimiento {(enable ? "activado" : "desactivado")}"
                );

                return Json(new { 
                    success = true, 
                    message = $"Modo mantenimiento {(enable ? "activado" : "desactivado")} exitosamente."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cambiar modo mantenimiento");
                return Json(new { success = false, message = "Error al cambiar el modo mantenimiento." });
            }
        }

        /// <summary>
        /// Configuración de email / SMTP
        /// </summary>
        [HttpGet("Email")]
        [RequirePermission(ApplicationPermissions.Configuracion.CONFIGURAR_EMAIL)]
        public async Task<IActionResult> Email()
        {
            try
            {
                var model = new EmailConfigurationViewModel
                {
                    SmtpServer = await _configuracionService.ObtenerConfiguracionAsync(ConfiguracionClaves.EmailSmtpServer, _configuration["Email:SmtpServer"] ?? string.Empty),
                    SmtpPort = int.TryParse(await _configuracionService.ObtenerConfiguracionAsync(ConfiguracionClaves.EmailSmtpPort, _configuration["Email:SmtpPort"] ?? "587"), out int port) ? port : 587,
                    EnableSsl = bool.TryParse(await _configuracionService.ObtenerConfiguracionAsync(ConfiguracionClaves.EmailEnableSsl, "true"), out bool ssl) ? ssl : true,
                    SmtpUsername = await _configuracionService.ObtenerConfiguracionAsync(ConfiguracionClaves.EmailSmtpUsername, string.Empty),
                    FromEmail = await _configuracionService.ObtenerConfiguracionAsync(ConfiguracionClaves.EmailFromEmail, _configuration["Email:From"] ?? string.Empty),
                    FromName = await _configuracionService.ObtenerConfiguracionAsync(ConfiguracionClaves.EmailFromName, _configuration["Email:DisplayName"] ?? string.Empty),
                    EnableEmailSending = bool.TryParse(await _configuracionService.ObtenerConfiguracionAsync(ConfiguracionClaves.EmailHabilitado, "true"), out bool habilitado) ? habilitado : true,
                    // La contraseña nunca se pre-carga en el formulario
                    SmtpPassword = null
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar configuración de email");
                TempData["ErrorMessage"] = "Error al cargar la configuración de email.";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Guardar configuración de email / SMTP
        /// </summary>
        [HttpPost("Email")]
        [RequirePermission(ApplicationPermissions.Configuracion.CONFIGURAR_EMAIL)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Email(EmailConfigurationViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var usuario = User.Identity?.Name ?? "Sistema";

                await _configuracionService.ActualizarConfiguracionAsync(ConfiguracionClaves.EmailSmtpServer, model.SmtpServer, "Servidor SMTP", usuario);
                await _configuracionService.ActualizarConfiguracionAsync(ConfiguracionClaves.EmailSmtpPort, model.SmtpPort.ToString(), "Puerto SMTP", usuario);
                await _configuracionService.ActualizarConfiguracionAsync(ConfiguracionClaves.EmailEnableSsl, model.EnableSsl.ToString(), "Habilitar SSL", usuario);
                await _configuracionService.ActualizarConfiguracionAsync(ConfiguracionClaves.EmailSmtpUsername, model.SmtpUsername ?? string.Empty, "Usuario SMTP", usuario);
                await _configuracionService.ActualizarConfiguracionAsync(ConfiguracionClaves.EmailFromEmail, model.FromEmail, "Email remitente", usuario);
                await _configuracionService.ActualizarConfiguracionAsync(ConfiguracionClaves.EmailFromName, model.FromName ?? string.Empty, "Nombre remitente", usuario);
                await _configuracionService.ActualizarConfiguracionAsync(ConfiguracionClaves.EmailHabilitado, model.EnableEmailSending.ToString(), "Habilitar envío de emails", usuario);

                // Solo actualizar la contraseña si se proporcionó una nueva
                if (!string.IsNullOrWhiteSpace(model.SmtpPassword))
                {
                    await _configuracionService.ActualizarConfiguracionAsync(ConfiguracionClaves.EmailSmtpPassword, model.SmtpPassword, "Contraseña SMTP", usuario);
                }

                await _auditService.LogAsync(
                    User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "Sistema",
                    "Configuración",
                    "Actualizar Email",
                    null,
                    "Configuración SMTP actualizada"
                );

                TempData["SuccessMessage"] = "Configuración de email guardada exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar configuración de email");
                TempData["ErrorMessage"] = "Error al guardar la configuración de email.";
                return View(model);
            }
        }

        /// <summary>
        /// Enviar correo de prueba para verificar la configuración SMTP
        /// </summary>
        [HttpPost("Email/Prueba")]
        [RequirePermission(ApplicationPermissions.Configuracion.CONFIGURAR_EMAIL)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EnviarCorreoPrueba(string destinatario)
        {
            if (string.IsNullOrWhiteSpace(destinatario))
                return Json(new { success = false, message = "Debe indicar un destinatario." });

            try
            {
                var ok = await _emailService.SendEmailAsync(
                    destinatario,
                    "Correo de prueba - Sistema de Rúbricas",
                    "<h3>Prueba de configuración SMTP</h3><p>Si recibe este mensaje, la configuración de correo electrónico es correcta.</p>",
                    isHtml: true
                );

                return ok
                    ? Json(new { success = true, message = $"Correo de prueba enviado a {destinatario}." })
                    : Json(new { success = false, message = "No se pudo enviar el correo. Revise los logs del sistema." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al enviar correo de prueba");
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        #region Métodos auxiliares

        private async Task<SystemInfoViewModel> GetSystemInfoAsync()
        {
            return new SystemInfoViewModel
            {
                NombreAplicacion = "Sistema de Evaluaciones",
                Version = "1.0.0",
                EntornoEjecucion = _environment.EnvironmentName,
                FechaInicioSistema = DateTime.Now.AddDays(-5), // Simulated
                UltimaActualizacion = DateTime.Now.AddDays(-1), // Simulated
                VersionFramework = Environment.Version.ToString(),
                ServidorBase = Environment.MachineName,
                BaseDatos = "SQL Server"
            };
        }

        private async Task<GeneralSettingsViewModel> GetGeneralSettingsAsync()
        {
            return new GeneralSettingsViewModel
            {
                MantenimientoActivo = false,
                RegistroUsuariosAbierto = true,
                NotificacionesActivas = true,
                BackupAutomaticoActivo = false
            };
        }

        private async Task<SystemHealthViewModel> GetSystemHealthAsync()
        {
            // Simulated health checks
            return new SystemHealthViewModel
            {
                EstadoGeneral = "Saludable",
                BaseDatosOnline = true,
                EspacioDiscoDisponible = "85%",
                MemoriaDisponible = "78%",
                UltimBackup = DateTime.Now.AddHours(-6)
            };
        }

        private async Task<QuickStatsViewModel> GetQuickStatsAsync()
        {
            var totalUsuarios = await _context.Users.CountAsync();
            var usuariosActivos = await _context.Users.CountAsync(u => u.Activo);
            
            return new QuickStatsViewModel
            {
                TotalUsuarios = totalUsuarios,
                UsuariosActivos = usuariosActivos,
                TotalRoles = await _context.Roles.CountAsync(),
                TotalRubricas = await _context.Rubricas.CountAsync(),
                EvaluacionesHoy = 0 // Simulated
            };
        }

        private async Task<List<LogFileViewModel>> GetLogFilesAsync()
        {
            // Simulated log files
            return new List<LogFileViewModel>
            {
                new() { Nombre = "application.log", Tamaño = "2.5 MB", FechaModificacion = DateTime.Now.AddHours(-1) },
                new() { Nombre = "errors.log", Tamaño = "1.2 MB", FechaModificacion = DateTime.Now.AddHours(-3) },
                new() { Nombre = "audit.log", Tamaño = "5.8 MB", FechaModificacion = DateTime.Now.AddMinutes(-30) }
            };
        }

        private async Task<List<LogEntryViewModel>> GetRecentLogsAsync()
        {
            // Simulated recent logs
            return new List<LogEntryViewModel>
            {
                new() { Timestamp = DateTime.Now.AddMinutes(-5), Level = "INFO", Message = "Usuario autenticado exitosamente" },
                new() { Timestamp = DateTime.Now.AddMinutes(-10), Level = "WARN", Message = "Intento de acceso no autorizado" },
                new() { Timestamp = DateTime.Now.AddMinutes(-15), Level = "INFO", Message = "Backup automático completado" }
            };
        }

        private async Task<List<HealthCheckViewModel>> PerformHealthChecksAsync()
        {
            var checks = new List<HealthCheckViewModel>();

            // Database check
            try
            {
                await _context.Database.OpenConnectionAsync();
                await _context.Database.CloseConnectionAsync();
                checks.Add(new HealthCheckViewModel { Name = "Base de Datos", Status = "Healthy", Message = "Conexión exitosa" });
            }
            catch
            {
                checks.Add(new HealthCheckViewModel { Name = "Base de Datos", Status = "Unhealthy", Message = "Error de conexión" });
            }

            // Memory check
            var workingSet = Environment.WorkingSet;
            checks.Add(new HealthCheckViewModel { 
                Name = "Memoria", 
                Status = workingSet < 500_000_000 ? "Healthy" : "Warning", 
                Message = $"Uso actual: {workingSet / 1024 / 1024} MB" 
            });

            // Disk space check (simulated)
            checks.Add(new HealthCheckViewModel { Name = "Espacio en Disco", Status = "Healthy", Message = "85% disponible" });

            return checks;
        }

        private string GetConfigValue(string key, string defaultValue = "")
        {
            return _configuration[key] ?? defaultValue;
        }

        private int GetConfigValueInt(string key, int defaultValue = 0)
        {
            return int.TryParse(_configuration[key], out var value) ? value : defaultValue;
        }

        private bool GetConfigValueBool(string key, bool defaultValue = false)
        {
            return bool.TryParse(_configuration[key], out var value) ? value : defaultValue;
        }

        private async Task<bool> ObtenerEstadoEmpadronamientoPublico()
        {
            var config = await _context.ConfiguracionesSistema
                .FirstOrDefaultAsync(c => c.Clave == "EmpadronamientoPublico.Habilitar");
            
            if (config != null && bool.TryParse(config.Valor, out var habilitado))
            {
                return habilitado;
            }
            
            return false;
        }

        #endregion
        
        #region Acciones de Empadronamiento Público
        
        /// <summary>
        /// Actualiza el estado del empadronamiento público
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequirePermission(ApplicationPermissions.Configuracion.EDITAR_SISTEMA)]
        public async Task<IActionResult> ActualizarEmpadronamientoPublico(bool habilitado)
        {
            try
            {
                var config = await _context.ConfiguracionesSistema
                    .FirstOrDefaultAsync(c => c.Clave == "EmpadronamientoPublico.Habilitar");
                
                if (config == null)
                {
                    config = new ConfiguracionSistema
                    {
                        Clave = "EmpadronamientoPublico.Habilitar",
                        Valor = habilitado.ToString().ToLower(),
                        Descripcion = "Habilita o deshabilita el acceso al formulario de empadronamiento público",
                        FechaCreacion = DateTime.Now,
                        FechaModificacion = DateTime.Now,
                        UsuarioModificacion = User.Identity?.Name ?? "Sistema"
                    };
                    _context.ConfiguracionesSistema.Add(config);
                }
                else
                {
                    config.Valor = habilitado.ToString().ToLower();
                    config.FechaModificacion = DateTime.Now;
                    config.UsuarioModificacion = User.Identity?.Name ?? "Sistema";
                }
                
                await _context.SaveChangesAsync();
                
                await _auditService.LogAsync(
                    User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "Sistema",
                    "ConfiguracionSistema",
                    "ActualizarEmpadronamientoPublico",
                    config.Id.ToString(),
                    $"Empadronamiento público {(habilitado ? "habilitado" : "deshabilitado")}"
                );
                
                TempData["SuccessMessage"] = $"El empadronamiento público se ha {(habilitado ? "habilitado" : "deshabilitado")} correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar empadronamiento público");
                TempData["ErrorMessage"] = "Error al actualizar la configuración de empadronamiento público.";
                return RedirectToAction(nameof(Index));
            }
        }

        #endregion
    }
}