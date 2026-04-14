using Microsoft.EntityFrameworkCore;
using RubricasApp.Web.Data;
using RubricasApp.Web.Models;
using System;
using System.Threading.Tasks;

namespace RubricasApp.Web.Services
{
    public class ErrorLogService : IErrorLogService
    {
        private readonly RubricasDbContext _context;

        public ErrorLogService(RubricasDbContext context)
        {
            _context = context;
        }

        public async Task LogErrorAsync(
            string controllerName,
            string actionName,
            Exception exception,
            string? userId = null,
            string? userName = null,
            string? requestPath = null,
            string? requestMethod = null,
            string? requestBody = null,
            string? ipAddress = null,
            string? userAgent = null)
        {
            try
            {
                var errorLog = new ErrorLog
                {
                    ControllerName = controllerName,
                    ActionName = actionName,
                    ErrorMessage = exception.Message,
                    StackTrace = exception.StackTrace,
                    InnerException = exception.InnerException?.Message,
                    UserId = userId,
                    UserName = userName,
                    RequestPath = requestPath,
                    RequestMethod = requestMethod,
                    RequestBody = requestBody,
                    IpAddress = ipAddress,
                    UserAgent = userAgent,
                    CreatedAt = DateTime.Now
                };

                _context.ErrorLogs.Add(errorLog);
                await _context.SaveChangesAsync();

                // Log adicional en consola para desarrollo
                Console.WriteLine($"[ERROR LOG] {controllerName}.{actionName}: {exception.Message}");
                if (exception.InnerException != null)
                {
                    Console.WriteLine($"[INNER EXCEPTION] {exception.InnerException.Message}");
                }
            }
            catch (Exception ex)
            {
                // Si falla el guardado en base de datos, al menos loguear en consola
                Console.WriteLine($"[ERROR LOGGING FAILED] {ex.Message}");
                Console.WriteLine($"[ORIGINAL ERROR] {controllerName}.{actionName}: {exception.Message}");
            }
        }
    }
}
