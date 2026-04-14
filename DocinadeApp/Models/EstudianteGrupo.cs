using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RubricasApp.Web.Models.Identity;

namespace RubricasApp.Web.Models
{
    public class EstudianteGrupo
    {
        [Key]
        public int Id { get; set; }
        
        [Required(ErrorMessage = "El estudiante es requerido")]
        [Display(Name = "Estudiante")]
        public int EstudianteId { get; set; }
        
        [Required(ErrorMessage = "El grupo es requerido")]
        [Display(Name = "Grupo")]
        public int GrupoId { get; set; }
        
        [Display(Name = "Fecha de Asignaci�n")]
        public DateTime FechaAsignacion { get; set; } = DateTime.Now;
        
        [Display(Name = "Fecha de Desasignaci�n")]
        public DateTime? FechaDesasignacion { get; set; }
        
    [Required]
    [StringLength(20)]
    [Display(Name = "Estado")]
    public EstadoAsignacion? Estado { get; set; } = EstadoAsignacion.Activo;
        
        [StringLength(450)]
        [Display(Name = "Asignado Por")]
        public string? AsignadoPorId { get; set; }
        
        [StringLength(200, ErrorMessage = "El motivo no puede exceder 200 caracteres")]
        [Display(Name = "Motivo de Asignaci�n")]
        public string? MotivoAsignacion { get; set; }
        
        [Display(Name = "Es Grupo Principal")]
        public bool EsGrupoPrincipal { get; set; } = true;
        
        // Navigation properties
        [ForeignKey("EstudianteId")]
        public virtual Estudiante? Estudiante { get; set; }
        
        [ForeignKey("GrupoId")]
        public virtual GrupoEstudiante? Grupo { get; set; }
        
        [ForeignKey("AsignadoPorId")]
        public virtual ApplicationUser? AsignadoPor { get; set; }
    }
    
    public enum EstadoAsignacion
    {
        [Display(Name = "Activo")]
        Activo = 1,
        
        [Display(Name = "Inactivo")]
        Inactivo = 2,
        
        [Display(Name = "Trasladado")]
        Trasladado = 3
    }
}