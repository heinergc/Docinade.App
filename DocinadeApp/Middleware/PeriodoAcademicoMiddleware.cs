using RubricasApp.Web.Services;

namespace RubricasApp.Web.Middleware
{
    public class PeriodoAcademicoMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<PeriodoAcademicoMiddleware> _logger;

        // Rutas que no requieren período académico
        private readonly string[] _excludedPaths = {
            "/Account/",
            "/Identity/",
            "/PeriodoAcademico/",
            "/Home/Privacy",
            "/Home/Error",
            "/api/",
            "/css/",
            "/js/",
            "/images/",
            "/favicon.ico"
        };

        public PeriodoAcademicoMiddleware(
            RequestDelegate next, 
            ILogger<PeriodoAcademicoMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // Solo aplicar a usuarios autenticados
                if (context.User.Identity?.IsAuthenticated == true)
                {
                    var path = context.Request.Path.Value ?? "";

                    // Verificar si la ruta está excluida
                    if (!IsExcludedPath(path))
                    {
                        // Obtener servicio de período académico de forma más robusta
                        var periodoService = context.RequestServices.GetService<IPeriodoAcademicoService>();
                        
                        if (periodoService != null)
                        {
                            var userId = context.User.Identity.Name ?? 
                                        context.User.FindFirst("sub")?.Value ?? 
                                        context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ??
                                        "anonymous";

                            // Verificar si el usuario tiene período seleccionado con manejo de errores
                            try
                            {
                                if (!periodoService.TienePeriodoSeleccionado(userId))
                                {
                                    _logger.LogInformation("Usuario {UserId} sin período seleccionado, redirigiendo a selección", userId);
                                    
                                    // Guardar URL original para regresar después
                                    var returnUrl = context.Request.Path + context.Request.QueryString;
                                    context.Response.Redirect($"/PeriodoAcademico/Seleccionar?returnUrl={Uri.EscapeDataString(returnUrl)}");
                                    return;
                                }
                            }
                            catch (Exception serviceEx)
                            {
                                _logger.LogWarning(serviceEx, "Error verificando período para usuario {UserId}, continuando sin redirección", userId);
                                // Continuar sin redirigir en caso de error del servicio para evitar interrumpir transacciones
                            }
                        }
                        else
                        {
                            _logger.LogWarning("PeriodoAcademicoService no está disponible en el contenedor de servicios");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en PeriodoAcademicoMiddleware para ruta {Path}", context.Request.Path);
                // Continuar con el pipeline en caso de error para no interrumpir transacciones activas
            }

            await _next(context);
        }

        private bool IsExcludedPath(string path)
        {
            return _excludedPaths.Any(excludedPath => 
                path.StartsWith(excludedPath, StringComparison.OrdinalIgnoreCase));
        }
    }

    // Extension method para registrar el middleware
    public static class PeriodoAcademicoMiddlewareExtensions
    {
        public static IApplicationBuilder UsePeriodoAcademicoMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<PeriodoAcademicoMiddleware>();
        }
    }}