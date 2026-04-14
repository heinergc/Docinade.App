using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DocinadeApp.Authorization;
using DocinadeApp.Data;
using DocinadeApp.Models;
using DocinadeApp.Models.Identity;
using DocinadeApp.Models.Permissions;
using DocinadeApp.Models.Audit;
using DocinadeApp.Services.Permissions;
using DocinadeApp.Services.Audit;
using DocinadeApp.ViewModels.Admin;
using System.Security.Claims;

namespace DocinadeApp.Areas.Admin.Controllers
{
    /// <summary>
    /// Controlador principal de administración
    /// </summary>
    [Authorize]
    [Area("Admin")]
    [Route("Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IPermissionService _permissionService;
        private readonly IAuditService _auditService;
        private readonly RubricasDbContext _context;
        private readonly ILogger<AdminController> _logger;

        public AdminController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IPermissionService permissionService,
            IAuditService auditService,
            RubricasDbContext context,
            ILogger<AdminController> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _permissionService = permissionService;
            _auditService = auditService;
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Dashboard principal de administración
        /// </summary>
        [HttpGet]
        [RequirePermission(ApplicationPermissions.Admin.VER_DASHBOARD)]
        public async Task<IActionResult> Index()
        {
            try
            {
                // Estadísticas básicas
                var totalUsers = await _userManager.Users.CountAsync();
                var activeUsers = await _userManager.Users.CountAsync(u => u.Activo);
                var totalRoles = await _roleManager.Roles.CountAsync();
                var totalPermissions = ApplicationPermissions.GetAllPermissions().Count;

                // Estadísticas de auditoría
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
                var recentLogs = await _auditService.GetAuditLogsAsync(pageNumber: 1, pageSize: 20);
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

                // Distribución de roles
                var roleDistribution = new Dictionary<string, int>();
                var roles = await _roleManager.Roles.ToListAsync();
                foreach (var role in roles)
                {
                    var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name!);
                    roleDistribution[role.Name!] = usersInRole.Count;
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

                // Alertas del sistema
                var systemAlerts = new List<SystemAlert>();
                
                // Verificar usuarios inactivos
                var inactiveUsers = await _userManager.Users
                    .Where(u => !u.Activo)
                    .CountAsync();
                if (inactiveUsers > 0)
                {
                    systemAlerts.Add(new SystemAlert
                    {
                        Type = "Warning",
                        Title = "Usuarios Inactivos",
                        Message = $"Hay {inactiveUsers} usuarios inactivos en el sistema.",
                        Timestamp = DateTime.UtcNow,
                        ActionUrl = "/Admin/Users?active=false"
                    });
                }

                // Verificar errores recientes
                if (errorsToday > 10)
                {
                    systemAlerts.Add(new SystemAlert
                    {
                        Type = "Error",
                        Title = "Errores Frecuentes",
                        Message = $"Se han registrado {errorsToday} errores hoy.",
                        Timestamp = DateTime.UtcNow,
                        ActionUrl = "/Admin/Audit?success=false"
                    });
                }

                // Verificar roles sin usuarios
                var emptyRoles = roleDistribution.Where(kvp => kvp.Value == 0).Count();
                if (emptyRoles > 0)
                {
                    systemAlerts.Add(new SystemAlert
                    {
                        Type = "Info",
                        Title = "Roles Sin Usuarios",
                        Message = $"Hay {emptyRoles} roles sin usuarios asignados.",
                        Timestamp = DateTime.UtcNow,
                        ActionUrl = "/Admin/Roles"
                    });
                }

                var viewModel = new AdminDashboardViewModel
                {
                    TotalUsers = totalUsers,
                    ActiveUsers = activeUsers,
                    TotalRoles = totalRoles,
                    TotalPermissions = totalPermissions,
                    TotalAuditLogs = totalAuditLogs,
                    LoginsToday = loginsToday,
                    LoginsThisWeek = loginsThisWeek,
                    ErrorsToday = errorsToday,
                    RecentUserActivity = recentActivity,
                    SystemAlerts = systemAlerts,
                    RoleDistribution = roleDistribution,
                    WeeklyLoginTrend = weeklyLoginTrend
                };

                await _auditService.LogAsync(
                    User.FindFirstValue(ClaimTypes.NameIdentifier)!,
                    AuditActionTypes.Admin.ViewDashboard,
                    AuditEntityTypes.SYSTEM,
                    null,
                    "Dashboard de administración consultado"
                );

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar dashboard de administración");
                TempData["ErrorMessage"] = "Error al cargar el dashboard de administración.";
                return View(new AdminDashboardViewModel());
            }
        }

        /// <summary>
        /// Inicializar datos del sistema (roles y permisos por defecto)
        /// </summary>
        [HttpPost("Initialize")]
        [RequirePermission(ApplicationPermissions.Configuracion.Manage)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Initialize()
        {
            try
            {
                await _permissionService.InitializeDefaultRolesAndPermissionsAsync();

                await _auditService.LogAsync(
                    User.FindFirstValue(ClaimTypes.NameIdentifier)!,
                    AuditActionTypes.Configuracion.Initialize,
                    AuditEntityTypes.SYSTEM,
                    null,
                    "Sistema inicializado: roles y permisos por defecto creados"
                );

                return Json(new { success = true, message = "Sistema inicializado exitosamente." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al inicializar sistema");
                return Json(new { success = false, message = "Error al inicializar el sistema." });
            }
        }

        /// <summary>
        /// Sincronizar permisos de todos los roles
        /// </summary>
        [HttpPost("SyncPermissions")]
        [RequirePermission(ApplicationPermissions.Configuracion.Manage)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SyncPermissions()
        {
            try
            {
                var roles = await _roleManager.Roles.ToListAsync();
                var syncedRoles = 0;

                await _permissionService.SyncPermissionsAsync();
                syncedRoles = roles.Count;

                await _auditService.LogAsync(
                    User.FindFirstValue(ClaimTypes.NameIdentifier)!,
                    AuditActionTypes.Configuracion.SyncPermissions,
                    AuditEntityTypes.SYSTEM,
                    null,
                    $"Permisos sincronizados: {syncedRoles} roles actualizados" 
                );

                return Json(new { 
                    success = true, 
                    message = $"Permisos sincronizados exitosamente para {syncedRoles} roles.",
                    syncedRoles = syncedRoles
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al sincronizar permisos");
                return Json(new { success = false, message = "Error al sincronizar permisos." });
            }
        }

        /// <summary>
        /// Obtener estadísticas en tiempo real para dashboard
        /// </summary>
        [HttpGet("Stats")]
        [RequirePermission(ApplicationPermissions.Admin.VER_DASHBOARD)]
        public async Task<IActionResult> Stats()
        {
            try
            {
                var stats = new
                {
                    totalUsers = await _userManager.Users.CountAsync(),
                    activeUsers = await _userManager.Users.CountAsync(u => u.Activo),
                    totalRoles = await _roleManager.Roles.CountAsync(),
                    loginsToday = await _auditService.GetAuditLogsCountAsync(
                        action: AuditActionTypes.Auth.Login,
                        fromDate: DateTime.UtcNow.Date,
                        toDate: DateTime.UtcNow.Date.AddDays(1)
                    ),
                    errorsToday = await _auditService.GetAuditLogsCountAsync(
                        fromDate: DateTime.UtcNow.Date,
                        toDate: DateTime.UtcNow.Date.AddDays(1)
                    ),
                    timestamp = DateTime.UtcNow
                };

                return Json(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener estadísticas");
                return Json(new { error = "Error al obtener estadísticas" });
            }
        }

        /// <summary>
        /// Verificar estado del sistema
        /// </summary>
        [HttpGet("Health")]
        [RequirePermission(ApplicationPermissions.Configuracion.HealthCheck)]
        public async Task<IActionResult> Health()
        {
            try
            {
                var healthStatus = new
                {
                    database = await CheckDatabaseHealthAsync(),
                    users = await CheckUsersHealthAsync(),
                    roles = await CheckRolesHealthAsync(),
                    permissions = await CheckPermissionsHealthAsync(),
                    audit = await CheckAuditHealthAsync(),
                    timestamp = DateTime.UtcNow
                };

                await _auditService.LogAsync(
                    User.FindFirstValue(ClaimTypes.NameIdentifier)!,
                    AuditActionTypes.Configuracion.HealthCheck,
                    AuditEntityTypes.SYSTEM,
                    null,
                    "Verificación de estado del sistema ejecutada"
                );

                return Json(healthStatus);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar estado del sistema");
                return Json(new { error = "Error al verificar estado del sistema" });
            }
        }

        #region Métodos privados para verificación de estado

        private async Task<object> CheckDatabaseHealthAsync()
        {
            try
            {
                await _context.Database.CanConnectAsync();
                return new { status = "healthy", message = "Conexión a base de datos exitosa" };
            }
            catch (Exception ex)
            {
                return new { status = "unhealthy", message = ex.Message };
            }
        }

        private async Task<object> CheckUsersHealthAsync()
        {
            try
            {
                var userCount = await _userManager.Users.CountAsync();
                var activeCount = await _userManager.Users.CountAsync(u => u.Activo);
                
                return new { 
                    status = "healthy", 
                    total = userCount, 
                    active = activeCount,
                    message = $"{activeCount}/{userCount} usuarios activos"
                };
            }
            catch (Exception ex)
            {
                return new { status = "unhealthy", message = ex.Message };
            }
        }

        private async Task<object> CheckRolesHealthAsync()
        {
            try
            {
                var roleCount = await _roleManager.Roles.CountAsync();
                var systemRoles = new[] { "SuperAdmin", "Admin", "Profesor", "Coordinador" };
                var missingRoles = new List<string>();

                foreach (var roleName in systemRoles)
                {
                    if (!await _roleManager.RoleExistsAsync(roleName))
                    {
                        missingRoles.Add(roleName);
                    }
                }

                if (missingRoles.Any())
                {
                    return new { 
                        status = "warning", 
                        total = roleCount,
                        message = $"Roles faltantes: {string.Join(", ", missingRoles)}"
                    };
                }

                return new { 
                    status = "healthy", 
                    total = roleCount,
                    message = $"{roleCount} roles configurados correctamente"
                };
            }
            catch (Exception ex)
            {
                return new { status = "unhealthy", message = ex.Message };
            }
        }

        private async Task<object> CheckPermissionsHealthAsync()
        {
            try
            {
                var allPermissions = ApplicationPermissions.GetAllPermissions();
                var configuredPermissions = 0;

                var roles = await _roleManager.Roles.ToListAsync();
                foreach (var role in roles)
                {
                    var claims = await _roleManager.GetClaimsAsync(role);
                    configuredPermissions += claims.Count(c => c.Type == "permission");
                }

                return new { 
                    status = "healthy",
                    total = allPermissions.Count,
                    configured = configuredPermissions,
                    message = $"{allPermissions.Count} permisos disponibles, {configuredPermissions} configurados en roles"
                };
            }
            catch (Exception ex)
            {
                return new { status = "unhealthy", message = ex.Message };
            }
        }

        private async Task<object> CheckAuditHealthAsync()
        {
            try
            {
                var totalLogs = await _auditService.GetAuditLogsCountAsync();
                var todayLogs = await _auditService.GetAuditLogsCountAsync(
                    fromDate: DateTime.UtcNow.Date,
                    toDate: DateTime.UtcNow.Date.AddDays(1)
                );

                return new { 
                    status = "healthy",
                    total = totalLogs,
                    today = todayLogs,
                    message = $"{totalLogs} logs totales, {todayLogs} hoy"
                };
            }
            catch (Exception ex)
            {
                return new { status = "unhealthy", message = ex.Message };
            }
        }

        #endregion
    }
}