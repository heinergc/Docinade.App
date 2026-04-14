using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DocinadeApp.Models
{
    /// <summary>
    /// Tabla para almacenar excepciones y errores del sistema
    /// </summary>
    [Table("ExcepcionesSistema")]
    public class ExcepcionSistema
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(500)]
        public string Operacion { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string TipoExcepcion { get; set; } = string.Empty;

        [Required]
        public string MensajeError { get; set; } = string.Empty;

        public string? StackTrace { get; set; }

        [StringLength(50)]
        public string? Usuario { get; set; }

        [StringLength(45)]
        public string? IpAddress { get; set; }

        [StringLength(500)]
        public string? UrlSolicitada { get; set; }

        [StringLength(10)]
        public string? MetodoHttp { get; set; }

        public string? ParametrosEntrada { get; set; }

        public DateTime FechaHora { get; set; } = DateTime.UtcNow;

        [StringLength(50)]
        public string? Severidad { get; set; } = "Error"; // Info, Warning, Error, Critical

        public bool Resuelta { get; set; } = false;

        public DateTime? FechaResolucion { get; set; }

        [StringLength(1000)]
        public string? NotasResolucion { get; set; }
    }
}
