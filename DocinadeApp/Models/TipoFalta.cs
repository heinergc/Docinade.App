using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DocinadeApp.Models
{
    /// <summary>
    /// Catálogo de tipos de faltas según REA 40862-V21, Art. 137
    /// </summary>
    [Table("TiposFalta")]
    public class TipoFalta
    {
        [Key]
        public int IdTipoFalta { get; set; }

        [Required]
        [StringLength(50)]
        public string Nombre { get; set; } = string.Empty; // Muy leve, Leve, Grave, Muy grave, Gravísima

        [Required]
        [StringLength(2000)]
        public string Definicion { get; set; } = string.Empty;

        [StringLength(4000)]
        public string? Ejemplos { get; set; }

        [StringLength(2000)]
        public string? AccionCorrectiva { get; set; } // Art. 148-150

        /// <summary>
        /// Rebajo mínimo en puntos según Art. 137
        /// </summary>
        [Required]
        public int RebajoMinimo { get; set; }

        /// <summary>
        /// Rebajo máximo en puntos según Art. 137
        /// </summary>
        [Required]
        public int RebajoMaximo { get; set; }

        [Required]
        public int Orden { get; set; } // Para mostrar en orden de gravedad

        [Required]
        public bool Activo { get; set; } = true;

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        // Navegación
        public virtual ICollection<BoletaConducta> Boletas { get; set; } = new List<BoletaConducta>();
    }
}
