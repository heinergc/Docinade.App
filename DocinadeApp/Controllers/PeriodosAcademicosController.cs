using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DocinadeApp.Data;
using DocinadeApp.Models;
using DocinadeApp.Models.Academic;
using DocinadeApp.Models.Permissions;
using DocinadeApp.ViewModels.Academic;
using DocinadeApp.Services.Academic;
using DocinadeApp.Authorization;
using DocinadeApp.Services;
using DocinadeApp.Services.Audit;
using Microsoft.AspNetCore.Authorization;



namespace DocinadeApp.Controllers
{
    [Authorize]
    public class PeriodosAcademicosController : Controller
    {
        private readonly IPeriodosAcademicosService _periodosService;
        private readonly ILogger<PeriodosAcademicosController> _logger;
        private readonly RubricasDbContext _context;
        private readonly IExcepcionService _excepcionService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IErrorLogService _errorLogService;

        public PeriodosAcademicosController(
            IPeriodosAcademicosService periodosService,
            ILogger<PeriodosAcademicosController> logger,
            RubricasDbContext context,
            IExcepcionService excepcionService,
            IHttpContextAccessor httpContextAccessor,
            IErrorLogService errorLogService)
        {
            _periodosService = periodosService;
            _logger = logger;
            _context = context;
            _excepcionService = excepcionService;
            _httpContextAccessor = httpContextAccessor;
            _errorLogService = errorLogService;
        }

        // GET: PeriodosAcademicos
        [RequirePermission(ApplicationPermissions.Periodos.VER)]
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10, string? searchTerm = null, int? tipoFiltro = null)
        {
            try
            {
                ViewBag.CurrentFilter = searchTerm;
                ViewBag.TipoFiltro = tipoFiltro;
                ViewBag.PageSize = pageSize;
                
                // Obtener tipos únicos para el filtro
                ViewBag.TiposDisponibles = await _periodosService.GetTiposUnicosAsync();
                
                var result = await _periodosService.GetPagedAsync(page, pageSize, searchTerm, tipoFiltro);
                return View(result);
            }
            catch (Exception ex)
            {
                await _errorLogService.LogErrorAsync(
                    "PeriodosAcademicosController",
                    "Index",
                    ex,
                    User?.Identity?.Name,
                    User?.Identity?.Name,
                    Request.Path,
                    Request.Method,
                    null,
                    HttpContext.Connection.RemoteIpAddress?.ToString(),
                    Request.Headers["User-Agent"].ToString()
                );

                _logger.LogError(ex, "Error al obtener los períodos académicos");
                TempData["ErrorMessage"] = "Error al cargar los períodos académicos: " + ex.Message;
                return View(new PagedResult<PeriodoListVm> { Items = new List<PeriodoListVm>() });
            }
        }

        // GET: PeriodosAcademicos/Details/5
        [RequirePermission(ApplicationPermissions.Periodos.VER)]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                // 🔧 CORRECCIÓN TEMPORAL: Usar consulta directa
                var periodoEntity = await _context.PeriodosAcademicos
                    .Where(p => p.Id == id)
                    .FirstOrDefaultAsync();
                
                if (periodoEntity == null)
                {
                    return NotFound();
                }
                
                // Mapear manualmente a PeriodoVm
                var periodo = new PeriodoVm
                {
                    Id = periodoEntity.Id,
                    Codigo = periodoEntity.Codigo ?? "",
                    Nombre = periodoEntity.Nombre ?? "",
                    Tipo = periodoEntity.Tipo.ToString(),
                    Anio = periodoEntity.Anio,
                    Ciclo = periodoEntity.Ciclo ?? "",
                    NumeroPeriodo = periodoEntity.NumeroPeriodo,
                    FechaInicio = periodoEntity.FechaInicio,
                    FechaFin = periodoEntity.FechaFin,
                    Activo = periodoEntity.Activo,
                    FechaCreacion = periodoEntity.FechaCreacion,
                    FechaModificacion = periodoEntity.FechaModificacion,
                    Estado = periodoEntity.Estado
                };
                
