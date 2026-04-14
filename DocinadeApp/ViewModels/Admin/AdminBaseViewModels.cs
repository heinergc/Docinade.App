namespace RubricasApp.Web.ViewModels.Admin
{
    /// <summary>
    /// ViewModel base para administración
    /// </summary>
    public class AdminBaseViewModel
    {
        public string CurrentUserId { get; set; } = string.Empty;
        public string CurrentUserEmail { get; set; } = string.Empty;
        public List<string> CurrentUserRoles { get; set; } = new();
        
        // Propiedades adicionales para configuración
        public string Titulo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public DateTime FechaActual { get; set; } = DateTime.Now;
        public string UsuarioActual { get; set; } = string.Empty;
    }

    /// <summary>
    /// Item de selección de rol
    /// </summary>
    public class RoleSelectionItem
    {
        public string Id { get; set; } = string.Empty; 
        public string Name { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public bool IsSelected { get; set; }
        public string? Description { get; set; }
    }
}