using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RubricasApp.Web.Data;
using RubricasApp.Web.Models;
using RubricasApp.Web.Services;
using System.Security.Claims;

namespace RubricasApp.Web.Controllers
{
    [Authorize]
    public class HorariosController : Controller
    {
        private readonly IHorarioService _horarioService;
        private readonly RubricasDbContext _context;
        private readonly ILogger<HorariosController> _logger;

        public HorariosController(
            IHorarioService horarioService,
            RubricasDbContext context,
            ILogger<HorariosController> logger)
        {
            _horarioService = horarioService;
            _context = context;
            _logger = logger;
        }

        // GET: Horarios
        public async Task<IActionResult> Index(int? grupoId, int? materiaId, DayOfWeek? diaSemana, bool? soloActivos)
        {
            try
            {
                IEnumerable<Leccion> horarios;

                // Aplicar filtros
                if (grupoId.HasValue)
                {
                    horarios = await _horarioService.ObtenerHorariosPorGrupoAsync(grupoId.Value);
                }
                else if (materiaId.HasValue)
                {
                    horarios = await _horarioService.ObtenerHorariosPorMateriaAsync(materiaId.Value);
                }
                else if (diaSemana.HasValue)
                {
                    horarios = await _horarioService.ObtenerHorariosPorDiaAsync(diaSemana.Value);
                }
                else
                {
                    horarios = await _horarioService.ObtenerTodosHorariosAsync();
                }

                // Filtrar por activos
                if (soloActivos.GetValueOrDefault(true))
                {
                    horarios = horarios.Where(h => h.Activa);
                }

                // Cargar datos para filtros
                ViewBag.Grupos = await _context.GruposEstudiantes
                    .OrderBy(g => g.Nombre)
                    .Select(g => new SelectListItem { Value = g.GrupoId.ToString(), Text = g.Nombre })
                    .ToListAsync();

                ViewBag.Materias = await _context.Materias
                    .OrderBy(m => m.Nombre)
                    .Select(m => new SelectListItem { Value = m.MateriaId.ToString(), Text = m.Nombre })
                    .ToListAsync();

                ViewBag.DiasSemanaDictionary = new Dictionary<DayOfWeek, string>
                {
                    { DayOfWeek.Monday, "Lunes" },
                    { DayOfWeek.Tuesday, "Martes" },
                    { DayOfWeek.Wednesday, "Miércoles" },
                    { DayOfWeek.Thursday, "Jueves" },
                    { DayOfWeek.Friday, "Viernes" },
                    { DayOfWeek.Saturday, "Sábado" },
                    { DayOfWeek.Sunday, "Domingo" }
                };

                ViewBag.GrupoIdSeleccionado = grupoId;
                ViewBag.MateriaIdSeleccionada = materiaId;
                ViewBag.DiaSemanaSeleccionado = diaSemana;
                ViewBag.SoloActivos = soloActivos.GetValueOrDefault(true);

                return View(horarios.OrderBy(h => h.DiaSemana).ThenBy(h => h.HoraInicio).ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener horarios");
                TempData["ErrorMessage"] = "Error al cargar los horarios";
                return View(new List<Leccion>());
            }
        }

        // GET: Horarios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var horario = await _horarioService.ObtenerHorarioPorIdAsync(id.Value);
            if (horario == null)
            {
                return NotFound();
            }

            ViewBag.DiasSemanaDictionary = new Dictionary<DayOfWeek, string>
            {
                { DayOfWeek.Monday, "Lunes" },
                { DayOfWeek.Tuesday, "Martes" },
                { DayOfWeek.Wednesday, "Miércoles" },
                { DayOfWeek.Thursday, "Jueves" },
                { DayOfWeek.Friday, "Viernes" },
                { DayOfWeek.Saturday, "Sábado" },
                { DayOfWeek.Sunday, "Domingo" }
            };

            return View(horario);
        }

        // GET: Horarios/Create
        public async Task<IActionResult> Create(int? grupoId, int? materiaId)
        {
            await CargarDatosVista();

            var horario = new Leccion
            {
                Activa = true
            };

            if (grupoId.HasValue)
                horario.IdGrupo = grupoId.Value;

            if (materiaId.HasValue)
                horario.MateriaId = materiaId.Value;

            return View(horario);
        }

