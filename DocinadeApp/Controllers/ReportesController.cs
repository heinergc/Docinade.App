using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RubricasApp.Web.Services.Reportes;
using RubricasApp.Web.ViewModels.Reportes;
using RubricasApp.Web.ViewModels.Conducta;
using RubricasApp.Web.Services;
using RubricasApp.Web.Data;
using RubricasApp.Web.Interfaces;
using RubricasApp.Web.Models;

namespace RubricasApp.Web.Controllers
{
    [Authorize]
    public class ReportesController : BaseController
    {
        private readonly IReporteService _reporteService;
        private readonly IConductaService _conductaService;
        private readonly ILogger<ReportesController> _logger;

        public ReportesController(
            RubricasDbContext context,
            IPeriodoAcademicoService periodoService,
            IReporteService reporteService,
            IConductaService conductaService,
            ILogger<ReportesController> logger) 
            : base(periodoService, context)
        {
            _reporteService = reporteService;
            _conductaService = conductaService;
            _logger = logger;
        }

        // GET: Reportes
        public IActionResult Index()
        {
            return View();
        }

        // GET: Reportes/ArbolEvaluaciones
        public async Task<IActionResult> ArbolEvaluaciones(
            int? materiaId = null,
            int? periodoId = null,
            bool incluirEstadisticas = true,
            bool soloActivos = true,
            bool incluirBorradores = false,
            string? estadoEvaluacion = null)
        {
            var filtros = new FiltrosReporte
            {
                MateriaId = materiaId,
                PeriodoId = periodoId,
                IncluirEstadisticas = incluirEstadisticas,
                SoloActivos = soloActivos,
                IncluirBorradores = incluirBorradores,
                EstadoEvaluacion = estadoEvaluacion
            };

            try
            {
                var reporte = await _reporteService.GenerarReporteArbolAsync(filtros);
                
                // Configurar ViewBag para filtros
                await CargarOpcionesFiltros();
                ViewBag.MateriaSeleccionada = materiaId;
                ViewBag.PeriodoSeleccionado = periodoId;
                ViewBag.SoloActivos = soloActivos;
                ViewBag.IncluirBorradores = incluirBorradores;
                ViewBag.EstadoEvaluacion = estadoEvaluacion;

                return View(reporte);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al generar el reporte: {ex.Message}";
                return View(new ReporteArbolViewModel());
            }
        }

        // GET: Reportes/ArbolEvaluaciones/API - Para uso con JavaScript
        [HttpGet]
        public async Task<JsonResult> GetArbolEvaluaciones(
            int? materiaId = null,
            int? periodoId = null,
            bool incluirEstadisticas = true,
            bool soloActivos = true)
        {
            try
            {
                var filtros = new FiltrosReporte
                {
                    MateriaId = materiaId,
                    PeriodoId = periodoId,
                    IncluirEstadisticas = incluirEstadisticas,
                    SoloActivos = soloActivos
                };

                var reporte = await _reporteService.GenerarReporteArbolAsync(filtros);
                
                // Configurar Content-Type expl�citamente para UTF-8
                Response.ContentType = "application/json; charset=utf-8";
                
                return Json(new
                {
                    success = true,
                    data = reporte,
                    metadata = new
                    {
                        fechaGeneracion = DateTime.Now,
                        filtrosAplicados = filtros,
                        totalNodos = reporte.TotalNodos
                    }
                }, new System.Text.Json.JsonSerializerOptions
                {
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                    PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
                });
            }
            catch (Exception ex)
            {
                Response.ContentType = "application/json; charset=utf-8";
                return Json(new
                {
                    success = false,
                    message = ex.Message,
                    error = "Error al generar el reporte en �rbol"
                }, new System.Text.Json.JsonSerializerOptions
                {
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                });
            }
        }

