using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DocinadeApp.Data;
using DocinadeApp.Models;
using DocinadeApp.ViewModels;
using DocinadeApp.Services;
using DocinadeApp.Authorization;
using DocinadeApp.Models.Permissions;
using Microsoft.AspNetCore.Authorization;

namespace DocinadeApp.Controllers
{
    [Authorize]
    public class InstitucionesController : BaseController
    {
        private readonly ILogger<InstitucionesController> _logger;

        public InstitucionesController(
            RubricasDbContext context,
            IPeriodoAcademicoService periodoService,
            ILogger<InstitucionesController> logger)
            : base(periodoService, context)
        {
            _logger = logger;
        }

        // GET: Instituciones
        [RequirePermission(ApplicationPermissions.Instituciones.VER)]
        public async Task<IActionResult> Index(string? searchTerm, bool? estado)
        {
            try
            {
                var query = _context.Instituciones.AsQueryable();

                // Aplicar filtro de búsqueda
                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    query = query.Where(i => i.Nombre.Contains(searchTerm) ||
                                            (i.CodigoMEP != null && i.CodigoMEP.Contains(searchTerm)) ||
                                            (i.DireccionRegional != null && i.DireccionRegional.Contains(searchTerm)));
                }

                // Aplicar filtro de estado
                if (estado.HasValue)
                {
                    query = query.Where(i => i.Estado == estado.Value);
                }

                var instituciones = await query
                    .OrderBy(i => i.Nombre)
                    .Select(i => new InstitucionListViewModel
                    {
                        Id = i.Id,
                        Nombre = i.Nombre,
                        CodigoMEP = i.CodigoMEP,
                        DireccionRegional = i.DireccionRegional,
                        CircuitoEscolar = i.CircuitoEscolar,
                        TipoInstitucion = i.TipoInstitucion,
                        Estado = i.Estado,
                        FechaCreacion = i.FechaCreacion
                    })
                    .ToListAsync();

                // Pasar valores de búsqueda a la vista
                ViewBag.SearchTerm = searchTerm;
                ViewBag.EstadoSeleccionado = estado;

                return View(instituciones);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar la lista de instituciones");
                TempData["Error"] = "Error al cargar la lista de instituciones. Por favor, intente nuevamente.";
                return View(new List<InstitucionListViewModel>());
            }
        }

        // GET: Instituciones/Details/5
        [RequirePermission(ApplicationPermissions.Instituciones.VER_DETALLES)]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var institucion = await _context.Instituciones
                    .Include(i => i.Facultades)
                    .FirstOrDefaultAsync(i => i.Id == id);

                if (institucion == null)
                {
                    return NotFound();
                }

