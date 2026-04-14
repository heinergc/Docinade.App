using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DocinadeApp.Data;
using DocinadeApp.Models;
using DocinadeApp.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using DocinadeApp.Authorization;
using DocinadeApp.Models.Permissions;
using Microsoft.AspNetCore.Authorization;

namespace DocinadeApp.Controllers
{
    [Authorize]
    public class InstrumentoRubricaController : Controller
    {
        private readonly RubricasDbContext _context;
        private readonly ILogger<InstrumentoRubricaController> _logger;

        public InstrumentoRubricaController(RubricasDbContext context, ILogger<InstrumentoRubricaController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: InstrumentoRubrica
        [RequirePermission(ApplicationPermissions.InstrumentoRubrica.VER)]
        public async Task<IActionResult> Index(string? materiaFilter)
        {
            var baseQuery = _context.InstrumentoRubricas
                .Include(ir => ir.InstrumentoEvaluacion)
                    .ThenInclude(ie => ie.InstrumentoMaterias)
                    .ThenInclude(im => im.Materia)
                .Include(ir => ir.Rubrica);

            IQueryable<InstrumentoRubrica> query = baseQuery;

            // Aplicar filtro por materia si se proporciona
            if (!string.IsNullOrEmpty(materiaFilter))
            {
                query = baseQuery.Where(ir => ir.InstrumentoEvaluacion.InstrumentoMaterias
                    .Any(im => im.Materia.Nombre.Contains(materiaFilter) || 
                              im.Materia.Codigo.Contains(materiaFilter)));
            }

            var asignaciones = await query
                .OrderBy(ir => ir.InstrumentoEvaluacion.Nombre)
                .ThenBy(ir => ir.Rubrica.NombreRubrica)
                .ToListAsync();

            // Obtener todas las materias que tienen instrumentos asociados para el filtro
            ViewBag.MateriasDisponibles = await _context.InstrumentoMaterias
                .Include(im => im.Materia)
                .Select(im => im.Materia.Nombre)
                .Distinct()
                .OrderBy(m => m)
                .ToListAsync();

            ViewBag.FiltroActual = materiaFilter;

            return View(asignaciones);
        }

        // GET: InstrumentoRubrica/Details/instrumentoId/rubricaId
        public async Task<IActionResult> Details(int? instrumentoId, int? rubricaId)
        {
            if (instrumentoId == null || rubricaId == null)
            {
                return NotFound();
            }

            var asignacion = await _context.InstrumentoRubricas
                .Include(ir => ir.InstrumentoEvaluacion)
                    .ThenInclude(ie => ie.InstrumentoMaterias)
                    .ThenInclude(im => im.Materia)
                .Include(ir => ir.Rubrica)
                .FirstOrDefaultAsync(ir => ir.InstrumentoEvaluacionId == instrumentoId && ir.RubricaId == rubricaId);

            if (asignacion == null)
            {
                return NotFound();
            }

            // Cargar materias disponibles para el botón de asignar
            var materiasDisponibles = await _context.Materias
                .Where(m => m.Activa)
                .OrderBy(m => m.Codigo)
                .Select(m => new SelectListItem
                {
                    Value = m.MateriaId.ToString(),
                    Text = $"{m.Codigo} - {m.Nombre}"
                })
                .ToListAsync();

            ViewBag.MateriasDisponibles = materiasDisponibles;

            return View(asignacion);
        }

        // GET: InstrumentoRubrica/Create
        public async Task<IActionResult> Create()
        {
            var viewModel = new InstrumentoRubricaViewModel();
            await LoadViewBagDataWithMaterias(viewModel);
            return View(viewModel);
        }

        // POST: InstrumentoRubrica/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(InstrumentoRubricaViewModel viewModel)
        {
            // Verificar si ya existe la asignación
            var existeAsignacion = await _context.InstrumentoRubricas
                .AnyAsync(ir => ir.InstrumentoEvaluacionId == viewModel.InstrumentoId && 
                               ir.RubricaId == viewModel.RubricaId);

            if (existeAsignacion)
            {
                ModelState.AddModelError("", "Esta rúbrica ya está asignada al instrumento seleccionado.");
            }

            // Debug: Mostrar errores de validación
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ModelState is invalid. Errors:");
                foreach (var error in ModelState)
                {
                    _logger.LogWarning($"Campo: {error.Key}");
                    foreach (var errorMessage in error.Value.Errors)
                    {
                        _logger.LogWarning($"  Error: {errorMessage.ErrorMessage}");
                    }
                }
            }
            else
            {
                _logger.LogInformation("ModelState is valid. Proceeding to save...");
            }

            if (ModelState.IsValid)
            {
                var instrumentoRubrica = new InstrumentoRubrica
                {
                    InstrumentoEvaluacionId = viewModel.InstrumentoId,
                    RubricaId = viewModel.RubricaId,
                    Ponderacion = viewModel.Ponderacion,
                    FechaAsignacion = DateTime.UtcNow
                };
                
                _context.Add(instrumentoRubrica);

                // Si se seleccionó una materia, asegurar que existe la relación InstrumentoMateria
                if (viewModel.MateriaId.HasValue)
                {
                    var existeInstrumentoMateria = await _context.InstrumentoMaterias
                        .AnyAsync(im => im.InstrumentoEvaluacionId == viewModel.InstrumentoId && 
                                       im.MateriaId == viewModel.MateriaId.Value);

                    if (!existeInstrumentoMateria)
                    {
                        // Crear la relación InstrumentoMateria si no existe
                        var instrumentoMateria = new InstrumentoMateria
                        {
                            InstrumentoEvaluacionId = viewModel.InstrumentoId,
                            MateriaId = viewModel.MateriaId.Value,
                            PeriodoAcademicoId = 1, // Se puede obtener del contexto del usuario o configuración
                            FechaAsignacion = DateTime.UtcNow,
                            EsObligatorio = false
                        };
                        _context.Add(instrumentoMateria);
                    }
                }

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Asignación de rúbrica creada exitosamente.";
                return RedirectToAction(nameof(Index));
            }

            await LoadViewBagDataWithMaterias(viewModel);
            return View(viewModel);
        }

