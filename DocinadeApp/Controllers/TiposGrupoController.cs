using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using DocinadeApp.Data;
using DocinadeApp.Models;
using DocinadeApp.ViewModels.Grupos;
using DocinadeApp.Authorization;
using DocinadeApp.Models.Permissions;

namespace DocinadeApp.Controllers
{
    [Authorize]
    public class TiposGrupoController : Controller
    {
        private readonly RubricasDbContext _context;
        private readonly ILogger<TiposGrupoController> _logger;

        public TiposGrupoController(RubricasDbContext context, ILogger<TiposGrupoController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: TiposGrupo
        [RequirePermission(ApplicationPermissions.TiposGrupo.VER)]
        public async Task<IActionResult> Index()
        {
            try
            {
                var tiposGrupo = await _context.TiposGrupo
                    .Include(t => t.GruposEstudiantes)
                    .OrderBy(t => t.Nombre)
                    .ToListAsync();

                var viewModel = tiposGrupo.Select(t => new TipoGrupoListViewModel
                {
                    IdTipoGrupo = t.IdTipoGrupo,
                    Nombre = t.Nombre,
                    FechaRegistro = t.FechaRegistro,
                    Estado = t.Estado,
                    EsActivo = t.EsActivo,
                    CantidadGrupos = t.GruposEstudiantes.Count,
                    PuedeEliminar = t.GruposEstudiantes.Count == 0
                }).ToList();

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar los tipos de grupo");
                TempData["ErrorMessage"] = "Error al cargar los tipos de grupo. Por favor, intente nuevamente.";
                return View(new List<TipoGrupoListViewModel>());
            }
        }

        // GET: TiposGrupo/Details/5
        [RequirePermission(ApplicationPermissions.TiposGrupo.VER_DETALLES)]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var tipoGrupo = await _context.TiposGrupo
                    .Include(t => t.GruposEstudiantes)
                        .ThenInclude(g => g.PeriodoAcademico)
                    .FirstOrDefaultAsync(t => t.IdTipoGrupo == id);

                if (tipoGrupo == null)
                {
                    return NotFound();
                }

                var viewModel = new TipoGrupoDetailsViewModel
                {
                    IdTipoGrupo = tipoGrupo.IdTipoGrupo,
                    Nombre = tipoGrupo.Nombre,
                    FechaRegistro = tipoGrupo.FechaRegistro,
                    Estado = tipoGrupo.Estado,
                    EsActivo = tipoGrupo.EsActivo,
                    GruposAsociados = tipoGrupo.GruposEstudiantes.Select(g => new GrupoAsociadoViewModel
                    {
                        GrupoId = g.GrupoId,
                        Codigo = g.Codigo,
                        Nombre = g.Nombre,
                        PeriodoAcademico = g.PeriodoAcademico?.Nombre ?? "",
                        Estado = g.Estado,
                        EstadoDisplay = g.Estado.ToString(),
                        FechaCreacion = g.FechaCreacion
                    }).OrderByDescending(g => g.FechaCreacion).ToList()
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar los detalles del tipo de grupo {TipoGrupoId}", id);
                return NotFound();
            }
        }

        // GET: TiposGrupo/Create
        [RequirePermission(ApplicationPermissions.TiposGrupo.CREAR)]
        public IActionResult Create()
        {
            var viewModel = new CreateTipoGrupoViewModel
            {
                Estado = "Activo"
            };

            ViewBag.EstadosDisponibles = GetEstadosDisponibles();
            return View(viewModel);
        }

        // POST: TiposGrupo/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequirePermission(ApplicationPermissions.TiposGrupo.CREAR)]
        public async Task<IActionResult> Create(CreateTipoGrupoViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Verificar que el nombre sea �nico
                    var nombreExiste = await _context.TiposGrupo
                        .AnyAsync(t => t.Nombre == viewModel.Nombre);

                    if (nombreExiste)
                    {
                        ModelState.AddModelError("Nombre", "Ya existe un tipo de grupo con este nombre");
                    }
                    else
                    {
                        var tipoGrupo = new TipoGrupoCatalogo
                        {
                            Nombre = viewModel.Nombre,
                            Estado = viewModel.Estado,
                            FechaRegistro = DateTime.Now
                        };

                        _context.TiposGrupo.Add(tipoGrupo);
                        await _context.SaveChangesAsync();

                        _logger.LogInformation("Tipo de grupo '{Nombre}' creado exitosamente", viewModel.Nombre);
                        TempData["SuccessMessage"] = "Tipo de grupo creado exitosamente";

                        return RedirectToAction(nameof(Details), new { id = tipoGrupo.IdTipoGrupo });
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al crear tipo de grupo '{Nombre}'", viewModel.Nombre);
                    ModelState.AddModelError("", "Error interno al crear el tipo de grupo");
                }
            }

            ViewBag.EstadosDisponibles = GetEstadosDisponibles();
            return View(viewModel);
        }

        // GET: TiposGrupo/Edit/5
        [RequirePermission(ApplicationPermissions.TiposGrupo.EDITAR)]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var tipoGrupo = await _context.TiposGrupo.FindAsync(id);
                if (tipoGrupo == null)
                {
                    return NotFound();
                }

                var viewModel = new EditTipoGrupoViewModel
                {
                    IdTipoGrupo = tipoGrupo.IdTipoGrupo,
                    Nombre = tipoGrupo.Nombre,
                    Estado = tipoGrupo.Estado,
                    FechaRegistro = tipoGrupo.FechaRegistro
                };

