using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RubricasApp.Web.Data;
using RubricasApp.Web.Models;
using RubricasApp.Web.ViewModels;

namespace RubricasApp.Web.Controllers
{
    [Authorize]
    public class ProfesorGuiaController : Controller
    {
        private readonly RubricasDbContext _context;
        private readonly ILogger<ProfesorGuiaController> _logger;

        public ProfesorGuiaController(RubricasDbContext context, ILogger<ProfesorGuiaController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: ProfesorGuia
        public async Task<IActionResult> Index(int? grupoId, int? periodoId)
        {
            var periodosQuery = _context.PeriodosAcademicos
                .Where(p => p.Activo)
                .OrderByDescending(p => p.FechaInicio);

            var periodos = await periodosQuery.ToListAsync();
            
            // Si no hay período seleccionado, usar el más reciente
            if (!periodoId.HasValue && periodos.Any())
            {
                periodoId = periodos.First().Id;
            }

            var query = _context.ProfesorGuia
                .Include(pg => pg.Profesor)
                .Include(pg => pg.Grupo)
                    .ThenInclude(g => g.PeriodoAcademico)
                .Where(pg => pg.Estado);

            // Filtrar por período si está seleccionado
            if (periodoId.HasValue)
            {
                query = query.Where(pg => pg.Grupo.PeriodoAcademicoId == periodoId.Value);
            }

            // Filtrar por grupo si está seleccionado
            if (grupoId.HasValue)
            {
                query = query.Where(pg => pg.GrupoId == grupoId.Value);
            }

            var profesoresGuia = await query
                .OrderBy(pg => pg.Grupo.Nombre)
                .ThenBy(pg => pg.Profesor.PrimerApellido)
                .Select(pg => new ProfesorGuiaViewModel
                {
                    Id = pg.Id,
                    ProfesorId = pg.ProfesorId,
                    NombreProfesor = pg.Profesor.PrimerApellido + " " + 
                                   pg.Profesor.SegundoApellido + ", " + 
                                   pg.Profesor.Nombres,
                    EmailProfesor = pg.Profesor.EmailInstitucional ?? pg.Profesor.EmailPersonal,
                    GrupoId = pg.GrupoId,
                    NombreGrupo = pg.Grupo.Nombre ?? "Sin nombre",
                    CodigoGrupo = pg.Grupo.Codigo,
                    PeriodoAcademico = pg.Grupo.PeriodoAcademico.Nombre,
                    FechaAsignacion = pg.FechaAsignacion,
                    FechaInicio = pg.FechaInicio,
                    FechaFin = pg.FechaFin,
                    Estado = pg.Estado,
                    Observaciones = pg.Observaciones
                })
                .ToListAsync();

            // Cargar grupos para el filtro
            var gruposQuery = _context.GruposEstudiantes
                .Where(g => g.Estado == EstadoGrupo.Activo);

            if (periodoId.HasValue)
            {
                gruposQuery = gruposQuery.Where(g => g.PeriodoAcademicoId == periodoId.Value);
            }

            ViewBag.Grupos = new SelectList(
                await gruposQuery.OrderBy(g => g.Nombre).ToListAsync(),
                "GrupoId",
                "Nombre",
                grupoId
            );

            ViewBag.Periodos = new SelectList(periodos, "Id", "Nombre", periodoId);
            ViewBag.PeriodoSeleccionado = periodoId;
            ViewBag.GrupoSeleccionado = grupoId;

            return View(profesoresGuia);
        }

        // GET: ProfesorGuia/Create
        public async Task<IActionResult> Create()
        {
            await CargarDatosAsync();
            return View(new ProfesorGuiaCreateViewModel());
        }

        // POST: ProfesorGuia/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProfesorGuiaCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Verificar que no exista ya un profesor guía activo para este grupo
                    var existeGuiaActivo = await _context.ProfesorGuia
                        .AnyAsync(pg => pg.GrupoId == model.GrupoId && pg.Estado);

                    if (existeGuiaActivo)
                    {
                        ModelState.AddModelError("", "Ya existe un profesor guía activo para este grupo. Debe desactivar el actual primero.");
                        await CargarDatosAsync();
                        return View(model);
                    }

                    var profesorGuia = new ProfesorGuia
                    {
                        ProfesorId = model.ProfesorId,
                        GrupoId = model.GrupoId,
                        FechaAsignacion = DateTime.Now,
                        FechaInicio = model.FechaInicio,
                        FechaFin = model.FechaFin,
                        Estado = true,
                        Observaciones = model.Observaciones
                    };

                    _context.Add(profesorGuia);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("Profesor guía {ProfesorId} asignado al grupo {GrupoId} por {Usuario}",
                        model.ProfesorId, model.GrupoId, User.Identity?.Name);

                    TempData["Success"] = "Profesor guía asignado exitosamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al asignar profesor guía");
                    ModelState.AddModelError("", "Error al guardar. Por favor, inténtelo nuevamente.");
                }
            }

            await CargarDatosAsync();
            return View(model);
        }

        // GET: ProfesorGuia/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var profesorGuia = await _context.ProfesorGuia
                .Include(pg => pg.Profesor)
                .Include(pg => pg.Grupo)
                .FirstOrDefaultAsync(pg => pg.Id == id);

            if (profesorGuia == null)
            {
                return NotFound();
            }

            var model = new ProfesorGuiaEditViewModel
            {
                Id = profesorGuia.Id,
                ProfesorId = profesorGuia.ProfesorId,
                GrupoId = profesorGuia.GrupoId,
                FechaAsignacion = profesorGuia.FechaAsignacion,
                FechaInicio = profesorGuia.FechaInicio,
                FechaFin = profesorGuia.FechaFin,
                Estado = profesorGuia.Estado,
                Observaciones = profesorGuia.Observaciones,
                NombreProfesor = $"{profesorGuia.Profesor.PrimerApellido} {profesorGuia.Profesor.SegundoApellido}, {profesorGuia.Profesor.Nombres}",
                NombreGrupo = profesorGuia.Grupo.Nombre ?? "Sin nombre"
            };

            return View(model);
        }

        // POST: ProfesorGuia/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProfesorGuiaEditViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var profesorGuia = await _context.ProfesorGuia.FindAsync(id);
                    if (profesorGuia == null)
                    {
                        return NotFound();
                    }

                    profesorGuia.FechaInicio = model.FechaInicio;
                    profesorGuia.FechaFin = model.FechaFin;
                    profesorGuia.Estado = model.Estado;
                    profesorGuia.Observaciones = model.Observaciones;

                    _context.Update(profesorGuia);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("Profesor guía {Id} actualizado por {Usuario}", id, User.Identity?.Name);

                    TempData["Success"] = "Profesor guía actualizado exitosamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProfesorGuiaExists(model.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al actualizar profesor guía");
                    ModelState.AddModelError("", "Error al actualizar. Por favor, inténtelo nuevamente.");
                }
            }

            return View(model);
        }

        // GET: ProfesorGuia/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var profesorGuia = await _context.ProfesorGuia
                .Include(pg => pg.Profesor)
                .Include(pg => pg.Grupo)
                    .ThenInclude(g => g.PeriodoAcademico)
                .FirstOrDefaultAsync(pg => pg.Id == id);

            if (profesorGuia == null)
            {
                return NotFound();
            }

            var model = new ProfesorGuiaViewModel
            {
                Id = profesorGuia.Id,
                ProfesorId = profesorGuia.ProfesorId,
                NombreProfesor = $"{profesorGuia.Profesor.PrimerApellido} {profesorGuia.Profesor.SegundoApellido}, {profesorGuia.Profesor.Nombres}",
                EmailProfesor = profesorGuia.Profesor.EmailInstitucional ?? profesorGuia.Profesor.EmailPersonal,
                GrupoId = profesorGuia.GrupoId,
                NombreGrupo = profesorGuia.Grupo.Nombre ?? "Sin nombre",
                CodigoGrupo = profesorGuia.Grupo.Codigo,
                PeriodoAcademico = profesorGuia.Grupo.PeriodoAcademico?.Nombre ?? "N/A",
                FechaAsignacion = profesorGuia.FechaAsignacion,
                FechaInicio = profesorGuia.FechaInicio,
                FechaFin = profesorGuia.FechaFin,
                Estado = profesorGuia.Estado,
                Observaciones = profesorGuia.Observaciones
            };

            return View(model);
        }

        // POST: ProfesorGuia/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var profesorGuia = await _context.ProfesorGuia.FindAsync(id);
                if (profesorGuia == null)
                {
                    return NotFound();
                }

                // En lugar de eliminar, desactivar
                profesorGuia.Estado = false;
                profesorGuia.FechaFin = DateTime.Now;

                _context.Update(profesorGuia);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Profesor guía {Id} desactivado por {Usuario}", id, User.Identity?.Name);

                TempData["Success"] = "Profesor guía desactivado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al desactivar profesor guía");
                TempData["Error"] = "Error al desactivar. Por favor, inténtelo nuevamente.";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: ProfesorGuia/PorGrupo/5
        public async Task<IActionResult> PorGrupo(int id)
        {
            var grupo = await _context.GruposEstudiantes
                .Include(g => g.PeriodoAcademico)
                .FirstOrDefaultAsync(g => g.GrupoId == id);

            if (grupo == null)
            {
                return NotFound();
            }

            var profesoresGuia = await _context.ProfesorGuia
                .Include(pg => pg.Profesor)
                .Where(pg => pg.GrupoId == id)
                .OrderByDescending(pg => pg.Estado)
                .ThenByDescending(pg => pg.FechaAsignacion)
                .Select(pg => new ProfesorGuiaViewModel
                {
                    Id = pg.Id,
                    ProfesorId = pg.ProfesorId,
                    NombreProfesor = pg.Profesor.PrimerApellido + " " + 
                                   pg.Profesor.SegundoApellido + ", " + 
                                   pg.Profesor.Nombres,
                    EmailProfesor = pg.Profesor.EmailInstitucional ?? pg.Profesor.EmailPersonal,
                    GrupoId = pg.GrupoId,
                    FechaAsignacion = pg.FechaAsignacion,
                    FechaInicio = pg.FechaInicio,
                    FechaFin = pg.FechaFin,
                    Estado = pg.Estado,
                    Observaciones = pg.Observaciones
                })
                .ToListAsync();

            ViewBag.NombreGrupo = grupo.Nombre;
            ViewBag.CodigoGrupo = grupo.Codigo;
            ViewBag.PeriodoAcademico = grupo.PeriodoAcademico?.Nombre;
            ViewBag.GrupoId = id;

            return View(profesoresGuia);
        }

        private bool ProfesorGuiaExists(int id)
        {
            return _context.ProfesorGuia.Any(e => e.Id == id);
        }

        private async Task CargarDatosAsync()
        {
            // Cargar profesores activos
            var profesores = await _context.Profesores
                .Where(p => p.Estado == true)  // Comparación explícita con bool
                .OrderBy(p => p.PrimerApellido)
                .ThenBy(p => p.SegundoApellido)
                .ThenBy(p => p.Nombres)
                .Select(p => new
                {
                    p.Id,
                    NombreCompleto = p.PrimerApellido + " " + p.SegundoApellido + ", " + p.Nombres
                })
                .ToListAsync();

            ViewBag.Profesores = new SelectList(profesores, "Id", "NombreCompleto");

            // Cargar grupos activos
            var grupos = await _context.GruposEstudiantes
                .Include(g => g.PeriodoAcademico)
                .Where(g => g.Estado == EstadoGrupo.Activo)
                .OrderBy(g => g.PeriodoAcademico.FechaInicio)
                .ThenBy(g => g.Nombre)
                .Select(g => new
                {
                    g.GrupoId,
                    NombreCompleto = g.Nombre + " - " + g.PeriodoAcademico.Nombre
                })
                .ToListAsync();

            ViewBag.Grupos = new SelectList(grupos, "GrupoId", "NombreCompleto");
        }
    }
}
