using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using DocinadeApp.Data;
using DocinadeApp.Models;
using DocinadeApp.Services;
using DocinadeApp.Authorization;
using DocinadeApp.Models.Permissions;
using System.Text.Json;

namespace DocinadeApp.Controllers
{
    public class EstudiantesController : BaseController
    {
        private readonly IEstudianteImportService _importService;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly IErrorLogService _errorLogService;

        public EstudiantesController(
            RubricasDbContext context, 
            IEstudianteImportService importService,
            IPeriodoAcademicoService periodoService,
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            IErrorLogService errorLogService) 
            : base(periodoService, context)
        {
            _importService = importService;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _errorLogService = errorLogService;
        }

        // GET: Estudiantes
        [RequirePermission(ApplicationPermissions.Estudiantes.VER)]
        public async Task<IActionResult> Index(int? periodoId, int? año)
        {
            try
            {
                // Usar el filtro automático de período del BaseController
                var periodoParaFiltrar = GetFiltroPeridoParaQuery(periodoId);
                
                var query = _context.Estudiantes
                    .Include(e => e.PeriodoAcademico)
                    .AsQueryable();

                // Aplicar filtro de período (automático o manual)
                if (periodoParaFiltrar.HasValue)
                {
                    query = query.Where(e => e.PeriodoAcademicoId == periodoParaFiltrar.Value);
                }

                if (año.HasValue)
                {
                    query = query.Where(e => e.Anio == año.Value);
                }

                var estudiantes = await query
                    .OrderBy(e => e.Apellidos)
                    .ThenBy(e => e.Nombre)
                    .ToListAsync();

                // Usar el método centralizado del BaseController para los dropdowns
                ViewBag.Periodos = await GetPeriodosAcademicosSelectListAsync();

                ViewBag.Años = await _context.Estudiantes
                    .Select(e => e.Anio)
                    .Distinct()
                    .OrderBy(a => a)
                    .Select(a => new SelectListItem
                    {
                        Value = a.ToString(),
                        Text = a.ToString()
                    })
                    .ToListAsync();

                // Establecer valores seleccionados para los dropdowns
                ViewBag.PeriodoSeleccionado = periodoParaFiltrar;
                ViewBag.AñoSeleccionado = año;

                return View(estudiantes);
            }
            catch (Exception ex)
            {
                await _errorLogService.LogErrorAsync(
                    "EstudiantesController",
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

                TempData["Error"] = $"Error al cargar la lista de estudiantes: {ex.Message}";
                return View(new List<Estudiante>());
            }
        }

        // GET: Estudiantes/Details/5
        [RequirePermission(ApplicationPermissions.Estudiantes.VER)]
        public async Task<IActionResult> Details(int? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                var estudiante = await _context.Estudiantes
                    .Include(e => e.PeriodoAcademico)
                    .Include(e => e.Evaluaciones)
                    .ThenInclude(ev => ev.Rubrica)
                    .FirstOrDefaultAsync(m => m.IdEstudiante == id);

                if (estudiante == null)
                {
                    return NotFound();
                }

                return View(estudiante);
            }
            catch (Exception ex)
            {
                await _errorLogService.LogErrorAsync(
                    "EstudiantesController",
                    "Details",
                    ex,
                    User?.Identity?.Name,
                    User?.Identity?.Name,
                    Request.Path,
                    Request.Method,
                    null,
                    HttpContext.Connection.RemoteIpAddress?.ToString(),
                    Request.Headers["User-Agent"].ToString()
                );

                TempData["Error"] = $"Error al cargar los detalles del estudiante: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Estudiantes/Create
        [RequirePermission(ApplicationPermissions.Estudiantes.CREAR)]
        public async Task<IActionResult> Create()
        {
            try
            {
                ViewBag.PeriodosAcademicos = await GetPeriodosAcademicosSelectListAsync();
                return View();
            }
            catch (Exception ex)
            {
                await _errorLogService.LogErrorAsync(
                    "EstudiantesController",
                    "Create",
                    ex,
                    User?.Identity?.Name,
                    User?.Identity?.Name,
                    Request.Path,
                    Request.Method,
                    null,
                    HttpContext.Connection.RemoteIpAddress?.ToString(),
                    Request.Headers["User-Agent"].ToString()
                );

                TempData["Error"] = $"Error al cargar el formulario de creación: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Estudiantes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequirePermission(ApplicationPermissions.Estudiantes.CREAR)]
        public async Task<IActionResult> Create([Bind("Nombre,Apellidos,NumeroId,DireccionCorreo,Institucion,Grupos,Anio,PeriodoAcademicoId")] Estudiante estudiante)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        _context.Add(estudiante);
                        await _context.SaveChangesAsync();
                        TempData["SuccessMessage"] = "Estudiante creado exitosamente.";
                        return RedirectToAction(nameof(Index));
                    }
                    catch (DbUpdateException ex)
                    {
                        await _errorLogService.LogErrorAsync(
                            "EstudiantesController",
                            "Create",
                            ex,
                            User?.Identity?.Name,
                            User?.Identity?.Name,
                            Request.Path,
                            Request.Method,
                            JsonSerializer.Serialize(estudiante),
                            HttpContext.Connection.RemoteIpAddress?.ToString(),
                            Request.Headers["User-Agent"].ToString()
                        );

                        if (ex.InnerException != null && ex.InnerException.Message.Contains("UNIQUE"))
                        {
                            if (ex.InnerException.Message.Contains("NumeroId"))
                            {
                                ModelState.AddModelError("NumeroId", "Ya existe un estudiante con este número de ID.");
                            }
                            else if (ex.InnerException.Message.Contains("DireccionCorreo"))
                            {
                                ModelState.AddModelError("DireccionCorreo", "Ya existe un estudiante con esta dirección de correo.");
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("", "Error al guardar el estudiante: " + ex.InnerException?.Message ?? ex.Message);
                        }
                    }
                }
                
                ViewBag.PeriodosAcademicos = await GetPeriodosAcademicosSelectListAsync();
                return View(estudiante);
            }
            catch (Exception ex)
            {
                await _errorLogService.LogErrorAsync(
                    "EstudiantesController",
                    "Create",
                    ex,
                    User?.Identity?.Name,
                    User?.Identity?.Name,
                    Request.Path,
                    Request.Method,
                    JsonSerializer.Serialize(estudiante),
                    HttpContext.Connection.RemoteIpAddress?.ToString(),
                    Request.Headers["User-Agent"].ToString()
                );

                TempData["Error"] = $"Error al crear el estudiante: {ex.Message}";
                ViewBag.PeriodosAcademicos = await GetPeriodosAcademicosSelectListAsync();
                return View(estudiante);
            }
        }

        // GET: Estudiantes/Edit/5
        [RequirePermission(ApplicationPermissions.Estudiantes.EDITAR)]
        public async Task<IActionResult> Edit(int? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                var estudiante = await _context.Estudiantes.FindAsync(id);
                if (estudiante == null)
                {
                    return NotFound();
                }
                
                // Pasar el período actual del estudiante como valor seleccionado
                ViewBag.PeriodosAcademicos = await GetPeriodosAcademicosSelectListAsync(estudiante.PeriodoAcademicoId);
                return View(estudiante);
            }
            catch (Exception ex)
            {
                await _errorLogService.LogErrorAsync(
                    "EstudiantesController",
                    "Edit",
                    ex,
                    User?.Identity?.Name,
                    User?.Identity?.Name,
                    Request.Path,
                    Request.Method,
                    null,
                    HttpContext.Connection.RemoteIpAddress?.ToString(),
                    Request.Headers["User-Agent"].ToString()
                );

                TempData["Error"] = $"Error al cargar el formulario de edición: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Estudiantes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequirePermission(ApplicationPermissions.Estudiantes.EDITAR)]
        public async Task<IActionResult> Edit(int id, [Bind("IdEstudiante,Nombre,Apellidos,NumeroId,DireccionCorreo,Institucion,Grupos,Anio,PeriodoAcademicoId,Estado,TipoAdecuacion,DetallesACS,FechaInicioACS,PeriodoInicioACSId,AplicarACSPeriodosAnteriores")] Estudiante estudiante)
        {
            try
            {
                if (id != estudiante.IdEstudiante)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        _context.Update(estudiante);
                        await _context.SaveChangesAsync();
                        TempData["SuccessMessage"] = "Estudiante actualizado exitosamente.";
                        return RedirectToAction(nameof(Index));
                    }
                    catch (DbUpdateConcurrencyException ex)
                    {
                        await _errorLogService.LogErrorAsync(
                            "EstudiantesController",
                            "Edit",
                            ex,
                            User?.Identity?.Name,
                            User?.Identity?.Name,
                            Request.Path,
                            Request.Method,
                            JsonSerializer.Serialize(estudiante),
                            HttpContext.Connection.RemoteIpAddress?.ToString(),
                            Request.Headers["User-Agent"].ToString()
                        );

                        if (!EstudianteExists(estudiante.IdEstudiante))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                    catch (DbUpdateException ex)
                    {
                        await _errorLogService.LogErrorAsync(
                            "EstudiantesController",
                            "Edit",
                            ex,
                            User?.Identity?.Name,
                            User?.Identity?.Name,
                            Request.Path,
                            Request.Method,
                            JsonSerializer.Serialize(estudiante),
                            HttpContext.Connection.RemoteIpAddress?.ToString(),
                            Request.Headers["User-Agent"].ToString()
                        );

                        if (ex.InnerException != null && ex.InnerException.Message.Contains("UNIQUE"))
                        {
                            if (ex.InnerException.Message.Contains("NumeroId"))
                            {
                                ModelState.AddModelError("NumeroId", "Ya existe un estudiante con este número de ID.");
                            }
                            else if (ex.InnerException.Message.Contains("DireccionCorreo"))
                            {
                                ModelState.AddModelError("DireccionCorreo", "Ya existe un estudiante con esta dirección de correo.");
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("", "Error al actualizar el estudiante: " + (ex.InnerException?.Message ?? ex.Message));
                        }
                    }
                }
                
                ViewBag.PeriodosAcademicos = await GetPeriodosAcademicosSelectListAsync();
                return View(estudiante);
            }
            catch (Exception ex)
            {
                await _errorLogService.LogErrorAsync(
                    "EstudiantesController",
                    "Edit",
                    ex,
                    User?.Identity?.Name,
                    User?.Identity?.Name,
                    Request.Path,
                    Request.Method,
                    JsonSerializer.Serialize(estudiante),
                    HttpContext.Connection.RemoteIpAddress?.ToString(),
                    Request.Headers["User-Agent"].ToString()
                );

                TempData["Error"] = $"Error al actualizar el estudiante: {ex.Message}";
                ViewBag.PeriodosAcademicos = await GetPeriodosAcademicosSelectListAsync();
                return View(estudiante);
            }
        }

        // GET: Estudiantes/Delete/5
        [RequirePermission(ApplicationPermissions.Estudiantes.ELIMINAR)]
        public async Task<IActionResult> Delete(int? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                var estudiante = await _context.Estudiantes
                    .Include(e => e.PeriodoAcademico)
                    .FirstOrDefaultAsync(m => m.IdEstudiante == id);
                if (estudiante == null)
                {
                    return NotFound();
                }

                return View(estudiante);
            }
            catch (Exception ex)
            {
                await _errorLogService.LogErrorAsync(
                    "EstudiantesController",
                    "Delete",
                    ex,
                    User?.Identity?.Name,
                    User?.Identity?.Name,
                    Request.Path,
                    Request.Method,
                    null,
                    HttpContext.Connection.RemoteIpAddress?.ToString(),
                    Request.Headers["User-Agent"].ToString()
                );

                TempData["Error"] = $"Error al cargar el formulario de eliminación: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Estudiantes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [RequirePermission(ApplicationPermissions.Estudiantes.ELIMINAR)]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var estudiante = await _context.Estudiantes.FindAsync(id);
                if (estudiante != null)
                {
                    try
                    {
                        _context.Estudiantes.Remove(estudiante);
                        await _context.SaveChangesAsync();
                        TempData["SuccessMessage"] = "Estudiante eliminado exitosamente.";
                    }
                    catch (DbUpdateException ex)
                    {
                        await _errorLogService.LogErrorAsync(
                            "EstudiantesController",
                            "DeleteConfirmed",
                            ex,
                            User?.Identity?.Name,
                            User?.Identity?.Name,
                            Request.Path,
                            Request.Method,
                            id.ToString(),
                            HttpContext.Connection.RemoteIpAddress?.ToString(),
                            Request.Headers["User-Agent"].ToString()
                        );

                        TempData["ErrorMessage"] = "No se puede eliminar el estudiante porque tiene evaluaciones asociadas.";
                        return RedirectToAction(nameof(Index));
                    }
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                await _errorLogService.LogErrorAsync(
                    "EstudiantesController",
                    "DeleteConfirmed",
                    ex,
                    User?.Identity?.Name,
                    User?.Identity?.Name,
                    Request.Path,
                    Request.Method,
                    id.ToString(),
                    HttpContext.Connection.RemoteIpAddress?.ToString(),
                    Request.Headers["User-Agent"].ToString()
                );

                TempData["Error"] = $"Error al eliminar el estudiante: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Estudiantes/ImportarExcel
        [RequirePermission(ApplicationPermissions.Estudiantes.IMPORTAR)]
        public async Task<IActionResult> ImportarExcel()
        {
            ViewBag.PeriodosAcademicos = await GetPeriodosAcademicosSelectListAsync();
            
            // Estadísticas para la vista
            ViewBag.TotalEstudiantes = await _context.Estudiantes.CountAsync();
            ViewBag.ImportacionesRecientes = 0; // Esto se podría implementar con un log de importaciones
            ViewBag.UltimaImportacion = "No disponible"; // Esto también se podría implementar con un log
            
            return View();
        }

        // POST: Estudiantes/ImportarExcel
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequirePermission(ApplicationPermissions.Estudiantes.IMPORTAR)]
        public async Task<IActionResult> ImportarExcel(IFormFile archivo, int periodoAcademicoId)
        {
            if (archivo == null || archivo.Length == 0)
            {
                ModelState.AddModelError("archivo", "Por favor seleccione un archivo Excel.");
                ViewBag.PeriodosAcademicos = await GetPeriodosAcademicosSelectListAsync();
                return View();
            }

            if (!archivo.FileName.EndsWith(".xlsx") && !archivo.FileName.EndsWith(".xls"))
            {
                ModelState.AddModelError("archivo", "El archivo debe ser un archivo Excel (.xlsx o .xls).");
                ViewBag.PeriodosAcademicos = await GetPeriodosAcademicosSelectListAsync();
                return View();
            }

            try
            {
                var resultado = await _importService.ImportarEstudiantesAsync(archivo, periodoAcademicoId);
                
                if (resultado.Exitoso)
                {
                    TempData["SuccessMessage"] = $"Importación completada. {resultado.EstudiantesImportados} estudiantes importados.";
                    
                    if (resultado.Advertencias.Any())
                    {
                        TempData["WarningMessage"] = string.Join("<br>", resultado.Advertencias);
                    }
                    
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    foreach (var error in resultado.Errores)
                    {
                        ModelState.AddModelError("", error);
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error al importar archivo: {ex.Message}");
            }

            ViewBag.PeriodosAcademicos = await GetPeriodosAcademicosSelectListAsync();
            ViewBag.TotalEstudiantes = await _context.Estudiantes.CountAsync();
            ViewBag.ImportacionesRecientes = 0;
            ViewBag.UltimaImportacion = "No disponible";
            return View();
        }

        // GET: Estudiantes/DescargarPlantilla
        [RequirePermission(ApplicationPermissions.Estudiantes.EXPORTAR)]
        public IActionResult DescargarPlantilla()
        {
            // Configurar licencia de EPPlus 8+
            ExcelPackage.License.SetNonCommercialPersonal("RubricasApp");
            
            // Crear un archivo Excel con la plantilla
            var stream = new MemoryStream();
            
            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.Add("Estudiantes");
                
                // Encabezados
                worksheet.Cells[1, 1].Value = "Nombre";
                worksheet.Cells[1, 2].Value = "Apellido(s)";
                worksheet.Cells[1, 3].Value = "Número de ID";
                worksheet.Cells[1, 4].Value = "Dirección de correo";
                worksheet.Cells[1, 5].Value = "Institución";
                worksheet.Cells[1, 6].Value = "Grupos";
                
                // Ejemplos
                worksheet.Cells[2, 1].Value = "LADY YARENIS";
                worksheet.Cells[2, 2].Value = "ABARCA BRENES";
                worksheet.Cells[2, 3].Value = "0208390339";
                worksheet.Cells[2, 4].Value = "lady.abarca@uned.cr";
                worksheet.Cells[2, 5].Value = "PALMARES (06)";
                worksheet.Cells[2, 6].Value = "Grupo 02: Tutor Heiner Guido Cambronero";
                
                // Formato
                worksheet.Cells[1, 1, 1, 6].Style.Font.Bold = true;
                worksheet.Cells.AutoFitColumns();
                
                package.Save();
            }
            
            stream.Position = 0;
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "PlantillaEstudiantes.xlsx");
        }

        // POST: Estudiantes/EliminarPorFiltro
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequirePermission(ApplicationPermissions.Estudiantes.ELIMINAR)]
        public async Task<IActionResult> EliminarPorFiltro(int? periodoId, int? año)
        {
            try
            {
                var query = _context.Estudiantes.AsQueryable();

                if (periodoId.HasValue)
                {
                    query = query.Where(e => e.PeriodoAcademicoId == periodoId.Value);
                }

                if (año.HasValue)
                {
                    query = query.Where(e => e.Anio == año.Value);
                }

                var estudiantesAEliminar = await query.ToListAsync();

                if (!estudiantesAEliminar.Any())
                {
                    TempData["WarningMessage"] = "No se encontraron estudiantes que coincidan con los filtros aplicados.";
                    return RedirectToAction(nameof(Index), new { periodoId, año });
                }

                // Verificar si algún estudiante tiene evaluaciones asociadas
                var estudiantesConEvaluaciones = await _context.Evaluaciones
                    .Where(ev => estudiantesAEliminar.Select(e => e.IdEstudiante).Contains(ev.IdEstudiante))
                    .Select(ev => ev.IdEstudiante)
                    .Distinct()
                    .ToListAsync();

                if (estudiantesConEvaluaciones.Any())
                {
                    var nombresPeriodo = "";
                    var nombresAño = "";
                    
                    if (periodoId.HasValue)
                    {
                        var periodo = await _context.PeriodosAcademicos.FindAsync(periodoId.Value);
                        nombresPeriodo = $" del período {periodo?.Anio}-{periodo?.NumeroPeriodo}";
                    }
                    
                    if (año.HasValue)
                    {
                        nombresAño = $" del año {año.Value}";
                    }

                    TempData["ErrorMessage"] = $"No se pueden eliminar algunos estudiantes{nombresPeriodo}{nombresAño} porque tienen evaluaciones asociadas. " +
                                             $"Elimine primero las evaluaciones o contacte al administrador.";
                    return RedirectToAction(nameof(Index), new { periodoId, año });
                }

                var cantidadEliminados = estudiantesAEliminar.Count;
                _context.Estudiantes.RemoveRange(estudiantesAEliminar);
                await _context.SaveChangesAsync();

                var filtroTexto = "";
                if (periodoId.HasValue || año.HasValue)
                {
                    var periodo = periodoId.HasValue ? await _context.PeriodosAcademicos.FindAsync(periodoId.Value) : null;
                    var partesPeriodo = periodo != null ? $"período {periodo.Anio}-{periodo.NumeroPeriodo}" : "";
                    var partesAño = año.HasValue ? $"año {año.Value}" : "";
                    
                    if (!string.IsNullOrEmpty(partesPeriodo) && !string.IsNullOrEmpty(partesAño))
                        filtroTexto = $" del {partesPeriodo} y {partesAño}";
                    else if (!string.IsNullOrEmpty(partesPeriodo))
                        filtroTexto = $" del {partesPeriodo}";
                    else if (!string.IsNullOrEmpty(partesAño))
                        filtroTexto = $" del {partesAño}";
                }

                TempData["SuccessMessage"] = $"Se eliminaron exitosamente {cantidadEliminados} estudiante(s){filtroTexto}.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al eliminar estudiantes: {ex.Message}";
            }

            return RedirectToAction(nameof(Index), new { periodoId, año });
        }

        /// <summary>
        /// Búsqueda AJAX de estudiantes para componente SearchableSelect
        /// </summary>
        /// <param name="q">Término de búsqueda</param>
        /// <param name="id">ID específico para precargar (opcional)</param>
        /// <param name="periodoId">Filtrar por período académico (opcional)</param>
        /// <param name="grupoId">Filtrar por grupo (opcional)</param>
        /// <param name="page">Página para paginación (opcional)</param>
        /// <param name="pageSize">Tamaño de página (opcional)</param>
        /// <returns>JSON con formato { id, text } compatible con Tom Select y Select2</returns>
        [HttpGet]
        [RequirePermission(ApplicationPermissions.Estudiantes.VER)]
        public async Task<IActionResult> Search(string q, int? id, int? periodoId, int? grupoId, int page = 1, int pageSize = 20)
        {
            try
            {
                var query = _context.Estudiantes
                    .Include(e => e.PeriodoAcademico)
                    .AsQueryable();

                // Si se solicita un ID específico (para precargar), devolver solo ese estudiante
                if (id.HasValue)
                {
                    var estudiante = await query
                        .Where(e => e.IdEstudiante == id.Value)
                        .Select(e => new { 
                            id = e.IdEstudiante, 
                            text = $"{e.Apellidos}, {e.Nombre} ({e.NumeroId})",
                            numeroId = e.NumeroId,
                            periodo = e.PeriodoAcademico != null ? $"{e.PeriodoAcademico.Anio}-{e.PeriodoAcademico.NumeroPeriodo}" : ""
                        })
                        .FirstOrDefaultAsync();

                    return Json(estudiante != null ? new[] { estudiante } : new object[0]);
                }

                // Filtros opcionales
                if (periodoId.HasValue)
                {
                    query = query.Where(e => e.PeriodoAcademicoId == periodoId.Value);
                }

                if (grupoId.HasValue)
                {
                    // Filtrar por estudiantes que pertenezcan al grupo
                    var estudiantesEnGrupo = await _context.EstudianteGrupos
                        .Where(eg => eg.GrupoId == grupoId.Value)
                        .Select(eg => eg.EstudianteId)
                        .ToListAsync();
                    
                    query = query.Where(e => estudiantesEnGrupo.Contains(e.IdEstudiante));
                }

                // Búsqueda por término
                if (!string.IsNullOrWhiteSpace(q))
                {
                    q = q.Trim().ToLowerInvariant();
                    query = query.Where(e => 
                        e.Nombre.ToLower().Contains(q) ||
                        e.Apellidos.ToLower().Contains(q) ||
                        e.NumeroId.Contains(q) ||
                        (e.Nombre.ToLower() + " " + e.Apellidos.ToLower()).Contains(q) ||
                        (e.Apellidos.ToLower() + " " + e.Nombre.ToLower()).Contains(q)
                    );
                }

                // Aplicar paginación
                var totalCount = await query.CountAsync();
                var estudiantes = await query
                    .OrderBy(e => e.Apellidos)
                    .ThenBy(e => e.Nombre)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(e => new { 
                        id = e.IdEstudiante, 
                        text = $"{e.Apellidos}, {e.Nombre} ({e.NumeroId})",
                        numeroId = e.NumeroId,
                        periodo = e.PeriodoAcademico != null ? $"{e.PeriodoAcademico.Anio}-{e.PeriodoAcademico.NumeroPeriodo}" : ""
                    })
                    .ToListAsync();

                // Respuesta compatible con Tom Select y Select2
                var response = new
                {
                    items = estudiantes,
                    total_count = totalCount,
                    incomplete_results = (page * pageSize) < totalCount
                };

                return Json(response);
            }
            catch (Exception ex)
            {
                return Json(new { error = "Error en la búsqueda: " + ex.Message });
            }
        }

        // GET: Estudiantes/BuscarPorCedula
        [HttpGet]
        [RequirePermission(ApplicationPermissions.Estudiantes.VER)]
        public async Task<IActionResult> BuscarPorCedula(string cedula)
        {
            try
            {
                // Validar que la cédula no esté vacía
                if (string.IsNullOrWhiteSpace(cedula))
                {
                    return Json(new { success = false, message = "Debe ingresar una cédula" });
                }

                // Limpiar la cédula (remover espacios, guiones, etc.)
                cedula = cedula.Trim().Replace("-", "").Replace(" ", "");

                // Verificar si el API está habilitado
                var apiEnabled = _configuration.GetValue<bool>("ExternalApis:CedulaCostaRica:Enabled");
                if (!apiEnabled)
                {
                    return Json(new { success = false, message = "El servicio de búsqueda de cédula no está habilitado" });
                }

                // Obtener la URL base del API desde la configuración
                var baseUrl = _configuration.GetValue<string>("ExternalApis:CedulaCostaRica:BaseUrl");
                if (string.IsNullOrWhiteSpace(baseUrl))
                {
                    return Json(new { success = false, message = "URL del servicio no configurada" });
                }

                // Construir la URL completa
                var url = $"{baseUrl}/{cedula}";

                // Crear cliente HTTP
                var httpClient = _httpClientFactory.CreateClient();
                httpClient.Timeout = TimeSpan.FromSeconds(10);

                // Realizar la petición
                var response = await httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    return Json(new { 
                        success = false, 
                        message = $"Error al consultar el servicio: {response.StatusCode}" 
                    });
                }

                // Leer la respuesta
                var jsonContent = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                
                var apiResponse = JsonSerializer.Deserialize<CedulaApiResponse>(jsonContent, options);

                // Verificar si se encontraron resultados
                if (apiResponse == null || apiResponse.ResultCount == 0 || apiResponse.Results == null || !apiResponse.Results.Any())
                {
                    return Json(new { 
                        success = false, 
                        message = "No se encontró información para esta cédula" 
                    });
                }

                // Obtener el primer resultado
                var resultado = apiResponse.Results.First();

                // Separar nombre y apellidos
                var nombre = string.Empty;
                var apellidos = string.Empty;

                // El API retorna firstname (nombre completo) y lastname (apellidos completos)
                if (!string.IsNullOrWhiteSpace(resultado.Firstname))
                {
                    nombre = resultado.Firstname.Trim();
                }

                if (!string.IsNullOrWhiteSpace(resultado.Lastname))
                {
                    apellidos = resultado.Lastname.Trim();
                }

                // Si no hay datos separados, intentar con fullname
                if (string.IsNullOrWhiteSpace(nombre) && string.IsNullOrWhiteSpace(apellidos) && !string.IsNullOrWhiteSpace(resultado.Fullname))
                {
                    var partes = resultado.Fullname.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    if (partes.Length > 2)
                    {
                        apellidos = $"{partes[0]} {partes[1]}";
                        nombre = string.Join(" ", partes.Skip(2));
                    }
                    else if (partes.Length == 2)
                    {
                        apellidos = partes[0];
                        nombre = partes[1];
                    }
                    else
                    {
                        nombre = resultado.Fullname;
                    }
                }

                return Json(new
                {
                    success = true,
                    data = new
                    {
                        nombre = nombre,
                        apellidos = apellidos,
                        nombreCompleto = resultado.Fullname ?? $"{apellidos} {nombre}".Trim()
                    }
                });
            }
            catch (TaskCanceledException)
            {
                return Json(new { 
                    success = false, 
                    message = "Tiempo de espera agotado al consultar el servicio" 
                });
            }
            catch (HttpRequestException ex)
            {
                return Json(new { 
                    success = false, 
                    message = $"Error de conexión: {ex.Message}" 
                });
            }
            catch (Exception ex)
            {
                return Json(new { 
                    success = false, 
                    message = $"Error inesperado: {ex.Message}" 
                });
            }
        }

        private bool EstudianteExists(int id)
        {
            return _context.Estudiantes.Any(e => e.IdEstudiante == id);
        }
    }

    // Clases para deserializar la respuesta del API
    public class CedulaApiResponse
    {
        public int ResultCount { get; set; }
        public List<CedulaResult>? Results { get; set; }
        public string? Nombre { get; set; }
    }

    public class CedulaResult
    {
        public string? Cedula { get; set; }
        public string? Firstname { get; set; }
        public string? Firstname1 { get; set; }
        public string? Firstname2 { get; set; }
        public string? Lastname { get; set; }
        public string? Lastname1 { get; set; }
        public string? Lastname2 { get; set; }
        public string? Fullname { get; set; }
        public string? Type { get; set; }
    }
}