        // GET: Reportes/ExportarArbolExcel
        public async Task<IActionResult> ExportarArbolExcel(
            int? materiaId = null,
            int? periodoId = null,
            bool soloActivos = true,
            bool incluirBorradores = false)
        {
            try
            {
                var filtros = new FiltrosReporte
                {
                    MateriaId = materiaId,
                    PeriodoId = periodoId,
                    SoloActivos = soloActivos,
                    IncluirBorradores = incluirBorradores
                };

                var excelBytes = await _reporteService.ExportarReporteAExcelAsync(filtros);

                var fileName = $"ReporteArbolEvaluaciones_{DateTime.Now:yyyyMMdd_HHmm}.xlsx";
                
                if (materiaId.HasValue)
                {
                    var materia = await _context.Materias.FindAsync(materiaId.Value);
                    if (materia != null)
                    {
                        fileName = $"ReporteArbol_{materia.Codigo}_{DateTime.Now:yyyyMMdd_HHmm}.xlsx";
                    }
                }

                return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al exportar Excel: {ex.Message}";
                return RedirectToAction(nameof(ArbolEvaluaciones), new { materiaId, periodoId });
            }
        }

        // GET: Reportes/ExportarArbolCSV
        public async Task<IActionResult> ExportarArbolCSV(
            int? materiaId = null,
            int? periodoId = null,
            bool soloActivos = true,
            bool incluirBorradores = false)
        {
            try
            {
                var filtros = new FiltrosReporte
                {
                    MateriaId = materiaId,
                    PeriodoId = periodoId,
                    SoloActivos = soloActivos,
                    IncluirBorradores = incluirBorradores
                };

                var csvBytes = await _reporteService.ExportarReporteACSVAsync(filtros);

                var fileName = $"ReporteArbolEvaluaciones_{DateTime.Now:yyyyMMdd_HHmm}.csv";
                
                if (materiaId.HasValue)
                {
                    var materia = await _context.Materias.FindAsync(materiaId.Value);
                    if (materia != null)
                    {
                        fileName = $"ReporteArbol_{materia.Codigo}_{DateTime.Now:yyyyMMdd_HHmm}.csv";
                    }
                }

                return File(csvBytes, "text/csv", fileName);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al exportar CSV: {ex.Message}";
                return RedirectToAction(nameof(ArbolEvaluaciones), new { materiaId, periodoId });
            }
        }

        // GET: Reportes/EstadisticasRapidas
        [HttpGet]
        public async Task<JsonResult> EstadisticasRapidas()
        {
            try
            {
                var estadisticas = await _reporteService.ObtenerEstadisticasRapidasAsync();
                
                // Configurar Content-Type expl�citamente para UTF-8
                Response.ContentType = "application/json; charset=utf-8";
                
                return Json(new
                {
                    success = true,
                    data = estadisticas,
                    timestamp = DateTime.Now
                }, new System.Text.Json.JsonSerializerOptions
                {
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                    PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
                });
            }
            catch (Exception ex)
            {
                Response.ContentType = "application/json; charset=utf-8";
                return Json(new
                {
                    success = false,
                    message = ex.Message
                }, new System.Text.Json.JsonSerializerOptions
                {
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                });
            }
        }

        // GET: Reportes/Dashboard
        public async Task<IActionResult> Dashboard()
        {
            try
            {
                var estadisticas = await _reporteService.ObtenerEstadisticasRapidasAsync();
                return View(estadisticas);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al cargar el dashboard: {ex.Message}";
                return View(new Dictionary<string, object>());
            }
        }

        private async Task CargarOpcionesFiltros()
        {
            // Materias activas
            var materias = await _context.Materias
                .Where(m => m.Activa)
                .OrderBy(m => m.Codigo)
                .ToListAsync();

            ViewBag.MateriasDisponibles = materias.Select(m => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Value = m.MateriaId.ToString(),
                Text = $"[{m.Codigo}] {m.Nombre}"
            }).ToList();

            // Periodos activos
            var periodos = await _context.PeriodosAcademicos
                .Where(p => p.Activo)
                .OrderByDescending(p => p.Anio).ThenByDescending(p => p.NumeroPeriodo)
                .ToListAsync();

            ViewBag.PeriodosDisponibles = periodos.Select(p => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Value = p.Id.ToString(),
                Text = $"{p.Nombre} ({p.Anio})"
            }).ToList();

