namespace RubricasApp.Web.ViewModels.Admin
{
    /// <summary>
    /// ViewModel para el dashboard de administración
    /// </summary>
    public class AdminDashboardViewModel
    {
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
        public int TotalRoles { get; set; }
        public int TotalPermissions { get; set; }
        public int TotalAuditLogs { get; set; }
        public int LoginsToday { get; set; }
        public int LoginsThisWeek { get; set; }
        public int ErrorsToday { get; set; }
        public List<UserActivitySummary> RecentUserActivity { get; set; } = new();
        public List<SystemAlert> SystemAlerts { get; set; } = new();
        public Dictionary<string, int> RoleDistribution { get; set; } = new();
        public Dictionary<string, int> WeeklyLoginTrend { get; set; } = new();
    }

    /// <summary>
    /// Resumen de actividad del usuario
    /// </summary>
    public class UserActivitySummary
    {
        public string UserId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
        public DateTime LastActivity { get; set; }
        public string LastAction { get; set; } = string.Empty;
        public int ActivityCount { get; set; }
    }

    /// <summary>
    /// Alerta del sistema
    /// </summary>
    public class SystemAlert
    {
        public string Type { get; set; } = string.Empty; // Info, Warning, Error, Success
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public string? ActionUrl { get; set; }
    }
}