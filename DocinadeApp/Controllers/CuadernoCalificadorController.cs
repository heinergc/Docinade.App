using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RubricasApp.Web.Data;
using RubricasApp.Web.Models;
using RubricasApp.Web.Services;
using RubricasApp.Web.Services.CuadernoCalificador;
using RubricasApp.Web.ViewModels;
using RubricasApp.Web.Models.Permissions;
using RubricasApp.Web.Authorization;

namespace RubricasApp.Web.Controllers
{
    [Authorize]
    public class CuadernoCalificadorController : BaseController
    {
        private readonly ICuadernoCalificadorService _cuadernoService;
        private readonly ICuadernoCalificadorDinamicoService _cuadernoDinamicoService;

        public CuadernoCalificadorController(
            RubricasDbContext context, 
            IPeriodoAcademicoService periodoService,
            ICuadernoCalificadorService cuadernoService,
            ICuadernoCalificadorDinamicoService cuadernoDinamicoService) 
            : base(periodoService, context)
        {
            _cuadernoService = cuadernoService;
            _cuadernoDinamicoService = cuadernoDinamicoService;
        }

        // GET: CuadernoCalificador - Vista principal con cuaderno din�mico
        [RequirePermission(ApplicationPermissions.CuadernoCalificador.VER)]
        public async Task<IActionResult> Index(int? materiaId, int? periodoAcademicoId)
        {
            var periodoParaFiltrar = GetFiltroPeridoParaQuery(periodoAcademicoId);
            var cuadernoDinamico = await _cuadernoDinamicoService.GenerarCuadernoCalificadorAsync(materiaId, periodoParaFiltrar);

            ViewBag.MateriaSeleccionada = materiaId;
            ViewBag.PeriodoSeleccionado = periodoParaFiltrar;

            return View(cuadernoDinamico);
        }

        // GET: CuadernoCalificador/ExportarExcelDinamico
        [RequirePermission(ApplicationPermissions.CuadernoCalificador.EXPORTAR_DINAMICO)]
        public async Task<IActionResult> ExportarExcelDinamico(int? materiaId, int? periodoAcademicoId)
        {
            try
            {
                var periodoParaFiltrar = GetFiltroPeridoParaQuery(periodoAcademicoId);
                var excelBytes = await _cuadernoDinamicoService.ExportarCuadernoAExcelAsync(materiaId, periodoParaFiltrar);

                var materia = materiaId.HasValue ? await _context.Materias.FindAsync(materiaId.Value) : null;
                var periodo = periodoParaFiltrar.HasValue ? await _context.PeriodosAcademicos.FindAsync(periodoParaFiltrar.Value) : null;

                var fileName = $"CuadernoCalificador_{materia?.Codigo ?? "General"}_{periodo?.Codigo ?? "SinPeriodo"}_{DateTime.Now:yyyyMMdd_HHmm}.xlsx";
                
                return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al exportar Excel: {ex.Message}";
                return RedirectToAction(nameof(Index), new { materiaId, periodoAcademicoId });
            }
        }

        // GET: CuadernoCalificador/EstadisticasRapidas
        [HttpGet]
        public async Task<JsonResult> EstadisticasRapidas(int? materiaId, int? periodoAcademicoId)
        {
            try
            {
                var periodoParaFiltrar = GetFiltroPeridoParaQuery(periodoAcademicoId);
                var estadisticas = await _cuadernoDinamicoService.ObtenerEstadisticasRapidasAsync(materiaId, periodoParaFiltrar);
                
                return Json(new { success = true, data = estadisticas });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // GET: CuadernoCalificador/DiagnosticoPonderaciones
        [HttpGet]
        public async Task<JsonResult> DiagnosticoPonderaciones(int materiaId)
        {
            try
            {
                var diagnostico = await _cuadernoDinamicoService.DiagnosticarPonderacionesAsync(materiaId);
                return Json(new { success = true, data = diagnostico });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // GET: CuadernoCalificador/Tradicional
        [RequirePermission(ApplicationPermissions.CuadernoCalificador.VER_TRADICIONAL)]
        public async Task<IActionResult> Tradicional(int? materiaId, int? periodoAcademicoId)
        {
            var periodoParaFiltrar = GetFiltroPeridoParaQuery(periodoAcademicoId);
            var cuadernos = await _cuadernoService.ObtenerCuadernosPorFiltroAsync(materiaId, periodoParaFiltrar);

            ViewBag.Materias = await ObtenerMateriasSelectListAsync();
            ViewBag.Periodos = await GetPeriodosAcademicosSelectListAsync();
            ViewBag.MateriaSeleccionada = materiaId;
            ViewBag.PeriodoSeleccionado = periodoParaFiltrar;

            return View(cuadernos);
        }

        private async Task<List<SelectListItem>> ObtenerMateriasSelectListAsync()
        {
            var materias = await _context.Materias
                .Where(m => m.Activa)
                .OrderBy(m => m.Nombre)
                .Select(m => new SelectListItem
                {
                    Value = m.MateriaId.ToString(),
                    Text = $"{m.Codigo} - {m.Nombre}"
                })
                .ToListAsync();

            materias.Insert(0, new SelectListItem { Value = "", Text = "-- Todas las materias --" });
            return materias;
        }
    }

    public class ActualizarPonderacionesRequest
    {
        public int CuadernoId { get; set; }
        public List<InstrumentoConfiguracion> Instrumentos { get; set; } = new();
    }
}