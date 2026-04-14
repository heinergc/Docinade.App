using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DocinadeApp.Services;
using DocinadeApp.Models;

namespace DocinadeApp.Controllers
{
    public class InvitacionController : Controller
    {
        private readonly IConfiguracionService _configuracionService;
        private readonly ILogger<InvitacionController> _logger;

        public InvitacionController(IConfiguracionService configuracionService, ILogger<InvitacionController> logger)
        {
            _configuracionService = configuracionService;
            _logger = logger;
        }

        /// <summary>
        /// Acceso especial mediante token de invitación (funcionalidad futura)
        /// </summary>
        [AllowAnonymous]
        public async Task<IActionResult> Acceso(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Register", "Account");
            }

            // Verificar si las invitaciones especiales están habilitadas
            var permitirInvitaciones = await _configuracionService.ObtenerConfiguracionAsync(
                ConfiguracionClaves.PermitirInvitacionesEspeciales, "false");

            if (permitirInvitaciones.ToLower() != "true")
            {
                _logger.LogWarning("Intento de acceso por invitación deshabilitado. Token: {Token}", token);
                return RedirectToAction("Register", "Account");
            }

            // TODO: Implementar validación de token de invitación
            // Por ahora, redirigir al registro normal
            _logger.LogInformation("Acceso por invitación con token: {Token}", token);
            
            // En el futuro, aquí se validaría el token y se permitiría el registro
            // incluso con el modo cerrado
            TempData["InvitationToken"] = token;
            TempData["InvitationMessage"] = "Acceso autorizado mediante invitación especial.";
            
            return RedirectToAction("Register", "Account");
        }
    }
}