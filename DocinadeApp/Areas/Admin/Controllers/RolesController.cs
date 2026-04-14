using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RubricasApp.Web.Authorization;
using RubricasApp.Web.Data;
using RubricasApp.Web.Models;
using RubricasApp.Web.Models.Identity;
using RubricasApp.Web.Models.Permissions;
using RubricasApp.Web.Models.Audit;
using RubricasApp.Web.Services.Permissions;
using RubricasApp.Web.Services.Audit;
using RubricasApp.Web.ViewModels.Admin;
using System.Security.Claims;

namespace RubricasApp.Web.Areas.Admin.Controllers
{
    /// <summary>
    /// Controlador para administración de roles
    /// </summary>
    [Authorize]
    [Area("Admin")]
    [Route("Admin/[controller]")]
    public class RolesController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IPermissionService _permissionService;
        private readonly IAuditService _auditService;
        private readonly ILogger<RolesController> _logger;

        public RolesController(
            RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager,
            IPermissionService permissionService,
            IAuditService auditService,
            ILogger<RolesController> logger)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _permissionService = permissionService;
            _auditService = auditService;
            _logger = logger;
        }

        /// <summary>
        /// Lista de roles
        /// </summary>
        [HttpGet]
        //[RequirePermission(ApplicationPermissions.Configuracion.GESTIONAR_ROLES)]
        public async Task<IActionResult> Index()
        {
            try
            {
                var roles = await _roleManager.Roles.ToListAsync();
                var roleViewModels = new List<RoleViewModel>();

                foreach (var role in roles)
                {
                    var claims = await _roleManager.GetClaimsAsync(role);
                    var permissions = claims.Where(c => c.Type == "permission").Select(c => c.Value).ToList();
                    var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name!);

                    roleViewModels.Add(new RoleViewModel
                    {
                        Id = role.Id,
                        Name = role.Name!,
                        PermissionCount = permissions.Count,
                        UserCount = usersInRole.Count,
                        Permissions = permissions,
                        IsSystemRole = IsSystemRole(role.Name!),
                        CreatedDate = DateTime.UtcNow // TODO: Agregar fecha de creación al modelo si es necesario
                    });
                }

                await _auditService.LogAsync(
                    User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "Sistema",
                    "Roles",
                    "Ver",
                    null,
                    "Lista de roles consultada"
                );

                return View(roleViewModels);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar lista de roles");
                TempData["ErrorMessage"] = "Error al cargar la lista de roles.";
                return View(new List<RoleViewModel>());
            }
        }

        /// <summary>
        /// Formulario para crear nuevo rol
        /// </summary>
        [HttpGet("Create")]
        [RequirePermission(ApplicationPermissions.Configuracion.GESTIONAR_ROLES)]
        public IActionResult Create()
        {
            var model = new CreateRoleViewModel
            {
                AvailablePermissions = GenerateAvailablePermissions()
            };

            return View(model);
        }

        /// <summary>
        /// Crear nuevo rol
        /// </summary>
        [HttpPost("Create")]
        [RequirePermission(ApplicationPermissions.Configuracion.GESTIONAR_ROLES)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateRoleViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.AvailablePermissions = GenerateAvailablePermissions();
                return View(model);
            }

            try
            {
                // Verificar si el rol ya existe
                if (await _roleManager.RoleExistsAsync(model.Name))
                {
                    ModelState.AddModelError("Name", "Ya existe un rol con este nombre.");
                    model.AvailablePermissions = GenerateAvailablePermissions();
                    return View(model);
                }

                var role = new IdentityRole(model.Name);
                var result = await _roleManager.CreateAsync(role);

                if (result.Succeeded)
                {
                    // Asignar permisos seleccionados
                    if (model.SelectedPermissionIds?.Any() == true)
                    {
                        foreach (var permission in model.SelectedPermissionIds)
                        {
                            if (ApplicationPermissions.GetAllPermissions().Contains(permission))
                            {
                                await _roleManager.AddClaimAsync(role, new Claim("permission", permission));
                            }
                        }
                    }

                    await _auditService.LogAsync(
                        User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "Sistema",
                        "Roles",
                        "Crear",
                        role.Id,
                        $"Nombre: {role.Name}, Permisos: {string.Join(", ", model.SelectedPermissionIds ?? new List<string>())}"
                    );

                    TempData["SuccessMessage"] = $"Rol '{role.Name}' creado exitosamente.";
                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                model.AvailablePermissions = GenerateAvailablePermissions();
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear rol {RoleName}", model.Name);
                ModelState.AddModelError(string.Empty, "Error interno al crear el rol.");
                model.AvailablePermissions = GenerateAvailablePermissions();
                return View(model);
            }
        }

        /// <summary>
        /// Formulario para editar rol
        /// </summary>
        [HttpGet("Edit/{id}")]
        [RequirePermission(ApplicationPermissions.Configuracion.GESTIONAR_ROLES)]
        public async Task<IActionResult> Edit(string id)
        {
            try
            {
                var role = await _roleManager.FindByIdAsync(id);
                if (role == null)
                {
                    return NotFound("Rol no encontrado.");
                }

                var claims = await _roleManager.GetClaimsAsync(role);
                var currentPermissions = claims.Where(c => c.Type == "permission").Select(c => c.Value).ToList();

                var model = new EditRoleViewModel
                {
                    Id = role.Id,
                    Name = role.Name!,
                    SelectedPermissionIds = currentPermissions,
                    AvailablePermissions = GenerateAvailablePermissions(currentPermissions),
                    IsSystemRole = IsSystemRole(role.Name!)
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar formulario de edición para rol {RoleId}", id);
                TempData["ErrorMessage"] = "Error al cargar el formulario de edición.";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Actualizar rol
        /// </summary>
        [HttpPost("Edit/{id}")]
        [RequirePermission(ApplicationPermissions.Configuracion.GESTIONAR_ROLES)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, EditRoleViewModel model)
        {
            if (id != model.Id)
            {
                return BadRequest("ID de rol no coincide.");
            }

            if (!ModelState.IsValid)
            {
                model.AvailablePermissions = GenerateAvailablePermissions(model.SelectedPermissionIds);
                return View(model);
            }

            try
            {
                var role = await _roleManager.FindByIdAsync(id);
                if (role == null)
                {
                    return NotFound("Rol no encontrado.");
                }

                // No permitir editar roles del sistema
                if (IsSystemRole(role.Name!))
                {
                    TempData["ErrorMessage"] = "No se pueden editar los roles del sistema.";
                    return RedirectToAction(nameof(Index));
                }

                var oldValues = $"Nombre: {role.Name}";

                // Actualizar nombre del rol
                if (role.Name != model.Name)
                {
                    role.Name = model.Name;
                    role.NormalizedName = model.Name.ToUpperInvariant();
                    await _roleManager.UpdateAsync(role);
                }

                // Actualizar permisos
                var currentClaims = await _roleManager.GetClaimsAsync(role);
                var currentPermissions = currentClaims.Where(c => c.Type == "permission").ToList();

                // Remover permisos actuales
                if (currentPermissions.Any())
                {
                    foreach (var claim in currentPermissions)
                    {
                        await _roleManager.RemoveClaimAsync(role, claim);
                    }
                }

                // Agregar nuevos permisos
                if (model.SelectedPermissionIds?.Any() == true)
                {
                    foreach (var permission in model.SelectedPermissionIds)
                    {
                        if (ApplicationPermissions.GetAllPermissions().Contains(permission))
                        {
                            await _roleManager.AddClaimAsync(role, new Claim("permission", permission));
                        }
                    }
                }

                var newValues = $"Nombre: {role.Name}, Permisos: {string.Join(", ", model.SelectedPermissionIds ?? new List<string>())}";

                await _auditService.LogAsync(
                    User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "Sistema",
                    "Roles",
                    "Editar",
                    role.Id,
                    $"Rol actualizado: {role.Name}"
                );

                TempData["SuccessMessage"] = "Rol actualizado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar rol {RoleId}", id);
                ModelState.AddModelError(string.Empty, "Error interno al actualizar el rol.");
                model.AvailablePermissions = GenerateAvailablePermissions(model.SelectedPermissionIds);
                return View(model);
            }
        }

        /// <summary>
        /// Eliminar rol
        /// </summary>
        [HttpPost("Delete/{id}")]
        [RequirePermission(ApplicationPermissions.Configuracion.GESTIONAR_ROLES)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var role = await _roleManager.FindByIdAsync(id);
                if (role == null)
                {
                    return Json(new { success = false, message = "Rol no encontrado." });
                }

                // No permitir eliminar roles del sistema
                if (IsSystemRole(role.Name!))
                {
                    return Json(new { success = false, message = "No se pueden eliminar los roles del sistema." });
                }

                // Verificar si hay usuarios asignados al rol
                var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name!);
                if (usersInRole.Any())
                {
                    return Json(new { 
                        success = false, 
                        message = $"No se puede eliminar el rol porque hay {usersInRole.Count} usuario(s) asignado(s)." 
                    });
                }

                var result = await _roleManager.DeleteAsync(role);
                if (result.Succeeded)
                {
                    await _auditService.LogAsync(
                        User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "Sistema",
                        "Roles",
                        "Eliminar",
                        role.Id,
                        $"Rol eliminado: {role.Name}"
                    );

                    return Json(new { success = true, message = "Rol eliminado exitosamente." });
                }

                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return Json(new { success = false, message = $"Error al eliminar rol: {errors}" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar rol {RoleId}", id);
                return Json(new { success = false, message = "Error interno al eliminar el rol." });
            }
        }

        /// <summary>
        /// Obtener usuarios en un rol específico
        /// </summary>
        [HttpGet("GetRoleUsers/{id}")]
        [RequirePermission(ApplicationPermissions.Usuarios.VER)]
        public async Task<IActionResult> GetRoleUsers(string id)
        {
            try
            {
                var role = await _roleManager.FindByIdAsync(id);
                if (role == null)
                {
                    return Json(new { success = false, message = "Rol no encontrado." });
                }

                var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name!);
                var userList = usersInRole.Select(u => new UserInRoleViewModel
                {
                    Id = u.Id,
                    Email = u.Email ?? string.Empty,
                    NombreCompleto = u.NombreCompleto,
                    Institucion = u.Institucion,
                    Activo = u.Activo,
                    FechaRegistro = u.FechaRegistro,
                    UltimoAcceso = u.UltimoAcceso
                }).ToList();

                return Json(new { success = true, users = userList });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener usuarios del rol {RoleId}", id);
                return Json(new { success = false, message = "Error al obtener los usuarios del rol." });
            }
        }

        /// <summary>
        /// Sincronizar permisos de todos los roles con los permisos por defecto
        /// </summary>
        [HttpPost("SyncPermissions")]
        [RequirePermission(ApplicationPermissions.Configuracion.GESTIONAR_PERMISOS)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SyncPermissions()
        {
            try
            {
                // Llamar al método sin parámetros que sincroniza todos los roles
                await _permissionService.SyncRolePermissionsAsync();

                // Obtener el conteo de roles para el mensaje
                var roles = await _roleManager.Roles.ToListAsync();
                var syncedRoles = roles.Count;

                await _auditService.LogAsync(
                    User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "Sistema",
                    "Roles",
                    "SincronizarPermisos",
                    null,
                    $"Permisos sincronizados: {syncedRoles} roles actualizados"
                );

                return Json(new { 
                    success = true, 
                    message = $"Permisos sincronizados exitosamente para {syncedRoles} roles.",
                    syncedCount = syncedRoles
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al sincronizar permisos de roles");
                return Json(new { success = false, message = "Error al sincronizar permisos." });
            }
        }

        #region Métodos auxiliares

        /// <summary>
        /// Genera la lista de permisos disponibles para los ViewModels
        /// </summary>
        private static List<PermissionSelectionItem> GenerateAvailablePermissions(List<string>? selectedPermissions = null)
        {
            var permissionsByCategory = ApplicationPermissions.GetPermissionsByCategory();
            var availablePermissions = new List<PermissionSelectionItem>();
            
            foreach (var category in permissionsByCategory)
            {
                foreach (var permission in category.Value)
                {
                    availablePermissions.Add(new PermissionSelectionItem
                    {
                        Id = permission.Name, // Usamos el nombre como ID
                        Name = permission.Name,
                        DisplayName = permission.DisplayName,
                        Module = category.Key, // Agregamos la propiedad Module
                        Category = category.Key,
                        Description = permission.Description,
                        IsSelected = selectedPermissions?.Contains(permission.Name) == true
                    });
                }
            }
            
            return availablePermissions;
        }

        /// <summary>
        /// Determina si un rol es del sistema (no editable)
        /// </summary>
        private static bool IsSystemRole(string roleName)
        {
            return ApplicationRoles.IsSystemRole(roleName);
        }

        /// <summary>
        /// Sobrecarga para verificar si un rol (objeto IdentityRole) es del sistema
        /// </summary>
        private static bool IsSystemRole(IdentityRole role)
        {
            return ApplicationRoles.IsSystemRole(role.Name!);
        }

        #endregion
    }
}