using Microsoft.EntityFrameworkCore;
using RubricasApp.Web.Data;
using RubricasApp.Web.Models;

namespace RubricasApp.Web.Services
{
    public class PeriodoAcademicoService : IPeriodoAcademicoService
    {
        private readonly RubricasDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<PeriodoAcademicoService> _logger;
        
        private const string SESSION_KEY = "PeriodoAcademicoActivo";
        private const string COOKIE_KEY = "UltimoPeriodoAcademico";

        public PeriodoAcademicoService(
            RubricasDbContext context, 
            IHttpContextAccessor httpContextAccessor,
            ILogger<PeriodoAcademicoService> logger)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task<PeriodoAcademico?> GetPeriodoActivoAsync(string userId)
        {
            try
            {
                // 1. Primero verificar la sesión
                var periodoSesion = GetPeriodoActivoDeSesion();
                if (periodoSesion != null)
                {
                    return periodoSesion;
                }

                // 2. Buscar en cookies como respaldo
                var periodoIdFromCookie = GetPeriodoFromCookie();
                if (periodoIdFromCookie.HasValue)
                {
                    var periodoFromCookie = await _context.PeriodosAcademicos
                        .FirstOrDefaultAsync(p => p.Id == periodoIdFromCookie.Value);
                    
                    if (periodoFromCookie != null)
                    {
                        SetPeriodoActivoEnSesion(periodoFromCookie);
                        return periodoFromCookie;
                    }
                }

                // 3. Si no hay nada, obtener el período por defecto
                var periodoDefault = await GetPeriodoDefaultAsync();
                if (periodoDefault != null)
                {
                    SetPeriodoActivoEnSesion(periodoDefault);
                    SetPeriodoInCookie(periodoDefault.Id);
                }

                return periodoDefault;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener período activo para usuario {UserId}", userId);
                return await GetPeriodoDefaultAsync();
            }
        }

        public async Task SetPeriodoActivoAsync(string userId, int periodoId)
        {
            try
            {
                // Verificar que el período existe y el usuario puede accederlo
                var periodo = await _context.PeriodosAcademicos
                    .FirstOrDefaultAsync(p => p.Id == periodoId);

                if (periodo == null)
                {
                    throw new ArgumentException($"Período académico con ID {periodoId} no encontrado");
                }

                if (!await UsuarioPuedeAccederPeriodo(userId, periodoId))
                {
                    throw new UnauthorizedAccessException($"Usuario {userId} no tiene permisos para acceder al período {periodoId}");
                }

                // Establecer en sesión y cookie
                SetPeriodoActivoEnSesion(periodo);
                SetPeriodoInCookie(periodoId);

                _logger.LogInformation("Período académico {PeriodoId} establecido para usuario {UserId}", 
                    periodoId, userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al establecer período activo {PeriodoId} para usuario {UserId}", 
                    periodoId, userId);
                throw;
            }
        }

        public async Task<IEnumerable<PeriodoAcademico>> GetPeriodosDisponiblesAsync()
        {
            try
            {
                return await _context.PeriodosAcademicos
                    .Where(p => p.Activo) // Usar campo boolean Activo en lugar de Estado string
                    .OrderByDescending(p => p.FechaInicio)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener períodos disponibles");
                return new List<PeriodoAcademico>();
            }
        }

        public async Task<PeriodoAcademico?> GetPeriodoDefaultAsync()
        {
            try
            {
                // Obtener el período más reciente que esté activo
                return await _context.PeriodosAcademicos
                    .Where(p => p.Activo) // Usar campo boolean Activo
                    .OrderByDescending(p => p.FechaInicio)
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener período por defecto");
                return null;
            }
        }

        public bool TienePeriodoSeleccionado(string userId)
        {
            var periodoSesion = GetPeriodoActivoDeSesion();
            var periodoIdFromCookie = GetPeriodoFromCookie();
            
            return periodoSesion != null || periodoIdFromCookie.HasValue;
        }

        public PeriodoAcademico? GetPeriodoActivoDeSesion()
        {
            try
            {
                var session = _httpContextAccessor.HttpContext?.Session;
                if (session == null) return null;

                var periodoJson = session.GetString(SESSION_KEY);
                if (string.IsNullOrEmpty(periodoJson)) return null;

                return System.Text.Json.JsonSerializer.Deserialize<PeriodoAcademico>(periodoJson);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener período de sesión");
                return null;
            }
        }

        public void SetPeriodoActivoEnSesion(PeriodoAcademico periodo)
        {
            try
            {
                var session = _httpContextAccessor.HttpContext?.Session;
                if (session == null) return;

                var periodoJson = System.Text.Json.JsonSerializer.Serialize(periodo);
                session.SetString(SESSION_KEY, periodoJson);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al establecer período en sesión");
            }
        }

        public void LimpiarPeriodoActivoDeSesion()
        {
            try
            {
                var session = _httpContextAccessor.HttpContext?.Session;
                session?.Remove(SESSION_KEY);

                // También limpiar cookie
                var response = _httpContextAccessor.HttpContext?.Response;
                response?.Cookies.Delete(COOKIE_KEY);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al limpiar período de sesión");
            }
        }

        public async Task<bool> UsuarioPuedeAccederPeriodo(string userId, int periodoId)
        {
            try
            {
                // Por ahora, todos los usuarios autenticados pueden acceder a todos los períodos activos
                // En el futuro se puede implementar lógica más específica según roles
                
                var periodo = await _context.PeriodosAcademicos
                    .FirstOrDefaultAsync(p => p.Id == periodoId && p.Activo); // Usar campo boolean
                
                return periodo != null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar permisos de usuario {UserId} para período {PeriodoId}", 
                    userId, periodoId);
                return false;
            }
        }

        private int? GetPeriodoFromCookie()
        {
            try
            {
                var request = _httpContextAccessor.HttpContext?.Request;
                if (request == null) return null;

                var cookieValue = request.Cookies[COOKIE_KEY];
                if (string.IsNullOrEmpty(cookieValue)) return null;

                return int.TryParse(cookieValue, out var periodoId) ? periodoId : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener período de cookie");
                return null;
            }
        }

        private void SetPeriodoInCookie(int periodoId)
        {
            try
            {
                var response = _httpContextAccessor.HttpContext?.Response;
                if (response == null) return;

                var cookieOptions = new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddDays(30),
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict
                };

                response.Cookies.Append(COOKIE_KEY, periodoId.ToString(), cookieOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al establecer período en cookie");
            }
        }
    }
}