using System.ComponentModel.DataAnnotations;

namespace DocinadeApp.Models
{
    public class CuadernoCalificador
    {
        public int Id { get; set; }
        
        [Required]
        [Display(Name = "Materia")]
        public int MateriaId { get; set; }
        
        [Required]
        [Display(Name = "Período Académico")]
        public int PeriodoAcademicoId { get; set; }
        
        [Required]
        [StringLength(200)]
        [Display(Name = "Nombre del Cuaderno")]
        public string Nombre { get; set; } = string.Empty;
        
        [Display(Name = "Fecha de Creación")]
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        
        [Display(Name = "Estado")]
        public string Estado { get; set; } = "ACTIVO"; // ACTIVO, CERRADO, FINALIZADO
        
        [Display(Name = "Fecha de Cierre")]
        public DateTime? FechaCierre { get; set; }
        
        // Navigation Properties
        public virtual Materia Materia { get; set; } = null!;
        public virtual PeriodoAcademico PeriodoAcademico { get; set; } = null!;
        public virtual ICollection<CuadernoInstrumento> CuadernoInstrumentos { get; set; } = new List<CuadernoInstrumento>();
    }
}