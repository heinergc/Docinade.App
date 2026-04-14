using DocinadeApp.Models.Permissions;

namespace DocinadeApp.ViewModels.Admin
{
    /// <summary>
    /// ViewModel para la vista de permiso
    /// </summary>
    public class PermissionViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Module { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int RoleCount { get; set; }
        public int UserCount { get; set; }
        public List<string> AssignedRoles { get; set; } = new();
        public bool IsSystemPermission { get; set; }
        public bool IsAssigned { get; set; }
    }

    /// <summary>
    /// ViewModel para la lista de permisos
    /// </summary>
    public class PermissionListViewModel : AdminBaseViewModel
    {
        public List<PermissionViewModel> Permissions { get; set; } = new();
        public Dictionary<string, List<PermissionViewModel>> PermissionsByCategory { get; set; } = new();
        public List<PermissionViewModel> AllPermissions { get; set; } = new();
        public string? SearchTerm { get; set; }
        public string? SelectedCategory { get; set; }
        public List<string> Categories { get; set; } = new();
        public List<string> Modules { get; set; } = new();
        public List<string> AvailableCategories { get; set; } = new();
        public int TotalPermissions { get; set; }
    }

    /// <summary>
    /// ViewModel para detalles de permiso
    /// </summary>
    public class PermissionDetailsViewModel
    {
        public string Name { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Module { get; set; } = string.Empty;
        public string? Description { get; set; }
        public List<RolePermissionViewModel> RolesWithPermission { get; set; } = new();
        public List<UserPermissionViewModel> UsersWithPermission { get; set; } = new();
        public int TotalRoles { get; set; }
        public int TotalUsers { get; set; }
        public bool IsSystemPermission { get; set; }
    }

    /// <summary>
    /// ViewModel para rol con permiso
    /// </summary>
    public class RolePermissionViewModel
    {
        public string RoleId { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
        public int UserCount { get; set; }
        public bool IsSystemRole { get; set; }
    }

    /// <summary>
    /// ViewModel para usuario con permiso
    /// </summary>
    public class UserPermissionViewModel
    {
        public string UserId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
        public bool Activo { get; set; }
        public string ThroughRole { get; set; } = string.Empty;
    }

    /// <summary>
    /// ViewModel para asignar permisos a un rol
    /// </summary>
    public class AssignPermissionsToRoleViewModel
    {
        public string RoleId { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
        public Dictionary<string, List<PermissionSelectionItem>> PermissionsByCategory { get; set; } = new();
        public List<string> CurrentPermissions { get; set; } = new();
        public List<string> SelectedPermissions { get; set; } = new();
    }

    /// <summary>
    /// ViewModel para asignar permisos a un usuario
    /// </summary>
    public class AssignPermissionsToUserViewModel
    {
        public string UserId { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public Dictionary<string, List<PermissionSelectionItem>> PermissionsByCategory { get; set; } = new();
        public List<string> RolePermissions { get; set; } = new();
        public List<string> DirectPermissions { get; set; } = new();
        public List<string> SelectedPermissions { get; set; } = new();
        public List<string> UserRoles { get; set; } = new();
    }

    /// <summary>
    /// ViewModel para matriz de permisos
    /// </summary>
    public class PermissionMatrixViewModel : AdminBaseViewModel
    {
        public List<PermissionMatrixRow> Matrix { get; set; } = new();
        public List<string> Roles { get; set; } = new();
        public DateTime GeneratedDate { get; set; }
        public string? SelectedCategory { get; set; }
        public List<string> AvailableCategories { get; set; } = new();
        public int TotalPermissions => Matrix.Count;
        public int TotalRoles => Roles.Count;
        public int TotalAssignments => Matrix.Sum(m => m.RoleAssignments.Count(ra => ra.Value));
        public List<string> Categories => Matrix.Select(m => m.Category).Distinct().OrderBy(c => c).ToList();
    }

    /// <summary>
    /// Fila de la matriz de permisos
    /// </summary>
    public class PermissionMatrixRow
    {
        public string Category { get; set; } = string.Empty;
        public string Permission { get; set; } = string.Empty;
        public string PermissionName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public Dictionary<string, bool> RoleAssignments { get; set; } = new();
        public int AssignedRolesCount => RoleAssignments.Count(ra => ra.Value);
        public double AssignmentPercentage => RoleAssignments.Count > 0 
            ? (double)AssignedRolesCount / RoleAssignments.Count * 100 
            : 0;
    }

    /// <summary>
    /// ViewModel para selección de rol para asignación de permisos
    /// </summary>
    public class RoleSelectionViewModel : AdminBaseViewModel
    {
        /// <summary>
        /// Lista de roles disponibles para asignación
        /// </summary>
        public List<RolePermissionViewModel> AvailableRoles { get; set; } = new();

        /// <summary>
        /// Filtro de búsqueda
        /// </summary>
        public string? SearchTerm { get; set; }

        /// <summary>
        /// Total de roles en el sistema
        /// </summary>
        public int TotalRoles => AvailableRoles.Count;

        /// <summary>
        /// Roles del sistema vs personalizados
        /// </summary>
        public int SystemRoles => AvailableRoles.Count(r => r.IsSystemRole);
        public int CustomRoles => AvailableRoles.Count(r => !r.IsSystemRole);
    }

    /// <summary>
    /// Item de selección de permiso
    /// </summary>
    public class PermissionSelectionItem
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Module { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsSelected { get; set; }
        public bool IsSystemPermission { get; set; }
    }
}