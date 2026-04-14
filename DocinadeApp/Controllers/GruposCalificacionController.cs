using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RubricasApp.Web.Data;
using RubricasApp.Web.Models;
using Microsoft.AspNetCore.Authorization;

namespace RubricasApp.Web.Controllers
{
    [Authorize]
    public class GruposCalificacionController : Controller
    {
        private readonly RubricasDbContext _context;

        public GruposCalificacionController(RubricasDbContext context)
        {
            _context = context;
        }

        // GET: GruposCalificacion
        public async Task<IActionResult> Index()
        {
            try
            {
                var grupos = await _context.GruposCalificacion
                    .Include(g => g.NivelesCalificacion)
                    .OrderBy(g => g.NombreGrupo)
                    .ToListAsync();
                return View(grupos);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al cargar los grupos: {ex.Message}";
                return View(new List<GrupoCalificacion>());
            }
        }

        // GET: GruposCalificacion/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var grupo = await _context.GruposCalificacion
                .Include(g => g.NivelesCalificacion)
                .FirstOrDefaultAsync(m => m.IdGrupo == id);
                
            if (grupo == null)
            {
                return NotFound();
            }

            return View(grupo);
        }

        // GET: GruposCalificacion/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: GruposCalificacion/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NombreGrupo,Descripcion")] GrupoCalificacion grupo)
        {
            if (ModelState.IsValid)
            {
                grupo.Estado = "ACTIVO";
                grupo.FechaCreacion = DateTime.Now;
                _context.Add(grupo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(grupo);
        }

        // GET: GruposCalificacion/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var grupo = await _context.GruposCalificacion.FindAsync(id);
            if (grupo == null)
            {
                return NotFound();
            }
            return View(grupo);
        }

        // POST: GruposCalificacion/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdGrupo,NombreGrupo,Descripcion,Estado")] GrupoCalificacion grupo)
        {
            if (id != grupo.IdGrupo)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Mantener la fecha de creación original
                    var grupoOriginal = await _context.GruposCalificacion.AsNoTracking().FirstOrDefaultAsync(g => g.IdGrupo == id);
                    if (grupoOriginal != null)
                    {
                        grupo.FechaCreacion = grupoOriginal.FechaCreacion;
                    }

                    _context.Update(grupo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GrupoCalificacionExists(grupo.IdGrupo))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(grupo);
        }

        // GET: GruposCalificacion/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var grupo = await _context.GruposCalificacion
                .Include(g => g.NivelesCalificacion)
                .FirstOrDefaultAsync(m => m.IdGrupo == id);
                
            if (grupo == null)
            {
                return NotFound();
            }

            return View(grupo);
        }

        // POST: GruposCalificacion/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var grupo = await _context.GruposCalificacion.FindAsync(id);
            if (grupo != null)
            {
                // Verificar si tiene niveles o rúbricas asociadas
                var tieneDependencias = await _context.NivelesCalificacion.AnyAsync(n => n.IdGrupo == id) ||
                                       await _context.Rubricas.AnyAsync(r => r.IdGrupo == id);

                if (tieneDependencias)
                {
                    TempData["Error"] = "No se puede eliminar el grupo porque tiene niveles de calificación o rúbricas asociadas.";
                    return RedirectToAction(nameof(Index));
                }

                _context.GruposCalificacion.Remove(grupo);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Grupo de calificación eliminado exitosamente.";
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: GruposCalificacion/CreateDynamicGroup
        [HttpPost]
        public async Task<IActionResult> CreateDynamicGroup(int rubricaId, int[] selectedLevels)
        {
            try
            {
                if (selectedLevels == null || selectedLevels.Length == 0)
                {
                    return Json(new { success = false, message = "Debe seleccionar al menos un nivel de calificación." });
                }

                var rubrica = await _context.Rubricas.FindAsync(rubricaId);
                if (rubrica == null)
                {
                    return Json(new { success = false, message = "Rúbrica no encontrada." });
                }

                // Crear nombre del grupo dinámico
                var nombreGrupo = $"Grupo_{rubrica.NombreRubrica}_{DateTime.Now:yyyyMMdd_HHmmss}";

                // Crear el nuevo grupo
                var nuevoGrupo = new GrupoCalificacion
                {
                    NombreGrupo = nombreGrupo,
                    Descripcion = $"Grupo dinámico creado para la rúbrica '{rubrica.NombreRubrica}' con {selectedLevels.Length} niveles seleccionados",
                    Estado = "ACTIVO",
                    FechaCreacion = DateTime.Now
                };

                _context.GruposCalificacion.Add(nuevoGrupo);
                await _context.SaveChangesAsync();

                // Asignar los niveles seleccionados al grupo
                var niveles = await _context.NivelesCalificacion
                    .Where(n => selectedLevels.Contains(n.IdNivel))
                    .ToListAsync();

                foreach (var nivel in niveles)
                {
                    nivel.IdGrupo = nuevoGrupo.IdGrupo;
                }

                // Asignar el grupo a la rúbrica
                rubrica.IdGrupo = nuevoGrupo.IdGrupo;

                await _context.SaveChangesAsync();

                return Json(new { 
                    success = true, 
                    message = $"Grupo dinámico '{nombreGrupo}' creado exitosamente con {selectedLevels.Length} niveles.",
                    groupId = nuevoGrupo.IdGrupo,
                    groupName = nombreGrupo
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error al crear el grupo dinámico: {ex.Message}" });
            }
        }

        // GET: GruposCalificacion/GetAvailableLevels
        [HttpGet]
        public async Task<IActionResult> GetAvailableLevels()
        {
            var niveles = await _context.NivelesCalificacion
                .Where(n => n.IdGrupo == null) // Solo niveles sin grupo asignado
                .OrderBy(n => n.OrdenNivel ?? int.MaxValue)
                .ThenBy(n => n.NombreNivel)
                .Select(n => new { 
                    value = n.IdNivel, 
                    text = n.NombreNivel,
                    description = n.Descripcion
                })
                .ToListAsync();

            return Json(niveles);
        }

        // GET: GruposCalificacion/AssignLevels/5
        public async Task<IActionResult> AssignLevels(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var grupo = await _context.GruposCalificacion
                .Include(g => g.NivelesCalificacion)
                .FirstOrDefaultAsync(m => m.IdGrupo == id);
                
            if (grupo == null)
            {
                return NotFound();
            }

            return View(grupo);
        }

        // POST: GruposCalificacion/AssignLevels
        [HttpPost]
        public async Task<IActionResult> AssignLevels([FromBody] AssignLevelsRequest request)
        {
            try
            {
                var grupo = await _context.GruposCalificacion.FindAsync(request.GroupId);
                if (grupo == null)
                {
                    return Json(new { success = false, message = "Grupo no encontrado." });
                }

                // Obtener niveles actuales del grupo
                var nivelesActuales = await _context.NivelesCalificacion
                    .Where(n => n.IdGrupo == request.GroupId)
                    .ToListAsync();

                // Desasignar niveles actuales
                foreach (var nivel in nivelesActuales)
                {
                    nivel.IdGrupo = null;
                }

                // Asignar nuevos niveles
                if (request.LevelIds != null && request.LevelIds.Any())
                {
                    var nuevosNiveles = await _context.NivelesCalificacion
                        .Where(n => request.LevelIds.Contains(n.IdNivel))
                        .ToListAsync();

                    foreach (var nivel in nuevosNiveles)
                    {
                        nivel.IdGrupo = request.GroupId;
                    }
                }

                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Niveles asignados exitosamente." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error al asignar niveles: {ex.Message}" });
            }
        }

        // API: Crear grupo rápido desde modal
        [HttpPost]
        public async Task<IActionResult> CrearGrupoRapido([FromBody] CrearGrupoRapidoRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.NombreGrupo))
                {
                    return Json(new { success = false, message = "El nombre del grupo es requerido." });
                }

