using System.ComponentModel.DataAnnotations;

namespace RubricasApp.Web.Models
{
    public class ValorRubrica
    {
        public int IdValor { get; set; }
        
        public int IdRubrica { get; set; }
        
        public int IdItem { get; set; }
        
        public int IdNivel { get; set; }

        [Required(ErrorMessage = "El valor en puntos es requerido")]
        [Range(0, 999.99, ErrorMessage = "El valor debe estar entre 0 y 999.99")]
        [Display(Name = "Valor en Puntos")]
        public decimal ValorPuntos { get; set; } = 0;
        
        // Propiedad calculada para compatibilidad
        public decimal Valor => ValorPuntos;
        
        // Navigation properties
        public virtual Rubrica Rubrica { get; set; } = null!;
        public virtual ItemEvaluacion ItemEvaluacion { get; set; } = null!;
        public virtual NivelCalificacion NivelCalificacion { get; set; } = null!;
    }
}