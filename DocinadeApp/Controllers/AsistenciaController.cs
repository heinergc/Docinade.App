using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DocinadeApp.Data;
using DocinadeApp.ViewModels;
using DocinadeApp.Models;
using DocinadeApp.Authorization;
using System.IO;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace DocinadeApp.Controllers
{
    /// <summary>
    /// Controlador para el manejo de asistencia y pase de lista de estudiantes
    /// </summary>
    [Authorize]
    public class AsistenciaController : Controller
    {
        private readonly ILogger<AsistenciaController> _logger;
        private readonly RubricasDbContext _context;

        public AsistenciaController(ILogger<AsistenciaController> logger, RubricasDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        /// <summary>
        /// Página principal de asistencia con filtros por grupo
        /// </summary>
        /// <param name="periodoId">ID del período académico</param>
        /// <param name="grupoId">ID del grupo seleccionado</param>
        /// <param name="buscar">Término de búsqueda</param>
        /// <returns></returns>
        [RequirePermission("asistencia.ver")]
        public async Task<IActionResult> Index(int? periodoId, int? grupoId, string buscar)
        {
            try
            {
                _logger.LogInformation($"Iniciando carga de asistencia - PeriodoId: {periodoId}, GrupoId: {grupoId}, Buscar: '{buscar}'");

                // Obtener todos los períodos para el filtro
                var periodos = await _context.PeriodosAcademicos
                    .OrderByDescending(p => p.Anio)
                    .ThenByDescending(p => p.NumeroPeriodo)
                    .Select(p => new SelectListItem
                    {
                        Value = p.Id.ToString(),
                        Text = p.Nombre,
                        Selected = periodoId.HasValue && p.Id == periodoId.Value
                    })
                    .ToListAsync();

                var totalPeriodos = periodos.Count;
                _logger.LogInformation($"Períodos encontrados: {totalPeriodos}");

                // Si no se especifica período, usar el más reciente
                if (!periodoId.HasValue && periodos.Any())
                {
                    periodoId = int.Parse(periodos.First().Value);
                    _logger.LogInformation($"Período seleccionado automáticamente: {periodoId}");
                }

                // Obtener grupos del período seleccionado con filtros
                var gruposQuery = _context.GruposEstudiantes
                    .Where(g => g.Estado == Models.EstadoGrupo.Activo && 
                               (!periodoId.HasValue || g.PeriodoAcademicoId == periodoId.Value) &&
                               g.EstudianteGrupos.Any(eg => eg.Estado == Models.EstadoAsignacion.Activo)); // Solo grupos con estudiantes

                // Aplicar filtro de búsqueda si se proporciona
                if (!string.IsNullOrEmpty(buscar))
                {
                    gruposQuery = gruposQuery.Where(g => 
                        g.Nombre.Contains(buscar) || 
                        (g.Codigo != null && g.Codigo.Contains(buscar)) ||
                        (g.Descripcion != null && g.Descripcion.Contains(buscar)));
                }

                var grupos = await gruposQuery
                    .Include(g => g.PeriodoAcademico)
                    .Include(g => g.EstudianteGrupos.Where(eg => eg.Estado == Models.EstadoAsignacion.Activo))
                        .ThenInclude(eg => eg.Estudiante)
                    .OrderBy(g => g.Nombre)
                    .ToListAsync();

                // Log para diagnóstico
                _logger.LogInformation($"Grupos encontrados: {grupos.Count}, Período: {periodoId}, Búsqueda: '{buscar}'");

                // Diagnóstico adicional: verificar si hay grupos sin filtros
                var totalGrupos = await _context.GruposEstudiantes.CountAsync();
                var gruposActivos = await _context.GruposEstudiantes.Where(g => g.Estado == Models.EstadoGrupo.Activo).CountAsync();
                var gruposConEstudiantes = await _context.GruposEstudiantes
                    .Where(g => g.Estado == Models.EstadoGrupo.Activo && g.EstudianteGrupos.Any(eg => eg.Estado == Models.EstadoAsignacion.Activo))
                    .CountAsync();
                
                _logger.LogInformation($"Diagnóstico DB - Total grupos: {totalGrupos}, Activos: {gruposActivos}, Con estudiantes: {gruposConEstudiantes}");

                ViewBag.Periodos = periodos;
                ViewBag.PeriodoSeleccionado = periodoId;
                ViewBag.GrupoSeleccionado = grupoId;
                ViewBag.TerminoBusqueda = buscar;
                ViewBag.Grupos = grupos;

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar la página principal de asistencia");
                TempData["Error"] = "Error al cargar la información. Intente nuevamente.";
                return View();
            }
        }

        /// <summary>
        /// Método de diagnóstico temporal para verificar datos de grupos
        /// </summary>
        [HttpGet]
        [RequirePermission("asistencia.ver_diagnostico")]
        public async Task<IActionResult> Diagnostico()
        {
            try
            {
                var totalGrupos = await _context.GruposEstudiantes.CountAsync();
                var gruposActivos = await _context.GruposEstudiantes.Where(g => g.Estado == Models.EstadoGrupo.Activo).CountAsync();
                
                var gruposConDetalles = await _context.GruposEstudiantes
                    .Include(g => g.PeriodoAcademico)
                    .Include(g => g.EstudianteGrupos)
                    .Select(g => new {
                        g.GrupoId,
                        g.Nombre,
                        g.Codigo,
                        g.Estado,
                        PeriodoNombre = g.PeriodoAcademico != null ? g.PeriodoAcademico.Nombre : "Sin período",
                        TotalEstudiantes = g.EstudianteGrupos.Count(),
                        EstudiantesActivos = g.EstudianteGrupos.Count(eg => eg.Estado == Models.EstadoAsignacion.Activo)
                    })
                    .ToListAsync();

                var diagnostico = new
                {
                    TotalGrupos = totalGrupos,
                    GruposActivos = gruposActivos,
                    GruposConEstudiantesActivos = gruposConDetalles.Count(g => g.EstudiantesActivos > 0),
                    Grupos = gruposConDetalles
                };

                return Json(diagnostico);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en diagnóstico");
                return Json(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Muestra el formulario de pase de lista para un grupo específico
        /// </summary>
        /// <param name="grupoId">ID del grupo</param>
        /// <returns></returns>
        [HttpGet]
        [RequirePermission("asistencia.tomar_asistencia")]
        public async Task<IActionResult> PaseLista(int grupoId, int? materiaId = null)
        {
            try
            {
                // Cargar el grupo con sus estudiantes
                var grupo = await _context.GruposEstudiantes
                    .Include(g => g.PeriodoAcademico)
                    .Include(g => g.EstudianteGrupos.Where(eg => eg.Estado == Models.EstadoAsignacion.Activo))
                        .ThenInclude(eg => eg.Estudiante)
                    .FirstOrDefaultAsync(g => g.GrupoId == grupoId);

                if (grupo == null)
                {
                    TempData["Error"] = "Grupo no encontrado.";
                    return RedirectToAction(nameof(Index));
                }

                // Si no se especifica materia, usar la primera materia disponible
                if (!materiaId.HasValue)
                {
                    var primeraMateria = await _context.Materias.FirstOrDefaultAsync();
                    materiaId = primeraMateria?.MateriaId ?? 1;
                }

                var modelo = new PaseListaViewModel
                {
                    Grupo = new GrupoEstudiantesViewModel
                    {
                        Id = grupo.GrupoId,
                        Nombre = grupo.Nombre,
                        Codigo = grupo.Codigo,
                        Descripcion = grupo.Descripcion,
                        PeriodoAcademico = grupo.PeriodoAcademico?.Nombre
                    },
                    MateriaId = materiaId.Value,
                    Fecha = DateTime.Now.Date,
                    Estudiantes = grupo.EstudianteGrupos
                        .Where(eg => eg.Estado == Models.EstadoAsignacion.Activo && eg.Estudiante != null)
                        .OrderBy(eg => eg.Estudiante!.Apellidos)
                        .ThenBy(eg => eg.Estudiante!.Nombre)
                        .Select(eg => new EstudianteAsistenciaViewModel
                        {
                            EstudianteId = eg.Estudiante!.IdEstudiante,
                            NombreCompleto = $"{eg.Estudiante.Apellidos}, {eg.Estudiante.Nombre}",
                            NumeroId = eg.Estudiante.NumeroId,
                            Estado = "P" // Por defecto presente
                        })
                        .ToList()
                };

                // Cargar lista de materias para el dropdown
                ViewBag.Materias = await _context.Materias
                    .Select(m => new SelectListItem
                    {
                        Value = m.MateriaId.ToString(),
                        Text = m.Nombre,
                        Selected = m.MateriaId == materiaId
                    })
                    .ToListAsync();

                return View(modelo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar el pase de lista para el grupo {GrupoId}", grupoId);
                TempData["Error"] = "Error al cargar la información del grupo. Intente nuevamente.";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Procesa y guarda la asistencia enviada desde el formulario
        /// </summary>
        /// <param name="modelo">Datos del pase de lista</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequirePermission("asistencia.tomar_asistencia")]
        public async Task<IActionResult> GuardarAsistencia(PaseListaViewModel modelo)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    TempData["Error"] = "Por favor revise los datos ingresados.";
                    
                    // Recargar los datos necesarios para la vista
                    await RecargarDatosPaseLista(modelo);
                    
                    return View("PaseLista", modelo);
                }

                var usuarioId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                var registrosAsistencia = new List<Asistencia>();

                // Procesar cada estudiante y crear registro de asistencia
                foreach (var estudiante in modelo.Estudiantes)
                {
                    // Verificar si ya existe un registro para este estudiante, grupo y fecha
                    var asistenciaExistente = await _context.Asistencias
                        .FirstOrDefaultAsync(a => a.EstudianteId == estudiante.EstudianteId && 
                                                 a.GrupoId == modelo.Grupo.Id && 
                                                 a.MateriaId == modelo.MateriaId &&
                                                 a.Fecha.Date == modelo.Fecha.Date);

                    if (asistenciaExistente != null)
                    {
                        // Actualizar registro existente
                        asistenciaExistente.Estado = estudiante.Estado ?? "N";
                        asistenciaExistente.Justificacion = estudiante.Justificacion;
                        asistenciaExistente.EsModificacion = true;
                        asistenciaExistente.FechaModificacion = DateTime.Now;
                        asistenciaExistente.ModificadoPorId = usuarioId;

                        _context.Asistencias.Update(asistenciaExistente);
                        
                        _logger.LogInformation("Asistencia actualizada para estudiante {EstudianteId}, estado: {Estado}", 
                            estudiante.EstudianteId, estudiante.Estado);
                    }
                    else
                    {
                        // Crear nuevo registro
                        var nuevaAsistencia = new Asistencia
                        {
                            EstudianteId = estudiante.EstudianteId,
                            GrupoId = modelo.Grupo.Id,
                            MateriaId = modelo.MateriaId,
                            Fecha = modelo.Fecha.Date,
                            Estado = estudiante.Estado ?? "N",
                            Justificacion = estudiante.Justificacion,
                            RegistradoPorId = usuarioId,
                            FechaRegistro = DateTime.Now,
                            HoraLlegada = estudiante.Estado == "T" ? DateTime.Now.TimeOfDay : null
                        };

                        registrosAsistencia.Add(nuevaAsistencia);
                        
                        _logger.LogInformation("Nueva asistencia creada para estudiante {EstudianteId}, estado: {Estado}", 
                            estudiante.EstudianteId, estudiante.Estado);
                    }
                }

                // Agregar nuevos registros si los hay
                if (registrosAsistencia.Any())
                {
                    await _context.Asistencias.AddRangeAsync(registrosAsistencia);
                }

                // Guardar cambios en la base de datos
                await _context.SaveChangesAsync();

                TempData["Success"] = $"La asistencia ha sido guardada exitosamente. Se procesaron {modelo.Estudiantes.Count} estudiantes.";
                return RedirectToAction(nameof(Resumen), new { grupoId = modelo.Grupo.Id, fecha = modelo.Fecha.ToString("yyyy-MM-dd") });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar la asistencia para el grupo {GrupoId}", modelo.Grupo?.Id);
                TempData["Error"] = "Error al guardar la asistencia. Intente nuevamente.";
                
                // Recargar los datos necesarios para la vista
                await RecargarDatosPaseLista(modelo);
                
                return View("PaseLista", modelo);
            }
        }

        /// <summary>
        /// Muestra el resumen de asistencia guardada
        /// </summary>
        /// <param name="grupoId">ID del grupo</param>
        /// <param name="fecha">Fecha del pase de lista</param>
        /// <returns></returns>
        [HttpGet]
        [RequirePermission("asistencia.ver_resumen")]
        public async Task<IActionResult> Resumen(int grupoId, string fecha)
        {
            try
            {
                if (!DateTime.TryParse(fecha, out var fechaParsed))
                {
                    fechaParsed = DateTime.Now.Date;
                }

                // Cargar grupo desde la base de datos
                var grupo = await _context.GruposEstudiantes
                    .Include(g => g.PeriodoAcademico)
                    .FirstOrDefaultAsync(g => g.GrupoId == grupoId);

                if (grupo == null)
                {
                    TempData["Error"] = "Grupo no encontrado.";
                    return RedirectToAction(nameof(Index));
                }

                // Cargar asistencias del día
                var asistencias = await _context.Asistencias
                    .Include(a => a.Estudiante)
                    .Where(a => a.GrupoId == grupoId && a.Fecha.Date == fechaParsed.Date)
                    .OrderBy(a => a.Estudiante.Apellidos)
                    .ThenBy(a => a.Estudiante.Nombre)
                    .ToListAsync();

                // Calcular estadísticas
                var totalEstudiantes = asistencias.Count;
                var presentes = asistencias.Count(a => a.Estado == "P");
                var ausentes = asistencias.Count(a => a.Estado == "A");
                var tardanzas = asistencias.Count(a => a.Estado == "T");
                var ausenciasJustificadas = asistencias.Count(a => a.Estado == "AJ");
                var porcentajeAsistencia = totalEstudiantes > 0 ? Math.Round((double)presentes / totalEstudiantes * 100, 2) : 0;

                var resumen = new ResumenAsistenciaViewModel
                {
                    Grupo = new GrupoEstudiantesViewModel
                    {
                        Id = grupo.GrupoId,
                        Nombre = grupo.Nombre,
                        Codigo = grupo.Codigo,
                        Descripcion = grupo.Descripcion,
                        PeriodoAcademico = grupo.PeriodoAcademico?.Nombre
                    },
                    Fecha = fechaParsed,
                    TotalEstudiantes = totalEstudiantes,
                    EstudiantesPresentes = presentes,
                    EstudiantesAusentes = ausentes,
                    EstudiantesTardanza = tardanzas,
                    AusenciasJustificadas = ausenciasJustificadas,
                    PorcentajeAsistencia = porcentajeAsistencia,
                    DetalleEstudiantes = asistencias.Select(a => new EstudianteAsistenciaViewModel
                    {
                        EstudianteId = a.EstudianteId,
                        NombreCompleto = $"{a.Estudiante.Apellidos}, {a.Estudiante.Nombre}",
                        NumeroId = a.Estudiante.NumeroId,
                        Estado = a.Estado,
                        Justificacion = a.Justificacion
                    }).ToList()
                };

                return View(resumen);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar el resumen de asistencia");
                TempData["Error"] = "Error al cargar el resumen. Intente nuevamente.";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Imprimir resumen de asistencia en PDF usando Rotativa
        /// </summary>
        /// <param name="grupoId">ID del grupo</param>
        /// <param name="fecha">Fecha del pase de lista</param>
        /// <returns></returns>
        [HttpGet]
        [RequirePermission("asistencia.imprimir")]
        public async Task<IActionResult> ImprimirResumen(int grupoId, string fecha)
        {
            try
            {
                if (!DateTime.TryParse(fecha, out var fechaParsed))
                {
                    fechaParsed = DateTime.Now.Date;
                }

                // Cargar grupo desde la base de datos
                var grupo = await _context.GruposEstudiantes
                    .Include(g => g.PeriodoAcademico)
                    .FirstOrDefaultAsync(g => g.GrupoId == grupoId);

                if (grupo == null)
                {
                    TempData["Error"] = "Grupo no encontrado.";
                    return RedirectToAction(nameof(Index));
                }

                // Cargar asistencias del día
                var asistencias = await _context.Asistencias
                    .Include(a => a.Estudiante)
                    .Where(a => a.GrupoId == grupoId && a.Fecha.Date == fechaParsed.Date)
                    .OrderBy(a => a.Estudiante.Apellidos)
                    .ThenBy(a => a.Estudiante.Nombre)
                    .ToListAsync();

                // Calcular estadísticas
                var totalEstudiantes = asistencias.Count;
                var presentes = asistencias.Count(a => a.Estado == "P");
                var ausentes = asistencias.Count(a => a.Estado == "A");
                var tardanzas = asistencias.Count(a => a.Estado == "T");
                var ausenciasJustificadas = asistencias.Count(a => a.Estado == "AJ");
                var porcentajeAsistencia = totalEstudiantes > 0 ? Math.Round((double)presentes / totalEstudiantes * 100, 2) : 0;

                var resumen = new ResumenAsistenciaViewModel
                {
                    Grupo = new GrupoEstudiantesViewModel
                    {
                        Id = grupo.GrupoId,
                        Nombre = grupo.Nombre,
                        Codigo = grupo.Codigo,
                        Descripcion = grupo.Descripcion,
                        PeriodoAcademico = grupo.PeriodoAcademico?.Nombre
                    },
                    Fecha = fechaParsed,
                    TotalEstudiantes = totalEstudiantes,
                    EstudiantesPresentes = presentes,
                    EstudiantesAusentes = ausentes,
                    EstudiantesTardanza = tardanzas,
                    AusenciasJustificadas = ausenciasJustificadas,
                    PorcentajeAsistencia = porcentajeAsistencia,
                    DetalleEstudiantes = asistencias.Select(a => new EstudianteAsistenciaViewModel
                    {
                        EstudianteId = a.EstudianteId,
                        NombreCompleto = $"{a.Estudiante.Apellidos}, {a.Estudiante.Nombre}",
                        NumeroId = a.Estudiante.NumeroId,
                        Estado = a.Estado,
                        Justificacion = a.Justificacion
                    }).ToList()
                };

                // TODO: Migrar a QuestPDF - temporalmente deshabilitado
                // Generar HTML de la vista
                // var htmlContent = await RenderViewToStringAsync("ResumenPdf", resumen);

                // Generar PDF usando QuestPDF (pendiente implementación)
                // var renderer = new ChromePdfRenderer();
                // var pdfDocument = renderer.RenderHtmlAsPdf(htmlContent);
                // var pdfBytes = pdfDocument.BinaryData;

                // var fileName = $"Resumen_Asistencia_{grupo.Nombre}_{fechaParsed:yyyy-MM-dd}.pdf";
                // return File(pdfBytes, "application/pdf", fileName);
                
                // Retornar error temporal hasta migrar a QuestPDF
                return BadRequest("La generación de PDF de asistencia está siendo migrada a QuestPDF. Use el servicio de curriculum como referencia.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar PDF del resumen de asistencia para grupo {GrupoId}, fecha {Fecha}", grupoId, fecha);
                TempData["Error"] = "Error al generar el PDF. Intente nuevamente.";
                return RedirectToAction(nameof(Resumen), new { grupoId, fecha });
            }
        }

        /// <summary>
        /// Exportar asistencia a Excel
        /// </summary>
        /// <param name="grupoId">ID del grupo</param>
        /// <param name="fecha">Fecha del pase</param>
        /// <returns></returns>
        [HttpGet]
        [RequirePermission("asistencia.exportar")]
        public IActionResult ExportarExcel(int grupoId, string fecha)
        {
            try
            {
                // Aquí implementarías la lógica de exportación a Excel
                // usando librerías como EPPlus o ClosedXML
                
                TempData["Info"] = "La exportación a Excel estará disponible próximamente.";
                return RedirectToAction(nameof(Resumen), new { grupoId, fecha });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al exportar asistencia a Excel");
                TempData["Error"] = "Error al exportar. Intente nuevamente.";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Renderiza una vista a string para generar PDFs
        /// </summary>
        private async Task<string> RenderViewToStringAsync(string viewName, object model)
        {
            // Usar una aproximación más simple: generar HTML básico para el PDF
            var htmlBuilder = new StringBuilder();
            
            htmlBuilder.AppendLine("<!DOCTYPE html>");
            htmlBuilder.AppendLine("<html><head>");
            htmlBuilder.AppendLine("<meta charset='utf-8'>");
            htmlBuilder.AppendLine("<title>Resumen de Asistencia</title>");
            htmlBuilder.AppendLine("<style>");
            htmlBuilder.AppendLine("body { font-family: Arial, sans-serif; margin: 20px; }");
            htmlBuilder.AppendLine(".header { text-align: center; margin-bottom: 30px; }");
            htmlBuilder.AppendLine(".stats { display: flex; justify-content: space-around; margin: 20px 0; }");
            htmlBuilder.AppendLine(".stat-card { border: 1px solid #ddd; padding: 15px; border-radius: 5px; text-align: center; }");
            htmlBuilder.AppendLine("table { width: 100%; border-collapse: collapse; margin-top: 20px; }");
            htmlBuilder.AppendLine("th, td { border: 1px solid #ddd; padding: 8px; text-align: left; }");
            htmlBuilder.AppendLine("th { background-color: #f2f2f2; }");
            htmlBuilder.AppendLine("</style>");
            htmlBuilder.AppendLine("</head><body>");
            
            if (model is ResumenAsistenciaViewModel resumen)
            {
                htmlBuilder.AppendLine("<div class='header'>");
                htmlBuilder.AppendLine($"<h1>Resumen de Asistencia</h1>");
                htmlBuilder.AppendLine($"<h2>{resumen.Grupo.Nombre}</h2>");
                htmlBuilder.AppendLine($"<p>Período: {resumen.Grupo.PeriodoAcademico}</p>");
                htmlBuilder.AppendLine($"<p>Fecha: {resumen.Fecha:dd/MM/yyyy}</p>");
                htmlBuilder.AppendLine("</div>");
                
                htmlBuilder.AppendLine("<div class='stats'>");
                htmlBuilder.AppendLine($"<div class='stat-card'><h3>{resumen.TotalEstudiantes}</h3><p>Total Estudiantes</p></div>");
                htmlBuilder.AppendLine($"<div class='stat-card'><h3>{resumen.EstudiantesPresentes}</h3><p>Presentes</p></div>");
                htmlBuilder.AppendLine($"<div class='stat-card'><h3>{resumen.EstudiantesAusentes}</h3><p>Ausentes</p></div>");
                htmlBuilder.AppendLine($"<div class='stat-card'><h3>{resumen.PorcentajeAsistencia:F1}%</h3><p>Asistencia</p></div>");
                htmlBuilder.AppendLine("</div>");
                
                htmlBuilder.AppendLine("<table>");
                htmlBuilder.AppendLine("<thead><tr><th>Estudiante</th><th>Número ID</th><th>Estado</th><th>Justificación</th></tr></thead>");
                htmlBuilder.AppendLine("<tbody>");
                
                foreach (var estudiante in resumen.DetalleEstudiantes)
                {
                    var estadoTexto = estudiante.Estado switch
                    {
                        "P" => "Presente",
                        "A" => "Ausente", 
                        "T" => "Tardanza",
                        "AJ" => "Ausencia Justificada",
                        _ => estudiante.Estado
                    };
                    
                    htmlBuilder.AppendLine($"<tr><td>{estudiante.NombreCompleto}</td><td>{estudiante.NumeroId}</td><td>{estadoTexto}</td><td>{estudiante.Justificacion ?? ""}</td></tr>");
                }
                
                htmlBuilder.AppendLine("</tbody></table>");
            }
            
            htmlBuilder.AppendLine("</body></html>");
            
            return htmlBuilder.ToString();
        }

        /// <summary>
        /// Recarga los datos necesarios para la vista PaseLista cuando hay errores de validación
        /// </summary>
        /// <param name="modelo">Modelo a recargar con datos</param>
        private async Task RecargarDatosPaseLista(PaseListaViewModel modelo)
        {
            try
            {
                // Asegurar que el modelo tenga un objeto Grupo válido
                if (modelo.Grupo == null)
                {
                    modelo.Grupo = new GrupoEstudiantesViewModel();
                }

                // Recargar información del grupo desde la base de datos
                if (modelo.Grupo.Id > 0)
                {
                    var grupo = await _context.GruposEstudiantes
                        .Include(g => g.PeriodoAcademico)
                        .FirstOrDefaultAsync(g => g.GrupoId == modelo.Grupo.Id);

                    if (grupo != null)
                    {
                        modelo.Grupo.Nombre = grupo.Nombre;
                        modelo.Grupo.Codigo = grupo.Codigo;
                        modelo.Grupo.Descripcion = grupo.Descripcion;
                        modelo.Grupo.PeriodoAcademico = grupo.PeriodoAcademico?.Nombre;
                    }
                }
                else
                {
                    // Si no hay ID de grupo, intentar extraerlo de la URL o de otro contexto
                    _logger.LogWarning("El modelo no contiene ID de grupo válido. Intentando recuperar de contexto.");
                    
                    // Intentar obtener el grupoId desde TempData, ViewBag o Request
                    if (int.TryParse(Request.Query["grupoId"].ToString(), out var grupoIdFromQuery))
                    {
                        modelo.Grupo.Id = grupoIdFromQuery;
                        
                        var grupo = await _context.GruposEstudiantes
                            .Include(g => g.PeriodoAcademico)
                            .FirstOrDefaultAsync(g => g.GrupoId == grupoIdFromQuery);

                        if (grupo != null)
                        {
                            modelo.Grupo.Nombre = grupo.Nombre;
                            modelo.Grupo.Codigo = grupo.Codigo;
                            modelo.Grupo.Descripcion = grupo.Descripcion;
                            modelo.Grupo.PeriodoAcademico = grupo.PeriodoAcademico?.Nombre;
                        }
                    }
                }

                // Recargar lista de materias para el dropdown
                ViewBag.Materias = await _context.Materias
                    .Select(m => new SelectListItem
                    {
                        Value = m.MateriaId.ToString(),
                        Text = m.Nombre,
                        Selected = m.MateriaId == modelo.MateriaId
                    })
                    .ToListAsync();

                // Recargar lista de estudiantes si no existe o está incompleta
                if (modelo.Grupo?.Id > 0)
                {
                    // Verificar si necesitamos recargar estudiantes
                    bool necesitaRecargarEstudiantes = modelo.Estudiantes == null || 
                                                       !modelo.Estudiantes.Any() || 
                                                       modelo.Estudiantes.Any(e => string.IsNullOrEmpty(e.NombreCompleto));

                    if (necesitaRecargarEstudiantes)
                    {
                        var grupo = await _context.GruposEstudiantes
                            .Include(g => g.EstudianteGrupos.Where(eg => eg.Estado == Models.EstadoAsignacion.Activo))
                                .ThenInclude(eg => eg.Estudiante)
                            .FirstOrDefaultAsync(g => g.GrupoId == modelo.Grupo.Id);

                        if (grupo != null)
                        {
                            var estudiantesCompletos = grupo.EstudianteGrupos
                                .Where(eg => eg.Estado == Models.EstadoAsignacion.Activo && eg.Estudiante != null)
                                .OrderBy(eg => eg.Estudiante!.Apellidos)
                                .ThenBy(eg => eg.Estudiante!.Nombre)
                                .Select(eg => new EstudianteAsistenciaViewModel
                                {
                                    EstudianteId = eg.Estudiante!.IdEstudiante,
                                    NombreCompleto = $"{eg.Estudiante.Apellidos}, {eg.Estudiante.Nombre}",
                                    NumeroId = eg.Estudiante.NumeroId,
                                    Estado = "P" // Por defecto presente si no hay datos previos
                                })
                                .ToList();

                            // Mantener los estados de asistencia ya seleccionados si existen
                            if (modelo.Estudiantes?.Any() == true)
                            {
                                foreach (var estudianteCompleto in estudiantesCompletos)
                                {
                                    var estudianteExistente = modelo.Estudiantes.FirstOrDefault(e => e.EstudianteId == estudianteCompleto.EstudianteId);
                                    if (estudianteExistente != null)
                                    {
                                        estudianteCompleto.Estado = estudianteExistente.Estado;
                                        estudianteCompleto.Justificacion = estudianteExistente.Justificacion;
                                        estudianteCompleto.AsistenciaId = estudianteExistente.AsistenciaId;
                                    }
                                }
                            }

                            modelo.Estudiantes = estudiantesCompletos;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al recargar datos para PaseLista");
                // En caso de error, mantener los datos que ya existen en el modelo
            }
        }
    }
}