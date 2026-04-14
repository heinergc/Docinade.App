using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DocinadeApp.Data;
using DocinadeApp.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DocinadeApp.Controllers
{
    public class InstrumentosController : Controller
    {
        private readonly RubricasDbContext _context;
        private readonly ILogger<InstrumentosController> _logger;

        public InstrumentosController(RubricasDbContext context, ILogger<InstrumentosController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Instrumentos
        public async Task<IActionResult> Index()
        {
            try
            {
                var instrumentos = await _context.InstrumentosEvaluacion
                    .Include(i => i.InstrumentoRubricas)
                        .ThenInclude(ir => ir.Rubrica)
                    .Include(i => i.InstrumentoMaterias)
                        .ThenInclude(im => im.Materia)
                    .ToListAsync();
                
                return View(instrumentos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los instrumentos de evaluaci�n");
                TempData["ErrorMessage"] = "Error al cargar los instrumentos de evaluaci�n";
                return View(new List<InstrumentoEvaluacion>());
            }
        }

        // GET: Instrumentos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instrumento = await _context.InstrumentosEvaluacion
                .Include(i => i.InstrumentoRubricas)
                    .ThenInclude(ir => ir.Rubrica)
                .Include(i => i.InstrumentoMaterias)
                    .ThenInclude(im => im.Materia)
                .FirstOrDefaultAsync(m => m.InstrumentoId == id);

            if (instrumento == null)
            {
                return NotFound();
            }

            ViewBag.RubricasDisponibles = await GetRubricasDisponiblesAsync(id.Value);
            ViewBag.MateriasDisponibles = await GetMateriasDisponiblesAsync(id.Value);

            return View(instrumento);
        }

        // GET: Instrumentos/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Instrumentos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nombre,Descripcion,Activo")] InstrumentoEvaluacion instrumento)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    instrumento.FechaCreacion = DateTime.UtcNow;
                    _context.Add(instrumento);
                    await _context.SaveChangesAsync();
                    
                    TempData["SuccessMessage"] = "Instrumento de evaluaci�n creado exitosamente";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al crear el instrumento de evaluaci�n");
                    ModelState.AddModelError("", "Error al crear el instrumento de evaluaci�n");
                }
            }
            return View(instrumento);
        }

        // GET: Instrumentos/Edit/5
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

        // POST: Instrumentos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
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
                    _context.Update(instrumento);
                    await _context.SaveChangesAsync();
                    
                    TempData["SuccessMessage"] = "Instrumento de evaluaci�n actualizado exitosamente";
                    return RedirectToAction(nameof(Index));
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
                    _logger.LogError(ex, "Error al actualizar el instrumento de evaluaci�n");
                    ModelState.AddModelError("", "Error al actualizar el instrumento de evaluaci�n");
                }
            }
            return View(instrumento);
        }

        // GET: Instrumentos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instrumento = await _context.InstrumentosEvaluacion
                .Include(i => i.InstrumentoRubricas)
                    .ThenInclude(ir => ir.Rubrica)
                .Include(i => i.InstrumentoMaterias)
                    .ThenInclude(im => im.Materia)
                .FirstOrDefaultAsync(m => m.InstrumentoId == id);

            if (instrumento == null)
            {
                return NotFound();
            }

            return View(instrumento);
        }

        // POST: Instrumentos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var instrumento = await _context.InstrumentosEvaluacion.FindAsync(id);
                if (instrumento != null)
                {
                    _context.InstrumentosEvaluacion.Remove(instrumento);
                    await _context.SaveChangesAsync();
                    
                    TempData["SuccessMessage"] = "Instrumento de evaluaci�n eliminado exitosamente";
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el instrumento de evaluaci�n");
                TempData["ErrorMessage"] = "Error al eliminar el instrumento de evaluaci�n";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Instrumentos/AgregarRubrica
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AgregarRubrica(int instrumentoId, int rubricaId, decimal ponderacion)
        {
            try
            {
                // 🔧 CORRECCIÓN: Usar InstrumentoEvaluacionId en lugar de InstrumentoId
                var existe = await _context.InstrumentoRubricas
                    .AnyAsync(ir => ir.InstrumentoEvaluacionId == instrumentoId && ir.RubricaId == rubricaId);

                if (existe)
                {
                    TempData["ErrorMessage"] = "La rúbrica ya está asociada a este instrumento";
                    return RedirectToAction(nameof(Details), new { id = instrumentoId });
                }

                var instrumentoRubrica = new InstrumentoRubrica
                {
                    InstrumentoEvaluacionId = instrumentoId, // 🔧 Usar la propiedad mapeada
                    RubricaId = rubricaId,
                    Ponderacion = ponderacion,
                    FechaAsignacion = DateTime.UtcNow
                };

                _context.InstrumentoRubricas.Add(instrumentoRubrica);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Rúbrica asociada exitosamente";
                return RedirectToAction(nameof(Details), new { id = instrumentoId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al asociar la rúbrica al instrumento");
                TempData["ErrorMessage"] = "Error al asociar la rúbrica";
                return RedirectToAction(nameof(Details), new { id = instrumentoId });
            }
        }

        // POST: Instrumentos/QuitarRubrica
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> QuitarRubrica(int instrumentoId, int rubricaId)
        {
            try
            {
                // 🔧 CORRECCIÓN: Usar InstrumentoEvaluacionId en lugar de InstrumentoId
                var instrumentoRubrica = await _context.InstrumentoRubricas
                    .FirstOrDefaultAsync(ir => ir.InstrumentoEvaluacionId == instrumentoId && ir.RubricaId == rubricaId);

                if (instrumentoRubrica != null)
                {
                    _context.InstrumentoRubricas.Remove(instrumentoRubrica);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Rúbrica desasociada exitosamente";
                }

                return RedirectToAction(nameof(Details), new { id = instrumentoId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al desasociar la rúbrica del instrumento");
                TempData["ErrorMessage"] = "Error al desasociar la rúbrica";
                return RedirectToAction(nameof(Details), new { id = instrumentoId });
            }
        }

        // POST: Instrumentos/AgregarMateria
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AgregarMateria(int instrumentoId, int materiaId, int periodoAcademicoId)
        {
            try
            {
                // 🔧 CORRECCIÓN: Usar InstrumentoEvaluacionId en lugar de InstrumentoId
                var existe = await _context.InstrumentoMaterias
                    .AnyAsync(im => im.InstrumentoEvaluacionId == instrumentoId && im.MateriaId == materiaId);

                if (existe)
                {
                    TempData["ErrorMessage"] = "La materia ya está asociada a este instrumento";
                    return RedirectToAction(nameof(Details), new { id = instrumentoId });
                }

                var instrumentoMateria = new InstrumentoMateria
                {
                    InstrumentoEvaluacionId = instrumentoId, // 🔧 Usar la propiedad mapeada
                    MateriaId = materiaId,
                    PeriodoAcademicoId = periodoAcademicoId
                };

                _context.InstrumentoMaterias.Add(instrumentoMateria);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Materia asociada exitosamente";
                return RedirectToAction(nameof(Details), new { id = instrumentoId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al asociar la materia al instrumento");
                TempData["ErrorMessage"] = "Error al asociar la materia";
                return RedirectToAction(nameof(Details), new { id = instrumentoId });
            }
        }

        // POST: Instrumentos/QuitarMateria
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> QuitarMateria(int instrumentoId, int materiaId)
        {
            try
            {
                // 🔧 CORRECCIÓN: Usar InstrumentoEvaluacionId en lugar de InstrumentoId
                var instrumentoMateria = await _context.InstrumentoMaterias
                    .FirstOrDefaultAsync(im => im.InstrumentoEvaluacionId == instrumentoId && im.MateriaId == materiaId);

                if (instrumentoMateria != null)
                {
                    _context.InstrumentoMaterias.Remove(instrumentoMateria);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Materia desasociada exitosamente";
                }

                return RedirectToAction(nameof(Details), new { id = instrumentoId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al desasociar la materia del instrumento");
                TempData["ErrorMessage"] = "Error al desasociar la materia";
                return RedirectToAction(nameof(Details), new { id = instrumentoId });
            }
        }

        private bool InstrumentoEvaluacionExists(int id)
        {
            return _context.InstrumentosEvaluacion.Any(e => e.InstrumentoId == id);
        }

        private async Task<SelectList> GetRubricasDisponiblesAsync(int instrumentoId)
        {
            // 🔧 CORRECCIÓN: Usar InstrumentoEvaluacionId en lugar de InstrumentoId
            var rubricasAsociadas = await _context.InstrumentoRubricas
                .Where(ir => ir.InstrumentoEvaluacionId == instrumentoId)
                .Select(ir => ir.RubricaId)
                .ToListAsync();

            var rubricasDisponibles = await _context.Rubricas
                .Where(r => !rubricasAsociadas.Contains(r.IdRubrica))
                .Select(r => new { r.IdRubrica, r.NombreRubrica })
                .ToListAsync();

            return new SelectList(rubricasDisponibles, "IdRubrica", "NombreRubrica");
        }

        private async Task<SelectList> GetMateriasDisponiblesAsync(int instrumentoId)
        {
            // 🔧 CORRECCIÓN: Usar InstrumentoEvaluacionId en lugar de InstrumentoId
            var materiasAsociadas = await _context.InstrumentoMaterias
                .Where(im => im.InstrumentoEvaluacionId == instrumentoId)
                .Select(im => im.MateriaId)
                .ToListAsync();

            var materiasDisponibles = await _context.Materias
                .Where(m => !materiasAsociadas.Contains(m.MateriaId))
                .Select(m => new { m.MateriaId, m.Nombre })
                .ToListAsync();

            return new SelectList(materiasDisponibles, "MateriaId", "Nombre");
        }
    }
}