        // GET: InstrumentoRubrica/Edit/instrumentoId/rubricaId
        public async Task<IActionResult> Edit(int? instrumentoId, int? rubricaId)
        {
            if (instrumentoId == null || rubricaId == null)
            {
                return NotFound();
            }

            var asignacion = await _context.InstrumentoRubricas
                .FirstOrDefaultAsync(ir => ir.InstrumentoEvaluacionId == instrumentoId && ir.RubricaId == rubricaId);

            if (asignacion == null)
            {
                return NotFound();
            }

            var viewModel = new InstrumentoRubricaViewModel
            {
                InstrumentoId = asignacion.InstrumentoEvaluacionId,
                RubricaId = asignacion.RubricaId,
                Ponderacion = asignacion.Ponderacion
            };

            await LoadViewBagData();
            return View(viewModel);
        }

        // POST: InstrumentoRubrica/Edit/instrumentoId/rubricaId
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int instrumentoId, int rubricaId, InstrumentoRubricaViewModel viewModel)
        {
            if (instrumentoId != viewModel.InstrumentoId || rubricaId != viewModel.RubricaId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var asignacion = await _context.InstrumentoRubricas
                        .FirstOrDefaultAsync(ir => ir.InstrumentoEvaluacionId == instrumentoId && ir.RubricaId == rubricaId);
                    
                    if (asignacion == null)
                    {
                        return NotFound();
                    }
                    
                    asignacion.Ponderacion = viewModel.Ponderacion;
                    
                    _context.Update(asignacion);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Asignaci�n de r�brica actualizada exitosamente.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InstrumentoRubricaExists(viewModel.InstrumentoId, viewModel.RubricaId))
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

            await LoadViewBagData();
            return View(viewModel);
        }

        // GET: InstrumentoRubrica/Delete/instrumentoId/rubricaId
        public async Task<IActionResult> Delete(int? instrumentoId, int? rubricaId)
        {
            if (instrumentoId == null || rubricaId == null)
            {
                return NotFound();
            }

            var asignacion = await _context.InstrumentoRubricas
                .Include(ir => ir.InstrumentoEvaluacion)
                .Include(ir => ir.Rubrica)
                .FirstOrDefaultAsync(ir => ir.InstrumentoEvaluacionId == instrumentoId && ir.RubricaId == rubricaId);

            if (asignacion == null)
            {
                return NotFound();
            }

            return View(asignacion);
        }

