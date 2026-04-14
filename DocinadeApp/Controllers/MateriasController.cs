using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DocinadeApp.Data;
using DocinadeApp.Models;
using DocinadeApp.Models.Academic;
using DocinadeApp.ViewModels.Academic;
using DocinadeApp.Services.Academic;
using DocinadeApp.ViewModels;
using DocinadeApp.Authorization;
using DocinadeApp.Models.Permissions;
using Microsoft.AspNetCore.Authorization;

namespace DocinadeApp.Controllers
{
    [Authorize]
    public class MateriasController : Controller
    {
        private readonly IMateriasService _materiasService;
        private readonly RubricasDbContext _context;
        private readonly ILogger<MateriasController> _logger;

        public MateriasController(
            IMateriasService materiasService,
            RubricasDbContext context,
            ILogger<MateriasController> logger)
        {
            _materiasService = materiasService;
            _context = context;
            _logger = logger;
        }

        // GET: Materias
        [RequirePermission(ApplicationPermissions.Materias.VER)]
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10, string? searchTerm = null, string? tipoFiltro = null)
        {
            try
            {
                ViewBag.CurrentFilter = searchTerm;
                ViewBag.TipoFiltro = tipoFiltro;
                ViewBag.PageSize = pageSize;
                
                // Obtener tipos únicos para el filtro
                ViewBag.TiposDisponibles = await _materiasService.GetTiposUnicosAsync();
                
                var result = await _materiasService.GetPagedAsync(page, pageSize, searchTerm, tipoFiltro);
                return View(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener las materias");
                TempData["ErrorMessage"] = "Error al cargar las materias";
                return View(new PagedResult<MateriaListVm> { Items = new List<MateriaListVm>() });
            }
        }

        // GET: Materias/Details/5
        [RequirePermission(ApplicationPermissions.Materias.VER_DETALLES)]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var materia = await _materiasService.GetByIdAsync(id);
                if (materia == null)
                {
                    return NotFound();
                }
                return View(materia);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los detalles de la materia {Id}", id);
                return NotFound();
            }
        }

        // GET: Materias/Create
        [RequirePermission(ApplicationPermissions.Materias.CREAR)]
        public IActionResult Create()
        {
            return View(new CrearMateriaVm());
        }

        // POST: Materias/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequirePermission(ApplicationPermissions.Materias.CREAR)]
        public async Task<IActionResult> Create(CrearMateriaVm model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var result = await _materiasService.CreateAsync(model);
                if (result.Success)
                {
                    TempData["SuccessMessage"] = result.Message;
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError("", result.Message);
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear la materia");
                ModelState.AddModelError("", "Error interno del servidor");
                return View(model);
            }
        }

        // GET: Materias/Edit/5
        [RequirePermission(ApplicationPermissions.Materias.EDITAR)]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var materia = await _materiasService.GetByIdAsync(id);
                if (materia == null)
                {
                    return NotFound();
                }

                var model = new EditarMateriaVm
                {
                    Codigo = materia.Codigo,
                    Nombre = materia.Nombre,
                    Tipo = materia.Tipo,
                    Descripcion = materia.Descripcion,
                    Creditos = materia.Creditos,
                    Activa = materia.Activa
                };

                ViewBag.MateriaId = id; // Agregar el ID para los enlaces
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la materia para editar {Id}", id);
                return NotFound();
            }
        }

        // POST: Materias/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequirePermission(ApplicationPermissions.Materias.EDITAR)]
        public async Task<IActionResult> Edit(int id, EditarMateriaVm model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.MateriaId = id; // Mantener el ID en caso de error
                return View(model);
            }

            try
            {
                var result = await _materiasService.UpdateAsync(id, model);
                if (result.Success)
                {
                    TempData["SuccessMessage"] = result.Message;
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError("", result.Message);
                ViewBag.MateriaId = id; // Mantener el ID en caso de error
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar la materia {Id}", id);
                ModelState.AddModelError("", "Error interno del servidor");
                ViewBag.MateriaId = id; // Mantener el ID en caso de error
                return View(model);
            }
        }

        // GET: Materias/Delete/5
        [RequirePermission(ApplicationPermissions.Materias.ELIMINAR)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var materia = await _materiasService.GetByIdAsync(id);
                if (materia == null)
                {
                    return NotFound();
                }
                return View(materia);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la materia para eliminar {Id}", id);
                return NotFound();
            }
        }

        // POST: Materias/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [RequirePermission(ApplicationPermissions.Materias.ELIMINAR)]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var result = await _materiasService.DeleteAsync(id);
                if (result.Success)
                {
                    TempData["SuccessMessage"] = result.Message;
                }
                else
                {
                    TempData["ErrorMessage"] = result.Message;
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar la materia {Id}", id);
                TempData["ErrorMessage"] = "Error interno del servidor";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Materias/AsignarInstrumentos/5
        [RequirePermission(ApplicationPermissions.Materias.ASIGNAR_INSTRUMENTOS)]
        public async Task<IActionResult> AsignarInstrumentos(int id)
        {
            try
            {
                var materia = await _materiasService.GetByIdAsync(id);
                if (materia == null)
                {
                    return NotFound();
                }

                // Obtener instrumentos ya asignados a esta materia
                var instrumentosAsignados = await _context.InstrumentoMaterias
                    .Where(im => im.MateriaId == id)
                    .Include(im => im.InstrumentoEvaluacion)
                    .Include(im => im.PeriodoAcademico)
                    .ToListAsync();

                // Obtener instrumentos disponibles (no asignados a esta materia)
                var instrumentosAsignadosIds = instrumentosAsignados.Select(im => im.InstrumentoEvaluacionId).ToList();
                var instrumentosDisponibles = await _context.InstrumentosEvaluacion
                    .Where(i => i.Activo && !instrumentosAsignadosIds.Contains(i.InstrumentoId))
                    .OrderBy(i => i.Nombre)
                    .ToListAsync();

                var model = new AsignarInstrumentosVm
                {
                    MateriaId = id,
                    MateriaNombre = $"{materia.Codigo} - {materia.Nombre}",
                    InstrumentosDisponibles = instrumentosDisponibles.Select(i => new SelectListItem
                    {
                        Value = i.InstrumentoId.ToString(),
                        Text = i.Nombre
                    }).ToList(),
                    InstrumentosAsignados = instrumentosAsignados.Select(im => new InstrumentoAsignadoVm
                    {
                        InstrumentoId = im.InstrumentoEvaluacionId,
                        InstrumentoNombre = im.InstrumentoEvaluacion.Nombre,
                        PeriodoAcademicoId = im.PeriodoAcademicoId,
                        PeriodoAcademicoNombre = im.PeriodoAcademico?.NombreCompleto ?? "Sin período",
                        FechaAsignacion = im.FechaAsignacion,
                        EsObligatorio = im.EsObligatorio,
                        OrdenPresentacion = im.OrdenPresentacion
                    }).ToList()
                };

                // Cargar períodos académicos para la asignación
                ViewBag.PeriodosAcademicos = new SelectList(
                    await _context.PeriodosAcademicos
                        .Where(p => p.Activo)
                        .OrderByDescending(p => p.Anio)
                        .ThenBy(p => p.Ciclo)
                        .ToListAsync(),
                    nameof(PeriodoAcademico.Id),
                    nameof(PeriodoAcademico.NombreCompleto)
                );

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar asignación de instrumentos para materia {Id}", id);
                return NotFound();
            }
        }

        // POST: Materias/AsignarInstrumento
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AsignarInstrumento(int materiaId, int instrumentoId, int? periodoAcademicoId, bool esObligatorio = false)
        {
            try
            {
                // Verificar que no exista ya la relación
                var existeRelacion = await _context.InstrumentoMaterias
                    .AnyAsync(im => im.MateriaId == materiaId && im.InstrumentoEvaluacionId == instrumentoId);

                if (existeRelacion)
                {
                    TempData["ErrorMessage"] = "Este instrumento ya está asignado a la materia.";
                    return RedirectToAction(nameof(AsignarInstrumentos), new { id = materiaId });
                }

                var instrumentoMateria = new InstrumentoMateria
                {
                    MateriaId = materiaId,
                    InstrumentoEvaluacionId = instrumentoId,
                    PeriodoAcademicoId = periodoAcademicoId ?? 0,
                    FechaAsignacion = DateTime.Now,
                    EsObligatorio = esObligatorio
                };

                _context.InstrumentoMaterias.Add(instrumentoMateria);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Instrumento asignado exitosamente a la materia.";
                return RedirectToAction(nameof(AsignarInstrumentos), new { id = materiaId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al asignar instrumento {InstrumentoId} a materia {MateriaId}", instrumentoId, materiaId);
                TempData["ErrorMessage"] = "Error interno del servidor";
                return RedirectToAction(nameof(AsignarInstrumentos), new { id = materiaId });
            }
        }

        // POST: Materias/QuitarInstrumento
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> QuitarInstrumento(int materiaId, int instrumentoId)
        {
            try
            {
                var instrumentoMateria = await _context.InstrumentoMaterias
                    .FirstOrDefaultAsync(im => im.MateriaId == materiaId && im.InstrumentoEvaluacionId == instrumentoId);

                if (instrumentoMateria != null)
                {
                    _context.InstrumentoMaterias.Remove(instrumentoMateria);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Instrumento removido de la materia exitosamente.";
                }
                else
                {
                    TempData["ErrorMessage"] = "No se encontró la relación instrumento-materia.";
                }

                return RedirectToAction(nameof(AsignarInstrumentos), new { id = materiaId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al quitar instrumento {InstrumentoId} de materia {MateriaId}", instrumentoId, materiaId);
                TempData["ErrorMessage"] = "Error interno del servidor";
                return RedirectToAction(nameof(AsignarInstrumentos), new { id = materiaId });
            }
        }

        // GET: Materias/Instrumentos/5
        public async Task<IActionResult> Instrumentos(int id)
        {
            try
            {
                var materia = await _materiasService.GetByIdAsync(id);
                if (materia == null)
                {
                    return NotFound();
                }

                var instrumentos = await _context.InstrumentoMaterias
                    .Where(im => im.MateriaId == id)
                    .Include(im => im.InstrumentoEvaluacion)
                    .Include(im => im.PeriodoAcademico)
                    .OrderBy(im => im.OrdenPresentacion ?? int.MaxValue)
                    .ThenBy(im => im.InstrumentoEvaluacion.Nombre)
                    .Select(im => new InstrumentoAsignadoVm
                    {
                        InstrumentoId = im.InstrumentoEvaluacionId,
                        InstrumentoNombre = im.InstrumentoEvaluacion.Nombre,
                        PeriodoAcademicoId = im.PeriodoAcademicoId,
                        PeriodoAcademicoNombre = im.PeriodoAcademico!.NombreCompleto,
                        FechaAsignacion = im.FechaAsignacion,
                        EsObligatorio = im.EsObligatorio,
                        OrdenPresentacion = im.OrdenPresentacion
                    })
                    .ToListAsync();
                
                ViewBag.Materia = materia;
                return View(instrumentos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener instrumentos de materia {Id}", id);
                return NotFound();
            }
        }
    }
}