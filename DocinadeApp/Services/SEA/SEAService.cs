using Microsoft.EntityFrameworkCore;
using RubricasApp.Web.Data;
using RubricasApp.Web.Models;
using RubricasApp.Web.Models.SEA;
using RubricasApp.Web.Services.CuadernoCalificador;
using RubricasApp.Web.ViewModels.SEA;
using System.Text;
using System.Text.RegularExpressions;
using ClosedXML.Excel;

namespace RubricasApp.Web.Services.SEA
{
    /// <summary>
    /// Implementación del servicio de reportes SEA (Sistema de Evaluación MEP)
    /// Integra calificaciones del Cuaderno Calificador con asistencia
    /// </summary>
    public class SEAService : ISEAService
    {
        private readonly RubricasDbContext _context;
        private readonly ICuadernoCalificadorDinamicoService _cuadernoService;
        private readonly ILogger<SEAService> _logger;

        public SEAService(
            RubricasDbContext context,
            ICuadernoCalificadorDinamicoService cuadernoService,
            ILogger<SEAService> logger)
        {
            _context = context;
            _cuadernoService = cuadernoService;
            _logger = logger;
        }

        public async Task<SEAReporteViewModel> GenerarReporteSEAAsync(int? materiaId, int? periodoAcademicoId)
        {
            try
            {
                _logger.LogInformation("Generando reporte SEA - Materia: {MateriaId}, Periodo: {PeriodoId}", materiaId, periodoAcademicoId);

                var reporte = new SEAReporteViewModel
                {
                    MateriaId = materiaId,
                    PeriodoAcademicoId = periodoAcademicoId,
                    FechaGeneracion = DateTime.Now
                };

                // Obtener información de materia y periodo
                if (materiaId.HasValue)
                {
                    var materia = await _context.Materias.FindAsync(materiaId.Value);
                    reporte.NombreMateria = materia?.Nombre ?? "Materia no encontrada";
                }

                if (periodoAcademicoId.HasValue)
                {
                    var periodo = await _context.PeriodosAcademicos.FindAsync(periodoAcademicoId.Value);
                    reporte.NombrePeriodo = periodo?.NombreCompleto ?? "Periodo no encontrado";
                }

                // Obtener configuración de componentes SEA
                var mapeoComponentes = await ObtenerMapeoComponentesAsync(materiaId ?? 0);
                reporte.ComponentesActivos = mapeoComponentes.Values.Distinct().ToList();

                // Obtener datos del cuaderno calificador (reutiliza cálculos existentes)
                var cuadernoData = await _cuadernoService.GenerarCuadernoCalificadorAsync(materiaId, periodoAcademicoId);

                // Obtener asistencias
                var asistencias = await CalcularAsistenciasPorEstudianteAsync(materiaId ?? 0, periodoAcademicoId ?? 0);

                // Construir reportes por estudiante
                foreach (var estudiante in cuadernoData.EstudiantesCalificaciones)
                {
                    var seaEstudiante = new SEAEstudianteReporte
                    {
                        EstudianteId = estudiante.EstudianteId,
                        NombreCompleto = estudiante.NombreCompleto,
                        NumeroIdentificacion = estudiante.NumeroIdentificacion ?? "",
                        CorreoElectronico = estudiante.CorreoElectronico ?? "",
                        RequiereACS = estudiante.RequiereACS,
                        TipoAdecuacion = estudiante.TipoAdecuacion
                    };

                    // Mapear notas de instrumentos a componentes SEA
                    foreach (var (instrumentoId, notaInstrumento) in estudiante.NotasPorInstrumento)
                    {
                        if (mapeoComponentes.TryGetValue(instrumentoId, out var componenteSEA))
                        {
                            var notaComponente = notaInstrumento.NotaConPonderacion;
                            seaEstudiante.NotasPorComponente[componenteSEA] = notaComponente;

                            // Asignar a propiedad específica
                            switch (componenteSEA)
                            {
                                case ComponentesSEA.TRABAJO_COTIDIANO:
                                    seaEstudiante.TrabajoCotidiano = notaComponente;
                                    break;
                                case ComponentesSEA.TAREAS:
                                    seaEstudiante.Tareas = notaComponente;
                                    break;
                                case ComponentesSEA.PRUEBAS:
                                    seaEstudiante.Pruebas = notaComponente;
                                    break;
                                case ComponentesSEA.PROYECTO:
                                    seaEstudiante.Proyecto = notaComponente;
                                    break;
                            }
                        }
                    }

                    // Asignar asistencia
                    if (asistencias.TryGetValue(estudiante.EstudianteId, out var asistencia))
                    {
                        seaEstudiante.Asistencia = asistencia.Porcentaje;
                        seaEstudiante.TotalLecciones = asistencia.TotalLecciones;
                        seaEstudiante.LeccionesPresente = asistencia.Presentes;
                        seaEstudiante.LeccionesAusente = asistencia.Ausentes;
                        seaEstudiante.LeccionesTardanza = asistencia.Tardanzas;
                        seaEstudiante.LeccionesJustificadas = asistencia.Justificadas;
                    }

                    // Nota final del cuaderno calificador
                    seaEstudiante.NotaFinal = estudiante.NotaFinalPonderada;
                    seaEstudiante.PorcentajeCompletado = estudiante.PorcentajeCompletado;

                    // Determinar estado general
                    seaEstudiante.EstadoGeneral = DeterminarEstadoGeneral(seaEstudiante.NotaFinal);

                    reporte.Estudiantes.Add(seaEstudiante);
                }

                // Validar IDs de estudiantes
                var advertencias = await ValidarIDsEstudiantesAsync(reporte.Estudiantes.Select(e => e.EstudianteId).ToList());
                foreach (var estudiante in reporte.Estudiantes)
                {
                    if (advertencias.TryGetValue(estudiante.EstudianteId, out var advs))
                    {
                        estudiante.AdvertenciasValidacion = advs;
                    }
                }

                // Calcular estadísticas
                reporte.Estadisticas = CalcularEstadisticas(reporte.Estudiantes);

                // Validar ponderaciones
                var validacion = await ValidarPonderacionesAsync(materiaId ?? 0);
                reporte.PonderacionesValidas = validacion.esValida;
                reporte.PonderacionTotal = validacion.total;

                _logger.LogInformation("Reporte SEA generado exitosamente. Total estudiantes: {Total}", reporte.Estudiantes.Count);

                return reporte;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generando reporte SEA");
                throw;
            }
        }

