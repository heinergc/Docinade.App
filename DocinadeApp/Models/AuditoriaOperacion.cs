using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using DocinadeApp.Models.Identity;

namespace DocinadeApp.Models
{
    /// <summary>
    /// Modelo para registrar las operaciones de auditoría del sistema
    /// </summary>
    [Table("AuditoriasOperaciones")]
    public class AuditoriaOperacion
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Tipo de operación realizada (CREATE, UPDATE, DELETE, etc.)
        /// </summary>
        [Required]
        [StringLength(50)]
        public string TipoOperacion { get; set; } = string.Empty;

        /// <summary>
        /// Nombre de la tabla afectada
        /// </summary>
        [Required]
        [StringLength(100)]
        public string TablaAfectada { get; set; } = string.Empty;

        /// <summary>
        /// ID del registro afectado
        /// </summary>
        public int RegistroId { get; set; }

        /// <summary>
        /// Descripción de la operación realizada
        /// </summary>
        [Required]
        [StringLength(500)]
        public string Descripcion { get; set; } = string.Empty;

        /// <summary>
        /// Motivo o justificación de la operación
        /// </summary>
        [StringLength(500)]
        public string? Motivo { get; set; }

        /// <summary>
        /// Dirección IP desde donde se realizó la operación
        /// </summary>
        [StringLength(45)] // IPv6 máximo
        public string? DireccionIP { get; set; }

        /// <summary>
        /// User Agent del navegador
        /// </summary>
        [StringLength(500)]
        public string? UserAgent { get; set; }

        /// <summary>
        /// Fecha y hora de la operación
        /// </summary>
        public DateTime FechaOperacion { get; set; }

        /// <summary>
        /// ID del usuario que realizó la operación
        /// </summary>
        [Required]
        [StringLength(450)] // Tamaño estándar para ASP.NET Core Identity
        public string UsuarioId { get; set; } = string.Empty;

        /// <summary>
        /// Usuario que realizó la operación
        /// </summary>
        [ForeignKey("UsuarioId")]
        public virtual ApplicationUser? Usuario { get; set; }

        /// <summary>
        /// Indica si la operación fue exitosa
        /// </summary>
        public bool OperacionExitosa { get; set; } = true;

        /// <summary>
        /// Mensaje de error si la operación falló
        /// </summary>
        [StringLength(1000)]
        public string? MensajeError { get; set; }

        /// <summary>
        /// Datos anteriores (JSON) - para operaciones UPDATE y DELETE
        /// </summary>
        [Column(TypeName = "ntext")]
        public string? DatosAnteriores { get; set; }

        /// <summary>
        /// Datos nuevos (JSON) - para operaciones CREATE y UPDATE
        /// </summary>
        [Column(TypeName = "ntext")]
        public string? DatosNuevos { get; set; }
    }

    /// <summary>
    /// Enumeración para los tipos de operaciones de auditoría
    /// </summary>
    public static class TiposOperacionAuditoria
    {
        public const string CREATE = "CREATE";
        public const string UPDATE = "UPDATE";
        public const string DELETE = "DELETE";
        public const string LOGIN = "LOGIN";
        public const string LOGOUT = "LOGOUT";
        public const string ASSIGN = "ASSIGN";
        public const string UNASSIGN = "UNASSIGN";
        public const string TRANSFER = "TRANSFER";
        public const string EXPORT = "EXPORT";
        public const string IMPORT = "IMPORT";
    }
}
