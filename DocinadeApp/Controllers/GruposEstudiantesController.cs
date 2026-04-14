using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RubricasApp.Web.Data;
using RubricasApp.Web.Models;
using RubricasApp.Web.Models.Identity;
using RubricasApp.Web.Services.Grupos;
using RubricasApp.Web.ViewModels.Grupos;
using RubricasApp.Web.ViewModels.Shared;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Rendering; // 🔧 CORRECCIÓN: Agregar usando para SelectListItem del framework
using RubricasApp.Web.Authorization;
using RubricasApp.Web.Models.Permissions;

namespace RubricasApp.Web.Controllers
{
    [Authorize]
    public class GruposEstudiantesController : Controller
    {
        private readonly IGrupoEstudianteService _grupoService;
        private readonly RubricasDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<GruposEstudiantesController> _logger;

        public GruposEstudiantesController(
            IGrupoEstudianteService grupoService,
            RubricasDbContext context,
            UserManager<ApplicationUser> userManager,
            ILogger<GruposEstudiantesController> logger)
        {
            _grupoService = grupoService;
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        // GET: GruposEstudiantes
        [RequirePermission(ApplicationPermissions.GruposEstudiantes.VER)]
        public async Task<IActionResult> Index(FiltrosGrupoViewModel? filtros = null, int pagina = 1)
        {
            try
            {
                // Obtener período actual de la sesión si no se especifica uno
                if (!filtros?.PeriodoAcademicoId.HasValue ?? true)
                {
                    var periodoActualId = HttpContext.Session.GetInt32("PeriodoAcademicoId");
                    if (periodoActualId.HasValue)
                    {
                        filtros ??= new FiltrosGrupoViewModel();
                        filtros.PeriodoAcademicoId = periodoActualId.Value;
                    }
                }

                var grupos = await _grupoService.ObtenerGruposAsync(filtros);
                var estadisticas = await _grupoService.ObtenerEstadisticasAsync(filtros?.PeriodoAcademicoId);

                // Paginación
                const int elementosPorPagina = 20;
                var totalElementos = grupos.Count();
                var gruposPaginados = grupos
                    .Skip((pagina - 1) * elementosPorPagina)
                    .Take(elementosPorPagina)
                    .ToList();

                var viewModel = new GrupoEstudianteIndexViewModel
                {
                    Grupos = gruposPaginados.Select(g => new GrupoEstudianteListItemViewModel
                    {
                        GrupoId = g.GrupoId,
                        Codigo = g.Codigo,
                        Nombre = g.Nombre,
                        Descripcion = g.Descripcion,
                        TipoGrupo = g.TipoGrupo, // 🔧 MANTENER: Compatibilidad con enum
                        TipoGrupoDisplay = g.TipoGrupoCatalogo?.Nombre ?? g.TipoGrupo.ToString(), // 🔧 USAR: Nombre del catálogo
                        Nivel = g.Nivel,
                        CapacidadMaxima = g.CapacidadMaxima,
                        Estado = g.Estado,
                        EstadoDisplay = g.Estado.ToString(),
                        FechaCreacion = g.FechaCreacion,
                        CreadoPor = g.CreadoPor?.NombreCompleto,
                        CantidadEstudiantes = g.CantidadEstudiantes,
                        EstaCompleto = g.EstaCompleto,
                        EspaciosDisponibles = g.EspaciosDisponibles,
                        EstadoCapacidad = g.EstadoCapacidad,
                        InstitucionId = g.InstitucionId,
                        InstitucionNombre = g.Institucion?.Nombre,
                        Materias = g.GrupoMaterias.Where(gm => gm.Estado == EstadoAsignacion.Activo)
                            .Select(gm => gm.Materia?.Nombre ?? "")
                            .ToList()
                    }).ToList(),
                    Filtros = filtros ?? new FiltrosGrupoViewModel(),
                    Paginacion = new PaginacionViewModel
                    {
                        PaginaActual = pagina,
                        ElementosPorPagina = elementosPorPagina,
                        TotalElementos = totalElementos
                    },
                    EstadisticasPorTipo = estadisticas.GruposPorTipo
                };

                // Para los dropdowns de filtros - USANDO MODELO TIPADO
                var periodosAcademicos = await _context.PeriodosAcademicos
                    .Where(p => p.Estado == "Activo")
                    .Select(p => new PeriodoAcademicoDropdownViewModel
                    {
                        Id = p.Id,
                        Nombre = p.Nombre,
                        Estado = !string.IsNullOrEmpty(p.Estado) ? p.Estado : EstadoAsignacion.Activo.ToString()
                    })
                    .ToListAsync();

                ViewBag.PeriodosAcademicos = periodosAcademicos;

                // 🔧 NUEVA: Cargar tipos de grupo del catálogo para filtros
                var tiposGrupoDisponibles = await _context.TiposGrupo
                    .Where(t => t.Estado == "Activo")

                    
                    .OrderBy(t => t.Nombre)
                    .Select(t => new SelectListItem 
                    { 
                        Value = t.IdTipoGrupo.ToString(), 
                        Text = t.Nombre 
                    })
                    .ToListAsync();

                ViewBag.TiposGrupoDisponibles = tiposGrupoDisponibles;

                // Cargar instituciones para filtro
                var instituciones = await _context.Instituciones
                    .Where(i => i.Estado)
                    .OrderBy(i => i.Nombre)
                    .Select(i => new SelectListItem 
                    { 
                        Value = i.Id.ToString(), 
                        Text = i.Nombre 
                    })
                    .ToListAsync();

                ViewBag.Instituciones = instituciones;

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar la lista de grupos");
                TempData["ErrorMessage"] = "Error al cargar los grupos. Por favor, intente nuevamente.";
                
                // Crear ViewModel vacío en caso de error
                var emptyViewModel = new GrupoEstudianteIndexViewModel
                {
                    Grupos = new List<GrupoEstudianteListItemViewModel>(),
                    Filtros = filtros ?? new FiltrosGrupoViewModel(),
                    Paginacion = new PaginacionViewModel(),
                    EstadisticasPorTipo = new Dictionary<TipoGrupo, int>()
                };

                // ViewBag vacío para evitar errores en la vista
                ViewBag.PeriodosAcademicos = new List<PeriodoAcademicoDropdownViewModel>();
                ViewBag.TiposGrupoDisponibles = new List<SelectListItem>(); // 🔧 NUEVA: Para evitar errores en filtros
                
                return View(emptyViewModel);
            }
        }

        // GET: GruposEstudiantes/Details/5
        [RequirePermission(ApplicationPermissions.GruposEstudiantes.VER_DETALLES)]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var grupo = await _grupoService.ObtenerGrupoPorIdAsync(id);
                if (grupo == null)
                {
                    return NotFound();
                }

                var viewModel = new DetalleGrupoViewModel
                {
                    GrupoId = grupo.GrupoId,
                    Codigo = grupo.Codigo,
                    Nombre = grupo.Nombre,
                    Descripcion = grupo.Descripcion,
                    TipoGrupo = grupo.TipoGrupo, // 🔧 MANTENER: Compatibilidad con enum
                    TipoGrupoDisplay = grupo.TipoGrupoCatalogo?.Nombre ?? grupo.TipoGrupo.ToString(), // 🔧 USAR: Nombre del catálogo
                    Nivel = grupo.Nivel,
                    CapacidadMaxima = grupo.CapacidadMaxima,
                    Estado = grupo.Estado,
                    EstadoDisplay = grupo.Estado.ToString(),
                    FechaCreacion = grupo.FechaCreacion,
                    FechaModificacion = grupo.FechaModificacion,
                    CreadoPor = grupo.CreadoPor?.NombreCompleto,
                    Observaciones = grupo.Observaciones,
                    PeriodoAcademicoId = grupo.PeriodoAcademicoId,
                    PeriodoAcademico = grupo.PeriodoAcademico?.Nombre ?? "",
                    CantidadEstudiantes = grupo.CantidadEstudiantes,
                    EstaCompleto = grupo.EstaCompleto,
                    EspaciosDisponibles = grupo.EspaciosDisponibles,
                    EstadoCapacidad = grupo.EstadoCapacidad,
                    Estudiantes = grupo.EstudianteGrupos
                        .Where(eg => eg.Estado == EstadoAsignacion.Activo)
                        .Select(eg => new EstudianteEnGrupoViewModel
                        {
                            EstudianteId = eg.EstudianteId,
                            NumeroId = eg.Estudiante?.NumeroId ?? "",
                            NombreCompleto = eg.Estudiante?.NombreCompleto ?? "",
                            Email = eg.Estudiante?.DireccionCorreo ?? "",
                            FechaAsignacion = eg.FechaAsignacion,
                            Estado = eg.Estado ?? EstadoAsignacion.Activo,
                            EstadoDisplay = (eg.Estado ?? EstadoAsignacion.Activo).ToString(),
                            EsGrupoPrincipal = eg.EsGrupoPrincipal,
                            MotivoAsignacion = eg.MotivoAsignacion,
                            AsignadoPor = eg.AsignadoPor?.NombreCompleto
                        }).ToList(),
                    Materias = grupo.GrupoMaterias
                        .Where(gm => gm.Estado == EstadoAsignacion.Activo)
                        .Select(gm => new MateriaEnGrupoViewModel
                        {
                            MateriaId = gm.MateriaId,
                            Codigo = gm.Materia?.Codigo ?? "",
                            Nombre = gm.Materia?.Nombre ?? "",
                            FechaAsignacion = gm.FechaAsignacion,
                            Estado = gm.Estado ?? EstadoAsignacion.Activo,
                            EstadoDisplay = gm.Estado != null ? gm.Estado.ToString() : EstadoAsignacion.Activo.ToString(),
                            Observaciones = gm.Observaciones
                        }).ToList()
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar los detalles del grupo {GrupoId}", id);
                return NotFound();
            }
        }

