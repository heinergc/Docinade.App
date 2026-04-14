using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DocinadeApp.Data;
using DocinadeApp.Models;
using DocinadeApp.Models.Identity;
using DocinadeApp.Services;
using DocinadeApp.ViewModels.Conducta;

namespace DocinadeApp.Controllers
{
    [Authorize]
    public class BoletasConductaController : Controller
    {
        private readonly RubricasDbContext _context;
        private readonly IConductaService _conductaService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<BoletasConductaController> _logger;

        public BoletasConductaController(
            RubricasDbContext context,
            IConductaService conductaService,
            UserManager<ApplicationUser> userManager,
            IWebHostEnvironment environment,
            ILogger<BoletasConductaController> logger)
        {
            _context = context;
            _conductaService = conductaService;
            _userManager = userManager;
            _environment = environment;
            _logger = logger;
        }

        // GET: BoletasConducta
        public async Task<IActionResult> Index(int? idEstudiante, int? idGrupo, int? idPeriodo, int? idTipoFalta)
        {
            try
            {
                var periodosQuery = _context.PeriodosAcademicos
                    .OrderByDescending(p => p.Anio)
                    .ThenByDescending(p => p.NumeroPeriodo);

                ViewBag.Periodos = await periodosQuery
                    .Select(p => new SelectListItem
                    {
                        Value = p.Id.ToString(),
                        Text = p.Nombre
                    })
                    .ToListAsync();

                ViewBag.TiposFalta = await _context.TiposFalta
                    .Where(t => t.Activo)
                    .OrderBy(t => t.Orden)
                    .Select(t => new SelectListItem
                    {
                        Value = t.IdTipoFalta.ToString(),
                        Text = t.Nombre
                    })
                    .ToListAsync();

                var boletasQuery = _context.BoletasConducta
                    .Include(b => b.Estudiante)
                    .Include(b => b.TipoFalta)
                    .Include(b => b.Periodo)
                    .Include(b => b.DocenteEmisor)
                    .Where(b => b.Estado != "Anulada")
                    .AsQueryable();

                // Filtros
                if (idEstudiante.HasValue)
                {
                    boletasQuery = boletasQuery.Where(b => b.IdEstudiante == idEstudiante.Value);
                }

                if (idPeriodo.HasValue)
                {
                    boletasQuery = boletasQuery.Where(b => b.IdPeriodo == idPeriodo.Value);
                }
                else
                {
                    // Por defecto, mostrar período actual
                    var periodoActual = await periodosQuery.FirstOrDefaultAsync();
                    if (periodoActual != null)
                    {
                        boletasQuery = boletasQuery.Where(b => b.IdPeriodo == periodoActual.Id);
                        ViewBag.PeriodoSeleccionado = periodoActual.Id;
                    }
                }

                if (idTipoFalta.HasValue)
                {
                    boletasQuery = boletasQuery.Where(b => b.IdTipoFalta == idTipoFalta.Value);
                }

                if (idGrupo.HasValue)
                {
                    var estudiantesGrupo = await _context.EstudianteGrupos
                        .Where(eg => eg.GrupoId == idGrupo.Value && eg.Estado == EstadoAsignacion.Activo)
                        .Select(eg => eg.EstudianteId)
                        .ToListAsync();

                    boletasQuery = boletasQuery.Where(b => estudiantesGrupo.Contains(b.IdEstudiante));
                }

                var boletas = await boletasQuery
                    .OrderByDescending(b => b.FechaEmision)
                    .Select(b => new BoletaConductaListViewModel
                    {
                        IdBoleta = b.IdBoleta,
                        IdEstudiante = b.IdEstudiante,
                        NombreEstudiante = $"{b.Estudiante.Nombre} {b.Estudiante.Apellidos}",
                        NumeroId = b.Estudiante.NumeroId,
                        TipoFalta = b.TipoFalta.Nombre,
                        RebajoAplicado = b.RebajoAplicado,
                        Descripcion = b.Descripcion,
                        DocenteEmisor = $"{b.DocenteEmisor.Nombre} {b.DocenteEmisor.Apellidos}",
                        FechaEmision = b.FechaEmision,
                        Estado = b.Estado,
                        NotificacionEnviada = b.NotificacionEnviada
                    })
                    .ToListAsync();

                ViewBag.IdEstudiante = idEstudiante;
                ViewBag.IdGrupo = idGrupo;
                ViewBag.IdPeriodo = idPeriodo;
                ViewBag.IdTipoFalta = idTipoFalta;

                return View(boletas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar lista de boletas de conducta");
                TempData["Error"] = "Error al cargar las boletas de conducta";
                return View(new List<BoletaConductaListViewModel>());
            }
        }

        // GET: BoletasConducta/Create
        public async Task<IActionResult> Create(int? idEstudiante)
        {
            try
            {
                var modelo = new RegistrarBoletaConductaViewModel();

                // Cargar períodos
                var periodos = await _context.PeriodosAcademicos
                    .OrderByDescending(p => p.Anio)
                    .ThenByDescending(p => p.NumeroPeriodo)
                    .ToListAsync();

                ViewBag.Periodos = periodos.Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.Nombre,
                    Selected = p.Id == periodos.FirstOrDefault()?.Id
                }).ToList();

                // Cargar estudiantes
                var estudiantes = await _context.Estudiantes
                    .OrderBy(e => e.Apellidos)
                    .ThenBy(e => e.Nombre)
                    .ToListAsync();

                ViewBag.Estudiantes = estudiantes.Select(e => new SelectListItem
                {
                    Value = e.IdEstudiante.ToString(),
                    Text = $"{e.Apellidos} {e.Nombre} - {e.NumeroId}",
                    Selected = idEstudiante.HasValue && e.IdEstudiante == idEstudiante.Value
                }).ToList();

                if (idEstudiante.HasValue)
                {
                    modelo.IdEstudiante = idEstudiante.Value;
                }

                // Cargar tipos de falta con detalles
                var tiposFalta = await _context.TiposFalta
                    .Where(t => t.Activo)
                    .OrderBy(t => t.Orden)
                    .ToListAsync();

                ViewBag.TiposFalta = tiposFalta;

                return View(modelo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar formulario de nueva boleta");
                TempData["Error"] = "Error al cargar el formulario";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: BoletasConducta/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RegistrarBoletaConductaViewModel modelo, IFormFile? archivoEvidencia)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    await CargarDatosFormularioAsync(modelo.IdEstudiante);
                    return View(modelo);
                }