            // Estados de evaluacion - sin caracteres especiales
            ViewBag.EstadosEvaluacion = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>
            {
                new() { Value = "", Text = "-- Todos los estados --" },
                new() { Value = "FINALIZADA", Text = "Finalizadas" },
                new() { Value = "BORRADOR", Text = "Borradores" },
                new() { Value = "TODAS", Text = "Todas" }
            };
        }

        #region Reportes de Conducta

        // GET: Reportes/SeleccionarEstudiante
        public async Task<IActionResult> SeleccionarEstudiante()
        {
            try
            {
                var estudiantes = await _context.Estudiantes
                    .Where(e => e.Estado == 1) // 1 = Activo
                    .OrderBy(e => e.Apellidos)
                    .ThenBy(e => e.Nombre)
                    .Select(e => new
                    {
                        e.IdEstudiante,
                        NombreCompleto = e.Nombre + " " + e.Apellidos,
                        e.NumeroId,
                        e.Grupos
                    })
                    .ToListAsync();

                ViewBag.Estudiantes = estudiantes.Select(e => new SelectListItem
                {
                    Value = e.IdEstudiante.ToString(),
                    Text = $"{e.NombreCompleto} - {e.NumeroId} ({e.Grupos ?? "Sin grupo"})"
                }).ToList();

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar selector de estudiantes");
                TempData["Error"] = "Error al cargar la lista de estudiantes.";
                return RedirectToAction("Dashboard", "NotaConducta");
            }
        }

        // GET: Reportes/HistorialConductaEstudiante
        public async Task<IActionResult> HistorialConductaEstudiante(int idEstudiante)
        {
            try
            {
                var estudiante = await _context.Estudiantes
                    .FirstOrDefaultAsync(e => e.IdEstudiante == idEstudiante);

                if (estudiante == null)
                {
                    TempData["Error"] = "Estudiante no encontrado.";
                    return RedirectToAction("Dashboard", "NotaConducta");
                }

                // Obtener el profesor guía actual del estudiante
                string profesorGuiaActual = "Sin asignar";
                var grupoEstudiante = await _context.EstudianteGrupos
                    .Include(eg => eg.Grupo)
                    .Where(eg => eg.EstudianteId == idEstudiante && eg.Estado == EstadoAsignacion.Activo)
                    .OrderByDescending(eg => eg.FechaAsignacion)
                    .FirstOrDefaultAsync();

                if (grupoEstudiante?.Grupo != null)
                {
                    var profesorGuia = await _context.ProfesorGuia
                        .Include(pg => pg.Profesor)
                        .Where(pg => pg.GrupoId == grupoEstudiante.GrupoId && pg.Estado)
                        .FirstOrDefaultAsync();

                    if (profesorGuia?.Profesor != null)
                    {
                        var profesor = profesorGuia.Profesor;
                        profesorGuiaActual = $"{profesor.Nombres} {profesor.PrimerApellido} {profesor.SegundoApellido}".Trim();
                    }
                }

                // Obtener todas las notas de conducta por período
                var notasConducta = await _context.NotasConducta
                    .Include(n => n.Periodo)
                    .Include(n => n.ProgramaAcciones)
                    .Include(n => n.DecisionProfesional)
                    .Where(n => n.IdEstudiante == idEstudiante)
                    .OrderByDescending(n => n.Periodo.Anio)
                        .ThenByDescending(n => n.Periodo.NumeroPeriodo)
                    .ToListAsync();

                // Obtener todas las boletas del estudiante
                var boletasEstudiante = await _context.BoletasConducta
                    .Include(b => b.TipoFalta)
                    .Include(b => b.DocenteEmisor)
                    .Where(b => b.IdEstudiante == idEstudiante)
                    .ToListAsync();

                // Mapear a ViewModel
                var modelo = new HistorialConductaEstudianteViewModel
                {
                    IdEstudiante = estudiante.IdEstudiante,
                    NombreEstudiante = estudiante.NombreCompleto,
                    NumeroIdentificacion = estudiante.NumeroId,
                    GrupoActual = estudiante.Grupos ?? "Sin grupo",
                    ProfesorGuiaActual = profesorGuiaActual,
                    TotalBoletasHistorico = boletasEstudiante.Count,
                    TotalRebajosHistorico = notasConducta.Sum(n => n.TotalRebajos),
                    PromedioHistorico = notasConducta.Any() ? notasConducta.Average(n => n.NotaFinal) : 100,
                    NotasPorPeriodo = notasConducta.Select(n => new NotaConductaPorPeriodoViewModel
                    {
                        IdPeriodo = n.IdPeriodo,
                        NombrePeriodo = n.Periodo.Nombre,
                        NotaFinal = n.NotaFinal,
                        TotalRebajos = n.TotalRebajos,
                        CantidadBoletas = boletasEstudiante.Count(b => b.IdPeriodo == n.IdPeriodo),
                        Estado = n.Estado,
                        TienePrograma = n.IdProgramaAcciones.HasValue,
                        EstadoPrograma = n.ProgramaAcciones?.Estado,
                        TieneDecision = n.IdDecisionProfesional.HasValue,
                        DecisionAplicada = n.DecisionProfesional?.DecisionTomada,
                        Boletas = boletasEstudiante
                            .Where(b => b.IdPeriodo == n.IdPeriodo)
                            .Select(b => new BoletaResumenViewModel
                            {
                                IdBoleta = b.IdBoleta,
                                FechaEmision = b.FechaEmision,
                                TipoFalta = b.TipoFalta.Definicion,
                                Rebajo = b.RebajoAplicado,
                                EmitidaPor = b.DocenteEmisor?.NombreCompleto ?? "Desconocido",
                                Estado = b.Estado
                            }).ToList()
                    }).ToList()
                };

                return View(modelo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar historial de conducta del estudiante {IdEstudiante}", idEstudiante);
                TempData["Error"] = "Error al generar el historial de conducta.";
                return RedirectToAction("Dashboard", "NotaConducta");
            }
        }

        // GET: Reportes/ReporteGeneralConducta
        public async Task<IActionResult> ReporteGeneralConducta(int? idPeriodo, int? idNivel)
        {
            try
            {
                // Si no se especifica período, usar el activo
                if (!idPeriodo.HasValue)
                {
                    var periodoActivo = await _context.PeriodosAcademicos
                        .Where(p => p.Activo)
                        .OrderByDescending(p => p.Anio)
                        .ThenByDescending(p => p.NumeroPeriodo)
                        .FirstOrDefaultAsync();

                    idPeriodo = periodoActivo?.Id;
                }

                if (!idPeriodo.HasValue)
                {
                    TempData["Error"] = "No hay períodos académicos disponibles.";
                    return RedirectToAction("Dashboard", "NotaConducta");
                }

                var periodo = await _context.PeriodosAcademicos.FindAsync(idPeriodo.Value);

                // Obtener todas las notas de conducta del período
                var notasConducta = await _context.NotasConducta
                    .Include(n => n.Estudiante)
                    .Include(n => n.ProgramaAcciones)
                    .Include(n => n.DecisionProfesional)
                    .Where(n => n.IdPeriodo == idPeriodo.Value)
                    .ToListAsync();

                // Obtener todas las boletas del período
                var boletasPeriodo = await _context.BoletasConducta
                    .Include(b => b.TipoFalta)
                    .Where(b => b.IdPeriodo == idPeriodo.Value && b.Estado == "Activa")
                    .ToListAsync();

                // Calcular estadísticas globales
                var totalEstudiantes = notasConducta.Count;
                var aprobados = notasConducta.Count(n => n.Estado == "Aprobado");
                var enRiesgo = notasConducta.Count(n => n.Estado == "Riesgo");
                var aplazados = notasConducta.Count(n => n.Estado == "Aplazado");
                var totalBoletas = boletasPeriodo.Count;
                var promedioGeneral = notasConducta.Any() ? notasConducta.Average(n => n.NotaFinal) : 100;

                // Boletas por tipo de falta
                var boletasPorTipo = boletasPeriodo
                    .GroupBy(b => b.TipoFalta.Nombre)
                    .Select(g => new BoletasPorTipoViewModel
                    {
                        TipoFalta = g.Key,
                        Cantidad = g.Count()
                    })
                    .OrderByDescending(b => b.Cantidad)
                    .ToList();

                // Top 10 mejor conducta
                var top10Mejor = notasConducta
                    .OrderByDescending(n => n.NotaFinal)
                    .Take(10)
                    .Select(n => new EstudianteTopViewModel
                    {
                        NombreEstudiante = n.Estudiante.NombreCompleto,
                        Grupo = n.Estudiante.Grupos ?? "Sin grupo",
                        Nota = n.NotaFinal
                    })
                    .ToList();

                // Estudiantes requieren atención urgente
                var requierenAtencion = notasConducta
                    .Where(n => n.Estado == "Aplazado" && !n.IdProgramaAcciones.HasValue && !n.IdDecisionProfesional.HasValue)
                    .OrderBy(n => n.NotaFinal)
                    .Take(20)
                    .Select(n => new EstudianteAtencionViewModel
                    {
                        NombreEstudiante = n.Estudiante.NombreCompleto,
                        Grupo = n.Estudiante.Grupos ?? "Sin grupo",
                        Nota = n.NotaFinal,
                        CantidadBoletas = boletasPeriodo.Count(b => b.IdEstudiante == n.IdEstudiante),
                        TienePrograma = n.IdProgramaAcciones.HasValue,
                        TieneDecision = n.IdDecisionProfesional.HasValue
                    })
                    .ToList();

                // Calcular estadísticas por grupo
                var estadisticasPorGrupo = await _context.GruposEstudiantes
                    .Include(g => g.TipoGrupoCatalogo)
                    .Include(g => g.EstudianteGrupos)
                        .ThenInclude(eg => eg.Estudiante)
                    .Where(g => g.PeriodoAcademicoId == idPeriodo.Value && 
                                g.Estado == EstadoGrupo.Activo)
                    .ToListAsync();

                // Obtener profesores guías relacionados con los grupos
                var gruposIds = estadisticasPorGrupo.Select(g => g.GrupoId).ToList();
                var profesoresGuiasRel = await _context.Set<ProfesorGuia>()
                    .Include(pg => pg.Profesor)
                    .Where(pg => gruposIds.Contains(pg.GrupoId) && pg.Estado)
                    .ToListAsync();

                var estadisticasGrupo = estadisticasPorGrupo.Select(grupo =>
                {
                    var estudiantesGrupo = grupo.EstudianteGrupos
                        .Where(eg => eg.Estado == EstadoAsignacion.Activo)
                        .Select(eg => eg.EstudianteId)
                        .ToList();
                    
                    var notasGrupo = notasConducta
                        .Where(n => estudiantesGrupo.Contains(n.IdEstudiante))
                        .ToList();
                    
                    var boletasGrupo = boletasPeriodo
                        .Where(b => estudiantesGrupo.Contains(b.IdEstudiante))
                        .Count();

                    // Buscar profesor guía
                    var profesorGuiaRel = profesoresGuiasRel
                        .FirstOrDefault(pg => pg.GrupoId == grupo.GrupoId);
                    
                    var nombreProfesorGuia = profesorGuiaRel != null && profesorGuiaRel.Profesor != null
                        ? $"{profesorGuiaRel.Profesor.Nombres} {profesorGuiaRel.Profesor.PrimerApellido}".Trim()
                        : "Sin asignar";

                    return new EstadisticasGrupoConductaViewModel
                    {
                        NombreGrupo = grupo.Nombre,
                        ProfesorGuia = nombreProfesorGuia,
                        TotalEstudiantes = notasGrupo.Count,
                        Aprobados = notasGrupo.Count(n => n.Estado == "Aprobado"),
                        EnRiesgo = notasGrupo.Count(n => n.Estado == "Riesgo"),
                        Aplazados = notasGrupo.Count(n => n.Estado == "Aplazado"),
                        PromedioGrupo = notasGrupo.Any() ? notasGrupo.Average(n => n.NotaFinal) : 100,
                        TotalBoletas = boletasGrupo
                    };
                }).Where(e => e.TotalEstudiantes > 0).ToList();

                // Crear el modelo del reporte
                var modelo = new ReporteGeneralConductaViewModel
                {
                    IdPeriodo = idPeriodo.Value,
                    NombrePeriodo = periodo!.Nombre,
                    IdNivel = idNivel,
                    NombreNivel = null, // Simplificado por ahora
                    TotalEstudiantes = totalEstudiantes,
                    EstudiantesAprobados = aprobados,
                    EstudiantesEnRiesgo = enRiesgo,
                    EstudiantesAplazados = aplazados,
                    PorcentajeAprobados = totalEstudiantes > 0 ? (decimal)aprobados / totalEstudiantes * 100 : 0,
                    PorcentajeEnRiesgo = totalEstudiantes > 0 ? (decimal)enRiesgo / totalEstudiantes * 100 : 0,
                    PorcentajeAplazados = totalEstudiantes > 0 ? (decimal)aplazados / totalEstudiantes * 100 : 0,
                    TotalBoletas = totalBoletas,
                    PromedioGeneral = promedioGeneral,
                    EstadisticasPorGrupo = estadisticasGrupo,
                    BoletasPorTipo = boletasPorTipo,
                    Top10MejorConducta = top10Mejor,
                    EstudiantesAtencionUrgente = requierenAtencion
                };

                // Cargar filtros
                await CargarFiltrosConducta(idPeriodo, idNivel);

                return View(modelo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar reporte general de conducta");
                TempData["Error"] = "Error al generar el reporte general de conducta.";
                return RedirectToAction("Dashboard", "NotaConducta");
            }
        }

        // GET: Reportes/ExportarHistorialPDF
        public async Task<IActionResult> ExportarHistorialPDF(int idEstudiante)
        {
            try
            {
                // Por ahora, devolver CSV en lugar de PDF
                // Implementar generación de PDF requiere una librería como iTextSharp o Rotativa
                TempData["Info"] = "Exportación a PDF estará disponible próximamente. Se exportó como CSV.";
                return await ExportarHistorialCSV(idEstudiante);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al exportar historial PDF del estudiante {IdEstudiante}", idEstudiante);
                TempData["Error"] = "Error al exportar el historial.";
                return RedirectToAction(nameof(HistorialConductaEstudiante), new { idEstudiante });
            }
        }

        // GET: Reportes/ExportarHistorialCSV
        private async Task<IActionResult> ExportarHistorialCSV(int idEstudiante)
        {
            try
            {
                var estudiante = await _context.Estudiantes.FindAsync(idEstudiante);
                if (estudiante == null)
                {
                    TempData["Error"] = "Estudiante no encontrado.";
                    return RedirectToAction("Dashboard", "NotaConducta");
                }

                var notasConducta = await _context.NotasConducta
                    .Include(n => n.Periodo)
                    .Where(n => n.IdEstudiante == idEstudiante)
                    .OrderByDescending(n => n.Periodo.Anio)
                        .ThenByDescending(n => n.Periodo.NumeroPeriodo)
                    .ToListAsync();

                // Obtener todas las boletas del estudiante
                var boletasEstudiante = await _context.BoletasConducta
                    .Where(b => b.IdEstudiante == idEstudiante)
                    .ToListAsync();

                var csv = new System.Text.StringBuilder();
                csv.AppendLine($"Historial de Conducta - {estudiante.NombreCompleto}");
                csv.AppendLine($"Identificación: {estudiante.NumeroId}");
                csv.AppendLine($"Fecha: {DateTime.Now:dd/MM/yyyy HH:mm}");
                csv.AppendLine();
                csv.AppendLine("Período,Nota Final,Total Rebajos,Cantidad Boletas,Estado");

                foreach (var nota in notasConducta)
                {
                    var cantidadBoletas = boletasEstudiante.Count(b => b.IdPeriodo == nota.IdPeriodo);
                    csv.AppendLine($"\"{nota.Periodo.Nombre}\"," +
                        $"{nota.NotaFinal:F2}," +
                        $"{nota.TotalRebajos}," +
                        $"{cantidadBoletas}," +
                        $"\"{nota.Estado}\"");
                }

                var bytes = System.Text.Encoding.UTF8.GetBytes(csv.ToString());
                var fileName = $"HistorialConducta_{estudiante.NumeroId}_{DateTime.Now:yyyyMMdd}.csv";

                return File(bytes, "text/csv", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al exportar historial CSV del estudiante {IdEstudiante}", idEstudiante);
                TempData["Error"] = "Error al exportar el historial.";
                return RedirectToAction("Dashboard", "NotaConducta");
            }
        }

        // GET: Reportes/ExportarReporteGeneral
        public async Task<IActionResult> ExportarReporteGeneral(int idPeriodo, int? idNivel)
        {
            try
            {
                var periodo = await _context.PeriodosAcademicos.FindAsync(idPeriodo);
                if (periodo == null)
                {
                    TempData["Error"] = "Período no encontrado.";
                    return RedirectToAction(nameof(ReporteGeneralConducta));
                }

                // Obtener datos
                var notas = await _context.NotasConducta
                    .Include(n => n.Estudiante)
                    .Where(n => n.IdPeriodo == idPeriodo)
                    .ToListAsync();

                // Obtener boletas
                var boletasPeriodo = await _context.BoletasConducta
                    .Where(b => b.IdPeriodo == idPeriodo)
                    .ToListAsync();

                // Generar CSV
                var csv = new System.Text.StringBuilder();
                csv.AppendLine($"Reporte General de Conducta - {periodo.Nombre}");
                csv.AppendLine($"Fecha: {DateTime.Now:dd/MM/yyyy HH:mm}");
                csv.AppendLine();
                csv.AppendLine("Estudiante,Identificación,Grupo,Nota Final,Estado,Boletas,Rebajos");

                foreach (var nota in notas.OrderBy(n => n.Estudiante.NombreCompleto))
                {
                    var cantidadBoletas = boletasPeriodo.Count(b => b.IdEstudiante == nota.IdEstudiante);
                    var grupo = nota.Estudiante.Grupos ?? "Sin grupo";

                    csv.AppendLine($"\"{nota.Estudiante.NombreCompleto}\"," +
                        $"\"{nota.Estudiante.NumeroId}\"," +
                        $"\"{grupo}\"," +
                        $"{nota.NotaFinal:F2}," +
                        $"\"{nota.Estado}\"," +
                        $"{cantidadBoletas}," +
                        $"{nota.TotalRebajos}");
                }

                var bytes = System.Text.Encoding.UTF8.GetBytes(csv.ToString());
                var fileName = $"ReporteConducta_{periodo.Nombre.Replace(" ", "_")}_{DateTime.Now:yyyyMMdd}.csv";

                return File(bytes, "text/csv", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al exportar reporte general de conducta");
                TempData["Error"] = "Error al exportar el reporte.";
                return RedirectToAction(nameof(ReporteGeneralConducta), new { idPeriodo, idNivel });
            }
        }

        private async Task CargarFiltrosConducta(int? idPeriodoSeleccionado, int? idNivelSeleccionado)
        {
            // Períodos
            var periodos = await _context.PeriodosAcademicos
                .Where(p => p.Activo)
                .OrderByDescending(p => p.Anio)
                .ThenByDescending(p => p.NumeroPeriodo)
                .ToListAsync();

            ViewBag.Periodos = periodos.Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = p.Nombre,
                Selected = p.Id == idPeriodoSeleccionado
            }).ToList();

            // Niveles - Inicializar lista vacía si no hay niveles en la base de datos
            ViewBag.Niveles = new List<SelectListItem>();

            ViewBag.NombreInstitucion = "Institución Educativa"; // Cargar desde configuración
        }

        #endregion
    }
}