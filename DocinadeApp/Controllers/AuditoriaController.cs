using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using DocinadeApp.Controllers;
using DocinadeApp.Services.Auditoria;
using DocinadeApp.Services;
using DocinadeApp.ViewModels.Admin;
using DocinadeApp.ViewModels.Auditoria;
using DocinadeApp.Data;
using Microsoft.AspNetCore.Identity;
using DocinadeApp.Models.Identity;
using DocinadeApp.Models.Permissions;
using DocinadeApp.Authorization;

namespace DocinadeApp.Controllers
{
    [Authorize]
    public class AuditoriaController : BaseController
    {
        private readonly IAuditoriaService _auditoriaService;

        public AuditoriaController(
            IPeriodoAcademicoService periodoService,
            RubricasDbContext context,
            IAuditoriaService auditoriaService) : base(periodoService, context)
        {
            _auditoriaService = auditoriaService;
        }

        [RequirePermission(ApplicationPermissions.Auditoria.VER)]
        public async Task<IActionResult> Index(int? tipoOperacion, string? usuario, DateTime? fechaDesde, DateTime? fechaHasta, int pageNumber = 1, int pageSize = 20)
        {
            try
            {
                var historial = await _auditoriaService.ObtenerHistorialAsync(tipoOperacion, usuario, fechaDesde, fechaHasta, pageNumber, pageSize);
                var viewModel = new HistorialAuditoriaViewModel
                {
                    TablaAfectada = "Todas",
                    RegistroId = 0,
                    TituloRegistro = "Historial General de Auditoría",
                    Operaciones = historial.Select(MapToAuditoriaOperacionViewModel).ToList(),
                    TotalOperaciones = historial.Count(),
                    FechaDesde = fechaDesde,
                    FechaHasta = fechaHasta
                };

                // Establecer filtros en ViewBag
                ViewBag.Filtros = new FiltrosAuditoriaViewModel
                {
                    TipoOperacion = tipoOperacion?.ToString(),
                    UsuarioId = usuario,
                    FechaDesde = fechaDesde,
                    FechaHasta = fechaHasta
                };

                return View(viewModel);
            }
            catch
            {
                ModelState.AddModelError(string.Empty, "Error al cargar el historial de auditoría");
                return View(new HistorialAuditoriaViewModel());
            }
        }

        public async Task<IActionResult> Detalles(int id)
        {
            try
            {
                var operacion = await _auditoriaService.ObtenerOperacionAsync(id);
                if (operacion == null)
                    return NotFound();

                var viewModel = MapToViewModel(operacion);
                return View(viewModel);
            }
            catch
            {
                return NotFound();
            }
        }

