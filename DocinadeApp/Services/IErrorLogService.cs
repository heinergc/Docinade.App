using DocinadeApp.Models;
using System;
using System.Threading.Tasks;

namespace DocinadeApp.Services
{
    public interface IErrorLogService
    {
        Task LogErrorAsync(
            string controllerName,
            string actionName,
            Exception exception,
            string? userId = null,
            string? userName = null,
            string? requestPath = null,
            string? requestMethod = null,
            string? requestBody = null,
            string? ipAddress = null,
            string? userAgent = null);
    }
}
