using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RubricasApp.Web.Data;
using RubricasApp.Web.Models;
using RubricasApp.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using RubricasApp.Web.Authorization;
using RubricasApp.Web.Models.Permissions;

namespace RubricasApp.Web.Controllers
{
    [Authorize]
    public class InstrumentoMateriasController : Controller
    {
        private readonly RubricasDbContext _context;

        public InstrumentoMateriasController(RubricasDbContext context)
        {
            _context = context;
        }

        // GET: InstrumentoMaterias
        [RequirePermission(ApplicationPermissions.InstrumentoMaterias.VER)]
        public async Task<IActionResult> Index(int? instrumentoId, int? materiaId, int? periodoAcademicoId)
        {
            var query = _context.InstrumentoMaterias
                .Include(im => im.InstrumentoEvaluacion)
                .Include(im => im.Materia)
                .Include(im => im.PeriodoAcademico)
                .AsQueryable();

            // 🔧 CORRECCIÓN: Usar InstrumentoEvaluacionId en lugar de InstrumentoId en las consultas
            if (instrumentoId.HasValue)
            {
                query = query.Where(im => im.InstrumentoEvaluacionId == instrumentoId.Value);
            }

            if (materiaId.HasValue)
            {
                query = query.Where(im => im.MateriaId == materiaId.Value);
            }

            if (periodoAcademicoId.HasValue)
            {
                query = query.Where(im => im.PeriodoAcademicoId == periodoAcademicoId.Value);
            }

            var instrumentoMaterias = await query
                .OrderBy(im => im.InstrumentoEvaluacion.Nombre)
                .ThenBy(im => im.Materia.Nombre)
                .ToListAsync();

            // Preparar datos para los filtros
            ViewBag.InstrumentoId = new SelectList(
                await _context.InstrumentosEvaluacion
                    .Where(i => i.Activo)
                    .OrderBy(i => i.Nombre)
                    .ToListAsync(),
                nameof(InstrumentoEvaluacion.InstrumentoId),
                nameof(InstrumentoEvaluacion.Nombre),
                instrumentoId
            );

            ViewBag.MateriaId = new SelectList(
                await _context.Materias
                    .Where(m => m.Activa)
                    .OrderBy(m => m.Nombre)
                    .ToListAsync(),
                nameof(Materia.MateriaId),
                nameof(Materia.Nombre),
                materiaId
            );

            ViewBag.PeriodoAcademicoId = new SelectList(
                await _context.PeriodosAcademicos
                    .Where(p => p.Activo)
                    .OrderByDescending(p => p.Anio)
                    .ThenBy(p => p.Ciclo)
                    .ToListAsync(),
                nameof(PeriodoAcademico.Id),
                nameof(PeriodoAcademico.NombreCompleto),
                periodoAcademicoId
            );

            return View(instrumentoMaterias);
        }

        // GET: InstrumentoMaterias/Details/5/3
        [RequirePermission(ApplicationPermissions.InstrumentoMaterias.VER_DETALLES)]
        public async Task<IActionResult> Details(int instrumentoId, int materiaId)
        {
            // 🔧 CORRECCIÓN: Usar InstrumentoEvaluacionId en lugar de InstrumentoId
            var instrumentoMateria = await _context.InstrumentoMaterias
                .Include(im => im.InstrumentoEvaluacion)
                .Include(im => im.Materia)
                .Include(im => im.PeriodoAcademico)
                .FirstOrDefaultAsync(im => im.InstrumentoEvaluacionId == instrumentoId && im.MateriaId == materiaId);

            if (instrumentoMateria == null)
            {
                return NotFound();
            }

            var viewModel = new InstrumentoMateriaViewModel
            {
                InstrumentoId = instrumentoMateria.InstrumentoId,
                MateriaId = instrumentoMateria.MateriaId,
                PeriodoAcademicoId = instrumentoMateria.PeriodoAcademicoId,
                InstrumentoNombre = instrumentoMateria.InstrumentoEvaluacion.Nombre,
                MateriaNombre = instrumentoMateria.Materia.Nombre,
                MateriaDescripcion = instrumentoMateria.Materia.Descripcion,
                InstrumentoActivo = instrumentoMateria.InstrumentoEvaluacion.Activo,
                PeriodoAcademicoNombre = instrumentoMateria.PeriodoAcademico?.NombreCompleto
            };

            return View(viewModel);
        }