                if (request.NivelesIds == null || request.NivelesIds.Length == 0)
                {
                    return Json(new { success = false, message = "Debe seleccionar al menos un nivel." });
                }

                var grupo = new GrupoCalificacion
                {
                    NombreGrupo = request.NombreGrupo,
                    Descripcion = request.Descripcion ?? $"Grupo creado con {request.NivelesIds.Length} niveles",
                    Estado = "ACTIVO",
                    FechaCreacion = DateTime.Now
                };

                _context.Add(grupo);
                await _context.SaveChangesAsync();

                // Asignar niveles al grupo
                var niveles = await _context.NivelesCalificacion
                    .Where(n => request.NivelesIds.Contains(n.IdNivel))
                    .ToListAsync();

                foreach (var nivel in niveles)
                {
                    nivel.IdGrupo = grupo.IdGrupo;
                }

                await _context.SaveChangesAsync();

                return Json(new
                {
                    success = true,
                    message = $"Grupo '{grupo.NombreGrupo}' creado exitosamente.",
                    grupoId = grupo.IdGrupo,
                    nombreGrupo = grupo.NombreGrupo,
                    cantidadNiveles = niveles.Count
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error al crear el grupo: {ex.Message}" });
            }
        }

        // API: Eliminar grupo vía AJAX
        [HttpPost]
        public async Task<IActionResult> EliminarGrupoAjax([FromBody] int grupoId)
        {
            try
            {
                var grupo = await _context.GruposCalificacion
                    .Include(g => g.NivelesCalificacion)
                    .Include(g => g.Rubricas)
                    .FirstOrDefaultAsync(g => g.IdGrupo == grupoId);

                if (grupo == null)
                {
                    return Json(new { success = false, message = "Grupo no encontrado." });
                }

                // Verificar si tiene niveles o rúbricas asociadas
                var tieneDependencias = grupo.NivelesCalificacion.Any() || grupo.Rubricas.Any();

                if (tieneDependencias)
                {
                    return Json(new
                    {
                        success = false,
                        message = "No se puede eliminar el grupo porque tiene niveles de calificación o rúbricas asociadas.",
                        nivelesCount = grupo.NivelesCalificacion.Count,
                        rubricasCount = grupo.Rubricas.Count
                    });
                }

                _context.GruposCalificacion.Remove(grupo);
                await _context.SaveChangesAsync();

                return Json(new
                {
                    success = true,
                    message = $"Grupo '{grupo.NombreGrupo}' eliminado exitosamente."
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error al eliminar el grupo: {ex.Message}" });
            }
        }

        private bool GrupoCalificacionExists(int id)
        {
            return _context.GruposCalificacion.Any(e => e.IdGrupo == id);
        }
    }

    // Clase para el request de asignación de niveles
    public class AssignLevelsRequest
    {
        public int GroupId { get; set; }
        public List<int> LevelIds { get; set; } = new List<int>();
    }

    // Clase para el request de creación rápida
    public class CrearGrupoRapidoRequest
    {
        public string NombreGrupo { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public int[] NivelesIds { get; set; } = Array.Empty<int>();
    }
}