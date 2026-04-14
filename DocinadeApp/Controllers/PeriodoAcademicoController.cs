using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RubricasApp.Web.Data;
using RubricasApp.Web.Services;
using RubricasApp.Web.ViewModels;

namespace RubricasApp.Web.Controllers
{
    [Authorize]
    public class PeriodoAcademicoController : BaseController
    {
        private readonly ILogger<PeriodoAcademicoController> _logger;

        public PeriodoAcademicoController(
            IPeriodoAcademicoService periodoService,
            RubricasDbContext context,
            ILogger<PeriodoAcademicoController> logger) : base(periodoService, context)
        {
            _logger = logger;
        }

        /// <summary>
        /// Página para seleccionar período académico
        /// </summary>
        public async Task<IActionResult> Seleccionar()
        {
            try
            {
                var viewModel = new SeleccionarPeriodoViewModel
                {
                    PeriodosDisponibles = await _periodoService.GetPeriodosDisponiblesAsync(),
                    PeriodoActivoId = PeriodoActivo?.Id,
                    MostrarMensajeBienvenida = !_periodoService.TienePeriodoSeleccionado(User.Identity?.Name ?? "")
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar página de selección de período");
                TempData["Error"] = "Error al cargar los períodos académicos disponibles.";
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Establece el período académico seleccionado
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Establecer(int periodoId, string? returnUrl = null)
        {
            try
            {
                var userId = User.Identity?.Name ?? "";
                await _periodoService.SetPeriodoActivoAsync(userId, periodoId);

                TempData["Success"] = "Período académico establecido correctamente.";

                // Redirigir a la URL de retorno o al dashboard
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }

                return RedirectToAction("Index", "Home");
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Período inválido seleccionado: {PeriodoId}", periodoId);
                TempData["Error"] = "El período académico seleccionado no es válido.";
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Usuario sin permisos para período: {PeriodoId}", periodoId);
                TempData["Error"] = "No tiene permisos para acceder a este período académico.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al establecer período académico: {PeriodoId}", periodoId);
                TempData["Error"] = "Error al establecer el período académico.";
            }

            return RedirectToAction("Seleccionar");
        }

        /// <summary>
        /// Cambia el período académico vía AJAX
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CambiarPeriodo([FromBody] CambiarPeriodoRequest request)
        {
            try
            {
                if (request?.PeriodoId == null)
                {
                    return Json(new { success = false, message = "ID de período requerido" });
                }

                var userId = User.Identity?.Name ?? "";
                await _periodoService.SetPeriodoActivoAsync(userId, request.PeriodoId);

                return Json(new { 
                    success = true, 
                    message = "Período académico cambiado correctamente",
                    periodoId = request.PeriodoId
                });
            }
            catch (ArgumentException)
            {
                return Json(new { success = false, message = "Período académico no válido" });
            }
            catch (UnauthorizedAccessException)
            {
                return Json(new { success = false, message = "Sin permisos para este período" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cambiar período vía AJAX: {PeriodoId}", request?.PeriodoId);
                return Json(new { success = false, message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene información del período activo vía AJAX
        /// </summary>
        [HttpGet]
        public IActionResult GetPeriodoActivo()
        {
            try
            {
                if (PeriodoActivo == null)
                {
                    return Json(new { success = false, message = "No hay período activo" });
                }

                return Json(new { 
                    success = true, 
                    periodo = new {
                        id = PeriodoActivo.Id,
                        nombre = PeriodoActivo.NombreCompleto,
                        fechaInicio = PeriodoActivo.FechaInicio.ToString("yyyy-MM-dd"),
                        fechaFin = PeriodoActivo.FechaFin.ToString("yyyy-MM-dd")
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener período activo vía AJAX");
                return Json(new { success = false, message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Limpia el período de la sesión
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Limpiar()
        {
            try
            {
                _periodoService.LimpiarPeriodoActivoDeSesion();
                TempData["Info"] = "Período académico limpiado de la sesión.";
                
                return RedirectToAction("Seleccionar");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al limpiar período de sesión");
                TempData["Error"] = "Error al limpiar el período de la sesión.";
                return RedirectToAction("Seleccionar");
            }
        }
    }

    /// <summary>
    /// Request para cambio de período vía AJAX
    /// </summary>
    public class CambiarPeriodoRequest
    {
        public int PeriodoId { get; set; }
    }
}