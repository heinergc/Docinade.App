using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DocinadeApp.Data;
using DocinadeApp.Models;
using DocinadeApp.Authorization;
using DocinadeApp.Models.Permissions;
using Microsoft.AspNetCore.Authorization;

namespace DocinadeApp.Controllers
{
    [Authorize]
    public class InstrumentosEvaluacionController : Controller
    {
        private readonly RubricasDbContext _context;
        private readonly ILogger<InstrumentosEvaluacionController> _logger;

        public InstrumentosEvaluacionController(RubricasDbContext context, ILogger<InstrumentosEvaluacionController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: InstrumentosEvaluacion
        [RequirePermission(ApplicationPermissions.InstrumentosEvaluacion.VER)]
        public async Task<IActionResult> Index(string busqueda)
        {
            var instrumentos = _context.InstrumentosEvaluacion.AsQueryable();

            // Aplicar filtro de búsqueda si se proporciona
            if (!string.IsNullOrWhiteSpace(busqueda))
            {
                var busquedaLower = busqueda.ToLower();
                instrumentos = instrumentos.Where(i => 
                    i.Nombre.ToLower().Contains(busquedaLower) ||
                    (i.Descripcion != null && i.Descripcion.ToLower().Contains(busquedaLower))
                );
            }

            var resultado = await instrumentos
                .OrderBy(i => i.Nombre)
                .ToListAsync();

            ViewBag.BusquedaActual = busqueda;
            return View(resultado);
        }

        // GET: InstrumentosEvaluacion/Details/5
        [RequirePermission(ApplicationPermissions.InstrumentosEvaluacion.VER)]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instrumento = await _context.InstrumentosEvaluacion
                .Include(i => i.InstrumentoRubricas)
                .Include(i => i.InstrumentoMaterias)
                .FirstOrDefaultAsync(m => m.InstrumentoId == id);

            if (instrumento == null)
            {
                return NotFound();
            }

            return View(instrumento);
        }

        // GET: InstrumentosEvaluacion/Create
        [RequirePermission(ApplicationPermissions.InstrumentosEvaluacion.CREAR)]
        public IActionResult Create()
        {
            return View();
        }

        // POST: InstrumentosEvaluacion/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequirePermission(ApplicationPermissions.InstrumentosEvaluacion.CREAR)]
        public async Task<IActionResult> Create([Bind("Nombre,Descripcion,Activo")] InstrumentoEvaluacion instrumento)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // 🔧 CORRECCIÓN: Asegurar que el esquema esté correcto antes de guardar
                    await InstrumentoEvaluacionFixer.EnsureSchemaIsFixed(_context);
                    
                    // 🔧 CORRECCIÓN: Asegurar valores por defecto
                    instrumento.FechaCreacion = DateTime.UtcNow;
                    
                    // 🔧 CORRECCIÓN: Asegurar que Activo no sea null
                    if (!instrumento.Activo && !Request.Form.ContainsKey("Activo"))
                    {
                        instrumento.Activo = true; // Valor por defecto si no se especifica
                    }
                    
                    _logger.LogInformation("Creando InstrumentoEvaluacion: Nombre={Nombre}, Activo={Activo}", 
                                         instrumento.Nombre, instrumento.Activo);
                    
                    _context.Add(instrumento);
                    await _context.SaveChangesAsync();
                    
                    TempData["SuccessMessage"] = "Instrumento de evaluación creado exitosamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al crear instrumento de evaluación: {Error}", ex.Message);
                    ModelState.AddModelError("", $"Error al crear el instrumento: {ex.Message}");
                }
            }
            else
            {
                _logger.LogWarning("ModelState inválido para InstrumentoEvaluacion. Errores: {Errors}", 
                                 string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
            }
            
            return View(instrumento);
        }

        // GET: InstrumentosEvaluacion/Edit/5
        [RequirePermission(ApplicationPermissions.InstrumentosEvaluacion.EDITAR)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instrumento = await _context.InstrumentosEvaluacion.FindAsync(id);
            if (instrumento == null)
            {
                return NotFound();
            }
            return View(instrumento);
        }

        // POST: InstrumentosEvaluacion/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequirePermission(ApplicationPermissions.InstrumentosEvaluacion.EDITAR)]
        public async Task<IActionResult> Edit(int id, [Bind("InstrumentoId,Nombre,Descripcion,Activo,FechaCreacion")] InstrumentoEvaluacion instrumento)
        {
            if (id != instrumento.InstrumentoId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // 🔧 CORRECCIÓN: Asegurar que el esquema esté correcto antes de actualizar
                    await InstrumentoEvaluacionFixer.EnsureSchemaIsFixed(_context);
                    
                    _context.Update(instrumento);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Instrumento de evaluación actualizado exitosamente.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InstrumentoEvaluacionExists(instrumento.InstrumentoId))
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
                    _logger.LogError(ex, "Error al actualizar instrumento de evaluación: {Error}", ex.Message);
                    ModelState.AddModelError("", $"Error al actualizar el instrumento: {ex.Message}");
                }
                return RedirectToAction(nameof(Index));
            }
            return View(instrumento);
        }

        // GET: InstrumentosEvaluacion/Delete/5
        [RequirePermission(ApplicationPermissions.InstrumentosEvaluacion.ELIMINAR)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instrumento = await _context.InstrumentosEvaluacion
                .FirstOrDefaultAsync(m => m.InstrumentoId == id);
            if (instrumento == null)
            {
                return NotFound();
            }

            return View(instrumento);
        }

        // POST: InstrumentosEvaluacion/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [RequirePermission(ApplicationPermissions.InstrumentosEvaluacion.ELIMINAR)]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var instrumento = await _context.InstrumentosEvaluacion.FindAsync(id);
            if (instrumento != null)
            {
                try
                {
                    _context.InstrumentosEvaluacion.Remove(instrumento);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Instrumento de evaluación eliminado exitosamente.";
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError(ex, "Error al eliminar instrumento de evaluación con ID {Id}", id);
                    TempData["ErrorMessage"] = "No se puede eliminar el instrumento porque tiene datos relacionados.";
                    return RedirectToAction(nameof(Index));
                }
            }

            return RedirectToAction(nameof(Index));
        }

        private bool InstrumentoEvaluacionExists(int id)
        {
            return _context.InstrumentosEvaluacion.Any(e => e.InstrumentoId == id);
        }
    }
}