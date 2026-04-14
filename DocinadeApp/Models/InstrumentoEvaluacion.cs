using System.ComponentModel.DataAnnotations;

namespace DocinadeApp.Models
{
    public class InstrumentoEvaluacion
    {
        //public int Id { get; set; }

        // Alias para compatibilidad con vistas legacy
        public int InstrumentoId  { get; set; }

        [Required]
        [StringLength(200)]
        public string Nombre { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string? Descripcion { get; set; }
        
        [Display(Name = "Estado Activo")]
        public bool Activo { get; set; } = true;
        
        // Alias para compatibilidad con vistas legacy
        public bool EstaActivo => Activo;
        
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        
        // Navigation properties
        public virtual ICollection<InstrumentoRubrica> InstrumentoRubricas { get; set; } = new List<InstrumentoRubrica>();
        public virtual ICollection<InstrumentoMateria> InstrumentoMaterias { get; set; } = new List<InstrumentoMateria>();
    }
}