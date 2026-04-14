using System.ComponentModel.DataAnnotations;

namespace DocinadeApp.ViewModels.Admin
{
    /// <summary>
    /// ViewModel para la vista de usuario
    /// </summary>
    public class UserViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
        public string? Institucion { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaRegistro { get; set; }
        public DateTime? UltimoAcceso { get; set; }
        public List<string> Roles { get; set; } = new();
        public int PermissionCount { get; set; }
        public bool EmailConfirmed { get; set; }
        public string Nombre { get; set; } = string.Empty;
        
        // Propiedades adicionales requeridas por el controlador
        public string Apellido { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedDate { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
        public int RoleCount { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
    }

    /// <summary>
    /// ViewModel para la lista de usuarios
    /// </summary>
    public class UserListViewModel : AdminBaseViewModel
    {
        public List<UserViewModel> Users { get; set; } = new();
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public int TotalUsers { get; set; }
        public int TotalPages { get; set; }
        public string? SearchTerm { get; set; }
        public string? SelectedRole { get; set; }
        public bool? ActiveFilter { get; set; }
        public List<RoleSelectionItem> AvailableRoles { get; set; } = new();
        
        // Propiedades adicionales requeridas por el controlador
        public int ActiveUsers { get; set; }
        public int InactiveUsers { get; set; }
        public int LockedUsers { get; set; }
    }

    /// <summary>
    /// ViewModel para detalles de usuario
    /// </summary>
    public class UserDetailsViewModel : UserViewModel
    {
        public string? PhoneNumber { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
        public int AccessFailedCount { get; set; }
        public List<string> DirectPermissions { get; set; } = new();
        public List<string> AllPermissions { get; set; } = new();
        
        // Propiedades adicionales requeridas por el controlador
        public string Apellido { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedDate { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public List<string> RolePermissions { get; set; } = new();
        public Dictionary<string, List<DocinadeApp.Models.Permissions.PermissionInfo>> PermissionsByCategory { get; set; } = new();
    }

    /// <summary>
    /// ViewModel para crear usuario
    /// </summary>
    public class CreateUserViewModel
    {
        [Required(ErrorMessage = "El email es requerido")]
        [EmailAddress(ErrorMessage = "Formato de email inválido")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es requerida")]
        [StringLength(100, ErrorMessage = "La {0} debe tener al menos {2} y máximo {1} caracteres.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar contraseña")]
        [Compare("Password", ErrorMessage = "La contraseña y la confirmación no coinciden.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Display(Name = "Nombre Completo")]
        public string NombreCompleto { get; set; } = string.Empty;

        [Display(Name = "Institución")]
        public string? Institucion { get; set; }

        [Display(Name = "Número de Identificación")]
        public string? NumeroIdentificacion { get; set; }

        [Display(Name = "Departamento")]
        public string? Departamento { get; set; }

        [Display(Name = "Email Confirmado")]
        public bool EmailConfirmed { get; set; } = false;

        [Display(Name = "Roles")]
        public List<string>? SelectedRoles { get; set; }

        public List<RoleSelectionItem> AvailableRoles { get; set; } = new();
        
        // Propiedades adicionales requeridas por el controlador
        [Required(ErrorMessage = "El nombre es requerido")]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "El apellido es requerido")]
        [Display(Name = "Apellido")]
        public string Apellido { get; set; } = string.Empty;
        
        [Display(Name = "Usuario Activo")]
        public bool IsActive { get; set; } = true;
        
        [Display(Name = "Teléfono")]
        public string? PhoneNumber { get; set; }
        
        [Display(Name = "Autenticación de Dos Factores")]
        public bool TwoFactorEnabled { get; set; } = false;
        
        [Display(Name = "Permisos")]
        public List<string>? SelectedPermissions { get; set; }
        
        public List<PermissionSelectionItem> AvailablePermissions { get; set; } = new();
        
        [Display(Name = "Enviar Email de Bienvenida")]
        public bool SendWelcomeEmail { get; set; } = true;
    }

    /// <summary>
    /// ViewModel para editar usuario
    /// </summary>
    public class EditUserViewModel
    {
        public string Id { get; set; } = string.Empty;

        [Required(ErrorMessage = "El email es requerido")]
        [EmailAddress(ErrorMessage = "Formato de email inválido")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Nombre Completo")]
        public string NombreCompleto { get; set; } = string.Empty;

        [Display(Name = "Institución")]
        public string? Institucion { get; set; }

        [Display(Name = "Activo")]
        public bool Activo { get; set; } = true;

        [Display(Name = "Email Confirmado")]
        public bool EmailConfirmed { get; set; }

        [Display(Name = "Roles")]
        public List<string>? SelectedRoles { get; set; }

        public List<RoleSelectionItem> AvailableRoles { get; set; } = new();
        
        // Propiedades adicionales requeridas por el controlador
        [Required(ErrorMessage = "El nombre es requerido")]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "El apellido es requerido")]
        [Display(Name = "Apellido")]
        public string Apellido { get; set; } = string.Empty;
        
        [Display(Name = "Usuario Activo")]
        public bool IsActive { get; set; } = true;
        
        [Display(Name = "Teléfono")]
        public string PhoneNumber { get; set; } = string.Empty;
        
        [Display(Name = "Autenticación de Dos Factores")]
        public bool TwoFactorEnabled { get; set; } = false;
        
        [Display(Name = "Permisos")]
        public List<string> SelectedPermissions { get; set; } = new();
        
        public List<PermissionSelectionItem> AvailablePermissions { get; set; } = new();
        
        [DataType(DataType.Password)]
        [Display(Name = "Nueva Contraseña")]
        public string? NewPassword { get; set; }
    }

    /// <summary>
    /// ViewModel para usuario en rol (utilizado en listas de usuarios de un rol específico)
    /// </summary>
    public class UserInRoleViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
        public string? Institucion { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaRegistro { get; set; }
        public DateTime? UltimoAcceso { get; set; }
    }
}