        // GET: InstrumentoMaterias/Create
        [RequirePermission(ApplicationPermissions.InstrumentoMaterias.CREAR)]
        public async Task<IActionResult> Create()
        {
            await CargarListasDesplegables();
            return View(new InstrumentoMateriaViewModel());
        }

        // POST: InstrumentoMaterias/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequirePermission(ApplicationPermissions.InstrumentoMaterias.CREAR)]
        public async Task<IActionResult> Create(InstrumentoMateriaViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                // 🔧 CORRECCIÓN: Usar InstrumentoEvaluacionId en lugar de InstrumentoId
                var existeRelacion = await _context.InstrumentoMaterias
                    .AnyAsync(im => im.InstrumentoEvaluacionId == viewModel.InstrumentoId && 
                                   im.MateriaId == viewModel.MateriaId);

                if (existeRelacion)
                {
                    ModelState.AddModelError("", "Ya existe una relación entre este instrumento y esta materia.");
                    await CargarListasDesplegables(viewModel.InstrumentoId, viewModel.MateriaId, viewModel.PeriodoAcademicoId);
                    return View(viewModel);
                }

                var instrumentoMateria = new InstrumentoMateria
                {
                    InstrumentoEvaluacionId = viewModel.InstrumentoId, // 🔧 Usar la propiedad mapeada
                    MateriaId = viewModel.MateriaId,
                    PeriodoAcademicoId = viewModel.PeriodoAcademicoId
                };

                _context.Add(instrumentoMateria);
                await _context.SaveChangesAsync();
                
                TempData["SuccessMessage"] = "Relación instrumento-materia creada exitosamente.";
                return RedirectToAction(nameof(Index));
            }

            await CargarListasDesplegables(viewModel.InstrumentoId, viewModel.MateriaId, viewModel.PeriodoAcademicoId);
            return View(viewModel);
        }

        // GET: InstrumentoMaterias/Edit/5/3
        [RequirePermission(ApplicationPermissions.InstrumentoMaterias.EDITAR)]
        public async Task<IActionResult> Edit(int instrumentoId, int materiaId)
        {
            // 🔧 CORRECCIÓN: Usar InstrumentoEvaluacionId en lugar de InstrumentoId
            var instrumentoMateria = await _context.InstrumentoMaterias
                .Include(im => im.InstrumentoEvaluacion)
                .Include(im => im.Materia)
                .Include(im => im.PeriodoAcademico)
                .FirstOrDefaultAsync(im => im.InstrumentoEvaluacionId == instrumentoId && im.MateriaId == materiaId);

            if (instrumentoMateria == null)
            {
                return NotFound();
            }

            var viewModel = new InstrumentoMateriaViewModel
            {
                InstrumentoId = instrumentoMateria.InstrumentoId,
                MateriaId = instrumentoMateria.MateriaId,
                PeriodoAcademicoId = instrumentoMateria.PeriodoAcademicoId,
                InstrumentoNombre = instrumentoMateria.InstrumentoEvaluacion.Nombre,
                MateriaNombre = instrumentoMateria.Materia.Nombre,
                PeriodoAcademicoNombre = instrumentoMateria.PeriodoAcademico?.NombreCompleto
            };

            await CargarListasDesplegables(instrumentoId, materiaId, instrumentoMateria.PeriodoAcademicoId);
            return View(viewModel);
        }

