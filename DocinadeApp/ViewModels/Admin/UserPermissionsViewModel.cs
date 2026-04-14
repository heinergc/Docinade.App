using RubricasApp.Web.Models.Permissions;

namespace RubricasApp.Web.ViewModels.Admin
{
    public class UserPermissionsViewModel
    {
        public string UserId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
        public List<string> UserRoles { get; set; } = new List<string>();
        public List<string> DirectPermissions { get; set; } = new List<string>();
        public List<string> RolePermissions { get; set; } = new List<string>();
        public List<string> AllPermissions { get; set; } = new List<string>();
        public Dictionary<string, List<PermissionInfo>> PermissionsByCategory { get; set; } = new();
        public List<string>? SelectedPermissions { get; set; }
    }
}