        // POST: Horarios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdGrupo,MateriaId,NumeroBloque,DiaSemana,HoraInicio,HoraFin,Activa,Observaciones")] Leccion leccion)
        {
            try
            {
                // Validar datos
                var erroresValidacion = await _horarioService.ValidarHorarioAsync(leccion);
                if (erroresValidacion.Any())
                {
                    foreach (var error in erroresValidacion)
                    {
                        ModelState.AddModelError(string.Empty, error);
                    }
                    await CargarDatosVista();
                    return View(leccion);
                }

                // Verificar conflictos
                var conflictos = await _horarioService.ObtenerConflictosHorarioAsync(leccion);
                if (conflictos.Any())
                {
                    TempData["WarningMessage"] = $"Se detectaron {conflictos.Count} conflictos de horario. Por favor, revise los horarios existentes.";
                    ViewBag.Conflictos = conflictos;
                    await CargarDatosVista();
                    return View(leccion);
                }

                // Crear horario
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "system";
                var resultado = await _horarioService.CrearHorarioAsync(leccion, userId);

                if (resultado.Exito)
                {
                    TempData["SuccessMessage"] = resultado.Mensaje;
                    return RedirectToAction(nameof(Index), new { grupoId = leccion.IdGrupo });
                }
                else
                {
                    TempData["ErrorMessage"] = resultado.Mensaje;
                    await CargarDatosVista();
                    return View(leccion);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear horario");
                TempData["ErrorMessage"] = "Error al crear el horario";
                await CargarDatosVista();
                return View(leccion);
            }
        }

        // GET: Horarios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var horario = await _horarioService.ObtenerHorarioPorIdAsync(id.Value);
            if (horario == null)
            {
                return NotFound();
            }

            await CargarDatosVista();
            return View(horario);
        }

