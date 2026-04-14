using System.ComponentModel.DataAnnotations;

namespace RubricasApp.Web.Models
{
    public class ItemEvaluacion
    {
        public int IdItem { get; set; }
        
        [Required(ErrorMessage = "Debe seleccionar una rúbrica")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar una rúbrica válida")]
        [Display(Name = "Rúbrica")]
        public int IdRubrica { get; set; }
        
        [Required(ErrorMessage = "El nombre del item es requerido")]
        [StringLength(200, ErrorMessage = "El nombre no puede exceder 200 caracteres")]
        [Display(Name = "Nombre del Item")]
        public string NombreItem { get; set; } = string.Empty;
        
        [Display(Name = "Descripción")]
        public string? Descripcion { get; set; }
        
        [Display(Name = "Orden")]
        public int? OrdenItem { get; set; }
        
        [Range(0, 100, ErrorMessage = "El peso debe estar entre 0 y 100")]
        [Display(Name = "Peso (%)")]
        public decimal Peso { get; set; } = 0;
        
        // Propiedades calculadas para compatibilidad
        public int Orden => OrdenItem ?? 0;
        
        // Navigation properties
        public virtual Rubrica Rubrica { get; set; } = null!;
        public virtual ICollection<ValorRubrica> ValoresRubrica { get; set; } = new List<ValorRubrica>();
        public virtual ICollection<DetalleEvaluacion> DetallesEvaluacion { get; set; } = new List<DetalleEvaluacion>();
    }
}