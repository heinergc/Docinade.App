using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RubricasApp.Web.Models
{
    /// <summary>
    /// Catálogo simple de tipos de grupo
    /// </summary>
    [Table("TiposGrupo")]
    public class TipoGrupoCatalogo
    {
        [Key]
        [Display(Name = "ID")]
        public int IdTipoGrupo { get; set; }

        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; } = string.Empty;

        [Display(Name = "Fecha de Registro")]
        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "El estado es requerido")]
        [StringLength(20, ErrorMessage = "El estado no puede exceder 20 caracteres")]
        [Display(Name = "Estado")]
        public string Estado { get; set; } = "Activo";

        // Navigation property para grupos que usan este tipo
        public virtual ICollection<GrupoEstudiante> GruposEstudiantes { get; set; } = new List<GrupoEstudiante>();

        // Propiedades de conveniencia
        [NotMapped]
        public bool EsActivo => Estado == "Activo";
    }
}