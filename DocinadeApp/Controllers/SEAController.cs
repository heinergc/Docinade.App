using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RubricasApp.Web.Authorization;
using RubricasApp.Web.Data;
using RubricasApp.Web.Models.Permissions;
using RubricasApp.Web.Models.SEA;
using RubricasApp.Web.Services;
using RubricasApp.Web.Services.SEA;

namespace RubricasApp.Web.Controllers
{
    /// <summary>
    /// Controlador para reportes SEA (Sistema de Evaluación MEP)
    /// Integra Cuaderno Calificador con Asistencia según estándares MEP
    /// </summary>
    [Authorize]
    public class SEAController : BaseController
    {
        private readonly ISEAService _seaService;
        private readonly ILogger<SEAController> _logger;

        public SEAController(
            RubricasDbContext context,
            IPeriodoAcademicoService periodoService,
            ISEAService seaService,
            ILogger<SEAController> logger)
            : base(periodoService, context)
        {
            _seaService = seaService;
            _logger = logger;
        }

        /// <summary>
        /// Vista principal del reporte SEA
        /// GET: /SEA/Index
        /// </summary>
        [RequirePermission(ApplicationPermissions.SEA.VER_REPORTE)]
        public async Task<IActionResult> Index(
            int? materiaId,
            int? periodoAcademicoId,
            OpcionRedondeoSEA redondeo = OpcionRedondeoSEA.SinDecimales,
            string? busqueda = null)
        {
            try
            {
                var periodoFiltrar = GetFiltroPeridoParaQuery(periodoAcademicoId);

                var reporte = await _seaService.GenerarReporteSEAAsync(materiaId, periodoFiltrar);
                reporte.OpcionRedondeo = redondeo;

                // Aplicar búsqueda si existe
                if (!string.IsNullOrWhiteSpace(busqueda))
                {
                    var busquedaLower = busqueda.ToLower();
                    reporte.Estudiantes = reporte.Estudiantes
                        .Where(e => e.NombreCompleto.ToLower().Contains(busquedaLower) ||
                                   e.NumeroIdentificacion.Contains(busquedaLower))
                        .ToList();
                    ViewBag.Busqueda = busqueda;
                }

                // Cargar listas para filtros
                ViewBag.Materias = await ObtenerMateriasSelectListAsync();
                ViewBag.Periodos = await GetPeriodosAcademicosSelectListAsync(periodoFiltrar);
                ViewBag.MateriaSeleccionada = materiaId;
                ViewBag.PeriodoSeleccionado = periodoFiltrar;
                ViewBag.OpcionRedondeo = redondeo;

                return View(reporte);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generando reporte SEA");
                TempData["Error"] = $"Error al generar el reporte: {ex.Message}";
                return View(new ViewModels.SEA.SEAReporteViewModel());
            }
        }

