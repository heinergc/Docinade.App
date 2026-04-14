using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DocinadeApp.Data;
using DocinadeApp.Models;
using DocinadeApp.Models.Permissions;
using DocinadeApp.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace DocinadeApp.Controllers
{
    [Authorize]
    public class ItemsEvaluacionController : Controller
    {
        private readonly RubricasDbContext _context;

        public ItemsEvaluacionController(RubricasDbContext context)
        {
            _context = context;
        }

        // GET: ItemsEvaluacion
        [RequirePermission(ApplicationPermissions.ItemsEvaluacion.VER)]
        public async Task<IActionResult> Index(int? rubricaId)
        {
            // Obtener todas las rúbricas para el filtro
            var rubricas = await _context.Rubricas
                .Where(r => r.Estado == "ACTIVO")
                .OrderBy(r => r.NombreRubrica)
                .ToListAsync();
            
            ViewBag.Rubricas = new SelectList(rubricas, "IdRubrica", "NombreRubrica", rubricaId);
            ViewBag.RubricaSeleccionada = rubricaId;

            // Filtrar items por rúbrica si se especifica
            var itemsQuery = _context.ItemsEvaluacion
                .Include(i => i.Rubrica)
                .AsQueryable();

            if (rubricaId.HasValue)
            {
                itemsQuery = itemsQuery.Where(i => i.IdRubrica == rubricaId.Value);
            }

            var items = await itemsQuery
                .OrderBy(i => i.Rubrica.NombreRubrica)
                .ThenBy(i => i.OrdenItem ?? int.MaxValue)
                .ThenBy(i => i.NombreItem)
                .ToListAsync();

            return View(items);
        }

        // GET: ItemsEvaluacion/Details/5
        [RequirePermission(ApplicationPermissions.ItemsEvaluacion.VER)]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await _context.ItemsEvaluacion
                .Include(i => i.Rubrica)
                .FirstOrDefaultAsync(m => m.IdItem == id);

            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        // GET: ItemsEvaluacion/Create
        [RequirePermission(ApplicationPermissions.ItemsEvaluacion.CREAR)]
        public async Task<IActionResult> Create()
        {
            var rubricasActivas = await _context.Rubricas
                .Where(r => r.Estado == "ACTIVO")
                .OrderBy(r => r.NombreRubrica)
                .ToListAsync();

            ViewData["IdRubrica"] = new SelectList(rubricasActivas, "IdRubrica", "NombreRubrica");

            if (!rubricasActivas.Any())
            {
                TempData["WarningMessage"] = "No hay rúbricas activas disponibles. Debe crear una rúbrica antes de agregar items de evaluación.";
            }

            return View();
        }

        // POST: ItemsEvaluacion/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequirePermission(ApplicationPermissions.ItemsEvaluacion.CREAR)]
        public async Task<IActionResult> Create([Bind("IdRubrica,NombreItem,Descripcion,OrdenItem")] ItemEvaluacion item)
        {
            // Eliminar error de validación de la propiedad de navegación Rubrica
            ModelState.Remove("Rubrica");

            // Manejar OrdenItem vacío
            if (item.OrdenItem == null || item.OrdenItem == 0)
            {
                item.OrdenItem = null; // Permitir que sea null para usar el valor por defecto
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(item);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Item de evaluación creado exitosamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Error al crear el item: {ex.Message}";
                }
            }

            // Recargar la lista de rúbricas para la vista
            var rubricasActivas = await _context.Rubricas
                .Where(r => r.Estado == "ACTIVO")
                .OrderBy(r => r.NombreRubrica)
                .ToListAsync();
            ViewData["IdRubrica"] = new SelectList(rubricasActivas, "IdRubrica", "NombreRubrica", item.IdRubrica);
            return View(item);
        }

        // GET: ItemsEvaluacion/Edit/5
        [RequirePermission(ApplicationPermissions.ItemsEvaluacion.EDITAR)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await _context.ItemsEvaluacion.FindAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            var rubricasActivas = await _context.Rubricas
                .Where(r => r.Estado == "ACTIVO")
                .OrderBy(r => r.NombreRubrica)
                .ToListAsync();

            ViewData["IdRubrica"] = new SelectList(rubricasActivas, "IdRubrica", "NombreRubrica", item.IdRubrica);
            return View(item);
        }

        // POST: ItemsEvaluacion/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequirePermission(ApplicationPermissions.ItemsEvaluacion.EDITAR)]
        public async Task<IActionResult> Edit(int id, [Bind("IdItem,IdRubrica,NombreItem,Descripcion,OrdenItem")] ItemEvaluacion item)
        {
            if (id != item.IdItem)
            {
                return NotFound();
            }

            // Eliminar error de validación de la propiedad de navegación Rubrica
            ModelState.Remove("Rubrica");

            // Manejar OrdenItem vacío
            if (item.OrdenItem == null || item.OrdenItem == 0)
            {
                item.OrdenItem = null; // Permitir que sea null para usar el valor por defecto
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(item);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Item de evaluación actualizado exitosamente.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ItemExists(item.IdItem))
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
            ViewData["IdRubrica"] = new SelectList(_context.Rubricas.Where(r => r.Estado == "ACTIVO"), "IdRubrica", "NombreRubrica", item.IdRubrica);
            return View(item);
        }

        // GET: ItemsEvaluacion/Delete/5
        [RequirePermission(ApplicationPermissions.ItemsEvaluacion.ELIMINAR)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }



            var item = await _context.ItemsEvaluacion
            .Include(i => i.Rubrica)
            .FirstOrDefaultAsync(m => m.IdItem == id);

            //var evaluacionesAsociadas = await _context.DetallesEvaluacion
            //    .Where(d => d.IdItem == id)
            //    .Include(d => d.Evaluacion)
            //    .Select(d => d.Evaluacion)
            //    .Distinct()
            //    .ToListAsync();

            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        // POST: ItemsEvaluacion/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [RequirePermission(ApplicationPermissions.ItemsEvaluacion.ELIMINAR)]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var item = await _context.ItemsEvaluacion.FindAsync(id);
            if (item != null)
            {
                try
                {


                    // Eliminar valores de rúbrica asociados al item
                    var valoresAsociados = await _context.ValoresRubrica
                        .Where(v => v.IdItem == id)
                        .ToListAsync();

                    if (valoresAsociados.Count != 0)
                    {
                        _context.ValoresRubrica.RemoveRange(valoresAsociados);
                        await _context.SaveChangesAsync();
                    }
                    _context.ItemsEvaluacion.Remove(item);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Item de evaluación eliminado exitosamente.";
                }
                catch (DbUpdateException)
                {
                    TempData["ErrorMessage"] = "No se puede eliminar el item porque está siendo usado en evaluaciones.";
                    return RedirectToAction(nameof(Index));
                }
            }

            return RedirectToAction(nameof(Index));
        }

        // Método AJAX para obtener items por rúbrica
        [HttpGet]
        [RequirePermission(ApplicationPermissions.ItemsEvaluacion.VER)]
        public async Task<IActionResult> GetItemsByRubrica(int rubricaId)
        {
            var items = await _context.ItemsEvaluacion
                .Where(i => i.IdRubrica == rubricaId)
                .OrderBy(i => i.OrdenItem ?? int.MaxValue)
                .ThenBy(i => i.NombreItem)
                .Select(i => new { value = i.IdItem, text = i.NombreItem })
                .ToListAsync();

            return Json(items);
        }

        private bool ItemExists(int id)
        {
            return _context.ItemsEvaluacion.Any(e => e.IdItem == id);
        }
    }
}