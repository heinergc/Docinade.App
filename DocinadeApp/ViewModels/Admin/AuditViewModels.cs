using System.ComponentModel.DataAnnotations;

namespace DocinadeApp.ViewModels.Admin
{
    /// <summary>
    /// ViewModel para lista de auditoría
    /// </summary>
    public class AuditListViewModel : AdminBaseViewModel
    {
        public List<AuditLogViewModel> AuditLogs { get; set; } = new();
        public AuditFilterViewModel Filter { get; set; } = new();
        public int TotalRecords { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        
        // Propiedades adicionales compatibles
        public int PageSize { get; set; } = 50;
        public int TotalLogs { get; set; }
        
        // Filtros adicionales para compatibilidad
        public string? SearchTerm { get; set; }
        public string? SelectedAction { get; set; }
        public string? SelectedEntityType { get; set; }
        public string? SelectedUserId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool? SuccessFilter { get; set; }
        
        // Opciones para filtros
        public List<string> AvailableActions { get; set; } = new();
        public List<string> AvailableEntityTypes { get; set; } = new();
        public List<UserViewModel> AvailableUsers { get; set; } = new();
    }

    /// <summary>
    /// ViewModel para filtros de auditoría
    /// </summary>
    public class AuditFilterViewModel
    {
        public string? UserId { get; set; }
        public string? Action { get; set; }
        public string? EntityType { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? LogLevel { get; set; }
        public bool? Success { get; set; }
        public string? SearchTerm { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 50;
        
        // Opciones para filtros
        public List<string> AvailableActions { get; set; } = new();
        public List<string> AvailableEntityTypes { get; set; } = new();
        public List<string> AvailableLogLevels { get; set; } = new();

        /// <summary>
        /// Verifica si hay filtros activos (excluyendo page y pageSize)
        /// </summary>
        /// <returns>True si hay filtros activos</returns>
        public bool HasActiveFilters()
        {
            return !string.IsNullOrWhiteSpace(UserId) ||
                   !string.IsNullOrWhiteSpace(Action) ||
                   !string.IsNullOrWhiteSpace(EntityType) ||
                   FromDate.HasValue ||
                   ToDate.HasValue ||
                   !string.IsNullOrWhiteSpace(LogLevel) ||
                   Success.HasValue ||
                   !string.IsNullOrWhiteSpace(SearchTerm);
        }

        /// <summary>
        /// Obtiene una descripción de los filtros activos
        /// </summary>
        /// <returns>Lista de filtros activos</returns>
        public List<string> GetActiveFiltersDescription()
        {
            var activeFilters = new List<string>();

            if (!string.IsNullOrWhiteSpace(Action))
                activeFilters.Add($"Acción: {Action}");
            
            if (!string.IsNullOrWhiteSpace(EntityType))
                activeFilters.Add($"Entidad: {EntityType}");
            
            if (FromDate.HasValue)
                activeFilters.Add($"Desde: {FromDate.Value:dd/MM/yyyy}");
            
            if (ToDate.HasValue)
                activeFilters.Add($"Hasta: {ToDate.Value:dd/MM/yyyy}");
            
            if (!string.IsNullOrWhiteSpace(LogLevel))
                activeFilters.Add($"Nivel: {LogLevel}");
            
            if (Success.HasValue)
                activeFilters.Add($"Resultado: {(Success.Value ? "Exitosos" : "Con errores")}");
            
            if (!string.IsNullOrWhiteSpace(SearchTerm))
                activeFilters.Add($"Búsqueda: '{SearchTerm}'");
            
            if (!string.IsNullOrWhiteSpace(UserId))
                activeFilters.Add($"Usuario: {UserId}");

            return activeFilters;
        }

        /// <summary>
        /// Limpia todos los filtros
        /// </summary>
        public void ClearFilters()
        {
            UserId = null;
            Action = null;
            EntityType = null;
            FromDate = null;
            ToDate = null;
            LogLevel = null;
            Success = null;
            SearchTerm = null;
            Page = 1;
            // PageSize se mantiene
        }
    }

    /// <summary>
    /// ViewModel para estadísticas de auditoría
    /// </summary>
    public class AuditStatisticsViewModel
    {
        public Dictionary<string, int> ActionStatistics { get; set; } = new();
        public Dictionary<string, int> EntityStatistics { get; set; } = new();
        public Dictionary<string, int> UserActivityStatistics { get; set; } = new();
        public Dictionary<string, int> DailyActivityStatistics { get; set; } = new();
        public int TotalLogsToday { get; set; }
        public int TotalLogsThisWeek { get; set; }
        public int TotalLogsThisMonth { get; set; }
        public int ErrorCount { get; set; }
        public List<string> MostActiveUsers { get; set; } = new();
        public List<string> MostCommonActions { get; set; } = new();
    }
    /// <summary>
    /// ViewModel para entrada de auditoría
    /// </summary>
    public class AuditLogViewModel
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string EntityType { get; set; } = string.Empty;
        public string? EntityId { get; set; }
        public string EntityName { get; set; } = string.Empty;
        public string? OldValues { get; set; }
        public string? NewValues { get; set; }
        public string? Details { get; set; }
        public DateTime Timestamp { get; set; }
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
        public string LogLevel { get; set; } = "Information";
        public string? AdditionalInfo { get; set; }
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public TimeSpan? Duration { get; set; }
        public long? DurationMs { get; set; }
        public string? SessionId { get; set; }
        public string? ClientInfo { get; set; }
        public string? Referrer { get; set; }
        public string? Metadata { get; set; }
        public string? HttpMethod { get; set; }
        public string? RequestUrl { get; set; }
        public int? ResponseStatusCode { get; set; }
    }

