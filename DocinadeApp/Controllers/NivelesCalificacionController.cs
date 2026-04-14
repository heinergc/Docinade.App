using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DocinadeApp.Data;
using DocinadeApp.Models;
using DocinadeApp.Models.Permissions;
using DocinadeApp.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace DocinadeApp.Controllers
{
    [Authorize]
    public class NivelesCalificacionController : Controller
    {
        private readonly RubricasDbContext _context;

        public NivelesCalificacionController(RubricasDbContext context)
        {
            _context = context;
        }

        // GET: NivelesCalificacion
        [RequirePermission(ApplicationPermissions.Niveles.VER)]
        public async Task<IActionResult> Index()
        {
            var niveles = await _context.NivelesCalificacion
                .OrderBy(n => n.OrdenNivel ?? int.MaxValue)
                .ThenBy(n => n.NombreNivel)
                .ToListAsync();
            return View(niveles);
        }

        // GET: NivelesCalificacion/Details/5
        [RequirePermission(ApplicationPermissions.Niveles.VER)]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nivel = await _context.NivelesCalificacion
                .FirstOrDefaultAsync(m => m.IdNivel == id);

            if (nivel == null)
            {
                return NotFound();
            }

            return View(nivel);
        }

        // GET: NivelesCalificacion/Create
        [RequirePermission(ApplicationPermissions.Niveles.CREAR)]
        public IActionResult Create()
        {
            return View();
        }

        // POST: NivelesCalificacion/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequirePermission(ApplicationPermissions.Niveles.CREAR)]
        public async Task<IActionResult> Create([Bind("NombreNivel,Descripcion,OrdenNivel")] NivelCalificacion nivel)
        {
            // Validar que no exista un nivel con el mismo nombre (case-insensitive)
            if (!string.IsNullOrEmpty(nivel.NombreNivel))
            {
                var existeNivel = await _context.NivelesCalificacion
                    .AnyAsync(n => n.NombreNivel.ToLower() == nivel.NombreNivel.ToLower());
                
                if (existeNivel)
                {
                    ModelState.AddModelError("NombreNivel", $"Ya existe un nivel de calificación con el nombre '{nivel.NombreNivel}'. Por favor, elija un nombre diferente.");
                }
            }
            
            if (ModelState.IsValid)
            {
                _context.Add(nivel);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Nivel de calificación creado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            return View(nivel);
        }

        // GET: NivelesCalificacion/Edit/5
        [RequirePermission(ApplicationPermissions.Niveles.EDITAR)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nivel = await _context.NivelesCalificacion.FindAsync(id);
            if (nivel == null)
            {
                return NotFound();
            }
            return View(nivel);
        }

        // POST: NivelesCalificacion/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequirePermission(ApplicationPermissions.Niveles.EDITAR)]
        public async Task<IActionResult> Edit(int id, [Bind("IdNivel,NombreNivel,Descripcion,OrdenNivel")] NivelCalificacion nivel)
        {
            if (id != nivel.IdNivel)
            {
                return NotFound();
            }

            // Validar que no exista otro nivel con el mismo nombre (case-insensitive)
            if (!string.IsNullOrEmpty(nivel.NombreNivel))
            {
                var existeOtroNivel = await _context.NivelesCalificacion
                    .AnyAsync(n => n.NombreNivel.ToLower() == nivel.NombreNivel.ToLower() && n.IdNivel != nivel.IdNivel);
                
                if (existeOtroNivel)
                {
                    ModelState.AddModelError("NombreNivel", $"Ya existe otro nivel de calificación con el nombre '{nivel.NombreNivel}'. Por favor, elija un nombre diferente.");
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(nivel);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Nivel de calificación actualizado exitosamente.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NivelExists(nivel.IdNivel))
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
            return View(nivel);
        }

        // GET: NivelesCalificacion/Delete/5
        [RequirePermission(ApplicationPermissions.Niveles.ELIMINAR)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nivel = await _context.NivelesCalificacion
                .FirstOrDefaultAsync(m => m.IdNivel == id);
            if (nivel == null)
            {
                return NotFound();
            }

            return View(nivel);
        }

        // POST: NivelesCalificacion/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [RequirePermission(ApplicationPermissions.Niveles.ELIMINAR)]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var nivel = await _context.NivelesCalificacion.FindAsync(id);
            if (nivel != null)
            {
                try
                {
                    _context.NivelesCalificacion.Remove(nivel);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Nivel de calificación eliminado exitosamente.";
                }
                catch (DbUpdateException)
                {
                    TempData["ErrorMessage"] = "No se puede eliminar el nivel porque está siendo usado en evaluaciones.";
                    return RedirectToAction(nameof(Index));
                }
            }

            return RedirectToAction(nameof(Index));
        }

        private bool NivelExists(int id)
        {
            return _context.NivelesCalificacion.Any(e => e.IdNivel == id);
        }

        // Método AJAX para verificar si ya existe un nivel con el mismo nombre
        [HttpPost]
        [RequirePermission(ApplicationPermissions.Niveles.VER)]
        public async Task<IActionResult> VerificarNombreDuplicado(string nombreNivel, int? idNivel = null)
        {
            try
            {
                if (string.IsNullOrEmpty(nombreNivel))
                {
                    return Json(new { existe = false });
                }

                bool existe;
                if (idNivel.HasValue) // Es edición
                {
                    existe = await _context.NivelesCalificacion
                        .AnyAsync(n => n.NombreNivel.ToLower() == nombreNivel.ToLower() && n.IdNivel != idNivel.Value);
                }
                else // Es creación
                {
                    existe = await _context.NivelesCalificacion
                        .AnyAsync(n => n.NombreNivel.ToLower() == nombreNivel.ToLower());
                }

                return Json(new {
                    existe = existe,
                    mensaje = existe ? $"Ya existe un nivel de calificación con el nombre '{nombreNivel}'." : "Nombre disponible."
                });
            }
            catch (Exception ex)
            {
                return Json(new {
                    existe = false,
                    error = ex.Message
                });
            }
        }
    }
}