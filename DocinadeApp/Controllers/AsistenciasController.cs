using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RubricasApp.Web.Data;
using RubricasApp.Web.Models;
using RubricasApp.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace RubricasApp.Web.Controllers
{
    /// <summary>
    /// Controlador CRUD para la gestión completa de registros de asistencia
    /// </summary>
    [Authorize]
    public class AsistenciasController : Controller
    {
        private readonly RubricasDbContext _context;
        private readonly ILogger<AsistenciasController> _logger;

        public AsistenciasController(RubricasDbContext context, ILogger<AsistenciasController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Lista todos los registros de asistencia con filtros
        /// </summary>
        /// <param name="grupoId">Filtro por grupo</param>
        /// <param name="fecha">Filtro por fecha</param>
        /// <param name="estado">Filtro por estado</param>
        /// <param name="pageNumber">Número de página</param>
        /// <returns></returns>
        public async Task<IActionResult> Index(int? grupoId, DateTime? fecha, string? estado, int pageNumber = 1)
        {
            try
            {
                const int pageSize = 20;

                // Query base
                var query = _context.Asistencias
                    .Include(a => a.Estudiante)
                    .Include(a => a.Grupo)
                        .ThenInclude(g => g.PeriodoAcademico)
                    .Include(a => a.RegistradoPor)
                    .AsQueryable();

                // Aplicar filtros
                if (grupoId.HasValue)
                {
                    query = query.Where(a => a.GrupoId == grupoId.Value);
                }

                if (fecha.HasValue)
                {
                    query = query.Where(a => a.Fecha.Date == fecha.Value.Date);
                }

                if (!string.IsNullOrEmpty(estado))
                {
                    query = query.Where(a => a.Estado == estado);
                }

                // Ordenar por fecha descendente y luego por estudiante
                query = query.OrderByDescending(a => a.Fecha)
                            .ThenBy(a => a.Estudiante.Apellidos)
                            .ThenBy(a => a.Estudiante.Nombre);

                // Paginación
                var totalRecords = await query.CountAsync();
                var asistencias = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                // Preparar ViewBag para filtros
                await PrepararFiltrosViewBag(grupoId, estado);

                // Información de paginación
                ViewBag.CurrentPage = pageNumber;
                ViewBag.TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);
                ViewBag.TotalRecords = totalRecords;
                ViewBag.FechaFiltro = fecha;

                return View(asistencias);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar la lista de asistencias");
                TempData["Error"] = "Error al cargar los registros de asistencia.";
                return View(new List<Asistencia>());
            }
        }

        /// <summary>
        /// Muestra los detalles de un registro de asistencia
        /// </summary>
        /// <param name="id">ID del registro</param>
        /// <returns></returns>
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var asistencia = await _context.Asistencias
                    .Include(a => a.Estudiante)
                    .Include(a => a.Grupo)
                        .ThenInclude(g => g.PeriodoAcademico)
                    .Include(a => a.RegistradoPor)
                    .Include(a => a.ModificadoPor)
                    .FirstOrDefaultAsync(a => a.AsistenciaId == id);

                if (asistencia == null)
                {
                    return NotFound();
                }

                return View(asistencia);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar los detalles de la asistencia {AsistenciaId}", id);
                TempData["Error"] = "Error al cargar los detalles del registro.";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Muestra el formulario para crear un nuevo registro de asistencia
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Create()
        {
            try
            {
                await PrepararFiltrosViewBag();
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al preparar el formulario de creación");
                TempData["Error"] = "Error al cargar el formulario.";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Procesa la creación de un nuevo registro de asistencia
        /// </summary>
        /// <param name="asistencia">Datos del registro</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EstudianteId,GrupoId,Fecha,Estado,Justificacion,Observaciones,HoraLlegada")] Asistencia asistencia)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Verificar que no exista ya un registro para la misma fecha, estudiante y grupo
                    var existeRegistro = await _context.Asistencias
                        .AnyAsync(a => a.EstudianteId == asistencia.EstudianteId && 
                                      a.GrupoId == asistencia.GrupoId && 
                                      a.Fecha.Date == asistencia.Fecha.Date);

                    if (existeRegistro)
                    {
                        ModelState.AddModelError("", "Ya existe un registro de asistencia para este estudiante en la fecha seleccionada.");
                        await PrepararFiltrosViewBag();
                        return View(asistencia);
                    }

                    asistencia.RegistradoPorId = User.Identity?.Name;
                    asistencia.FechaRegistro = DateTime.Now;

                    _context.Add(asistencia);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Registro de asistencia creado exitosamente.";
                    return RedirectToAction(nameof(Index));
                }

                await PrepararFiltrosViewBag();
                return View(asistencia);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el registro de asistencia");
                TempData["Error"] = "Error al guardar el registro. Intente nuevamente.";
                await PrepararFiltrosViewBag();
                return View(asistencia);
            }
        }

        /// <summary>
        /// Muestra el formulario para editar un registro de asistencia
        /// </summary>
        /// <param name="id">ID del registro</param>
        /// <returns></returns>
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var asistencia = await _context.Asistencias.FindAsync(id);
                if (asistencia == null)
                {
                    return NotFound();
                }

                await PrepararFiltrosViewBag();
                return View(asistencia);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar el formulario de edición para la asistencia {AsistenciaId}", id);
                TempData["Error"] = "Error al cargar el formulario de edición.";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Procesa la actualización de un registro de asistencia
        /// </summary>
        /// <param name="id">ID del registro</param>
        /// <param name="asistencia">Datos actualizados</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AsistenciaId,EstudianteId,GrupoId,Fecha,Estado,Justificacion,Observaciones,HoraLlegada")] Asistencia asistencia)
        {
            if (id != asistencia.AsistenciaId)
            {
                return NotFound();
            }

            try
            {
                if (ModelState.IsValid)
                {
                    var asistenciaOriginal = await _context.Asistencias.AsNoTracking()
                        .FirstOrDefaultAsync(a => a.AsistenciaId == id);

                    if (asistenciaOriginal == null)
                    {
                        return NotFound();
                    }

                    // Verificar duplicados (excluyendo el registro actual)
                    var existeOtroRegistro = await _context.Asistencias
                        .AnyAsync(a => a.AsistenciaId != id &&
                                      a.EstudianteId == asistencia.EstudianteId && 
                                      a.GrupoId == asistencia.GrupoId && 
                                      a.Fecha.Date == asistencia.Fecha.Date);

                    if (existeOtroRegistro)
                    {
                        ModelState.AddModelError("", "Ya existe otro registro de asistencia para este estudiante en la fecha seleccionada.");
                        await PrepararFiltrosViewBag();
                        return View(asistencia);
                    }

                    // Mantener datos originales de registro
                    asistencia.RegistradoPorId = asistenciaOriginal.RegistradoPorId;
                    asistencia.FechaRegistro = asistenciaOriginal.FechaRegistro;
                    
                    // Marcar como modificación
                    asistencia.EsModificacion = true;
                    asistencia.FechaModificacion = DateTime.Now;
                    asistencia.ModificadoPorId = User.Identity?.Name;

                    _context.Update(asistencia);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Registro de asistencia actualizado exitosamente.";
                    return RedirectToAction(nameof(Index));
                }

                await PrepararFiltrosViewBag();
                return View(asistencia);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AsistenciaExists(asistencia.AsistenciaId))
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
                _logger.LogError(ex, "Error al actualizar el registro de asistencia {AsistenciaId}", id);
                TempData["Error"] = "Error al actualizar el registro. Intente nuevamente.";
                await PrepararFiltrosViewBag();
                return View(asistencia);
            }
        }

        /// <summary>
        /// Muestra la confirmación para eliminar un registro
        /// </summary>
        /// <param name="id">ID del registro</param>
        /// <returns></returns>
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var asistencia = await _context.Asistencias
                    .Include(a => a.Estudiante)
                    .Include(a => a.Grupo)
                    .Include(a => a.RegistradoPor)
                    .FirstOrDefaultAsync(a => a.AsistenciaId == id);

                if (asistencia == null)
                {
                    return NotFound();
                }

                return View(asistencia);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar la confirmación de eliminación para la asistencia {AsistenciaId}", id);
                TempData["Error"] = "Error al cargar la confirmación de eliminación.";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Confirma la eliminación de un registro
        /// </summary>
        /// <param name="id">ID del registro</param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var asistencia = await _context.Asistencias.FindAsync(id);
                if (asistencia != null)
                {
                    _context.Asistencias.Remove(asistencia);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Registro de asistencia eliminado exitosamente.";
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el registro de asistencia {AsistenciaId}", id);
                TempData["Error"] = "Error al eliminar el registro. Intente nuevamente.";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Reportes y estadísticas de asistencia
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Reportes()
        {
            try
            {
                var fechaInicio = DateTime.Now.AddDays(-30).Date;
                var fechaFin = DateTime.Now.Date;

                var estadisticas = await _context.Asistencias
                    .Where(a => a.Fecha >= fechaInicio && a.Fecha <= fechaFin)
                    .GroupBy(a => a.Estado)
                    .Select(g => new { Estado = g.Key, Cantidad = g.Count() })
                    .ToListAsync();

                ViewBag.FechaInicio = fechaInicio;
                ViewBag.FechaFin = fechaFin;
                ViewBag.Estadisticas = estadisticas;

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar los reportes de asistencia");
                TempData["Error"] = "Error al cargar los reportes.";
                return RedirectToAction(nameof(Index));
            }
        }

        #region Métodos Privados

        /// <summary>
        /// Prepara los datos para los filtros en ViewBag
        /// </summary>
        private async Task PrepararFiltrosViewBag(int? grupoSeleccionado = null, string? estadoSeleccionado = null)
        {
            // Grupos activos
            var grupos = await _context.GruposEstudiantes
                .Where(g => g.Estado == Models.EstadoGrupo.Activo)
                .Include(g => g.PeriodoAcademico)
                .OrderBy(g => g.Nombre)
                .ToListAsync();

            ViewBag.Grupos = new SelectList(
                grupos.Select(g => new { 
                    Value = g.GrupoId, 
                    Text = $"{g.Nombre} ({(g.PeriodoAcademico?.Nombre ?? "Sin período")})" 
                }),
                "Value", "Text", grupoSeleccionado);

            // Estudiantes activos
            ViewBag.Estudiantes = new SelectList(
                await _context.Estudiantes
                    .OrderBy(e => e.Apellidos)
                    .ThenBy(e => e.Nombre)
                    .Select(e => new { 
                        Value = e.IdEstudiante, 
                        Text = $"{e.Apellidos}, {e.Nombre} ({e.NumeroId})" 
                    })
                    .ToListAsync(),
                "Value", "Text");

            // Estados de asistencia
            ViewBag.Estados = new SelectList(new[]
            {
                new { Value = "P", Text = "Presente" },
                new { Value = "A", Text = "Ausente" },
                new { Value = "T", Text = "Tardanza" },
                new { Value = "AJ", Text = "Ausencia Justificada" }
            }, "Value", "Text", estadoSeleccionado);
        }

        /// <summary>
        /// Verifica si existe un registro de asistencia
        /// </summary>
        private bool AsistenciaExists(int id)
        {
            return _context.Asistencias.Any(e => e.AsistenciaId == id);
        }

        #endregion

        #region Toma de Asistencia Masiva

        /// <summary>
        /// Vista principal para tomar asistencia por grupo y materia
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> TomarAsistencia(int? grupoId, int? materiaId, DateTime? fecha)
        {
            try
            {
                // Cargar grupos para el dropdown
                ViewBag.Grupos = await _context.GruposEstudiantes
                    .Include(g => g.PeriodoAcademico)
                    .Where(g => g.Estado == EstadoGrupo.Activo)
                    .Select(g => new SelectListItem
                    {
                        Value = g.GrupoId.ToString(),
                        Text = $"{g.Nombre} - {g.PeriodoAcademico!.Nombre}"
                    })
                    .ToListAsync();

                // Cargar materias para el dropdown
                ViewBag.Materias = await _context.Materias
                    .Where(m => m.Activa)
                    .Select(m => new SelectListItem
                    {
                        Value = m.MateriaId.ToString(),
                        Text = m.Nombre
                    })
                    .ToListAsync();

                // Si hay grupo y materia seleccionados, cargar estudiantes
                var viewModel = new TomarAsistenciaViewModel
                {
                    GrupoId = grupoId,
                    MateriaId = materiaId,
                    Fecha = fecha ?? DateTime.Today,
                    Estudiantes = new List<EstudianteAsistenciaViewModel>()
                };

                if (grupoId.HasValue && materiaId.HasValue)
                {
                    await CargarEstudiantesAsistencia(viewModel);
                }

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar la vista de tomar asistencia");
                TempData["ErrorMessage"] = "Error al cargar la página de asistencia. Intente nuevamente.";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Carga los estudiantes y su asistencia existente para la fecha seleccionada
        /// </summary>
        /// <param name="viewModel"></param>
        private async Task CargarEstudiantesAsistencia(TomarAsistenciaViewModel viewModel)
        {
            if (!viewModel.GrupoId.HasValue || !viewModel.MateriaId.HasValue)
                return;

            // Obtener estudiantes del grupo
            var estudiantesGrupo = await _context.EstudianteGrupos
                .Include(eg => eg.Estudiante)
                .Where(eg => eg.GrupoId == viewModel.GrupoId.Value && eg.Estado == EstadoAsignacion.Activo)
                .Select(eg => eg.Estudiante)
                .OrderBy(e => e.Apellidos)
                .ThenBy(e => e.Nombre)
                .ToListAsync();

            // Verificar que el grupo tenga asignada la materia
            var grupoMateria = await _context.GrupoMaterias
                .FirstOrDefaultAsync(gm => gm.GrupoId == viewModel.GrupoId.Value && 
                                          gm.MateriaId == viewModel.MateriaId.Value && 
                                          gm.Estado == EstadoAsignacion.Activo);

            if (grupoMateria == null)
            {
                throw new InvalidOperationException("La materia no está asignada a este grupo.");
            }

            // Obtener asistencias existentes para la fecha
            var asistenciasExistentes = await _context.Asistencias
                .Where(a => a.GrupoId == viewModel.GrupoId.Value && 
                           a.MateriaId == viewModel.MateriaId.Value && 
                           a.Fecha.Date == viewModel.Fecha.Date)
                .ToDictionaryAsync(a => a.EstudianteId);

            // Crear lista de estudiantes con su estado de asistencia
            viewModel.Estudiantes = estudiantesGrupo.Select(estudiante => new EstudianteAsistenciaViewModel
            {
                EstudianteId = estudiante.IdEstudiante,
                NombreCompleto = estudiante.NombreCompleto,
                NumeroId = estudiante.NumeroId,
                Estado = asistenciasExistentes.ContainsKey(estudiante.IdEstudiante) 
                    ? asistenciasExistentes[estudiante.IdEstudiante].Estado 
                    : null, // null = neutro
                Justificacion = asistenciasExistentes.ContainsKey(estudiante.IdEstudiante) 
                    ? asistenciasExistentes[estudiante.IdEstudiante].Justificacion 
                    : null,
                AsistenciaId = asistenciasExistentes.ContainsKey(estudiante.IdEstudiante) 
                    ? asistenciasExistentes[estudiante.IdEstudiante].AsistenciaId 
                    : null
            }).ToList();
        }

        /// <summary>
        /// Guarda la asistencia masiva
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GuardarAsistencia(TomarAsistenciaViewModel model)
        {
            try
            {
                if (!model.GrupoId.HasValue || !model.MateriaId.HasValue)
                {
                    ModelState.AddModelError("", "Debe seleccionar un grupo y una materia.");
                    return await TomarAsistencia(model.GrupoId, model.MateriaId, model.Fecha);
                }

                var usuario = User.Identity?.Name ?? "Sistema";
                var fechaActual = DateTime.Now;

                foreach (var estudiante in model.Estudiantes)
                {
                    // Si no hay estado seleccionado, usar estado neutro "N"
                    var estadoFinal = string.IsNullOrEmpty(estudiante.Estado) ? "N" : estudiante.Estado;

                    // Buscar si ya existe un registro de asistencia
                    var asistenciaExistente = await _context.Asistencias
                        .FirstOrDefaultAsync(a => a.EstudianteId == estudiante.EstudianteId &&
                                                 a.GrupoId == model.GrupoId.Value &&
                                                 a.MateriaId == model.MateriaId.Value &&
                                                 a.Fecha.Date == model.Fecha.Date);

                    if (asistenciaExistente != null)
                    {
                        // Actualizar registro existente
                        asistenciaExistente.Estado = estadoFinal;
                        asistenciaExistente.Justificacion = estudiante.Justificacion;
                        asistenciaExistente.EsModificacion = true;
                        asistenciaExistente.FechaModificacion = fechaActual;
                        asistenciaExistente.ModificadoPorId = usuario;
                        
                        _context.Update(asistenciaExistente);
                    }
                    else
                    {
                        // Crear nuevo registro
                        var nuevaAsistencia = new Asistencia
                        {
                            EstudianteId = estudiante.EstudianteId,
                            GrupoId = model.GrupoId.Value,
                            MateriaId = model.MateriaId.Value,
                            Fecha = model.Fecha,
                            Estado = estadoFinal,
                            Justificacion = estudiante.Justificacion,
                            FechaRegistro = fechaActual,
                            RegistradoPorId = usuario,
                            EsModificacion = false
                        };

                        _context.Add(nuevaAsistencia);
                    }
                }

                await _context.SaveChangesAsync();

                var grupo = await _context.GruposEstudiantes.FindAsync(model.GrupoId.Value);
                var materia = await _context.Materias.FindAsync(model.MateriaId.Value);

                TempData["SuccessMessage"] = $"Asistencia guardada correctamente para el grupo {grupo?.Nombre} en la materia {materia?.Nombre} el {model.Fecha:dd/MM/yyyy}.";
                
                return RedirectToAction(nameof(TomarAsistencia), new { 
                    grupoId = model.GrupoId, 
                    materiaId = model.MateriaId, 
                    fecha = model.Fecha 
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar la asistencia masiva");
                ModelState.AddModelError("", "Error al guardar la asistencia. Intente nuevamente.");
                return await TomarAsistencia(model.GrupoId, model.MateriaId, model.Fecha);
            }
        }

        /// <summary>
        /// Obtiene estudiantes por grupo vía AJAX
        /// </summary>
        /// <param name="grupoId"></param>
        /// <param name="materiaId"></param>
        /// <param name="fecha"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> ObtenerEstudiantes(int grupoId, int materiaId, DateTime fecha)
        {
            try
            {
                var viewModel = new TomarAsistenciaViewModel
                {
                    GrupoId = grupoId,
                    MateriaId = materiaId,
                    Fecha = fecha
                };

                await CargarEstudiantesAsistencia(viewModel);

                return PartialView("_TablaEstudiantes", viewModel.Estudiantes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener estudiantes para asistencia");
                return Json(new { error = "Error al cargar estudiantes" });
            }
        }

        #endregion
    }
}