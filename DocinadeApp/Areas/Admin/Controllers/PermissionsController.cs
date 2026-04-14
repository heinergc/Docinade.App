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
using System.Text;
using OfficeOpenXml;

namespace DocinadeApp.Areas.Admin.Controllers
{
    /// <summary>
    /// Controlador para administración de permisos
    /// </summary>
    [Authorize]
    [Area("Admin")]
    [Route("Admin/[controller]")]
    public class PermissionsController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IPermissionService _permissionService;
        private readonly IAuditService _auditService;
        private readonly ILogger<PermissionsController> _logger;
        //private readonly ApplicationRoles applicationRoles;

        public PermissionsController(
            RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager,
            IPermissionService permissionService,
            IAuditService auditService,
            ILogger<PermissionsController> logger)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _permissionService = permissionService;
            _auditService = auditService;
            _logger = logger;
        }

        /// <summary>
        /// Lista de permisos del sistema
        /// </summary>
        [HttpGet]
        [RequirePermission(ApplicationPermissions.Configuracion.GESTIONAR_PERMISOS)]
        public async Task<IActionResult> Index()
        {
            try
            {
                var permissionsByCategory = ApplicationPermissions.GetPermissionsByCategory();
                var allRoles = await _roleManager.Roles.ToListAsync();

                var permissionViewModels = new List<PermissionViewModel>();

                foreach (var category in permissionsByCategory)
                {
                    foreach (var permission in category.Value)
                    {
                        var rolesWithPermission = new List<string>();

                        // Verificar qué roles tienen este permiso
                        foreach (var role in allRoles)
                        {
                            var claims = await _roleManager.GetClaimsAsync(role);
                            if (claims.Any(c => c.Type == "permission" && c.Value == permission.Name))
                            {
                                rolesWithPermission.Add(role.Name!);
                            }
                        }

                        permissionViewModels.Add(new PermissionViewModel
                        {
                            Name = permission.Name,
                            DisplayName = permission.DisplayName,
                            Description = permission.Description,
                            Category = category.Key,
                            Module = category.Key,
                            AssignedRoles = rolesWithPermission,
                            RoleCount = rolesWithPermission.Count,
                            IsSystemPermission = true // Todos los permisos definidos en código son del sistema
                        });
                    }
                }

                var model = new PermissionListViewModel
                {
                    Permissions = permissionViewModels.OrderBy(p => p.Category).ThenBy(p => p.DisplayName).ToList(),
                    TotalPermissions = permissionViewModels.Count,
                    Categories = permissionsByCategory.Keys.ToList(),
                    Modules = permissionsByCategory.Keys.ToList() // AGREGAR ESTA LÍNEA
                };

                await _auditService.LogAsync(
                    User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "Sistema",
                    "Permisos",
                    "Ver",
                    null,
                    "Lista de permisos consultada"
                );

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar lista de permisos");
                TempData["ErrorMessage"] = "Error al cargar la lista de permisos.";
                return View(new PermissionListViewModel());
            }
        }

        /// <summary>
        /// Ver detalles de un permiso específico
        /// </summary>
        [HttpGet("Details/{permissionName}")]
        [RequirePermission(ApplicationPermissions.Configuracion.GESTIONAR_PERMISOS)]
        public async Task<IActionResult> Details(string permissionName)
        {
            try
            {
                var allPermissions = ApplicationPermissions.GetAllPermissionsList();
                var permission = allPermissions.FirstOrDefault(p => p.Name == permissionName);

                if (permission == null)
                {
                    return NotFound("Permiso no encontrado.");
                }

                var allRoles = await _roleManager.Roles.ToListAsync();
                var rolesWithPermission = new List<RolePermissionViewModel>();
                var usersWithPermission = new List<UserPermissionViewModel>();

                // Obtener roles que tienen este permiso
                foreach (var role in allRoles)
                {
                    var claims = await _roleManager.GetClaimsAsync(role);
                    var hasPermission = claims.Any(c => c.Type == "permission" && c.Value == permissionName);

                    if (hasPermission)
                    {
                        var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name!);
                        rolesWithPermission.Add(new RolePermissionViewModel
                        {
                            RoleId = role.Id,
                            RoleName = role.Name!,
                            UserCount = usersInRole.Count,
                            IsSystemRole = IsSystemRole(role.Name!)
                        });

                        // Agregar usuarios de este rol
                        foreach (var user in usersInRole)
                        {
                            if (!usersWithPermission.Any(u => u.UserId == user.Id))
                            {
                                usersWithPermission.Add(new UserPermissionViewModel
                                {
                                    UserId = user.Id,
                                    Email = user.Email ?? string.Empty,
                                    NombreCompleto = user.NombreCompleto,
                                    Activo = user.Activo,
                                    ThroughRole = role.Name!
                                });
                            }
                        }
                    }
                }

                var model = new PermissionDetailsViewModel
                {
                    Name = permission.Name,
                    DisplayName = permission.DisplayName,
                    Description = permission.Description,
                    Category = GetPermissionCategory(permission.Name),
                    Module = GetPermissionCategory(permission.Name),
                    RolesWithPermission = rolesWithPermission,
                    UsersWithPermission = usersWithPermission.OrderBy(u => u.Email).ToList(),
                    TotalRoles = rolesWithPermission.Count,
                    TotalUsers = usersWithPermission.Count,
                    IsSystemPermission = true
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar detalles del permiso {PermissionName}", permissionName);
                TempData["ErrorMessage"] = "Error al cargar los detalles del permiso.";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Página para seleccionar rol al que asignar permisos
        /// </summary>
        [HttpGet("SelectRoleForAssignment")]
        [RequirePermission(ApplicationPermissions.Configuracion.GESTIONAR_PERMISOS)]
        public async Task<IActionResult> SelectRoleForAssignment()
        {
            try
            {
                var allRoles = await _roleManager.Roles.OrderBy(r => r.Name).ToListAsync();
                var roleViewModels = new List<RolePermissionViewModel>();

                foreach (var role in allRoles)
                {
                    var claims = await _roleManager.GetClaimsAsync(role);
                    var permissionCount = claims.Count(c => c.Type == "permission");
                    var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name!);

                    roleViewModels.Add(new RolePermissionViewModel
                    {
                        RoleId = role.Id,
                        RoleName = role.Name!,
                        UserCount = usersInRole.Count,
                        IsSystemRole = IsSystemRole(role.Name!)
                    });
                }

                var model = new RoleSelectionViewModel
                {
                    AvailableRoles = roleViewModels
                };

                return View("SelectRole", model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar página de selección de rol");
                TempData["ErrorMessage"] = "Error al cargar la página de selección de rol.";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Página para asignar permisos a un rol específico
        /// </summary>
        [HttpGet("AssignToRole/{roleId}")]
        [RequirePermission(ApplicationPermissions.Configuracion.GESTIONAR_PERMISOS)]
        public async Task<IActionResult> AssignToRolePage(string roleId)
        {
            try
            {
                var role = await _roleManager.FindByIdAsync(roleId);
                if (role == null)
                {
                    TempData["ErrorMessage"] = "El rol especificado no fue encontrado.";
                    return RedirectToAction(nameof(Index));
                }

                // Obtener todos los permisos organizados por categoría
                var permissionsByCategory = ApplicationPermissions.GetPermissionsByCategory();
                var currentClaims = await _roleManager.GetClaimsAsync(role);
                var currentPermissions = currentClaims
                    .Where(c => c.Type == "permission")
                    .Select(c => c.Value)
                    .ToList();

                var permissionSelectionsByCategory = new Dictionary<string, List<PermissionSelectionItem>>();

                foreach (var category in permissionsByCategory)
                {
                    var permissionItems = category.Value.Select(p => new PermissionSelectionItem
                    {
                        Id = p.Name,
                        Name = p.Name,
                        DisplayName = p.DisplayName,
                        Category = category.Key,
                        Module = category.Key,
                        Description = p.Description,
                        IsSelected = currentPermissions.Contains(p.Name),
                        IsSystemPermission = true
                    }).ToList();

                    permissionSelectionsByCategory[category.Key] = permissionItems;
                }

                var model = new AssignPermissionsToRoleViewModel
                {
                    RoleId = role.Id,
                    RoleName = role.Name!,
                    PermissionsByCategory = permissionSelectionsByCategory,
                    CurrentPermissions = currentPermissions,
                    SelectedPermissions = currentPermissions
                };

                await _auditService.LogAsync(
                    User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "Sistema",
                    "Permisos",
                    "Ver Asignación",
                    null,
                    $"Página de asignación de permisos consultada para rol '{role.Name}'"
                );

                return View("AssignToRole", model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar página de asignación de permisos para rol {RoleId}", roleId);
                TempData["ErrorMessage"] = "Error al cargar la página de asignación de permisos.";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Procesar asignación masiva de permisos a un rol
        /// </summary>
        [HttpPost("AssignToRole/{roleId}")]
        [RequirePermission(ApplicationPermissions.Configuracion.GESTIONAR_PERMISOS)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignToRole(string roleId, AssignPermissionsToRoleViewModel model)
        {
            try
            {
                var role = await _roleManager.FindByIdAsync(roleId);
                if (role == null)
                {
                    return Json(new { success = false, message = "Rol no encontrado." });
                }

                // Obtener permisos actuales
                var currentClaims = await _roleManager.GetClaimsAsync(role);
                var currentPermissions = currentClaims
                    .Where(c => c.Type == "permission")
                    .Select(c => c.Value)
                    .ToHashSet();

                var selectedPermissions = model.SelectedPermissions?.ToHashSet() ?? new HashSet<string>();
                var validPermissions = ApplicationPermissions.GetAllPermissions();

                // Validar que todos los permisos seleccionados sean válidos
                var invalidPermissions = selectedPermissions.Except(validPermissions).ToList();
                if (invalidPermissions.Any())
                {
                    return Json(new { success = false, message = $"Permisos inválidos: {string.Join(", ", invalidPermissions)}" });
                }

                var permissionsToAdd = selectedPermissions.Except(currentPermissions).ToList();
                var permissionsToRemove = currentPermissions.Except(selectedPermissions).ToList();

                var addedCount = 0;
                var removedCount = 0;
                var errors = new List<string>();

                // Agregar nuevos permisos
                foreach (var permission in permissionsToAdd)
                {
                    var result = await _roleManager.AddClaimAsync(role, new Claim("permission", permission));
                    if (result.Succeeded)
                    {
                        addedCount++;
                    }
                    else
                    {
                        errors.AddRange(result.Errors.Select(e => e.Description));
                    }
                }

                // Remover permisos no seleccionados
                foreach (var permission in permissionsToRemove)
                {
                    var claimToRemove = currentClaims.FirstOrDefault(c => c.Type == "permission" && c.Value == permission);
                    if (claimToRemove != null)
                    {
                        var result = await _roleManager.RemoveClaimAsync(role, claimToRemove);
                        if (result.Succeeded)
                        {
                            removedCount++;
                        }
                        else
                        {
                            errors.AddRange(result.Errors.Select(e => e.Description));
                        }
                    }
                }

                await _auditService.LogAsync(
                    User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "Sistema",
                    "Permisos",
                    "Asignación Masiva",
                    null,
                    $"Permisos asignados masivamente al rol '{role.Name}': {addedCount} agregados, {removedCount} removidos"
                );

                if (errors.Any())
                {
                    return Json(new { success = false, message = $"Operación completada con errores: {string.Join(", ", errors)}" });
                }

                return Json(new
                {
                    success = true,
                    message = $"Permisos actualizados exitosamente. {addedCount} agregados, {removedCount} removidos.",
                    added = addedCount,
                    removed = removedCount
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al asignar permisos masivamente al rol {RoleId}", roleId);
                return Json(new { success = false, message = "Error interno al asignar permisos." });
            }
        }

        /// <summary>
        /// Asignar permiso a rol
        /// </summary>
        [HttpPost("AssignToRole")]
        [RequirePermission(ApplicationPermissions.Configuracion.GESTIONAR_PERMISOS)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignToRole(string permissionName, string roleId)
        {
            try
            {
                var role = await _roleManager.FindByIdAsync(roleId);
                if (role == null)
                {
                    return Json(new { success = false, message = "Rol no encontrado." });
                }

                // Verificar que el permiso existe
                if (!ApplicationPermissions.GetAllPermissions().Contains(permissionName))
                {
                    return Json(new { success = false, message = "Permiso no válido." });
                }

                // Verificar si ya tiene el permiso
                var claims = await _roleManager.GetClaimsAsync(role);
                if (claims.Any(c => c.Type == "permission" && c.Value == permissionName))
                {
                    return Json(new { success = false, message = "El rol ya tiene este permiso asignado." });
                }

                // Asignar el permiso
                var result = await _roleManager.AddClaimAsync(role, new Claim("permission", permissionName));

                if (result.Succeeded)
                {
                    await _auditService.LogAsync(
                        User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "Sistema",
                        "Permisos",
                        "Asignar",
                        null,
                        $"Permiso '{permissionName}' asignado al rol '{role.Name}'"
                    );

                    return Json(new { success = true, message = "Permiso asignado exitosamente." });
                }

                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return Json(new { success = false, message = $"Error al asignar permiso: {errors}" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al asignar permiso {PermissionName} al rol {RoleId}", permissionName, roleId);
                return Json(new { success = false, message = "Error interno al asignar el permiso." });
            }
        }

        /// <summary>
        /// Remover permiso de rol
        /// </summary>
        [HttpPost("RemoveFromRole")]
        [RequirePermission(ApplicationPermissions.Configuracion.GESTIONAR_PERMISOS)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveFromRole(string permissionName, string roleId)
        {
            try
            {
                var role = await _roleManager.FindByIdAsync(roleId);
                if (role == null)
                {
                    return Json(new { success = false, message = "Rol no encontrado." });
                }

                // Buscar el claim del permiso
                var claims = await _roleManager.GetClaimsAsync(role);
                var permissionClaim = claims.FirstOrDefault(c => c.Type == "permission" && c.Value == permissionName);

                if (permissionClaim == null)
                {
                    return Json(new { success = false, message = "El rol no tiene este permiso asignado." });
                }

                // Remover el permiso
                var result = await _roleManager.RemoveClaimAsync(role, permissionClaim);

                if (result.Succeeded)
                {
                    await _auditService.LogAsync(
                        User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "Sistema",
                        "Permisos",
                        "Remover",
                        null,
                        $"Permiso '{permissionName}' removido del rol '{role.Name}'"
                    );

                    return Json(new { success = true, message = "Permiso removido exitosamente." });
                }

                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return Json(new { success = false, message = $"Error al remover permiso: {errors}" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al remover permiso {PermissionName} del rol {RoleId}", permissionName, roleId);
                return Json(new { success = false, message = "Error interno al remover el permiso." });
            }
        }

        /// <summary>
        /// Página para sincronizar permisos del sistema
        /// </summary>
        [HttpGet("Sync")]
        [RequirePermission(ApplicationPermissions.Configuracion.GESTIONAR_PERMISOS)]
        public IActionResult SyncPage()
        {
            return View();
        }

        /// <summary>
        /// Sincronizar permisos del sistema
        /// </summary>
        [HttpPost("Sync")]
        [RequirePermission(ApplicationPermissions.Configuracion.GESTIONAR_PERMISOS)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Sync()
        {
            try
            {
                await _permissionService.SyncRolePermissionsAsync();

                await _auditService.LogAsync(
                    User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "Sistema",
                    "Permisos",
                    "Sincronizar",
                    null,
                    "Permisos del sistema sincronizados"
                );

                TempData["SuccessMessage"] = "Permisos sincronizados exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al sincronizar permisos");
                TempData["ErrorMessage"] = "Error al sincronizar permisos.";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Exportar matriz de permisos
        /// </summary>
        [HttpGet("ExportMatrix")]
        [RequirePermission(ApplicationPermissions.Configuracion.GESTIONAR_PERMISOS)]
        public async Task<IActionResult> ExportMatrix()
        {
            try
            {
                var permissionsByCategory = ApplicationPermissions.GetPermissionsByCategory();
                var allRoles = await _roleManager.Roles.OrderBy(r => r.Name).ToListAsync();

                var matrix = new List<PermissionMatrixRow>();

                foreach (var category in permissionsByCategory)
                {
                    foreach (var permission in category.Value)
                    {
                        var row = new PermissionMatrixRow
                        {
                            Category = category.Key,
                            Permission = permission.DisplayName,
                            PermissionName = permission.Name,
                            Description = permission.Description,
                            RoleAssignments = new Dictionary<string, bool>()
                        };

                        foreach (var role in allRoles)
                        {
                            var claims = await _roleManager.GetClaimsAsync(role);
                            var hasPermission = claims.Any(c => c.Type == "permission" && c.Value == permission.Name);
                            row.RoleAssignments[role.Name!] = hasPermission;
                        }

                        matrix.Add(row);
                    }
                }

                var model = new PermissionMatrixViewModel
                {
                    Matrix = matrix,
                    Roles = allRoles.Select(r => r.Name!).ToList(),
                    GeneratedDate = DateTime.Now
                };

                return View("Matrix", model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar matriz de permisos");
                TempData["ErrorMessage"] = "Error al generar la matriz de permisos.";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Exportar matriz de permisos a Excel desde el controlador
        /// </summary>
        [HttpGet("ExportToExcel")]
        [RequirePermission(ApplicationPermissions.Configuracion.GESTIONAR_PERMISOS)]
        public async Task<IActionResult> ExportToExcel()
        {
            try
            {
                var permissionsByCategory = ApplicationPermissions.GetPermissionsByCategory();
                var allRoles = await _roleManager.Roles.OrderBy(r => r.Name).ToListAsync();

                var matrix = new List<PermissionMatrixRow>();

                foreach (var category in permissionsByCategory)
                {
                    foreach (var permission in category.Value)
                    {
                        var matrixRow = new PermissionMatrixRow
                        {
                            Category = category.Key,
                            Permission = permission.DisplayName,
                            PermissionName = permission.Name,
                            Description = permission.Description,
                            RoleAssignments = new Dictionary<string, bool>()
                        };

                        foreach (var role in allRoles)
                        {
                            var claims = await _roleManager.GetClaimsAsync(role);
                            var hasPermission = claims.Any(c => c.Type == "permission" && c.Value == permission.Name);
                            matrixRow.RoleAssignments[role.Name!] = hasPermission;
                        }

                        matrix.Add(matrixRow);
                    }
                }

                // Generar archivo Excel usando EPPlus
                using var package = new OfficeOpenXml.ExcelPackage();
                var worksheet = package.Workbook.Worksheets.Add("Matriz de Permisos");

                // Configurar headers
                worksheet.Cells[1, 1].Value = "Categoría";
                worksheet.Cells[1, 2].Value = "Permiso";
                worksheet.Cells[1, 3].Value = "Descripción";

                int col = 4;
                foreach (var role in allRoles.OrderBy(r => r.Name))
                {
                    worksheet.Cells[1, col].Value = role.Name;
                    col++;
                }

                // Estilo para headers
                using (var range = worksheet.Cells[1, 1, 1, col - 1])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Navy);
                    range.Style.Font.Color.SetColor(System.Drawing.Color.White);
                    range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                }

                // Llenar datos
                int row = 2;
                foreach (var permission in matrix.OrderBy(m => m.Category).ThenBy(m => m.Permission))
                {
                    worksheet.Cells[row, 1].Value = permission.Category;
                    worksheet.Cells[row, 2].Value = permission.Permission;
                    worksheet.Cells[row, 3].Value = permission.Description;

                    col = 4;
                    foreach (var role in allRoles.OrderBy(r => r.Name))
                    {
                        var hasPermission = permission.RoleAssignments.ContainsKey(role.Name!) &&
                                           permission.RoleAssignments[role.Name!];
                        worksheet.Cells[row, col].Value = hasPermission ? "SÍ" : "NO";

                        // Colorear celda según el valor
                        if (hasPermission)
                        {
                            worksheet.Cells[row, col].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            worksheet.Cells[row, col].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGreen);
                        }

                        col++;
                    }
                    row++;
                }

                // Ajustar ancho de columnas
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                worksheet.Column(1).Width = 20; // Categoría
                worksheet.Column(2).Width = 35; // Permiso
                worksheet.Column(3).Width = 50; // Descripción

                // Agregar información del reporte
                worksheet.Cells[row + 2, 1].Value = "Generado el:";
                worksheet.Cells[row + 2, 2].Value = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                worksheet.Cells[row + 3, 1].Value = "Total Permisos:";
                worksheet.Cells[row + 3, 2].Value = matrix.Count;
                worksheet.Cells[row + 4, 1].Value = "Total Roles:";
                worksheet.Cells[row + 4, 2].Value = allRoles.Count;

                // Generar el archivo
                var fileName = $"Matriz_Permisos_{DateTime.Now:yyyy-MM-dd}.xlsx";
                var fileBytes = package.GetAsByteArray();

                await _auditService.LogAsync(
                    User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "Sistema",
                    "Permisos",
                    "Exportar Excel",
                    null,
                    $"Matriz de permisos exportada a Excel: {fileName}"
                );

                return File(fileBytes,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al exportar matriz de permisos a Excel");
                TempData["ErrorMessage"] = "Error al exportar la matriz de permisos.";
                return RedirectToAction(nameof(ExportMatrix));
            }
        }

        /// <summary>
        /// Generar PDF para impresión de la matriz de permisos
        /// </summary>
        [HttpGet("PrintMatrix")]
        [RequirePermission(ApplicationPermissions.Configuracion.GESTIONAR_PERMISOS)]
        public async Task<IActionResult> PrintMatrix()
        {
            try
            {
                var permissionsByCategory = ApplicationPermissions.GetPermissionsByCategory();
                var allRoles = await _roleManager.Roles.OrderBy(r => r.Name).ToListAsync();

                var matrix = new List<PermissionMatrixRow>();

                foreach (var category in permissionsByCategory)
                {
                    foreach (var permission in category.Value)
                    {
                        var row = new PermissionMatrixRow
                        {
                            Category = category.Key,
                            Permission = permission.DisplayName,
                            PermissionName = permission.Name,
                            Description = permission.Description,
                            RoleAssignments = new Dictionary<string, bool>()
                        };

                        foreach (var role in allRoles)
                        {
                            var claims = await _roleManager.GetClaimsAsync(role);
                            var hasPermission = claims.Any(c => c.Type == "permission" && c.Value == permission.Name);
                            row.RoleAssignments[role.Name!] = hasPermission;
                        }

                        matrix.Add(row);
                    }
                }

                var model = new PermissionMatrixViewModel
                {
                    Matrix = matrix,
                    Roles = allRoles.Select(r => r.Name!).ToList(),
                    GeneratedDate = DateTime.Now
                };

                await _auditService.LogAsync(
                    User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "Sistema",
                    "Permisos",
                    "Imprimir Matriz",
                    null,
                    "Matriz de permisos preparada para impresión"
                );

                return View("PrintMatrix", model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al preparar matriz para impresión");
                TempData["ErrorMessage"] = "Error al preparar la matriz para impresión.";
                return RedirectToAction(nameof(ExportMatrix));
            }
        }

        /// <summary>
        /// Exportar matriz de permisos a CSV
        /// </summary>
        [HttpGet("ExportToCsv")]
        [RequirePermission(ApplicationPermissions.Configuracion.GESTIONAR_PERMISOS)]
        public async Task<IActionResult> ExportToCsv()
        {
            try
            {
                var permissionsByCategory = ApplicationPermissions.GetPermissionsByCategory();
                var allRoles = await _roleManager.Roles.OrderBy(r => r.Name).ToListAsync();

                var matrix = new List<PermissionMatrixRow>();

                foreach (var category in permissionsByCategory)
                {
                    foreach (var permission in category.Value)
                    {
                        var csvRow = new PermissionMatrixRow
                        {
                            Category = category.Key,
                            Permission = permission.DisplayName,
                            PermissionName = permission.Name,
                            Description = permission.Description,
                            RoleAssignments = new Dictionary<string, bool>()
                        };

                        foreach (var role in allRoles)
                        {
                            var claims = await _roleManager.GetClaimsAsync(role);
                            var hasPermission = claims.Any(c => c.Type == "permission" && c.Value == permission.Name);
                            csvRow.RoleAssignments[role.Name!] = hasPermission;
                        }

                        matrix.Add(csvRow);
                    }
                }

                // Generar contenido CSV
                var csvContent = new StringBuilder();
                
                // Agregar encabezado
                var headers = new List<string> { "Categoría", "Permiso", "Descripción" };
                headers.AddRange(allRoles.OrderBy(r => r.Name).Select(r => r.Name!));
                csvContent.AppendLine(string.Join(",", headers.Select(EscapeCsvField)));

                // Agregar datos
                foreach (var permission in matrix.OrderBy(m => m.Category).ThenBy(m => m.Permission))
                {
                    var row = new List<string>
                    {
                        permission.Category,
                        permission.Permission,
                        permission.Description ?? ""
                    };

                    foreach (var role in allRoles.OrderBy(r => r.Name))
                    {
                        var hasPermission = permission.RoleAssignments.ContainsKey(role.Name!) && 
                                           permission.RoleAssignments[role.Name!];
                        row.Add(hasPermission ? "SÍ" : "NO");
                    }

                    csvContent.AppendLine(string.Join(",", row.Select(EscapeCsvField)));
                }

                // Agregar información adicional al final
                csvContent.AppendLine();
                csvContent.AppendLine($"\"Generado el:\",\"{DateTime.Now:dd/MM/yyyy HH:mm:ss}\"");
                csvContent.AppendLine($"\"Total Permisos:\",\"{matrix.Count}\"");
                csvContent.AppendLine($"\"Total Roles:\",\"{allRoles.Count}\"");

                // Generar nombre del archivo
                var fileName = $"Matriz_Permisos_{DateTime.Now:yyyy-MM-dd}.csv";
                var fileBytes = Encoding.UTF8.GetBytes(csvContent.ToString());

                await _auditService.LogAsync(
                    User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "Sistema",
                    "Permisos",
                    "Exportar CSV",
                    null,
                    $"Matriz de permisos exportada a CSV: {fileName}"
                );

                return File(fileBytes, "text/csv", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al exportar matriz de permisos a CSV");
                TempData["ErrorMessage"] = "Error al exportar la matriz de permisos a CSV.";
                return RedirectToAction(nameof(ExportMatrix));
            }
        }

        /// <summary>
        /// Escapa un campo para formato CSV
        /// </summary>
        private static string EscapeCsvField(string field)
        {
            if (string.IsNullOrEmpty(field))
                return "\"\"";

            // Si el campo contiene comillas, comas o saltos de línea, debe ir entre comillas
            if (field.Contains(",") || field.Contains("\"") || field.Contains("\n") || field.Contains("\r"))
            {
                // Escapar comillas duplicándolas
                field = field.Replace("\"", "\"\"");
                return $"\"{field}\"";
            }

            return field;
        }

        /// <summary>
        /// Obtiene la categoría de un permiso
        /// </summary>
        private static string GetPermissionCategory(string permissionName)
        {
            var permissionsByCategory = ApplicationPermissions.GetPermissionsByCategory();

            foreach (var category in permissionsByCategory)
            {
                if (category.Value.Any(p => p.Name == permissionName))
                {
                    return category.Key;
                }
            }

            return "Sin Categoría";
        }

        /// <summary>
        /// Determina si un rol es del sistema
        /// </summary>
        private static bool IsSystemRole(string roleName)
        {
            return ApplicationRoles.IsSystemRole(roleName);
        }
    }
}