                var viewModel = new InstitucionDetailsViewModel
                {
                    Id = institucion.Id,
                    Nombre = institucion.Nombre,
                    Siglas = institucion.Siglas,
                    TipoInstitucion = institucion.TipoInstitucion,
                    CodigoMEP = institucion.CodigoMEP,
                    DireccionRegional = institucion.DireccionRegional,
                    CircuitoEscolar = institucion.CircuitoEscolar,
                    Telefono = institucion.Telefono,
                    Email = institucion.Email,
                    SitioWeb = institucion.SitioWeb,
                    Direccion = institucion.Direccion,
                    Estado = institucion.Estado,
                    FechaCreacion = institucion.FechaCreacion,
                    CantidadFacultades = institucion.Facultades.Count,
                    Facultades = institucion.Facultades.Select(f => new FacultadSimpleViewModel
                    {
                        Id = f.Id,
                        Nombre = f.Nombre,
                        Codigo = f.Codigo,
                        Estado = f.Estado
                    }).ToList()
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar detalles de la institución {Id}", id);
                TempData["Error"] = "Error al cargar los detalles de la institución.";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Instituciones/Create
        [RequirePermission(ApplicationPermissions.Instituciones.CREAR)]
        public IActionResult Create()
        {
            ViewBag.TiposInstitucion = GetTiposInstitucionSelectList();
            return View(new InstitucionCreateViewModel());
        }

        // POST: Instituciones/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequirePermission(ApplicationPermissions.Instituciones.CREAR)]
        public async Task<IActionResult> Create(InstitucionCreateViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var institucion = new Institucion
                    {
                        Nombre = viewModel.Nombre,
                        Siglas = viewModel.Siglas,
                        TipoInstitucion = viewModel.TipoInstitucion,
                        CodigoMEP = viewModel.CodigoMEP,
                        DireccionRegional = viewModel.DireccionRegional,
                        CircuitoEscolar = viewModel.CircuitoEscolar,
                        Telefono = viewModel.Telefono,
                        Email = viewModel.Email,
                        SitioWeb = viewModel.SitioWeb,
                        Direccion = viewModel.Direccion,
                        Estado = viewModel.Estado,
                        FechaCreacion = DateTime.Now
                    };

                    _context.Instituciones.Add(institucion);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = $"La institución '{institucion.Nombre}' ha sido creada exitosamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al crear la institución");
                    ModelState.AddModelError("", "Error al crear la institución. Por favor, intente nuevamente.");
                }
            }

            ViewBag.TiposInstitucion = GetTiposInstitucionSelectList();
            return View(viewModel);
        }

        // GET: Instituciones/Edit/5
        [RequirePermission(ApplicationPermissions.Instituciones.EDITAR)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var institucion = await _context.Instituciones.FindAsync(id);
                if (institucion == null)
                {
                    return NotFound();
                }

                var viewModel = new InstitucionEditViewModel
                {
                    Id = institucion.Id,
                    Nombre = institucion.Nombre,
                    Siglas = institucion.Siglas,
                    TipoInstitucion = institucion.TipoInstitucion,
                    CodigoMEP = institucion.CodigoMEP,
                    DireccionRegional = institucion.DireccionRegional,
                    CircuitoEscolar = institucion.CircuitoEscolar,
                    Telefono = institucion.Telefono,
                    Email = institucion.Email,
                    SitioWeb = institucion.SitioWeb,
                    Direccion = institucion.Direccion,
                    Estado = institucion.Estado,
                    FechaCreacion = institucion.FechaCreacion
                };

                ViewBag.TiposInstitucion = GetTiposInstitucionSelectList(institucion.TipoInstitucion);
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar la institución para editar {Id}", id);
                TempData["Error"] = "Error al cargar la institución.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Instituciones/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequirePermission(ApplicationPermissions.Instituciones.EDITAR)]
        public async Task<IActionResult> Edit(int id, InstitucionEditViewModel viewModel)
        {
            if (id != viewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var institucion = await _context.Instituciones.FindAsync(id);
                    if (institucion == null)
                    {
                        return NotFound();
                    }

                    institucion.Nombre = viewModel.Nombre;
                    institucion.Siglas = viewModel.Siglas;
                    institucion.TipoInstitucion = viewModel.TipoInstitucion;
                    institucion.CodigoMEP = viewModel.CodigoMEP;
                    institucion.DireccionRegional = viewModel.DireccionRegional;
                    institucion.CircuitoEscolar = viewModel.CircuitoEscolar;
                    institucion.Telefono = viewModel.Telefono;
                    institucion.Email = viewModel.Email;
                    institucion.SitioWeb = viewModel.SitioWeb;
                    institucion.Direccion = viewModel.Direccion;
                    institucion.Estado = viewModel.Estado;

                    _context.Update(institucion);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = $"La institución '{institucion.Nombre}' ha sido actualizada exitosamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!InstitucionExists(viewModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        _logger.LogError(ex, "Error de concurrencia al editar la institución {Id}", id);
                        ModelState.AddModelError("", "La institución fue modificada por otro usuario. Por favor, recargue la página.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al editar la institución {Id}", id);
                    ModelState.AddModelError("", "Error al editar la institución. Por favor, intente nuevamente.");
                }
            }

            ViewBag.TiposInstitucion = GetTiposInstitucionSelectList(viewModel.TipoInstitucion);
            return View(viewModel);
        }

        // GET: Instituciones/Delete/5
        [RequirePermission(ApplicationPermissions.Instituciones.ELIMINAR)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var institucion = await _context.Instituciones
                    .Include(i => i.Facultades)
                    .FirstOrDefaultAsync(i => i.Id == id);

                if (institucion == null)
                {
                    return NotFound();
                }

                var viewModel = new InstitucionDetailsViewModel
                {
                    Id = institucion.Id,
                    Nombre = institucion.Nombre,
                    Siglas = institucion.Siglas,
                    TipoInstitucion = institucion.TipoInstitucion,
                    CodigoMEP = institucion.CodigoMEP,
                    DireccionRegional = institucion.DireccionRegional,
                    CircuitoEscolar = institucion.CircuitoEscolar,
                    Telefono = institucion.Telefono,
                    Email = institucion.Email,
                    SitioWeb = institucion.SitioWeb,
                    Direccion = institucion.Direccion,
                    Estado = institucion.Estado,
                    FechaCreacion = institucion.FechaCreacion,
                    CantidadFacultades = institucion.Facultades.Count
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar la institución para eliminar {Id}", id);
                TempData["Error"] = "Error al cargar la institución.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Instituciones/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [RequirePermission(ApplicationPermissions.Instituciones.ELIMINAR)]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var institucion = await _context.Instituciones
                    .Include(i => i.Facultades.Where(f => f.Estado))
                    .FirstOrDefaultAsync(i => i.Id == id);

                if (institucion == null)
                {
                    return NotFound();
                }

                // Verificar si tiene facultades activas
                if (institucion.Facultades.Any())
                {
                    TempData["Error"] = $"No se puede eliminar la institución '{institucion.Nombre}' porque tiene {institucion.Facultades.Count} facultades activas asociadas.";
                    return RedirectToAction(nameof(Delete), new { id });
                }

                // Eliminación lógica
                institucion.Estado = false;
                _context.Update(institucion);
                await _context.SaveChangesAsync();

                TempData["Success"] = $"La institución '{institucion.Nombre}' ha sido desactivada exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar la institución {Id}", id);
                TempData["Error"] = "Error al eliminar la institución. Por favor, intente nuevamente.";
                return RedirectToAction(nameof(Index));
            }
        }

        private bool InstitucionExists(int id)
        {
            return _context.Instituciones.Any(e => e.Id == id);
        }

        private SelectList GetTiposInstitucionSelectList(string? selectedValue = null)
        {
            var tipos = new List<string>
            {
                "Universidad Pública",
                "Universidad Privada",
                "Colegio Técnico Profesional",
                "Liceo",
                "Colegio",
                "Escuela",
                "Instituto",
                "Centro Educativo",
                "Parauniversidad"
            };

            return new SelectList(tipos, selectedValue);
        }
    }
}
