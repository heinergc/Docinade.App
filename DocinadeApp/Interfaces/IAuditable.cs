using RubricasApp.Web.Models.Identity;

namespace RubricasApp.Web.Interfaces
{
    /// <summary>
    /// Interface para entidades que requieren auditoría
    /// </summary>
    public interface IAuditable
    {
        string? CreadoPorId { get; set; }
        DateTime FechaCreacion { get; set; }
        string? ModificadoPorId { get; set; }
        DateTime? FechaModificacion { get; set; }
        
        // Navigation properties
        ApplicationUser? CreadoPor { get; set; }
        ApplicationUser? ModificadoPor { get; set; }
    }
}
