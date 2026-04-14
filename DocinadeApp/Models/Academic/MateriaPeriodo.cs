using System.ComponentModel.DataAnnotations;

namespace RubricasApp.Web.Models.Academic
{
    /// <summary>
    /// Representa la oferta de una materia en un período académico específico
    /// </summary>
    public class MateriaPeriodo
    {
        public int Id { get; set; }

        [Required]
        public int MateriaId { get; set; }

        [Required]
        public int PeriodoAcademicoId { get; set; }

        [Display(Name = "Cupo")]
        [Range(0, 500, ErrorMessage = "El cupo debe estar entre 0 y 500")]
        public int Cupo { get; set; } = 0;

        [Display(Name = "Estado")]
        [StringLength(20)]
        public string Estado { get; set; } = "Abierta"; // Abierta, Cerrada, Cancelada

        [Display(Name = "Fecha de Publicación")]
        public DateTime? FechaPublicacion { get; set; }

        [Display(Name = "Fecha de Creación")]
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        [Display(Name = "Observaciones")]
        [StringLength(500)]
        public string? Observaciones { get; set; }

        // Navigation properties
        public virtual Materia Materia { get; set; } = null!;
        public virtual PeriodoAcademico PeriodoAcademico { get; set; } = null!;
    }
}