        // POST: InstrumentoRubrica/Delete/instrumentoId/rubricaId
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int instrumentoId, int rubricaId)
        {
            var asignacion = await _context.InstrumentoRubricas
                .FirstOrDefaultAsync(ir => ir.InstrumentoEvaluacionId == instrumentoId && ir.RubricaId == rubricaId);

            if (asignacion != null)
            {
                try
                {
                    _context.InstrumentoRubricas.Remove(asignacion);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Asignaci�n eliminada exitosamente.";
                }
                catch (DbUpdateException)
                {
                    TempData["ErrorMessage"] = "No se puede eliminar la asignaci�n porque tiene datos relacionados.";
                    return RedirectToAction(nameof(Index));
                }
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: InstrumentoRubrica/ByInstrumento/id
        public async Task<IActionResult> ByInstrumento(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instrumento = await _context.InstrumentosEvaluacion
                .FirstOrDefaultAsync(i => i.InstrumentoId == id);

            if (instrumento == null)
            {
                return NotFound();
            }

            var asignaciones = await _context.InstrumentoRubricas
                .Include(ir => ir.Rubrica)
                .Where(ir => ir.InstrumentoEvaluacionId == id)
                .OrderBy(ir => ir.Rubrica.NombreRubrica)
                .ToListAsync();

            ViewBag.InstrumentoNombre = instrumento.Nombre;
            ViewBag.InstrumentoId = id;

            return View(asignaciones);
        }

        // GET: InstrumentoRubrica/ByRubrica/id
        public async Task<IActionResult> ByRubrica(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rubrica = await _context.Rubricas
                .FirstOrDefaultAsync(r => r.IdRubrica == id);

            if (rubrica == null)
            {
                return NotFound();
            }

            var asignaciones = await _context.InstrumentoRubricas
                .Include(ir => ir.InstrumentoEvaluacion)
                .Where(ir => ir.RubricaId == id)
                .OrderBy(ir => ir.InstrumentoEvaluacion.Nombre)
                .ToListAsync();

            ViewBag.RubricaNombre = rubrica.NombreRubrica;
            ViewBag.RubricaId = id;

            return View(asignaciones);
        }

        // GET: Api/InstrumentoRubrica/GetMateriasInstrumento/instrumentoId
        [HttpGet]
        public async Task<JsonResult> GetMateriasInstrumento(int instrumentoId)
        {
            try
            {
                var materias = await _context.InstrumentoMaterias
                    .Where(im => im.InstrumentoEvaluacionId == instrumentoId)
                    .Include(im => im.Materia)
                    .Select(im => new { 
                        value = im.MateriaId, 
                        text = $"{im.Materia.Codigo} - {im.Materia.Nombre}" 
                    })
                    .OrderBy(m => m.text)
                    .ToListAsync();

                return Json(materias, new System.Text.Json.JsonSerializerOptions
                {
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                    PropertyNamingPolicy = null
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener materias para el instrumento {InstrumentoId}", instrumentoId);
                
                return Json(new List<object>(), new System.Text.Json.JsonSerializerOptions
                {
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                    PropertyNamingPolicy = null
                });
            }
        }

        // GET: Api/InstrumentoRubrica/GetRubricasDisponibles/instrumentoId
        [HttpGet]
        public async Task<JsonResult> GetRubricasDisponibles(int instrumentoId)
        {
            try
            {
                var rubricasAsignadas = await _context.InstrumentoRubricas
                    .Where(ir => ir.InstrumentoEvaluacionId == instrumentoId)
                    .Select(ir => ir.RubricaId)
                    .ToListAsync();

                var rubricasDisponibles = await _context.Rubricas
                    .Where(r => r.Estado == "ACTIVO" && !rubricasAsignadas.Contains(r.IdRubrica))
                    .Select(r => new { value = r.IdRubrica, text = r.NombreRubrica })
                    .OrderBy(r => r.text)
                    .ToListAsync();

                return Json(rubricasDisponibles, new System.Text.Json.JsonSerializerOptions
                {
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                    PropertyNamingPolicy = null
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener r�bricas disponibles para el instrumento {InstrumentoId}", instrumentoId);
                
                // Return empty array in case of error
                return Json(new List<object>(), new System.Text.Json.JsonSerializerOptions
                {
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                    PropertyNamingPolicy = null
                });
            }
        }

        // GET: Api/InstrumentoRubrica/GetInstrumentosDisponibles/rubricaId
        [HttpGet]
        public async Task<JsonResult> GetInstrumentosDisponibles(int rubricaId)
        {
            try
            {
                var instrumentosAsignados = await _context.InstrumentoRubricas
                    .Where(ir => ir.RubricaId == rubricaId)
                    .Select(ir => ir.InstrumentoEvaluacionId)
                    .ToListAsync();

                var instrumentosDisponibles = await _context.InstrumentosEvaluacion
                    .Where(i => i.Activo && !instrumentosAsignados.Contains(i.InstrumentoId))
                    .Select(i => new { value = i.InstrumentoId, text = i.Nombre })
                    .OrderBy(i => i.text)
                    .ToListAsync();

                return Json(instrumentosDisponibles, new System.Text.Json.JsonSerializerOptions
                {
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                    PropertyNamingPolicy = null
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener instrumentos disponibles para la r�brica {RubricaId}", rubricaId);
                
                // Return empty array in case of error
                return Json(new List<object>(), new System.Text.Json.JsonSerializerOptions
                {
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                    PropertyNamingPolicy = null
                });
            }
        }

        private bool InstrumentoRubricaExists(int instrumentoId, int rubricaId)
        {
            return _context.InstrumentoRubricas.Any(e => e.InstrumentoEvaluacionId == instrumentoId && e.RubricaId == rubricaId);
        }

        private async Task LoadViewBagData()
        {
            ViewBag.InstrumentoId = new SelectList(
                await _context.InstrumentosEvaluacion
                    .Where(i => i.Activo)
                    .OrderBy(i => i.Nombre)
                    .ToListAsync(),
                "InstrumentoId", "Nombre"
            );

            ViewBag.RubricaId = new SelectList(
                await _context.Rubricas
                    .Where(r => r.Estado == "ACTIVO")
                    .OrderBy(r => r.NombreRubrica)
                    .ToListAsync(),
                "IdRubrica", "NombreRubrica"
            );
        }

        private async Task LoadViewBagDataWithMaterias(InstrumentoRubricaViewModel viewModel)
        {
            // Cargar instrumentos activos
            ViewBag.InstrumentoId = new SelectList(
                await _context.InstrumentosEvaluacion
                    .Where(i => i.Activo)
                    .OrderBy(i => i.Nombre)
                    .ToListAsync(),
                "InstrumentoId", "Nombre"
            );

            // Cargar rúbricas activas
            ViewBag.RubricaId = new SelectList(
                await _context.Rubricas
                    .Where(r => r.Estado == "ACTIVO")
                    .OrderBy(r => r.NombreRubrica)
                    .ToListAsync(),
                "IdRubrica", "NombreRubrica"
            );

            // Cargar materias que ya están asignadas a instrumentos de rúbrica
            var materiasAsignadas = await _context.InstrumentoMaterias
                .Include(im => im.Materia)
                .Select(im => new { 
                    im.Materia.MateriaId, 
                    im.Materia.Nombre,
                    im.Materia.Codigo 
                })
                .Distinct()
                .OrderBy(m => m.Nombre)
                .ToListAsync();

            viewModel.MateriasDisponibles = materiasAsignadas
                .Select(m => new SelectListItem 
                { 
                    Value = m.MateriaId.ToString(), 
                    Text = $"{m.Codigo} - {m.Nombre}" 
                })
                .ToList();
        }

        // POST: InstrumentoRubrica/AsignarMateria
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AsignarMateria(int instrumentoId, int materiaId, int rubricaId)
        {
            try
            {
                // Verificar que el instrumento existe
                var instrumento = await _context.InstrumentosEvaluacion
                    .FindAsync(instrumentoId);

                if (instrumento == null)
                {
                    TempData["Error"] = "El instrumento de evaluación no existe.";
                    return RedirectToAction(nameof(Details), new { instrumentoId, rubricaId });
                }

                // Verificar que la materia existe
                var materia = await _context.Materias
                    .FindAsync(materiaId);

                if (materia == null)
                {
                    TempData["Error"] = "La materia seleccionada no existe.";
                    return RedirectToAction(nameof(Details), new { instrumentoId, rubricaId });
                }

                // Verificar si ya existe una asignación para este instrumento y materia
                var existeAsignacion = await _context.InstrumentoMaterias
                    .AnyAsync(im => im.InstrumentoEvaluacionId == instrumentoId && im.MateriaId == materiaId);

                if (existeAsignacion)
                {
                    TempData["Warning"] = $"El instrumento ya está asignado a la materia {materia.Codigo} - {materia.Nombre}.";
                    return RedirectToAction(nameof(Details), new { instrumentoId, rubricaId });
                }

                // Crear la nueva asignación
                var instrumentoMateria = new InstrumentoMateria
                {
                    InstrumentoEvaluacionId = instrumentoId,
                    MateriaId = materiaId,
                    FechaAsignacion = DateTime.Now
                };

                _context.InstrumentoMaterias.Add(instrumentoMateria);
                await _context.SaveChangesAsync();

                TempData["Success"] = $"Materia {materia.Codigo} - {materia.Nombre} asignada exitosamente al instrumento.";
                return RedirectToAction(nameof(Details), new { instrumentoId, rubricaId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al asignar materia {MateriaId} al instrumento {InstrumentoId}", 
                    materiaId, instrumentoId);
                TempData["Error"] = "Ocurrió un error al asignar la materia. Por favor, inténtelo nuevamente.";
                return RedirectToAction(nameof(Details), new { instrumentoId, rubricaId });
            }
        }
    }
}