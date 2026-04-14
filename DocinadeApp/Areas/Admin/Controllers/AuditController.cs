using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DocinadeApp.Authorization;
using DocinadeApp.Data;
using DocinadeApp.Models;
using DocinadeApp.Models.Identity;
using DocinadeApp.Models.Audit;
using DocinadeApp.Models.Permissions;
using DocinadeApp.Services.Permissions;
using DocinadeApp.Services.Audit;
using DocinadeApp.ViewModels.Admin;
using System.Security.Claims;

namespace DocinadeApp.Areas.Admin.Controllers
{
    /// <summary>
    /// Controlador para administración de auditoría
    /// </summary>
    [Authorize]
    [Area("Admin")]
    [Route("Admin/[controller]")]
    public class AuditController : Controller
    {
        private const string DateFormat_DayMonth = "dd/MM";
        private readonly IAuditService _auditService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<AuditController> _logger;

        public AuditController(
            IAuditService auditService,
            UserManager<ApplicationUser> userManager,
            ILogger<AuditController> logger)
        {
            _auditService = auditService;
            _userManager = userManager;
            _logger = logger;
        }

        /// <summary>
        /// Lista de logs de auditoría con filtros y paginación
        /// </summary>
        [HttpGet]
        [RequirePermission(ApplicationPermissions.Auditoria.VER)]
        public async Task<IActionResult> Index(
            string? userId,
            string? action,
            string? entityType,
            DateTime? fromDate,
            DateTime? toDate,
            string? logLevel,
            bool? success,
            string? searchTerm,
            int page = 1,
            int pageSize = 50)
        {
            try
            {
                // Validar parámetros
                if (page < 1) page = 1;
                if (pageSize < 1 || pageSize > 1000) pageSize = 50;

                // Limpiar y normalizar filtros
                action = string.IsNullOrWhiteSpace(action) ? null : action.Trim();
                entityType = string.IsNullOrWhiteSpace(entityType) ? null : entityType.Trim();
                logLevel = string.IsNullOrWhiteSpace(logLevel) ? null : logLevel.Trim();
                searchTerm = string.IsNullOrWhiteSpace(searchTerm) ? null : searchTerm.Trim();
                userId = string.IsNullOrWhiteSpace(userId) ? null : userId.Trim();

                // Validar rango de fechas
                if (fromDate.HasValue && toDate.HasValue && fromDate.Value > toDate.Value)
                {
                    TempData["ErrorMessage"] = "La fecha de inicio no puede ser mayor que la fecha de fin.";
                    fromDate = null;
                    toDate = null;
                }

                var filter = new AuditFilterViewModel
                {
                    UserId = userId,
                    Action = action,
                    EntityType = entityType,
                    FromDate = fromDate,
                    ToDate = toDate,
                    LogLevel = logLevel,
                    Success = success,
                    SearchTerm = searchTerm,
                    Page = page,
                    PageSize = pageSize
                };

                // Obtener logs con filtros mejorados
                var auditLogs = await _auditService.GetAuditLogsAsync(
                    startDate: fromDate,
                    endDate: toDate,
                    userId: userId,
                    action: action,
                    pageSize: pageSize,
                    pageNumber: page
                );

                var totalRecords = await _auditService.GetAuditLogsCountAsync(
                    userId: userId,
                    action: action,
                    entityType: entityType,
                    fromDate: fromDate,
                    toDate: toDate
                );

                // Convertir a ViewModels con manejo de errores y aplicar filtros adicionales
                var auditViewModels = new List<AuditLogViewModel>();
                foreach (var log in auditLogs)
                {
                    try
                    {
                        var user = await _userManager.FindByIdAsync(log.UserId);
                        var logViewModel = new AuditLogViewModel
                        {
                            Id = log.Id,
                            UserId = log.UserId,
                            UserName = user?.Email ?? "Usuario eliminado",
                            Action = log.Action,
                            EntityType = log.EntityType,
                            EntityId = log.EntityId,
                            EntityName = log.EntityName ?? string.Empty,
                            OldValues = log.OldValues,
                            NewValues = log.NewValues,
                            IpAddress = log.IpAddress,
                            UserAgent = log.UserAgent,
                            Timestamp = log.Timestamp,
                            LogLevel = log.LogLevel,
                            AdditionalInfo = log.AdditionalInfo,
                            Success = log.Success,
                            ErrorMessage = log.ErrorMessage
                        };

                        // Aplicar filtros adicionales que no están soportados por el servicio
                        bool includeLog = true;

                        // Filtro por tipo de entidad
                        if (!string.IsNullOrWhiteSpace(entityType) && 
                            !log.EntityType.Contains(entityType, StringComparison.OrdinalIgnoreCase))
                        {
                            includeLog = false;
                        }

                        // Filtro por nivel de log
                        if (!string.IsNullOrWhiteSpace(logLevel) && 
                            !string.Equals(log.LogLevel, logLevel, StringComparison.OrdinalIgnoreCase))
                        {
                            includeLog = false;
                        }

                        // Filtro por éxito/fallo
                        if (success.HasValue && log.Success != success.Value)
                        {
                            includeLog = false;
                        }

                        // Filtro por término de búsqueda
                        if (!string.IsNullOrWhiteSpace(searchTerm))
                        {
                            var searchLower = searchTerm.ToLower();
                            var searchFields = new[]
                            {
                                log.Action?.ToLower(),
                                log.EntityType?.ToLower(),
                                log.EntityName?.ToLower(),
                                logViewModel.UserName?.ToLower(),
                                log.AdditionalInfo?.ToLower(),
                                log.IpAddress?.ToLower()
                            };

                            if (!searchFields.Any(field => field?.Contains(searchLower) == true))
                            {
                                includeLog = false;
                            }
                        }

                        if (includeLog)
                        {
                            auditViewModels.Add(logViewModel);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Error al procesar log de auditoría {LogId}", log.Id);
                        // Añadir log con información mínima solo si pasa los filtros básicos
                        var basicLog = new AuditLogViewModel
                        {
                            Id = log.Id,
                            UserId = log.UserId,
                            UserName = "Error al cargar usuario",
                            Action = log.Action,
                            EntityType = log.EntityType,
                            EntityId = log.EntityId,
                            EntityName = log.EntityName ?? string.Empty,
                            Timestamp = log.Timestamp,
                            Success = log.Success,
                            ErrorMessage = log.ErrorMessage
                        };

                        // Aplicar filtros básicos incluso para logs con error
                        bool includeErrorLog = true;
                        
                        if (!string.IsNullOrWhiteSpace(entityType) && 
                            !log.EntityType.Contains(entityType, StringComparison.OrdinalIgnoreCase))
                        {
                            includeErrorLog = false;
                        }

                        if (success.HasValue && log.Success != success.Value)
                        {
                            includeErrorLog = false;
                        }

                        if (includeErrorLog)
                        {
                            auditViewModels.Add(basicLog);
                        }
                    }
                }

                // Recalcular el total de registros considerando los filtros aplicados
                // Nota: Esto es una aproximación ya que aplicamos filtros después de la consulta
                var filteredTotalRecords = auditViewModels.Count;
                if (filteredTotalRecords < pageSize && page == 1)
                {
                    totalRecords = filteredTotalRecords;
                }

                // Cargar datos para filtros con manejo de errores
                try
                {
                    filter.AvailableActions = (await _auditService.GetAvailableActionsAsync())?.Where(a => !string.IsNullOrWhiteSpace(a)).ToList() ?? new List<string>();
                    filter.AvailableEntityTypes = (await _auditService.GetAvailableEntityTypesAsync())?.Where(e => !string.IsNullOrWhiteSpace(e)).ToList() ?? new List<string>();
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Error al cargar opciones de filtros");
                    filter.AvailableActions = new List<string>();
                    filter.AvailableEntityTypes = new List<string>();
                }
                
                filter.AvailableLogLevels = new List<string> { "Information", "Warning", "Error", "Critical" };

                var viewModel = new AuditListViewModel
                {
                    AuditLogs = auditViewModels,
                    Filter = filter,
                    TotalRecords = totalRecords,
                    CurrentPage = page,
                    TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize)
                };

                // Log de la consulta de auditoría
                var filterInfo = filter.HasActiveFilters() 
                    ? $"Filtros activos: {string.Join(", ", filter.GetActiveFiltersDescription())}"
                    : "Sin filtros aplicados";

                await _auditService.LogAsync(
                    userId: User.FindFirstValue(ClaimTypes.NameIdentifier)!,
                    action: AuditActionTypes.Auditoria.Ver,
                    entityType: AuditEntityTypes.AUDIT_LOG,
                    entityId: null,
                    additionalInfo: $"Lista de auditoría consultada. Página: {page}, Registros: {auditViewModels.Count}/{totalRecords}. {filterInfo}"
                );

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar logs de auditoría");
                TempData["ErrorMessage"] = "Error al cargar los logs de auditoría. Por favor, intente nuevamente.";
                
                // Devolver vista con datos mínimos para evitar errores adicionales
                return View(new AuditListViewModel
                {
                    Filter = new AuditFilterViewModel { Page = page, PageSize = pageSize },
                    AuditLogs = new List<AuditLogViewModel>(),
                    TotalRecords = 0,
                    CurrentPage = page,
                    TotalPages = 0
                });
            }
        }