                var usuario = await _userManager.GetUserAsync(User);
                if (usuario == null)
                {
                    ModelState.AddModelError("", "No se pudo identificar al usuario");
                    await CargarDatosFormularioAsync(modelo.IdEstudiante);
                    return View(modelo);
                }

                // Validar que el rebajo esté dentro del rango del tipo de falta
                var tipoFalta = await _context.TiposFalta.FindAsync(modelo.IdTipoFalta);
                if (tipoFalta == null)
                {
                    ModelState.AddModelError("", "Tipo de falta no encontrado");
                    await CargarDatosFormularioAsync(modelo.IdEstudiante);
                    return View(modelo);
                }

                if (modelo.RebajoAplicado < tipoFalta.RebajoMinimo || modelo.RebajoAplicado > tipoFalta.RebajoMaximo)
                {
                    ModelState.AddModelError(nameof(modelo.RebajoAplicado), 
                        $"El rebajo debe estar entre {tipoFalta.RebajoMinimo} y {tipoFalta.RebajoMaximo} puntos para faltas {tipoFalta.Nombre}");
                    await CargarDatosFormularioAsync(modelo.IdEstudiante);
                    return View(modelo);
                }

                // Procesar archivo de evidencia si existe
                string? rutaArchivo = null;
                if (archivoEvidencia != null && archivoEvidencia.Length > 0)
                {
                    var extensionesPermitidas = new[] { ".jpg", ".jpeg", ".png", ".pdf", ".doc", ".docx" };
                    var extension = Path.GetExtension(archivoEvidencia.FileName).ToLowerInvariant();

                    if (!extensionesPermitidas.Contains(extension))
                    {
                        ModelState.AddModelError("", "Tipo de archivo no permitido");
                        await CargarDatosFormularioAsync(modelo.IdEstudiante);
                        return View(modelo);
                    }

                    if (archivoEvidencia.Length > 5 * 1024 * 1024) // 5 MB
                    {
                        ModelState.AddModelError("", "El archivo no debe superar 5 MB");
                        await CargarDatosFormularioAsync(modelo.IdEstudiante);
                        return View(modelo);
                    }

                    var carpetaEvidencias = Path.Combine(_environment.WebRootPath, "evidencias", "conducta");
                    if (!Directory.Exists(carpetaEvidencias))
                    {
                        Directory.CreateDirectory(carpetaEvidencias);
                    }

                    var nombreArchivo = $"{Guid.NewGuid()}{extension}";
                    rutaArchivo = Path.Combine(carpetaEvidencias, nombreArchivo);

                    using (var stream = new FileStream(rutaArchivo, FileMode.Create))
                    {
                        await archivoEvidencia.CopyToAsync(stream);
                    }

                    // Guardar ruta relativa
                    rutaArchivo = $"/evidencias/conducta/{nombreArchivo}";
                }

