using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RubricasApp.Web.Data;
using RubricasApp.Web.Models;
using RubricasApp.Web.Models.Identity;
using RubricasApp.Web.Services;
using RubricasApp.Web.ViewModels.Conducta;

namespace RubricasApp.Web.Controllers
{
    [Authorize]
    public class NotaConductaController : Controller
    {
        private readonly RubricasDbContext _context;
        private readonly IConductaService _conductaService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<NotaConductaController> _logger;
        private readonly IPdfService _pdfService;

        public NotaConductaController(
            RubricasDbContext context,
            IConductaService conductaService,
            UserManager<ApplicationUser> userManager,
            ILogger<NotaConductaController> logger,
            IPdfService pdfService)
        {
            _context = context;
            _conductaService = conductaService;
            _userManager = userManager;
            _logger = logger;
            _pdfService = pdfService;
        }

        // GET: NotaConducta (Index por defecto redirige a Dashboard)
        public IActionResult Index()
        {
            return RedirectToAction(nameof(Dashboard));
        }

        // GET: NotaConducta/Dashboard
        public async Task<IActionResult> Dashboard(int? idPeriodo, int? idGrupo)
        {
            try
            {
                // Cargar períodos
                var periodos = await _context.PeriodosAcademicos
                    .OrderByDescending(p => p.Anio)
                    .ThenByDescending(p => p.NumeroPeriodo)
                    .ToListAsync();

                ViewBag.Periodos = periodos.Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.Nombre
                }).ToList();

                // Seleccionar período actual si no se especifica
                if (!idPeriodo.HasValue && periodos.Any())
                {
                    idPeriodo = periodos.First().Id;
                }

                if (!idPeriodo.HasValue)
                {
                    TempData["Warning"] = "No hay períodos académicos configurados";
                    ViewBag.Grupos = new List<SelectListItem>();
                    return View(new DashboardConductaViewModel());
                }

                ViewBag.PeriodoSeleccionado = idPeriodo.Value;

                // Cargar grupos del período
                var grupos = await _context.GruposEstudiantes
                    .Where(g => g.PeriodoAcademicoId == idPeriodo.Value && g.Estado == EstadoGrupo.Activo)
                    .OrderBy(g => g.Nombre)
                    .ToListAsync();

                ViewBag.Grupos = grupos.Select(g => new SelectListItem
                {
                    Value = g.GrupoId.ToString(),
                    Text = g.Nombre
                }).ToList();

                var dashboard = await _conductaService.ObtenerDashboardConductaAsync(idPeriodo.Value);
                
                // Aplicar filtro de grupo si se especifica
                if (idGrupo.HasValue && dashboard != null)
                {
                    dashboard.IdGrupo = idGrupo.Value;
                    // TODO: Filtrar estadísticas por grupo específico si es necesario
                }

