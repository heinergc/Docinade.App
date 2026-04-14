using System.ComponentModel.DataAnnotations;

namespace RubricasApp.Web.ViewModels.Account
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "El email es requerido")]
        [EmailAddress(ErrorMessage = "El formato del email no es válido")]
        [Display(Name = "Correo Electrónico")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es requerida")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Recordarme")]
        public bool RememberMe { get; set; }

        public string? ReturnUrl { get; set; }
    }

    public class RegisterViewModel
    {
        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "Los apellidos son requeridos")]
        [StringLength(100, ErrorMessage = "Los apellidos no pueden exceder 100 caracteres")]
        [Display(Name = "Apellidos")]
        public string Apellidos { get; set; } = string.Empty;

        [Required(ErrorMessage = "El email es requerido")]
        [EmailAddress(ErrorMessage = "El formato del email no es válido")]
        [Display(Name = "Correo Electrónico")]
        public string Email { get; set; } = string.Empty;

        [StringLength(20, ErrorMessage = "El número de identificación no puede exceder 20 caracteres")]
        [Display(Name = "Número de Identificación")]
        public string? NumeroIdentificacion { get; set; }

        [StringLength(100, ErrorMessage = "La institución no puede exceder 100 caracteres")]
        [Display(Name = "Institución")]
        public string? Institucion { get; set; }

        [StringLength(50, ErrorMessage = "El departamento no puede exceder 50 caracteres")]
        [Display(Name = "Departamento")]
        public string? Departamento { get; set; }

        [Required(ErrorMessage = "La contraseña es requerida")]
        [StringLength(100, ErrorMessage = "La {0} debe tener al menos {2} y máximo {1} caracteres.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar Contraseña")]
        [Compare("Password", ErrorMessage = "La contraseña y la confirmación no coinciden")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Display(Name = "Rol")]
        public string? SelectedRole { get; set; }

        [Display(Name = "Teléfono")]
        [Required(ErrorMessage = "El campo Teléfono es obligatorio.")]
        [Phone(ErrorMessage = "El número de teléfono no es válido.")]
        public string? Telefono { get; set; }

        public List<string>? AvailableRoles { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "El email es requerido")]
        [EmailAddress(ErrorMessage = "El formato del email no es válido")]
        [Display(Name = "Correo Electrónico")]
        public string Email { get; set; } = string.Empty;
    }

    public class ResetPasswordViewModel
    {
        [Required(ErrorMessage = "El email es requerido")]
        [EmailAddress(ErrorMessage = "El formato del email no es válido")]
        [Display(Name = "Correo Electrónico")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es requerida")]
        [StringLength(100, ErrorMessage = "La {0} debe tener al menos {2} y máximo {1} caracteres.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar Contraseña")]
        [Compare("Password", ErrorMessage = "La contraseña y la confirmación no coinciden")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required]
        public string Token { get; set; } = string.Empty;
    }

    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "La contraseña actual es requerida")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña Actual")]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "La nueva contraseña es requerida")]
        [StringLength(100, ErrorMessage = "La {0} debe tener al menos {2} y máximo {1} caracteres.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Nueva Contraseña")]
        public string NewPassword { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar Nueva Contraseña")]
        [Compare("NewPassword", ErrorMessage = "La nueva contraseña y la confirmación no coinciden")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public class EditProfileViewModel
    {
        public string Id { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "Los apellidos son requeridos")]
        [StringLength(100, ErrorMessage = "Los apellidos no pueden exceder 100 caracteres")]
        [Display(Name = "Apellidos")]
        public string Apellidos { get; set; } = string.Empty;

        [Required(ErrorMessage = "El email es requerido")]
        [EmailAddress(ErrorMessage = "El formato del email no es válido")]
        [Display(Name = "Correo Electrónico")]
        public string Email { get; set; } = string.Empty;

        [StringLength(20, ErrorMessage = "El número de identificación no puede exceder 20 caracteres")]
        [Display(Name = "Número de Identificación")]
        public string? NumeroIdentificacion { get; set; }

        [StringLength(100, ErrorMessage = "La institución no puede exceder 100 caracteres")]
        [Display(Name = "Institución")]
        public string? Institucion { get; set; }

        [StringLength(50, ErrorMessage = "El departamento no puede exceder 50 caracteres")]
        [Display(Name = "Departamento")]
        public string? Departamento { get; set; }

        [StringLength(500, ErrorMessage = "Las observaciones no pueden exceder 500 caracteres")]
        [Display(Name = "Observaciones")]
        public string? Observaciones { get; set; }

        [Display(Name = "Teléfono")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "Usuario Activo")]
        public bool Activo { get; set; } = true;
    }

    public class ProfileViewModel
    {
        public string Id { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "Los apellidos son requeridos")]
        [StringLength(100, ErrorMessage = "Los apellidos no pueden exceder 100 caracteres")]
        [Display(Name = "Apellidos")]
        public string Apellidos { get; set; } = string.Empty;

        [Required(ErrorMessage = "El email es requerido")]
        [EmailAddress(ErrorMessage = "El formato del email no es válido")]
        [Display(Name = "Correo Electrónico")]
        public string Email { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "La institución no puede exceder 100 caracteres")]
        [Display(Name = "Institución")]
        public string Institucion { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "El departamento no puede exceder 50 caracteres")]
        [Display(Name = "Departamento")]
        public string? Departamento { get; set; }

        [Display(Name = "Teléfono")]
        public string? Telefono { get; set; }

        public string NombreCompleto => $"{Nombre} {Apellidos}".Trim();
        public string Iniciales => GetIniciales();
        public string? Roles { get; set; }

        private string GetIniciales()
        {
            var iniciales = "";
            if (!string.IsNullOrEmpty(Nombre))
                iniciales += Nombre[0];
            if (!string.IsNullOrEmpty(Apellidos))
                iniciales += Apellidos[0];
            return iniciales.ToUpper();
        }
    }

    public class CreateUserViewModel
    {
        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "Los apellidos son requeridos")]
        [StringLength(100, ErrorMessage = "Los apellidos no pueden exceder 100 caracteres")]
        [Display(Name = "Apellidos")]
        public string Apellidos { get; set; } = string.Empty;

        [Required(ErrorMessage = "El email es requerido")]
        [EmailAddress(ErrorMessage = "El formato del email no es válido")]
        [Display(Name = "Correo Electrónico")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La institución es requerida")]
        [StringLength(100, ErrorMessage = "La institución no puede exceder 100 caracteres")]
        [Display(Name = "Institución")]
        public string Institucion { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "El departamento no puede exceder 50 caracteres")]
        [Display(Name = "Departamento")]
        public string? Departamento { get; set; }

        [Display(Name = "Teléfono")]
        [Phone(ErrorMessage = "El número de teléfono no es válido")]
        public string? Telefono { get; set; }

        [Required(ErrorMessage = "Debe seleccionar al menos un rol")]
        [Display(Name = "Roles")]
        public List<string> Roles { get; set; } = new List<string>();
    }

    public class UserViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? NumeroIdentificacion { get; set; }
        public string? Institucion { get; set; }
        public string? Departamento { get; set; }
        public DateTime FechaRegistro { get; set; }
        public DateTime? UltimoAcceso { get; set; }
        public bool Activo { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
        public bool EmailConfirmed { get; set; }
        public bool LockoutEnabled { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
        public DateTime? LastLoginDate { get; set; }
    }

    public class UserManagementViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? NumeroIdentificacion { get; set; }
        public string? Institucion { get; set; }
        public string? Departamento { get; set; }
        public DateTime FechaRegistro { get; set; }
        public DateTime? UltimoAcceso { get; set; }
        public bool Activo { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
        public bool EmailConfirmed { get; set; }
        public bool LockoutEnabled { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }

        // Propiedades adicionales para la vista
        public string? SearchTerm { get; set; }
        public string? RoleFilter { get; set; }
        public string? StatusFilter { get; set; }
        public string? InstitutionFilter { get; set; }

        public DateTimeOffset? LastLoginDate { get; set; }
        public IEnumerable<UserViewModel> Users { get; set; } = new List<UserViewModel>();
    }
}