    /// <summary>
    /// ViewModel para detalles de auditoría
    /// </summary>
    public class AuditDetailsViewModel : AuditLogViewModel
    {
        public Dictionary<string, object?> ParsedOldValues { get; set; } = new();
        public Dictionary<string, object?> ParsedNewValues { get; set; } = new();
        public List<AuditLogViewModel> RelatedLogs { get; set; } = new();
    }

    /// <summary>
    /// ViewModel para estadísticas de auditoría
    /// </summary>
    public class AuditStatsViewModel : AdminBaseViewModel
    {
        public int TotalLogs { get; set; }
        public int LogsToday { get; set; }
        public int LogsThisWeek { get; set; }
        public int LogsThisMonth { get; set; }
        public int SuccessfulActions { get; set; }
        public int FailedActions { get; set; }
        public int UniqueUsers { get; set; }
        
        // Top actividades
        public List<ActionStatistic> TopActions { get; set; } = new();
        public List<UserStatistic> MostActiveUsers { get; set; } = new();
        public List<EntityStatistic> MostAuditedEntities { get; set; } = new();
        
        // Tendencias
        public Dictionary<string, int> DailyActivityTrend { get; set; } = new();
        public Dictionary<string, int> HourlyActivityTrend { get; set; } = new();
        public Dictionary<string, double> SuccessRateTrend { get; set; } = new();
    }

    /// <summary>
    /// Estadística de acción
    /// </summary>
    public class ActionStatistic
    {
        public string Action { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public int Count { get; set; }
        public double Percentage { get; set; }
        public int SuccessCount { get; set; }
        public int FailureCount { get; set; }
    }

    /// <summary>
    /// Estadística de usuario
    /// </summary>
    public class UserStatistic
    {
        public string UserId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int ActionCount { get; set; }
        public DateTime LastActivity { get; set; }
        public List<string> TopActions { get; set; } = new();
    }

    /// <summary>
    /// Estadística de entidad
    /// </summary>
    public class EntityStatistic
    {
        public string EntityType { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public int ActionCount { get; set; }
        public int CreateCount { get; set; }
        public int UpdateCount { get; set; }
        public int DeleteCount { get; set; }
        public int ViewCount { get; set; }
    }

    /// <summary>
    /// ViewModel para exportar auditoría
    /// </summary>
    public class ExportAuditViewModel
    {
        [Display(Name = "Formato de exportación")]
        public string Format { get; set; } = "CSV"; // CSV, Excel, JSON

        [Display(Name = "Fecha de inicio")]
        [DataType(DataType.Date)]
        public DateTime? StartDate { get; set; }

        [Display(Name = "Fecha de fin")]
        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }

        [Display(Name = "Acciones específicas")]
        public List<string>? SelectedActions { get; set; }

        [Display(Name = "Tipos de entidad")]
        public List<string>? SelectedEntityTypes { get; set; }

        [Display(Name = "Usuarios específicos")]
        public List<string>? SelectedUserIds { get; set; }

        [Display(Name = "Solo acciones exitosas")]
        public bool? SuccessOnly { get; set; }

        [Display(Name = "Incluir detalles")]
        public bool IncludeDetails { get; set; } = true;

        [Display(Name = "Límite de registros")]
        [Range(1, 100000, ErrorMessage = "El límite debe estar entre 1 y 100,000")]
        public int? MaxRecords { get; set; }

        // Opciones disponibles
        public List<string> AvailableFormats { get; set; } = new() { "CSV", "Excel", "JSON" };
        public List<string> AvailableActions { get; set; } = new();
        public List<string> AvailableEntityTypes { get; set; } = new();
        public List<UserViewModel> AvailableUsers { get; set; } = new();
    }
}