        // POST: Horarios/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdLeccion,IdGrupo,MateriaId,NumeroBloque,DiaSemana,HoraInicio,HoraFin,Activa,Observaciones")] Leccion leccion)
        {
            if (id != leccion.IdLeccion)
            {
                return NotFound();
            }

            try
            {
                // Validar datos
                var erroresValidacion = await _horarioService.ValidarHorarioAsync(leccion);
                if (erroresValidacion.Any())
                {
                    foreach (var error in erroresValidacion)
                    {
                        ModelState.AddModelError(string.Empty, error);
                    }
                    await CargarDatosVista();
                    return View(leccion);
                }

                // Verificar conflictos (excluyendo el mismo registro)
                var conflictos = await _horarioService.ObtenerConflictosHorarioAsync(leccion);
                conflictos = conflictos.Where(c => c.IdLeccion != id).ToList();
                
                if (conflictos.Any())
                {
                    TempData["WarningMessage"] = $"Se detectaron {conflictos.Count} conflictos de horario. Por favor, revise los horarios existentes.";
                    ViewBag.Conflictos = conflictos;
                    await CargarDatosVista();
                    return View(leccion);
                }

                // Actualizar horario
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "system";
                var resultado = await _horarioService.ActualizarHorarioAsync(leccion, userId);

                if (resultado.Exito)
                {
                    TempData["SuccessMessage"] = resultado.Mensaje;
                    return RedirectToAction(nameof(Index), new { grupoId = leccion.IdGrupo });
                }
                else
                {
                    TempData["ErrorMessage"] = resultado.Mensaje;
                    await CargarDatosVista();
                    return View(leccion);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar horario {IdLeccion}", id);
                TempData["ErrorMessage"] = "Error al actualizar el horario";
                await CargarDatosVista();
                return View(leccion);
            }
        }

        // POST: Horarios/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, int? grupoId)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "system";
                var resultado = await _horarioService.EliminarHorarioAsync(id, userId);

                if (resultado.Exito)
                {
                    TempData["SuccessMessage"] = resultado.Mensaje;
                }
                else
                {
                    TempData["ErrorMessage"] = resultado.Mensaje;
                }

                if (grupoId.HasValue)
                {
                    return RedirectToAction(nameof(Index), new { grupoId });
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar horario {IdLeccion}", id);
                TempData["ErrorMessage"] = "Error al eliminar el horario";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Horarios/ToggleActivo/5
        [HttpPost]
        public async Task<IActionResult> ToggleActivo(int id)
        {
            try
            {
                var horario = await _horarioService.ObtenerHorarioPorIdAsync(id);
                if (horario == null)
                {
                    return Json(new { success = false, message = "Horario no encontrado" });
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "system";
                var resultado = await _horarioService.ActivarDesactivarHorarioAsync(id, !horario.Activa, userId);

                return Json(new { success = resultado.Exito, message = resultado.Mensaje });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cambiar estado de horario {IdLeccion}", id);
                return Json(new { success = false, message = "Error al cambiar el estado" });
            }
        }

        // GET: Horarios/GetConflictos
        [HttpGet]
        public async Task<IActionResult> GetConflictos(int idGrupo, DayOfWeek diaSemana, string horaInicio, string horaFin, int? idLeccionActual)
        {
            try
            {
                var leccionTemporal = new Leccion
                {
                    IdGrupo = idGrupo,
                    DiaSemana = diaSemana,
                    HoraInicio = TimeSpan.Parse(horaInicio),
                    HoraFin = TimeSpan.Parse(horaFin),
                    IdLeccion = idLeccionActual ?? 0
                };

                var conflictos = await _horarioService.ObtenerConflictosHorarioAsync(leccionTemporal);

                // Excluir el registro actual si existe
                if (idLeccionActual.HasValue)
                {
                    conflictos = conflictos.Where(c => c.IdLeccion != idLeccionActual.Value).ToList();
                }

                var resultado = conflictos.Select(c => new
                {
                    grupo = c.Grupo?.Nombre,
                    materia = c.Materia?.Nombre,
                    bloque = c.NumeroBloque,
                    dia = c.NombreDia,
                    horario = c.HorarioFormateado
                });

                return Json(new { success = true, conflictos = resultado, cantidad = conflictos.Count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener conflictos");
                return Json(new { success = false, message = "Error al verificar conflictos" });
            }
        }

        // GET: Horarios/HorarioSemanal
        public async Task<IActionResult> HorarioSemanal(int? grupoId)
        {
            try
            {
                if (!grupoId.HasValue)
                {
                    ViewBag.Grupos = await _context.GruposEstudiantes
                        .OrderBy(g => g.Nombre)
                        .Select(g => new SelectListItem { Value = g.GrupoId.ToString(), Text = g.Nombre })
                        .ToListAsync();

                    return View(new Dictionary<DayOfWeek, List<Leccion>>());
                }

                var horarios = await _horarioService.ObtenerHorariosPorGrupoAsync(grupoId.Value);
                var horarioSemanal = horarios
                    .Where(h => h.Activa)
                    .GroupBy(h => h.DiaSemana)
                    .ToDictionary(g => g.Key, g => g.OrderBy(h => h.HoraInicio).ToList());

                ViewBag.Grupo = await _context.GruposEstudiantes
                    .FirstOrDefaultAsync(g => g.GrupoId == grupoId.Value);

                ViewBag.Grupos = await _context.GruposEstudiantes
                    .OrderBy(g => g.Nombre)
                    .Select(g => new SelectListItem 
                    { 
                        Value = g.GrupoId.ToString(), 
                        Text = g.Nombre,
                        Selected = g.GrupoId == grupoId.Value
                    })
                    .ToListAsync();

                ViewBag.DiasSemanaDictionary = new Dictionary<DayOfWeek, string>
                {
                    { DayOfWeek.Monday, "Lunes" },
                    { DayOfWeek.Tuesday, "Martes" },
                    { DayOfWeek.Wednesday, "Miércoles" },
                    { DayOfWeek.Thursday, "Jueves" },
                    { DayOfWeek.Friday, "Viernes" },
                    { DayOfWeek.Saturday, "Sábado" },
                    { DayOfWeek.Sunday, "Domingo" }
                };

                return View(horarioSemanal);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener horario semanal");
                TempData["ErrorMessage"] = "Error al cargar el horario semanal";
                return View(new Dictionary<DayOfWeek, List<Leccion>>());
            }
        }

        private async Task CargarDatosVista()
        {
            ViewBag.Grupos = await _context.GruposEstudiantes
                .OrderBy(g => g.Nombre)
                .Select(g => new SelectListItem { Value = g.GrupoId.ToString(), Text = g.Nombre })
                .ToListAsync();

            ViewBag.Materias = await _context.Materias
                .OrderBy(m => m.Nombre)
                .Select(m => new SelectListItem { Value = m.MateriaId.ToString(), Text = m.Nombre })
                .ToListAsync();

            ViewBag.DiasSemanaDictionary = new Dictionary<DayOfWeek, string>
            {
                { DayOfWeek.Monday, "Lunes" },
                { DayOfWeek.Tuesday, "Martes" },
                { DayOfWeek.Wednesday, "Miércoles" },
                { DayOfWeek.Thursday, "Jueves" },
                { DayOfWeek.Friday, "Viernes" },
                { DayOfWeek.Saturday, "Sábado" },
                { DayOfWeek.Sunday, "Domingo" }
            };

            ViewBag.DiasSemanaNumericos = new Dictionary<int, string>
            {
                { 1, "Lunes" },
                { 2, "Martes" },
                { 3, "Miércoles" },
                { 4, "Jueves" },
                { 5, "Viernes" },
                { 6, "Sábado" },
                { 0, "Domingo" }
            };
        }
    }
}