                return View(periodo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los detalles del período académico {Id}", id);
                return NotFound();
            }
        }

        // GET: PeriodosAcademicos/Create
        [RequirePermission(ApplicationPermissions.Periodos.CREAR)]
        public IActionResult Create()
        {
            try
            {
                return View(new CrearPeriodoVm());
            }
            catch (Exception ex)
            {
                _errorLogService.LogErrorAsync(
                    "PeriodosAcademicosController",
                    "Create",
                    ex,
                    User?.Identity?.Name,
                    User?.Identity?.Name,
                    Request.Path,
                    Request.Method,
                    null,
                    HttpContext.Connection.RemoteIpAddress?.ToString(),
                    Request.Headers["User-Agent"].ToString()
                ).Wait();

                TempData["ErrorMessage"] = $"Error al cargar el formulario: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: PeriodosAcademicos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequirePermission(ApplicationPermissions.Periodos.CREAR)]
        public async Task<IActionResult> Create(CrearPeriodoVm model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var result = await _periodosService.CreateAsync(model);
                if (result.Success)
                {
                    TempData["SuccessMessage"] = result.Message;
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError("", result.Message ?? "Error desconocido");
                return View(model);
            }
            catch (Exception ex)
            {
                await _errorLogService.LogErrorAsync(
                    "PeriodosAcademicosController",
                    "Create",
                    ex,
                    User?.Identity?.Name,
                    User?.Identity?.Name,
                    Request.Path,
                    Request.Method,
                    System.Text.Json.JsonSerializer.Serialize(model),
                    HttpContext.Connection.RemoteIpAddress?.ToString(),
                    Request.Headers["User-Agent"].ToString()
                );

                _logger.LogError(ex, "Error al crear el período académico");

                // Registrar excepción en BD
                await _excepcionService.RegistrarExcepcionAsync(
                    operacion: $"Crear Período Académico: {model.Nombre}",
                    ex: ex,
                    usuario: User?.Identity?.Name,
                    ipAddress: _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString(),
                    urlSolicitada: _httpContextAccessor.HttpContext?.Request?.Path,
                    metodoHttp: _httpContextAccessor.HttpContext?.Request?.Method,
                    parametrosEntrada: System.Text.Json.JsonSerializer.Serialize(model),
                    severidad: "Error"
                );

                ModelState.AddModelError("", "Error interno del servidor: " + ex.Message);
                return View(model);
            }
        }

        // GET: PeriodosAcademicos/Edit/5
        [RequirePermission(ApplicationPermissions.Periodos.EDITAR)]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                // 🔧 CORRECCIÓN TEMPORAL: Usar consulta directa para evitar error SQLite
                var periodoEntity = await _context.PeriodosAcademicos
                    .Where(p => p.Id == id)
                    .FirstOrDefaultAsync();
                
                if (periodoEntity == null)
                {
                    return NotFound();
                }
                
                var model = new EditarPeriodoVm
                {
                    Id = periodoEntity.Id,
                    Codigo = periodoEntity.Codigo ?? "",
                    Nombre = periodoEntity.Nombre ?? "",
                    Tipo = periodoEntity.Tipo.ToString(),
                    Anio = periodoEntity.Anio,
                    Ciclo = periodoEntity.Ciclo ?? "",
                    NumeroPeriodo = periodoEntity.NumeroPeriodo,
                    FechaInicio = periodoEntity.FechaInicio,
                    FechaFin = periodoEntity.FechaFin,
                    Activo = periodoEntity.Activo
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el período académico para editar {Id}. SQLite Error: {ErrorMessage}", id, ex.Message);
                TempData["ErrorMessage"] = $"Error de base de datos: {ex.Message}";
                return NotFound();
            }
        }

        // POST: PeriodosAcademicos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequirePermission(ApplicationPermissions.Periodos.EDITAR)]
        public async Task<IActionResult> Edit(int id, EditarPeriodoVm model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var result = await _periodosService.UpdateAsync(id, model);
                if (result.Success)
                {
                    TempData["SuccessMessage"] = result.Message;
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError("", result.Message ?? "Error desconocido");
                return View(model);
            }
            catch (Exception ex)
            {
                await _errorLogService.LogErrorAsync(
                    "PeriodosAcademicosController",
                    "Edit",
                    ex,
                    User?.Identity?.Name,
                    User?.Identity?.Name,
                    Request.Path,
                    Request.Method,
                    System.Text.Json.JsonSerializer.Serialize(new { id, model }),
                    HttpContext.Connection.RemoteIpAddress?.ToString(),
                    Request.Headers["User-Agent"].ToString()
                );

                _logger.LogError(ex, "Error al actualizar el período académico {Id}", id);

                // Registrar excepción en BD
                await _excepcionService.RegistrarExcepcionAsync(
                    operacion: $"Actualizar Período Académico ID: {id}, Nombre: {model.Nombre}",
                    ex: ex,
                    usuario: User?.Identity?.Name,
                    ipAddress: _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString(),
                    urlSolicitada: _httpContextAccessor.HttpContext?.Request?.Path,
                    metodoHttp: _httpContextAccessor.HttpContext?.Request?.Method,
                    parametrosEntrada: System.Text.Json.JsonSerializer.Serialize(new { id, model }),
                    severidad: "Error"
                );

                ModelState.AddModelError("", "Error interno del servidor: " + ex.Message);
                return View(model);
            }
        }

        // GET: PeriodosAcademicos/Delete/5
        [RequirePermission(ApplicationPermissions.Periodos.ELIMINAR)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                // 🔧 CORRECCIÓN TEMPORAL: Usar consulta directa
                var periodoEntity = await _context.PeriodosAcademicos
                    .Where(p => p.Id == id)
                    .FirstOrDefaultAsync();
                
                if (periodoEntity == null)
                {
                    return NotFound();
                }
                
                var periodo = new PeriodoVm
                {
                    Id = periodoEntity.Id,
                    Codigo = periodoEntity.Codigo ?? "",
                    Nombre = periodoEntity.Nombre ?? "",
                    Tipo = periodoEntity.Tipo.ToString(),
                    Anio = periodoEntity.Anio,
                    Ciclo = periodoEntity.Ciclo ?? "",
                    NumeroPeriodo = periodoEntity.NumeroPeriodo,
                    FechaInicio = periodoEntity.FechaInicio,
                    FechaFin = periodoEntity.FechaFin,
                    Activo = periodoEntity.Activo,
                    FechaCreacion = periodoEntity.FechaCreacion,
                    FechaModificacion = periodoEntity.FechaModificacion,
                    Estado = periodoEntity.Estado
                };
                
                return View(periodo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el período académico para eliminar {Id}", id);
                return NotFound();
            }
        }

        // POST: PeriodosAcademicos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [RequirePermission(ApplicationPermissions.Periodos.ELIMINAR)]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var result = await _periodosService.DeleteAsync(id);
                if (result.Success)
                {
                    TempData["SuccessMessage"] = result.Message;
                }
                else
                {
                    TempData["ErrorMessage"] = result.Message;
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el período académico {Id}", id);
                TempData["ErrorMessage"] = "Error interno del servidor";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: PeriodosAcademicos/Ofertas/5
        [RequirePermission(ApplicationPermissions.Periodos.VER)]
        public async Task<IActionResult> Ofertas(int id)
        {
            try
            {
                var periodo = await _periodosService.GetByIdAsync(id);
                if (periodo == null)
                {
                    return NotFound();
                }

                var ofertas = await _periodosService.GetOfertasAsync(id);
                
                ViewBag.Periodo = periodo;
                return View(ofertas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener ofertas del período académico {Id}", id);
                return NotFound();
            }
        }

        // GET: PeriodosAcademicos/CrearOferta/5
        [RequirePermission(ApplicationPermissions.Periodos.CREAR)]
        public async Task<IActionResult> CrearOferta(int id)
        {
            try
            {
                var periodo = await _periodosService.GetByIdAsync(id);
                if (periodo == null)
                {
                    return NotFound();
                }

                var model = new CrearOfertaMateriaVm
                {
                    PeriodoId = id,
                    PeriodoNombre = periodo.Nombre,
                    Estado = "Disponible",
                    Cupo = 30 // Valor por defecto
                };

                // Aquí se cargarían las materias disponibles
                // ViewBag.Materias = await GetMateriasDisponibles();

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al preparar creación de oferta para período {Id}", id);
                return NotFound();
            }
        }

        // POST: PeriodosAcademicos/CrearOferta
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequirePermission(ApplicationPermissions.Periodos.CREAR)]
        public async Task<IActionResult> CrearOferta(CrearOfertaMateriaVm model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var result = await _periodosService.CrearOfertaAsync(model);
                if (result.Success)
                {
                    TempData["SuccessMessage"] = result.Message;
                    return RedirectToAction(nameof(Ofertas), new { id = model.PeriodoId });
                }

                ModelState.AddModelError("", result.Message ?? "Error desconocido");
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear oferta de materia");
                ModelState.AddModelError("", "Error interno del servidor");
                return View(model);
            }
        }

        // POST: PeriodosAcademicos/CerrarOferta
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequirePermission(ApplicationPermissions.Periodos.EDITAR)]
        public async Task<IActionResult> CerrarOferta(int ofertaId, int periodoId)
        {
            try
            {
                var result = await _periodosService.CerrarOfertaAsync(ofertaId);
                if (result.Success)
                {
                    TempData["SuccessMessage"] = result.Message;
                }
                else
                {
                    TempData["ErrorMessage"] = result.Message;
                }

                return RedirectToAction(nameof(Ofertas), new { id = periodoId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cerrar oferta {OfertaId}", ofertaId);
                TempData["ErrorMessage"] = "Error interno del servidor";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: PeriodosAcademicos/Activar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequirePermission(ApplicationPermissions.Periodos.ACTIVAR)]
        public async Task<IActionResult> Activar(int id)
        {
            try
            {
                // Buscar el período
                var periodo = await _context.PeriodosAcademicos
                    .Where(p => p.Id == id)
                    .FirstOrDefaultAsync();

                if (periodo == null)
                {
                    TempData["ErrorMessage"] = "Período académico no encontrado";
                    return RedirectToAction(nameof(Index));
                }

                // Activar el período
                periodo.Activo = true;
                periodo.FechaModificacion = DateTime.Now;
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Período académico '{periodo.Nombre}' activado exitosamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al activar el período académico {Id}", id);
                TempData["ErrorMessage"] = "Error interno del servidor";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: PeriodosAcademicos/Cerrar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequirePermission(ApplicationPermissions.Periodos.CERRAR)]
        public async Task<IActionResult> Cerrar(int id)
        {
            try
            {
                // Buscar el período
                var periodo = await _context.PeriodosAcademicos
                    .Where(p => p.Id == id)
                    .FirstOrDefaultAsync();

                if (periodo == null)
                {
                    TempData["ErrorMessage"] = "Período académico no encontrado";
                    return RedirectToAction(nameof(Index));
                }

                // Cerrar el período (desactivar)
                periodo.Activo = false;
                periodo.Estado = "Cerrado";
                periodo.FechaModificacion = DateTime.Now;
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Período académico '{periodo.Nombre}' cerrado exitosamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cerrar el período académico {Id}", id);
                TempData["ErrorMessage"] = "Error interno del servidor";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: PeriodosAcademicos/GestionarCalendario/5
        [RequirePermission(ApplicationPermissions.Periodos.GESTIONAR_CALENDARIO)]
        public async Task<IActionResult> GestionarCalendario(int id)
        {
            try
            {
                var periodo = await _context.PeriodosAcademicos
                    .Where(p => p.Id == id)
                    .FirstOrDefaultAsync();

                if (periodo == null)
                {
                    return NotFound();
                }

                var periodoVm = new PeriodoVm
                {
                    Id = periodo.Id,
                    Codigo = periodo.Codigo ?? "",
                    Nombre = periodo.Nombre ?? "",
                    Tipo = periodo.Tipo.ToString(),
                    Anio = periodo.Anio,
                    Ciclo = periodo.Ciclo ?? "",
                    NumeroPeriodo = periodo.NumeroPeriodo,
                    FechaInicio = periodo.FechaInicio,
                    FechaFin = periodo.FechaFin,
                    Activo = periodo.Activo,
                    FechaCreacion = periodo.FechaCreacion,
                    FechaModificacion = periodo.FechaModificacion,
                    Estado = periodo.Estado
                };

                return View(periodoVm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al acceder a la gestión de calendario del período {Id}", id);
                return NotFound();
            }
        }
    }
}