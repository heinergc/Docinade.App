using System.ComponentModel.DataAnnotations;

namespace RubricasApp.Web.Models
{
    public class GrupoCalificacion
    {
        public int IdGrupo { get; set; }
        
        [Required(ErrorMessage = "El nombre del grupo es requerido")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        [Display(Name = "Nombre del Grupo")]
        public string NombreGrupo { get; set; } = string.Empty;
        
        [StringLength(200, ErrorMessage = "La descripción no puede exceder 200 caracteres")]
        [Display(Name = "Descripción")]
        public string? Descripcion { get; set; }
        
        [Display(Name = "Estado")]
        public string Estado { get; set; } = "ACTIVO";
        
        [Display(Name = "Fecha de Creación")]
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        
        // Navigation properties
        public virtual ICollection<Rubrica> Rubricas { get; set; } = new List<Rubrica>();
        public virtual ICollection<NivelCalificacion> NivelesCalificacion { get; set; } = new List<NivelCalificacion>();
    }
}