        // GET: GruposEstudiantes/Create
        [RequirePermission(ApplicationPermissions.GruposEstudiantes.CREAR)]
        public async Task<IActionResult> Create()
        {
            try
            {
                var viewModel = new CrearGrupoViewModel();
                await CargarDatosParaFormulario(viewModel);
                
                // 🔧 DEBUG: Log de tipos de grupo cargados
                _logger.LogInformation("🔍 Tipos de grupo cargados para Create: {Count}", viewModel.TiposGrupo?.Count ?? 0);
                if (viewModel.TiposGrupo?.Any() == true)
                {
                    foreach (var tipo in viewModel.TiposGrupo)
                    {
                        _logger.LogInformation("  - {Value}: {Text}", tipo.Value, tipo.Text);
                    }
                }
                
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar vista Create");
                return View(new CrearGrupoViewModel());
            }
        }

        // POST: GruposEstudiantes/Create
        [HttpPost]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequirePermission(ApplicationPermissions.GruposEstudiantes.CREAR)]
        public async Task<IActionResult> Create(CrearGrupoViewModel viewModel)
        {
            // 🔧 DEBUG COMPLETO: Log de todos los datos recibidos
            _logger.LogInformation("🔍 === DEBUG COMPLETO CREATE ===");
            _logger.LogInformation("Código: '{Codigo}'", viewModel.Codigo ?? "NULL");
            _logger.LogInformation("Nombre: '{Nombre}'", viewModel.Nombre ?? "NULL");
            _logger.LogInformation("IdTipoGrupo: {IdTipoGrupo}", viewModel.IdTipoGrupo);
            _logger.LogInformation("TipoGrupo (enum): {TipoGrupo}", viewModel.TipoGrupo);
            _logger.LogInformation("PeriodoAcademicoId: {PeriodoId}", viewModel.PeriodoAcademicoId);
            _logger.LogInformation("Descripcion: '{Descripcion}'", viewModel.Descripcion ?? "NULL");
            _logger.LogInformation("Nivel: '{Nivel}'", viewModel.Nivel ?? "NULL");
            _logger.LogInformation("CapacidadMaxima: {Capacidad}", viewModel.CapacidadMaxima?.ToString() ?? "NULL");
            _logger.LogInformation("Observaciones: '{Observaciones}'", viewModel.Observaciones ?? "NULL");
            _logger.LogInformation("MateriasSeleccionadas Count: {Count}", viewModel.MateriasSeleccionadas?.Count ?? 0);
            _logger.LogInformation("=================================");
            
            // 🔧 CORRECCIÓN: Log de errores de validación
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("❌ ModelState NO es válido:");
                var errores = ModelState
                    .Where(x => x.Value != null && x.Value.Errors != null && x.Value.Errors.Count > 0)
                    .Select(x => new { Campo = x.Key, Errores = x.Value != null && x.Value.Errors != null ? x.Value.Errors.Select(e => e.ErrorMessage) : new List<string>() });
                
                foreach (var error in errores)
                {
                    _logger.LogWarning("   Campo '{Campo}': {Errores}", error.Campo, string.Join(", ", error.Errores));
                }
            }
            else
            {
                _logger.LogInformation("✅ ModelState es válido");
            }
            
            if (ModelState.IsValid)
            {
                try
                {
                    var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
                    var resultado = await _grupoService.CrearGrupoAsync(viewModel, usuarioId);

                    if (resultado.Exitoso)
                    {
                        _logger.LogInformation("✅ Grupo creado exitosamente: {GrupoId}", resultado.Resultado?.GrupoId);
                        TempData["SuccessMessage"] = resultado.Mensaje;
                        return RedirectToAction(nameof(Details), new { id = resultado.Resultado?.GrupoId });
                    }

                    _logger.LogWarning("⚠️ Error del servicio al crear grupo: {Mensaje}", resultado.Mensaje);
                    ModelState.AddModelError("", resultado.Mensaje);
                    foreach (var error in resultado.Errores)
                    {
                        ModelState.AddModelError("", error);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "💥 Error al crear grupo");
                    ModelState.AddModelError("", "Error interno al crear el grupo");
                }
            }