        /// <summary>
        /// Exporta el reporte SEA en formato CSV compatible con sistema MEP
        /// GET: /SEA/ExportarCSV
        /// </summary>
        [RequirePermission(ApplicationPermissions.SEA.EXPORTAR_CSV)]
        public async Task<IActionResult> ExportarCSV(
            int? materiaId,
            int? periodoAcademicoId,
            OpcionRedondeoSEA redondeo = OpcionRedondeoSEA.SinDecimales)
        {
            try
            {
                var periodoFiltrar = GetFiltroPeridoParaQuery(periodoAcademicoId);
                var csvBytes = await _seaService.ExportarCSV_SEAAsync(materiaId, periodoFiltrar, redondeo);

                var materia = materiaId.HasValue ? await _context.Materias.FindAsync(materiaId.Value) : null;
                var periodo = periodoFiltrar.HasValue ? await _context.PeriodosAcademicos.FindAsync(periodoFiltrar.Value) : null;

                var fileName = $"SEA_{materia?.Codigo ?? "TODAS"}_{periodo?.Codigo ?? "SIN-PERIODO"}_{DateTime.Now:yyyyMMdd_HHmm}.csv";

                _logger.LogInformation("Exportando CSV SEA: {FileName}", fileName);

                return File(csvBytes, "text/csv", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exportando CSV SEA");
                TempData["Error"] = $"Error al exportar CSV: {ex.Message}";
                return RedirectToAction(nameof(Index), new { materiaId, periodoAcademicoId });
            }
        }

        /// <summary>
        /// Exporta el reporte SEA en formato Excel
        /// GET: /SEA/ExportarExcel
        /// </summary>
        [RequirePermission(ApplicationPermissions.SEA.EXPORTAR_EXCEL)]
        public async Task<IActionResult> ExportarExcel(
            int? materiaId,
            int? periodoAcademicoId,
            OpcionRedondeoSEA redondeo = OpcionRedondeoSEA.SinDecimales)
        {
            try
            {
                var periodoFiltrar = GetFiltroPeridoParaQuery(periodoAcademicoId);
                var excelBytes = await _seaService.ExportarExcel_SEAAsync(materiaId, periodoFiltrar, redondeo);

                var materia = materiaId.HasValue ? await _context.Materias.FindAsync(materiaId.Value) : null;
                var periodo = periodoFiltrar.HasValue ? await _context.PeriodosAcademicos.FindAsync(periodoFiltrar.Value) : null;

                var fileName = $"SEA_{materia?.Codigo ?? "TODAS"}_{periodo?.Codigo ?? "SIN-PERIODO"}_{DateTime.Now:yyyyMMdd_HHmm}.xlsx";

                _logger.LogInformation("Exportando Excel SEA: {FileName}", fileName);

                return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exportando Excel SEA");
                TempData["Error"] = $"Error al exportar Excel: {ex.Message}";
                return RedirectToAction(nameof(Index), new { materiaId, periodoAcademicoId });
            }
        }

        /// <summary>
        /// Exporta el reporte SEA en formato PDF
        /// GET: /SEA/ExportarPDF
        /// </summary>
        [RequirePermission(ApplicationPermissions.SEA.EXPORTAR_PDF)]
        public async Task<IActionResult> ExportarPDF(
            int? materiaId,
            int? periodoAcademicoId,
            OpcionRedondeoSEA redondeo = OpcionRedondeoSEA.SinDecimales)
        {
            try
            {
                var periodoFiltrar = GetFiltroPeridoParaQuery(periodoAcademicoId);
                var pdfBytes = await _seaService.ExportarPDF_SEAAsync(materiaId, periodoFiltrar, redondeo);

                var materia = materiaId.HasValue ? await _context.Materias.FindAsync(materiaId.Value) : null;
                var periodo = periodoFiltrar.HasValue ? await _context.PeriodosAcademicos.FindAsync(periodoFiltrar.Value) : null;

                var fileName = $"SEA_{materia?.Codigo ?? "TODAS"}_{periodo?.Codigo ?? "SIN-PERIODO"}_{DateTime.Now:yyyyMMdd_HHmm}.pdf";

                _logger.LogInformation("Exportando PDF SEA: {FileName}", fileName);

                return File(pdfBytes, "application/pdf", fileName);
            }
            catch (NotImplementedException)
            {
                TempData["Warning"] = "La exportación a PDF está pendiente de implementación";
                return RedirectToAction(nameof(Index), new { materiaId, periodoAcademicoId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exportando PDF SEA");
                TempData["Error"] = $"Error al exportar PDF: {ex.Message}";
                return RedirectToAction(nameof(Index), new { materiaId, periodoAcademicoId });
            }
        }

        /// <summary>
        /// Configuración de componentes SEA para una materia
        /// GET: /SEA/Configurar
        /// </summary>
        [RequirePermission(ApplicationPermissions.SEA.CONFIGURAR)]
        public async Task<IActionResult> Configurar(int materiaId)
        {
            try
            {
                var viewModel = await _seaService.ObtenerConfiguracionAsync(materiaId);

                // Cargar componentes disponibles
                ViewBag.ComponentesSEA = ComponentesSEA.ObtenerTodos()
                    .Select(c => new SelectListItem
                    {
                        Value = c,
                        Text = ComponentesSEA.ObtenerNombreAmigable(c)
                    })
                    .ToList();

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cargando configuración SEA");
                TempData["Error"] = $"Error al cargar configuración: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Guarda la configuración de componentes SEA
        /// POST: /SEA/Configurar
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequirePermission(ApplicationPermissions.SEA.CONFIGURAR)]
        public async Task<IActionResult> Configurar(int materiaId, List<ViewModels.SEA.ConfiguracionComponenteItem> configuraciones)
        {
            try
            {
                // Validar que la suma sea 100%
                var total = configuraciones.Where(c => c.Activo).Sum(c => c.Porcentaje);
                if (Math.Abs(total - 100) > 0.01m)
                {
                    TempData["Error"] = $"Las ponderaciones deben sumar 100%. Suma actual: {total}%";
                    return RedirectToAction(nameof(Configurar), new { materiaId });
                }

                var exito = await _seaService.GuardarConfiguracionAsync(materiaId, configuraciones);

                if (exito)
                {
                    TempData["Success"] = "Configuración SEA guardada exitosamente";
                    _logger.LogInformation("Configuración SEA guardada para materia {MateriaId}", materiaId);
                }
                else
                {
                    TempData["Error"] = "Error al guardar la configuración";
                }

                return RedirectToAction(nameof(Index), new { materiaId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error guardando configuración SEA");
                TempData["Error"] = $"Error al guardar configuración: {ex.Message}";
                return RedirectToAction(nameof(Configurar), new { materiaId });
            }
        }

        /// <summary>
        /// Endpoint AJAX para validar ponderaciones
        /// GET: /SEA/ValidarPonderaciones
        /// </summary>
        [HttpGet]
        public async Task<JsonResult> ValidarPonderaciones(int materiaId)
        {
            try
            {
                var validacion = await _seaService.ValidarPonderacionesAsync(materiaId);

                return Json(new
                {
                    success = true,
                    esValida = validacion.esValida,
                    total = validacion.total,
                    mensaje = validacion.mensaje
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validando ponderaciones");
                return Json(new { success = false, mensaje = ex.Message });
            }
        }

        /// <summary>
        /// Endpoint AJAX para obtener estadísticas rápidas
        /// GET: /SEA/ObtenerEstadisticas
        /// </summary>
        [HttpGet]
        public async Task<JsonResult> ObtenerEstadisticas(int? materiaId, int? periodoId)
        {
            try
            {
                var periodoFiltrar = GetFiltroPeridoParaQuery(periodoId);
                var reporte = await _seaService.GenerarReporteSEAAsync(materiaId, periodoFiltrar);

                return Json(new
                {
                    success = true,
                    data = new
                    {
                        totalEstudiantes = reporte.Estadisticas.TotalEstudiantes,
                        promedioGeneral = reporte.Estadisticas.PromedioGeneral,
                        porcentajeAprobacion = reporte.Estadisticas.PorcentajeAprobacion,
                        asistenciaPromedio = reporte.Estadisticas.AsistenciaPromedio,
                        estudiantesAprobados = reporte.Estadisticas.EstudiantesAprobados,
                        estudiantesReprobados = reporte.Estadisticas.EstudiantesReprobados
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo estadísticas");
                return Json(new { success = false, mensaje = ex.Message });
            }
        }

        // Métodos auxiliares privados

        private async Task<List<SelectListItem>> ObtenerMateriasSelectListAsync()
        {
            var materias = await _context.Materias
                .Where(m => m.Activa)
                .OrderBy(m => m.Codigo)
                .Select(m => new SelectListItem
                {
                    Value = m.MateriaId.ToString(),
                    Text = $"[{m.Codigo}] {m.Nombre}"
                })
                .ToListAsync();

            materias.Insert(0, new SelectListItem { Value = "", Text = "-- Todas las materias --" });

            return materias;
        }
    }
}