        public async Task<Dictionary<int, AsistenciaDetalle>> CalcularAsistenciasPorEstudianteAsync(int materiaId, int periodoId)
        {
            try
            {
                // Obtener fechas del periodo
                var periodo = await _context.PeriodosAcademicos.FindAsync(periodoId);
                if (periodo == null)
                {
                    _logger.LogWarning("Periodo {PeriodoId} no encontrado", periodoId);
                    return new Dictionary<int, AsistenciaDetalle>();
                }

                // Consultar asistencias del periodo
                var asistencias = await _context.Asistencias
                    .Where(a => a.MateriaId == materiaId &&
                               a.Fecha >= periodo.FechaInicio &&
                               a.Fecha <= periodo.FechaFin)
                    .GroupBy(a => a.EstudianteId)
                    .Select(g => new AsistenciaDetalle
                    {
                        TotalLecciones = g.Count(),
                        Presentes = g.Count(a => a.Estado == "P"),
                        Ausentes = g.Count(a => a.Estado == "A"),
                        Tardanzas = g.Count(a => a.Estado == "T"),
                        Justificadas = g.Count(a => a.Estado == "AJ")
                    })
                    .ToDictionaryAsync(
                        g => g.TotalLecciones, // Temporalmente usar TotalLecciones como key
                        g => g
                    );

                // Reconstruir con EstudianteId correcto
                var asistenciasPorEstudiante = await _context.Asistencias
                    .Where(a => a.MateriaId == materiaId &&
                               a.Fecha >= periodo.FechaInicio &&
                               a.Fecha <= periodo.FechaFin)
                    .GroupBy(a => a.EstudianteId)
                    .ToDictionaryAsync(
                        g => g.Key,
                        g => new AsistenciaDetalle
                        {
                            TotalLecciones = g.Count(),
                            Presentes = g.Count(a => a.Estado == "P"),
                            Ausentes = g.Count(a => a.Estado == "A"),
                            Tardanzas = g.Count(a => a.Estado == "T"),
                            Justificadas = g.Count(a => a.Estado == "AJ")
                        }
                    );

                _logger.LogInformation("Asistencias calculadas para {Count} estudiantes", asistenciasPorEstudiante.Count);

                return asistenciasPorEstudiante;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculando asistencias");
                return new Dictionary<int, AsistenciaDetalle>();
            }
        }