        // POST: InstrumentoMaterias/Edit/5/3
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequirePermission(ApplicationPermissions.InstrumentoMaterias.EDITAR)]
        public async Task<IActionResult> Edit(int instrumentoId, int materiaId, InstrumentoMateriaViewModel viewModel)
        {
            if (instrumentoId != viewModel.InstrumentoId || materiaId != viewModel.MateriaId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // 🔧 CORRECCIÓN: Usar InstrumentoEvaluacionId en lugar de InstrumentoId
                    var instrumentoMateria = await _context.InstrumentoMaterias
                        .FirstOrDefaultAsync(im => im.InstrumentoEvaluacionId == instrumentoId && im.MateriaId == materiaId);

                    if (instrumentoMateria == null)
                    {
                        return NotFound();
                    }

                    instrumentoMateria.PeriodoAcademicoId = viewModel.PeriodoAcademicoId;

                    _context.Update(instrumentoMateria);
                    await _context.SaveChangesAsync();
                    
                    TempData["SuccessMessage"] = "Relación instrumento-materia actualizada exitosamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InstrumentoMateriaExists(viewModel.InstrumentoId, viewModel.MateriaId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            await CargarListasDesplegables(viewModel.InstrumentoId, viewModel.MateriaId, viewModel.PeriodoAcademicoId);
            return View(viewModel);
        }

        // GET: InstrumentoMaterias/Delete/5/3
        [RequirePermission(ApplicationPermissions.InstrumentoMaterias.ELIMINAR)]
        public async Task<IActionResult> Delete(int instrumentoId, int materiaId)
        {
            // 🔧 CORRECCIÓN: Usar InstrumentoEvaluacionId en lugar de InstrumentoId
            var instrumentoMateria = await _context.InstrumentoMaterias
                .Include(im => im.InstrumentoEvaluacion)
                .Include(im => im.Materia)
                .Include(im => im.PeriodoAcademico)
                .FirstOrDefaultAsync(im => im.InstrumentoEvaluacionId == instrumentoId && im.MateriaId == materiaId);

            if (instrumentoMateria == null)
            {
                return NotFound();
            }

            var viewModel = new InstrumentoMateriaViewModel
            {
                InstrumentoId = instrumentoMateria.InstrumentoId,
                MateriaId = instrumentoMateria.MateriaId,
                PeriodoAcademicoId = instrumentoMateria.PeriodoAcademicoId,
                InstrumentoNombre = instrumentoMateria.InstrumentoEvaluacion.Nombre,
                MateriaNombre = instrumentoMateria.Materia.Nombre,
                MateriaDescripcion = instrumentoMateria.Materia.Descripcion,
                InstrumentoActivo = instrumentoMateria.InstrumentoEvaluacion.Activo,
                PeriodoAcademicoNombre = instrumentoMateria.PeriodoAcademico?.NombreCompleto
            };

            return View(viewModel);
        }

        // POST: InstrumentoMaterias/Delete/5/3
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [RequirePermission(ApplicationPermissions.InstrumentoMaterias.ELIMINAR)]
        public async Task<IActionResult> DeleteConfirmed(int instrumentoId, int materiaId)
        {
            // 🔧 CORRECCIÓN: Usar InstrumentoEvaluacionId en lugar de InstrumentoId
            var instrumentoMateria = await _context.InstrumentoMaterias
                .FirstOrDefaultAsync(im => im.InstrumentoEvaluacionId == instrumentoId && im.MateriaId == materiaId);

            if (instrumentoMateria != null)
            {
                _context.InstrumentoMaterias.Remove(instrumentoMateria);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Relación instrumento-materia eliminada exitosamente.";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool InstrumentoMateriaExists(int instrumentoId, int materiaId)
        {
            // 🔧 CORRECCIÓN: Usar InstrumentoEvaluacionId en lugar de InstrumentoId
            return _context.InstrumentoMaterias.Any(im => im.InstrumentoEvaluacionId == instrumentoId && im.MateriaId == materiaId);
        }

        private async Task CargarListasDesplegables(int? instrumentoSeleccionado = null, int? materiaSeleccionada = null, int? periodoSeleccionado = null)
        {
            ViewBag.InstrumentoId = new SelectList(
                await _context.InstrumentosEvaluacion
                    .Where(i => i.Activo)
                    .OrderBy(i => i.Nombre)
                    .ToListAsync(),
                nameof(InstrumentoEvaluacion.InstrumentoId),
                nameof(InstrumentoEvaluacion.Nombre),
                instrumentoSeleccionado
            );

            ViewBag.MateriaId = new SelectList(
                await _context.Materias
                    .Where(m => m.Activa)
                    .OrderBy(m => m.Nombre)
                    .ToListAsync(),
                nameof(Materia.MateriaId),
                nameof(Materia.Nombre),
                materiaSeleccionada
            );

            ViewBag.PeriodoAcademicoId = new SelectList(
                await _context.PeriodosAcademicos
                    .Where(p => p.Activo)
                    .OrderByDescending(p => p.Anio)
                    .ThenBy(p => p.Ciclo)
                    .ToListAsync(),
                nameof(PeriodoAcademico.Id),
                nameof(PeriodoAcademico.NombreCompleto),
                periodoSeleccionado
            );
        }
    }
}