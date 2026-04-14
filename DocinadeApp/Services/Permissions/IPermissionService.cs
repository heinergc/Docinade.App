using RubricasApp.Web.Models.Permissions;

namespace RubricasApp.Web.Services.Permissions
{
    /// <summary>
    /// Interfaz para el servicio de permisos
    /// </summary>
    public interface IPermissionService
    {
        /// <summary>
        /// Inicializa los roles y permisos por defecto del sistema
        /// </summary>
        Task InitializeDefaultRolesAndPermissionsAsync();

        /// <summary>
        /// Obtiene todos los permisos del sistema
        /// </summary>
        Task<List<string>> GetAllPermissionsAsync();

        /// <summary>
        /// Obtiene los permisos de un usuario específico
        /// </summary>
        Task<List<string>> GetUserPermissionsAsync(string userId);

        /// <summary>
        /// Obtiene los permisos de un rol específico
        /// </summary>
        Task<List<string>> GetRolePermissionsAsync(string roleName);

        /// <summary>
        /// Verifica si un usuario tiene un permiso específico
        /// </summary>
        Task<bool> UserHasPermissionAsync(string userId, string permission);

        /// <summary>
        /// Asigna un permiso a un rol
        /// </summary>
        Task<bool> AssignPermissionToRoleAsync(string roleName, string permission);

        /// <summary>
        /// Remueve un permiso de un rol
        /// </summary>
        Task<bool> RemovePermissionFromRoleAsync(string roleName, string permission);

        /// <summary>
        /// Asigna múltiples permisos a un rol
        /// </summary>
        Task<bool> AssignPermissionsToRoleAsync(string roleName, IEnumerable<string> permissions);

        /// <summary>
        /// Obtiene todos los permisos organizados por categoría
        /// </summary>
        Task<Dictionary<string, List<string>>> GetPermissionsByCategoryAsync();

        /// <summary>
        /// Sincroniza los permisos del sistema con los definidos en el código
        /// </summary>
        Task SyncPermissionsAsync();

        /// <summary>
        /// Obtiene la información detallada de un permiso
        /// </summary>
        Task<PermissionInfo?> GetPermissionInfoAsync(string permission);

        /// <summary>
        /// Obtiene todos los roles que tienen un permiso específico
        /// </summary>
        Task<List<string>> GetRolesWithPermissionAsync(string permission);

        /// <summary>
        /// Obtiene los permisos directos de un usuario (no a través de roles)
        /// </summary>
        Task<List<string>> GetUserDirectPermissionsAsync(string userId);

        /// <summary>
        /// Obtiene los permisos de un usuario a través de sus roles
        /// </summary>
        Task<List<string>> GetUserRolePermissionsAsync(string userId);

        /// <summary>
        /// Asigna un permiso directamente a un usuario
        /// </summary>
        Task<bool> AssignPermissionToUserAsync(string userId, string permission);

        /// <summary>
        /// Remueve un permiso directo de un usuario
        /// </summary>
        Task<bool> RemovePermissionFromUserAsync(string userId, string permission);

        /// <summary>
        /// Sincroniza los permisos de un rol con una lista específica
        /// </summary>
        Task<bool> SyncRolePermissionsAsync(string roleName, IEnumerable<string> permissions);

        /// <summary>
        /// Sincroniza los permisos de todos los roles del sistema
        /// </summary>
        Task SyncRolePermissionsAsync();

        /// <summary>
        /// Verifica si un usuario tiene un permiso específico (incluyendo roles)
        /// </summary>
        Task<bool> HasPermissionAsync(string userId, string permission);
    }

    /// <summary>
    /// Información detallada de un permiso
    /// </summary>
    public class PermissionInfo
    {
        public string Name { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Module { get; set; } = string.Empty;
        public string? Description { get; set; }
        public List<string> AssignedRoles { get; set; } = new();
        public int UserCount { get; set; }
        public bool IsSystemPermission { get; set; }
    }
}