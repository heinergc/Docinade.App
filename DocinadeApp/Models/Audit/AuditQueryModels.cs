namespace DocinadeApp.Models.Audit
{
    /// <summary>
    /// Filtros para consultas de auditoría
    /// </summary>
    public class AuditFilter
    {
        public string? UserId { get; set; }
        public string? UserName { get; set; }
        public string? Action { get; set; }
        public string? EntityType { get; set; }
        public string? EntityId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? LogLevel { get; set; }
        public bool? Success { get; set; }
        public string? IpAddress { get; set; }
        public string? SearchTerm { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 50;
        public string SortField { get; set; } = "Timestamp";
        public bool SortDescending { get; set; } = true;
    }

    /// <summary>
    /// Resultado paginado de consultas de auditoría
    /// </summary>
    public class AuditQueryResult
    {
        public List<AuditLog> Logs { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageCount { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < PageCount;
        public AuditFilter Filter { get; set; } = new();
    }

    /// <summary>
    /// Estadísticas de auditoría
    /// </summary>
    public class AuditStatistics
    {
        public int TotalLogs { get; set; }
        public int TotalUsers { get; set; }
        public int LogsToday { get; set; }
        public int LogsThisWeek { get; set; }
        public int LogsThisMonth { get; set; }
        public int SuccessfulActions { get; set; }
        public int FailedActions { get; set; }
        public double SuccessRate { get; set; }
        public Dictionary<string, int> ActionCounts { get; set; } = new();
        public Dictionary<string, int> EntityTypeCounts { get; set; } = new();
        public Dictionary<string, int> LogLevelCounts { get; set; } = new();
        public Dictionary<string, int> UserActivityCounts { get; set; } = new();
        public Dictionary<string, int> HourlyDistribution { get; set; } = new();
        public Dictionary<string, int> DailyDistribution { get; set; } = new();
        public List<TopUser> TopUsers { get; set; } = new();
        public List<TopAction> TopActions { get; set; } = new();
        public List<RecentError> RecentErrors { get; set; } = new();
    }

    /// <summary>
    /// Usuario más activo
    /// </summary>
    public class TopUser
    {
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int ActionCount { get; set; }
        public DateTime LastActivity { get; set; }
        public List<string> TopActions { get; set; } = new();
    }

    /// <summary>
    /// Acción más frecuente
    /// </summary>
    public class TopAction
    {
        public string Action { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public int Count { get; set; }
        public double Percentage { get; set; }
        public int SuccessCount { get; set; }
        public int FailureCount { get; set; }
    }

    /// <summary>
    /// Error reciente
    /// </summary>
    public class RecentError
    {
        public int Id { get; set; }
        public string Action { get; set; } = string.Empty;
        public string EntityType { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public int Frequency { get; set; }
    }

    /// <summary>
    /// Configuración de retención de logs
    /// </summary>
    public class AuditRetentionPolicy
    {
        public int RetentionDays { get; set; } = 90;
        public bool AutoCleanup { get; set; } = true;
        public int CleanupBatchSize { get; set; } = 1000;
        public TimeSpan CleanupInterval { get; set; } = TimeSpan.FromDays(1);
        public string[] CriticalActionsToKeep { get; set; } = Array.Empty<string>();
        public bool ArchiveBeforeDelete { get; set; } = true;
        public string? ArchivePath { get; set; }
    }

    /// <summary>
    /// Configuración de exportación de auditoría
    /// </summary>
    public class AuditExportConfiguration
    {
        public AuditExportFormat Format { get; set; } = AuditExportFormat.CSV;
        public bool IncludeHeaders { get; set; } = true;
        public bool IncludeMetadata { get; set; } = false;
        public string? CustomDelimiter { get; set; }
        public List<string> ColumnsToInclude { get; set; } = new();
        public int MaxRecords { get; set; } = 10000;
        public bool CompressOutput { get; set; } = false;
    }

    /// <summary>
    /// Formatos de exportación
    /// </summary>
    public enum AuditExportFormat
    {
        CSV,
        Excel,
        JSON,
        XML,
        PDF
    }

    /// <summary>
    /// Configuración de alertas de auditoría
    /// </summary>
    public class AuditAlertConfiguration
    {
        public bool EnableFailureAlerts { get; set; } = true;
        public int FailureThreshold { get; set; } = 10;
        public TimeSpan FailureTimeWindow { get; set; } = TimeSpan.FromMinutes(15);
        
        public bool EnableSuspiciousActivityAlerts { get; set; } = true;
        public int SuspiciousActionThreshold { get; set; } = 50;
        public TimeSpan SuspiciousTimeWindow { get; set; } = TimeSpan.FromMinutes(5);
        
        public bool EnableUnauthorizedAccessAlerts { get; set; } = true;
        public List<string> CriticalActions { get; set; } = new();
        public List<string> AlertRecipients { get; set; } = new();
        
        public bool EnableReportScheduling { get; set; } = false;
        public TimeSpan ReportInterval { get; set; } = TimeSpan.FromDays(7);
        public List<string> ReportRecipients { get; set; } = new();
    }
}