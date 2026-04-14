using System.ComponentModel.DataAnnotations;

namespace RubricasApp.Web.Models
{
    public class CuadernoInstrumento
    {
        public int Id { get; set; }
        
        [Required]
        public int CuadernoCalificadorId { get; set; }
        
        [Required]
        public int RubricaId { get; set; }
        
        [Required]
        [Range(0.01, 100.00)]
        [Display(Name = "Ponderación (%)")]
        public decimal PonderacionPorcentaje { get; set; }
        
        [Display(Name = "Orden")]
        public int Orden { get; set; }
        
        [Display(Name = "Es Obligatorio")]
        public bool EsObligatorio { get; set; } = true;
        
        // Navigation Properties
        public virtual CuadernoCalificador CuadernoCalificador { get; set; } = null!;
        public virtual Rubrica Rubrica { get; set; } = null!;
    }
}