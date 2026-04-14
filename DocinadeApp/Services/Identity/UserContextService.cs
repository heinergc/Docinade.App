using Microsoft.AspNetCore.Identity;
using DocinadeApp.Models.Identity;
using DocinadeApp.Models.Permissions;
using DocinadeApp.Interfaces;
using System.Security.Claims;

namespace DocinadeApp.Services.Identity
{
    public class UserContextService : IUserContextService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserContextService(
            IHttpContextAccessor httpContextAccessor,
            UserManager<ApplicationUser> userManager)
        {
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }

        public string? GetCurrentUserId()
        {
            return _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

        public ClaimsPrincipal? GetCurrentUser()
        {
            return _httpContextAccessor.HttpContext?.User;
        }

        public async Task<bool> IsInRoleAsync(string role)
        {
            var user = GetCurrentUser();
            if (user == null) return false;

            var applicationUser = await _userManager.GetUserAsync(user);
            if (applicationUser == null) return false;

            return await _userManager.IsInRoleAsync(applicationUser, role);
        }

        public async Task<bool> CanAccessEntityAsync<T>(T entity) where T : class
        {
            var currentUserId = GetCurrentUserId();
            if (string.IsNullOrEmpty(currentUserId)) return false;

            // SuperAdmin y Admin pueden acceder a todo
            if (await IsInRoleAsync(ApplicationRoles.SuperAdministrador) || 
                await IsInRoleAsync(ApplicationRoles.Administrador))
            {
                return true;
            }

            // Si la entidad implementa IUserOwned, verificar propiedad
            if (entity is IUserOwned userOwned)
            {
                return userOwned.BelongsToUser(currentUserId);
            }

            // Para coordinadores, verificar si pueden acceder por área/departamento
            if (await IsInRoleAsync(ApplicationRoles.Coordinador))
            {
                return await CanCoordinatorAccessAsync(entity, currentUserId);
            }

            return false;
        }

        public async Task<bool> CanEditEntityAsync<T>(T entity) where T : class
        {
            // Misma lógica que CanAccess para este ejemplo
            // Se puede personalizar según las reglas de negocio
            return await CanAccessEntityAsync(entity);
        }

        public async Task<bool> CanDeleteEntityAsync<T>(T entity) where T : class
        {
            var currentUserId = GetCurrentUserId();
            if (string.IsNullOrEmpty(currentUserId)) return false;

            // Solo SuperAdmin puede eliminar cualquier cosa
            if (await IsInRoleAsync(ApplicationRoles.SuperAdministrador))
            {
                return true;
            }

            // Admin puede eliminar según reglas específicas
            if (await IsInRoleAsync(ApplicationRoles.Administrador))
            {
                // Verificar si no es un elemento crítico del sistema
                return await CanAdminDeleteAsync(entity);
            }

            // Para otros roles, solo pueden eliminar lo que crearon
            if (entity is IUserOwned userOwned)
            {
                return userOwned.BelongsToUser(currentUserId);
            }

            return false;
        }

        private async Task<bool> CanCoordinatorAccessAsync<T>(T entity, string userId)
        {
            // Implementar lógica específica para coordinadores
            // Por ejemplo, verificar si la entidad pertenece a su departamento
            // Esta es una implementación simplificada
            return true; // TODO: Implementar lógica específica
        }

        private async Task<bool> CanAdminDeleteAsync<T>(T entity)
        {
            // Implementar reglas específicas para lo que puede eliminar un Admin
            // Por ejemplo, no eliminar datos críticos o del sistema
            return true; // TODO: Implementar lógica específica
        }
    }
}
