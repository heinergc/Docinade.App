using Microsoft.EntityFrameworkCore;
using RubricasApp.Web.Data;
using RubricasApp.Web.Models.Audit;

namespace RubricasApp.Web.Services.Audit
{
    /// <summary>
    /// Implementación del servicio de auditoría
    /// </summary>
    public class AuditService : IAuditService
    {
        private readonly RubricasDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<AuditService> _logger;

        public AuditService(
            RubricasDbContext context,
            IHttpContextAccessor httpContextAccessor,
            ILogger<AuditService> logger)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task LogActionAsync(string userId, string action, string entityType, string? entityId = null,
            string? oldValues = null, string? newValues = null, string? additionalInfo = null)
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;
                var ipAddress = GetClientIpAddress();
                var userAgent = httpContext?.Request.Headers["User-Agent"].ToString() ?? "Unknown";

                var auditLog = new AuditLog
                {
                    UserId = userId,
                    Action = action,
                    EntityType = entityType,
                    EntityId = entityId,
                    EntityName = $"{entityType}_{entityId}",
                    OldValues = oldValues,
                    NewValues = newValues,
                    IpAddress = ipAddress,
                    UserAgent = userAgent.Length > 500 ? userAgent.Substring(0, 500) : userAgent,
                    Timestamp = DateTime.UtcNow,
                    LogLevel = "Information",
                    AdditionalInfo = additionalInfo?.Length > 500 ? additionalInfo.Substring(0, 500) : additionalInfo,
                    Success = true,
                    HttpMethod = httpContext?.Request.Method,
                    RequestUrl = httpContext?.Request.Path.ToString()
                };