                // Crear boleta
                var boleta = new BoletaConducta
                {
                    IdEstudiante = modelo.IdEstudiante,
                    IdPeriodo = modelo.IdPeriodo,
                    IdTipoFalta = modelo.IdTipoFalta,
                    RebajoAplicado = modelo.RebajoAplicado,
                    Descripcion = modelo.Descripcion,
                    RutaEvidencia = rutaArchivo,
                    FechaEmision = DateTime.Now,
                    DocenteEmisorId = usuario.Id,
                    Estado = "Activa",
                    NotificacionEnviada = false
                };

                // Usar el servicio para registrar la boleta (incluye notificación automática)
                var idBoleta = await _conductaService.RegistrarBoletaAsync(boleta);

                TempData["Success"] = "Boleta de conducta registrada exitosamente. Se notificó al profesor guía.";
                return RedirectToAction(nameof(Details), new { id = idBoleta });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear boleta de conducta");
                ModelState.AddModelError("", "Error al crear la boleta");
                await CargarDatosFormularioAsync(modelo.IdEstudiante);
                return View(modelo);
            }
        }

        // GET: BoletasConducta/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var boleta = await _conductaService.ObtenerBoletaPorIdAsync(id.Value);

                if (boleta == null)
                {
                    return NotFound();
                }

                // Obtener el nombre del profesor guía si no está cargado
                string? nombreProfesorGuia = "No asignado";
                string? emailProfesorGuia = null;
                if (boleta.ProfesorGuiaId.HasValue)
                {
                    if (boleta.ProfesorGuia != null)
                    {
                        nombreProfesorGuia = $"{boleta.ProfesorGuia.Nombres} {boleta.ProfesorGuia.PrimerApellido} {boleta.ProfesorGuia.SegundoApellido}".Trim();
                        emailProfesorGuia = boleta.ProfesorGuia.EmailInstitucional ?? boleta.ProfesorGuia.EmailPersonal;
                    }
                    else
                    {
                        // Si no se cargó por Include, obtenerlo manualmente
                        var profesorGuia = await _context.Profesores.FindAsync(boleta.ProfesorGuiaId.Value);
                        if (profesorGuia != null)
                        {
                            nombreProfesorGuia = $"{profesorGuia.Nombres} {profesorGuia.PrimerApellido} {profesorGuia.SegundoApellido}".Trim();
                            emailProfesorGuia = profesorGuia.EmailInstitucional ?? profesorGuia.EmailPersonal;
                        }
                    }
                }

                var modelo = new BoletaConductaDetalleViewModel
                {
                    IdBoleta = boleta.IdBoleta,
                    IdEstudiante = boleta.IdEstudiante,
                    NombreEstudiante = $"{boleta.Estudiante.Nombre} {boleta.Estudiante.Apellidos}",
                    NumeroId = boleta.Estudiante.NumeroId,
                    NumeroIdEstudiante = boleta.Estudiante.NumeroId,
                    Periodo = boleta.Periodo.Nombre,
                    TipoFalta = boleta.TipoFalta.Nombre,
                    DefinicionFalta = boleta.TipoFalta.Definicion,
                    EjemplosFalta = boleta.TipoFalta.Ejemplos,
                    AccionCorrectiva = boleta.TipoFalta.AccionCorrectiva,
                    RebajoMinimo = boleta.TipoFalta.RebajoMinimo,
                    RebajoMaximo = boleta.TipoFalta.RebajoMaximo,
                    RebajoAplicado = boleta.RebajoAplicado,
                    Descripcion = boleta.Descripcion,
                    RutaEvidencia = boleta.RutaEvidencia,
                    FechaEmision = boleta.FechaEmision,
                    DocenteEmisor = $"{boleta.DocenteEmisor.Nombre} {boleta.DocenteEmisor.Apellidos}",
                    ProfesorGuia = nombreProfesorGuia,
                    EmailProfesorGuia = emailProfesorGuia,
                    NotificacionEnviada = boleta.NotificacionEnviada,
                    FechaNotificacion = boleta.FechaNotificacion,
                    Estado = boleta.Estado,
                    MotivoAnulacion = boleta.MotivoAnulacion,
                    FechaAnulacion = boleta.FechaAnulacion
                };

                return View(modelo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar detalles de boleta {IdBoleta}", id);
                TempData["Error"] = "Error al cargar los detalles de la boleta";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: BoletasConducta/Anular/5
        public async Task<IActionResult> Anular(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var boleta = await _conductaService.ObtenerBoletaPorIdAsync(id.Value);

            if (boleta == null)
            {
                return NotFound();
            }

            if (boleta.Estado == "Anulada")
            {
                TempData["Warning"] = "Esta boleta ya está anulada";
                return RedirectToAction(nameof(Details), new { id });
            }

            ViewBag.IdBoleta = id;
            ViewBag.NombreEstudiante = $"{boleta.Estudiante.Nombre} {boleta.Estudiante.Apellidos}";
            ViewBag.TipoFalta = boleta.TipoFalta.Nombre;
            ViewBag.FechaEmision = boleta.FechaEmision;

            return View();
        }

        // POST: BoletasConducta/Anular/5
        [HttpPost, ActionName("Anular")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AnularConfirmed(int id, string motivoAnulacion)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(motivoAnulacion))
                {
                    ModelState.AddModelError("", "Debe especificar un motivo para anular la boleta");
                    return View();
                }

                var usuario = await _userManager.GetUserAsync(User);
                if (usuario == null)
                {
                    TempData["Error"] = "No se pudo identificar al usuario";
                    return RedirectToAction(nameof(Details), new { id });
                }

                await _conductaService.AnularBoletaAsync(id, usuario.Id, motivoAnulacion);

                TempData["Success"] = "Boleta anulada exitosamente. La nota de conducta ha sido recalculada.";
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al anular boleta {IdBoleta}", id);
                TempData["Error"] = "Error al anular la boleta";
                return RedirectToAction(nameof(Details), new { id });
            }
        }

        // POST: BoletasConducta/ReenviarNotificacion/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReenviarNotificacion(int id)
        {
            try
            {
                var boleta = await _context.BoletasConducta
                    .Include(b => b.Estudiante)
                    .Include(b => b.TipoFalta)
                    .Include(b => b.DocenteEmisor)
                    .FirstOrDefaultAsync(b => b.IdBoleta == id);

                if (boleta == null)
                {
                    TempData["Error"] = "Boleta no encontrada";
                    return RedirectToAction(nameof(Index));
                }

                // Intentar notificar al profesor guía
                await _conductaService.NotificarProfesorGuiaAsync(boleta.IdBoleta);
                
                TempData["Success"] = "Notificación reenviada exitosamente al profesor guía";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al reenviar notificación de boleta {IdBoleta}", id);
                TempData["Error"] = "Error al enviar la notificación. Verifique que el estudiante tenga un profesor guía asignado.";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: BoletasConducta/GetTipoFalta/1
        [HttpGet]
        public async Task<IActionResult> GetTipoFalta(int id)
        {
            try
            {
                var tipoFalta = await _context.TiposFalta.FindAsync(id);

                if (tipoFalta == null)
                {
                    return NotFound();
                }

                return Json(new
                {
                    nombre = tipoFalta.Nombre,
                    definicion = tipoFalta.Definicion,
                    ejemplos = tipoFalta.Ejemplos,
                    accionCorrectiva = tipoFalta.AccionCorrectiva,
                    rebajoMinimo = tipoFalta.RebajoMinimo,
                    rebajoMaximo = tipoFalta.RebajoMaximo
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener tipo de falta {IdTipoFalta}", id);
                return StatusCode(500, "Error al obtener información del tipo de falta");
            }
        }

        #region Métodos Auxiliares

        private async Task CargarDatosFormularioAsync(int? idEstudianteSeleccionado = null)
        {
            var periodos = await _context.PeriodosAcademicos
                .OrderByDescending(p => p.Anio)
                .ThenByDescending(p => p.NumeroPeriodo)
                .ToListAsync();

            ViewBag.Periodos = periodos.Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = p.Nombre,
                Selected = p.Id == periodos.FirstOrDefault()?.Id
            }).ToList();

            var estudiantes = await _context.Estudiantes
                .OrderBy(e => e.Apellidos)
                .ThenBy(e => e.Nombre)
                .ToListAsync();

            ViewBag.Estudiantes = estudiantes.Select(e => new SelectListItem
            {
                Value = e.IdEstudiante.ToString(),
                Text = $"{e.Apellidos} {e.Nombre} - {e.NumeroId}",
                Selected = idEstudianteSeleccionado.HasValue && e.IdEstudiante == idEstudianteSeleccionado.Value
            }).ToList();

            var tiposFalta = await _context.TiposFalta
                .Where(t => t.Activo)
                .OrderBy(t => t.Orden)
                .ToListAsync();

            ViewBag.TiposFalta = tiposFalta;
        }

        #endregion
    }
}
