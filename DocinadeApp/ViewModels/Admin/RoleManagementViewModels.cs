using System.ComponentModel.DataAnnotations;

namespace DocinadeApp.ViewModels.Admin
{
    /// <summary>
    /// ViewModel para la vista de rol
    /// </summary>
    public class RoleViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int UserCount { get; set; }
        public int PermissionCount { get; set; }
        public DateTime CreatedDate { get; set; }
        public List<string> Permissions { get; set; } = new();
        public bool IsSystemRole { get; set; }
    }

    /// <summary>
    /// ViewModel para la lista de roles
    /// </summary>
    public class RoleListViewModel : AdminBaseViewModel
    {
        public List<RoleViewModel> Roles { get; set; } = new();
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public int TotalRoles { get; set; }
        public int TotalPages { get; set; }
        public string? SearchTerm { get; set; }
        public bool? SystemRolesFilter { get; set; }
    }

    /// <summary>
    /// ViewModel para crear rol
    /// </summary>
    public class CreateRoleViewModel
    {
        [Required(ErrorMessage = "El nombre del rol es requerido")]
        [StringLength(256, ErrorMessage = "El nombre no puede tener más de 256 caracteres")]
        [Display(Name = "Nombre del Rol")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Descripción")]
        [StringLength(1000, ErrorMessage = "La descripción no puede tener más de 1000 caracteres")]
        public string? Description { get; set; }

        [Display(Name = "Permisos")]
        public List<string>? SelectedPermissionIds { get; set; }

        public List<PermissionSelectionItem> AvailablePermissions { get; set; } = new();
    }

    /// <summary>
    /// ViewModel para editar rol
    /// </summary>
    public class EditRoleViewModel
    {
        public string Id { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre del rol es requerido")]
        [StringLength(256, ErrorMessage = "El nombre no puede tener más de 256 caracteres")]
        [Display(Name = "Nombre del Rol")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Descripción")]
        [StringLength(1000, ErrorMessage = "La descripción no puede tener más de 1000 caracteres")]
        public string? Description { get; set; }

        [Display(Name = "Permisos")]
        public List<string>? SelectedPermissionIds { get; set; }

        public List<PermissionSelectionItem> AvailablePermissions { get; set; } = new();
        public bool IsSystemRole { get; set; }
        public int UserCount { get; set; }
    }

    /// <summary>
    /// ViewModel para detalles de rol
    /// </summary>
    public class RoleDetailsViewModel : RoleViewModel
    {
        public List<UserViewModel> UsersInRole { get; set; } = new();
        public List<string> DetailedPermissions { get; set; } = new();
    }

    /// <summary>
    /// Item de selección de permiso
    /// </summary>
    //public class PermissionSelectionItem
    //{
    //    public string Id { get; set; } = string.Empty;
    //    public string Name { get; set; } = string.Empty;
    //    public string DisplayName { get; set; } = string.Empty;
    //    public string Module { get; set; } = string.Empty;
    //    public string Category { get; set; } = string.Empty;
    //    public string? Description { get; set; }
    //    public bool IsSelected { get; set; }
    //}
}