                return View(dashboard);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar dashboard de conducta");
                TempData["Error"] = "Error al cargar el dashboard";
                ViewBag.Periodos = new List<SelectListItem>();
                ViewBag.Grupos = new List<SelectListItem>();
                return View(new DashboardConductaViewModel());
            }
        }

        // GET: NotaConducta/EstudianteNota/5
        public async Task<IActionResult> EstudianteNota(int? idEstudiante, int? idPeriodo)
        {
            if (!idEstudiante.HasValue)
            {
                TempData["Error"] = "Debe especificar un estudiante";
                return RedirectToAction(nameof(Dashboard));
            }

            try
            {
                // Obtener estudiante primero
                var estudiante = await _context.Estudiantes.FindAsync(idEstudiante.Value);
                if (estudiante == null)
                {
                    TempData["Error"] = $"No se encontró el estudiante con ID {idEstudiante.Value}";
                    return RedirectToAction(nameof(Dashboard));
                }

                // Cargar períodos
                var periodos = await _context.PeriodosAcademicos
                    .OrderByDescending(p => p.Anio)
                    .ThenByDescending(p => p.NumeroPeriodo)
                    .ToListAsync();

                ViewBag.Periodos = periodos.Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.Nombre
                }).ToList();

                // Seleccionar período actual si no se especifica
                if (!idPeriodo.HasValue && periodos.Any())
                {
                    idPeriodo = periodos.First().Id;
                }

                if (!idPeriodo.HasValue)
                {
                    TempData["Warning"] = "No hay períodos académicos configurados";
                    return RedirectToAction(nameof(Dashboard));
                }

                ViewBag.PeriodoSeleccionado = idPeriodo.Value;
                ViewBag.IdEstudiante = idEstudiante.Value;

                // Obtener boletas del estudiante primero para verificar inconsistencias
                var boletas = await _conductaService.ObtenerBoletasPorEstudianteAsync(idEstudiante.Value, idPeriodo.Value);

                // Calcular estadísticas de boletas
                var boletasActivas = boletas.Where(b => b.Estado == "Activa").ToList();
                var boletasAnuladas = boletas.Where(boletasAnuladas => boletasAnuladas.Estado == "Anulada").ToList();
                
                var totalRebajosActivos = boletasActivas.Sum(b => b.RebajoAplicado);

                // Obtener nota de conducta
                var notaConducta = await _conductaService.ObtenerNotaConductaAsync(idEstudiante.Value, idPeriodo.Value);

                if (notaConducta == null)
                {
                    TempData["Info"] = "El estudiante no tiene registros de conducta en este período";
                    notaConducta = new NotaConducta
                    {
                        IdEstudiante = idEstudiante.Value,
                        IdPeriodo = idPeriodo.Value,
                        NotaInicial = 100,
                        TotalRebajos = 0,
                        NotaFinal = 100,
                        Estado = "Aprobado"
                    };
                }
                else
                {
                    // VERIFICAR INCONSISTENCIA: Si el total de rebajos en BD no coincide con las boletas activas
                    // o si tiene estado Aplazado/Riesgo pero no hay rebajos activos, recalcular automáticamente
                    bool necesitaRecalculo = Math.Abs(notaConducta.TotalRebajos - totalRebajosActivos) > 0.01m;
                    bool estadoInconsistente = (notaConducta.Estado == "Aplazado" || notaConducta.Estado == "Riesgo") 
                                                && totalRebajosActivos == 0;

                    if (necesitaRecalculo || estadoInconsistente)
                    {
                        _logger.LogInformation(
                            "Detectada inconsistencia en nota de conducta del estudiante {IdEstudiante}. " +
                            "Rebajos BD: {RebajosBD}, Rebajos Activos: {RebajosActivos}, Estado: {Estado}. Recalculando automáticamente...",
                            idEstudiante.Value, notaConducta.TotalRebajos, totalRebajosActivos, notaConducta.Estado
                        );

                        // Recalcular automáticamente
                        notaConducta = await _conductaService.CalcularNotaConductaAsync(idEstudiante.Value, idPeriodo.Value);
                        
                        TempData["Info"] = "La nota de conducta se recalculó automáticamente para reflejar el estado actual de las boletas.";
                    }
                }
                var rebajosAnulados = boletasAnuladas.Sum(b => b.RebajoAplicado);

                // Obtener grupo del estudiante
                var estudianteGrupo = await _context.EstudianteGrupos
                    .Include(eg => eg.Grupo)
                    .FirstOrDefaultAsync(eg => eg.EstudianteId == idEstudiante.Value);

                var nombreGrupo = estudianteGrupo?.Grupo?.Nombre ?? "Sin grupo";

                // Obtener el nombre del profesor guía
                var nombreProfesorGuia = await _conductaService.ObtenerNombreProfesorGuiaAsync(idEstudiante.Value) ?? "Sin asignar";

                var modelo = new EstudianteNotaConductaViewModel
                {
                    IdEstudiante = idEstudiante.Value,
                    NombreEstudiante = $"{estudiante.Nombre} {estudiante.Apellidos}",
                    NumeroIdentificacion = estudiante.NumeroId,
                    NombreGrupo = nombreGrupo,
                    IdPeriodo = idPeriodo.Value,
                    NombrePeriodo = periodos.FirstOrDefault(p => p.Id == idPeriodo.Value)?.Nombre ?? "",
                    NotaActual = notaConducta.NotaFinal,
                    NotaFinal = notaConducta.NotaFinal,
                    NotaMinima = 70.0m,
                    TotalRebajos = totalRebajosActivos,
                    Estado = notaConducta.Estado,
                    TotalBoletas = boletasActivas.Count,
                    CantidadBoletas = boletasActivas.Count,
                    BoletasAnuladas = boletasAnuladas.Count,
                    RebajosAnulados = rebajosAnulados,
                    NotificacionesEnviadas = boletasActivas.Count(b => b.NotificacionEnviada),
                    FechaUltimaBoleta = boletasActivas.Any() ? boletasActivas.Max(b => b.FechaEmision) : (DateTime?)null,
                    NombreProfesorGuia = nombreProfesorGuia,
                    TieneProgramaAcciones = notaConducta.RequiereProgramaAcciones && notaConducta.IdProgramaAcciones.HasValue,
                    IdProgramaAcciones = notaConducta.IdProgramaAcciones,
                    TieneDecisionProfesional = notaConducta.DecisionProfesionalAplicada && notaConducta.IdDecisionProfesional.HasValue,
                    IdDecisionProfesional = notaConducta.IdDecisionProfesional,
                    Boletas = boletas.Select(b => new BoletaConductaListViewModel
                    {
                        IdBoleta = b.IdBoleta,
                        IdEstudiante = b.IdEstudiante,
                        NombreEstudiante = $"{estudiante.Nombre} {estudiante.Apellidos}",
                        NumeroId = estudiante.NumeroId,
                        TipoFalta = b.TipoFalta?.Nombre ?? "Desconocido",
                        RebajoAplicado = b.RebajoAplicado,
                        Rebajo = b.RebajoAplicado,
                        Descripcion = b.Descripcion,
                        DocenteEmisor = b.DocenteEmisor != null ? $"{b.DocenteEmisor.Nombre} {b.DocenteEmisor.Apellidos}".Trim() : "Desconocido",
                        EmitidaPor = b.DocenteEmisor != null ? $"{b.DocenteEmisor.Nombre} {b.DocenteEmisor.Apellidos}".Trim() : "Desconocido",
                        FechaEmision = b.FechaEmision,
                        FechaNotificacion = b.FechaNotificacion,
                        Estado = b.Estado,
                        NotificacionEnviada = b.NotificacionEnviada
                    }).ToList()
                };

                return View(modelo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar nota de conducta del estudiante {IdEstudiante}", idEstudiante);
                TempData["Error"] = "Error al cargar la nota de conducta";
                return RedirectToAction(nameof(Dashboard));
            }
        }

        // GET: NotaConducta/EstudiantesRiesgo
        public async Task<IActionResult> EstudiantesRiesgo(int? idPeriodo)
        {
            try
            {
                // Cargar períodos
                var periodos = await _context.PeriodosAcademicos
                    .OrderByDescending(p => p.Anio)
                    .ThenByDescending(p => p.NumeroPeriodo)
                    .ToListAsync();

                ViewBag.Periodos = periodos.Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.Nombre
                }).ToList();

                // Seleccionar período actual si no se especifica
                if (!idPeriodo.HasValue && periodos.Any())
                {
                    idPeriodo = periodos.First().Id;
                }

                if (!idPeriodo.HasValue)
                {
                    TempData["Warning"] = "No hay períodos académicos configurados";
                    ViewBag.Grupos = new List<SelectListItem>();
                    return View(new EstudiantesRiesgoViewModel());
                }

                ViewBag.PeriodoSeleccionado = idPeriodo.Value;
                ViewBag.Grupos = new List<SelectListItem>(); // Grupos vacío por ahora

                var estudiantesRiesgo = await _conductaService.ObtenerEstudiantesEnRiesgoAsync(idPeriodo.Value);

                var modelo = new EstudiantesRiesgoViewModel
                {
                    Estudiantes = estudiantesRiesgo
                };

                ViewBag.PeriodoNombre = periodos.FirstOrDefault(p => p.Id == idPeriodo.Value)?.Nombre ?? "";
                return View(modelo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar estudiantes en riesgo");
                TempData["Error"] = "Error al cargar los estudiantes en riesgo";
                
                // Asegurar que ViewBag tenga valores aunque sea vacíos
                ViewBag.Periodos = new List<SelectListItem>();
                ViewBag.Grupos = new List<SelectListItem>();
                
                return View(new EstudiantesRiesgoViewModel());
            }
        }

        // GET: NotaConducta/EstudiantesAplazados
        public async Task<IActionResult> EstudiantesAplazados(int? idPeriodo)
        {
            try
            {
                // Cargar períodos
                var periodos = await _context.PeriodosAcademicos
                    .OrderByDescending(p => p.Anio)
                    .ThenByDescending(p => p.NumeroPeriodo)
                    .ToListAsync();

                ViewBag.Periodos = periodos.Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.Nombre
                }).ToList();

                // Seleccionar período actual si no se especifica
                if (!idPeriodo.HasValue && periodos.Any())
                {
                    idPeriodo = periodos.First().Id;
                }

                if (!idPeriodo.HasValue)
                {
                    TempData["Warning"] = "No hay períodos académicos configurados";
                    ViewBag.Grupos = new List<SelectListItem>();
                    return View(new EstudiantesRiesgoViewModel());
                }

                ViewBag.PeriodoSeleccionado = idPeriodo.Value;
                ViewBag.Grupos = new List<SelectListItem>(); // Grupos vacío por ahora

                var estudiantesAplazados = await _conductaService.ObtenerEstudiantesAplazadosAsync(idPeriodo.Value);

                var modelo = new EstudiantesRiesgoViewModel
                {
                    Estudiantes = estudiantesAplazados
                };

                ViewBag.PeriodoNombre = periodos.FirstOrDefault(p => p.Id == idPeriodo.Value)?.Nombre ?? "";
                return View(modelo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar estudiantes aplazados");
                TempData["Error"] = "Error al cargar los estudiantes aplazados";
                
                // Asegurar que ViewBag tenga valores aunque sea vacíos
                ViewBag.Periodos = new List<SelectListItem>();
                ViewBag.Grupos = new List<SelectListItem>();
                
                return View(new EstudiantesRiesgoViewModel());
            }
        }

        // GET: NotaConducta/CrearPrograma
        public async Task<IActionResult> CrearPrograma(int? idNotaConducta, int? idEstudiante, int? idPeriodo)
        {
            // Si vienen idEstudiante e idPeriodo, buscar la nota de conducta correspondiente
            if (idEstudiante.HasValue && idPeriodo.HasValue && !idNotaConducta.HasValue)
            {
                var nota = await _context.NotasConducta
                    .FirstOrDefaultAsync(n => n.IdEstudiante == idEstudiante.Value && n.IdPeriodo == idPeriodo.Value);
                
                if (nota == null)
                {
                    TempData["Error"] = "No se encontró la nota de conducta para este estudiante en el período indicado";
                    return RedirectToAction(nameof(EstudianteNota), new { idEstudiante, idPeriodo });
                }
                
                idNotaConducta = nota.IdNotaConducta;
            }
            
            if (!idNotaConducta.HasValue)
            {
                return NotFound();
            }

            try
            {
                var notaConducta = await _context.NotasConducta
                    .Include(n => n.Estudiante)
                    .Include(n => n.Periodo)
                    .FirstOrDefaultAsync(n => n.IdNotaConducta == idNotaConducta.Value);

                if (notaConducta == null)
                {
                    return NotFound();
                }

                if (!notaConducta.RequiereProgramaAcciones)
                {
                    TempData["Warning"] = "Este estudiante no requiere programa de acciones institucional";
                    return RedirectToAction(nameof(EstudianteNota), new { idEstudiante = notaConducta.IdEstudiante, idPeriodo = notaConducta.IdPeriodo });
                }

                if (notaConducta.IdProgramaAcciones.HasValue)
                {
                    TempData["Warning"] = "Este estudiante ya tiene un programa de acciones asignado";
                    return RedirectToAction(nameof(EstudianteNota), new { idEstudiante = notaConducta.IdEstudiante, idPeriodo = notaConducta.IdPeriodo });
                }

                // Obtener grupo del estudiante
                var estudianteGrupo = await _context.EstudianteGrupos
                    .Include(eg => eg.Grupo)
                    .FirstOrDefaultAsync(eg => eg.EstudianteId == notaConducta.IdEstudiante);

                var modelo = new CrearProgramaAccionesViewModel
                {
                    IdNotaConducta = notaConducta.IdNotaConducta,
                    IdEstudiante = notaConducta.IdEstudiante,
                    NombreEstudiante = $"{notaConducta.Estudiante.Nombre} {notaConducta.Estudiante.Apellidos}",
                    NumeroIdentificacion = notaConducta.Estudiante.NumeroId,
                    NombreGrupo = estudianteGrupo?.Grupo?.Nombre ?? "Sin grupo",
                    IdPeriodo = notaConducta.IdPeriodo,
                    NombrePeriodo = notaConducta.Periodo.Nombre,
                    NotaActual = notaConducta.NotaFinal,
                    NotaConducta = notaConducta.NotaFinal,
                    FechaInicio = DateTime.Today,
                    FechaFinPrevista = DateTime.Today.AddMonths(2)
                };

                // Cargar supervisores (profesores/orientadores)
                var supervisores = await _context.Users
                    .Where(u => u.Activo)
                    .OrderBy(u => u.Apellidos)
                    .ToListAsync();

                ViewBag.Supervisores = supervisores.Select(s => new SelectListItem
                {
                    Value = s.Id,
                    Text = $"{s.Apellidos} {s.Nombre}"
                }).ToList();

                return View(modelo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar formulario de programa de acciones");
                TempData["Error"] = "Error al cargar el formulario";
                return RedirectToAction(nameof(Dashboard));
            }
        }

        // POST: NotaConducta/CrearPrograma
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearPrograma(CrearProgramaAccionesViewModel modelo)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    await CargarSupervisoresAsync();
                    return View(modelo);
                }

                var usuario = await _userManager.GetUserAsync(User);
                if (usuario == null)
                {
                    ModelState.AddModelError("", "No se pudo identificar al usuario");
                    await CargarSupervisoresAsync();
                    return View(modelo);
                }

                var programa = new ProgramaAccionesInstitucional
                {
                    IdNotaConducta = modelo.IdNotaConducta,
                    IdEstudiante = modelo.IdEstudiante,
                    IdPeriodo = modelo.IdPeriodo,
                    TituloPrograma = modelo.TituloPrograma,
                    Descripcion = modelo.Descripcion,
                    ObjetivosEspecificos = modelo.ObjetivosEspecificos ?? modelo.Objetivos ?? string.Empty,
                    ActividadesARealizar = modelo.ActividadesARealizar ?? modelo.ActividadesPropuestas ?? string.Empty,
                    FechaInicio = modelo.FechaInicio,
                    FechaFinPrevista = modelo.FechaFinPrevista,
                    ResponsableSupervisionId = modelo.ResponsableSupervisionId ?? modelo.IdSupervisor ?? string.Empty,
                    Estado = "Pendiente",
                    FechaCreacion = DateTime.Now
                };

                var idPrograma = await _conductaService.CrearProgramaAccionesAsync(programa);

                TempData["Success"] = "Programa de Acciones Institucional creado exitosamente";
                return RedirectToAction(nameof(EstudianteNota), new { idEstudiante = modelo.IdEstudiante, idPeriodo = modelo.IdPeriodo });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear programa de acciones");
                ModelState.AddModelError("", "Error al crear el programa");
                await CargarSupervisoresAsync();
                return View(modelo);
            }
        }

        // GET: NotaConducta/AplicarDecisionProfesional
        public async Task<IActionResult> AplicarDecisionProfesional(int? idNotaConducta, int? idEstudiante, int? idPeriodo)
        {
            // Si vienen idEstudiante e idPeriodo, buscar la nota de conducta correspondiente
            if (idEstudiante.HasValue && idPeriodo.HasValue && !idNotaConducta.HasValue)
            {
                var nota = await _context.NotasConducta
                    .FirstOrDefaultAsync(n => n.IdEstudiante == idEstudiante.Value && n.IdPeriodo == idPeriodo.Value);
                
                if (nota == null)
                {
                    TempData["Error"] = "No se encontró la nota de conducta para este estudiante en el período indicado";
                    return RedirectToAction(nameof(EstudianteNota), new { idEstudiante, idPeriodo });
                }
                
                idNotaConducta = nota.IdNotaConducta;
            }
            
            if (!idNotaConducta.HasValue)
            {
                return NotFound();
            }

            try
            {
                var notaConducta = await _context.NotasConducta
                    .Include(n => n.Estudiante)
                    .Include(n => n.Periodo)
                    .FirstOrDefaultAsync(n => n.IdNotaConducta == idNotaConducta.Value);

                if (notaConducta == null)
                {
                    return NotFound();
                }

                if (notaConducta.Estado != "Aplazado")
                {
                    TempData["Warning"] = "Solo se puede aplicar decisión profesional a estudiantes aplazados";
                    return RedirectToAction(nameof(EstudianteNota), new { idEstudiante = notaConducta.IdEstudiante, idPeriodo = notaConducta.IdPeriodo });
                }

                if (notaConducta.DecisionProfesionalAplicada)
                {
                    TempData["Warning"] = "Este estudiante ya tiene una decisión profesional aplicada";
                    return RedirectToAction(nameof(EstudianteNota), new { idEstudiante = notaConducta.IdEstudiante, idPeriodo = notaConducta.IdPeriodo });
                }

                var notaMinima = await _conductaService.ObtenerNotaMinimaAprobacionAsync();
                
                // Obtener grupo del estudiante
                var estudianteGrupo = await _context.EstudianteGrupos
                    .Include(eg => eg.Grupo)
                    .FirstOrDefaultAsync(eg => eg.EstudianteId == notaConducta.IdEstudiante);

                // Obtener el nombre del profesor guía
                var nombreProfesorGuia = await _conductaService.ObtenerNombreProfesorGuiaAsync(notaConducta.IdEstudiante) ?? "Sin asignar";

                // Contar boletas activas
                var boletas = await _context.BoletasConducta
                    .Where(b => b.IdEstudiante == notaConducta.IdEstudiante && b.IdPeriodo == notaConducta.IdPeriodo && b.Estado == "Activa")
                    .ToListAsync();

                var modelo = new AplicarDecisionProfesionalViewModel
                {
                    IdEstudiante = notaConducta.IdEstudiante,
                    NombreEstudiante = $"{notaConducta.Estudiante.Nombre} {notaConducta.Estudiante.Apellidos}",
                    NumeroIdentificacion = notaConducta.Estudiante.NumeroId,
                    NombreGrupo = estudianteGrupo?.Grupo?.Nombre ?? "Sin grupo",
                    IdPeriodo = notaConducta.IdPeriodo,
                    NombrePeriodo = notaConducta.Periodo.Nombre,
                    NotaActual = notaConducta.NotaFinal,
                    TotalBoletas = boletas.Count,
                    TotalRebajos = (int)notaConducta.TotalRebajos,
                    NombreProfesorGuia = nombreProfesorGuia,
                    FechaReunion = DateTime.Today,
                    NotaAjustada = notaMinima,
                    Decision = "Mantener Aplazado"
                };

                ViewBag.NotaMinima = notaMinima;
                return View(modelo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar formulario de decisión profesional");
                TempData["Error"] = "Error al cargar el formulario";
                return RedirectToAction(nameof(Dashboard));
            }
        }

        // POST: NotaConducta/AplicarDecisionProfesional
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AplicarDecisionProfesional(AplicarDecisionProfesionalViewModel modelo)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(modelo);
                }

                var usuario = await _userManager.GetUserAsync(User);
                if (usuario == null)
                {
                    ModelState.AddModelError("", "No se pudo identificar al usuario");
                    return View(modelo);
                }

                // Obtener el IdNotaConducta
                var notaConducta = await _context.NotasConducta
                    .FirstOrDefaultAsync(n => n.IdEstudiante == modelo.IdEstudiante && n.IdPeriodo == modelo.IdPeriodo);

                if (notaConducta == null)
                {
                    ModelState.AddModelError("", "No se encontró la nota de conducta");
                    return View(modelo);
                }

                var decision = new DecisionProfesionalConducta
                {
                    IdNotaConducta = notaConducta.IdNotaConducta,
                    IdEstudiante = modelo.IdEstudiante,
                    IdPeriodo = modelo.IdPeriodo,
                    JustificacionPedagogica = modelo.JustificacionPedagogica,
                    ConsideracionesAdicionales = modelo.ConsideracionesAdicionales ?? modelo.ObservacionesAdicionales,
                    DecisionTomada = modelo.Decision,
                    NotaAjustada = modelo.NotaAjustada,
                    NumeroActa = modelo.NumeroActa,
                    FechaActa = modelo.FechaReunion,
                    MiembrosComitePresentes = modelo.MiembrosComite,
                    ObservacionesComite = modelo.AcuerdosComite,
                    TomaDecisionPorId = usuario.Id,
                    FechaDecision = DateTime.Now,
                    RegistradoEnExpediente = false
                };

                await _conductaService.AplicarDecisionProfesionalAsync(decision);

                TempData["Success"] = "Decisión profesional aplicada exitosamente";
                return RedirectToAction(nameof(EstudianteNota), new { idEstudiante = modelo.IdEstudiante, idPeriodo = modelo.IdPeriodo });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al aplicar decisión profesional");
                ModelState.AddModelError("", "Error al aplicar la decisión");
                return View(modelo);
            }
        }

        #region Métodos Auxiliares

        private string ObtenerColorEstado(string estado)
        {
            return estado switch
            {
                "Aprobado" => "success",
                "Riesgo" => "warning",
                "Aplazado" => "danger",
                _ => "secondary"
            };
        }

        private async Task CargarSupervisoresAsync()
        {
            var supervisores = await _context.Users
                .Where(u => u.Activo)
                .OrderBy(u => u.Apellidos)
                .ToListAsync();

            ViewBag.Supervisores = supervisores.Select(s => new SelectListItem
            {
                Value = s.Id,
                Text = $"{s.Apellidos} {s.Nombre}"
            }).ToList();
        }

        #endregion

        #region Reportes PDF

        // Reporte Individual - Programa de Acciones
        [HttpGet]
        public async Task<IActionResult> ReporteProgramaAcciones(int id)
        {
            try
            {
                var programa = await _context.ProgramasAccionesInstitucional
                    .Include(p => p.NotaConducta)
                        .ThenInclude(n => n.Estudiante)
                    .Include(p => p.NotaConducta)
                        .ThenInclude(n => n.Periodo)
                    .Include(p => p.ResponsableSupervision)
                    .FirstOrDefaultAsync(p => p.IdPrograma == id);

                if (programa == null)
                {
                    TempData["Error"] = "Programa de acciones no encontrado";
                    return RedirectToAction(nameof(Index));
                }

                var boletas = await _context.BoletasConducta
                    .Where(b => b.IdEstudiante == programa.NotaConducta.IdEstudiante && b.IdPeriodo == programa.NotaConducta.IdPeriodo)
                    .Include(b => b.DocenteEmisor)
                    .Include(b => b.TipoFalta)
                    .OrderBy(b => b.FechaEmision)
                    .ToListAsync();

                var model = new ReporteProgramaAccionesViewModel
                {
                    IdPrograma = programa.IdPrograma,
                    NumeroPrograma = "PROG-" + programa.IdPrograma,
                    FechaElaboracion = programa.FechaCreacion,
                    NombreEstudiante = $"{programa.NotaConducta.Estudiante.Nombre} {programa.NotaConducta.Estudiante.Apellidos}",
                    CarnetEstudiante = programa.NotaConducta.Estudiante.NumeroId,
                    GradoSeccion = programa.NotaConducta.Estudiante.Grupos ?? "",
                    PeriodoAcademico = programa.NotaConducta.Periodo.NombreCompleto,
                    DescripcionFaltas = programa.Descripcion ?? "",
                    AccionesFormativas = programa.ActividadesARealizar ?? "",
                    CompromisosEstudiante = programa.ObjetivosEspecificos ?? "",
                    ApoyoPadres = programa.ObservacionesSupervision ?? "",
                    SeguimientoEvaluacion = programa.ConclusionesComite ?? "",
                    Observaciones = programa.ResultadoFinal,
                    AutorizadoPor = programa.ResponsableSupervision != null 
                        ? $"{programa.ResponsableSupervision.Nombre} {programa.ResponsableSupervision.Apellidos}"
                        : "No asignado",
                    FechaAutorizacion = programa.FechaVerificacion,
                    Estado = programa.Estado,
                    Boletas = boletas.Select(b => new BoletaResumenViewModel
                    {
                        FechaIncidente = b.FechaEmision,
                        TipoFalta = b.TipoFalta.Nombre,
                        DescripcionIncidente = b.Descripcion,
                        ReportadoPor = $"{b.DocenteEmisor.Nombre} {b.DocenteEmisor.Apellidos}",
                        RebajoAplicado = b.RebajoAplicado
                    }).ToList()
                };

                var pdfBytes = await _pdfService.GeneratePdfFromViewAsync("ReporteProgramaAcciones", model, "Portrait");
                var fileName = $"ProgramaAcciones_{programa.IdPrograma}_{DateTime.Now:yyyyMMdd}.pdf";
                return File(pdfBytes, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generando reporte de Programa de Acciones ID: {Id}", id);
                TempData["Error"] = "Error al generar el reporte";
                return RedirectToAction(nameof(Index));
            }
        }

        // Reporte Individual - Decisión Profesional
        [HttpGet]
        public async Task<IActionResult> ReporteDecisionProfesional(int id)
        {
            try
            {
                var decision = await _context.DecisionesProfesionalesConducta
                    .Include(d => d.NotaConducta)
                        .ThenInclude(n => n.Estudiante)
                    .Include(d => d.NotaConducta)
                        .ThenInclude(n => n.Periodo)
                    .Include(d => d.TomaDecisionPor)
                    .FirstOrDefaultAsync(d => d.IdDecision == id);

                if (decision == null)
                {
                    TempData["Error"] = "Decisión profesional no encontrada";
                    return RedirectToAction(nameof(Index));
                }

                var boletas = await _context.BoletasConducta
                    .Where(b => b.IdEstudiante == decision.NotaConducta.IdEstudiante && b.IdPeriodo == decision.NotaConducta.IdPeriodo)
                    .Include(b => b.DocenteEmisor)
                    .Include(b => b.TipoFalta)
                    .OrderBy(b => b.FechaEmision)
                    .ToListAsync();

                var model = new ReporteDecisionProfesionalViewModel
                {
                    IdDecision = decision.IdDecision,
                    NumeroActa = decision.NumeroActa ?? "S/N",
                    FechaReunion = decision.FechaActa ?? decision.FechaDecision,
                    NombreEstudiante = $"{decision.NotaConducta.Estudiante.Nombre} {decision.NotaConducta.Estudiante.Apellidos}",
                    CarnetEstudiante = decision.NotaConducta.Estudiante.NumeroId,
                    GradoSeccion = decision.NotaConducta.Estudiante.Grupos ?? "",
                    PeriodoAcademico = decision.NotaConducta.Periodo.NombreCompleto,
                    AntecedentesExpuestos = decision.JustificacionPedagogica,
                    VersionEstudiante = "",
                    VersionPadres = "",
                    AnalisisComite = decision.ConsideracionesAdicionales ?? "",
                    DecisionTomada = decision.DecisionTomada,
                    AccionesSeguimiento = decision.MiembrosComitePresentes,
                    ObservacionesComite = decision.ObservacionesComite,
                    TomaDecisionPor = decision.TomaDecisionPor != null
                        ? $"{decision.TomaDecisionPor.Nombre} {decision.TomaDecisionPor.Apellidos}"
                        : "No asignado",
                    FechaDecision = decision.FechaDecision,
                    Estado = decision.RegistradoEnExpediente ? "Registrado" : "Pendiente",
                    Boletas = boletas.Select(b => new BoletaResumenViewModel
                    {
                        FechaIncidente = b.FechaEmision,
                        TipoFalta = b.TipoFalta.Nombre,
                        DescripcionIncidente = b.Descripcion,
                        ReportadoPor = $"{b.DocenteEmisor.Nombre} {b.DocenteEmisor.Apellidos}",
                        RebajoAplicado = b.RebajoAplicado
                    }).ToList()
                };

                var pdfBytes = await _pdfService.GeneratePdfFromViewAsync("ReporteDecisionProfesional", model, "Portrait");
                var fileName = $"DecisionProfesional_{decision.NumeroActa}_{DateTime.Now:yyyyMMdd}.pdf";
                return File(pdfBytes, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generando reporte de Decisión Profesional ID: {Id}", id);
                TempData["Error"] = "Error al generar el reporte";
                return RedirectToAction(nameof(Index));
            }
        }

        // Reporte Masivo - Programas de Acciones
        [HttpGet]
        public async Task<IActionResult> ReporteMasivoProgramasAcciones(int? idPeriodo, string? estado)
        {
            try
            {
                var query = _context.ProgramasAccionesInstitucional
                    .Include(p => p.NotaConducta)
                        .ThenInclude(n => n.Estudiante)
                    .Include(p => p.NotaConducta)
                        .ThenInclude(n => n.Periodo)
                    .Include(p => p.ResponsableSupervision)
                    .AsQueryable();

                if (idPeriodo.HasValue)
                {
                    query = query.Where(p => p.IdPeriodo == idPeriodo.Value);
                }

                if (!string.IsNullOrEmpty(estado))
                {
                    query = query.Where(p => p.Estado == estado);
                }

                var programas = await query
                    .OrderBy(p => p.FechaCreacion)
                    .ToListAsync();

                var periodoNombre = "Todos los períodos";
                if (idPeriodo.HasValue)
                {
                    var periodo = await _context.PeriodosAcademicos.FindAsync(idPeriodo.Value);
                    periodoNombre = periodo?.NombreCompleto ?? periodoNombre;
                }

                var model = new ReporteMasivoProgramasViewModel
                {
                    FechaGeneracion = DateTime.Now,
                    PeriodoAcademico = periodoNombre,
                    EstadoFiltro = string.IsNullOrEmpty(estado) ? "Todos" : estado,
                    TotalRegistros = programas.Count,
                    Programas = programas.Select(p => new ProgramaResumenViewModel
                    {
                        NumeroPrograma = "PROG-" + p.IdPrograma,
                        FechaElaboracion = p.FechaCreacion,
                        NombreEstudiante = $"{p.NotaConducta.Estudiante.Nombre} {p.NotaConducta.Estudiante.Apellidos}",
                        Carnet = p.NotaConducta.Estudiante.NumeroId ?? "",
                        GradoSeccion = p.NotaConducta.Estudiante.Grupos ?? "",
                        Estado = p.Estado,
                        AutorizadoPor = p.ResponsableSupervision != null
                            ? $"{p.ResponsableSupervision.Nombre} {p.ResponsableSupervision.Apellidos}"
                            : "Pendiente",
                        FechaAutorizacion = p.FechaVerificacion
                    }).ToList()
                };

                var pdfBytes = await _pdfService.GeneratePdfFromViewAsync("ReporteMasivoProgramas", model, "Landscape");
                var fileName = $"ProgramasAcciones_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                return File(pdfBytes, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generando reporte masivo de Programas de Acciones");
                TempData["Error"] = "Error al generar el reporte masivo";
                return RedirectToAction(nameof(Index));
            }
        }

        // Reporte Masivo - Decisiones Profesionales
        [HttpGet]
        public async Task<IActionResult> ReporteMasivoDecisionesProfesionales(int? idPeriodo, string? estado)
        {
            try
            {
                var query = _context.DecisionesProfesionalesConducta
                    .Include(d => d.NotaConducta)
                        .ThenInclude(n => n.Estudiante)
                    .Include(d => d.NotaConducta)
                        .ThenInclude(n => n.Periodo)
                    .Include(d => d.TomaDecisionPor)
                    .AsQueryable();

                if (idPeriodo.HasValue)
                {
                    query = query.Where(d => d.IdPeriodo == idPeriodo.Value);
                }

                if (!string.IsNullOrEmpty(estado))
                {
                    if (estado == "Registrado")
                        query = query.Where(d => d.RegistradoEnExpediente);
                    else if (estado == "Pendiente")
                        query = query.Where(d => !d.RegistradoEnExpediente);
                }

                var decisiones = await query
                    .OrderBy(d => d.FechaDecision)
                    .ToListAsync();

                var periodoNombre = "Todos los períodos";
                if (idPeriodo.HasValue)
                {
                    var periodo = await _context.PeriodosAcademicos.FindAsync(idPeriodo.Value);
                    periodoNombre = periodo?.NombreCompleto ?? periodoNombre;
                }

                var model = new ReporteMasivoDecisionesViewModel
                {
                    FechaGeneracion = DateTime.Now,
                    PeriodoAcademico = periodoNombre,
                    EstadoFiltro = string.IsNullOrEmpty(estado) ? "Todos" : estado,
                    TotalRegistros = decisiones.Count,
                    Decisiones = decisiones.Select(d => new DecisionResumenViewModel
                    {
                        NumeroActa = d.NumeroActa ?? "S/N",
                        FechaReunion = d.FechaActa ?? d.FechaDecision,
                        NombreEstudiante = $"{d.NotaConducta.Estudiante.Nombre} {d.NotaConducta.Estudiante.Apellidos}",
                        Carnet = d.NotaConducta.Estudiante.NumeroId ?? "",
                        GradoSeccion = d.NotaConducta.Estudiante.Grupos ?? "",
                        DecisionTomada = d.DecisionTomada,
                        Estado = d.RegistradoEnExpediente ? "Registrado" : "Pendiente",
                        TomaDecisionPor = d.TomaDecisionPor != null
                            ? $"{d.TomaDecisionPor.Nombre} {d.TomaDecisionPor.Apellidos}"
                            : "No asignado",
                        FechaDecision = d.FechaDecision
                    }).ToList()
                };

                var pdfBytes = await _pdfService.GeneratePdfFromViewAsync("ReporteMasivoDecisiones", model, "Landscape");
                var fileName = $"DecisionesProfesionales_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                return File(pdfBytes, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generando reporte masivo de Decisiones Profesionales");
                TempData["Error"] = "Error al generar el reporte masivo";
                return RedirectToAction(nameof(Index));
            }
        }

        #endregion

        #region Recálculo de Nota

        // POST: NotaConducta/RecalcularNota
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RecalcularNota(int idEstudiante, int idPeriodo)
        {
            try
            {
                // Verificar si el estudiante existe
                var estudiante = await _context.Estudiantes.FindAsync(idEstudiante);
                if (estudiante == null)
                {
                    TempData["Error"] = "Estudiante no encontrado";
                    return RedirectToAction(nameof(Dashboard));
                }

                // Verificar si tiene programa de acciones activo
                var tienePrograma = await _context.ProgramasAccionesInstitucional
                    .AnyAsync(p => p.IdEstudiante == idEstudiante 
                        && p.IdPeriodo == idPeriodo 
                        && p.Estado != "Completado" 
                        && p.Estado != "Cancelado");

                // Verificar si tiene decisión profesional
                var tieneDecision = await _context.DecisionesProfesionalesConducta
                    .AnyAsync(d => d.IdEstudiante == idEstudiante && d.IdPeriodo == idPeriodo);

                // Obtener información del estado actual
                var notaAnterior = await _context.NotasConducta
                    .FirstOrDefaultAsync(n => n.IdEstudiante == idEstudiante && n.IdPeriodo == idPeriodo);

                string estadoAnterior = notaAnterior?.Estado ?? "N/A";

                // Recalcular la nota
                var notaCalculada = await _conductaService.CalcularNotaConductaAsync(idEstudiante, idPeriodo);

                // Construir mensaje informativo mejorado con HTML estructurado
                var estadoBadgeAnterior = estadoAnterior == "Aprobado" ? "success" : 
                                         estadoAnterior == "Riesgo" ? "warning" : "danger";
                var estadoBadgeActual = notaCalculada.Estado == "Aprobado" ? "success" : 
                                       notaCalculada.Estado == "Riesgo" ? "warning" : "danger";

                var mensaje = @"
                    <div class='alert alert-success border-0 shadow-sm mb-0'>
                        <div class='d-flex align-items-start'>
                            <i class='fas fa-check-circle fa-2x text-success me-3 mt-1'></i>
                            <div class='flex-grow-1'>
                                <h5 class='alert-heading mb-3'>
                                    <i class='fas fa-calculator'></i> Nota de Conducta Recalculada
                                </h5>
                                <div class='row g-3 mb-3'>
                                    <div class='col-md-6'>
                                        <div class='card bg-light border-0'>
                                            <div class='card-body p-3'>
                                                <small class='text-muted d-block mb-1'>Estado Anterior</small>
                                                <h6 class='mb-0'>
                                                    <span class='badge bg-" + estadoBadgeAnterior + " fs-6'>" + estadoAnterior + @"</span>
                                                </h6>
                                            </div>
                                        </div>
                                    </div>
                                    <div class='col-md-6'>
                                        <div class='card bg-light border-0'>
                                            <div class='card-body p-3'>
                                                <small class='text-muted d-block mb-1'>Estado Actual</small>
                                                <h6 class='mb-0'>
                                                    <span class='badge bg-" + estadoBadgeActual + " fs-6'>" + notaCalculada.Estado + @"</span>
                                                </h6>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class='row g-3'>
                                    <div class='col-md-6'>
                                        <div class='d-flex align-items-center'>
                                            <i class='fas fa-chart-line text-primary me-2'></i>
                                            <div>
                                                <small class='text-muted d-block'>Nota Final</small>
                                                <strong class='text-primary'>" + notaCalculada.NotaFinal.ToString("F2") + @" / 100</strong>
                                            </div>
                                        </div>
                                    </div>
                                    <div class='col-md-6'>
                                        <div class='d-flex align-items-center'>
                                            <i class='fas fa-minus-circle text-danger me-2'></i>
                                            <div>
                                                <small class='text-muted d-block'>Total Rebajos</small>
                                                <strong class='text-danger'>" + notaCalculada.TotalRebajos + @" puntos</strong>
                                            </div>
                                        </div>
                                    </div>
                                </div>";

                // Agregar advertencias si hay procesos activos
                if (tienePrograma)
                {
                    mensaje += @"
                                <hr class='my-3'>
                                <div class='alert alert-warning border-0 mb-0 py-2'>
                                    <i class='fas fa-exclamation-triangle me-2'></i>
                                    <strong>Advertencia:</strong> El estudiante tiene un Programa de Acciones Institucional activo.
                                </div>";
                }

                if (tieneDecision)
                {
                    mensaje += @"
                                <hr class='my-3'>
                                <div class='alert alert-info border-0 mb-0 py-2'>
                                    <i class='fas fa-info-circle me-2'></i>
                                    <strong>Nota:</strong> El estudiante tiene una Decisión Profesional aplicada.
                                </div>";
                }

                // Verificar si mejoró de estado
                if ((estadoAnterior == "Riesgo" || estadoAnterior == "Aplazado") && notaCalculada.Estado == "Aprobado")
                {
                    mensaje += @"
                                <hr class='my-3'>
                                <div class='alert alert-success border-0 mb-0 py-2 bg-success bg-opacity-10'>
                                    <i class='fas fa-trophy me-2 text-success'></i>
                                    <strong>¡Excelente!</strong> El estudiante mejoró su estado y ya no requiere programa de acciones.
                                </div>";
                }

                mensaje += @"
                            </div>
                        </div>
                    </div>";

                TempData["Success"] = mensaje;

                _logger.LogInformation(
                    "Nota de conducta recalculada manualmente para estudiante {IdEstudiante} en periodo {IdPeriodo}. Estado: {EstadoAnterior} -> {EstadoNuevo}",
                    idEstudiante, idPeriodo, estadoAnterior, notaCalculada.Estado
                );

                return RedirectToAction(nameof(EstudianteNota), new { idEstudiante, idPeriodo });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al recalcular nota de conducta para estudiante {IdEstudiante}", idEstudiante);
                TempData["Error"] = "Error al recalcular la nota de conducta: " + ex.Message;
                return RedirectToAction(nameof(EstudianteNota), new { idEstudiante, idPeriodo });
            }
        }

        #endregion
    }
}
