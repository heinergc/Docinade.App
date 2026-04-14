using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RubricasApp.Web.Data;
using RubricasApp.Web.Models;
using RubricasApp.Web.Services;
using RubricasApp.Web.ViewModels;
using System.Security.Claims;

namespace RubricasApp.Web.Controllers
{
    [Authorize]
    public class AsistenciaLeccionController : Controller
    {
        private readonly IAsistenciaLeccionService _asistenciaService;
        private readonly IHorarioService _horarioService;
        private readonly RubricasDbContext _context;
        private readonly ILogger<AsistenciaLeccionController> _logger;

        public AsistenciaLeccionController(
            IAsistenciaLeccionService asistenciaService,
            IHorarioService horarioService,
            RubricasDbContext context,
            ILogger<AsistenciaLeccionController> logger)
        {
            _asistenciaService = asistenciaService;
            _horarioService = horarioService;
            _context = context;
            _logger = logger;
        }

        // GET: AsistenciaLeccion
        public async Task<IActionResult> Index(int? grupoId, DateTime? fecha)
        {
            try
            {
                fecha = fecha ?? DateTime.Today;

                ViewBag.Grupos = await _context.GruposEstudiantes
                    .OrderBy(g => g.Nombre)
                    .Select(g => new SelectListItem { Value = g.GrupoId.ToString(), Text = g.Nombre })
                    .ToListAsync();

                ViewBag.GrupoSeleccionado = grupoId;
                ViewBag.FechaSeleccionada = fecha.Value;

                if (!grupoId.HasValue)
                {
                    return View(new List<Leccion>());
                }

                // Obtener lecciones del día para el grupo
                var diaSemana = fecha.Value.DayOfWeek;
                var lecciones = await _horarioService.ObtenerHorariosPorGrupoYDiaAsync(grupoId.Value, diaSemana);

                // Para cada lección, obtener el conteo de asistencias registradas
                foreach (var leccion in lecciones)
                {
                    var asistencias = await _asistenciaService.ObtenerAsistenciasPorLeccionAsync(leccion.IdLeccion, fecha.Value);
                    leccion.AsistenciasRegistradas = asistencias.Count();
                }

                ViewBag.Grupo = await _context.GruposEstudiantes
                    .FirstOrDefaultAsync(g => g.GrupoId == grupoId.Value);

                return View(lecciones.OrderBy(l => l.HoraInicio).ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener lecciones del día");
                TempData["ErrorMessage"] = "Error al cargar las lecciones";
                return View(new List<Leccion>());
            }
        }

        // GET: AsistenciaLeccion/TomarAsistencia
        public async Task<IActionResult> TomarAsistencia(int idLeccion, DateTime fecha)
        {
            try
            {
                // Obtener información de la lección
                var leccion = await _horarioService.ObtenerHorarioPorIdAsync(idLeccion);
                if (leccion == null)
                {
                    TempData["ErrorMessage"] = "Lección no encontrada";
                    return RedirectToAction(nameof(Index));
                }

                // Obtener estudiantes del grupo
                var grupo = await _context.GruposEstudiantes
                    .Include(g => g.EstudianteGrupos)
                    .ThenInclude(eg => eg.Estudiante)
                    .FirstOrDefaultAsync(g => g.GrupoId == leccion.IdGrupo);

                if (grupo == null)
                {
                    TempData["ErrorMessage"] = "Grupo no encontrado";
                    return RedirectToAction(nameof(Index));
                }

                var estudiantes = grupo.EstudianteGrupos
                    .Where(eg => eg.Estado == EstadoAsignacion.Activo)
                    .Select(eg => eg.Estudiante)
                    .OrderBy(e => e.Apellidos)
                    .ThenBy(e => e.Nombre)
                    .ToList();

                // Obtener asistencias ya registradas para esta lección/fecha
                var asistenciasExistentes = await _asistenciaService.ObtenerAsistenciasPorLeccionAsync(idLeccion, fecha);
                var asistenciasDictionary = asistenciasExistentes.ToDictionary(a => a.EstudianteId, a => a);

                // Crear vista modelo
                var viewModel = new TomarAsistenciaViewModel
                {
                    Leccion = leccion,
                    FechaAsistencia = fecha,
                    RegistrosAsistencia = estudiantes.Select(e => new RegistroAsistenciaViewModel
                    {
                        EstudianteId = e.IdEstudiante,
                        NombreCompleto = $"{e.Apellidos}, {e.Nombre}",
                        Identificacion = e.NumeroId,
                        Estado = asistenciasDictionary.ContainsKey(e.IdEstudiante) 
                            ? asistenciasDictionary[e.IdEstudiante].Estado 
                            : "N",
                        Observaciones = asistenciasDictionary.ContainsKey(e.IdEstudiante) 
                            ? asistenciasDictionary[e.IdEstudiante].Observaciones 
                            : null,
                        Justificacion = asistenciasDictionary.ContainsKey(e.IdEstudiante) 
                            ? asistenciasDictionary[e.IdEstudiante].Justificacion 
                            : null,
                        AsistenciaId = asistenciasDictionary.ContainsKey(e.IdEstudiante) 
                            ? asistenciasDictionary[e.IdEstudiante].AsistenciaId 
                            : 0
                    }).ToList()
                };

                ViewBag.Estados = new SelectList(new[]
                {
                    new { Value = "P", Text = "Presente" },
                    new { Value = "A", Text = "Ausente" },
                    new { Value = "T", Text = "Tardanza" },
                    new { Value = "AJ", Text = "Ausencia Justificada" },
                    new { Value = "N", Text = "No Marcado" }
                }, "Value", "Text");

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al preparar toma de asistencia");
                TempData["ErrorMessage"] = "Error al cargar la interfaz de asistencia";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: AsistenciaLeccion/GuardarAsistencia
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GuardarAsistencia(TomarAsistenciaViewModel viewModel)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "system";
                var leccion = await _horarioService.ObtenerHorarioPorIdAsync(viewModel.IdLeccion);

                if (leccion == null)
                {
                    TempData["ErrorMessage"] = "Lección no encontrada";
                    return RedirectToAction(nameof(Index));
                }

                // Crear lista de asistencias
                var asistencias = viewModel.RegistrosAsistencia.Select(r => new Asistencia
                {
                    EstudianteId = r.EstudianteId,
                    GrupoId = leccion.IdGrupo,
                    MateriaId = leccion.MateriaId,
                    IdLeccion = leccion.IdLeccion,
                    Fecha = viewModel.FechaAsistencia,
                    Estado = r.Estado,
                    Observaciones = r.Observaciones,
                    Justificacion = r.Justificacion,
                    HoraLlegada = r.Estado == "T" ? DateTime.Now.TimeOfDay : (TimeSpan?)null
                }).ToList();

                // Registrar asistencias
                var resultado = await _asistenciaService.RegistrarAsistenciasMasivasAsync(asistencias, userId);

                if (resultado.Exito)
                {
                    TempData["SuccessMessage"] = resultado.Mensaje;
                    return RedirectToAction(nameof(Index), new { grupoId = leccion.IdGrupo, fecha = viewModel.FechaAsistencia });
                }
                else
                {
                    TempData["ErrorMessage"] = resultado.Mensaje;
                    return RedirectToAction(nameof(TomarAsistencia), new { idLeccion = viewModel.IdLeccion, fecha = viewModel.FechaAsistencia });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar asistencias");
                TempData["ErrorMessage"] = "Error al guardar las asistencias";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: AsistenciaLeccion/Historial
        public async Task<IActionResult> Historial(int? estudianteId, int? grupoId, DateTime? fechaInicio, DateTime? fechaFin)
        {
            try
            {
                fechaInicio = fechaInicio ?? DateTime.Today.AddMonths(-1);
                fechaFin = fechaFin ?? DateTime.Today;

                ViewBag.Estudiantes = await _context.Estudiantes
                    .OrderBy(e => e.Apellidos)
                    .ThenBy(e => e.Nombre)
                    .Select(e => new SelectListItem 
                    { 
                        Value = e.IdEstudiante.ToString(), 
                        Text = $"{e.Apellidos}, {e.Nombre}" 
                    })
                    .ToListAsync();

                ViewBag.Grupos = await _context.GruposEstudiantes
                    .OrderBy(g => g.Nombre)
                    .Select(g => new SelectListItem { Value = g.GrupoId.ToString(), Text = g.Nombre })
                    .ToListAsync();

                ViewBag.EstudianteSeleccionado = estudianteId;
                ViewBag.GrupoSeleccionado = grupoId;
                ViewBag.FechaInicio = fechaInicio.Value;
                ViewBag.FechaFin = fechaFin.Value;

                if (!estudianteId.HasValue)
                {
                    return View(new List<Asistencia>());
                }

                // Obtener historial de asistencias
                var asistencias = await _asistenciaService.ObtenerAsistenciasPorEstudianteAsync(
                    estudianteId.Value, 
                    fechaInicio.Value, 
                    fechaFin.Value);

                // Filtrar por grupo si se especifica
                if (grupoId.HasValue)
                {
                    asistencias = asistencias.Where(a => a.GrupoId == grupoId.Value);
                }

                // Obtener estadísticas
                var estadisticas = await _asistenciaService.ObtenerEstadisticasAsistenciaEstudianteAsync(
                    estudianteId.Value,
                    fechaInicio.Value,
                    fechaFin.Value);

                var porcentaje = await _asistenciaService.CalcularPorcentajeAsistenciaAsync(
                    estudianteId.Value,
                    grupoId,
                    null,
                    fechaInicio.Value,
                    fechaFin.Value);

                ViewBag.Estadisticas = estadisticas;
                ViewBag.PorcentajeAsistencia = porcentaje;

                ViewBag.Estudiante = await _context.Estudiantes
                    .FirstOrDefaultAsync(e => e.IdEstudiante == estudianteId.Value);

                return View(asistencias.ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener historial de asistencia");
                TempData["ErrorMessage"] = "Error al cargar el historial";
                return View(new List<Asistencia>());
            }
        }

        // GET: AsistenciaLeccion/Estadisticas
        public async Task<IActionResult> Estadisticas(int? estudianteId, int? grupoId, DateTime? fechaInicio, DateTime? fechaFin)
        {
            try
            {
                fechaInicio = fechaInicio ?? DateTime.Today.AddMonths(-1);
                fechaFin = fechaFin ?? DateTime.Today;

                var viewModel = new EstadisticasAsistenciaViewModel
                {
                    EstudianteId = estudianteId,
                    GrupoId = grupoId,
                    FechaInicio = fechaInicio.Value,
                    FechaFin = fechaFin.Value
                };

                ViewBag.Estudiantes = await _context.Estudiantes
                    .OrderBy(e => e.Apellidos)
                    .ThenBy(e => e.Nombre)
                    .Select(e => new SelectListItem 
                    { 
                        Value = e.IdEstudiante.ToString(), 
                        Text = $"{e.Apellidos}, {e.Nombre}" 
                    })
                    .ToListAsync();

                ViewBag.Grupos = await _context.GruposEstudiantes
                    .OrderBy(g => g.Nombre)
                    .Select(g => new SelectListItem { Value = g.GrupoId.ToString(), Text = g.Nombre })
                    .ToListAsync();

                ViewBag.EstudianteSeleccionado = estudianteId;
                ViewBag.GrupoSeleccionado = grupoId;
                ViewBag.FechaInicio = fechaInicio.Value;
                ViewBag.FechaFin = fechaFin.Value;

                if (estudianteId.HasValue)
                {
                    var estadisticas = await _asistenciaService.ObtenerEstadisticasAsistenciaEstudianteAsync(
                        estudianteId.Value,
                        fechaInicio.Value,
                        fechaFin.Value);

                    var porcentaje = await _asistenciaService.CalcularPorcentajeAsistenciaAsync(
                        estudianteId.Value,
                        grupoId,
                        null,
                        fechaInicio.Value,
                        fechaFin.Value);

                    var ausencias = await _asistenciaService.ObtenerAusenciasEstudianteAsync(
                        estudianteId.Value,
                        true,
                        fechaInicio.Value,
                        fechaFin.Value);

                    viewModel.Estadisticas = estadisticas;
                    viewModel.PorcentajeAsistencia = porcentaje;
                    viewModel.Ausencias = ausencias.ToList();

                    ViewBag.Estadisticas = estadisticas;
                    ViewBag.PorcentajeAsistencia = porcentaje;
                    ViewBag.Ausencias = ausencias.ToList();

                    ViewBag.Estudiante = await _context.Estudiantes
                        .FirstOrDefaultAsync(e => e.IdEstudiante == estudianteId.Value);
                }
                else if (grupoId.HasValue)
                {
                    var resumenGrupo = await _asistenciaService.ObtenerResumenAsistenciaGrupoAsync(
                        grupoId.Value,
                        fechaInicio.Value,
                        fechaFin.Value);

                    viewModel.ResumenGrupo = resumenGrupo;
                    ViewBag.ResumenGrupo = resumenGrupo;

                    ViewBag.Grupo = await _context.GruposEstudiantes
                        .Include(g => g.EstudianteGrupos)
                        .ThenInclude(eg => eg.Estudiante)
                        .FirstOrDefaultAsync(g => g.GrupoId == grupoId.Value);
                }

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener estadísticas de asistencia");
                TempData["ErrorMessage"] = "Error al cargar las estadísticas";
                
                var emptyModel = new EstadisticasAsistenciaViewModel
                {
                    FechaInicio = fechaInicio ?? DateTime.Today.AddMonths(-1),
                    FechaFin = fechaFin ?? DateTime.Today
                };
                
                return View(emptyModel);
            }
        }

        // POST: AsistenciaLeccion/ActualizarEstado
        [HttpPost]
        public async Task<IActionResult> ActualizarEstado(int asistenciaId, string nuevoEstado, string justificacion, string observaciones)
        {
            try
            {
                var asistencia = await _asistenciaService.ObtenerAsistenciaPorIdAsync(asistenciaId);
                if (asistencia == null)
                {
                    return Json(new { success = false, message = "Registro de asistencia no encontrado" });
                }

                asistencia.Estado = nuevoEstado;
                asistencia.Justificacion = justificacion;
                asistencia.Observaciones = observaciones;

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "system";
                var resultado = await _asistenciaService.ActualizarAsistenciaAsync(asistencia, userId);

                return Json(new { success = resultado.Exito, message = resultado.Mensaje });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar estado de asistencia");
                return Json(new { success = false, message = "Error al actualizar el estado" });
            }
        }

        // GET: AsistenciaLeccion/Ausencias
        public async Task<IActionResult> Ausencias(int? grupoId, DateTime? fechaInicio, DateTime? fechaFin, bool incluirJustificadas = false)
        {
            try
            {
                fechaInicio = fechaInicio ?? DateTime.Today.AddDays(-7);
                fechaFin = fechaFin ?? DateTime.Today;

                ViewBag.Grupos = await _context.GruposEstudiantes
                    .OrderBy(g => g.Nombre)
                    .Select(g => new SelectListItem { Value = g.GrupoId.ToString(), Text = g.Nombre })
                    .ToListAsync();

                ViewBag.GrupoSeleccionado = grupoId;
                ViewBag.FechaInicio = fechaInicio.Value;
                ViewBag.FechaFin = fechaFin.Value;
                ViewBag.IncluirJustificadas = incluirJustificadas;

                if (!grupoId.HasValue)
                {
                    return View(new List<Asistencia>());
                }

                // Obtener estudiantes del grupo
                var grupo = await _context.GruposEstudiantes
                    .Include(g => g.EstudianteGrupos)
                    .FirstOrDefaultAsync(g => g.GrupoId == grupoId.Value);

                if (grupo == null)
                {
                    return View(new List<Asistencia>());
                }

                var estudiantesGrupo = grupo.EstudianteGrupos
                    .Where(eg => eg.Estado == EstadoAsignacion.Activo)
                    .Select(eg => eg.EstudianteId)
                    .ToList();

                // Obtener ausencias de cada estudiante
                var ausenciasPorEstudiante = new List<Asistencia>();
                foreach (var estudianteId in estudiantesGrupo)
                {
                    var ausencias = await _asistenciaService.ObtenerAusenciasEstudianteAsync(
                        estudianteId,
                        incluirJustificadas,
                        fechaInicio.Value,
                        fechaFin.Value);

                    ausenciasPorEstudiante.AddRange(ausencias);
                }

                // Ordenar por fecha descendente y luego por estudiante
                var ausenciasOrdenadas = ausenciasPorEstudiante
                    .OrderByDescending(a => a.Fecha)
                    .ThenBy(a => a.Estudiante.Apellidos)
                    .ThenBy(a => a.Estudiante.Nombre)
                    .ToList();

                ViewBag.Grupo = await _context.GruposEstudiantes
                    .FirstOrDefaultAsync(g => g.GrupoId == grupoId.Value);

                return View(ausenciasOrdenadas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener reporte de ausencias");
                TempData["ErrorMessage"] = "Error al cargar el reporte de ausencias";
                return View(new List<Asistencia>());
            }
        }
    }
}
