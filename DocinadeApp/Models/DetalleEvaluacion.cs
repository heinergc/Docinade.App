using System.ComponentModel.DataAnnotations;

namespace RubricasApp.Web.Models
{
    public class DetalleEvaluacion
    {
        public int IdDetalle { get; set; }
        
        public int IdEvaluacion { get; set; }
        
        public int IdItem { get; set; }
        
        public int IdNivel { get; set; }
        
        [Display(Name = "Puntos Obtenidos")]
        public decimal PuntosObtenidos { get; set; }
        
        // Navigation properties
        public virtual Evaluacion Evaluacion { get; set; } = null!;
        public virtual ItemEvaluacion ItemEvaluacion { get; set; } = null!;
        public virtual NivelCalificacion NivelCalificacion { get; set; } = null!;
    }
}