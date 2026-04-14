using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RubricasApp.Web.Data;
using RubricasApp.Web.Models.Identity;
using RubricasApp.Web.Models.Permissions;
using System.Security.Claims;

namespace RubricasApp.Web.Services.Permissions
{
    /// <summary>
    /// Implementación del servicio de permisos
    /// </summary>
    public class PermissionService : IPermissionService
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RubricasDbContext _context;
        private readonly ILogger<PermissionService> _logger;

        public PermissionService(
            RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager,
            RubricasDbContext context,
            ILogger<PermissionService> logger)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _context = context;
            _logger = logger;
        }

        public async Task InitializeDefaultRolesAndPermissionsAsync()
        {
            try
            {
                // Crear roles si no existen
                var roles = new[]
                {
                    ApplicationRoles.SuperAdministrador,
                    ApplicationRoles.Administrador,
                    ApplicationRoles.Coordinador,
                    ApplicationRoles.Docente,
                    ApplicationRoles.Evaluador
                };

                foreach (var roleName in roles)
                {
                    if (!await _roleManager.RoleExistsAsync(roleName))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(roleName));
                        _logger.LogInformation($"Rol creado: {roleName}");
                    }
                }

                // Asignar permisos por defecto a SuperAdministrador
                await AssignAllPermissionsToSuperAdminAsync();

                _logger.LogInformation("Roles y permisos inicializados correctamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al inicializar roles y permisos.");
                throw;
            }
        }

        public async Task<List<string>> GetAllPermissionsAsync()
        {
            // Usar el método estático de ApplicationPermissions
            return ApplicationPermissions.GetAllPermissions();
        }

        public async Task<List<string>> GetUserPermissionsAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return new List<string>();

            var permissions = new List<string>();

            // Obtener permisos directos del usuario
            var userClaims = await _userManager.GetClaimsAsync(user);
            permissions.AddRange(userClaims
                .Where(c => c.Type == "permission")
                .Select(c => c.Value));

            // Obtener permisos a través de roles
            var userRoles = await _userManager.GetRolesAsync(user);
            foreach (var roleName in userRoles)
            {
                var rolePermissions = await GetRolePermissionsAsync(roleName);
                permissions.AddRange(rolePermissions);
            }

            return permissions.Distinct().ToList();
        }

        public async Task<List<string>> GetRolePermissionsAsync(string roleName)
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role == null) return new List<string>();

            var roleClaims = await _roleManager.GetClaimsAsync(role);
            return roleClaims
                .Where(c => c.Type == "permission")
                .Select(c => c.Value)
                .ToList();
        }

        public async Task<bool> UserHasPermissionAsync(string userId, string permission)
        {
            var userPermissions = await GetUserPermissionsAsync(userId);
            return userPermissions.Contains(permission);
        }

        public async Task<bool> AssignPermissionToRoleAsync(string roleName, string permission)
        {
            try
            {
                var role = await _roleManager.FindByNameAsync(roleName);
                if (role == null) return false;

                var existingClaims = await _roleManager.GetClaimsAsync(role);
                if (existingClaims.Any(c => c.Type == "permission" && c.Value == permission))
                {
                    return true; // Ya existe
                }

                var result = await _roleManager.AddClaimAsync(role, new Claim("permission", permission));
                return result.Succeeded;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error asignando permiso {permission} al rol {roleName}");
                return false;
            }
        }

        public async Task<bool> RemovePermissionFromRoleAsync(string roleName, string permission)
        {
            try
            {
                var role = await _roleManager.FindByNameAsync(roleName);
                if (role == null) return false;

                var result = await _roleManager.RemoveClaimAsync(role, new Claim("permission", permission));
                return result.Succeeded;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error removiendo permiso {permission} del rol {roleName}");
                return false;
            }
        }

        public async Task<bool> AssignPermissionsToRoleAsync(string roleName, IEnumerable<string> permissions)
        {
            try
            {
                var role = await _roleManager.FindByNameAsync(roleName);
                if (role == null) return false;

                // Remover permisos existentes
                var existingClaims = await _roleManager.GetClaimsAsync(role);
                var permissionClaims = existingClaims.Where(c => c.Type == "permission").ToList();
                
                foreach (var claim in permissionClaims)
                {
                    await _roleManager.RemoveClaimAsync(role, claim);
                }

                // Agregar nuevos permisos
                foreach (var permission in permissions)
                {
                    await _roleManager.AddClaimAsync(role, new Claim("permission", permission));
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error asignando permisos al rol {roleName}");
                return false;
            }
        }

        public async Task<Dictionary<string, List<string>>> GetPermissionsByCategoryAsync()
        {
            // Usar el método estático de ApplicationPermissions
            var permissionsByCategory = ApplicationPermissions.GetPermissionsByCategory();
            
            // Convertir a Dictionary<string, List<string>>
            var result = new Dictionary<string, List<string>>();
            foreach (var category in permissionsByCategory)
            {
                result[category.Key] = category.Value.Select(p => p.Name).ToList();
            }
            
            return result;
        }

        public async Task<PermissionInfo?> GetPermissionInfoAsync(string permission)
        {
            var roles = await GetRolesWithPermissionAsync(permission);
            var userCount = 0;

            // Contar usuarios que tienen este permiso
            foreach (var roleName in roles)
            {
                var role = await _roleManager.FindByNameAsync(roleName);
                if (role != null)
                {
                    var usersInRole = await _userManager.GetUsersInRoleAsync(roleName);
                    userCount += usersInRole.Count;
                }
            }

            // Buscar la información del permiso en ApplicationPermissions
            var allPermissions = ApplicationPermissions.GetAllPermissionsList();
            var permissionDetail = allPermissions.FirstOrDefault(p => p.Name == permission);
            
            if (permissionDetail != null)
            {
                return new PermissionInfo
                {
                    Name = permissionDetail.Name,
                    DisplayName = permissionDetail.DisplayName,
                    Category = GetCategoryFromPermission(permission),
                    Module = GetModuleFromPermission(permission),
                    Description = permissionDetail.Description,
                    AssignedRoles = roles,
                    UserCount = userCount,
                    IsSystemPermission = true
                };
            }

            // Fallback si no se encuentra en la definición
            var parts = permission.Split('.');
            var category = parts.Length > 0 ? parts[0] : "General";
            var module = parts.Length > 1 ? parts[1] : "General";

            return new PermissionInfo
            {
                Name = permission,
                DisplayName = permission.Replace('.', ' '),
                Category = category,
                Module = module,
                Description = $"Permiso para {permission.Replace('.', ' ').ToLower()}",
                AssignedRoles = roles,
                UserCount = userCount,
                IsSystemPermission = true
            };
        }

        private static string GetCategoryFromPermission(string permission)
        {
            var permissionsByCategory = ApplicationPermissions.GetPermissionsByCategory();
            foreach (var category in permissionsByCategory)
            {
                if (category.Value.Any(p => p.Name == permission))
                {
                    return category.Key;
                }
            }
            
            var parts = permission.Split('.');
            return parts.Length > 0 ? parts[0] : "General";
        }
        
        private static string GetModuleFromPermission(string permission)
        {
            var parts = permission.Split('.');
            return parts.Length > 1 ? parts[1] : GetCategoryFromPermission(permission);
        }

        public async Task<List<string>> GetRolesWithPermissionAsync(string permission)
        {
            var rolesWithPermission = new List<string>();
            var allRoles = await _roleManager.Roles.ToListAsync();

            foreach (var role in allRoles)
            {
                var roleClaims = await _roleManager.GetClaimsAsync(role);
                if (roleClaims.Any(c => c.Type == "permission" && c.Value == permission))
                {
                    rolesWithPermission.Add(role.Name ?? "");
                }
            }

            return rolesWithPermission;
        }

        public async Task<List<string>> GetUserDirectPermissionsAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return new List<string>();

            var userClaims = await _userManager.GetClaimsAsync(user);
            return userClaims
                .Where(c => c.Type == "permission")
                .Select(c => c.Value)
                .ToList();
        }

        public async Task<List<string>> GetUserRolePermissionsAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return new List<string>();

            var permissions = new List<string>();
            var userRoles = await _userManager.GetRolesAsync(user);
            
            foreach (var roleName in userRoles)
            {
                var rolePermissions = await GetRolePermissionsAsync(roleName);
                permissions.AddRange(rolePermissions);
            }

            return permissions.Distinct().ToList();
        }

        public async Task<bool> AssignPermissionToUserAsync(string userId, string permission)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null) return false;

                var existingClaims = await _userManager.GetClaimsAsync(user);
                if (existingClaims.Any(c => c.Type == "permission" && c.Value == permission))
                {
                    return true; // Ya existe
                }

                var result = await _userManager.AddClaimAsync(user, new Claim("permission", permission));
                return result.Succeeded;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error asignando permiso {permission} al usuario {userId}");
                return false;
            }
        }

        public async Task<bool> RemovePermissionFromUserAsync(string userId, string permission)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null) return false;

                var result = await _userManager.RemoveClaimAsync(user, new Claim("permission", permission));
                return result.Succeeded;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error removiendo permiso {permission} del usuario {userId}");
                return false;
            }
        }

        public async Task<bool> SyncRolePermissionsAsync(string roleName, IEnumerable<string> permissions)
        {
            try
            {
                return await AssignPermissionsToRoleAsync(roleName, permissions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sincronizando permisos del rol {roleName}");
                return false;
            }
        }

        /// <summary>
        /// Sincroniza los permisos de todos los roles del sistema con los permisos definidos en el código.
        /// Valida que todos los permisos asignados a los roles existan en ApplicationPermissions,
        /// limpia permisos obsoletos y garantiza que SuperAdmin tenga todos los permisos disponibles.
        /// </summary>
        /// <remarks>
        /// Este método:
        /// - Obtiene todos los roles del sistema de la base de datos
        /// - Verifica que los permisos asignados a cada rol sean válidos según ApplicationPermissions
        /// - Elimina permisos que ya no existen en el código
        /// - Mantiene solo los permisos válidos en cada rol
        /// - Asegura que el rol SuperAdministrador tenga todos los permisos del sistema
        /// - Registra estadísticas de la sincronización en los logs
        /// </remarks>
        /// <exception cref="Exception">Se lanza si ocurre un error durante la sincronización</exception>
        public async Task SyncRolePermissionsAsync()
        {
            try
            {
                _logger.LogInformation("Iniciando sincronización de permisos de todos los roles");
                
                // Obtener todos los roles del sistema
                var roles = await _roleManager.Roles.ToListAsync();
                var allPermissions = ApplicationPermissions.GetAllPermissions();
                
                foreach (var role in roles)
                {
                    var rolePermissions = await GetRolePermissionsAsync(role.Name!);
                    var validPermissions = rolePermissions.Where(p => allPermissions.Contains(p)).ToList();
                    
                    // Si hay permisos inválidos, limpiar y reasignar solo los válidos
                    if (rolePermissions.Count != validPermissions.Count)
                    {
                        // Limpiar permisos existentes
                        var existingClaims = await _roleManager.GetClaimsAsync(role);
                        var permissionClaims = existingClaims.Where(c => c.Type == "permission").ToList();
                        
                        foreach (var claim in permissionClaims)
                        {
                            await _roleManager.RemoveClaimAsync(role, claim);
                        }
                        
                        // Reasignar solo permisos válidos
                        foreach (var permission in validPermissions)
                        {
                            await _roleManager.AddClaimAsync(role, new Claim("permission", permission));
                        }
                        
                        _logger.LogInformation($"Rol {role.Name}: {permissionClaims.Count} permisos limpiados, {validPermissions.Count} permisos válidos reasignados");
                    }
                }
                
                // Asegurar que SuperAdmin tenga todos los permisos
                await AssignAllPermissionsToSuperAdminAsync();
                
                _logger.LogInformation($"Sincronización de permisos completada para {roles.Count} roles");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error durante la sincronización de permisos de roles");
                throw;
            }
        }

        public async Task SyncPermissionsAsync()
        {
            try
            {
                _logger.LogInformation("Iniciando sincronización de permisos del sistema");
                
                // Obtener todos los roles del sistema
                var roles = await _roleManager.Roles.ToListAsync();
                var allPermissions = ApplicationPermissions.GetAllPermissions();
                
                foreach (var role in roles)
                {
                    var rolePermissions = await GetRolePermissionsAsync(role.Name!);
                    var validPermissions = rolePermissions.Where(p => allPermissions.Contains(p)).ToList();
                    
                    // Si hay permisos inválidos, limpiar y reasignar solo los válidos
                    if (rolePermissions.Count != validPermissions.Count)
                    {
                        // Limpiar permisos existentes
                        var existingClaims = await _roleManager.GetClaimsAsync(role);
                        var permissionClaims = existingClaims.Where(c => c.Type == "permission").ToList();
                        
                        foreach (var claim in permissionClaims)
                        {
                            await _roleManager.RemoveClaimAsync(role, claim);
                        }
                        
                        // Reasignar solo permisos válidos
                        foreach (var permission in validPermissions)
                        {
                            await _roleManager.AddClaimAsync(role, new Claim("permission", permission));
                        }
                        
                        _logger.LogInformation($"Rol {role.Name}: {permissionClaims.Count} permisos limpiados, {validPermissions.Count} permisos válidos reasignados");
                    }
                }
                
                // Asegurar que SuperAdmin tenga todos los permisos
                await AssignAllPermissionsToSuperAdminAsync();
                
                _logger.LogInformation($"Sincronización de permisos completada para {roles.Count} roles");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error durante la sincronización de permisos");
                throw;
            }
        }

        public async Task<bool> HasPermissionAsync(string userId, string permission)
        {
            return await UserHasPermissionAsync(userId, permission);
        }

        private async Task AssignAllPermissionsToSuperAdminAsync()
        {
            var superAdminRole = await _roleManager.FindByNameAsync(ApplicationRoles.SuperAdministrador);
            if (superAdminRole == null) return;

            var allPermissions = await GetAllPermissionsAsync();
            await AssignPermissionsToRoleAsync(ApplicationRoles.SuperAdministrador, allPermissions);
        }
    }
}