using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RubricasApp.Web.Models
{
    /// <summary>
    /// Parámetros configurables a nivel institucional
    /// Incluye la nota mínima de aprobación en conducta
    /// </summary>
    [Table("ParametrosInstitucion")]
    public class ParametroInstitucion
    {
        [Key]
        public int IdParametro { get; set; }

        [Required]
        [StringLength(100)]
        public string Clave { get; set; } = string.Empty; // Ej: "NotaMinimaAprobacionConducta"

        [Required]
        [StringLength(200)]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Descripcion { get; set; }

        [Required]
        [StringLength(200)]
        public string Valor { get; set; } = string.Empty; // Ej: "65"

        [Required]
        [StringLength(50)]
        public string TipoDato { get; set; } = "String"; // String, Integer, Decimal, Boolean, Date

        [StringLength(50)]
        public string? Categoria { get; set; } // Conducta, Evaluación, General, etc.

        [Required]
        public bool Activo { get; set; } = true;

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        public DateTime? FechaModificacion { get; set; }

        [StringLength(450)]
        public string? ModificadoPorId { get; set; }
    }
}