            await CargarDatosParaFormulario(viewModel);
            return View(viewModel);
        }

        // GET: GruposEstudiantes/Edit/5
        [RequirePermission(ApplicationPermissions.GruposEstudiantes.EDITAR)]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var grupo = await _grupoService.ObtenerGrupoPorIdAsync(id);
                if (grupo == null)
                {
                    return NotFound();
                }

                var viewModel = new EditarGrupoViewModel
                {
                    GrupoId = grupo.GrupoId,
                    Codigo = grupo.Codigo,
                    Nombre = grupo.Nombre,
                    Descripcion = grupo.Descripcion,
                    TipoGrupo = grupo.TipoGrupo, // 🔧 MANTENER: Compatibilidad con enum
                    IdTipoGrupo = grupo.IdTipoGrupo, // 🔧 USAR: ID del catálogo
                    Nivel = grupo.Nivel,
                    CapacidadMaxima = grupo.CapacidadMaxima,
                    PeriodoAcademicoId = grupo.PeriodoAcademicoId,
                    Observaciones = grupo.Observaciones,
                    FechaCreacion = grupo.FechaCreacion,
                    CreadoPor = grupo.CreadoPor?.NombreCompleto,
                    CantidadEstudiantes = grupo.CantidadEstudiantes,
                    CantidadEstudiantesActuales = grupo.CantidadEstudiantes, // ?? CORRECCIÓN: Asignar ambas propiedades
                    TieneEstudiantesAsignados = grupo.CantidadEstudiantes > 0,
                    Estado = grupo.Estado, // ?? CORRECCIÓN: Agregar la propiedad Estado que faltaba
                    MateriasSeleccionadas = grupo.GrupoMaterias
                        .Where(gm => gm.Estado == EstadoAsignacion.Activo)
                        .Select(gm => gm.MateriaId)
                        .ToList()
                };

                await CargarDatosParaFormulario(viewModel);
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar grupo para edición {GrupoId}", id);
                return NotFound();
            }
        }

        // POST: GruposEstudiantes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequirePermission(ApplicationPermissions.GruposEstudiantes.EDITAR)]
        public async Task<IActionResult> Edit(EditarGrupoViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
                    var resultado = await _grupoService.ActualizarGrupoAsync(viewModel, usuarioId);

                    if (resultado.Exitoso)
                    {
                        TempData["SuccessMessage"] = resultado.Mensaje;
                        return RedirectToAction(nameof(Details), new { id = viewModel.GrupoId });
                    }

                    ModelState.AddModelError("", resultado.Mensaje);
                    foreach (var error in resultado.Errores)
                    {
                        ModelState.AddModelError("", error);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al actualizar grupo {GrupoId}", viewModel.GrupoId);
                    ModelState.AddModelError("", "Error interno al actualizar el grupo");
                }
            }

            await CargarDatosParaFormulario(viewModel);
            return View(viewModel);
        }

        // GET: GruposEstudiantes/Delete/5
        [RequirePermission(ApplicationPermissions.GruposEstudiantes.ELIMINAR)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var grupo = await _grupoService.ObtenerGrupoPorIdAsync(id);
                if (grupo == null)
                {
                    return NotFound();
                }

                var viewModel = new DetalleGrupoViewModel
                {
                    GrupoId = grupo.GrupoId,
                    Codigo = grupo.Codigo,
                    Nombre = grupo.Nombre,
                    Descripcion = grupo.Descripcion,
                    TipoGrupo = grupo.TipoGrupo,
                    TipoGrupoDisplay = grupo.TipoGrupoCatalogo?.Nombre ?? grupo.TipoGrupo.ToString(),
                    Nivel = grupo.Nivel,
                    CapacidadMaxima = grupo.CapacidadMaxima,
                    Estado = grupo.Estado,
                    EstadoDisplay = grupo.Estado.ToString(),
                    FechaCreacion = grupo.FechaCreacion,
                    FechaModificacion = grupo.FechaModificacion,
                    CreadoPor = grupo.CreadoPor?.NombreCompleto,
                    Observaciones = grupo.Observaciones,
                    PeriodoAcademicoId = grupo.PeriodoAcademicoId,
                    PeriodoAcademico = grupo.PeriodoAcademico?.Nombre ?? "",
                    CantidadEstudiantes = grupo.CantidadEstudiantes,
                    EstaCompleto = grupo.EstaCompleto,
                    EspaciosDisponibles = grupo.EspaciosDisponibles,
                    EstadoCapacidad = grupo.EstadoCapacidad,
                    Estudiantes = grupo.EstudianteGrupos
                        .Where(eg => eg.Estado == EstadoAsignacion.Activo)
                        .Select(eg => new EstudianteEnGrupoViewModel
                        {
                            EstudianteId = eg.EstudianteId,
                            NumeroId = eg.Estudiante?.NumeroId ?? "",
                            NombreCompleto = eg.Estudiante?.NombreCompleto ?? "",
                            Email = eg.Estudiante?.DireccionCorreo ?? "",
                            FechaAsignacion = eg.FechaAsignacion,
                            Estado = eg.Estado ?? EstadoAsignacion.Activo,
                            EstadoDisplay = (eg.Estado ?? EstadoAsignacion.Activo).ToString(),
                            EsGrupoPrincipal = eg.EsGrupoPrincipal,
                            MotivoAsignacion = eg.MotivoAsignacion,
                            AsignadoPor = eg.AsignadoPor?.NombreCompleto
                        }).ToList(),
                    Materias = grupo.GrupoMaterias
                        .Where(gm => gm.Estado == EstadoAsignacion.Activo)
                        .Select(gm => new MateriaEnGrupoViewModel
                        {
                            MateriaId = gm.MateriaId,
                            Codigo = gm.Materia?.Codigo ?? "",
                            Nombre = gm.Materia?.Nombre ?? "",
                            FechaAsignacion = gm.FechaAsignacion,
                            Estado = gm.Estado ?? EstadoAsignacion.Activo,
                            EstadoDisplay = (gm.Estado ?? EstadoAsignacion.Activo).ToString(),
                            Observaciones = gm.Observaciones
                        }).ToList()
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar grupo para eliminar {GrupoId}", id);
                return NotFound();
            }
        }

        // POST: GruposEstudiantes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [RequirePermission(ApplicationPermissions.GruposEstudiantes.ELIMINAR)]
        public async Task<IActionResult> DeleteConfirmed(int id, string? motivo)
        {
            try
            {
                _logger.LogInformation("🗑️ Intentando eliminar grupo {GrupoId} con motivo: {Motivo}", id, motivo ?? "No especificado");
                
                var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
                
                // Obtener información de la request para auditoría
                var direccionIP = HttpContext.Connection.RemoteIpAddress?.ToString();
                var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();
                
                var resultado = await _grupoService.EliminarGrupoAsync(id, usuarioId, motivo, direccionIP, userAgent);

                _logger.LogInformation("📝 Resultado de eliminación: Exitoso={Exitoso}, Mensaje={Mensaje}", resultado.Exitoso, resultado.Mensaje);

                if (resultado.Exitoso)
                {
                    TempData["SuccessMessage"] = resultado.Mensaje;
                    _logger.LogInformation("✅ Grupo {GrupoId} eliminado exitosamente", id);
                }
                else
                {
                    TempData["ErrorMessage"] = resultado.Mensaje;
                    _logger.LogWarning("❌ Error al eliminar grupo {GrupoId}: {Error}", id, resultado.Mensaje);
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "💥 Error interno al eliminar grupo {GrupoId}", id);
                TempData["ErrorMessage"] = "Error interno al eliminar el grupo";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: GruposEstudiantes/AsignarEstudiantes/5 - VERSIÓN SIMPLIFICADA PARA DIAGNÓSTICO
        [RequirePermission(ApplicationPermissions.GruposEstudiantes.ASIGNAR_ESTUDIANTES)]
        public async Task<IActionResult> AsignarEstudiantes(int id)
        {
            try
            {
                _logger.LogInformation("AsignarEstudiantes llamado para grupo ID: {GrupoId}", id);

                // Verificación básica primero
                var grupoExists = await _context.GruposEstudiantes.AnyAsync(g => g.GrupoId == id);
                if (!grupoExists)
                {
                    _logger.LogWarning("? Grupo {GrupoId} no encontrado", id);
                    return NotFound($"Grupo con ID {id} no encontrado");
                }

                // Intentar obtener grupo básico sin includes complejos
                var grupo = await _context.GruposEstudiantes
                    .Where(g => g.GrupoId == id)
                    .Select(g => new {
                        g.GrupoId,
                        g.Codigo,
                        g.Nombre,
                        g.CapacidadMaxima,
                        g.PeriodoAcademicoId,
                        CantidadEstudiantes = _context.EstudianteGrupos
                            .Count(eg => eg.GrupoId == g.GrupoId && eg.Estado == EstadoAsignacion.Activo)
                    })
                    .FirstOrDefaultAsync();

                if (grupo == null)
                {
                    _logger.LogWarning("? No se pudo obtener datos del grupo {GrupoId}", id);
                    return NotFound();
                }

                _logger.LogInformation("? Grupo encontrado: {Codigo}", grupo.Codigo);

                // Obtener estudiantes disponibles básicos
                var estudiantesQuery = _context.Estudiantes
                    .Where(e => e.PeriodoAcademicoId == grupo.PeriodoAcademicoId)
                    .Where(e => !_context.EstudianteGrupos
                        .Any(eg => eg.EstudianteId == e.IdEstudiante && 
                                  eg.GrupoId == id && 
                                  eg.Estado == EstadoAsignacion.Activo));

                var estudiantesDisponibles = await estudiantesQuery
                    .Take(50) // Limitar para evitar problemas de rendimiento
                    .Select(e => new {
                        EstudianteId = e.IdEstudiante,
                        NumeroId = e.NumeroId,
                        Nombre = e.Nombre,
                        Apellidos = e.Apellidos,
                        Email = e.DireccionCorreo
                    })
                    .ToListAsync();

                var estudiantesViewModel = estudiantesDisponibles.Select(e => new EstudianteDisponibleViewModel
                {
                    EstudianteId = e.EstudianteId,
                    NumeroId = e.NumeroId,
                    NombreCompleto = $"{e.Nombre} {e.Apellidos}",
                    Email = e.Email,
                    TieneGrupoPrincipal = false,
                    GrupoActual = ""
                }).ToList();

                _logger.LogInformation("? {Count} estudiantes disponibles encontrados", estudiantesDisponibles.Count);

                var viewModel = new AsignarEstudiantesViewModel
                {
                    GrupoId = grupo.GrupoId,
                    GrupoNombre = grupo.Nombre,
                    GrupoCodigo = grupo.Codigo,
                    CapacidadMaxima = grupo.CapacidadMaxima,
                    EstudiantesActuales = grupo.CantidadEstudiantes,
                    EspaciosDisponibles = grupo.CapacidadMaxima.HasValue ? 
                        Math.Max(0, grupo.CapacidadMaxima.Value - grupo.CantidadEstudiantes) : 
                        int.MaxValue,
                    EstudiantesDisponibles = estudiantesViewModel
                };

                // Pasar información al ViewBag para JavaScript
                ViewBag.GrupoId = grupo.GrupoId;
                ViewBag.CapacidadMaxima = grupo.CapacidadMaxima;
                ViewBag.TotalEstudiantesAsignados = grupo.CantidadEstudiantes;

                _logger.LogInformation("? ViewModel creado exitosamente para grupo {Codigo}", grupo.Codigo);

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "? Error al cargar estudiantes disponibles para grupo {GrupoId}", id);
                
                // Crear un ViewModel mínimo para mostrar el error
                var errorViewModel = new AsignarEstudiantesViewModel
                {
                    GrupoId = id,
                    GrupoNombre = "Error al cargar",
                    GrupoCodigo = $"ID-{id}",
                    EstudiantesDisponibles = new List<EstudianteDisponibleViewModel>()
                };

                TempData["ErrorMessage"] = $"Error al cargar la página: {ex.Message}";
                return View(errorViewModel);
            }
        }

        // POST: GruposEstudiantes/AsignarEstudiantes
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequirePermission(ApplicationPermissions.GruposEstudiantes.ASIGNAR_ESTUDIANTES)]
        public async Task<IActionResult> AsignarEstudiantes(AsignarEstudiantesViewModel viewModel)
        {
            if (ModelState.IsValid && viewModel.EstudiantesSeleccionados.Any())
            {
                try
                {
                    var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
                    var resultado = await _grupoService.AsignarEstudiantesAsync(
                        viewModel.GrupoId,
                        viewModel.EstudiantesSeleccionados,
                        usuarioId,
                        viewModel.MotivoAsignacion,
                        viewModel.EsGrupoPrincipal);

                    if (resultado.Exitoso)
                    {
                        TempData["SuccessMessage"] = resultado.Mensaje;
                        return RedirectToAction(nameof(Details), new { id = viewModel.GrupoId });
                    }

                    ModelState.AddModelError("", resultado.Mensaje);
                    foreach (var error in resultado.Errores)
                    {
                        ModelState.AddModelError("", error);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al asignar estudiantes al grupo {GrupoId}", viewModel.GrupoId);
                    ModelState.AddModelError("", "Error interno al asignar estudiantes");
                }
            }
            else if (!viewModel.EstudiantesSeleccionados.Any())
            {
                ModelState.AddModelError("EstudiantesSeleccionados", "Debe seleccionar al menos un estudiante");
            }

            // Recargar datos para mostrar el formulario nuevamente
            var grupo = await _grupoService.ObtenerGrupoPorIdAsync(viewModel.GrupoId);
            if (grupo != null)
            {
                viewModel.EstudiantesDisponibles = await _grupoService.ObtenerEstudiantesDisponiblesAsync(
                    viewModel.GrupoId, grupo.PeriodoAcademicoId);
            }

            return View(viewModel);
        }

        // POST: GruposEstudiantes/DesasignarEstudiante
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequirePermission(ApplicationPermissions.GruposEstudiantes.DESASIGNAR_ESTUDIANTES)]
        public async Task<IActionResult> DesasignarEstudiante(int grupoId, int estudianteId, string? motivo)
        {
            try
            {
                var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
                var resultado = await _grupoService.DesasignarEstudianteAsync(grupoId, estudianteId, usuarioId, motivo);

                if (resultado.Exitoso)
                {
                    TempData["SuccessMessage"] = resultado.Mensaje;
                }
                else
                {
                    TempData["ErrorMessage"] = resultado.Mensaje;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al desasignar estudiante {EstudianteId} del grupo {GrupoId}", estudianteId, grupoId);
                TempData["ErrorMessage"] = "Error interno al desasignar estudiante";
            }

            return RedirectToAction(nameof(Details), new { id = grupoId });
        }

        // POST: GruposEstudiantes/DesasignarEstudiantesMultiples
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DesasignarEstudiantesMultiples(int grupoId, List<int> estudiantesIds, string? motivo)
        {
            try
            {
                if (!estudiantesIds?.Any() ?? true)
                {
                    TempData["ErrorMessage"] = "No se seleccionaron estudiantes para desasignar.";
                    return RedirectToAction(nameof(Details), new { id = grupoId });
                }

                var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
                var exitosos = 0;
                var errores = 0;
                var erroresDetalle = new List<string>();

                _logger.LogInformation("🔄 Iniciando desasignación múltiple de {Count} estudiantes del grupo {GrupoId}", estudiantesIds?.Count ?? 0, grupoId);

                foreach (var estudianteId in estudiantesIds ?? new List<int>())
                {
                    try
                    {
                        var resultado = await _grupoService.DesasignarEstudianteAsync(grupoId, estudianteId, usuarioId, motivo);
                        if (resultado.Exitoso)
                        {
                            exitosos++;
                        }
                        else
                        {
                            errores++;
                            erroresDetalle.Add($"Estudiante ID {estudianteId}: {resultado.Mensaje}");
                        }
                    }
                    catch (Exception ex)
                    {
                        errores++;
                        erroresDetalle.Add($"Estudiante ID {estudianteId}: Error interno");
                        _logger.LogError(ex, "Error al desasignar estudiante {EstudianteId} del grupo {GrupoId}", estudianteId, grupoId);
                    }
                }

                // Construir mensaje de resultado
                if (exitosos > 0 && errores == 0)
                {
                    TempData["SuccessMessage"] = $"Se desasignaron exitosamente {exitosos} estudiante(s) del grupo.";
                }
                else if (exitosos > 0 && errores > 0)
                {
                    TempData["WarningMessage"] = $"Se desasignaron {exitosos} estudiante(s) exitosamente, pero {errores} tuvieron errores.";
                    TempData["ErrorDetails"] = string.Join("; ", erroresDetalle);
                }
                else
                {
                    TempData["ErrorMessage"] = $"No se pudo desasignar ningún estudiante. Errores: {string.Join("; ", erroresDetalle)}";
                }

                _logger.LogInformation("✅ Desasignación múltiple completada: {Exitosos} exitosos, {Errores} errores", exitosos, errores);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error general en desasignación múltiple del grupo {GrupoId}", grupoId);
                TempData["ErrorMessage"] = "Error interno al desasignar estudiantes";
            }

            return RedirectToAction(nameof(Details), new { id = grupoId });
        }

        // GET: GruposEstudiantes/Estadisticas
        [RequirePermission(ApplicationPermissions.GruposEstudiantes.VER_ESTADISTICAS)]
        public async Task<IActionResult> Estadisticas(int? periodoId)
        {
            try
            {
                periodoId ??= HttpContext.Session.GetInt32("PeriodoAcademicoId");
                var estadisticas = await _grupoService.ObtenerEstadisticasAsync(periodoId);

                ViewBag.PeriodosAcademicos = await _context.PeriodosAcademicos
                    .Where(p => p.Estado == "Activo")
                    .Select(p => new { p.Id, p.Nombre })
                    .ToListAsync();

                ViewBag.PeriodoSeleccionado = periodoId;

                return View(estadisticas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar estadísticas de grupos");
                TempData["ErrorMessage"] = "Error al cargar las estadísticas";
                return View(new GrupoEstadisticasViewModel());
            }
        }

        // GET: GruposEstudiantes/ExportarExcel
        [RequirePermission(ApplicationPermissions.GruposEstudiantes.EXPORTAR)]
        public async Task<IActionResult> ExportarExcel(FiltrosGrupoViewModel? filtros = null)
        {
            try
            {
                var bytes = await _grupoService.ExportarGruposExcelAsync(filtros);
                var fileName = $"Grupos_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al exportar grupos a Excel");
                TempData["ErrorMessage"] = "Error al exportar datos";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: GruposEstudiantes/ExportarEstudiantesExcel/5
        public async Task<IActionResult> ExportarEstudiantesExcel(int id)
        {
            try
            {
                var bytes = await _grupoService.ExportarEstudiantesPorGrupoExcelAsync(id);
                var grupo = await _grupoService.ObtenerGrupoPorIdAsync(id);
                var fileName = $"Estudiantes_{grupo?.Codigo}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al exportar estudiantes del grupo {GrupoId} a Excel", id);
                TempData["ErrorMessage"] = "Error al exportar datos";
                return RedirectToAction(nameof(Details), new { id });
            }
        }

        private async Task CargarDatosParaFormulario(CrearGrupoViewModel viewModel)
        {
            try
            {
                var periodos = await _context.PeriodosAcademicos
                    .Where(p => p.Estado == "Activo")
                    .ToListAsync();

                // 🔧 CORRECCIÓN: Usar SelectListItem del framework
                viewModel.PeriodosDisponibles = periodos
                    .Select(p => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Value = p.Id.ToString(), Text = p.Nombre })
                    .ToList();

                var materias = await _context.Materias
                    .Where(m => m.Activa)
                    .ToListAsync();

                // 🔧 CORRECCIÓN: Usar SelectListItem del framework
                viewModel.MateriasDisponibles = materias
                    .Select(m => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Value = m.MateriaId.ToString(), Text = $"{m.Codigo} - {m.Nombre}" })
                    .ToList();

                // 🔧 NUEVA IMPLEMENTACIÓN: Cargar tipos desde el catálogo
                var tiposGrupo = await _context.TiposGrupo
                    .Where(t => t.Estado == "Activo")
                    .OrderBy(t => t.Nombre)
                    .ToListAsync();

                _logger.LogInformation("🔍 Tipos de grupo encontrados en BD: {Count}", tiposGrupo.Count);

                viewModel.TiposGrupo = tiposGrupo
                    .Select(t => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { 
                        Value = t.IdTipoGrupo.ToString(), 
                        Text = t.Nombre 
                    })
                    .ToList();

                _logger.LogInformation("🔍 SelectListItem creados: {Count}", viewModel.TiposGrupo.Count);

                // 🔧 FALLBACK: Si no hay tipos en el catálogo, usar enum por compatibilidad
                if (!viewModel.TiposGrupo.Any())
                {
                    _logger.LogWarning("No se encontraron tipos de grupo en el catálogo, usando enum por defecto");
                    viewModel.TiposGrupo = Enum.GetValues<TipoGrupo>()
                        .Select(t => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { 
                            Value = ((int)t).ToString(), 
                            Text = t switch 
                            {
                                TipoGrupo.Seccion => "Sección",
                                TipoGrupo.Nivel => "Nivel", 
                                TipoGrupo.Modalidad => "Modalidad",
                                TipoGrupo.Custom => "Personalizado",
                                _ => t.ToString()
                            }
                        })
                        .ToList();
                }

                // Cargar instituciones
                var instituciones = await _context.Instituciones
                    .Where(i => i.Estado)
                    .OrderBy(i => i.Nombre)
                    .ToListAsync();

                viewModel.InstitucionesDisponibles = instituciones
                    .Select(i => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                    {
                        Value = i.Id.ToString(),
                        Text = i.Nombre
                    })
                    .ToList();

                // Establecer datos para ViewBag también
                ViewBag.PeriodosAcademicos = viewModel.PeriodosDisponibles;
                ViewBag.MateriasDisponibles = viewModel.MateriasDisponibles;

                // Establecer período por defecto si no hay uno seleccionado
                if (viewModel.PeriodoAcademicoId == 0)
                {
                    var periodoActualId = HttpContext.Session.GetInt32("PeriodoAcademicoId");
                    if (periodoActualId.HasValue)
                    {
                        viewModel.PeriodoAcademicoId = periodoActualId.Value;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar datos para formulario");
                // 🔧 CORRECCIÓN: Usar SelectListItem del framework
                // Inicializar listas vacías en caso de error
                viewModel.PeriodosDisponibles = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();
                viewModel.MateriasDisponibles = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();
                viewModel.TiposGrupo = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();
                ViewBag.PeriodosAcademicos = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();
                ViewBag.MateriasDisponibles = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();
            }
        }

        // ?? CORRECCIÓN: Sobrecarga específica para EditarGrupoViewModel
        private async Task CargarDatosParaFormulario(EditarGrupoViewModel viewModel)
        {
            try
            {
                var periodos = await _context.PeriodosAcademicos
                    .Where(p => p.Estado == "Activo")
                    .ToListAsync();

                // 🔧 NUEVA IMPLEMENTACIÓN: Cargar tipos desde el catálogo
                var tiposGrupo = await _context.TiposGrupo
                    .Where(t => t.Estado == "Activo")
                    .OrderBy(t => t.Nombre)
                    .ToListAsync();

                viewModel.TiposGrupo = tiposGrupo
                    .Select(t => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { 
                        Value = t.IdTipoGrupo.ToString(), 
                        Text = t.Nombre 
                    })
                    .ToList();

                // Cargar períodos en el ViewModel usando SelectListItem
                viewModel.PeriodosDisponibles = periodos
                    .Select(p => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                    {
                        Value = p.Id.ToString(),
                        Text = p.Nombre,
                        Selected = p.Id == viewModel.PeriodoAcademicoId
                    })
                    .ToList();

                var materias = await _context.Materias
                    .Where(m => m.Activa)
                    .ToListAsync();

                // Para compatibilidad con el checkbox en la vista Edit
                ViewBag.MateriasDisponibles = materias;

                // Cargar instituciones
                var instituciones = await _context.Instituciones
                    .Where(i => i.Estado)
                    .OrderBy(i => i.Nombre)
                    .ToListAsync();

                viewModel.InstitucionesDisponibles = instituciones
                    .Select(i => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                    {
                        Value = i.Id.ToString(),
                        Text = i.Nombre,
                        Selected = i.Id == viewModel.InstitucionId
                    })
                    .ToList();

                // Establecer período por defecto si no hay uno seleccionado
                if (viewModel.PeriodoAcademicoId == 0)
                {
                    var periodoActualId = HttpContext.Session.GetInt32("PeriodoAcademicoId");
                    if (periodoActualId.HasValue)
                    {
                        viewModel.PeriodoAcademicoId = periodoActualId.Value;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar datos para formulario de edición");
                // Inicializar listas vacías en caso de error
                viewModel.TiposGrupo = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();
                ViewBag.PeriodosAcademicos = new List<PeriodoAcademicoDropdownViewModel>();
                ViewBag.MateriasDisponibles = new List<object>();
            }
        }

        // GET: GruposEstudiantes/ObtenerMateriasDisponibles/5
        public async Task<IActionResult> ObtenerMateriasDisponibles(int id)
        {
            try
            {
                var grupo = await _context.GruposEstudiantes.FindAsync(id);
                if (grupo == null)
                {
                    return NotFound();
                }

                // Obtener todas las materias activas
                var todasLasMaterias = await _context.Materias
                    .Where(m => m.Activa)
                    .Select(m => new {
                        materiaId = m.MateriaId,
                        codigo = m.Codigo,
                        nombre = m.Nombre
                    })
                    .OrderBy(m => m.codigo)
                    .ToListAsync();

                // Obtener materias ya asociadas al grupo
                var materiasAsociadas = await _context.GrupoMaterias
                    .Where(gm => gm.GrupoId == id && gm.Estado == EstadoAsignacion.Activo)
                    .Select(gm => gm.MateriaId)
                    .ToListAsync();

                return Json(new { 
                    materias = todasLasMaterias,
                    materiasAsociadas = materiasAsociadas
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener materias disponibles para grupo {GrupoId}", id);
                return Json(new { 
                    error = true, 
                    message = "Error al cargar las materias" 
                });
            }
        }

        // POST: GruposEstudiantes/AsignarMaterias
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AsignarMaterias(int grupoId, List<int> materiasSeleccionadas, string? observaciones)
        {
            try
            {
                var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

                // Obtener materias actualmente asociadas
                var materiasActuales = await _context.GrupoMaterias
                    .Where(gm => gm.GrupoId == grupoId && gm.Estado == EstadoAsignacion.Activo)
                    .ToListAsync();

                // Materias a desasociar (están actualmente pero no en la nueva selección)
                var materiasADesasociar = materiasActuales
                    .Where(gm => !materiasSeleccionadas.Contains(gm.MateriaId))
                    .ToList();

                // Materias a asociar (están en la nueva selección pero no actualmente asociadas)
                var materiasActualesIds = materiasActuales.Select(gm => gm.MateriaId).ToList();
                var nuevasMaterias = materiasSeleccionadas
                    .Where(id => !materiasActualesIds.Contains(id))
                    .ToList();

                // Desasociar materias
                foreach (var materia in materiasADesasociar)
                {
                    materia.Estado = EstadoAsignacion.Inactivo;
                }

                // Asociar nuevas materias
                foreach (var materiaId in nuevasMaterias)
                {
                    _context.GrupoMaterias.Add(new GrupoMateria
                    {
                        GrupoId = grupoId,
                        MateriaId = materiaId,
                        Estado = EstadoAsignacion.Activo,
                        FechaAsignacion = DateTime.Now,
                        Observaciones = observaciones
                    });
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation("Materias actualizadas para grupo {GrupoId} por usuario {UserId}", grupoId, usuarioId);
                TempData["SuccessMessage"] = "Las materias del grupo se han actualizado correctamente.";

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al asignar materias al grupo {GrupoId}", grupoId);
                return BadRequest("Error al actualizar las materias del grupo");
            }
        }

        // POST: GruposEstudiantes/DesasignarMateria
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DesasignarMateria(int grupoId, int materiaId)
        {
            try
            {
                var grupoMateria = await _context.GrupoMaterias
                    .FirstOrDefaultAsync(gm => gm.GrupoId == grupoId && 
                                             gm.MateriaId == materiaId && 
                                             gm.Estado == EstadoAsignacion.Activo);

                if (grupoMateria != null)
                {
                    grupoMateria.Estado = EstadoAsignacion.Inactivo;
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Materia desasociada correctamente del grupo.";
                }
                else
                {
                    TempData["ErrorMessage"] = "La materia no está asociada a este grupo.";
                }

                return RedirectToAction(nameof(Details), new { id = grupoId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al desasignar materia {MateriaId} del grupo {GrupoId}", materiaId, grupoId);
                TempData["ErrorMessage"] = "Error al desasociar la materia del grupo.";
                return RedirectToAction(nameof(Details), new { id = grupoId });
            }
        }

        // GET: GruposEstudiantes/GetEstudiantesDisponibles - AJAX endpoint
        [HttpGet]
        public async Task<IActionResult> GetEstudiantesDisponibles(int? grupoId = null)
        {
            try
            {
                _logger.LogInformation("📊 GetEstudiantesDisponibles llamado para grupo {GrupoId}", grupoId?.ToString() ?? "TODOS");

                int periodoAcademicoId;

                if (grupoId.HasValue)
                {
                    // Obtener grupo y su período académico
                    var grupo = await _context.GruposEstudiantes
                        .Where(g => g.GrupoId == grupoId.Value)
                        .Select(g => new { g.PeriodoAcademicoId })
                        .FirstOrDefaultAsync();

                    if (grupo == null)
                    {
                        _logger.LogWarning("⚠️ Grupo {GrupoId} no encontrado", grupoId);
                        return Json(new List<object>());
                    }

                    periodoAcademicoId = grupo.PeriodoAcademicoId;
                }
                else
                {
                    // Obtener período académico activo
                    var periodoActivo = await _context.PeriodosAcademicos
                        .Where(p => p.Activo)
                        .Select(p => p.Id)
                        .FirstOrDefaultAsync();

                    if (periodoActivo == 0)
                    {
                        _logger.LogWarning("⚠️ No se encontró un período académico activo");
                        return Json(new List<object>());
                    }

                    periodoAcademicoId = periodoActivo;
                }

                // Obtener estudiantes disponibles (del período especificado)
                var query = _context.Estudiantes
                    .Where(e => e.PeriodoAcademicoId == periodoAcademicoId);

                if (grupoId.HasValue)
                {
                    // Si se especifica un grupo, excluir estudiantes ya asignados a ese grupo
                    query = query.Where(e => !_context.EstudianteGrupos
                        .Any(eg => eg.EstudianteId == e.IdEstudiante && 
                                  eg.GrupoId == grupoId.Value && 
                                  eg.Estado == EstadoAsignacion.Activo));
                }
                else
                {
                    // Si no se especifica grupo, mostrar estudiantes sin asignar a ningún grupo
                    query = query.Where(e => !_context.EstudianteGrupos
                        .Any(eg => eg.EstudianteId == e.IdEstudiante && 
                                  eg.Estado == EstadoAsignacion.Activo));
                }

                var estudiantesDisponibles = await query
                    .Select(e => new {
                        estudianteId = e.IdEstudiante,
                        nombre = e.Nombre,
                        apellidos = e.Apellidos,
                        cedula = e.NumeroId,
                        email = e.DireccionCorreo
                    })
                    .OrderBy(e => e.apellidos)
                    .ThenBy(e => e.nombre)
                    .ToListAsync();

                var result = estudiantesDisponibles.Select(e => new {
                    estudianteId = e.estudianteId,
                    nombreCompleto = $"{e.nombre} {e.apellidos}",
                    cedula = e.cedula,
                    email = e.email
                }).ToList();

                _logger.LogInformation("📊 Encontrados {Count} estudiantes disponibles para período {PeriodoId}", 
                    result.Count, periodoAcademicoId);

                return Json(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al obtener estudiantes disponibles para grupo {GrupoId}", grupoId);
                return Json(new List<object>());
            }
        }

        // GET: GruposEstudiantes/GetEstudiantesAsignados - AJAX endpoint
        [HttpGet]
        public async Task<IActionResult> GetEstudiantesAsignados(int grupoId)
        {
            try
            {
                _logger.LogInformation("📊 GetEstudiantesAsignados llamado para grupo {GrupoId}", grupoId);

                // Obtener estudiantes asignados al grupo
                var estudiantesAsignados = await _context.EstudianteGrupos
                    .Where(eg => eg.GrupoId == grupoId && eg.Estado == EstadoAsignacion.Activo)
                    .Include(eg => eg.Estudiante)
                    .Select(eg => new {
                        estudianteId = eg.EstudianteId,
                        nombre = eg.Estudiante != null ? eg.Estudiante.Nombre : string.Empty,
                        apellidos = eg.Estudiante != null ? eg.Estudiante.Apellidos : string.Empty,
                        cedula = eg.Estudiante.NumeroId,
                        email = eg.Estudiante.DireccionCorreo,
                        fechaAsignacion = eg.FechaAsignacion
                    })
                    .OrderBy(eg => eg.apellidos)
                    .ThenBy(eg => eg.nombre)
                    .ToListAsync();

                var result = estudiantesAsignados.Select(eg => new {
                    estudianteId = eg.estudianteId,
                    nombreCompleto = $"{eg.nombre} {eg.apellidos}",
                    cedula = eg.cedula,
                    email = eg.email,
                    fechaAsignacion = eg.fechaAsignacion
                }).ToList();

                _logger.LogInformation("📊 Encontrados {Count} estudiantes asignados", result.Count);

                return Json(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al obtener estudiantes asignados para grupo {GrupoId}", grupoId);
                return Json(new List<object>());
            }
        }

        // POST: GruposEstudiantes/AsignarEstudiante - AJAX endpoint para asignación individual
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AsignarEstudiante(int grupoId, int estudianteId)
        {
            try
            {
                var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
                var resultado = await _grupoService.AsignarEstudiantesAsync(
                    grupoId,
                    new List<int> { estudianteId },
                    usuarioId,
                    "Asignación individual desde interfaz",
                    false);

                return Json(new { 
                    success = resultado.Exitoso, 
                    message = resultado.Mensaje 
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al asignar estudiante {EstudianteId} al grupo {GrupoId}", estudianteId, grupoId);
                return Json(new { 
                    success = false, 
                    message = "Error interno al asignar estudiante" 
                });
            }
        }

        // POST: GruposEstudiantes/AsignarEstudiantesMultiples - AJAX endpoint para asignación múltiple
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AsignarEstudiantesMultiples(int grupoId, List<int> estudiantesIds)
        {
            try
            {
                if (!estudiantesIds?.Any() ?? true)
                {
                    return Json(new { 
                        success = false, 
                        message = "No se seleccionaron estudiantes" 
                    });
                }

                var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
                var resultado = await _grupoService.AsignarEstudiantesAsync(
                    grupoId,
                    estudiantesIds != null ? estudiantesIds : new List<int>(),
                    usuarioId,
                    "Asignación múltiple desde interfaz",
                    false);

                return Json(new { 
                    success = resultado.Exitoso, 
                    message = resultado.Mensaje,
                    asignados = estudiantesIds != null ? estudiantesIds.Count : 0
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al asignar múltiples estudiantes al grupo {GrupoId}", grupoId);
                return Json(new { 
                    success = false, 
                    message = "Error interno al asignar estudiantes" 
                });
            }
        }

        // POST: GruposEstudiantes/DesasignarEstudianteSingle - AJAX endpoint para desasignación individual
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DesasignarEstudianteSingle(int grupoId, int estudianteId)
        {
            try
            {
                var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
                var resultado = await _grupoService.DesasignarEstudianteAsync(
                    grupoId, 
                    estudianteId, 
                    usuarioId, 
                    "Desasignación individual desde interfaz");

                return Json(new { 
                    success = resultado.Exitoso, 
                    message = resultado.Mensaje 
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al desasignar estudiante {EstudianteId} del grupo {GrupoId}", estudianteId, grupoId);
                return Json(new { 
                    success = false, 
                    message = "Error interno al desasignar estudiante" 
                });
            }
        }

        // POST: GruposEstudiantes/DesasignarEstudiantesMultiplesAjax - AJAX endpoint para desasignación múltiple
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DesasignarEstudiantesMultiplesAjax(int grupoId, List<int> estudiantesIds, string? motivo)
        {
            try
            {
                if (!estudiantesIds?.Any() ?? true)
                {
                    return Json(new { 
                        success = false, 
                        message = "No se seleccionaron estudiantes para desasignar" 
                    });
                }

                var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
                var exitosos = 0;
                var errores = 0;
                var erroresDetalle = new List<string>();

                _logger.LogInformation("🔄 Iniciando desasignación múltiple AJAX de {Count} estudiantes del grupo {GrupoId}", estudiantesIds?.Count ?? 0, grupoId);

                foreach (var estudianteId in estudiantesIds ?? new List<int>())
                {
                    try
                    {
                        var resultado = await _grupoService.DesasignarEstudianteAsync(grupoId, estudianteId, usuarioId, motivo ?? "Desasignación múltiple desde interfaz");
                        if (resultado.Exitoso)
                        {
                            exitosos++;
                        }
                        else
                        {
                            errores++;
                            erroresDetalle.Add($"Estudiante ID {estudianteId}: {resultado.Mensaje}");
                        }
                    }
                    catch (Exception ex)
                    {
                        errores++;
                        erroresDetalle.Add($"Estudiante ID {estudianteId}: Error interno");
                        _logger.LogError(ex, "Error al desasignar estudiante {EstudianteId} del grupo {GrupoId}", estudianteId, grupoId);
                    }
                }

                _logger.LogInformation("✅ Desasignación múltiple AJAX completada: {Exitosos} exitosos, {Errores} errores", exitosos, errores);

                if (exitosos > 0 && errores == 0)
                {
                    return Json(new { 
                        success = true, 
                        message = $"Se desasignaron exitosamente {exitosos} estudiante(s) del grupo",
                        desasignados = exitosos
                    });
                }
                else if (exitosos > 0 && errores > 0)
                {
                    return Json(new { 
                        success = true, 
                        message = $"Se desasignaron {exitosos} estudiante(s) exitosamente, pero {errores} tuvieron errores",
                        desasignados = exitosos,
                        errores = errores,
                        detalleErrores = string.Join("; ", erroresDetalle)
                    });
                }
                else
                {
                    return Json(new { 
                        success = false, 
                        message = $"No se pudo desasignar ningún estudiante. Errores: {string.Join("; ", erroresDetalle)}"
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error general en desasignación múltiple AJAX del grupo {GrupoId}", grupoId);
                return Json(new { 
                    success = false, 
                    message = "Error interno al desasignar estudiantes" 
                });
            }
        }

        // 🔧 DIAGNÓSTICO: Método para debuggear problemas de creación
        [HttpGet]
        public async Task<IActionResult> DiagnosticoCreacion()
        {
            try
            {
                var diagnostico = new
                {
                    PeriodosDisponibles = await _context.PeriodosAcademicos
                        .Where(p => p.Estado == "Activo")
                        .Select(p => new { p.Id, p.Nombre, p.Estado })
                        .ToListAsync(),
                    MateriasDisponibles = await _context.Materias
                        .Where(m => m.Activa)
                        .Select(m => new { m.MateriaId, m.Codigo, m.Nombre })
                        .Take(10)
                        .ToListAsync(),
                    TiposGrupoEnum = Enum.GetValues<TipoGrupo>()
                        .Select(t => new { Valor = (int)t, Nombre = t.ToString() })
                        .ToList(),
                    EstadosGrupoEnum = Enum.GetValues<EstadoGrupo>()
                        .Select(e => new { Valor = (int)e, Nombre = e.ToString() })
                        .ToList(),
                    ConfiguracionServidor = new
                    {
                        Environment.Version,
                        Environment.Is64BitProcess,
                        Encoding = Console.OutputEncoding.EncodingName,
                        Culture = System.Globalization.CultureInfo.CurrentCulture.Name
                    }
                };

                return Json(diagnostico);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en diagnóstico de creación");
                return Json(new { error = ex.Message, stackTrace = ex.StackTrace });
            }
        }

        // GET: GruposEstudiantes/DebugEstudiantes - Endpoint de debug para verificar estudiantes
        [HttpGet]
        public async Task<IActionResult> DebugEstudiantes()
        {
            try
            {
                var totalEstudiantes = await _context.Estudiantes.CountAsync();
                var periodoActivo = await _context.PeriodosAcademicos
                    .Where(p => p.Activo)
                    .FirstOrDefaultAsync();
                
                var totalGrupos = await _context.GruposEstudiantes.CountAsync();
                var totalAsignaciones = await _context.EstudianteGrupos
                    .Where(eg => eg.Estado == EstadoAsignacion.Activo)
                    .CountAsync();

                var primerosEstudiantes = await _context.Estudiantes
                    .Take(5)
                    .Select(e => new { 
                        e.IdEstudiante, 
                        Nombre = e.Nombre,
                        Apellidos = e.Apellidos,
                        e.NumeroId, 
                        e.PeriodoAcademicoId 
                    })
                    .ToListAsync();

                var estudiantesConNombreCompleto = primerosEstudiantes.Select(e => new {
                    e.IdEstudiante,
                    NombreCompleto = $"{e.Nombre} {e.Apellidos}",
                    e.NumeroId,
                    e.PeriodoAcademicoId
                }).ToList();

                var debug = new {
                    totalEstudiantes,
                    periodoActivo = periodoActivo != null ? new { 
                        id = periodoActivo.Id, 
                        nombre = periodoActivo.Nombre 
                    } : null,
                    totalGrupos,
                    totalAsignaciones,
                    primerosEstudiantes = estudiantesConNombreCompleto,
                    timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                };

                _logger.LogInformation("🔍 DEBUG: {Debug}", System.Text.Json.JsonSerializer.Serialize(debug));

                return Json(debug);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error en DebugEstudiantes");
                return Json(new { error = ex.Message });
            }
        }
    }
}