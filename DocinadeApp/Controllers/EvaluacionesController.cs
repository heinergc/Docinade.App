using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DocinadeApp.Data;
using DocinadeApp.Models;
using DocinadeApp.Services;
using DocinadeApp.ViewModels;
using DocinadeApp.Extensions;
using DocinadeApp.DTOs;
using System.ComponentModel.DataAnnotations;
using System.Text.Json; // AGREGADO para JsonSerializerOptions
using Microsoft.AspNetCore.Authorization;

namespace DocinadeApp.Controllers
{
    [Authorize]
    public class EvaluacionesController : Controller
    {
        private readonly RubricasDbContext _context;
        private readonly IEmailService _emailService;
        private readonly ILogger<EvaluacionesController> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment; // AGREGADO

        public EvaluacionesController(
            RubricasDbContext context, 
            IEmailService emailService, 
            ILogger<EvaluacionesController> logger,
            IWebHostEnvironment webHostEnvironment) // AGREGADO
        {
            _context = context;
            _emailService = emailService;
            _logger = logger;
            _webHostEnvironment = webHostEnvironment; // AGREGADO
        }

        // GET: Evaluaciones - SIMPLIFICADO con filtros básicos
        public async Task<IActionResult> Index(
            int? estudianteId,
            int? rubricaId,
            int? periodoId,
            bool showAll = false,
            string sortOrder = "")
        {
            try
            {
                // Parámetros de ordenamiento para la vista
                ViewBag.CurrentSort = sortOrder;
                ViewBag.NombreSortParam = string.IsNullOrEmpty(sortOrder) ? "nombre_desc" : "";
                ViewBag.ApellidosSortParam = sortOrder == "apellidos" ? "apellidos_desc" : "apellidos";
                ViewBag.FechaSortParam = sortOrder == "fecha" ? "fecha_desc" : "fecha";

                var filtroViewModel = new EvaluacionesFiltroViewModel
                {
                    EstudianteId = estudianteId,
                    RubricaId = rubricaId,
                    PeriodoId = periodoId,
                    ShowAll = showAll
                };

                List<Evaluacion> evaluaciones;

                if (showAll)
                {
                    var query = _context.Evaluaciones
                        .Include(e => e.Estudiante)
                            .ThenInclude(est => est.PeriodoAcademico)
                        .Include(e => e.Rubrica)
                        .Include(e => e.DetallesEvaluacion)
                        .AsQueryable();

                    // Aplicar ordenamiento
                    query = ApplyOrdering(query, sortOrder);

                    evaluaciones = await query
                        .Take(50)
                        .ToListAsync();

                    await CargarListasVaciasParaShowAll(filtroViewModel);
                }
                else
                {
                    var query = _context.Evaluaciones
                        .Include(e => e.Estudiante)
                            .ThenInclude(est => est.PeriodoAcademico)
                        .Include(e => e.Rubrica)
                        .Include(e => e.DetallesEvaluacion)
                        .AsQueryable();

                    // Aplicar filtros simples
                    if (estudianteId.HasValue && estudianteId.Value > 0)
                        query = query.Where(e => e.IdEstudiante == estudianteId.Value);

                    if (rubricaId.HasValue && rubricaId.Value > 0)
                        query = query.Where(e => e.IdRubrica == rubricaId.Value);

                    if (periodoId.HasValue && periodoId.Value > 0)
                        query = query.Where(e => e.Estudiante.PeriodoAcademicoId == periodoId.Value);

                    // Aplicar ordenamiento
                    query = ApplyOrdering(query, sortOrder);

                    evaluaciones = await query.ToListAsync();

                    await CargarListasFiltros(filtroViewModel);
                }

                // Cargar información adicional para mostrar en la tabla (simplificada)
                await CargarInformacionAdicionalEvaluaciones(evaluaciones, filtroViewModel);

                ViewBag.Estudiantes = filtroViewModel.Estudiantes.ToList();
                ViewBag.Rubricas = filtroViewModel.Rubricas.ToList();
                ViewBag.Periodos = filtroViewModel.Periodos.ToList();
                ViewBag.EstudianteSeleccionado = estudianteId;
                ViewBag.RubricaSeleccionada = rubricaId;
                ViewBag.PeriodoSeleccionado = periodoId;
                ViewBag.FiltroViewModel = filtroViewModel;
                ViewBag.ShowAll = showAll;

                return View(evaluaciones);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar evaluaciones");
                TempData["ErrorMessage"] = "Error al cargar las evaluaciones";

                ViewBag.Estudiantes = new List<SelectListItem>();
                ViewBag.Rubricas = new List<SelectListItem>();
                ViewBag.Periodos = new List<SelectListItem>();
                ViewBag.FiltroViewModel = new EvaluacionesFiltroViewModel();
                ViewBag.ShowAll = false;

                return View(new List<Evaluacion>());
            }
        }