        public async Task<byte[]> ExportarCSV_SEAAsync(int? materiaId, int? periodoId, OpcionRedondeoSEA redondeo)
        {
            var reporte = await GenerarReporteSEAAsync(materiaId, periodoId);

            var csv = new StringBuilder();

            // Encoding UTF-8 con BOM (requerido por SEA)
            var encoding = new UTF8Encoding(true);

            // Encabezados
            var headers = new List<string> { "ID", "Nombre" };

            // Agregar columnas de componentes activos
            foreach (var componente in reporte.ComponentesActivos.OrderBy(c => c))
            {
                headers.Add(ComponentesSEA.ObtenerNombreAmigable(componente));
            }

            // Siempre incluir asistencia
            if (!headers.Contains("Asistencia"))
            {
                headers.Add("Asistencia");
            }

            headers.Add("Nota Final");

            csv.AppendLine(string.Join(",", headers));

            // Datos de estudiantes (ordenados alfabéticamente)
            foreach (var estudiante in reporte.Estudiantes.OrderBy(e => e.Apellidos).ThenBy(e => e.Nombre))
            {
                var row = new List<string>
                {
                    estudiante.NumeroIdentificacion,
                    $"\"{estudiante.Apellidos.ToUpper()}, {estudiante.Nombre.ToUpper()}\""
                };

                // Agregar componentes en el mismo orden que las columnas
                foreach (var componente in reporte.ComponentesActivos.OrderBy(c => c))
                {
                    var nota = estudiante.NotasPorComponente.TryGetValue(componente, out var n) ? n : null;
                    row.Add(FormatearNota(nota, redondeo));
                }

                // Asistencia
                row.Add(FormatearNota(estudiante.Asistencia, redondeo));

                // Nota final
                row.Add(FormatearNota(estudiante.NotaFinal, redondeo));

                csv.AppendLine(string.Join(",", row));
            }

            return encoding.GetBytes(csv.ToString());
        }

        public async Task<byte[]> ExportarPDF_SEAAsync(int? materiaId, int? periodoId, OpcionRedondeoSEA redondeo)
        {
            // TODO: Implementar exportación PDF usando QuestPDF o Rotativa
            throw new NotImplementedException("Exportación PDF pendiente de implementación");
        }