        public async Task<IActionResult> HistorialGrupo(int grupoId)
        {
            try
            {
                var historial = await _auditoriaService.ObtenerHistorialTablaAsync("GruposEstudiantes", grupoId.ToString());
                var viewModel = new HistorialAuditoriaViewModel
                {
                    TablaAfectada = "GruposEstudiantes",
                    RegistroId = grupoId,
                    TituloRegistro = $"Historial del Grupo ID: {grupoId}",
                    Operaciones = historial.Select(MapToAuditoriaOperacionViewModel).ToList(),
                    TotalOperaciones = historial.Count()
                };

                return View("Index", viewModel);
            }
            catch
            {
                ModelState.AddModelError(string.Empty, "Error al cargar el historial del grupo");
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerDatosJson(int id, string tipo)
        {
            try
            {
                var operacion = await _auditoriaService.ObtenerOperacionAsync(id);
                if (operacion == null)
                    return NotFound();

                var datos = tipo == "anteriores" ? operacion.DatosAnteriores : operacion.DatosNuevos;
                return Content(datos ?? "{}", "application/json");
            }
            catch
            {
                return BadRequest("Error al obtener los datos");
            }
        }

        private AuditoriaViewModel MapToViewModel(DocinadeApp.Models.AuditoriaOperacion operacion)
        {
            return new AuditoriaViewModel
            {
                Id = operacion.Id,
                TipoOperacion = operacion.TipoOperacion,
                TablaAfectada = operacion.TablaAfectada,
                RegistroId = operacion.RegistroId,
                Motivo = operacion.Motivo,
                UsuarioId = operacion.UsuarioId,
                UsuarioNombre = ((ApplicationUser?)operacion.Usuario)?.Nombre,
                FechaOperacion = operacion.FechaOperacion,
                DireccionIP = operacion.DireccionIP,
                UserAgent = operacion.UserAgent,
                DatosAnteriores = operacion.DatosAnteriores,
                DatosNuevos = operacion.DatosNuevos,
                Exitosa = operacion.OperacionExitosa,
                MensajeError = operacion.MensajeError
            };
        }

        private AuditoriaOperacionViewModel MapToAuditoriaOperacionViewModel(DocinadeApp.Models.AuditoriaOperacion operacion)
        {
            var usuario = (ApplicationUser?)operacion.Usuario;
            return new AuditoriaOperacionViewModel
            {
                Id = operacion.Id,
                TipoOperacion = operacion.TipoOperacion,
                TipoOperacionDisplay = GetTipoOperacionDisplay(operacion.TipoOperacion),
                Descripcion = operacion.Descripcion,
                Motivo = operacion.Motivo,
                FechaOperacion = operacion.FechaOperacion,
                UsuarioNombre = usuario?.Nombre ?? "Usuario desconocido",
                UsuarioEmail = usuario?.Email ?? string.Empty,
                OperacionExitosa = operacion.OperacionExitosa,
                MensajeError = operacion.MensajeError,
                DireccionIP = operacion.DireccionIP,
                UserAgent = operacion.UserAgent,
                TieneDatosAnteriores = !string.IsNullOrEmpty(operacion.DatosAnteriores),
                TieneDatosNuevos = !string.IsNullOrEmpty(operacion.DatosNuevos),
                DatosAnteriores = operacion.DatosAnteriores,
                DatosNuevos = operacion.DatosNuevos,
                IconoOperacion = GetIconoOperacion(operacion.TipoOperacion),
                ColorOperacion = GetColorOperacion(operacion.TipoOperacion)
            };
        }

        private string GetTipoOperacionDisplay(string tipoOperacion)
        {
            return tipoOperacion switch
            {
                "CREATE" => "Creación",
                "UPDATE" => "Actualización",
                "DELETE" => "Eliminación",
                "LOGIN" => "Inicio de Sesión",
                "LOGOUT" => "Cierre de Sesión",
                "ASSIGN" => "Asignación",
                "UNASSIGN" => "Desasignación",
                "TRANSFER" => "Transferencia",
                "EXPORT" => "Exportación",
                "IMPORT" => "Importación",
                _ => tipoOperacion
            };
        }

        private string GetIconoOperacion(string tipoOperacion)
        {
            return tipoOperacion switch
            {
                "CREATE" => "fas fa-plus-circle",
                "UPDATE" => "fas fa-edit",
                "DELETE" => "fas fa-trash-alt",
                "LOGIN" => "fas fa-sign-in-alt",
                "LOGOUT" => "fas fa-sign-out-alt",
                "ASSIGN" => "fas fa-link",
                "UNASSIGN" => "fas fa-unlink",
                "TRANSFER" => "fas fa-exchange-alt",
                "EXPORT" => "fas fa-download",
                "IMPORT" => "fas fa-upload",
                _ => "fas fa-question-circle"
            };
        }

        private string GetColorOperacion(string tipoOperacion)
        {
            return tipoOperacion switch
            {
                "CREATE" => "success",
                "UPDATE" => "warning",
                "DELETE" => "danger",
                "LOGIN" => "info",
                "LOGOUT" => "secondary",
                "ASSIGN" => "primary",
                "UNASSIGN" => "dark",
                "TRANSFER" => "info",
                "EXPORT" => "success",
                "IMPORT" => "primary",
                _ => "secondary"
            };
        }
    }
}