                _context.AuditLogs.Add(auditLog);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registrando acción de auditoría: {Action} para usuario {UserId}", action, userId);
            }
        }

        public async Task LogLoginAsync(string userId, string ipAddress, string userAgent, bool success = true, string? errorMessage = null)
        {
            try
            {
                var auditLog = new AuditLog
                {
                    UserId = userId,
                    Action = "Auth.Login",
                    EntityType = "Authentication",
                    EntityName = "Login",
                    IpAddress = ipAddress,
                    UserAgent = userAgent.Length > 500 ? userAgent.Substring(0, 500) : userAgent,
                    Timestamp = DateTime.UtcNow,
                    LogLevel = success ? "Information" : "Warning",
                    Success = success,
                    ErrorMessage = errorMessage?.Length > 1000 ? errorMessage.Substring(0, 1000) : errorMessage
                };

                _context.AuditLogs.Add(auditLog);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registrando login de usuario {UserId}", userId);
            }
        }

        public async Task LogLogoutAsync(string userId, string ipAddress, string userAgent)
        {
            try
            {
                var auditLog = new AuditLog
                {
                    UserId = userId,
                    Action = "Auth.Logout",
                    EntityType = "Authentication",
                    EntityName = "Logout",
                    IpAddress = ipAddress,
                    UserAgent = userAgent.Length > 500 ? userAgent.Substring(0, 500) : userAgent,
                    Timestamp = DateTime.UtcNow,
                    LogLevel = "Information",
                    Success = true
                };

                _context.AuditLogs.Add(auditLog);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registrando logout de usuario {UserId}", userId);
            }
        }

        public async Task<List<AuditLog>> GetAuditLogsAsync(DateTime? startDate = null, DateTime? endDate = null,
            string? userId = null, string? action = null, int pageSize = 50, int pageNumber = 1)
        {
            try
            {
                _logger.LogInformation("🔍 GetAuditLogsAsync llamado con: startDate={StartDate}, endDate={EndDate}, userId={UserId}, action={Action}, pageSize={PageSize}, pageNumber={PageNumber}",
                    startDate, endDate, userId, action, pageSize, pageNumber);

                var query = _context.AuditLogs.AsQueryable();

                // Log del conteo total antes de aplicar filtros
                var totalCount = await _context.AuditLogs.CountAsync();
                _logger.LogInformation("📊 Total de registros en AuditLogs: {TotalCount}", totalCount);

                if (startDate.HasValue)
                {
                    query = query.Where(a => a.Timestamp >= startDate.Value);
                    _logger.LogInformation("🕐 Filtro startDate aplicado: {StartDate}", startDate);
                }

                if (endDate.HasValue)
                {
                    query = query.Where(a => a.Timestamp <= endDate.Value);
                    _logger.LogInformation("🕐 Filtro endDate aplicado: {EndDate}", endDate);
                }

                if (!string.IsNullOrEmpty(userId))
                {
                    query = query.Where(a => a.UserId == userId);
                    _logger.LogInformation("👤 Filtro userId aplicado: {UserId}", userId);
                }

                if (!string.IsNullOrEmpty(action))
                {
                    //query = query.Where(a => a.Action.Contains(action));
                    _logger.LogInformation("⚡ Filtro action aplicado: {Action}", action);
                }

                // Log del conteo después de filtros
                var filteredCount = await query.CountAsync();
                _logger.LogInformation("📋 Registros después de filtros: {FilteredCount}", filteredCount);

                var result = await query
                    .OrderByDescending(a => a.Timestamp)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                _logger.LogInformation("✅ Registros devueltos: {ResultCount}", result.Count);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error en GetAuditLogsAsync");
                throw;
            }
        }

        public async Task<List<AuditLog>> GetUserAuditLogsAsync(string userId, int pageSize = 50, int pageNumber = 1)
        {
            return await _context.AuditLogs
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.Timestamp)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<List<AuditLog>> GetEntityAuditLogsAsync(string entityType, string entityId, int pageSize = 50, int pageNumber = 1)
        {
            return await _context.AuditLogs
                .Where(a => a.EntityType == entityType && a.EntityId == entityId)
                .OrderByDescending(a => a.Timestamp)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task CleanupOldLogsAsync(int daysToKeep = 90)
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-daysToKeep);

            var oldLogs = await _context.AuditLogs
                .Where(a => a.Timestamp < cutoffDate)
                .ToListAsync();

            if (oldLogs.Any())
            {
                _context.AuditLogs.RemoveRange(oldLogs);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Eliminados {Count} registros de auditoría anteriores a {CutoffDate}",
                    oldLogs.Count, cutoffDate);
            }
        }

        public async Task<AuditStatistics> GetAuditStatisticsAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _context.AuditLogs.AsQueryable();

            if (startDate.HasValue)
                query = query.Where(a => a.Timestamp >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(a => a.Timestamp <= endDate.Value);

            var logs = await query.ToListAsync();

            var statistics = new AuditStatistics
            {
                TotalLogs = logs.Count,
                TotalUsers = logs.Select(l => l.UserId).Distinct().Count(),
                SuccessfulActions = logs.Count(l => l.Success),
                FailedActions = logs.Count(l => !l.Success),
                ActionsByType = logs.GroupBy(l => l.Action)
                    .ToDictionary(g => g.Key, g => g.Count()),
                LogsByDay = logs.GroupBy(l => l.Timestamp.Date.ToString("yyyy-MM-dd"))
                    .ToDictionary(g => g.Key, g => g.Count()),
                MostActiveUsers = logs.GroupBy(l => l.UserId)
                    .OrderByDescending(g => g.Count())
                    .Take(10)
                    .Select(g => g.Key)
                    .ToList()
            };

            return statistics;
        }

        public async Task LogAsync(string userId, string entityType, string action, string? entityId = null, string? additionalInfo = null)
        {
            await LogActionAsync(userId, action, entityType, entityId, null, null, additionalInfo);
        }

        private string GetClientIpAddress()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null) return "Unknown";

            // Verificar si viene de un proxy o balanceador de carga
            var forwardedFor = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrEmpty(forwardedFor))
            {
                return forwardedFor.Split(',')[0].Trim();
            }

            var realIp = httpContext.Request.Headers["X-Real-IP"].FirstOrDefault();
            if (!string.IsNullOrEmpty(realIp))
            {
                return realIp;
            }

            return httpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        }

        public async Task<int> GetAuditLogsCountAsync(
            string? userId = null,
            string? action = null,
            string? entityType = null,
            DateTime? fromDate = null,
            DateTime? toDate = null
                                        )
        {
            var query = _context.AuditLogs.AsQueryable();

            if (!string.IsNullOrEmpty(userId))
                query = query.Where(a => a.UserId == userId);

            if (!string.IsNullOrEmpty(action))
                query = query.Where(a => a.Action.Contains(action));

            if (!string.IsNullOrEmpty(entityType))
                query = query.Where(a => a.EntityType == entityType);

            if (fromDate.HasValue)
                query = query.Where(a => a.Timestamp >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(a => a.Timestamp <= toDate.Value);

            return await query.CountAsync();
        }

        public async Task<List<string>> GetAvailableActionsAsync()
        {
            return await _context.AuditLogs
                .Select(a => a.Action)
                .Distinct()
                .OrderBy(a => a)
                .ToListAsync();
        }

        public async Task<List<string>> GetAvailableEntityTypesAsync()
        {
            return await _context.AuditLogs
                .Select(a => a.EntityType)
                .Where(e => !string.IsNullOrEmpty(e))
                .Distinct()
                .OrderBy(e => e)
                .ToListAsync();
        }

        public async Task<AuditLog?> GetAuditLogByIdAsync(int id)
        {
            return await _context.AuditLogs.FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<Dictionary<string, int>> GetActionStatisticsAsync()
        {
            return await _context.AuditLogs
                .GroupBy(a => a.Action)
                .ToDictionaryAsync(g => g.Key, g => g.Count());
        }

        public async Task<Dictionary<string, int>> GetEntityStatisticsAsync()
        {
            return await _context.AuditLogs
                .GroupBy(a => a.EntityType)
                .ToDictionaryAsync(g => g.Key, g => g.Count());
        }

        public async Task<Dictionary<string, int>> GetUserActivityStatisticsAsync()
        {
            return await _context.AuditLogs
                .GroupBy(a => a.UserId)
                .ToDictionaryAsync(g => g.Key, g => g.Count());
        }

        public async Task<Dictionary<string, int>> GetDailyActivityStatisticsAsync(DateTime fromDate)
        {
            return await _context.AuditLogs
                .Where(a => a.Timestamp >= fromDate)
                .GroupBy(a => a.Timestamp.Date.ToString("yyyy-MM-dd"))
                .ToDictionaryAsync(g => g.Key, g => g.Count());
        }

        /// <summary>
        /// Método de diagnóstico para verificar el estado de la tabla AuditLogs
        /// </summary>
        public async Task<AuditDiagnosticInfo> GetDiagnosticInfoAsync()
        {
            var totalCount = await _context.AuditLogs.CountAsync();
            var lastEntries = await _context.AuditLogs
                .OrderByDescending(a => a.Timestamp)
                .Take(5)
                .Select(a => new { a.Id, a.Action, a.Timestamp, a.UserId })
                .ToListAsync();

            var distinctActions = await _context.AuditLogs
                .Select(a => a.Action)
                .Distinct()
                .Take(10)
                .ToListAsync();

            return new AuditDiagnosticInfo
            {
                TotalRecords = totalCount,
                LastEntries = lastEntries.Select(e => $"Id: {e.Id}, Action: {e.Action}, Time: {e.Timestamp}, User: {e.UserId}").ToList(),
                DistinctActions = distinctActions
            };
        }


    }

    public class AuditDiagnosticInfo
    {
        public int TotalRecords { get; set; }
        public List<string> LastEntries { get; set; } = new();
        public List<string> DistinctActions { get; set; } = new();
    }
}

