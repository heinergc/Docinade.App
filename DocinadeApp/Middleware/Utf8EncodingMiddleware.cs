using System.Text;

namespace RubricasApp.Web.Middleware
{
    /// <summary>
    /// Middleware para asegurar que todas las respuestas usen codificación UTF-8
    /// </summary>
    public class Utf8EncodingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<Utf8EncodingMiddleware> _logger;

        public Utf8EncodingMiddleware(RequestDelegate next, ILogger<Utf8EncodingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Configurar headers de respuesta para UTF-8 antes de que se inicie la respuesta
            context.Response.OnStarting(() =>
            {
                try
                {
                    // Configurar Content-Type con UTF-8 si no está establecido
                    if (string.IsNullOrEmpty(context.Response.ContentType))
                    {
                        context.Response.ContentType = "text/html; charset=utf-8";
                    }
                    else if (!context.Response.ContentType.Contains("charset", StringComparison.OrdinalIgnoreCase))
                    {
                        // Agregar charset UTF-8 si no está presente
                        if (context.Response.ContentType.StartsWith("text/", StringComparison.OrdinalIgnoreCase) ||
                            context.Response.ContentType.Contains("application/json", StringComparison.OrdinalIgnoreCase) ||
                            context.Response.ContentType.Contains("application/javascript", StringComparison.OrdinalIgnoreCase))
                        {
                            context.Response.ContentType += "; charset=utf-8";
                        }
                    }

                    // Headers adicionales para asegurar UTF-8
                    if (!context.Response.Headers.ContainsKey("Content-Language"))
                    {
                        context.Response.Headers.Add("Content-Language", "es-ES");
                    }

                    // Meta tag para UTF-8 en respuestas HTML
                    if (context.Response.ContentType?.Contains("text/html", StringComparison.OrdinalIgnoreCase) == true)
                    {
                        context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Error configurando UTF-8 en response headers");
                }

                return Task.CompletedTask;
            });

            // Configurar request para usar UTF-8
            try
            {
                if (!string.IsNullOrEmpty(context.Request.ContentType) && 
                    !context.Request.ContentType.Contains("charset", StringComparison.OrdinalIgnoreCase) &&
                    (context.Request.ContentType.Contains("text/", StringComparison.OrdinalIgnoreCase) || 
                     context.Request.ContentType.Contains("application/json", StringComparison.OrdinalIgnoreCase)))
                {
                    context.Request.ContentType += "; charset=utf-8";
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error configurando UTF-8 en request headers");
            }

            await _next(context);
        }
    }

    /// <summary>
    /// Extensión para registrar el middleware de UTF-8
    /// </summary>
    public static class Utf8EncodingMiddlewareExtensions
    {
        public static IApplicationBuilder UseUtf8Encoding(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<Utf8EncodingMiddleware>();
        }
    }
}