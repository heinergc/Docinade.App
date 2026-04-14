using Microsoft.AspNetCore.Authorization;
using RubricasApp.Web.Services.Permissions;
using System.Security.Claims;

namespace RubricasApp.Web.Authorization
{
    /// <summary>
    /// Manejador de autorización para requisitos de permisos
    /// </summary>
    public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly IPermissionService _permissionService;
        private readonly ILogger<PermissionAuthorizationHandler> _logger;

        public PermissionAuthorizationHandler(
            IPermissionService permissionService,
            ILogger<PermissionAuthorizationHandler> logger)
        {
            _permissionService = permissionService;
            _logger = logger;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            PermissionRequirement requirement)
        {
            try
            {
                if (context.User?.Identity?.IsAuthenticated != true)
                {
                    _logger.LogDebug("Usuario no autenticado para permiso: {Permission}", requirement.Permission);
                    context.Fail();
                    return;
                }

                var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogDebug("No se pudo obtener ID de usuario para permiso: {Permission}", requirement.Permission);
                    context.Fail();
                    return;
                }

                _logger.LogDebug("Verificando permiso '{Permission}' para usuario {UserId}", requirement.Permission, userId);

                var hasPermission = await _permissionService.UserHasPermissionAsync(userId, requirement.Permission);
                
                if (hasPermission)
                {
                    _logger.LogDebug("Usuario {UserId} tiene permiso '{Permission}'", userId, requirement.Permission);
                    context.Succeed(requirement);
                }
                else
                {
                    _logger.LogDebug("Usuario {UserId} NO tiene permiso '{Permission}'", userId, requirement.Permission);
                    context.Fail();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar permiso '{Permission}' para usuario", requirement.Permission);
                context.Fail();
            }
        }
    }
}