        // Endpoints AJAX para filtros en cascada - CORREGIDOS Y MEJORADOS
        [HttpGet]
        public async Task<JsonResult> GetEstudiantesByGrupo(int grupoId)
        {
            try
            {
                _logger.LogInformation($"?? Iniciando búsqueda de estudiantes para grupo {grupoId}");

                if (grupoId <= 0)
                {
                    _logger.LogWarning("?? GrupoId inválido: {grupoId}", grupoId);
                    return Json(new List<SelectListItem>());
                }

                // Primero verificar si el grupo existe
                var grupoExiste = await _context.GruposEstudiantes.AnyAsync(g => g.GrupoId == grupoId);
                if (!grupoExiste)
                {
                    _logger.LogWarning($"?? No se encontró el grupo con ID {grupoId}");
                    return Json(new List<SelectListItem>());
                }

                // Consulta simplificada paso a paso para debug
                var estudianteGrupos = await _context.EstudianteGrupos
                    .Where(eg => eg.GrupoId == grupoId)
                    .ToListAsync();

                _logger.LogInformation($"?? Total relaciones EstudianteGrupo para grupo {grupoId}: {estudianteGrupos.Count}");

                // Filtrar solo los activos
                var estudianteGruposActivos = estudianteGrupos
                    .Where(eg => eg.Estado == EstadoAsignacion.Activo)
                    .ToList();

                _logger.LogInformation($"?? Relaciones EstudianteGrupo activas: {estudianteGruposActivos.Count}");

                if (!estudianteGruposActivos.Any())
                {
                    _logger.LogWarning($"?? No hay estudiantes activos asignados al grupo {grupoId}");
                    return Json(new List<SelectListItem>());
                }

                // Obtener IDs de estudiantes activos
                var estudiantesIds = estudianteGruposActivos.Select(eg => eg.EstudianteId).ToList();
                _logger.LogInformation($"?? IDs de estudiantes a buscar: [{string.Join(", ", estudiantesIds)}]");

                // CORRECCIÓN: Primero obtener los datos sin formatear desde la base de datos
                var estudiantesData = await _context.Estudiantes
                    .Where(e => estudiantesIds.Contains(e.IdEstudiante))
                    .Select(e => new {
                        IdEstudiante = e.IdEstudiante,
                        Apellidos = e.Apellidos,
                        Nombre = e.Nombre,
                        NumeroId = e.NumeroId
                    })
                    .OrderBy(e => e.Apellidos)
                    .ThenBy(e => e.Nombre)
                    .ToListAsync();

                // CORRECCIÓN: Luego formatear los datos en memoria (lado del cliente)
                var estudiantes = estudiantesData
                    .Select(e => new SelectListItem
                    {
                        Value = e.IdEstudiante.ToString(),
                        Text = $"{e.Apellidos}, {e.Nombre} ({e.NumeroId})"
                    })
                    .ToList();

                _logger.LogInformation($"? Estudiantes encontrados y formateados: {estudiantes.Count}");

                if (estudiantes.Any())
                {
                    _logger.LogInformation($"?? Primeros estudiantes: {string.Join("; ", estudiantes.Take(3).Select(e => e.Text))}");
                }

                return Json(estudiantes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "? Error crítico al obtener estudiantes por grupo {grupoId}", grupoId);
                return Json(new { 
                    error = true, 
                    message = "Error al cargar estudiantes", 
                    details = ex.Message 
                });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetMateriasByGrupo(int grupoId)
        {
            try
            {
                _logger.LogInformation($"Obteniendo materias para grupo {grupoId}");

                if (grupoId <= 0)
                {
                    _logger.LogWarning("GrupoId inv?lido: {grupoId}", grupoId);
                    return Json(new List<SelectListItem>());
                }

                // CORRECCIÓN: Primero obtener los datos sin formatear
                var materiasData = await _context.GrupoMaterias
                    .Where(gm => gm.GrupoId == grupoId && gm.Estado == EstadoAsignacion.Activo)
                    .Include(gm => gm.Materia)
                    .Select(gm => new {
                        MateriaId = gm.MateriaId,
                        Codigo = gm.Materia.Codigo,
                        Nombre = gm.Materia.Nombre
                    })
                    .ToListAsync();

                // CORRECCIÓN: Luego formatear en memoria
                var materias = materiasData
                    .Select(m => new SelectListItem
                    {
                        Value = m.MateriaId.ToString(),
                        Text = $"{m.Codigo} - {m.Nombre}"
                    })
                    .OrderBy(m => m.Text)
                    .ToList();

                _logger.LogInformation($"Encontradas {materias.Count} materias para grupo {grupoId}");
                return Json(materias);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener materias por grupo {grupoId}", grupoId);
                return Json(new List<SelectListItem>());
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetInstrumentosByMateria(int materiaId)
        {
            try
            {
                _logger.LogInformation($"Obteniendo instrumentos para materia {materiaId}");

                if (materiaId <= 0)
                {
                    _logger.LogWarning("MateriaId inválido: {materiaId}", materiaId);
                    return Json(new List<SelectListItem>());
                }

                var instrumentos = await _context.InstrumentoMaterias
                    .Where(im => im.MateriaId == materiaId)
                    .Include(im => im.InstrumentoEvaluacion)
                    .Where(im => im.InstrumentoEvaluacion.Activo)
                    .Select(im => new SelectListItem
                    {
                        Value = im.InstrumentoEvaluacionId.ToString(),
                        Text = im.InstrumentoEvaluacion.Nombre
                    })
                    .OrderBy(i => i.Text)
                    .ToListAsync();

                _logger.LogInformation($"? Encontrados {instrumentos.Count} instrumentos para materia {materiaId}");
                return Json(instrumentos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "? Error al obtener instrumentos por materia {materiaId}", materiaId);
                return Json(new List<SelectListItem>());
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetRubricasByInstrumento(int instrumentoEvaluacionId)
        {
            try
            {
                _logger.LogInformation($"Obteniendo rúbricas para instrumento {instrumentoEvaluacionId}");

                if (instrumentoEvaluacionId <= 0)
                {
                    _logger.LogWarning("InstrumentoEvaluacionId inválido: {instrumentoEvaluacionId}", instrumentoEvaluacionId);
                    return Json(new List<SelectListItem>());
                }

                var rubricas = await _context.InstrumentoRubricas
                    .Where(ir => ir.InstrumentoEvaluacionId == instrumentoEvaluacionId)
                    .Include(ir => ir.Rubrica)
                    .Where(ir => ir.Rubrica.Vigente)
                    .Select(ir => new SelectListItem
                    {
                        Value = ir.RubricaId.ToString(),
                        Text = ir.Rubrica.NombreRubrica
                    })
                    .OrderBy(r => r.Text)
                    .ToListAsync();

                _logger.LogInformation($"? Encontradas {rubricas.Count} r?bricas para instrumento {instrumentoEvaluacionId}");
                return Json(rubricas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "? Error al obtener r?bricas por instrumento {instrumentoEvaluacionId}", instrumentoEvaluacionId);
                return Json(new List<SelectListItem>());
            }
        }

        // M?todos antiguos mantenidos para compatibilidad - DEPRECATED
        [HttpGet]
        public async Task<JsonResult> OnGetEstudiantesByGrupoAsync(int grupoId)
        {
            return await GetEstudiantesByGrupo(grupoId);
        }

        [HttpGet]
        public async Task<JsonResult> OnGetMateriasByGrupoAsync(int grupoId)
        {
            return await GetMateriasByGrupo(grupoId);
        }

        [HttpGet]
        public async Task<JsonResult> OnGetInstrumentosByMateriaAsync(int materiaId)
        {
            return await GetInstrumentosByMateria(materiaId);
        }

        [HttpGet]
        public async Task<JsonResult> OnGetRubricasByInstrumentoAsync(int instrumentoEvaluacionId)
        {
            return await GetRubricasByInstrumento(instrumentoEvaluacionId);
        }

        // Método auxiliar para aplicar ordenamiento a las evaluaciones
        private IQueryable<Evaluacion> ApplyOrdering(IQueryable<Evaluacion> query, string sortOrder)
        {
            switch (sortOrder)
            {
                case "nombre_desc":
                    return query.OrderByDescending(e => e.Estudiante.Nombre).ThenByDescending(e => e.Estudiante.Apellidos);
                case "apellidos":
                    return query.OrderBy(e => e.Estudiante.Apellidos).ThenBy(e => e.Estudiante.Nombre);
                case "apellidos_desc":
                    return query.OrderByDescending(e => e.Estudiante.Apellidos).ThenByDescending(e => e.Estudiante.Nombre);
                case "fecha":
                    return query.OrderBy(e => e.FechaEvaluacion);
                case "fecha_desc":
                    return query.OrderByDescending(e => e.FechaEvaluacion);
                default: // Por defecto ordenar por nombre ascendente
                    return query.OrderBy(e => e.Estudiante.Nombre).ThenBy(e => e.Estudiante.Apellidos);
            }
        }

        // Métodos auxiliares para filtros - SIMPLIFICADOS
        private async Task CargarListasFiltros(EvaluacionesFiltroViewModel filtros)
        {
            // Cargar TODOS los estudiantes (sin filtros en cascada)
            var estudiantesData = await _context.Estudiantes
                .Select(e => new {
                    IdEstudiante = e.IdEstudiante,
                    Apellidos = e.Apellidos,
                    Nombre = e.Nombre,
                    NumeroId = e.NumeroId
                })
                .OrderBy(e => e.Apellidos)
                .ThenBy(e => e.Nombre)
                .ToListAsync();

            filtros.Estudiantes = estudiantesData
                .Select(e => new SelectListItem
                {
                    Value = e.IdEstudiante.ToString(),
                    Text = $"{e.Apellidos}, {e.Nombre} ({e.NumeroId})"
                })
                .ToList();

            // Cargar TODAS las rúbricas
            filtros.Rubricas = await _context.Rubricas
                .Where(r => r.Vigente)
                .OrderBy(r => r.NombreRubrica)
                .Select(r => new SelectListItem
                {
                    Value = r.IdRubrica.ToString(),
                    Text = r.NombreRubrica
                })
                .ToListAsync();

            // Cargar TODOS los períodos
            filtros.Periodos = await _context.PeriodosAcademicos
                .OrderBy(p => p.Anio).ThenBy(p => p.NumeroPeriodo)
                .Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.Nombre
                })
                .ToListAsync();

            // REMOVIDO: Listas de cascada que ya no se usan
            // filtros.Grupos = new List<SelectListItem>();
            // filtros.Materias = new List<SelectListItem>();
            // filtros.Instrumentos = new List<SelectListItem>();
        }

        private async Task CargarListasVaciasParaShowAll(EvaluacionesFiltroViewModel filtros)
        {
            filtros.Estudiantes = new List<SelectListItem>
            {
                new SelectListItem { Value = "", Text = "Modo Ver Todo Activo" }
            };
            filtros.Rubricas = new List<SelectListItem>
            {
                new SelectListItem { Value = "", Text = "Modo Ver Todo Activo" }
            };

            filtros.Periodos = await _context.PeriodosAcademicos
                .OrderBy(p => p.Anio).ThenBy(p => p.NumeroPeriodo)
                .Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.Nombre
                })
                .ToListAsync();

            // REMOVIDO: Listas de cascada que ya no se usan
            // filtros.Grupos = new List<SelectListItem>();
            // filtros.Materias = new List<SelectListItem>();
            // filtros.Instrumentos = new List<SelectListItem>();
        }

        // M?todos CRUD originales
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var evaluacion = await _context.Evaluaciones
                .Include(e => e.Estudiante)
                    .ThenInclude(est => est.PeriodoAcademico)
                .Include(e => e.Rubrica)
                .Include(e => e.DetallesEvaluacion)
                    .ThenInclude(d => d.ItemEvaluacion)
                .Include(e => e.DetallesEvaluacion)
                    .ThenInclude(d => d.NivelCalificacion)
                .FirstOrDefaultAsync(m => m.IdEvaluacion == id);

            if (evaluacion == null)
                return NotFound();

            return View(evaluacion);
        }

        public async Task<IActionResult> Create(int? estudianteId, int? rubricaId)
        {
            await CargarDatosParaFormulario();

            var viewModel = new CrearEvaluacionViewModel();

            if (estudianteId.HasValue)
                viewModel.IdEstudiante = estudianteId.Value;

            if (rubricaId.HasValue)
            {
                viewModel.IdRubrica = rubricaId.Value;
                await CargarDatosRubrica(viewModel, rubricaId.Value);
            }

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CrearEvaluacionViewModel viewModel, string submitAction = "guardarCompleta")
        {
            bool esBorrador = submitAction == "guardarBorrador";

            if (esBorrador)
            {
                ModelState.Clear();
                if (viewModel.IdEstudiante <= 0)
                    ModelState.AddModelError("IdEstudiante", "Debe seleccionar un estudiante.");
                if (viewModel.IdRubrica <= 0)
                    ModelState.AddModelError("IdRubrica", "Debe seleccionar una r?brica.");
            }

            if (ModelState.IsValid || esBorrador)
            {
                try
                {
                    var evaluacionExistente = await _context.Evaluaciones
                        .AnyAsync(e => e.IdEstudiante == viewModel.IdEstudiante &&
                                      e.IdRubrica == viewModel.IdRubrica);

                    if (evaluacionExistente)
                    {
                        ModelState.AddModelError("", "Ya existe una evaluacion para este estudiante con esta rubrica.");
                    }
                    else
                    {
                        var evaluacion = new Evaluacion
                        {
                            IdEstudiante = viewModel.IdEstudiante,
                            IdRubrica = viewModel.IdRubrica,
                            FechaEvaluacion = viewModel.FechaEvaluacion,
                            Observaciones = viewModel.Observaciones,
                            Estado = esBorrador ? "BORRADOR" : "COMPLETADA"
                        };

                        _context.Evaluaciones.Add(evaluacion);
                        await _context.SaveChangesAsync();

                        decimal totalPuntos = 0;
                        foreach (var detalle in viewModel.DetallesEvaluacion)
                        {
                            if (esBorrador && detalle.IdNivel == 0)
                                continue;

                            var valorRubrica = await _context.ValoresRubrica
                                .FirstOrDefaultAsync(vr => vr.IdRubrica == viewModel.IdRubrica &&
                                                          vr.IdItem == detalle.IdItem &&
                                                          vr.IdNivel == detalle.IdNivel);

                            if (valorRubrica != null)
                            {
                                var detalleEvaluacion = new DetalleEvaluacion
                                {
                                    IdEvaluacion = evaluacion.IdEvaluacion,
                                    IdItem = detalle.IdItem,
                                    IdNivel = detalle.IdNivel,
                                    PuntosObtenidos = valorRubrica.ValorPuntos
                                };

                                _context.DetallesEvaluacion.Add(detalleEvaluacion);
                                totalPuntos += valorRubrica.ValorPuntos;
                            }
                        }

                        evaluacion.TotalPuntos = totalPuntos;
                        _context.Update(evaluacion);
                        await _context.SaveChangesAsync();

                        string mensaje = esBorrador
                            ? "Evaluacion guardada como borrador exitosamente."
                            : $"Evaluacion completada exitosamente. Total de puntos: {totalPuntos:F2}";

                        TempData["SuccessMessage"] = mensaje;
                        return RedirectToAction(nameof(Details), new { id = evaluacion.IdEvaluacion });
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al crear evaluaci?n");
                    ModelState.AddModelError("", "Error al crear la evaluaci?n.");
                }
            }

            await CargarDatosParaFormulario();
            await CargarDatosRubrica(viewModel, viewModel.IdRubrica);
            return View(viewModel);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var evaluacion = await _context.Evaluaciones
                .Include(e => e.DetallesEvaluacion)
                .Include(e => e.Estudiante)
                .Include(e => e.Rubrica)
                .FirstOrDefaultAsync(e => e.IdEvaluacion == id);

            if (evaluacion == null)
                return NotFound();

            var viewModel = new EditarEvaluacionViewModel
            {
                IdEvaluacion = evaluacion.IdEvaluacion,
                IdEstudiante = evaluacion.IdEstudiante,
                IdRubrica = evaluacion.IdRubrica,
                FechaEvaluacion = evaluacion.FechaEvaluacion,
                Observaciones = evaluacion.Observaciones,
                DetallesEvaluacion = evaluacion.DetallesEvaluacion.Select(d => new DetalleEvaluacionViewModel
                {
                    IdItem = d.IdItem,
                    IdNivel = d.IdNivel
                }).ToList()
            };

            // Cargar nombres para mostrar en la vista
            ViewBag.EstudianteNombre = $"{evaluacion.Estudiante.Apellidos}, {evaluacion.Estudiante.Nombre}";
            ViewBag.RubricaNombre = evaluacion.Rubrica.NombreRubrica;

            await CargarDatosParaFormulario();
            await CargarDatosRubrica(viewModel, evaluacion.IdRubrica);

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditarEvaluacionViewModel viewModel, string submitAction)
        {
            if (id != viewModel.IdEvaluacion)
                return NotFound();

            bool esBorrador = submitAction == "guardarBorrador";

            if (esBorrador)
            {
                ModelState.Clear();
                if (viewModel.IdEstudiante <= 0)
                    ModelState.AddModelError("IdEstudiante", "Debe seleccionar un estudiante.");
                if (viewModel.IdRubrica <= 0)
                    ModelState.AddModelError("IdRubrica", "Debe seleccionar una rubrica.");
            }

            if (ModelState.IsValid || esBorrador)
            {
                try
                {
                    var evaluacion = await _context.Evaluaciones
                        .Include(e => e.DetallesEvaluacion)
                        .FirstOrDefaultAsync(e => e.IdEvaluacion == id);

                    if (evaluacion == null)
                        return NotFound();

                    evaluacion.FechaEvaluacion = viewModel.FechaEvaluacion;
                    evaluacion.Observaciones = viewModel.Observaciones;
                    evaluacion.Estado = esBorrador ? "BORRADOR" : "COMPLETADA";

                    _context.DetallesEvaluacion.RemoveRange(evaluacion.DetallesEvaluacion);

                    decimal totalPuntos = 0;
                    foreach (var detalle in viewModel.DetallesEvaluacion)
                    {
                        if (esBorrador && detalle.IdNivel == 0)
                            continue;

                        var valorRubrica = await _context.ValoresRubrica
                            .FirstOrDefaultAsync(vr => vr.IdRubrica == viewModel.IdRubrica &&
                                                      vr.IdItem == detalle.IdItem &&
                                                      vr.IdNivel == detalle.IdNivel);

                        if (valorRubrica != null)
                        {
                            var detalleEvaluacion = new DetalleEvaluacion
                            {
                                IdEvaluacion = evaluacion.IdEvaluacion,
                                IdItem = detalle.IdItem,
                                IdNivel = detalle.IdNivel,
                                PuntosObtenidos = valorRubrica.ValorPuntos
                            };

                            _context.DetallesEvaluacion.Add(detalleEvaluacion);
                            totalPuntos += valorRubrica.ValorPuntos;
                        }
                    }

                    evaluacion.TotalPuntos = totalPuntos;
                    await _context.SaveChangesAsync();

                    string mensaje = esBorrador
                        ? "Evaluacion guardada como borrador exitosamente."
                        : $"Evaluacion actualizada exitosamente. Total de puntos: {totalPuntos:F2}";

                    TempData["SuccessMessage"] = mensaje;
                    return RedirectToAction(nameof(Details), new { id = evaluacion.IdEvaluacion });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al actualizar evaluacion");
                    ModelState.AddModelError("", "Error al actualizar la evaluacion.");
                }
            }

            // Cargar nombres para mostrar en la vista en caso de error
            var estudiante = await _context.Estudiantes.FindAsync(viewModel.IdEstudiante);
            var rubrica = await _context.Rubricas.FindAsync(viewModel.IdRubrica);
            
            if (estudiante != null)
                ViewBag.EstudianteNombre = $"{estudiante.Apellidos}, {estudiante.Nombre}";
            
            if (rubrica != null)
                ViewBag.RubricaNombre = rubrica.NombreRubrica;

            await CargarDatosParaFormulario();
            await CargarDatosRubrica(viewModel, viewModel.IdRubrica);
            return View(viewModel);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var evaluacion = await _context.Evaluaciones
                .Include(e => e.Estudiante)
                .Include(e => e.Rubrica)
                .Include(e => e.DetallesEvaluacion)
                    .ThenInclude(d => d.ItemEvaluacion)
                .Include(e => e.DetallesEvaluacion)
                    .ThenInclude(d => d.NivelCalificacion)
                .FirstOrDefaultAsync(m => m.IdEvaluacion == id);

            if (evaluacion == null)
                return NotFound();

            return View(evaluacion);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var evaluacion = await _context.Evaluaciones
                .Include(e => e.DetallesEvaluacion)
                .FirstOrDefaultAsync(e => e.IdEvaluacion == id);

            if (evaluacion != null)
            {
                try
                {
                    _context.DetallesEvaluacion.RemoveRange(evaluacion.DetallesEvaluacion);
                    _context.Evaluaciones.Remove(evaluacion);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Evaluaci?n eliminada exitosamente.";
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al eliminar evaluacion");
                    TempData["ErrorMessage"] = "Error al eliminar la evaluacion.";
                }
            }

            return RedirectToAction(nameof(Index));
        }

        // M?todos auxiliares
        private async Task CargarDatosParaFormulario()
        {
            // CORRECCIÓN: Primero obtener los datos sin formatear 
            var estudiantesData = await _context.Estudiantes
                .Select(e => new {
                    IdEstudiante = e.IdEstudiante,
                    Apellidos = e.Apellidos,
                    Nombre = e.Nombre,
                    NumeroId = e.NumeroId
                })
                .OrderBy(e => e.Apellidos)
                .ThenBy(e => e.Nombre)
                .ToListAsync();

            // CORRECCIÓN: Luego formatear en memoria
            ViewBag.Estudiantes = estudiantesData
                .Select(e => new SelectListItem
                {
                    Value = e.IdEstudiante.ToString(),
                    Text = $"{e.Apellidos}, {e.Nombre} ({e.NumeroId})"
                })
                .ToList();

            var rubricasData = await _context.Rubricas
                .OrderBy(r => r.NombreRubrica)
                .Select(r => new { r.IdRubrica, r.NombreRubrica, r.Descripcion })
                .ToListAsync();

            ViewBag.Rubricas = rubricasData
                .Select(r => new SelectListItem
                {
                    Value = r.IdRubrica.ToString(),
                    Text = r.NombreRubrica
                })
                .ToList();

            ViewBag.RubricasDescripciones = rubricasData
                .Where(r => !string.IsNullOrWhiteSpace(r.Descripcion))
                .ToDictionary(r => r.IdRubrica, r => r.Descripcion!);
        }

        private async Task CargarDatosRubrica(CrearEvaluacionViewModel viewModel, int rubricaId)
        {
            var items = await _context.ItemsEvaluacion
                .Where(i => i.IdRubrica == rubricaId)
                .OrderBy(i => i.OrdenItem ?? int.MaxValue)
                .ToListAsync();

            var niveles = await _context.ValoresRubrica
                .Where(vr => vr.IdRubrica == rubricaId)
                .Include(vr => vr.NivelCalificacion)
                .ToListAsync();

            ViewBag.Items = items;
            ViewBag.Niveles = niveles;

            if (viewModel.DetallesEvaluacion == null || !viewModel.DetallesEvaluacion.Any())
            {
                viewModel.DetallesEvaluacion = items.Select(item => new DetalleEvaluacionViewModel
                {
                    IdItem = item.IdItem,
                    IdNivel = 0
                }).ToList();
            }
        }

        private async Task CargarDatosRubrica(EditarEvaluacionViewModel viewModel, int rubricaId)
        {
            var items = await _context.ItemsEvaluacion
                .Where(i => i.IdRubrica == rubricaId)
                .OrderBy(i => i.OrdenItem ?? int.MaxValue)
                .ToListAsync();

            var niveles = await _context.ValoresRubrica
                .Where(vr => vr.IdRubrica == rubricaId)
                .Include(vr => vr.NivelCalificacion)
                .ToListAsync();

            ViewBag.Items = items;
            ViewBag.Niveles = niveles;

            if (viewModel.DetallesEvaluacion == null || !viewModel.DetallesEvaluacion.Any())
            {
                viewModel.DetallesEvaluacion = items.Select(item => new DetalleEvaluacionViewModel
                {
                    IdItem = item.IdItem,
                    IdNivel = 0
                }).ToList();
            }
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerItemsPorRubrica(int rubricaId)
        {
            try
            {
                var items = await _context.ItemsEvaluacion
                    .Where(i => i.IdRubrica == rubricaId)
                    .OrderBy(i => i.OrdenItem ?? int.MaxValue)
                    .Select(i => new ItemEvaluacionDto(
                        i.IdItem,
                        i.NombreItem,
                        i.Descripcion,
                        i.Peso,
                        i.OrdenItem ?? 0
                    ))
                    .ToListAsync();

                var nivelesUnicos = await _context.NivelesCalificacion
                    .Where(nc => _context.ValoresRubrica
                        .Any(vr => vr.IdRubrica == rubricaId && vr.IdNivel == nc.IdNivel))
                    .OrderBy(nc => nc.OrdenNivel ?? int.MaxValue)
                    .ToListAsync();

                var valoresRubrica = await _context.ValoresRubrica
                    .Where(vr => vr.IdRubrica == rubricaId)
                    .ToListAsync();

                var niveles = new List<ValorNivelDto>();
                foreach (var item in items)
                {
                    foreach (var nivel in nivelesUnicos)
                    {
                        var valor = valoresRubrica
                            .FirstOrDefault(vr => vr.IdItem == item.IdItem && vr.IdNivel == nivel.IdNivel);

                        if (valor != null)
                        {
                            niveles.Add(new ValorNivelDto(
                                nivel.IdNivel,
                                nivel.NombreNivel,
                                item.IdItem,
                                valor.ValorPuntos
                            ));
                        }
                    }
                }

                var response = new RubricaItemsResponse(items, niveles);
                return Json(response, new System.Text.Json.JsonSerializerOptions
                {
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                    PropertyNamingPolicy = null
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener items por rubrica");
                return Json(new { error = ex.Message });
            }
        }

        // Métodos de envío de correos
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EnviarEvaluacion(int evaluacionId)
        {
            try
            {
                var evaluacion = await _context.Evaluaciones
                    .Include(e => e.Estudiante)
                        .ThenInclude(est => est.PeriodoAcademico)
                    .Include(e => e.Rubrica)
                    .Include(e => e.DetallesEvaluacion)
                        .ThenInclude(d => d.ItemEvaluacion)
                    .Include(e => e.DetallesEvaluacion)
                        .ThenInclude(d => d.NivelCalificacion)
                    .FirstOrDefaultAsync(e => e.IdEvaluacion == evaluacionId);

                if (evaluacion == null)
                {
                    return Json(new { success = false, message = "Evaluacion no encontrada" });
                }

                if (string.IsNullOrEmpty(evaluacion.Estudiante.DireccionCorreo))
                {
                    return Json(new { success = false, message = $"El estudiante {evaluacion.Estudiante.NombreCompleto} no tiene direccion de correo configurada" });
                }

                var enviado = await EnviarCorreoReal(evaluacion);

                if (!enviado)
                {
                    return Json(new { success = false, message = $"No se pudo enviar el correo a {evaluacion.Estudiante.DireccionCorreo}. Verifique la configuración SMTP." });
                }

                return Json(new
                {
                    success = true,
                    message = $"Evaluacion enviada exitosamente a {evaluacion.Estudiante.NombreCompleto} ({evaluacion.Estudiante.DireccionCorreo})"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al enviar evaluacion");
                return Json(new { success = false, message = $"Error al enviar evaluacion: {ex.Message}" });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EnviarTodasEvaluaciones(int? estudianteId, int? rubricaId, int? periodoId)
        {
            try
            {
                var query = _context.Evaluaciones
                    .Include(e => e.Estudiante)
                        .ThenInclude(est => est.PeriodoAcademico)
                    .Include(e => e.Rubrica)
                    .Include(e => e.DetallesEvaluacion)
                        .ThenInclude(d => d.ItemEvaluacion)
                    .Include(e => e.DetallesEvaluacion)
                        .ThenInclude(d => d.NivelCalificacion)
                    .Where(e => e.Estado == "COMPLETADA")
                    .AsQueryable();

                if (estudianteId.HasValue)
                    query = query.Where(e => e.IdEstudiante == estudianteId.Value);

                if (rubricaId.HasValue)
                    query = query.Where(e => e.IdRubrica == rubricaId.Value);

                if (periodoId.HasValue)
                    query = query.Where(e => e.Estudiante.PeriodoAcademicoId == periodoId.Value);

                var evaluaciones = await query.ToListAsync();

                if (!evaluaciones.Any())
                {
                    return Json(new { success = false, message = "No hay evaluaciones completadas para enviar con los filtros aplicados" });
                }

                int enviadosExitosos = 0;
                var errores = new List<string>();

                foreach (var evaluacion in evaluaciones)
                {
                    try
                    {
                        if (string.IsNullOrEmpty(evaluacion.Estudiante.DireccionCorreo))
                        {
                            errores.Add($"Error enviando a {evaluacion.Estudiante.NombreCompleto}: No tiene direccion de correo");
                            continue;
                        }

                        var enviado = await EnviarCorreoReal(evaluacion);
                        if (enviado)
                        {
                            enviadosExitosos++;
                        }
                        else
                        {
                            errores.Add($"Error enviando a {evaluacion.Estudiante.NombreCompleto}: No se pudo enviar el correo");
                        }
                    }
                    catch (Exception ex)
                    {
                        errores.Add($"Error enviando a {evaluacion.Estudiante.NombreCompleto}: {ex.Message}");
                    }
                }

                string mensaje = $"Proceso completado: {enviadosExitosos} evaluaciones enviadas exitosamente";
                if (errores.Any())
                {
                    mensaje += $", {errores.Count} errores";
                }

                return Json(new
                {
                    success = true,
                    message = mensaje,
                    detalles = new
                    {
                        totalEvaluaciones = evaluaciones.Count,
                        enviadosExitosos = enviadosExitosos,
                        errores = errores
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al enviar evaluaciones masivas");
                return Json(new { success = false, message = $"Error general: {ex.Message}" });
            }
        }

        private async Task<bool> EnviarCorreoReal(Evaluacion evaluacion)
        {
            try
            {
                var contenidoCorreo = GenerarContenidoCorreo(evaluacion);
                var asunto = $"Resultado de Evaluación - {evaluacion.Rubrica.NombreRubrica}";

                var resultado = await _emailService.SendEmailAsync(
                    to: evaluacion.Estudiante.DireccionCorreo,
                    subject: asunto,
                    body: contenidoCorreo,
                    isHtml: true
                );

                if (resultado)
                {
                    _logger.LogInformation($"✅ Correo enviado exitosamente a: {evaluacion.Estudiante.DireccionCorreo}");
                }
                else
                {
                    _logger.LogWarning($"⚠️ Falló el envío de correo a: {evaluacion.Estudiante.DireccionCorreo}");
                }

                return resultado;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ Error al enviar correo a {evaluacion.Estudiante.DireccionCorreo}");
                return false;
            }
        }

        private string GenerarContenidoCorreo(Evaluacion evaluacion)
        {
            var contenido = $@"
        <div style='font-family: Arial, sans-serif; color: #333; font-size: 15px; line-height: 1.6;'>
            <h2 style='color: #004085;'>Resultado de Evaluación</h2>
            <p><strong>Estimado/a {evaluacion.Estudiante.NombreCompleto},</strong></p>
            
            <p>Le informamos que su evaluación ha sido completada con los siguientes resultados:</p>
            
            <div style='border: 1px solid #ced4da; border-radius: 6px; padding: 20px; background-color: #f8f9fa; margin: 20px 0;'>
                <h3 style='margin-top: 0; color: #495057;'>Detalles de la Evaluación</h3>
                <ul style='list-style: none; padding: 0;'>
                    <li><strong>📋 Rúbrica:</strong> {evaluacion.Rubrica.Titulo}</li>
                    <li><strong>📅 Fecha de evaluación:</strong> {evaluacion.FechaEvaluacion:dd/MM/yyyy HH:mm}</li>
                    <li><strong>🏆 Puntaje total:</strong> {evaluacion.TotalPuntos:F2} puntos</li>
                    <li><strong>📚 Período académico:</strong> {evaluacion.Estudiante?.PeriodoAcademico?.Nombre}</li>
                </ul>
            </div>
            
            <h4 style='color: #004085;'>Detalles por ?tem</h4>
            <table style='border-collapse: collapse; width: 100%; margin: 10px 0;'>
                <thead>
                    <tr style='background-color: #e9ecef; color: #212529;'>
                        <th style='border: 1px solid #dee2e6; padding: 10px; text-align: left;'>?tem</th>
                        <th style='border: 1px solid #dee2e6; padding: 10px; text-align: left;'>Nivel Alcanzado</th>
                        <th style='border: 1px solid #dee2e6; padding: 10px; text-align: center;'>Puntos</th>
                    </tr>
                </thead>
                <tbody>";

            foreach (var detalle in evaluacion.DetallesEvaluacion.OrderBy(d => d.ItemEvaluacion.OrdenItem))
            {
                contenido += $@"
                    <tr>
                        <td style='border: 1px solid #dee2e6; padding: 10px;'>{detalle.ItemEvaluacion.NombreItem}</td>
                        <td style='border: 1px solid #dee2e6; padding: 10px;'>{detalle.NivelCalificacion.NombreNivel}</td>
                        <td style='border: 1px solid #dee2e6; padding: 10px; text-align: center;'>{detalle.PuntosObtenidos:F2}</td>
                    </tr>";
            }

            contenido += $@"
                </tbody>
            </table>";

            if (!string.IsNullOrEmpty(evaluacion.Observaciones))
            {
                contenido += $@"
            <div style='margin-top: 20px;'>
                <h4 style='color: #004085;'>Observaciones</h4>
                <blockquote style='font-style: italic; background-color: #f1f3f5; padding: 10px; border-left: 4px solid #007bff; border-radius: 5px;'>
                    {evaluacion.Observaciones}
                </blockquote>
            </div>";
            }

            contenido += $@"
            <p style='margin-top: 30px;'>Si tiene alguna consulta sobre esta evaluacion, no dude en contactarnos.</p>

            <p>Saludos cordiales,<br>
            <strong>Sistema de Evaluacion por Rúbricas</strong>
            </p>
        </div>";

            return contenido;
        }

        // Método para cargar información adicional de evaluaciones (grupos, materias, instrumentos)
        private async Task CargarInformacionAdicionalEvaluaciones(List<Evaluacion> evaluaciones, EvaluacionesFiltroViewModel filtros)
        {
            // Crear diccionarios para mapear información adicional
            var estudiantesIds = evaluaciones.Select(e => e.IdEstudiante).Distinct().ToList();
            var rubricasIds = evaluaciones.Select(e => e.IdRubrica).Distinct().ToList();

            // Obtener información de grupos por estudiante
            var gruposEstudiantes = await _context.EstudianteGrupos
                .Where(eg => estudiantesIds.Contains(eg.EstudianteId) && eg.Estado == EstadoAsignacion.Activo)
                .Include(eg => eg.Grupo)
                .GroupBy(eg => eg.EstudianteId)
                .ToDictionaryAsync(
                    g => g.Key,
                    g => g.Select(eg => $"{eg.Grupo.Codigo} - {eg.Grupo.Nombre}").ToList()
                );

            // Obtener información de materias e instrumentos por rúbrica
            var materiasInstrumentos = await _context.InstrumentoRubricas
                .Where(ir => rubricasIds.Contains(ir.RubricaId))
                .Include(ir => ir.InstrumentoEvaluacion)
                .SelectMany(ir => _context.InstrumentoMaterias
                    .Where(im => im.InstrumentoEvaluacionId == ir.InstrumentoEvaluacionId)
                    .Include(im => im.Materia)
                    .Select(im => new {
                        ir.RubricaId,
                        InstrumentoId = ir.InstrumentoEvaluacionId,
                        InstrumentoNombre = ir.InstrumentoEvaluacion.Nombre,
                        MateriaId = im.MateriaId,
                        MateriaCodigo = im.Materia.Codigo,
                        MateriaNombre = im.Materia.Nombre
                    })
                )
                .GroupBy(x => x.RubricaId)
                .ToDictionaryAsync(
                    g => g.Key,
                    g => g.GroupBy(x => new { x.InstrumentoId, x.InstrumentoNombre })
                          .Select(ig => new {
                              Instrumento = ig.Key,
                              Materias = ig.Select(x => $"{x.MateriaCodigo} - {x.MateriaNombre}").Distinct().ToList()
                          }).ToList()
                );

            // Almacenar la información en ViewBag para uso en la vista
            ViewBag.GruposEstudiantes = gruposEstudiantes;
            ViewBag.MateriasInstrumentos = materiasInstrumentos;
        }

        // GET: Evaluaciones/Reportes
        public async Task<IActionResult> Reportes(int? periodoId, int? rubricaId)
        {
            try
            {
                // Aplicar filtros base
                var query = _context.Evaluaciones
                    .Include(e => e.Estudiante)
                        .ThenInclude(est => est.PeriodoAcademico)
                    .Include(e => e.Rubrica)
                    .Include(e => e.DetallesEvaluacion)
                        .ThenInclude(d => d.NivelCalificacion)
                    .Include(e => e.DetallesEvaluacion)
                        .ThenInclude(d => d.ItemEvaluacion)
                    .AsQueryable();

                // Aplicar filtros específicos
                if (periodoId.HasValue && periodoId.Value > 0)
                {
                    query = query.Where(e => e.Estudiante.PeriodoAcademicoId == periodoId.Value);
                }

                if (rubricaId.HasValue && rubricaId.Value > 0)
                {
                    query = query.Where(e => e.IdRubrica == rubricaId.Value);
                }

                var evaluaciones = await query
                    .OrderByDescending(e => e.FechaEvaluacion)
                    .ToListAsync();

                // Calcular estadísticas
                var totalEvaluaciones = evaluaciones.Count;
                var evaluacionesConPuntaje = evaluaciones.Where(e => e.TotalPuntos.HasValue).ToList();

                var promedioGeneral = evaluacionesConPuntaje.Any() 
                    ? evaluacionesConPuntaje.Average(e => e.TotalPuntos.Value) 
                    : 0;

                var puntajeMaximo = evaluacionesConPuntaje.Any() 
                    ? evaluacionesConPuntaje.Max(e => e.TotalPuntos.Value) 
                    : 0;

                var puntajeMinimo = evaluacionesConPuntaje.Any() 
                    ? evaluacionesConPuntaje.Min(e => e.TotalPuntos.Value) 
                    : 0;

                // Crear ViewModel
                var viewModel = new ReporteEvaluacionesViewModel
                {
                    Evaluaciones = evaluaciones,
                    TotalEvaluaciones = totalEvaluaciones,
                    PromedioGeneral = promedioGeneral,
                    PuntajeMaximo = puntajeMaximo,
                    PuntajeMinimo = puntajeMinimo
                };

                // Cargar listas para filtros
                var filtros = new EvaluacionesFiltroViewModel();
                await CargarListasFiltros(filtros);

                // Asignar al ViewBag para la vista
                ViewBag.Periodos = filtros.Periodos;
                ViewBag.Rubricas = filtros.Rubricas;
                ViewBag.PeriodoSeleccionado = periodoId;
                ViewBag.RubricaSeleccionada = rubricaId;

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar los reportes de evaluaciones");
                TempData["Error"] = "Error al cargar los reportes. Intente nuevamente.";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
