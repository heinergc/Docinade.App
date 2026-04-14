using System.ComponentModel.DataAnnotations;

namespace DocinadeApp.Models.Audit
{
    /// <summary>
    /// Registro de auditoría para seguimiento de acciones de usuarios
    /// </summary>
    public class AuditLog
    {
        public int Id { get; set; }

        [Required]
        [StringLength(450)]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Action { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string EntityType { get; set; } = string.Empty;

        [StringLength(50)]
        public string? EntityId { get; set; }

        [StringLength(200)]
        public string EntityName { get; set; } = string.Empty;

        public string? OldValues { get; set; }
        public string? NewValues { get; set; }

        [StringLength(45)]
        public string IpAddress { get; set; } = string.Empty;

        [StringLength(500)]
        public string UserAgent { get; set; } = string.Empty;

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        [StringLength(50)]
        public string LogLevel { get; set; } = "Information";

        [StringLength(500)]
        public string? AdditionalInfo { get; set; }

        public bool Success { get; set; } = true;

        [StringLength(1000)]
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// Duración de la operación (en millisegundos)
        /// </summary>
        public long? DurationMs { get; set; }

        /// <summary>
        /// Identificador de sesión
        /// </summary>
        [StringLength(36)]
        public string? SessionId { get; set; }

        /// <summary>
        /// Información del navegador/cliente
        /// </summary>
        [StringLength(100)]
        public string? ClientInfo { get; set; }

        /// <summary>
        /// Referrer de la acción
        /// </summary>
        [StringLength(500)]
        public string? Referrer { get; set; }

        /// <summary>
        /// Datos adicionales en formato JSON
        /// </summary>
        public string? Metadata { get; set; }

        /// <summary>
        /// Método HTTP utilizado
        /// </summary>
        [StringLength(10)]
        public string? HttpMethod { get; set; }

        /// <summary>
        /// URL de la acción
        /// </summary>
        [StringLength(500)]
        public string? RequestUrl { get; set; }

        /// <summary>
        /// Código de respuesta HTTP
        /// </summary>
        public int? ResponseStatusCode { get; set; }
    }
}