        public async Task<byte[]> ExportarExcel_SEAAsync(int? materiaId, int? periodoId, OpcionRedondeoSEA redondeo)
        {
            var reporte = await GenerarReporteSEAAsync(materiaId, periodoId);

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Reporte SEA");

            // Encabezado del reporte
            int fila = 1;
            worksheet.Cell(fila, 1).Value = "REPORTE SEA - SISTEMA DE EVALUACIÓN MEP";
            worksheet.Cell(fila, 1).Style.Font.Bold = true;
            worksheet.Cell(fila, 1).Style.Font.FontSize = 14;
            fila++;

            worksheet.Cell(fila, 1).Value = $"Materia: {reporte.NombreMateria}";
            worksheet.Cell(fila, 1).Style.Font.Bold = true;
            fila++;

            worksheet.Cell(fila, 1).Value = $"Periodo: {reporte.NombrePeriodo}";
            worksheet.Cell(fila, 1).Style.Font.Bold = true;
            fila++;

            worksheet.Cell(fila, 1).Value = $"Fecha: {reporte.FechaGeneracion:dd/MM/yyyy HH:mm}";
            fila += 2;

            // Encabezados de tabla
            int columna = 1;
            worksheet.Cell(fila, columna++).Value = "ID";
            worksheet.Cell(fila, columna++).Value = "Nombre";

            foreach (var componente in reporte.ComponentesActivos.OrderBy(c => c))
            {
                worksheet.Cell(fila, columna++).Value = ComponentesSEA.ObtenerNombreAmigable(componente);
            }

            worksheet.Cell(fila, columna++).Value = "Asistencia";
            worksheet.Cell(fila, columna++).Value = "Nota Final";
            worksheet.Cell(fila, columna++).Value = "Estado";

            // Formatear encabezados
            worksheet.Row(fila).Style.Font.Bold = true;
            worksheet.Row(fila).Style.Fill.BackgroundColor = XLColor.LightBlue;
            fila++;

            // Datos
            foreach (var estudiante in reporte.Estudiantes.OrderBy(e => e.Apellidos).ThenBy(e => e.Nombre))
            {
                columna = 1;
                worksheet.Cell(fila, columna++).Value = estudiante.NumeroIdentificacion;
                worksheet.Cell(fila, columna++).Value = estudiante.NombreCompleto;

                foreach (var componente in reporte.ComponentesActivos.OrderBy(c => c))
                {
                    var nota = estudiante.NotasPorComponente.TryGetValue(componente, out var n) ? n : null;
                    if (nota.HasValue)
                    {
                        worksheet.Cell(fila, columna).Value = Math.Round(nota.Value, (int)redondeo);
                    }
                    else
                    {
                        worksheet.Cell(fila, columna).Value = "-";
                    }
                    columna++;
                }

                // Asistencia
                if (estudiante.Asistencia.HasValue)
                {
                    worksheet.Cell(fila, columna).Value = Math.Round(estudiante.Asistencia.Value, (int)redondeo);
                }
                else
                {
                    worksheet.Cell(fila, columna).Value = "-";
                }
                columna++;

                // Nota final
                if (estudiante.NotaFinal.HasValue)
                {
                    worksheet.Cell(fila, columna).Value = Math.Round(estudiante.NotaFinal.Value, (int)redondeo);
                    
                    // Color según aprobación
                    if (estudiante.Aprobado)
                    {
                        worksheet.Cell(fila, columna).Style.Fill.BackgroundColor = XLColor.LightGreen;
                    }
                    else
                    {
                        worksheet.Cell(fila, columna).Style.Fill.BackgroundColor = XLColor.LightPink;
                    }
                }
                else
                {
                    worksheet.Cell(fila, columna).Value = "-";
                }
                columna++;

                // Estado
                worksheet.Cell(fila, columna).Value = estudiante.EstadoGeneral;

                fila++;
            }

            // Auto-ajustar columnas
            worksheet.Columns().AdjustToContents();

            // Escribir a stream
            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        public async Task<Dictionary<int, List<string>>> ValidarIDsEstudiantesAsync(List<int> estudianteIds)
        {
            var advertencias = new Dictionary<int, List<string>>();

            var estudiantes = await _context.Estudiantes
                .Where(e => estudianteIds.Contains(e.IdEstudiante))
                .ToListAsync();

            // Patrón de cédula costarricense: 1-9999-9999
            var patronCedula = new Regex(@"^\d{1}-\d{4}-\d{4}$");

            foreach (var estudiante in estudiantes)
            {
                var advs = new List<string>();

                if (string.IsNullOrWhiteSpace(estudiante.NumeroId))
                {
                    advs.Add("ID vacío - requerido por sistema SEA");
                }
                else if (!patronCedula.IsMatch(estudiante.NumeroId))
                {
                    advs.Add($"ID '{estudiante.NumeroId}' no coincide con formato de cédula MEP (1-9999-9999)");
                }

                if (advs.Any())
                {
                    advertencias[estudiante.IdEstudiante] = advs;
                }
            }

            return advertencias;
        }

        public async Task<Dictionary<int, string>> ObtenerMapeoComponentesAsync(int materiaId)
        {
            var configuraciones = await _context.Set<ConfiguracionComponenteSEA>()
                .Where(c => c.MateriaId == materiaId && c.Activo)
                .ToDictionaryAsync(c => c.InstrumentoEvaluacionId, c => c.ComponenteSEA);

            return configuraciones;
        }

        public async Task<bool> GuardarConfiguracionAsync(int materiaId, List<ConfiguracionComponenteItem> configuraciones)
        {
            try
            {
                // Eliminar configuraciones antiguas
                var configuracionesAntiguas = await _context.Set<ConfiguracionComponenteSEA>()
                    .Where(c => c.MateriaId == materiaId)
                    .ToListAsync();

                _context.Set<ConfiguracionComponenteSEA>().RemoveRange(configuracionesAntiguas);

                // Agregar nuevas configuraciones
                foreach (var config in configuraciones.Where(c => c.Activo))
                {
                    var nuevaConfig = new ConfiguracionComponenteSEA
                    {
                        MateriaId = materiaId,
                        InstrumentoEvaluacionId = config.InstrumentoEvaluacionId,
                        ComponenteSEA = config.ComponenteSEA,
                        Porcentaje = config.Porcentaje,
                        Activo = true,
                        FechaConfiguracion = DateTime.Now
                    };

                    _context.Set<ConfiguracionComponenteSEA>().Add(nuevaConfig);
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error guardando configuración SEA");
                return false;
            }
        }

        public async Task<ConfiguracionSEAViewModel> ObtenerConfiguracionAsync(int materiaId)
        {
            var materia = await _context.Materias.FindAsync(materiaId);
            var viewModel = new ConfiguracionSEAViewModel
            {
                MateriaId = materiaId,
                NombreMateria = materia?.Nombre ?? "Materia no encontrada"
            };

            // Obtener instrumentos disponibles
            var instrumentos = await _context.InstrumentoMaterias
                .Include(im => im.InstrumentoEvaluacion)
                .Where(im => im.MateriaId == materiaId && im.InstrumentoEvaluacion.Activo)
                .Select(im => new InstrumentoDisponibleSEA
                {
                    InstrumentoId = im.InstrumentoEvaluacionId,
                    Nombre = im.InstrumentoEvaluacion.Nombre,
                    TotalRubricas = im.InstrumentoEvaluacion.InstrumentoRubricas.Count
                })
                .ToListAsync();

            viewModel.InstrumentosDisponibles = instrumentos;

            // Obtener configuraciones actuales
            var configuraciones = await _context.Set<ConfiguracionComponenteSEA>()
                .Include(c => c.InstrumentoEvaluacion)
                .Where(c => c.MateriaId == materiaId)
                .Select(c => new ConfiguracionComponenteItem
                {
                    Id = c.Id,
                    InstrumentoEvaluacionId = c.InstrumentoEvaluacionId,
                    NombreInstrumento = c.InstrumentoEvaluacion.Nombre,
                    ComponenteSEA = c.ComponenteSEA,
                    Porcentaje = c.Porcentaje,
                    Activo = c.Activo
                })
                .ToListAsync();

            viewModel.ConfiguracionesActuales = configuraciones;

            return viewModel;
        }

        public async Task<(bool esValida, decimal total, string mensaje)> ValidarPonderacionesAsync(int materiaId)
        {
            var configuraciones = await _context.Set<ConfiguracionComponenteSEA>()
                .Where(c => c.MateriaId == materiaId && c.Activo)
                .ToListAsync();

            var total = configuraciones.Sum(c => c.Porcentaje);
            var esValida = Math.Abs(total - 100) < 0.01m;

            var mensaje = esValida
                ? "Ponderaciones válidas (100%)"
                : $"Ponderaciones inválidas: {total}% (debe sumar 100%)";

            return (esValida, total, mensaje);
        }

        // Métodos auxiliares privados

        private string DeterminarEstadoGeneral(decimal? notaFinal)
        {
            if (!notaFinal.HasValue) return EstadosEstudianteSEA.SIN_DATOS;
            if (notaFinal.Value >= 90) return EstadosEstudianteSEA.EXCELENTE;
            if (notaFinal.Value >= 70) return EstadosEstudianteSEA.SATISFACTORIO;
            if (notaFinal.Value >= 60) return EstadosEstudianteSEA.EN_RIESGO;
            return EstadosEstudianteSEA.CRITICO;
        }

        private string FormatearNota(decimal? nota, OpcionRedondeoSEA redondeo)
        {
            if (!nota.HasValue) return "";

            var notaRedondeada = Math.Round(nota.Value, (int)redondeo);
            return redondeo switch
            {
                OpcionRedondeoSEA.SinDecimales => notaRedondeada.ToString("F0"),
                OpcionRedondeoSEA.UnDecimal => notaRedondeada.ToString("F1"),
                OpcionRedondeoSEA.DosDecimales => notaRedondeada.ToString("F2"),
                _ => notaRedondeada.ToString("F0")
            };
        }

        private SEAEstadisticasGenerales CalcularEstadisticas(List<SEAEstudianteReporte> estudiantes)
        {
            var estadisticas = new SEAEstadisticasGenerales
            {
                TotalEstudiantes = estudiantes.Count,
                EstudiantesConDatos = estudiantes.Count(e => e.NotaFinal.HasValue),
                EstudiantesSinDatos = estudiantes.Count(e => !e.NotaFinal.HasValue)
            };

            var estudiantesConNotas = estudiantes.Where(e => e.NotaFinal.HasValue).ToList();

            if (estudiantesConNotas.Any())
            {
                var notas = estudiantesConNotas.Select(e => e.NotaFinal!.Value).ToList();

                estadisticas.PromedioGeneral = Math.Round(notas.Average(), 2);
                estadisticas.NotaMaxima = notas.Max();
                estadisticas.NotaMinima = notas.Min();
                estadisticas.EstudiantesAprobados = estudiantesConNotas.Count(e => e.Aprobado);
                estadisticas.EstudiantesReprobados = estudiantesConNotas.Count(e => !e.Aprobado);
                estadisticas.PorcentajeAprobacion = Math.Round(
                    (decimal)estadisticas.EstudiantesAprobados / estudiantesConNotas.Count * 100, 2);

                // Distribución de notas
                estadisticas.DistribucionNotas["90-100"] = notas.Count(n => n >= 90);
                estadisticas.DistribucionNotas["80-89"] = notas.Count(n => n >= 80 && n < 90);
                estadisticas.DistribucionNotas["70-79"] = notas.Count(n => n >= 70 && n < 80);
                estadisticas.DistribucionNotas["60-69"] = notas.Count(n => n >= 60 && n < 70);
                estadisticas.DistribucionNotas["< 60"] = notas.Count(n => n < 60);
            }

            // Asistencia promedio
            var estudiantesConAsistencia = estudiantes.Where(e => e.Asistencia.HasValue).ToList();
            if (estudiantesConAsistencia.Any())
            {
                estadisticas.AsistenciaPromedio = Math.Round(
                    estudiantesConAsistencia.Average(e => e.Asistencia!.Value), 2);
            }

            // Promedios por componente
            foreach (var componente in ComponentesSEA.ObtenerTodos())
            {
                var notasComponente = estudiantes
                    .Where(e => e.NotasPorComponente.ContainsKey(componente) && e.NotasPorComponente[componente].HasValue)
                    .Select(e => e.NotasPorComponente[componente]!.Value)
                    .ToList();

                if (notasComponente.Any())
                {
                    estadisticas.PromediosPorComponente[componente] = Math.Round(notasComponente.Average(), 2);
                }
            }

            return estadisticas;
        }
    }
}
