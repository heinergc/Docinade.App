using System.ComponentModel.DataAnnotations;

namespace RubricasApp.Web.Models
{
    public class NivelCalificacion
    {
        public int IdNivel { get; set; }
        
        [Required(ErrorMessage = "El nombre del nivel es requerido")]
        [StringLength(50, ErrorMessage = "El nombre no puede exceder 50 caracteres")]
        [Display(Name = "Nombre del Nivel")]
        public string NombreNivel { get; set; } = string.Empty;
        
        [StringLength(200, ErrorMessage = "La descripción no puede exceder 200 caracteres")]
        [Display(Name = "Descripción")]
        public string? Descripcion { get; set; }
        
        [Display(Name = "Orden")]
        public int? OrdenNivel { get; set; }
        
        [Display(Name = "Grupo de Calificación")]
        public int? IdGrupo { get; set; }
        
        // Navigation properties
        public virtual GrupoCalificacion? GrupoCalificacion { get; set; }
        public virtual ICollection<ValorRubrica> ValoresRubrica { get; set; } = new List<ValorRubrica>();
        public virtual ICollection<DetalleEvaluacion> DetallesEvaluacion { get; set; } = new List<DetalleEvaluacion>();
        public virtual ICollection<RubricaNivel> RubricaNiveles { get; set; } = new List<RubricaNivel>();
    }
}