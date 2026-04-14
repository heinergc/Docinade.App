using System.ComponentModel.DataAnnotations;
using DocinadeApp.Models.Academic;

namespace DocinadeApp.Models
{
    public class Materia
    {
        public int MateriaId { get; set; }
        
        [Required]
        [StringLength(100)]
        [Display(Name = "Código")]
        public string Codigo { get; set; } = string.Empty;
        
        [Required]
        [StringLength(200)]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; } = string.Empty;
        
        [Display(Name = "Créditos")]
        public int Creditos { get; set; }
        
        [Display(Name = "Activa")]
        public bool Activa { get; set; } = true;
        
        [Display(Name = "Fecha de Creación")]
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        
        [StringLength(50)]
        [Display(Name = "Tipo")]
        public string? Tipo { get; set; }
        
        [Display(Name = "Ciclo Sugerido")]
        public int CicloSugerido { get; set; }
        
        [StringLength(500)]
        [Display(Name = "Descripción")]
        public string? Descripcion { get; set; }
        
        [Display(Name = "Fecha de Modificación")]
        public DateTime? FechaModificacion { get; set; }
        
        [StringLength(20)]
        [Display(Name = "Estado")]
        public string? Estado { get; set; }
        
        // Navigation Properties
        public virtual ICollection<MateriaRequisito> Prerequisitos { get; set; } = new List<MateriaRequisito>();
        public virtual ICollection<MateriaRequisito> EsRequisitoPara { get; set; } = new List<MateriaRequisito>();
        public virtual ICollection<MateriaPeriodo> Ofertas { get; set; } = new List<MateriaPeriodo>();
        public virtual ICollection<InstrumentoMateria> InstrumentoMaterias { get; set; } = new List<InstrumentoMateria>();
        public virtual ICollection<CuadernoCalificador> CuadernosCalificadores { get; set; } = new List<CuadernoCalificador>();
    }
}