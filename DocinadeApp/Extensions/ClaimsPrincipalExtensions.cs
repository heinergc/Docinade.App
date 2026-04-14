using System.Security.Claims;

namespace RubricasApp.Web.Extensions
{
    /// <summary>
    /// Extensiones para ClaimsPrincipal para verificación de permisos
    /// </summary>
    public static class ClaimsPrincipalExtensions
    {
        /// <summary>
        /// Verifica si el usuario tiene un permiso específico
        /// </summary>
        /// <param name="user">Usuario actual</param>
        /// <param name="permission">Permiso a verificar</param>
        /// <returns>True si tiene el permiso, false en caso contrario</returns>
        public static bool HasPermission(this ClaimsPrincipal user, string permission)
        {
            if (user?.Identity?.IsAuthenticated != true)
                return false;

            // Verificar si tiene el claim de permiso directamente
            return user.HasClaim("permission", permission);
        }

        /// <summary>
        /// Verifica si el usuario tiene alguno de los permisos especificados
        /// </summary>
        /// <param name="user">Usuario actual</param>
        /// <param name="permissions">Lista de permisos a verificar</param>
        /// <returns>True si tiene al menos uno de los permisos, false en caso contrario</returns>
        public static bool HasAnyPermission(this ClaimsPrincipal user, params string[] permissions)
        {
            if (user?.Identity?.IsAuthenticated != true)
                return false;

            return permissions.Any(permission => user.HasPermission(permission));
        }

        /// <summary>
        /// Verifica si el usuario tiene todos los permisos especificados
        /// </summary>
        /// <param name="user">Usuario actual</param>
        /// <param name="permissions">Lista de permisos a verificar</param>
        /// <returns>True si tiene todos los permisos, false en caso contrario</returns>
        public static bool HasAllPermissions(this ClaimsPrincipal user, params string[] permissions)
        {
            if (user?.Identity?.IsAuthenticated != true)
                return false;

            return permissions.All(permission => user.HasPermission(permission));
        }

        /// <summary>
        /// Verifica si el usuario tiene acceso de lectura (VER) para un módulo
        /// </summary>
        /// <param name="user">Usuario actual</param>
        /// <param name="module">Módulo a verificar (ej: "usuarios", "rubricas", etc.)</param>
        /// <returns>True si tiene acceso de lectura, false en caso contrario</returns>
        public static bool CanView(this ClaimsPrincipal user, string module)
        {
            if (user?.Identity?.IsAuthenticated != true)
                return false;

            var viewPermission = $"{module.ToLower()}.ver";
            var viewAllPermission = $"{module.ToLower()}.ver_todas";
            
            return user.HasPermission(viewPermission) || user.HasPermission(viewAllPermission);
        }

        /// <summary>
        /// Verifica si el usuario puede crear elementos en un módulo
        /// </summary>
        /// <param name="user">Usuario actual</param>
        /// <param name="module">Módulo a verificar</param>
        /// <returns>True si puede crear, false en caso contrario</returns>
        public static bool CanCreate(this ClaimsPrincipal user, string module)
        {
            if (user?.Identity?.IsAuthenticated != true)
                return false;

            var createPermission = $"{module.ToLower()}.crear";
            return user.HasPermission(createPermission);
        }

        /// <summary>
        /// Verifica si el usuario puede editar elementos en un módulo
        /// </summary>
        /// <param name="user">Usuario actual</param>
        /// <param name="module">Módulo a verificar</param>
        /// <returns>True si puede editar, false en caso contrario</returns>
        public static bool CanEdit(this ClaimsPrincipal user, string module)
        {
            if (user?.Identity?.IsAuthenticated != true)
                return false;

            var editPermission = $"{module.ToLower()}.editar";
            return user.HasPermission(editPermission);
        }

        /// <summary>
        /// Verifica si el usuario puede eliminar elementos en un módulo
        /// </summary>
        /// <param name="user">Usuario actual</param>
        /// <param name="module">Módulo a verificar</param>
        /// <returns>True si puede eliminar, false en caso contrario</returns>
        public static bool CanDelete(this ClaimsPrincipal user, string module)
        {
            if (user?.Identity?.IsAuthenticated != true)
                return false;

            var deletePermission = $"{module.ToLower()}.eliminar";
            return user.HasPermission(deletePermission);
        }
    }
}