        /// <summary>
        /// Detalles de un log de auditoría específico
        /// </summary>
        [HttpGet("Details/{id}")]
        [RequirePermission(ApplicationPermissions.Auditoria.VER)]
        public async Task<IActionResult> Details(int id)
        {
            try
            {

                var auditLog = await _auditService.GetAuditLogByIdAsync(id) as AuditLog;

                if (auditLog == null)
                {
                    return NotFound("Log de auditoría no encontrado.");
                }

                var user = await _userManager.FindByIdAsync(auditLog.UserId);

                var viewModel = new AuditLogViewModel
                {
                    Id = auditLog.Id,
                    UserId = auditLog.UserId,
                    UserName = user?.Email ?? "Usuario eliminado",
                    Action = auditLog.Action,
                    EntityType = auditLog.EntityType,
                    EntityId = auditLog.EntityId,
                    EntityName = auditLog.EntityName,
                    OldValues = auditLog.OldValues,
                    NewValues = auditLog.NewValues,
                    IpAddress = auditLog.IpAddress,
                    UserAgent = auditLog.UserAgent,
                    Timestamp = auditLog.Timestamp,
                    LogLevel = auditLog.LogLevel,
                    AdditionalInfo = auditLog.AdditionalInfo,
                    Success = auditLog.Success,
                    ErrorMessage = auditLog.ErrorMessage
                };

                await _auditService.LogAsync(
                    userId: User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier) ?? "Sistema",
                    entityType: AuditEntityTypes.AUDIT_LOG,
                    action: AuditActionTypes.Auditoria.Ver,
                    entityId: id.ToString(),
                    additionalInfo: $"Detalles de log de auditoría visualizados: {id}"
                );

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar detalles del log de auditoría {LogId}", id);
                TempData["ErrorMessage"] = "Error al cargar los detalles del log.";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Estadísticas de auditoría
        /// </summary>
        [HttpGet("Statistics")]
        [RequirePermission(ApplicationPermissions.Auditoria.VER_METRICAS)]
        public async Task<IActionResult> Statistics()
        {
            try
            {
                var actionStats = await _auditService.GetActionStatisticsAsync();
                var entityStats = await _auditService.GetEntityStatisticsAsync();
                var userStats = await _auditService.GetUserActivityStatisticsAsync();
                var dailyStatsRaw = await _auditService.GetDailyActivityStatisticsAsync(DateTime.UtcNow.AddDays(-30));

                // Convertir fechas a strings para el ViewModel
                var dailyStats = dailyStatsRaw.ToDictionary(
                       kvp => DateTime.ParseExact(kvp.Key, "yyyy-MM-dd", null).ToString(DateFormat_DayMonth),
                       kvp => kvp.Value
                   );

                var viewModel = new AuditStatisticsViewModel
                {
                    ActionStatistics = actionStats,
                    EntityStatistics = entityStats,
                    UserActivityStatistics = userStats,
                    DailyActivityStatistics = dailyStats,
                    TotalLogsToday = await _auditService.GetAuditLogsCountAsync(
                        fromDate: DateTime.UtcNow.Date,
                        toDate: DateTime.UtcNow.Date.AddDays(1)
                    ),
                    TotalLogsThisWeek = await _auditService.GetAuditLogsCountAsync(
                        fromDate: DateTime.UtcNow.Date.AddDays(-7),
                        toDate: DateTime.UtcNow.Date.AddDays(1)
                    ),
                    TotalLogsThisMonth = await _auditService.GetAuditLogsCountAsync(
                        fromDate: DateTime.UtcNow.Date.AddDays(-30),
                        toDate: DateTime.UtcNow.Date.AddDays(1)
                    ),
                    ErrorCount = await _auditService.GetAuditLogsCountAsync(
                        fromDate: DateTime.UtcNow.Date.AddDays(-7)
                    )
                };

                // Obtener usuarios más activos
                var topUsers = userStats.OrderByDescending(kvp => kvp.Value).Take(10);
                foreach (var userStat in topUsers)
                {
                    var user = await _userManager.FindByIdAsync(userStat.Key);
                    if (user != null)
                    {
                        viewModel.MostActiveUsers.Add($"{user.Email} ({userStat.Value} acciones)");
                    }
                }

                // Obtener acciones más comunes
                viewModel.MostCommonActions = actionStats
                    .OrderByDescending(kvp => kvp.Value)
                    .Take(10)
                    .Select(kvp => $"{kvp.Key} ({kvp.Value} veces)")
                    .ToList();


                await _auditService.LogAsync(userId: User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier) ?? "Sistema",
                    AuditActionTypes.Auditoria.VerMetricas,
                    AuditEntityTypes.AUDIT_LOG,
                    null,
                    "Estadísticas de auditoría consultadas"
                );

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar estadísticas de auditoría");
                TempData["ErrorMessage"] = "Error al cargar las estadísticas.";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Exportar logs de auditoría a CSV
        /// </summary>
        [HttpPost("Export")]
        [RequirePermission(ApplicationPermissions.Auditoria.EXPORTAR)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Export(
            string? userId,
            string? action,
            string? entityType,
            DateTime? fromDate,
            DateTime? toDate,
            string? logLevel,
            bool? success,
            string? searchTerm)
        {
            try
            {
                // Obtener todos los logs con los filtros aplicados (sin paginación)
                var auditLogs = await _auditService.GetAuditLogsAsync(
                    startDate: fromDate,
                    endDate: toDate,
                    userId: userId,
                    action: action,
                    pageSize: int.MaxValue,
                    pageNumber: 1
                );

                // Aplicar filtros adicionales si es necesario
                if (!string.IsNullOrWhiteSpace(entityType) || 
                    !string.IsNullOrWhiteSpace(logLevel) || 
                    success.HasValue || 
                    !string.IsNullOrWhiteSpace(searchTerm))
                {
                    auditLogs = auditLogs.Where(log =>
                    {
                        // Filtro por tipo de entidad
                        if (!string.IsNullOrWhiteSpace(entityType) && 
                            !log.EntityType.Contains(entityType, StringComparison.OrdinalIgnoreCase))
                            return false;

                        // Filtro por nivel de log
                        if (!string.IsNullOrWhiteSpace(logLevel) && 
                            !string.Equals(log.LogLevel, logLevel, StringComparison.OrdinalIgnoreCase))
                            return false;

                        // Filtro por éxito/fallo
                        if (success.HasValue && log.Success != success.Value)
                            return false;

                        // Filtro por término de búsqueda
                        if (!string.IsNullOrWhiteSpace(searchTerm))
                        {
                            var searchLower = searchTerm.ToLower();
                            var searchFields = new[]
                            {
                                log.Action?.ToLower(),
                                log.EntityType?.ToLower(),
                                log.EntityName?.ToLower(),
                                log.AdditionalInfo?.ToLower(),
                                log.IpAddress?.ToLower()
                            };

                            if (!searchFields.Any(field => field?.Contains(searchLower) == true))
                                return false;
                        }

                        return true;
                    }).ToList();
                }



                // Generar CSV
                var csv = new System.Text.StringBuilder();
                csv.AppendLine("Fecha,Usuario,Acción,Tipo Entidad,Entidad,Dirección IP,Éxito,Mensaje Error");

                foreach (var log in auditLogs)
                {
                    var user = await _userManager.FindByIdAsync(log.UserId);
                    csv.AppendLine($"{log.Timestamp:yyyy-MM-dd HH:mm:ss}," +
                                  $"\"{user?.Email ?? "Usuario eliminado"}\"," +
                                  $"\"{log.Action}\"," +
                                  $"\"{log.EntityType}\"," +
                                  $"\"{log.EntityName}\"," +
                                  $"\"{log.IpAddress}\"," +
                                  $"{(log.Success ? "Sí" : "No")}," +
                                  $"\"{log.ErrorMessage?.Replace("\"", "\"\"")}\"");
                }

                var fileName = $"audit_logs_{DateTime.UtcNow:yyyyMMdd_HHmmss}.csv";
                var bytes = System.Text.Encoding.UTF8.GetBytes(csv.ToString());

                await _auditService.LogAsync(userId: User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier) ?? "Sistema",
                    AuditActionTypes.Auditoria.Exportar,
                    AuditEntityTypes.AUDIT_LOG,
                    null,
                    $"Logs de auditoría exportados: {auditLogs.Count} registros"
                );

                return File(bytes, "text/csv", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al exportar logs de auditoría");
                TempData["ErrorMessage"] = "Error al exportar los logs.";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Limpiar logs antiguos
        /// </summary>
        [HttpPost("Cleanup")]
        [RequirePermission(ApplicationPermissions.Auditoria.LIMPIAR)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cleanup(int daysToKeep = 90)
        {
            try
            {
                if (daysToKeep < 30)
                {
                    return Json(new { success = false, message = "No se pueden eliminar logs de menos de 30 días." });
                }

                await _auditService.CleanupOldLogsAsync(daysToKeep);
                var deletedCount = 0; // Aquí deberías obtener el número de registros eliminados

                await _auditService.LogAsync(
                    User.FindFirstValue(ClaimTypes.NameIdentifier)!,
                    AuditActionTypes.Auditoria.Limpiar,
                    AuditEntityTypes.AUDIT_LOG, null,
                    additionalInfo: $"Días conservados: {daysToKeep} Limpieza de logs ejecutada: {deletedCount} registros eliminados"
                );

                return Json(new
                {
                    success = true,
                    message = $"Se eliminaron {deletedCount} logs antiguos exitosamente.",
                    deletedCount = deletedCount
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al limpiar logs de auditoría");
                return Json(new { success = false, message = "Error interno al limpiar los logs." });
            }
        }

        /// <summary>
        /// Obtener actividad de usuario específico
        /// </summary>
        [HttpGet("UserActivity/{userId}")]
        [RequirePermission(ApplicationPermissions.Auditoria.VER)]
        public async Task<IActionResult> UserActivity(string userId, int page = 1, int pageSize = 50)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);


                if (user == null)
                {
                    return NotFound("Usuario no encontrado.");
                }

                var auditLogs = await _auditService.GetAuditLogsAsync(
                    userId: userId,
                    pageSize: pageSize,
                    pageNumber: page
                );


                var totalRecords = await _auditService.GetAuditLogsCountAsync(userId: userId);

                var auditViewModels = auditLogs.Select(log => new AuditLogViewModel
                {
                    Id = log.Id,
                    UserId = log.UserId,
                    UserName = user.Email ?? string.Empty,
                    Action = log.Action,
                    EntityType = log.EntityType,
                    EntityId = log.EntityId,
                    EntityName = log.EntityName,
                    OldValues = log.OldValues,
                    NewValues = log.NewValues,
                    IpAddress = log.IpAddress,
                    UserAgent = log.UserAgent,
                    Timestamp = log.Timestamp,
                    LogLevel = log.LogLevel,
                    AdditionalInfo = log.AdditionalInfo,
                    Success = log.Success,
                    ErrorMessage = log.ErrorMessage
                }).ToList();

                var viewModel = new AuditListViewModel
                {
                    AuditLogs = auditViewModels,
                    Filter = new AuditFilterViewModel { UserId = userId, Page = page, PageSize = pageSize },
                    TotalRecords = totalRecords,
                    CurrentPage = page,
                    TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize)
                };

                ViewBag.UserName = user.Email;
                ViewBag.UserFullName = user.NombreCompleto;

                await _auditService.LogAsync(
                    User.FindFirstValue(ClaimTypes.NameIdentifier)!,
                    AuditActionTypes.Auditoria.Ver,
                    AuditEntityTypes.USER,
                    userId,
                    $"Actividad de usuario consultada: {user.Email}"
                );

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar actividad del usuario {UserId}", userId);
                TempData["ErrorMessage"] = "Error al cargar la actividad del usuario.";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Dashboard de auditoría (resumen ejecutivo)
        /// </summary>
        [HttpGet("Dashboard")]
        [RequirePermission(ApplicationPermissions.Auditoria.VER_METRICAS)]
        public async Task<IActionResult> Dashboard()
        {
            try
            {
                var totalUsers = await _userManager.Users.CountAsync();
                var activeUsers = await _userManager.Users.CountAsync(u => u.Activo);

                var totalAuditLogs = await _auditService.GetAuditLogsCountAsync();
                var loginsToday = await _auditService.GetAuditLogsCountAsync(
                    action: AuditActionTypes.Auth.Login,
                    fromDate: DateTime.UtcNow.Date,
                    toDate: DateTime.UtcNow.Date.AddDays(1)
                );
                var loginsThisWeek = await _auditService.GetAuditLogsCountAsync(
                    action: AuditActionTypes.Auth.Login,
                    fromDate: DateTime.UtcNow.Date.AddDays(-7)
                );
                var errorsToday = await _auditService.GetAuditLogsCountAsync(
                    fromDate: DateTime.UtcNow.Date,
                    toDate: DateTime.UtcNow.Date.AddDays(1)
                );

                // Actividad reciente de usuarios
                var recentActivity = new List<UserActivitySummary>();
                var recentLogs = await _auditService.GetAuditLogsAsync(
                    pageSize: 20,
                    pageNumber: 1

                );
                var userGroups = recentLogs.GroupBy(l => l.UserId).Take(10);

                foreach (var group in userGroups)
                {
                    var user = await _userManager.FindByIdAsync(group.Key);
                    if (user != null)
                    {
                        var lastLog = group.OrderByDescending(l => l.Timestamp).First();
                        recentActivity.Add(new UserActivitySummary
                        {
                            UserId = user.Id,
                            Email = user.Email ?? string.Empty,
                            NombreCompleto = user.NombreCompleto,
                            LastActivity = lastLog.Timestamp,
                            LastAction = lastLog.Action,
                            ActivityCount = group.Count()
                        });
                    }
                }

                // Tendencia semanal de logins
                var weeklyLoginTrend = new Dictionary<string, int>();
                for (int i = 6; i >= 0; i--)
                {
                    var date = DateTime.UtcNow.Date.AddDays(-i);
                    var loginCount = await _auditService.GetAuditLogsCountAsync(
                        action: AuditActionTypes.Auth.Login,
                        fromDate: date,
                        toDate: date.AddDays(1)
                    );
                    weeklyLoginTrend[date.ToString("dd/MM")] = loginCount;
                }

                var viewModel = new AdminDashboardViewModel
                {
                    TotalUsers = totalUsers,
                    ActiveUsers = activeUsers,
                    TotalAuditLogs = totalAuditLogs,
                    LoginsToday = loginsToday,
                    LoginsThisWeek = loginsThisWeek,
                    ErrorsToday = errorsToday,
                    RecentUserActivity = recentActivity,
                    WeeklyLoginTrend = weeklyLoginTrend
                };

                await _auditService.LogAsync(
                    User.FindFirstValue(ClaimTypes.NameIdentifier)!,
                    AuditActionTypes.Auditoria.VerMetricas,
                    AuditEntityTypes.AUDIT_LOG,
                    null,
                    "Dashboard de auditoría consultado"
                );

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar dashboard de auditoría");
                TempData["ErrorMessage"] = "Error al cargar el dashboard.";
                return View(new AdminDashboardViewModel());
            }
        }
    }
}