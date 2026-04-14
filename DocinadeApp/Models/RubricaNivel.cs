using System.ComponentModel.DataAnnotations;

namespace DocinadeApp.Models
{
    public class RubricaNivel
    {
        public int IdRubrica { get; set; }
        public int IdNivel { get; set; }
        
        [Display(Name = "Orden en la Rúbrica")]
        public int OrdenEnRubrica { get; set; }
        
        // Navigation properties
        public virtual Rubrica Rubrica { get; set; } = null!;
        public virtual NivelCalificacion NivelCalificacion { get; set; } = null!;
    }
}
