using RubricasApp.Web.Models.Audit;

namespace RubricasApp.Web.Services.Audit
{
    /// <summary>
    /// Interfaz para el servicio de auditoría
    /// </summary>
    public interface IAuditService
    {
        /// <summary>
        /// Registra una acción de auditoría
        /// </summary>
        Task LogActionAsync(string userId, string action, string entityType, string? entityId = null, 
            string? oldValues = null, string? newValues = null, string? additionalInfo = null);

        /// <summary>
        /// Registra un inicio de sesión
        /// </summary>
        Task LogLoginAsync(string userId, string ipAddress, string userAgent, bool success = true, string? errorMessage = null);

        /// <summary>
        /// Registra un cierre de sesión
        /// </summary>
        Task LogLogoutAsync(string userId, string ipAddress, string userAgent);

        /// <summary>
        /// Obtiene registros de auditoría con filtros
        /// </summary>
        Task<List<AuditLog>> GetAuditLogsAsync(DateTime? startDate = null, DateTime? endDate = null, 
            string? userId = null, string? action = null, int pageSize = 50, int pageNumber = 1);

        /// <summary>
        /// Obtiene registros de auditoría de un usuario específico
        /// </summary>
        Task<List<AuditLog>> GetUserAuditLogsAsync(string userId, int pageSize = 50, int pageNumber = 1);

        /// <summary>
        /// Obtiene registros de auditoría de una entidad específica
        /// </summary>
        Task<List<AuditLog>> GetEntityAuditLogsAsync(string entityType, string entityId, int pageSize = 50, int pageNumber = 1);

        /// <summary>
        /// Limpia registros de auditoría antiguos
        /// </summary>
        Task CleanupOldLogsAsync(int daysToKeep = 90);

        /// <summary>
        /// Obtiene estadísticas de auditoría
        /// </summary>
        Task<AuditStatistics> GetAuditStatisticsAsync(DateTime? startDate = null, DateTime? endDate = null);

        /// <summary>
        /// Método conveniente para registrar una acción de auditoría (alias de LogActionAsync)
        /// </summary>
        Task LogAsync(string userId, string entityType, string action, string? entityId = null, string? additionalInfo = null);

        /// <summary>
        /// Obtiene el conteo de registros de auditoría con filtros
        /// </summary>
        Task<int> GetAuditLogsCountAsync(
            string? userId = null,
            string? action = null,
            string? entityType = null,
            DateTime? fromDate = null,
            DateTime? toDate = null
        );

        /// <summary>
        /// Obtiene la lista de acciones disponibles en los registros de auditoría
        /// </summary>
        Task<List<string>> GetAvailableActionsAsync();

        /// <summary>
        /// Obtiene la lista de tipos de entidad disponibles en los registros de auditoría
        /// </summary>
        Task<List<string>> GetAvailableEntityTypesAsync();
        /// <summary>
        /// Obtiene un registro de auditoría por su Id
        /// </summary>
        Task<AuditLog?> GetAuditLogByIdAsync(int id);

        /// <summary>
        /// Estadísticas de acciones de auditoría
        /// </summary>
        Task<Dictionary<string, int>> GetActionStatisticsAsync();

        /// <summary>
        /// Estadísticas de entidades auditadas
        /// </summary>
        Task<Dictionary<string, int>> GetEntityStatisticsAsync();

        /// <summary>
        /// Estadísticas de actividad de usuarios
        /// </summary>
        Task<Dictionary<string, int>> GetUserActivityStatisticsAsync();

        /// <summary>
        /// Estadísticas diarias de actividad de auditoría
        /// </summary>
        Task<Dictionary<string, int>> GetDailyActivityStatisticsAsync(DateTime fromDate);
    }

    /// <summary>
    /// Estadísticas de auditoría
    /// </summary>
    public class AuditStatistics
    {
        public int TotalLogs { get; set; }
        public int TotalUsers { get; set; }
        public int SuccessfulActions { get; set; }
        public int FailedActions { get; set; }
        public Dictionary<string, int> ActionsByType { get; set; } = new();
        public Dictionary<string, int> LogsByDay { get; set; } = new();
        public List<string> MostActiveUsers { get; set; } = new();
    }
}