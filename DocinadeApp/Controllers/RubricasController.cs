using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DocinadeApp.Data;
using DocinadeApp.Models;
using DocinadeApp.Services;
using DocinadeApp.Services.Identity;
using DocinadeApp.Extensions;
using DocinadeApp.Filters;
using DocinadeApp.Models.Permissions;
using Microsoft.AspNetCore.Authorization;
using ViewModels = DocinadeApp.ViewModels;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace DocinadeApp.Controllers
{
    [Authorize]
    public class RubricasController : BaseController
    {
        private readonly IUserContextService _userContextService;

        public RubricasController(
            RubricasDbContext context, 
            IPeriodoAcademicoService periodoService,
            IUserContextService userContextService)
            : base(periodoService, context)
        {
            _userContextService = userContextService;
        }

        // GET: Rubricas
        public async Task<IActionResult> Index()
        {
            try
            {
                var currentUserId = _userContextService.GetCurrentUserId();
                if (string.IsNullOrEmpty(currentUserId))
                {
                    return Challenge();
                }

                IQueryable<Rubrica> query = _context.Rubricas.AsNoTracking();

                // Aplicar filtros según el rol del usuario
                if (await _userContextService.IsInRoleAsync(ApplicationRoles.SuperAdministrador) ||
                    await _userContextService.IsInRoleAsync(ApplicationRoles.Administrador))
                {
                    // SuperAdmin y Admin pueden ver todas las rúbricas (sin filtro adicional)
                }
                else if (await _userContextService.IsInRoleAsync(ApplicationRoles.Coordinador))
                {
                    // Coordinador puede ver todas las públicas y las que creó
                    query = query.Where(r => r.EsPublica == 1 || r.CreadoPorId == currentUserId);
                }
                else
                {
                    // Docente, Evaluador, Observador: solo las que creó y las públicas
                    query = query.ForCurrentUser(currentUserId);
                }

                var rubricas = await query
                    .Include(r => r.CreadoPor)
                    .OrderBy(r => r.NombreRubrica)
                    .ToListAsync();

                // Crear ViewModel con permisos
                var rubricasWithPermissions = new List<ViewModels.RubricaWithPermissionsViewModel>();
                
                foreach (var rubrica in rubricas)
                {
                    var canEdit = await _userContextService.CanEditEntityAsync(rubrica);
                    var canDelete = await _userContextService.CanDeleteEntityAsync(rubrica);
                    var isOwner = rubrica.BelongsToUser(currentUserId);

                    rubricasWithPermissions.Add(new ViewModels.RubricaWithPermissionsViewModel
                    {
                        Rubrica = rubrica,
                        CanEdit = canEdit,
                        CanDelete = canDelete,
                        CanView = true, // Si está en la lista, puede verla
                        IsOwner = isOwner,
                        CreatedByName = rubrica.CreadoPor?.Nombre + " " + rubrica.CreadoPor?.Apellidos,
                        ModifiedByName = rubrica.ModificadoPor?.Nombre + " " + rubrica.ModificadoPor?.Apellidos
                    });
                }

                var viewModel = new ViewModels.RubricasIndexViewModel
                {
                    Rubricas = rubricasWithPermissions,
                    CanCreateNew = await _userContextService.IsInRoleAsync(ApplicationRoles.SuperAdministrador) ||
                                   await _userContextService.IsInRoleAsync(ApplicationRoles.Administrador) ||
                                   await _userContextService.IsInRoleAsync(ApplicationRoles.Coordinador) ||
                                   await _userContextService.IsInRoleAsync(ApplicationRoles.Docente),
                    CurrentUserRole = await GetCurrentUserRoleAsync(),
                    TotalRubricas = rubricas.Count,
                    MisRubricas = rubricas.Count(r => r.BelongsToUser(currentUserId)),
                    RubricasPublicas = rubricas.Count(r => r.EsPublica == 1)
                };

                // Usar la nueva vista con permisos si existe, sino la tradicional
                if (System.IO.File.Exists(Path.Combine("Views", "Rubricas", "IndexWithPermissions.cshtml")))
                {
                    return View("IndexWithPermissions", viewModel);
                }
                else
                {
                    return View(rubricas); // Fallback a la vista original
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al cargar las rúbricas: {ex.Message}";
                return View(new List<Rubrica>());
            }
        }

        private async Task<string> GetCurrentUserRoleAsync()
        {
            if (await _userContextService.IsInRoleAsync(ApplicationRoles.SuperAdministrador))
                return "Super Administrador";
            if (await _userContextService.IsInRoleAsync(ApplicationRoles.Administrador))
                return "Administrador";
            if (await _userContextService.IsInRoleAsync(ApplicationRoles.Coordinador))
                return "Coordinador";
            if (await _userContextService.IsInRoleAsync(ApplicationRoles.Docente))
                return "Docente";
            if (await _userContextService.IsInRoleAsync(ApplicationRoles.Evaluador))
                return "Evaluador";
            if (await _userContextService.IsInRoleAsync(ApplicationRoles.Observador))
                return "Observador";
            
            return "Usuario";
        }

        // GET: Rubricas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var currentUserId = _userContextService.GetCurrentUserId();
            if (string.IsNullOrEmpty(currentUserId))
            {
                return Challenge();
            }

            var rubrica = await _context.Rubricas
                .Include(r => r.ItemsEvaluacion)
                .Include(r => r.ValoresRubrica)
                .Include(r => r.CreadoPor)
                .FirstOrDefaultAsync(m => m.IdRubrica == id);

            if (rubrica == null)
            {
                return NotFound();
            }

            // Verificar si el usuario puede acceder a esta rúbrica
            if (!await _userContextService.CanAccessEntityAsync(rubrica))
            {
                return Forbid();
            }

            return View(rubrica);
        }

        // GET: Rubricas/Create - Fixed version with proper ViewBag handling
        public async Task<IActionResult> Create()
        {
            try
            {
                // Crear listas simples que funcionen con la vista
                var gruposDisponibles = new List<ViewModels.GrupoViewModel>();
                var nivelesDisponibles = new List<ViewModels.NivelViewModel>();
                
                try
                {
                    // Obtener grupos de calificación activos
                    var grupos = await _context.GruposCalificacion
                        .Where(g => g.Estado == "ACTIVO")
                        .Include(g => g.NivelesCalificacion)
                        .ToListAsync();
                    
                    foreach (var grupo in grupos)
                    {
                        gruposDisponibles.Add(new ViewModels.GrupoViewModel
                        {
                            IdGrupo = grupo.IdGrupo,
                            NombreGrupo = grupo.NombreGrupo ?? "Sin nombre",
                            CantidadNiveles = grupo.NivelesCalificacion?.Count ?? 0
                        });
                    }
                    
                    // Obtener niveles disponibles (sin grupo o disponibles para asignación)
                    var niveles = await _context.NivelesCalificacion
                        .OrderBy(n => n.OrdenNivel ?? int.MaxValue)
                        .ThenBy(n => n.NombreNivel)
                        .ToListAsync();
                    
                    foreach (var nivel in niveles)
                    {
                        nivelesDisponibles.Add(new ViewModels.NivelViewModel
                        {
                            IdNivel = nivel.IdNivel,
                            NombreNivel = nivel.NombreNivel ?? "Sin nombre",
                            Descripcion = nivel.Descripcion ?? ""
                        });
                    }
                }
                catch (Exception ex)
                {
                    // En caso de error, usar listas vacías pero log el error
                    ViewBag.ErrorMessage = $"Error al cargar datos: {ex.Message}";
                }
                
                // Asignar a ViewBag con el tipo correcto
                ViewBag.GruposDisponibles = gruposDisponibles;
                ViewBag.NivelesDisponibles = nivelesDisponibles;
                
                return View();
            }
            catch (Exception ex)
            {
                // Manejo de error completo
                ViewBag.GruposDisponibles = new List<ViewModels.GrupoViewModel>();
                ViewBag.NivelesDisponibles = new List<ViewModels.NivelViewModel>();
                ViewBag.ErrorMessage = $"Error crítico al cargar la página: {ex.Message}";
                TempData["ErrorMessage"] = $"Error al cargar la página: {ex.Message}";
                
                return View();
            }
        }

        // POST: Rubricas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NombreRubrica,Descripcion,Estado")] Rubrica rubrica, int? IdGrupo, string TipoGrupo, string NivelesSeleccionados)
        {
            var currentUserId = _userContextService.GetCurrentUserId();
            if (string.IsNullOrEmpty(currentUserId))
            {
                return Challenge();
            }

            if (ModelState.IsValid)
            {
                // Aplicar auditoría automática
                rubrica.CreadoPorId = currentUserId;
                rubrica.FechaCreacion = DateTime.Now;
                
                if (string.IsNullOrEmpty(rubrica.Estado))
                {
                    rubrica.Estado = "ACTIVO";
                }
                
                // Crear la rúbrica primero
                _context.Add(rubrica);
                
                // Aplicar auditoría a todas las entidades modificadas
                _context.ApplyAuditInfo(currentUserId);
                await _context.SaveChangesAsync();
                
                try
                {
                    // Manejar asignación de grupos
                    if (!string.IsNullOrEmpty(TipoGrupo))
                    {
                        if (TipoGrupo == "existente" && IdGrupo.HasValue)
                        {
                            // Asignar grupo existente
                            rubrica.IdGrupo = IdGrupo.Value;
                            
                            // Obtener niveles del grupo y crear asociaciones
                            var nivelesDelGrupo = await _context.NivelesCalificacion
                                .Where(n => n.IdGrupo == IdGrupo.Value)
                                .OrderBy(n => n.OrdenNivel ?? int.MaxValue)
                                .ToListAsync();
                            
                            var orden = 1;
                            foreach (var nivel in nivelesDelGrupo)
                            {
                                var rubricaNivel = new RubricaNivel
                                {
                                    IdRubrica = rubrica.IdRubrica,
                                    IdNivel = nivel.IdNivel,
                                    OrdenEnRubrica = orden++
                                };
                                _context.RubricaNiveles.Add(rubricaNivel);
                            }
                        }
                        else if (TipoGrupo == "dinamico" && !string.IsNullOrEmpty(NivelesSeleccionados))
                        {
                            // Crear grupo dinámico
                            var nombreGrupo = $"Grupo_{rubrica.NombreRubrica.Replace(" ", "_")}_{DateTime.Now:yyyyMMdd_HHmmss}";
                            
                            var nuevoGrupo = new GrupoCalificacion
                            {
                                NombreGrupo = nombreGrupo,
                                Descripcion = $"Grupo dinámico para rúbrica '{rubrica.NombreRubrica}'",
                                Estado = "ACTIVO",
                                FechaCreacion = DateTime.Now
                            };
                            
                            _context.GruposCalificacion.Add(nuevoGrupo);
                            await _context.SaveChangesAsync();
                            
                            // Procesar niveles seleccionados
                            var nivelesIds = NivelesSeleccionados.Split(',')
                                .Where(s => int.TryParse(s, out _))
                                .Select(int.Parse)
                                .ToList();
                            
                            var nivelesOriginales = await _context.NivelesCalificacion
                                .Where(n => nivelesIds.Contains(n.IdNivel))
                                .ToListAsync();
                            
                            // Crear copias de los niveles para el nuevo grupo
                            var orden = 1;
                            foreach (var nivelOriginal in nivelesOriginales)
                            {
                                var nuevoNivel = new NivelCalificacion
                                {
                                    NombreNivel = nivelOriginal.NombreNivel,
                                    Descripcion = nivelOriginal.Descripcion,
                                    OrdenNivel = nivelOriginal.OrdenNivel,
                                    IdGrupo = nuevoGrupo.IdGrupo
                                };
                                _context.NivelesCalificacion.Add(nuevoNivel);
                                await _context.SaveChangesAsync(); // Para obtener el ID
                                
                                // Crear asociación RubricaNivel
                                var rubricaNivel = new RubricaNivel
                                {
                                    IdRubrica = rubrica.IdRubrica,
                                    IdNivel = nuevoNivel.IdNivel,
                                    OrdenEnRubrica = orden++
                                };
                                _context.RubricaNiveles.Add(rubricaNivel);
                            }
                            
                            // Asignar el grupo a la rúbrica
                            rubrica.IdGrupo = nuevoGrupo.IdGrupo;
                        }
                    }
                    
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Rúbrica creada exitosamente" + 
                        (!string.IsNullOrEmpty(TipoGrupo) ? " con grupo de calificación asignado." : ".");
                    
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    // Si hay error, eliminar la rúbrica creada
                    _context.Rubricas.Remove(rubrica);
                    await _context.SaveChangesAsync();
                    
                    TempData["ErrorMessage"] = $"Error al crear la rúbrica: {ex.Message}";
                }
            }
            
            // Recargar datos para la vista en caso de error
            await LoadViewBagData();
            
            return View(rubrica);
        }

        // Método auxiliar para cargar datos del ViewBag
        private async Task LoadViewBagData()
        {
            try
            {
                var gruposDisponibles = new List<ViewModels.GrupoViewModel>();
                var nivelesDisponibles = new List<ViewModels.NivelViewModel>();
                
                var grupos = await _context.GruposCalificacion
                    .Where(g => g.Estado == "ACTIVO")
                    .Include(g => g.NivelesCalificacion)
                    .ToListAsync();
                
                foreach (var grupo in grupos)
                {
                    gruposDisponibles.Add(new ViewModels.GrupoViewModel
                    {
                        IdGrupo = grupo.IdGrupo,
                        NombreGrupo = grupo.NombreGrupo ?? "",
                        CantidadNiveles = grupo.NivelesCalificacion?.Count ?? 0
                    });
                }
                
                var niveles = await _context.NivelesCalificacion
                    .OrderBy(n => n.OrdenNivel ?? int.MaxValue)
                    .ThenBy(n => n.NombreNivel)
                    .ToListAsync();
                
                foreach (var nivel in niveles)
                {
                    nivelesDisponibles.Add(new ViewModels.NivelViewModel
                    {
                        IdNivel = nivel.IdNivel,
                        NombreNivel = nivel.NombreNivel ?? "",
                        Descripcion = nivel.Descripcion ?? ""
                    });
                }
                
                ViewBag.GruposDisponibles = gruposDisponibles;
                ViewBag.NivelesDisponibles = nivelesDisponibles;
            }
            catch (Exception)
            {
                ViewBag.GruposDisponibles = new List<ViewModels.GrupoViewModel>();
                ViewBag.NivelesDisponibles = new List<ViewModels.NivelViewModel>();
            }
        }

        // GET: Rubricas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var currentUserId = _userContextService.GetCurrentUserId();
            if (string.IsNullOrEmpty(currentUserId))
            {
                return Challenge();
            }

            // Cargar rúbrica con relaciones necesarias para evitar errores de navegación
            var rubrica = await _context.Rubricas
                .Include(r => r.CreadoPor)
                .Include(r => r.ModificadoPor)
                .Include(r => r.GrupoCalificacion)
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.IdRubrica == id);

            if (rubrica == null)
            {
                return NotFound();
            }

            // Verificar si el usuario puede editar esta rúbrica
            if (!await _userContextService.CanEditEntityAsync(rubrica))
            {
                TempData["ErrorMessage"] = "No tienes permisos para editar esta rúbrica.";
                return RedirectToAction(nameof(Index));
            }

            return View(rubrica);
        }

        // POST: Rubricas/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdRubrica,NombreRubrica,Descripcion,Estado,FechaCreacion")] Rubrica rubrica)
        {
            if (id != rubrica.IdRubrica)
            {
                return NotFound();
            }

            var currentUserId = _userContextService.GetCurrentUserId();
            if (string.IsNullOrEmpty(currentUserId))
            {
                return Challenge();
            }

            // Verificar permisos antes de la edición - cargar con relaciones
            var rubricaExistente = await _context.Rubricas
                .Include(r => r.CreadoPor)
                .Include(r => r.ModificadoPor)
                .FirstOrDefaultAsync(r => r.IdRubrica == id);
                
            if (rubricaExistente == null)
            {
                return NotFound();
            }

            if (!await _userContextService.CanEditEntityAsync(rubricaExistente))
            {
                TempData["ErrorMessage"] = "No tienes permisos para editar esta rúbrica.";
                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Actualizar propiedades de la entidad rastreada
                    rubricaExistente.NombreRubrica = rubrica.NombreRubrica;
                    rubricaExistente.Descripcion = rubrica.Descripcion;
                    rubricaExistente.Estado = rubrica.Estado;
                    rubricaExistente.ModificadoPorId = currentUserId;
                    rubricaExistente.FechaModificacion = DateTime.Now;
                    
                    // Aplicar auditoría automática
                    _context.ApplyAuditInfo(currentUserId);
                    await _context.SaveChangesAsync();
                    
                    TempData["SuccessMessage"] = "Rúbrica actualizada exitosamente.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RubricaExists(rubrica.IdRubrica))
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
            return View(rubrica);
        }

        // GET: Rubricas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var currentUserId = _userContextService.GetCurrentUserId();
            if (string.IsNullOrEmpty(currentUserId))
            {
                return Challenge();
            }

            var rubrica = await _context.Rubricas
                .Include(r => r.CreadoPor)
                .FirstOrDefaultAsync(m => m.IdRubrica == id);
            
            if (rubrica == null)
            {
                return NotFound();
            }

            // Verificar si el usuario puede eliminar esta rúbrica
            if (!await _userContextService.CanDeleteEntityAsync(rubrica))
            {
                TempData["ErrorMessage"] = "No tienes permisos para eliminar esta rúbrica.";
                return RedirectToAction(nameof(Index));
            }

            return View(rubrica);
        }

        // POST: Rubricas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var currentUserId = _userContextService.GetCurrentUserId();
            if (string.IsNullOrEmpty(currentUserId))
            {
                return Challenge();
            }

            // Cargar con relaciones para verificación de permisos
            var rubrica = await _context.Rubricas
                .Include(r => r.CreadoPor)
                .Include(r => r.ModificadoPor)
                .FirstOrDefaultAsync(r => r.IdRubrica == id);
                
            if (rubrica != null)
            {
                // Verificar permisos antes de eliminar
                if (!await _userContextService.CanDeleteEntityAsync(rubrica))
                {
                    TempData["ErrorMessage"] = "No tienes permisos para eliminar esta rúbrica.";
                    return RedirectToAction(nameof(Index));
                }

                try
                {
                    _context.Rubricas.Remove(rubrica);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Rúbrica eliminada exitosamente.";
                }
                catch (DbUpdateException)
                {
                    TempData["ErrorMessage"] = "No se puede eliminar la rúbrica porque tiene datos relacionados.";
                    return RedirectToAction(nameof(Index));
                }
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Rubricas/GestionarValoresItems/5
        public async Task<IActionResult> GestionarValoresItems(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var currentUserId = _userContextService.GetCurrentUserId();
            if (string.IsNullOrEmpty(currentUserId))
            {
                return Challenge();
            }

            try
            {
                // Obtener la rúbrica con sus items de evaluación
                var rubrica = await _context.Rubricas
                    .Include(r => r.ItemsEvaluacion.OrderBy(i => i.OrdenItem ?? int.MaxValue))
                    .FirstOrDefaultAsync(r => r.IdRubrica == id);

                if (rubrica == null)
                {
                    return NotFound();
                }

                // Verificar si el usuario puede gestionar esta rúbrica
                if (!await _userContextService.CanEditEntityAsync(rubrica))
                {
                    TempData["ErrorMessage"] = "No tienes permisos para gestionar los valores de esta rúbrica.";
                    return RedirectToAction(nameof(Index));
                }

                // Obtener todos los items de evaluación de la rúbrica
                var items = rubrica.ItemsEvaluacion.ToList();

                // Obtener niveles disponibles para esta rúbrica
                var niveles = await _context.NivelesCalificacion
                    .Where(nc => _context.RubricaNiveles
                        .Any(rn => rn.IdRubrica == id && rn.IdNivel == nc.IdNivel))
                    .OrderBy(nc => nc.OrdenNivel ?? int.MaxValue)
                    .ToListAsync();

                // Si no hay niveles específicos, obtener todos los niveles disponibles
                if (!niveles.Any())
                {
                    niveles = await _context.NivelesCalificacion
                        .OrderBy(nc => nc.OrdenNivel ?? int.MaxValue)
                        .ToListAsync();
                }

                // Obtener valores existentes
                var valoresExistentes = await _context.ValoresRubrica
                    .Where(vr => vr.IdRubrica == id)
                    .ToListAsync();

                // Identificar items sin configurar
                var itemsSinConfigurar = items
                    .Where(item => !valoresExistentes.Any(v => v.IdItem == item.IdItem))
                    .ToList();

                // Crear matriz de valores para la vista - usar cast explícito para compatibilidad
                var matrizValores = new Dictionary<int, Dictionary<int, ValorRubrica>>();
                foreach (var item in items)
                {
                    matrizValores[item.IdItem] = new Dictionary<int, ValorRubrica>();
                    foreach (var nivel in niveles)
                    {
                        var valor = valoresExistentes.FirstOrDefault(v => v.IdItem == item.IdItem && v.IdNivel == nivel.IdNivel);
                        matrizValores[item.IdItem][nivel.IdNivel] = valor!; // Nota: puede ser null pero se maneja en la vista
                    }
                }

                var viewModel = new ViewModels.GestionarValoresItemsViewModel
                {
                    RubricaId = id.Value,
                    RubricaNombre = rubrica.NombreRubrica ?? "Sin nombre",
                    Rubrica = rubrica, // Para compatibilidad con la vista
                    Items = items,
                    Niveles = niveles,
                    NivelesDisponibles = niveles, // Para compatibilidad con la vista
                    ValoresExistentes = valoresExistentes,
                    ItemsSinConfigurar = itemsSinConfigurar,
                    ItemsSinValores = itemsSinConfigurar, // Para compatibilidad con la vista
                    MatrizValores = matrizValores // Para compatibilidad con la vista
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al cargar la gestión de valores: {ex.Message}";
                return RedirectToAction(nameof(Details), new { id });
            }
        }

        // POST: Rubricas/AsignarNivelesAutomatico
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AsignarNivelesAutomatico(int rubricaId, int itemId)
        {
            try
            {
                var item = await _context.ItemsEvaluacion
                    .FirstOrDefaultAsync(i => i.IdItem == itemId && i.IdRubrica == rubricaId);

                if (item == null)
                {
                    return Json(new { success = false, message = "Item no encontrado" });
                }

                // Obtener niveles que ya tienen valores configurados para otros items de esta rúbrica
                var nivelesExistentes = await _context.ValoresRubrica
                    .Where(vr => vr.IdRubrica == rubricaId)
                    .Select(vr => vr.IdNivel)
                    .Distinct()
                    .ToListAsync();

                // Si no hay niveles existentes, usar niveles asociados a la rúbrica
                if (!nivelesExistentes.Any())
                {
                    nivelesExistentes = await _context.RubricaNiveles
                        .Where(rn => rn.IdRubrica == rubricaId)
                        .Select(rn => rn.IdNivel)
                        .ToListAsync();
                }

                // Si aún no hay niveles, usar todos los niveles disponibles
                if (!nivelesExistentes.Any())
                {
                    nivelesExistentes = await _context.NivelesCalificacion
                        .Select(nc => nc.IdNivel)
                        .ToListAsync();
                }

                // Verificar si ya existen valores para este item
                var valoresExistentes = await _context.ValoresRubrica
                    .Where(vr => vr.IdRubrica == rubricaId && vr.IdItem == itemId)
                    .ToListAsync();

                if (valoresExistentes.Any())
                {
                    return Json(new { 
                        success = false, 
                        message = $"El item '{item.NombreItem}' ya tiene valores configurados"
                    });
                }

                // Obtener información de los niveles
                var niveles = await _context.NivelesCalificacion
                    .Where(nc => nivelesExistentes.Contains(nc.IdNivel))
                    .OrderBy(nc => nc.OrdenNivel ?? int.MaxValue)
                    .ToListAsync();

                if (!niveles.Any())
                {
                    return Json(new { 
                        success = false, 
                        message = "No se encontraron niveles configurados para esta rúbrica"
                    });
                }

                // Crear valores estándar basados en el orden del nivel
                var valoresCreados = new List<ValorRubrica>();
                foreach (var nivel in niveles)
                {
                    decimal valorPuntos = nivel.OrdenNivel switch
                    {
                        1 => 4.0m, // Excelente
                        2 => 3.0m, // Bueno
                        3 => 2.0m, // Regular
                        4 => 1.0m, // Deficiente
                        _ => 2.0m  // Valor por defecto
                    };

                    var valorRubrica = new ValorRubrica
                    {
                        IdRubrica = rubricaId,
                        IdItem = itemId,
                        IdNivel = nivel.IdNivel,
                        ValorPuntos = valorPuntos
                    };

                    valoresCreados.Add(valorRubrica);
                }

                // Guardar los nuevos valores
                _context.ValoresRubrica.AddRange(valoresCreados);
                await _context.SaveChangesAsync();

                return Json(new { 
                    success = true, 
                    message = $"Se asignaron {valoresCreados.Count} niveles al item '{item.NombreItem}' exitosamente",
                    valoresCreados = valoresCreados.Select(v => new {
                        nivelNombre = niveles.First(n => n.IdNivel == v.IdNivel).NombreNivel,
                        v.ValorPuntos
                    })
                });
            }
            catch (Exception ex)
            {
                return Json(new { 
                    success = false, 
                    message = $"Error: {ex.Message}"
                });
            }
        }

        // POST: Rubricas/AsignarNivelesPersonalizado
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AsignarNivelesPersonalizado(int rubricaId, int itemId, Dictionary<int, decimal> valoresPorNivel)
        {
            try
            {
                var item = await _context.ItemsEvaluacion
                    .FirstOrDefaultAsync(i => i.IdItem == itemId && i.IdRubrica == rubricaId);

                if (item == null)
                {
                    return Json(new { success = false, message = "Item no encontrado" });
                }

                // Verificar si ya existen valores para este item
                var valoresExistentes = await _context.ValoresRubrica
                    .Where(vr => vr.IdRubrica == rubricaId && vr.IdItem == itemId)
                    .ToListAsync();

                // Eliminar valores existentes si los hay
                if (valoresExistentes.Any())
                {
                    _context.ValoresRubrica.RemoveRange(valoresExistentes);
                }

                // Crear nuevos valores
                var valoresCreados = new List<ValorRubrica>();
                foreach (var kvp in valoresPorNivel)
                {
                    if (kvp.Value >= 0) // Solo crear si el valor es válido
                    {
                        var valorRubrica = new ValorRubrica
                        {
                            IdRubrica = rubricaId,
                            IdItem = itemId,
                            IdNivel = kvp.Key,
                            ValorPuntos = kvp.Value
                        };
                        valoresCreados.Add(valorRubrica);
                    }
                }

                if (!valoresCreados.Any())
                {
                    return Json(new { 
                        success = false, 
                        message = "Debe especificar al menos un valor válido"
                    });
                }

                // Guardar los nuevos valores
                _context.ValoresRubrica.AddRange(valoresCreados);
                await _context.SaveChangesAsync();

                // Obtener nombres de niveles para la respuesta
                var niveles = await _context.NivelesCalificacion
                    .Where(nc => valoresPorNivel.Keys.Contains(nc.IdNivel))
                    .ToListAsync();

                return Json(new { 
                    success = true, 
                    message = $"Se configuraron {valoresCreados.Count} valores personalizados para el item '{item.NombreItem}' exitosamente",
                    valoresCreados = valoresCreados.Select(v => new {
                        nivelNombre = niveles.First(n => n.IdNivel == v.IdNivel).NombreNivel,
                        v.ValorPuntos
                    })
                });
            }
            catch (Exception ex)
            {
                return Json(new { 
                    success = false, 
                    message = $"Error: {ex.Message}"
                });
            }
        }

        // GET: Rubricas/DiagnosticarItems/5
        [HttpGet]
        public async Task<IActionResult> DiagnosticarItems(int id)
        {
            try
            {
                var rubrica = await _context.Rubricas
                    .Include(r => r.ItemsEvaluacion)
                    .FirstOrDefaultAsync(r => r.IdRubrica == id);

                if (rubrica == null)
                {
                    return Json(new { error = "Rúbrica no encontrada" });
                }

                var diagnostico = new List<object>();

                foreach (var item in rubrica.ItemsEvaluacion)
                {
                    var valoresConfigurados = await _context.ValoresRubrica
                        .Where(vr => vr.IdRubrica == id && vr.IdItem == item.IdItem)
                        .Include(vr => vr.NivelCalificacion)
                        .ToListAsync();

                    diagnostico.Add(new {
                        item.IdItem,
                        item.NombreItem,
                        item.OrdenItem,
                        tieneValoresConfigurados = valoresConfigurados.Any(),
                        cantidadNiveles = valoresConfigurados.Count,
                        niveles = valoresConfigurados.Select(v => new {
                            v.NivelCalificacion.NombreNivel,
                            v.ValorPuntos
                        }),
                        problema = !valoresConfigurados.Any() ? "SIN_VALORES" : "OK"
                    });
                }

                return Json(new {
                    rubrica = new {
                        rubrica.IdRubrica,
                        rubrica.NombreRubrica
                    },
                    totalItems = rubrica.ItemsEvaluacion.Count,
                    itemsSinValores = diagnostico.Count(d => ((string)((dynamic)d).problema) == "SIN_VALORES"),
                    itemsConValores = diagnostico.Count(d => ((string)((dynamic)d).problema) == "OK"),
                    detalleItems = diagnostico
                });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        private bool RubricaExists(int id)
        {
            return _context.Rubricas.Any(e => e.IdRubrica == id);
        }

        // GET: Rubricas/ConfigureQualificationLevels/5
        public async Task<IActionResult> ConfigureQualificationLevels(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var rubrica = await _context.Rubricas
                    .Include(r => r.RubricaNiveles)
                    .ThenInclude(rn => rn.NivelCalificacion)
                    .FirstOrDefaultAsync(m => m.IdRubrica == id);

                if (rubrica == null)
                {
                    return NotFound();
                }

                // Obtener todos los niveles disponibles
                var todosLosNiveles = await _context.NivelesCalificacion
                    .OrderBy(n => n.OrdenNivel ?? int.MaxValue)
                    .ThenBy(n => n.NombreNivel)
                    .ToListAsync();

                var viewModel = new ViewModels.RubricaConfiguracionViewModel
                {
                    Rubrica = rubrica,
                    TodosLosNiveles = todosLosNiveles,
                    NivelesSeleccionados = rubrica.RubricaNiveles?.Select(rn => rn.IdNivel).ToList() ?? new List<int>()
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al cargar la configuración: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Rubricas/ConfigureQualificationLevels/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfigureQualificationLevels(int id, int[] selectedLevels, bool createDynamicGroup = false)
        {
            try
            {
                var rubrica = await _context.Rubricas
                    .Include(r => r.RubricaNiveles)
                    .FirstOrDefaultAsync(r => r.IdRubrica == id);

                if (rubrica == null)
                {
                    return NotFound();
                }

                if (selectedLevels == null || selectedLevels.Length == 0)
                {
                    TempData["ErrorMessage"] = "Debe seleccionar al menos un nivel de calificación.";
                    return RedirectToAction(nameof(ConfigureQualificationLevels), new { id });
                }

                // Si se solicita crear un grupo dinámico
                if (createDynamicGroup)
                {
                    var nombreGrupo = $"Grupo_{rubrica.NombreRubrica.Replace(" ", "_")}_{DateTime.Now:yyyyMMdd_HHmmss}";

                    var nuevoGrupo = new GrupoCalificacion
                    {
                        NombreGrupo = nombreGrupo,
                        Descripcion = $"Grupo dinámico para rúbrica '{rubrica.NombreRubrica}' con {selectedLevels.Length} niveles",
                        Estado = "ACTIVO",
                        FechaCreacion = DateTime.Now
                    };

                    _context.GruposCalificacion.Add(nuevoGrupo);
                    await _context.SaveChangesAsync();

                    // Crear copias de los niveles seleccionados para el nuevo grupo
                    var nivelesOriginales = await _context.NivelesCalificacion
                        .Where(n => selectedLevels.Contains(n.IdNivel))
                        .ToListAsync();

                    var nuevosNiveles = new List<NivelCalificacion>();
                    foreach (var nivelOriginal in nivelesOriginales)
                    {
                        var nuevoNivel = new NivelCalificacion
                        {
                            NombreNivel = nivelOriginal.NombreNivel,
                            Descripcion = nivelOriginal.Descripcion,
                            OrdenNivel = nivelOriginal.OrdenNivel,
                            IdGrupo = nuevoGrupo.IdGrupo
                        };
                        nuevosNiveles.Add(nuevoNivel);
                    }

                    _context.NivelesCalificacion.AddRange(nuevosNiveles);
                    await _context.SaveChangesAsync();

                    // Asignar el grupo a la rúbrica
                    rubrica.IdGrupo = nuevoGrupo.IdGrupo;

                    // Actualizar selectedLevels con los IDs de los nuevos niveles
                    selectedLevels = nuevosNiveles.Select(n => n.IdNivel).ToArray();
                }

                // Eliminar asociaciones existentes
                var asociacionesExistentes = rubrica.RubricaNiveles.ToList();
                _context.RubricaNiveles.RemoveRange(asociacionesExistentes);

                // Agregar nuevas asociaciones
                foreach (var nivelId in selectedLevels)
                {
                    var nuevaAsociacion = new RubricaNivel
                    {
                        IdRubrica = rubrica.IdRubrica,
                        IdNivel = nivelId
                    };
                    _context.RubricaNiveles.Add(nuevaAsociacion);
                }

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = createDynamicGroup 
                    ? $"Grupo dinámico creado y niveles de calificación configurados exitosamente para la rúbrica."
                    : "Niveles de calificación configurados exitosamente para la rúbrica.";

                return RedirectToAction(nameof(Details), new { id });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al configurar niveles: {ex.Message}";
                return RedirectToAction(nameof(ConfigureQualificationLevels), new { id });
            }
        }

        // GET: Rubricas/ExportarExcel/5
        public async Task<IActionResult> ExportarExcel(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var currentUserId = _userContextService.GetCurrentUserId();
            if (string.IsNullOrEmpty(currentUserId))
            {
                return Challenge();
            }

            // Cargar la rúbrica con todos sus datos relacionados
            var rubrica = await _context.Rubricas
                .Include(r => r.ItemsEvaluacion.OrderBy(i => i.OrdenItem))
                .Include(r => r.ValoresRubrica)
                    .ThenInclude(v => v.NivelCalificacion)
                .Include(r => r.ValoresRubrica)
                    .ThenInclude(v => v.ItemEvaluacion)
                .Include(r => r.RubricaNiveles.OrderBy(rn => rn.OrdenEnRubrica))
                    .ThenInclude(rn => rn.NivelCalificacion)
                .Include(r => r.CreadoPor)
                .FirstOrDefaultAsync(r => r.IdRubrica == id);

            if (rubrica == null)
            {
                return NotFound();
            }

            // Verificar permisos de acceso
            if (!await _userContextService.CanAccessEntityAsync(rubrica))
            {
                return Forbid();
            }

            // Configurar licencia de EPPlus 8+
            ExcelPackage.License.SetNonCommercialPersonal("RubricasApp");

            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Rubrica");

            // --- ENCABEZADO DE LA RÚBRICA ---
            int currentRow = 1;

            // Título principal
            worksheet.Cells[currentRow, 1].Value = "RUBRICA DE EVALUACION";
            worksheet.Cells[currentRow, 1, currentRow, 8].Merge = true;
            worksheet.Cells[currentRow, 1].Style.Font.Size = 16;
            worksheet.Cells[currentRow, 1].Style.Font.Bold = true;
            worksheet.Cells[currentRow, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells[currentRow, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells[currentRow, 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(68, 114, 196));
            worksheet.Cells[currentRow, 1].Style.Font.Color.SetColor(System.Drawing.Color.White);
            currentRow += 2;

            // Información de la rúbrica
            worksheet.Cells[currentRow, 1].Value = "Nombre:";
            worksheet.Cells[currentRow, 1].Style.Font.Bold = true;
            worksheet.Cells[currentRow, 2].Value = rubrica.NombreRubrica;
            worksheet.Cells[currentRow, 2, currentRow, 8].Merge = true;
            currentRow++;

            if (!string.IsNullOrEmpty(rubrica.Descripcion))
            {
                worksheet.Cells[currentRow, 1].Value = "Descripcion:";
                worksheet.Cells[currentRow, 1].Style.Font.Bold = true;
                worksheet.Cells[currentRow, 2].Value = rubrica.Descripcion;
                worksheet.Cells[currentRow, 2, currentRow, 8].Merge = true;
                worksheet.Cells[currentRow, 2].Style.WrapText = true;
                currentRow++;
            }

            worksheet.Cells[currentRow, 1].Value = "Fecha de Creacion:";
            worksheet.Cells[currentRow, 1].Style.Font.Bold = true;
            worksheet.Cells[currentRow, 2].Value = rubrica.FechaCreacion.ToString("dd/MM/yyyy");
            currentRow++;

            worksheet.Cells[currentRow, 1].Value = "Estado:";
            worksheet.Cells[currentRow, 1].Style.Font.Bold = true;
            worksheet.Cells[currentRow, 2].Value = rubrica.Estado;
            currentRow++;

            if (rubrica.CreadoPor != null)
            {
                worksheet.Cells[currentRow, 1].Value = "Creado por:";
                worksheet.Cells[currentRow, 1].Style.Font.Bold = true;
                worksheet.Cells[currentRow, 2].Value = $"{rubrica.CreadoPor.Nombre} {rubrica.CreadoPor.Apellidos}";
                currentRow++;
            }

            currentRow += 2; // Espacio antes de la tabla

            // --- TABLA DE RÚBRICA ---
            int startTableRow = currentRow;

            // Obtener niveles ordenados
            var niveles = rubrica.RubricaNiveles
                .OrderBy(rn => rn.OrdenEnRubrica)
                .Select(rn => rn.NivelCalificacion)
                .ToList();

            // Si no hay niveles asociados mediante RubricaNiveles, obtener todos los niveles únicos de los valores
            if (!niveles.Any())
            {
                niveles = rubrica.ValoresRubrica
                    .Select(v => v.NivelCalificacion)
                    .Distinct()
                    .OrderBy(n => n.OrdenNivel ?? int.MaxValue)
                    .ToList();
            }

            // Encabezados de la tabla
            worksheet.Cells[currentRow, 1].Value = "Items / Criterios";
            worksheet.Cells[currentRow, 1].Style.Font.Bold = true;
            worksheet.Cells[currentRow, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells[currentRow, 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(68, 114, 196));
            worksheet.Cells[currentRow, 1].Style.Font.Color.SetColor(System.Drawing.Color.White);
            worksheet.Cells[currentRow, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells[currentRow, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            int colIndex = 2;
            foreach (var nivel in niveles)
            {
                worksheet.Cells[currentRow, colIndex].Value = nivel.NombreNivel;
                worksheet.Cells[currentRow, colIndex].Style.Font.Bold = true;
                worksheet.Cells[currentRow, colIndex].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[currentRow, colIndex].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(68, 114, 196));
                worksheet.Cells[currentRow, colIndex].Style.Font.Color.SetColor(System.Drawing.Color.White);
                worksheet.Cells[currentRow, colIndex].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[currentRow, colIndex].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[currentRow, colIndex].Style.WrapText = true;
                colIndex++;
            }

            currentRow++;

            // Filas de items
            var items = rubrica.ItemsEvaluacion
                .OrderBy(i => i.OrdenItem ?? int.MaxValue)
                .ThenBy(i => i.NombreItem)
                .ToList();

            foreach (var item in items)
            {
                // Nombre del item
                worksheet.Cells[currentRow, 1].Value = item.NombreItem;
                worksheet.Cells[currentRow, 1].Style.Font.Bold = true;
                worksheet.Cells[currentRow, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[currentRow, 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(217, 225, 242));
                worksheet.Cells[currentRow, 1].Style.WrapText = true;

                // Valores para cada nivel
                colIndex = 2;
                foreach (var nivel in niveles)
                {
                    var valor = rubrica.ValoresRubrica
                        .FirstOrDefault(v => v.IdItem == item.IdItem && v.IdNivel == nivel.IdNivel);

                    if (valor != null)
                    {
                        worksheet.Cells[currentRow, colIndex].Value = valor.ValorPuntos;
                        worksheet.Cells[currentRow, colIndex].Style.Numberformat.Format = "0.00";
                    }
                    else
                    {
                        worksheet.Cells[currentRow, colIndex].Value = "-";
                    }

                    worksheet.Cells[currentRow, colIndex].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[currentRow, colIndex].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    colIndex++;
                }

                currentRow++;
            }

            // Aplicar bordes a toda la tabla
            int endTableRow = currentRow - 1;
            int endTableCol = 1 + niveles.Count;
            var tableRange = worksheet.Cells[startTableRow, 1, endTableRow, endTableCol];
            tableRange.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            tableRange.Style.Border.Left.Style = ExcelBorderStyle.Thin;
            tableRange.Style.Border.Right.Style = ExcelBorderStyle.Thin;
            tableRange.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

            // Ajustar ancho de columnas
            worksheet.Column(1).Width = 40; // Columna de items más ancha
            for (int i = 2; i <= endTableCol; i++)
            {
                worksheet.Column(i).Width = 15;
            }

            // Agregar nota al final
            currentRow += 2;
            worksheet.Cells[currentRow, 1].Value = $"Documento generado el {DateTime.Now:dd/MM/yyyy HH:mm}";
            worksheet.Cells[currentRow, 1].Style.Font.Italic = true;
            worksheet.Cells[currentRow, 1].Style.Font.Size = 9;
            worksheet.Cells[currentRow, 1, currentRow, endTableCol].Merge = true;

            // Preparar archivo para descarga
            var fileName = $"Rubrica_{rubrica.NombreRubrica.Replace(" ", "_")}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
            var fileBytes = await package.GetAsByteArrayAsync();

            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
    }
}