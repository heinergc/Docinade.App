using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RubricasApp.Web.Models
{
    public class GrupoMateria
    {
        [Key]
        public int Id { get; set; }
        
        [Required(ErrorMessage = "El grupo es requerido")]
        [Display(Name = "Grupo")]
        public int GrupoId { get; set; }
        
        [Required(ErrorMessage = "La materia es requerida")]
        [Display(Name = "Materia")]
        public int MateriaId { get; set; }
        
        [Display(Name = "Fecha de Asignaci�n")]
        public DateTime FechaAsignacion { get; set; } = DateTime.Now;
        
    [Required]
    [StringLength(20)]
    [Display(Name = "Estado")]
    public EstadoAsignacion? Estado { get; set; } = EstadoAsignacion.Activo;
        
        [StringLength(500, ErrorMessage = "Las observaciones no pueden exceder 500 caracteres")]
        [Display(Name = "Observaciones")]
        public string? Observaciones { get; set; }
        
        // Navigation properties
        [ForeignKey("GrupoId")]
        public virtual GrupoEstudiante? Grupo { get; set; }
        
        [ForeignKey("MateriaId")]
        public virtual Materia? Materia { get; set; }
    }
}