                ViewBag.EstadosDisponibles = GetEstadosDisponibles();
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar tipo de grupo para edici�n {TipoGrupoId}", id);
                return NotFound();
            }
        }

        // POST: TiposGrupo/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequirePermission(ApplicationPermissions.TiposGrupo.EDITAR)]
        public async Task<IActionResult> Edit(EditTipoGrupoViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var tipoGrupo = await _context.TiposGrupo.FindAsync(viewModel.IdTipoGrupo);
                    if (tipoGrupo == null)
                    {
                        return NotFound();
                    }

                    // Verificar que el nombre sea �nico (excepto para el registro actual)
                    if (tipoGrupo.Nombre != viewModel.Nombre)
                    {
                        var nombreExiste = await _context.TiposGrupo
                            .AnyAsync(t => t.Nombre == viewModel.Nombre && t.IdTipoGrupo != viewModel.IdTipoGrupo);

                        if (nombreExiste)
                        {
                            ModelState.AddModelError("Nombre", "Ya existe un tipo de grupo con este nombre");
                            ViewBag.EstadosDisponibles = GetEstadosDisponibles();
                            return View(viewModel);
                        }
                    }

                    tipoGrupo.Nombre = viewModel.Nombre;
                    tipoGrupo.Estado = viewModel.Estado;

                    _context.TiposGrupo.Update(tipoGrupo);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("Tipo de grupo '{Nombre}' actualizado exitosamente", viewModel.Nombre);
                    TempData["SuccessMessage"] = "Tipo de grupo actualizado exitosamente";

                    return RedirectToAction(nameof(Details), new { id = viewModel.IdTipoGrupo });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al actualizar tipo de grupo {TipoGrupoId}", viewModel.IdTipoGrupo);
                    ModelState.AddModelError("", "Error interno al actualizar el tipo de grupo");
                }
            }

            ViewBag.EstadosDisponibles = GetEstadosDisponibles();
            return View(viewModel);
        }

        // GET: TiposGrupo/Delete/5
        [RequirePermission(ApplicationPermissions.TiposGrupo.ELIMINAR)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var tipoGrupo = await _context.TiposGrupo
                    .Include(t => t.GruposEstudiantes)
                    .FirstOrDefaultAsync(t => t.IdTipoGrupo == id);

                if (tipoGrupo == null)
                {
                    return NotFound();
                }

                var viewModel = new DeleteTipoGrupoViewModel
                {
                    IdTipoGrupo = tipoGrupo.IdTipoGrupo,
                    Nombre = tipoGrupo.Nombre,
                    FechaRegistro = tipoGrupo.FechaRegistro,
                    Estado = tipoGrupo.Estado,
                    CantidadGrupos = tipoGrupo.GruposEstudiantes.Count,
                    PuedeEliminar = tipoGrupo.GruposEstudiantes.Count == 0,
                    GruposAsociados = tipoGrupo.GruposEstudiantes.Take(5).Select(g => g.Codigo).ToList()
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar tipo de grupo para eliminar {TipoGrupoId}", id);
                return NotFound();
            }
        }

        // POST: TiposGrupo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [RequirePermission(ApplicationPermissions.TiposGrupo.ELIMINAR)]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var tipoGrupo = await _context.TiposGrupo
                    .Include(t => t.GruposEstudiantes)
                    .FirstOrDefaultAsync(t => t.IdTipoGrupo == id);

                if (tipoGrupo == null)
                {
                    return NotFound();
                }

                if (tipoGrupo.GruposEstudiantes.Any())
                {
                    TempData["ErrorMessage"] = "No se puede eliminar un tipo de grupo que tiene grupos asociados";
                    return RedirectToAction(nameof(Delete), new { id });
                }

                _context.TiposGrupo.Remove(tipoGrupo);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Tipo de grupo '{Nombre}' eliminado exitosamente", tipoGrupo.Nombre);
                TempData["SuccessMessage"] = "Tipo de grupo eliminado exitosamente";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar tipo de grupo {TipoGrupoId}", id);
                TempData["ErrorMessage"] = "Error interno al eliminar el tipo de grupo";
                return RedirectToAction(nameof(Index));
            }
        }

        // AJAX: Cambiar estado
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequirePermission(ApplicationPermissions.TiposGrupo.CAMBIAR_ESTADO)]
        public async Task<IActionResult> ToggleEstado(int id)
        {
            try
            {
                var tipoGrupo = await _context.TiposGrupo.FindAsync(id);
                if (tipoGrupo == null)
                {
                    return Json(new { success = false, message = "Tipo de grupo no encontrado" });
                }

                tipoGrupo.Estado = tipoGrupo.Estado == "Activo" ? "Inactivo" : "Activo";
                await _context.SaveChangesAsync();

                return Json(new { 
                    success = true, 
                    nuevoEstado = tipoGrupo.Estado,
                    mensaje = $"Tipo de grupo {tipoGrupo.Estado.ToLower()} exitosamente"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cambiar estado del tipo de grupo {TipoGrupoId}", id);
                return Json(new { success = false, message = "Error interno al cambiar el estado" });
            }
        }

        #region M�todos Privados

        private List<EstadoOption> GetEstadosDisponibles()
        {
            return new List<EstadoOption>
            {
                new EstadoOption { Valor = "Activo", Nombre = "Activo", Clase = "bg-success" },
                new EstadoOption { Valor = "Inactivo", Nombre = "Inactivo", Clase = "bg-secondary" }
            };
        }

        #endregion
    }

    // Clase auxiliar para estados
    public class EstadoOption
    {
        public string Valor { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Clase { get; set; } = string.Empty;
    }
}