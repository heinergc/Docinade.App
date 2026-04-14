using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RubricasApp.Web.Data;
using RubricasApp.Web.Models.Identity;
using RubricasApp.Web.Models.Permissions;
using RubricasApp.Web.Services.Permissions;
using RubricasApp.Web.Services;
using RubricasApp.Web.Services.Identity;
using RubricasApp.Web.ViewModels.Admin;
using RubricasApp.Web.Authorization;
using RubricasApp.Web.Extensions;
using System.Security.Claims;
using PermissionInfo = RubricasApp.Web.Models.Permissions.PermissionInfo;
using RubricasApp.Web.Services.Audit;



namespace RubricasApp.Web.Areas.Admin.Controllers
{
    /// <summary>
    /// Controlador para administración de usuarios
    /// </summary>
    [Authorize]
    [Area("Admin")]
    [Route("Admin/[controller]")]
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IPermissionService _permissionService;
        private readonly IAuditService _auditService;
        private readonly IEmailService _emailService;
        private readonly IUserContextService _userContextService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IPermissionService permissionService,
            IAuditService auditService,
            IEmailService emailService,
            IUserContextService userContextService,
            ILogger<UsersController> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _permissionService = permissionService;
            _auditService = auditService;
            _emailService = emailService;
            _userContextService = userContextService;
            _logger = logger;
        }

        /// <summary>
        /// Lista de usuarios con paginación y filtros
        /// </summary>
        [HttpGet]
        [RequirePermission(ApplicationPermissions.Usuarios.VER)]
        public async Task<IActionResult> Index(string? search, string? role, int page = 1, int pageSize = 20, bool? active = null)
        {
            try
            {
                var usersQuery = _userManager.Users.AsQueryable();

                // Aplicar filtros
                if (!string.IsNullOrEmpty(search))
                {
                    usersQuery = usersQuery.Where(u => 
                        u.Email!.Contains(search) || 
                        u.Nombre.Contains(search) ||
                        u.Apellidos.Contains(search));
                }

                if (active.HasValue)
                {
                    usersQuery = usersQuery.Where(u => u.IsActive == active.Value);
                }

                var totalUsers = await usersQuery.CountAsync();
                var users = await usersQuery
                    .OrderBy(u => u.Email)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var userViewModels = new List<UserViewModel>();

                foreach (var user in users)
                {
                    var userRoles = await _userManager.GetRolesAsync(user);
                    var userPermissions = await _permissionService.GetUserPermissionsAsync(user.Id);

                    userViewModels.Add(new UserViewModel
                    {
                        Id = user.Id,
                        Email = user.Email ?? string.Empty,
                        NombreCompleto = $"{user.Nombre} {user.Apellidos}",
                        Nombre = user.Nombre,
                        Apellido = user.Apellidos,
                        IsActive = user.IsActive,
                        Activo = user.Activo,
                        CreatedDate = user.CreatedDate,
                        LastLoginDate = user.LastLoginDate,
                        Roles = userRoles.ToList(),
                        PermissionCount = userPermissions.Count,
                        EmailConfirmed = user.EmailConfirmed,
                        PhoneNumber = user.PhoneNumber ?? "",
                        RoleCount = userRoles.Count
                    });
                }

                var viewModel = new UserListViewModel
                {
                    Users = userViewModels,
                    TotalUsers = totalUsers,
                    ActiveUsers = userViewModels.Count(u => u.IsActive),
                    InactiveUsers = userViewModels.Count(u => !u.IsActive),
                    LockedUsers = userViewModels.Count(u => u.LockoutEnd > DateTimeOffset.UtcNow)
                };

                // Cargar roles para filtro
                var availableRoles = await _roleManager.Roles.ToListAsync();
                viewModel.AvailableRoles = availableRoles.Select(r => new RoleSelectionItem
                {
                    Id = r.Id,
                    Name = r.Name!,
                    IsSelected = false
                }).ToList();

                await _auditService.LogAsync(
                    User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "Sistema",
                    "Usuarios",
                    "Ver",
                    null,
                    "Lista de usuarios vista"
                );

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar lista de usuarios");
                TempData["ErrorMessage"] = "Error al cargar la lista de usuarios.";
                return View(new UserListViewModel());
            }
        }

        /// <summary>
        /// Detalles de un usuario específico
        /// </summary>
        [HttpGet("Details/{id}")]
        [RequirePermission(ApplicationPermissions.Usuarios.VER)]
        public async Task<IActionResult> Details(string id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return NotFound("Usuario no encontrado.");
                }

                var userRoles = await _userManager.GetRolesAsync(user);
                var directPermissions = await _permissionService.GetUserDirectPermissionsAsync(user.Id);
                var rolePermissions = await _permissionService.GetUserRolePermissionsAsync(user.Id);
                var allPermissions = await _permissionService.GetUserPermissionsAsync(user.Id);

                var viewModel = new UserDetailsViewModel
                {
                    Id = user.Id,
                    Email = user.Email ?? string.Empty,
                    NombreCompleto = $"{user.Nombre} {user.Apellidos}",
                    Nombre = user.Nombre,
                    Apellido = user.Apellidos, // CORREGIDO: usar Apellidos en lugar de Apellido
                    IsActive = user.IsActive,
                    CreatedDate = user.CreatedDate,
                    LastLoginDate = user.LastLoginDate,
                    EmailConfirmed = user.EmailConfirmed,
                    PhoneNumber = user.PhoneNumber,
                    TwoFactorEnabled = user.TwoFactorEnabled,
                    LockoutEnd = user.LockoutEnd,
                    AccessFailedCount = user.AccessFailedCount,
                    Roles = userRoles.ToList(),
                    DirectPermissions = directPermissions,
                    RolePermissions = rolePermissions,
                    AllPermissions = allPermissions,
                    PermissionsByCategory = GroupPermissionsByCategory(allPermissions)
                };

                await _auditService.LogAsync(
                    User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "Sistema",
                    "Usuarios",
                    "Ver",
                    id,
                    $"Detalles de usuario visualizados: {user.Email}"
                );

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar detalles del usuario {UserId}", id);
                TempData["ErrorMessage"] = "Error al cargar los detalles del usuario.";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Formulario para crear nuevo usuario
        /// </summary>
        [HttpGet("Create")]
        [RequirePermission(ApplicationPermissions.Usuarios.CREAR)]
        public async Task<IActionResult> Create()
        {
            var viewModel = new CreateUserViewModel
            {
                AvailableRoles = await GetAvailableRoles(),
                AvailablePermissions = await GenerateAvailablePermissions()
            };

            return View(viewModel);
        }

        /// <summary>
        /// Crear nuevo usuario
        /// </summary>
        [HttpPost("Create")]
        [RequirePermission(ApplicationPermissions.Usuarios.CREAR)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.AvailableRoles = await GetAvailableRoles();
                model.AvailablePermissions = await GenerateAvailablePermissions();
                return View(model);
            }

            // Validación adicional de contraseña para mensajes más claros
            if (!IsValidPassword(model.Password, out var passwordErrors))
            {
                foreach (var error in passwordErrors)
                {
                    ModelState.AddModelError("Password", error);
                }
                model.AvailableRoles = await GetAvailableRoles();
                model.AvailablePermissions = await GenerateAvailablePermissions();
                return View(model);
            }

            try
            {
                // Verificar si el email ya existe
                var existingUser = await _userManager.FindByEmailAsync(model.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError("Email", "Ya existe un usuario con este email.");
                    model.AvailableRoles = await GetAvailableRoles();
                    model.AvailablePermissions = await GenerateAvailablePermissions();
                    return View(model);
                }

                // Obtener el usuario actual para auditoría
                var currentUserId = _userContextService.GetCurrentUserId();

                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    Nombre = model.Nombre,
                    Apellidos = model.Apellido,
                    NumeroIdentificacion = model.NumeroIdentificacion ?? "",
                    Institucion = model.Institucion ?? "",
                    Departamento = model.Departamento ?? "",
                    Activo = model.IsActive,
                    FechaRegistro = DateTime.Now,
                    EmailConfirmed = model.EmailConfirmed,
                    PhoneNumber = model.PhoneNumber,
                    TwoFactorEnabled = model.TwoFactorEnabled,
                    // Campos de auditoría heredados del sistema antiguo
                    IsActive = model.IsActive,
                    CreatedDate = DateTime.UtcNow
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // Asignar roles seleccionados
                    if (model.SelectedRoles?.Any() == true)
                    {
                        await _userManager.AddToRolesAsync(user, model.SelectedRoles);
                    }

                    // Asignar permisos directos
                    if (model.SelectedPermissions?.Any() == true)
                    {
                        foreach (var permission in model.SelectedPermissions)
                        {
                            await _permissionService.AssignPermissionToUserAsync(user.Id, permission);
                        }
                    }

                    // Enviar email de bienvenida
                    if (model.SendWelcomeEmail)
                    {
                        try
                        {
                            var confirmationLink = Url.Action("ConfirmEmail", "Account", 
                                new { userId = user.Id }, Request.Scheme) ?? "";
                            await _emailService.SendConfirmationEmailAsync(user.Email, confirmationLink);
                        }
                        catch (Exception emailEx)
                        {
                            _logger.LogWarning(emailEx, "No se pudo enviar email de bienvenida a {Email}", user.Email);
                        }
                    }

                    await _auditService.LogAsync(
                        User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "Sistema",
                        "Usuarios",
                        "Crear",
                        user.Id,
                        $"Usuario creado: {user.Email}"
                    );

                    TempData["SuccessMessage"] = $"Usuario {user.Email} creado exitosamente.";
                    return RedirectToAction(nameof(Details), new { id = user.Id });
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                model.AvailableRoles = await GetAvailableRoles();
                model.AvailablePermissions = await GenerateAvailablePermissions();
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear usuario");
                ModelState.AddModelError(string.Empty, "Error interno al crear el usuario.");
                model.AvailableRoles = await GetAvailableRoles();
                model.AvailablePermissions = await GenerateAvailablePermissions();
                return View(model);
            }
        }

        /// <summary>
        /// Formulario para editar usuario
        /// </summary>
        [HttpGet("Edit/{id}")]
        [RequirePermission(ApplicationPermissions.Usuarios.EDITAR)]
        public async Task<IActionResult> Edit(string id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return NotFound("Usuario no encontrado.");
                }

                var userRoles = await _userManager.GetRolesAsync(user);
                var userPermissions = await _permissionService.GetUserDirectPermissionsAsync(user.Id);

                var viewModel = new EditUserViewModel
                {
                    Id = user.Id,
                    Email = user.Email ?? string.Empty,
                    Nombre = user.Nombre,
                    Apellido = user.Apellido,
                    IsActive = user.IsActive,
                    EmailConfirmed = user.EmailConfirmed,
                    PhoneNumber = user.PhoneNumber ?? "",
                    TwoFactorEnabled = user.TwoFactorEnabled,
                    SelectedRoles = userRoles.ToList(),
                    SelectedPermissions = userPermissions,
                    AvailableRoles = await GetAvailableRoles(userRoles.ToList()),
                    AvailablePermissions = await GenerateAvailablePermissions(userPermissions)
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar formulario de edición para usuario {UserId}", id);
                TempData["ErrorMessage"] = "Error al cargar el formulario de edición.";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Actualizar usuario
        /// </summary>
        [HttpPost("Edit/{id}")]
        [RequirePermission(ApplicationPermissions.Usuarios.EDITAR)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, EditUserViewModel model)
        {
            if (id != model.Id)
            {
                return BadRequest("ID de usuario no coincide.");
            }

            if (!ModelState.IsValid)
            {
                model.AvailableRoles = await GetAvailableRoles(model.SelectedRoles);
                model.AvailablePermissions = await GenerateAvailablePermissions(model.SelectedPermissions);
                return View(model);
            }

            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return NotFound("Usuario no encontrado.");
                }

                // Actualizar propiedades básicas
                user.Email = model.Email;
                user.UserName = model.Email;
                user.Nombre = model.Nombre;
                user.Apellidos = model.Apellido;
                user.IsActive = model.IsActive;
                user.EmailConfirmed = model.EmailConfirmed;
                user.PhoneNumber = model.PhoneNumber;
                user.TwoFactorEnabled = model.TwoFactorEnabled;

                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    // Actualizar roles
                    var currentRoles = await _userManager.GetRolesAsync(user);
                    var newRoles = model.SelectedRoles ?? new List<string>();

                    var rolesToRemove = currentRoles.Except(newRoles).ToList();
                    if (rolesToRemove.Any())
                    {
                        await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
                    }

                    var rolesToAdd = newRoles.Except(currentRoles).ToList();
                    if (rolesToAdd.Any())
                    {
                        await _userManager.AddToRolesAsync(user, rolesToAdd);
                    }

                    // Actualizar permisos directos
                    var currentPermissions = await _permissionService.GetUserDirectPermissionsAsync(user.Id);
                    var newPermissions = model.SelectedPermissions ?? new List<string>();

                    var permissionsToRemove = currentPermissions.Except(newPermissions).ToList();
                    foreach (var permission in permissionsToRemove)
                    {
                        await _permissionService.RemovePermissionFromUserAsync(user.Id, permission);
                    }

                    var permissionsToAdd = newPermissions.Except(currentPermissions).ToList();
                    foreach (var permission in permissionsToAdd)
                    {
                        await _permissionService.AssignPermissionToUserAsync(user.Id, permission);
                    }

                    // Cambiar contraseña si se proporciona
                    if (!string.IsNullOrEmpty(model.NewPassword))
                    {
                        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                        await _userManager.ResetPasswordAsync(user, token, model.NewPassword);
                    }

                    await _auditService.LogAsync(
                        User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "Sistema",
                        "Usuarios",
                        "Editar",
                        user.Id,
                        $"Usuario actualizado: {user.Email}"
                    );

                    TempData["SuccessMessage"] = "Usuario actualizado exitosamente.";
                    return RedirectToAction(nameof(Details), new { id = user.Id });
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                model.AvailableRoles = await GetAvailableRoles(model.SelectedRoles);
                model.AvailablePermissions = await GenerateAvailablePermissions(model.SelectedPermissions);
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar usuario {UserId}", id);
                ModelState.AddModelError(string.Empty, "Error interno al actualizar el usuario.");
                
                model.AvailableRoles = await GetAvailableRoles(model.SelectedRoles);
                model.AvailablePermissions = await GenerateAvailablePermissions(model.SelectedPermissions);
                return View(model);
            }
        }

        /// <summary>
        /// Eliminar usuario
        /// </summary>
        [HttpPost("Delete/{id}")]
        [RequirePermission(ApplicationPermissions.Usuarios.ELIMINAR)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return Json(new { success = false, message = "Usuario no encontrado." });
                }

                // No permitir eliminar su propio usuario
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (user.Id == currentUserId)
                {
                    return Json(new { success = false, message = "No puede eliminar su propia cuenta." });
                }

                // Desactivar en lugar de eliminar
                user.IsActive = false;
                user.LockoutEnd = DateTimeOffset.MaxValue;

                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    await _auditService.LogAsync(
                        currentUserId ?? "Sistema",
                        "Usuarios",
                        "Eliminar",
                        user.Id,
                        $"Usuario desactivado: {user.Email}"
                    );

                    return Json(new { success = true, message = "Usuario desactivado exitosamente." });
                }

                return Json(new { success = false, message = "Error al desactivar el usuario." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar usuario {UserId}", id);
                return Json(new { success = false, message = "Error interno al eliminar el usuario." });
            }
        }

        /// <summary>
        /// Formulario para resetear/cambiar password de un usuario
        /// </summary>
        [HttpGet("reset-password/{id}")]
        [RequirePermission(ApplicationPermissions.Usuarios.EDITAR)]
        public async Task<IActionResult> ResetPassword(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var model = new ResetPasswordViewModel
            {
                UserId = user.Id,
                Email = user.Email!,
                NombreCompleto = $"{user.Nombre} {user.Apellidos}"
            };

            return View(model);
        }

        /// <summary>
        /// Procesa el reseteo/cambio de password
        /// </summary>
        [HttpPost("reset-password/{id}")]
        [ValidateAntiForgeryToken]
        [RequirePermission(ApplicationPermissions.Usuarios.EDITAR)]
        public async Task<IActionResult> ResetPassword(string id, ResetPasswordViewModel model)
        {
            if (id != model.UserId)
            {
                return BadRequest();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // Validar que las contraseñas coincidan
            if (model.NewPassword != model.ConfirmPassword)
            {
                ModelState.AddModelError(string.Empty, "La nueva contraseña y la confirmación no coinciden.");
                return View(model);
            }

            // Validar contraseña
            if (!IsValidPassword(model.NewPassword, out var passwordErrors))
            {
                foreach (var error in passwordErrors)
                {
                    ModelState.AddModelError(string.Empty, error);
                }
                return View(model);
            }

            try
            {
                // Remover password anterior
                var removeResult = await _userManager.RemovePasswordAsync(user);
                if (!removeResult.Succeeded)
                {
                    foreach (var error in removeResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(model);
                }

                // Agregar nueva password
                var addResult = await _userManager.AddPasswordAsync(user, model.NewPassword);
                if (!addResult.Succeeded)
                {
                    foreach (var error in addResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(model);
                }

                // Registrar auditoría
                var currentUserName = _userContextService.GetCurrentUser()?.Identity?.Name ?? "Sistema";
                await _auditService.LogActionAsync(
                    userId: _userContextService.GetCurrentUserId(),
                    action: "Resetear Password",
                    entityType: "Usuario",
                    entityId: user.Id,
                    additionalInfo: $"Password reseteada por {currentUserName} para usuario {user.Email}"
                );

                TempData["SuccessMessage"] = $"Password actualizada exitosamente para {user.Email}";
                return RedirectToAction(nameof(Edit), new { id = user.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al resetear password para usuario {UserId}", id);
                ModelState.AddModelError(string.Empty, "Ocurrió un error al resetear la password. Intente nuevamente.");
                return View(model);
            }
        }

        #region Métodos auxiliares

        /// <summary>
        /// Valida una contraseña según los criterios establecidos
        /// </summary>
        private bool IsValidPassword(string password, out List<string> errors)
        {
            errors = new List<string>();

            if (string.IsNullOrEmpty(password))
            {
                errors.Add("La contraseña es requerida.");
                return false;
            }

            if (password.Length < 6)
            {
                errors.Add("La contraseña debe tener al menos 6 caracteres.");
            }

            if (!password.Any(char.IsUpper))
            {
                errors.Add("La contraseña debe tener al menos una letra mayúscula ('A'-'Z').");
            }

            if (!password.Any(char.IsLower))
            {
                errors.Add("La contraseña debe tener al menos una letra minúscula ('a'-'z').");
            }

            if (!password.Any(char.IsDigit))
            {
                errors.Add("La contraseña debe tener al menos un número ('0'-'9').");
            }

            // Validar caracteres únicos
            if (password.Distinct().Count() < 1)
            {
                errors.Add("La contraseña debe tener al menos 1 caracter único.");
            }

            return errors.Count == 0;
        }

        /// <summary>
        /// Obtiene roles disponibles para selección
        /// </summary>
        private async Task<List<RoleSelectionItem>> GetAvailableRoles(List<string>? selectedRoles = null)
        {
            var roles = await _roleManager.Roles.ToListAsync();
            return roles.Select(r => new RoleSelectionItem
            {
                Id = r.Id,
                Name = r.Name!,
                IsSelected = selectedRoles?.Contains(r.Name!) ?? false
            }).OrderBy(r => r.Name).ToList();
        }

        /// <summary>
        /// Genera permisos disponibles para selección
        /// </summary>
        private async Task<List<PermissionSelectionItem>> GenerateAvailablePermissions(List<string>? selectedPermissions = null)
        {
            var permissions = new List<PermissionSelectionItem>();
            var permissionsByCategory = ApplicationPermissions.GetPermissionsByCategory();

            foreach (var category in permissionsByCategory)
            {
                foreach (var permission in category.Value)
                {
                    permissions.Add(new PermissionSelectionItem
                    {
                        Id = permission.Name,
                        Name = permission.Name,
                        DisplayName = permission.DisplayName,
                        Description = permission.Description,
                        Category = category.Key,
                        Module = category.Key,
                        IsSelected = selectedPermissions?.Contains(permission.Name) ?? false
                    });
                }
            }

            return permissions.OrderBy(p => p.Category).ThenBy(p => p.DisplayName).ToList();
        }

        /// <summary>
        /// Agrupa permisos por categoría
        /// </summary>
        private Dictionary<string, List<PermissionInfo>> GroupPermissionsByCategory(List<string> permissions)
        {
            var allPermissions = ApplicationPermissions.GetPermissionsByCategory();
            var result = new Dictionary<string, List<PermissionInfo>>();

            foreach (var category in allPermissions)
            {
                var categoryPermissions = category.Value
                    .Where(p => permissions.Contains(p.Name))
                    .ToList();

                if (categoryPermissions.Any())
                {
                    result[category.Key] = categoryPermissions;
                }
            }

            return result;
        }

        #endregion
    }
}