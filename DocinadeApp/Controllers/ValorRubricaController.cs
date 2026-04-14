using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RubricasApp.Web.Data;
using RubricasApp.Web.Models;
using RubricasApp.Web.Models.Permissions;
using RubricasApp.Web.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace RubricasApp.Web.Controllers
{
    [Authorize]
    public class ValorRubricaController : Controller
    {
        private readonly RubricasDbContext _context;

        public ValorRubricaController(RubricasDbContext context)
        {
            _context = context;
        }

        // GET: ValorRubrica
        [RequirePermission(ApplicationPermissions.Rubricas.VER)]
        public async Task<IActionResult> Index(int? idRubrica, int? idItem)
        {
            var valores = _context.ValoresRubrica
                .Include(v => v.Rubrica)
                .Include(v => v.ItemEvaluacion)
                .Include(v => v.NivelCalificacion)
                .AsQueryable();

            // Filtrar por rúbrica si se especifica
            if (idRubrica.HasValue)
            {
                valores = valores.Where(v => v.IdRubrica == idRubrica.Value);
                ViewBag.RubricaSeleccionada = await _context.Rubricas
                    .FirstOrDefaultAsync(r => r.IdRubrica == idRubrica.Value);
            }

            // Filtrar por item si se especifica
            if (idItem.HasValue)
            {
                valores = valores.Where(v => v.IdItem == idItem.Value);
                ViewBag.ItemSeleccionado = await _context.ItemsEvaluacion
                    .Include(i => i.Rubrica)
                    .FirstOrDefaultAsync(i => i.IdItem == idItem.Value);
            }

            var resultado = await valores
                .OrderBy(v => v.Rubrica.NombreRubrica)
                .ThenBy(v => v.ItemEvaluacion.OrdenItem ?? int.MaxValue)
                .ThenBy(v => v.ItemEvaluacion.NombreItem)
                .ThenBy(v => v.NivelCalificacion.OrdenNivel ?? int.MaxValue)
                .ThenBy(v => v.NivelCalificacion.NombreNivel)
                .ToListAsync();

            // Para los filtros
            ViewBag.Rubricas = new SelectList(
                await _context.Rubricas.Where(r => r.Estado == "ACTIVO").ToListAsync(),
                "IdRubrica", "NombreRubrica", idRubrica);

            // Cargar items - si hay rúbrica seleccionada, filtrar por ella, si no, cargar todos
            if (idRubrica.HasValue)
            {
                ViewBag.Items = new SelectList(
                    await _context.ItemsEvaluacion
                        .Where(i => i.IdRubrica == idRubrica.Value)
                        .OrderBy(i => i.OrdenItem ?? int.MaxValue)
                        .ThenBy(i => i.NombreItem)
                        .ToListAsync(),
                    "IdItem", "NombreItem", idItem);
            }
            else
            {
                ViewBag.Items = new SelectList(
                    await _context.ItemsEvaluacion
                        .Include(i => i.Rubrica)
                        .OrderBy(i => i.Rubrica.NombreRubrica)
                        .ThenBy(i => i.OrdenItem ?? int.MaxValue)
                        .ThenBy(i => i.NombreItem)
                        .Select(i => new { 
                            IdItem = i.IdItem, 
                            NombreCompleto = i.Rubrica.NombreRubrica + " - " + i.NombreItem 
                        })
                        .ToListAsync(),
                    "IdItem", "NombreCompleto", idItem);
            }

            return View(resultado);
        }

        // GET: ValorRubrica/Details/5
        [RequirePermission(ApplicationPermissions.Rubricas.VER)]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var valorRubrica = await _context.ValoresRubrica
                .Include(v => v.Rubrica)
                .Include(v => v.ItemEvaluacion)
                .Include(v => v.NivelCalificacion)
                .FirstOrDefaultAsync(m => m.IdValor == id);

            if (valorRubrica == null)
            {
                return NotFound();
            }

            return View(valorRubrica);
        }

        // GET: ValorRubrica/Create
        [RequirePermission(ApplicationPermissions.Rubricas.EDITAR)]
        public async Task<IActionResult> Create(int? idRubrica, int? idItem)
        {
            await CargarListasSelectAsync(idRubrica, idItem);

            var modelo = new ValorRubrica();
            if (idRubrica.HasValue)
            {
                modelo.IdRubrica = idRubrica.Value;
            }
            if (idItem.HasValue)
            {
                modelo.IdItem = idItem.Value;
            }

            return View(modelo);
        }

        // POST: ValorRubrica/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequirePermission(ApplicationPermissions.Rubricas.EDITAR)]
        public async Task<IActionResult> Create([Bind("IdRubrica,IdItem,IdNivel,ValorPuntos")] ValorRubrica valorRubrica)
        {
            // Remover navigation properties del ModelState
            ModelState.Remove("Rubrica");
            ModelState.Remove("ItemEvaluacion");
            ModelState.Remove("NivelCalificacion");

            // Validar que no exista ya una combinación igual
            var existeValor = await _context.ValoresRubrica
                .AnyAsync(v => v.IdRubrica == valorRubrica.IdRubrica && 
                              v.IdItem == valorRubrica.IdItem && 
                              v.IdNivel == valorRubrica.IdNivel);

            if (existeValor)
            {
                ModelState.AddModelError("", "Ya existe un valor definido para esta combinación de rúbrica, item y nivel.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(valorRubrica);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Valor de rúbrica creado exitosamente.";
                    return RedirectToAction(nameof(Index), new { idRubrica = valorRubrica.IdRubrica, idItem = valorRubrica.IdItem });
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Error al crear el valor: {ex.Message}";
                }
            }

            await CargarListasSelectAsync(valorRubrica.IdRubrica, valorRubrica.IdItem);
            return View(valorRubrica);
        }

        // GET: ValorRubrica/Edit/5
        [RequirePermission(ApplicationPermissions.Rubricas.EDITAR)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var valorRubrica = await _context.ValoresRubrica.FindAsync(id);
            if (valorRubrica == null)
            {
                return NotFound();
            }

            await CargarListasSelectAsync(valorRubrica.IdRubrica, valorRubrica.IdItem);
            return View(valorRubrica);
        }

        // POST: ValorRubrica/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequirePermission(ApplicationPermissions.Rubricas.EDITAR)]
        public async Task<IActionResult> Edit(int id, [Bind("IdValor,IdRubrica,IdItem,IdNivel,ValorPuntos")] ValorRubrica valorRubrica)
        {
            if (id != valorRubrica.IdValor)
            {
                return NotFound();
            }

            // Remover navigation properties del ModelState
            ModelState.Remove("Rubrica");
            ModelState.Remove("ItemEvaluacion");
            ModelState.Remove("NivelCalificacion");

            // Validar que no exista ya una combinación igual (excluyendo el registro actual)
            var existeValor = await _context.ValoresRubrica
                .AnyAsync(v => v.IdRubrica == valorRubrica.IdRubrica && 
                              v.IdItem == valorRubrica.IdItem && 
                              v.IdNivel == valorRubrica.IdNivel &&
                              v.IdValor != valorRubrica.IdValor);

            if (existeValor)
            {
                ModelState.AddModelError("", "Ya existe un valor definido para esta combinación de rúbrica, item y nivel.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(valorRubrica);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Valor de rúbrica actualizado exitosamente.";
                    return RedirectToAction(nameof(Index), new { idRubrica = valorRubrica.IdRubrica, idItem = valorRubrica.IdItem });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ValorRubricaExists(valorRubrica.IdValor))
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
                    TempData["ErrorMessage"] = $"Error al actualizar el valor: {ex.Message}";
                }
            }

            await CargarListasSelectAsync(valorRubrica.IdRubrica, valorRubrica.IdItem);
            return View(valorRubrica);
        }

        // GET: ValorRubrica/Delete/5
        [RequirePermission(ApplicationPermissions.Rubricas.ELIMINAR)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var valorRubrica = await _context.ValoresRubrica
                .Include(v => v.Rubrica)
                .Include(v => v.ItemEvaluacion)
                .Include(v => v.NivelCalificacion)
                .FirstOrDefaultAsync(m => m.IdValor == id);

            if (valorRubrica == null)
            {
                return NotFound();
            }

            return View(valorRubrica);
        }

        // POST: ValorRubrica/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [RequirePermission(ApplicationPermissions.Rubricas.ELIMINAR)]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var valorRubrica = await _context.ValoresRubrica.FindAsync(id);
            if (valorRubrica != null)
            {
                try
                {
                    _context.ValoresRubrica.Remove(valorRubrica);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Valor de rúbrica eliminado exitosamente.";
                }
                catch (DbUpdateException)
                {
                    TempData["ErrorMessage"] = "No se puede eliminar el valor porque está siendo usado en evaluaciones.";
                }
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: ValorRubrica/ConfigurarRubrica/5
        [RequirePermission(ApplicationPermissions.Rubricas.EDITAR)]
        public async Task<IActionResult> ConfigurarRubrica(int id)
        {
            var rubrica = await _context.Rubricas
                .Include(r => r.ItemsEvaluacion)
                .Include(r => r.GrupoCalificacion)
                .ThenInclude(g => g.NivelesCalificacion)
                .FirstOrDefaultAsync(r => r.IdRubrica == id);

            if (rubrica == null)
            {
                return NotFound();
            }

            // Obtener niveles específicos de esta rúbrica usando RubricaNiveles
            var niveles = await _context.RubricaNiveles
                .Where(rn => rn.IdRubrica == id)
                .Include(rn => rn.NivelCalificacion)
                .OrderBy(rn => rn.OrdenEnRubrica)
                .Select(rn => rn.NivelCalificacion)
                .ToListAsync();

            // Si no hay niveles específicos, usar todos los niveles disponibles
            if (!niveles.Any())
            {
                niveles = await _context.NivelesCalificacion
                    .Where(n => n.IdGrupo == null) // Solo niveles sin grupo asignado
                    .OrderBy(n => n.OrdenNivel ?? int.MaxValue)
                    .ThenBy(n => n.NombreNivel)
                    .ToListAsync();
            }

            var valoresExistentes = await _context.ValoresRubrica
                .Where(v => v.IdRubrica == id)
                .ToListAsync();

            var modelo = new ConfigurarRubricaViewModel
            {
                Rubrica = rubrica,
                Niveles = niveles,
                ValoresExistentes = valoresExistentes
            };

            return View(modelo);
        }

        // POST: ValorRubrica/ConfigurarRubrica
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequirePermission(ApplicationPermissions.Rubricas.EDITAR)]
        public async Task<IActionResult> ConfigurarRubrica(int id, IFormCollection form)
        {
            try
            {
                Console.WriteLine($"=== INICIO ConfigurarRubrica POST ===");
                Console.WriteLine($"id recibido: {id}");
                Console.WriteLine($"Request.Form.Count: {Request.Form.Count}");

                // Extraer valores del formulario manualmente
                var valores = new Dictionary<int, Dictionary<int, decimal?>>();
                var contadorValores = 0;
                
                Console.WriteLine($"=== EXTRAYENDO VALORES ===");
                foreach (var key in Request.Form.Keys.Where(k => k.StartsWith("valores[")))
                {
                    try
                    {
                        // Formato: valores[itemId][nivelId]
                        var match = System.Text.RegularExpressions.Regex.Match(key, @"valores\[(\d+)\]\[(\d+)\]");
                        if (match.Success)
                        {
                            var itemId = int.Parse(match.Groups[1].Value);
                            var nivelId = int.Parse(match.Groups[2].Value);
                            var valorStr = Request.Form[key].ToString().Trim();
                            
                            Console.WriteLine($"Procesando: Item={itemId}, Nivel={nivelId}, Valor='{valorStr}'");
                            
                            if (!string.IsNullOrEmpty(valorStr) && decimal.TryParse(valorStr, out var valor))
                            {
                                if (!valores.ContainsKey(itemId))
                                    valores[itemId] = new Dictionary<int, decimal?>();
                                
                                valores[itemId][nivelId] = valor;
                                contadorValores++;
                                Console.WriteLine($"  ✓ Valor agregado: {valor}");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"  ✗ Error procesando {key}: {ex.Message}");
                    }
                }
                
                Console.WriteLine($"Total valores extraídos: {contadorValores}");

                if (contadorValores == 0)
                {
                    Console.WriteLine("⚠️ NO SE ENCONTRARON VALORES");
                    TempData["ErrorMessage"] = "No se recibieron valores para configurar.";
                    return RedirectToAction(nameof(ConfigurarRubrica), new { id });
                }

                var configuracionesCreadas = 0;
                var configuracionesActualizadas = 0;
                
                // Procesar cada valor
                foreach (var item in valores)
                {
                    var idItem = item.Key;
                    
                    foreach (var nivel in item.Value)
                    {
                        var idNivel = nivel.Key;
                        var valorPuntos = nivel.Value;
                        
                        if (valorPuntos.HasValue)
                        {
                            // Verificar si ya existe un registro
                            var valorExistente = await _context.ValoresRubrica
                                .FirstOrDefaultAsync(v => v.IdRubrica == id && 
                                                        v.IdItem == idItem && 
                                                        v.IdNivel == idNivel);

                            if (valorExistente != null)
                            {
                                // Actualizar valor existente
                                valorExistente.ValorPuntos = valorPuntos.Value;
                                _context.Update(valorExistente);
                                configuracionesActualizadas++;
                            }
                            else
                            {
                                // Crear nuevo valor
                                var nuevoValor = new ValorRubrica
                                {
                                    IdRubrica = id,
                                    IdItem = idItem,
                                    IdNivel = idNivel,
                                    ValorPuntos = valorPuntos.Value
                                };
                                _context.ValoresRubrica.Add(nuevoValor);
                                configuracionesCreadas++;
                            }
                        }
                    }
                }

                Console.WriteLine($"Guardando: {configuracionesCreadas} creadas, {configuracionesActualizadas} actualizadas");
                
                var cambiosRealizados = await _context.SaveChangesAsync();
                Console.WriteLine($"SaveChanges() retornó: {cambiosRealizados}");

                var mensaje = $"Configuración guardada: {configuracionesCreadas} creadas, {configuracionesActualizadas} actualizadas";
                TempData["SuccessMessage"] = mensaje;
                Console.WriteLine($"✓ {mensaje}");
                
                return RedirectToAction(nameof(Index), new { idRubrica = id });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"💥 ERROR: {ex.Message}");
                TempData["ErrorMessage"] = $"Error al guardar: {ex.Message}";
                return RedirectToAction(nameof(ConfigurarRubrica), new { id });
            }
        }

        // Métodos auxiliares para AJAX
        [HttpGet]
        [RequirePermission(ApplicationPermissions.Rubricas.VER)]
        public async Task<IActionResult> CheckDuplicado(int idRubrica, int idItem, int idNivel, int? excludeId = null)
        {
            var exists = await _context.ValoresRubrica
                .AnyAsync(v => v.IdRubrica == idRubrica && 
                              v.IdItem == idItem && 
                              v.IdNivel == idNivel &&
                              (excludeId == null || v.IdValor != excludeId));
            
            return Json(exists);
        }

        private bool ValorRubricaExists(int id)
        {
            return _context.ValoresRubrica.Any(e => e.IdValor == id);
        }

        private async Task CargarListasSelectAsync(int? rubricaSeleccionada = null, int? itemSeleccionado = null)
        {
            // Lista de rúbricas activas
            ViewBag.Rubricas = new SelectList(
                await _context.Rubricas.Where(r => r.Estado == "ACTIVO").ToListAsync(),
                "IdRubrica", "NombreRubrica", rubricaSeleccionada);

            // Lista de items (filtrada por rúbrica si está seleccionada)
            var items = _context.ItemsEvaluacion.AsQueryable();
            if (rubricaSeleccionada.HasValue)
            {
                items = items.Where(i => i.IdRubrica == rubricaSeleccionada.Value);
            }
            ViewBag.Items = new SelectList(
                await items.ToListAsync(),
                "IdItem", "NombreItem", itemSeleccionado);

            // Lista de niveles de calificación
            ViewBag.Niveles = new SelectList(
                await _context.NivelesCalificacion
                    .OrderBy(n => n.OrdenNivel ?? int.MaxValue)
                    .ThenBy(n => n.NombreNivel)
                    .ToListAsync(),
                "IdNivel", "NombreNivel");
        }
    }
}
