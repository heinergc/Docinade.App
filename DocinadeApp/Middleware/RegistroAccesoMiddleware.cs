using RubricasApp.Web.Services;

namespace RubricasApp.Web.Middleware
{
    public class RegistroAccesoMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RegistroAccesoMiddleware> _logger;

        public RegistroAccesoMiddleware(RequestDelegate next, ILogger<RegistroAccesoMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, IConfiguracionService configuracionService)
        {
            // Verificar si es una solicitud a la página de registro
            if (context.Request.Path.StartsWithSegments("/Account/Register", StringComparison.OrdinalIgnoreCase))
            {
                var esRegistroAbierto = await configuracionService.EsRegistroAbiertoAsync();
                if (!esRegistroAbierto)
                {
                    // Registrar información detallada del intento bloqueado
                    var ip = context.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
                    var userAgent = context.Request.Headers["User-Agent"].ToString();
                    var referer = context.Request.Headers["Referer"].ToString();
                    var timestamp = DateTime.UtcNow;

                    _logger.LogWarning(
                        "Acceso bloqueado al registro. IP: {IP}, UserAgent: {UserAgent}, Referer: {Referer}, Timestamp: {Timestamp}",
                        ip, userAgent, referer, timestamp);

                    // Agregar información adicional para estadísticas
                    context.Items["AccessBlocked"] = true;
                    context.Items["BlockedTimestamp"] = timestamp;
                    context.Items["BlockedIP"] = ip;
                }
            }

            await _next(context);
        }
    }

    // Extension method para facilitar el registro del middleware
    public static class RegistroAccesoMiddlewareExtensions
    {
        public static IApplicationBuilder UseRegistroAcceso(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RegistroAccesoMiddleware>();
        }
    }
}