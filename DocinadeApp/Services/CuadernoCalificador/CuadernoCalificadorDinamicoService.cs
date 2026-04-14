using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DocinadeApp.Data;
using DocinadeApp.ViewModels;
using ClosedXML.Excel;
using System.Text;
using DocinadeApp.Extensions;

namespace DocinadeApp.Services.CuadernoCalificador
{
    public class CuadernoCalificadorDinamicoService : ICuadernoCalificadorDinamicoService
    {
        private readonly RubricasDbContext _context;
        private readonly ILogger<CuadernoCalificadorDinamicoService> _logger;

        public CuadernoCalificadorDinamicoService(
            RubricasDbContext context, 
            ILogger<CuadernoCalificadorDinamicoService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<CuadernoCalificadorDinamicoViewModel> GenerarCuadernoCalificadorAsync(int? materiaId, int? periodoAcademicoId)
        {
            var viewModel = new CuadernoCalificadorDinamicoViewModel
            {
                MateriaId = materiaId,
                PeriodoAcademicoId = periodoAcademicoId,
                FechaGeneracion = DateTime.Now
            };

            // Si no se especifican filtros, retornar modelo vac�o con opciones
            if (!materiaId.HasValue || !periodoAcademicoId.HasValue)
            {
                viewModel.MateriasDisponibles = await ObtenerMateriasDisponiblesAsync();
                viewModel.PeriodosDisponibles = await ObtenerPeriodosDisponiblesAsync();
                return viewModel;
            }

            try
            {
                // Obtener informacion de materia y periodo
                var materia = await _context.Materias.FindAsync(materiaId.Value);
                var periodo = await _context.PeriodosAcademicos.FindAsync(periodoAcademicoId.Value);

                if (materia == null || periodo == null)
                {
                    viewModel.MateriasDisponibles = await ObtenerMateriasDisponiblesAsync();
                    viewModel.PeriodosDisponibles = await ObtenerPeriodosDisponiblesAsync();
                    return viewModel;
                }

                viewModel.NombreMateria = materia.Nombre;
                viewModel.NombrePeriodoAcademico = $"{periodo.Nombre} ({periodo.Anio})";

                // 1. Obtener instrumentos de la materia con sus ponderaciones
                var (instrumentos, instrumentosSinRubricas, totalAsignados) = await ObtenerInstrumentosConDiagnosticoAsync(materiaId.Value);
                viewModel.Instrumentos = instrumentos;
                viewModel.InstrumentosSinRubricas = instrumentosSinRubricas;
                viewModel.TotalInstrumentosAsignados = totalAsignados;

                _logger.LogInformation(">>> DIAGNÓSTICO: Materia {MateriaId} tiene {Count} instrumentos configurados", 
                    materiaId, viewModel.Instrumentos.Count);
                
                foreach (var inst in viewModel.Instrumentos)
                {
                    _logger.LogInformation(">>> Instrumento: {Id} - {Nombre}, Ponderación: {Ponderacion}%, Rúbricas: {RubricasCount}",
                        inst.InstrumentoId, inst.NombreInstrumento, inst.PorcentajePonderacion, inst.Rubricas.Count);
                }

                if (!viewModel.Instrumentos.Any())
                {
                    _logger.LogWarning("No se encontraron instrumentos para la materia {MateriaId}", materiaId);
                    viewModel.MateriasDisponibles = await ObtenerMateriasDisponiblesAsync();
                    viewModel.PeriodosDisponibles = await ObtenerPeriodosDisponiblesAsync();
                    return viewModel;
                }

                // 2. Obtener estudiantes del período académico
                var estudiantes = await _context.Estudiantes
                    .Where(e => e.PeriodoAcademicoId == periodoAcademicoId.Value)
                    .OrderBy(e => e.Apellidos).ThenBy(e => e.Nombre)
                    .ToListAsync();

                viewModel.TotalEstudiantes = estudiantes.Count;

                // 3. Obtener todas las evaluaciones relevantes
                var instrumentosIds = viewModel.Instrumentos.Select(i => i.InstrumentoId).ToList();
                var rubricasIds = viewModel.Instrumentos.SelectMany(i => i.Rubricas.Select(r => r.RubricaId)).ToList();

                _logger.LogInformation("Buscando evaluaciones para {RubricasCount} rúbricas: {Rubricas}", 
                    rubricasIds.Count, string.Join(", ", rubricasIds));

                var evaluaciones = await _context.Evaluaciones
                    .Where(e => rubricasIds.Contains(e.IdRubrica) && 
                               e.TotalPuntos.HasValue && // Solo evaluaciones con calificación
                               estudiantes.Select(est => est.IdEstudiante).Contains(e.IdEstudiante))
                    .Include(e => e.Estudiante)
                    .Include(e => e.Rubrica)
                    .ToListAsync();

                _logger.LogInformation("Se encontraron {EvaluacionesCount} evaluaciones", evaluaciones.Count);
                
                // Log detallado de las evaluaciones encontradas
                foreach (var eval in evaluaciones)
                {
                    _logger.LogInformation("Evaluación: Estudiante={EstudianteId}, Rúbrica={RubricaId} ({RubricaNombre}), Puntos={Puntos}, Estado={Estado}",
                        eval.IdEstudiante, eval.IdRubrica, eval.Rubrica?.NombreRubrica, eval.TotalPuntos, eval.Estado);
                }

                // 4. Procesar calificaciones de estudiantes
                viewModel.EstudiantesCalificaciones = await ProcesarCalificacionesEstudiantesAsync(
                    estudiantes, viewModel.Instrumentos, evaluaciones);

                viewModel.EstudiantesConEvaluaciones = viewModel.EstudiantesCalificaciones.Count(e => e.NotasPorInstrumento.Any());

                // 5. Generar estad�sticas
                viewModel.Estadisticas = GenerarEstadisticas(viewModel.EstudiantesCalificaciones, viewModel.Instrumentos);

                // 6. Cargar opciones para filtros
                viewModel.MateriasDisponibles = await ObtenerMateriasDisponiblesAsync();
                viewModel.PeriodosDisponibles = await ObtenerPeriodosDisponiblesAsync();

                return viewModel;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generando cuaderno calificador para Materia: {MateriaId}, Per�odo: {PeriodoId}", 
                    materiaId, periodoAcademicoId);
                
                viewModel.MateriasDisponibles = await ObtenerMateriasDisponiblesAsync();
                viewModel.PeriodosDisponibles = await ObtenerPeriodosDisponiblesAsync();
                return viewModel;
            }
        }

        private async Task<(List<InstrumentoEvaluacionInfo>, List<string>, int)> ObtenerInstrumentosConDiagnosticoAsync(int materiaId)
        {
            var instrumentos = new List<InstrumentoEvaluacionInfo>();
            var instrumentosSinRubricas = new List<string>();

            // Obtener instrumentos de la materia con sus r�bricas y ponderaciones espec�ficas
            var instrumentosMaterias = await _context.InstrumentoMaterias
                .Include(im => im.InstrumentoEvaluacion)
                    .ThenInclude(ie => ie.InstrumentoRubricas)
                        .ThenInclude(ir => ir.Rubrica)
                .Where(im => im.MateriaId == materiaId && im.InstrumentoEvaluacion.Activo)
                .ToListAsync();

            _logger.LogInformation("Materia {MateriaId}: Se encontraron {Count} instrumentos asignados", 
                materiaId, instrumentosMaterias.Count);

            foreach (var instrumentoMateria in instrumentosMaterias)
            {
                var instrumento = instrumentoMateria.InstrumentoEvaluacion;
                
                _logger.LogInformation("Instrumento {InstrumentoId} ({Nombre}): {RubricasCount} r�bricas asignadas",
                    instrumento.InstrumentoId, instrumento.Nombre, instrumento.InstrumentoRubricas.Count);

                // Verificar si el instrumento tiene rubricas asignadas
                if (instrumento.InstrumentoRubricas == null || instrumento.InstrumentoRubricas.Count == 0)
                {
                    instrumentosSinRubricas.Add(instrumento.Nombre);
                    _logger.LogWarning("ADVERTENCIA: Instrumento '{Nombre}' (ID: {Id}) no tiene rubricas asignadas y sera omitido",
                        instrumento.Nombre, instrumento.InstrumentoId);
                    continue; // Saltar este instrumento
                }

                // Calcular la ponderación total del instrumento sumando las ponderaciones de sus rúbricas
                var ponderacionTotalInstrumento = instrumento.InstrumentoRubricas.Sum(ir => ir.Ponderacion);
                
                var instrumentoInfo = new InstrumentoEvaluacionInfo
                {
                    InstrumentoId = instrumento.InstrumentoId,
                    NombreInstrumento = instrumento.Nombre,
                    DescripcionInstrumento = instrumento.Descripcion ?? "",
                    // Usar la ponderación total real del instrumento
                    PorcentajePonderacion = ponderacionTotalInstrumento,
                    TotalRubricas = instrumento.InstrumentoRubricas.Count
                };

                // Obtener r�bricas del instrumento con sus ponderaciones espec�ficas
                foreach (var instrumentoRubrica in instrumento.InstrumentoRubricas)
                {
                    _logger.LogInformation("  - R�brica {RubricaId} ({Nombre}): Ponderaci�n {Ponderacion}%",
                        instrumentoRubrica.RubricaId, instrumentoRubrica.Rubrica.NombreRubrica, 
                        instrumentoRubrica.Ponderacion);

                    var rubricaInfo = new RubricaInstrumentoInfo
                    {
                        RubricaId = instrumentoRubrica.RubricaId,
                        NombreRubrica = instrumentoRubrica.Rubrica.NombreRubrica,
                        // Usar la ponderaci�n espec�fica de la asignaci�n
                        PonderacionEnInstrumento = instrumentoRubrica.Ponderacion
                    };

                    instrumentoInfo.Rubricas.Add(rubricaInfo);
                }

                instrumentos.Add(instrumentoInfo);
            }

            return (instrumentos, instrumentosSinRubricas, instrumentosMaterias.Count);
        }

        private async Task<List<InstrumentoEvaluacionInfo>> ObtenerInstrumentosConPonderacionesAsync(int materiaId)
        {
            var (instrumentos, _, _) = await ObtenerInstrumentosConDiagnosticoAsync(materiaId);
            return instrumentos;
        }

        private async Task<List<EstudianteCalificacionInfo>> ProcesarCalificacionesEstudiantesAsync(
            List<Models.Estudiante> estudiantes,
            List<InstrumentoEvaluacionInfo> instrumentos,
            List<Models.Evaluacion> evaluaciones)
        {
            var estudiantesCalificaciones = new List<EstudianteCalificacionInfo>();

            foreach (var estudiante in estudiantes)
            {
                var estudianteInfo = new EstudianteCalificacionInfo
                {
                    EstudianteId = estudiante.IdEstudiante,
                    NumeroIdentificacion = estudiante.NumeroId,
                    NombreCompleto = $"{estudiante.Apellidos}, {estudiante.Nombre}",
                    CorreoElectronico = estudiante.DireccionCorreo ?? ""
                };

                decimal notaFinalAcumulada = 0;
                decimal ponderacionTotalAplicada = 0;
                int instrumentosCompletos = 0;

                // Procesar cada instrumento
                foreach (var instrumento in instrumentos)
                {
                    var notaInstrumento = new NotaInstrumentoEstudiante
                    {
                        InstrumentoId = instrumento.InstrumentoId,
                        TotalEvaluacionesEsperadas = instrumento.Rubricas.Count
                    };

                    decimal sumaPuntosRubricas = 0;
                    decimal sumaPonderacionesRubricas = 0;
                    int evaluacionesCompletasInstrumento = 0;

                    // Procesar cada r�brica del instrumento
                    foreach (var rubrica in instrumento.Rubricas)
                    {
                        var evaluacionRubrica = evaluaciones.FirstOrDefault(e => 
                            e.IdEstudiante == estudiante.IdEstudiante && 
                            e.IdRubrica == rubrica.RubricaId);

                        if (evaluacionRubrica?.TotalPuntos.HasValue == true)
                        {
                            // Convertir puntos a escala de 100
                            var notaRubrica = evaluacionRubrica.TotalPuntos.Value;
                            notaInstrumento.NotasPorRubrica[rubrica.RubricaId] = notaRubrica;
                            
                            // Aplicar ponderaci�n de la r�brica dentro del instrumento
                            sumaPuntosRubricas += notaRubrica * (rubrica.PonderacionEnInstrumento / 100);
                            sumaPonderacionesRubricas += rubrica.PonderacionEnInstrumento;
                            evaluacionesCompletasInstrumento++;
                        }
                    }

                    notaInstrumento.EvaluacionesCompletas = evaluacionesCompletasInstrumento;

                    // Calcular nota del instrumento
                    if (evaluacionesCompletasInstrumento > 0 && sumaPonderacionesRubricas > 0)
                    {
                        // Normalizar a 100 si las ponderaciones no suman 100%
                        var factorNormalizacion = 100m / sumaPonderacionesRubricas;
                        notaInstrumento.NotaInstrumento = sumaPuntosRubricas * factorNormalizacion;
                        
                        // Aplicar ponderacion del instrumento en la materia
                        // CAMBIO IMPORTANTE: Usar la ponderacion del instrumento tal como está configurada
                        notaInstrumento.NotaConPonderacion = notaInstrumento.NotaInstrumento.Value * (instrumento.PorcentajePonderacion / 100);
                        
                        notaFinalAcumulada += notaInstrumento.NotaConPonderacion;
                        ponderacionTotalAplicada += instrumento.PorcentajePonderacion;
                    }

                    if (notaInstrumento.EstaCompleto)
                    {
                        instrumentosCompletos++;
                    }

                    estudianteInfo.NotasPorInstrumento[instrumento.InstrumentoId] = notaInstrumento;
                }

                // Calcular nota final y estado
                if (instrumentosCompletos == instrumentos.Count)
                {
                    estudianteInfo.NotaFinalPonderada = notaFinalAcumulada;
                    estudianteInfo.EstadoGeneral = "COMPLETO";
                    estudianteInfo.PorcentajeCompletado = 100;
                }
                else if (instrumentosCompletos > 0)
                {
                    // Calcular nota proporcional
                    if (ponderacionTotalAplicada > 0)
                    {
                        estudianteInfo.NotaFinalPonderada = notaFinalAcumulada;
                    }
                    estudianteInfo.EstadoGeneral = "PARCIAL";
                    estudianteInfo.PorcentajeCompletado = (decimal)instrumentosCompletos / instrumentos.Count * 100;
                }
                else
                {
                    estudianteInfo.EstadoGeneral = "PENDIENTE";
                    estudianteInfo.PorcentajeCompletado = 0;
                }

                estudianteInfo.Aprobado = estudianteInfo.NotaFinalPonderada >= 70; // 70% m�nimo para aprobar

                estudiantesCalificaciones.Add(estudianteInfo);
            }

            return estudiantesCalificaciones;
        }

        private EstadisticasGenerales GenerarEstadisticas(
            List<EstudianteCalificacionInfo> estudiantes,
            List<InstrumentoEvaluacionInfo> instrumentos)
        {
            var estadisticas = new EstadisticasGenerales();

            var estudiantesConNotas = estudiantes.Where(e => e.NotaFinalPonderada.HasValue).ToList();

            if (estudiantesConNotas.Any())
            {
                var notas = estudiantesConNotas.Select(e => e.NotaFinalPonderada.Value).ToList();
                
                estadisticas.PromedioGeneral = notas.Average();
                estadisticas.NotaMaxima = notas.Max();
                estadisticas.NotaMinima = notas.Min();
                estadisticas.EstudiantesAprobados = estudiantesConNotas.Count(e => e.Aprobado);
                estadisticas.EstudiantesReprobados = estudiantesConNotas.Count(e => !e.Aprobado);
                estadisticas.PorcentajeAprobacion = estudiantesConNotas.Count > 0 ? 
                    (decimal)estadisticas.EstudiantesAprobados / estudiantesConNotas.Count * 100 : 0;

                // Distribuci�n de notas
                estadisticas.DistribucionNotas = new Dictionary<string, int>
                {
                    ["90-100"] = notas.Count(n => n >= 90),
                    ["80-89"] = notas.Count(n => n >= 80 && n < 90),
                    ["70-79"] = notas.Count(n => n >= 70 && n < 80),
                    ["60-69"] = notas.Count(n => n >= 60 && n < 70),
                    ["< 60"] = notas.Count(n => n < 60)
                };
            }

            // Estad�sticas por instrumento
            foreach (var instrumento in instrumentos)
            {
                var estudiantesInstrumento = estudiantes
                    .Where(e => e.NotasPorInstrumento.ContainsKey(instrumento.InstrumentoId))
                    .Select(e => e.NotasPorInstrumento[instrumento.InstrumentoId])
                    .Where(n => n.NotaInstrumento.HasValue)
                    .ToList();

                if (estudiantesInstrumento.Any())
                {
                    var notasInstrumento = estudiantesInstrumento.Select(n => n.NotaInstrumento.Value).ToList();
                    
                    estadisticas.EstadisticasPorInstrumento[instrumento.InstrumentoId] = new EstadisticasInstrumento
                    {
                        InstrumentoId = instrumento.InstrumentoId,
                        NombreInstrumento = instrumento.NombreInstrumento,
                        PromedioInstrumento = notasInstrumento.Average(),
                        NotaMaximaInstrumento = notasInstrumento.Max(),
                        NotaMinimaInstrumento = notasInstrumento.Min(),
                        EstudiantesConNotaCompleta = estudiantesInstrumento.Count(n => n.EstaCompleto),
                        TotalEstudiantes = estudiantes.Count,
                        PorcentajeCompletado = estudiantes.Count > 0 ? 
                            (decimal)estudiantesInstrumento.Count(n => n.EstaCompleto) / estudiantes.Count * 100 : 0
                    };
                }
            }

            return estadisticas;
        }

        public async Task<List<SelectListItem>> ObtenerMateriasDisponiblesAsync()
        {
            var materias = await _context.Materias
                .Where(m => m.Activa)
                .OrderBy(m => m.Nombre)
                .Select(m => new SelectListItem
                {
                    Value = m.MateriaId.ToString(),
                    Text = $"{m.Codigo} - {m.Nombre}"
                })
                .ToListAsync();

            materias.Insert(0, new SelectListItem { Value = "", Text = "-- Seleccione una materia --" });
            return materias;
        }

        public async Task<List<SelectListItem>> ObtenerPeriodosDisponiblesAsync()
        {
            var periodos = await _context.PeriodosAcademicos
                .Where(p => p.Activo)
                .OrderByDescending(p => p.Anio).ThenByDescending(p => p.NumeroPeriodo)
                .Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = $"{p.Nombre} ({p.Anio})"
                })
                .ToListAsync();

            periodos.Insert(0, new SelectListItem { Value = "", Text = "-- Seleccione un per�odo --" });
            return periodos;
        }

        public async Task<byte[]> ExportarCuadernoAExcelAsync(int? materiaId, int? periodoAcademicoId)
        {
            var cuaderno = await GenerarCuadernoCalificadorAsync(materiaId, periodoAcademicoId);

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Cuaderno Calificador");

            // Encabezado principal
            worksheet.Cell(1, 1).Value = "CUADERNO CALIFICADOR";
            worksheet.Cell(1, 1).Style.Font.FontSize = 16;
            worksheet.Cell(1, 1).Style.Font.Bold = true;

            worksheet.Cell(2, 1).Value = $"Materia: {cuaderno.NombreMateria}";
            worksheet.Cell(3, 1).Value = $"Período: {cuaderno.NombrePeriodoAcademico}";
            worksheet.Cell(4, 1).Value = $"Fecha: {DateTime.Now:dd/MM/yyyy HH:mm}";

            // Headers de la tabla
            int fila = 6;
            int columna = 1;

            worksheet.Cell(fila, columna++).Value = "ID";
            worksheet.Cell(fila, columna++).Value = "Nombre";
            worksheet.Cell(fila, columna++).Value = "Correo";

            foreach (var instrumento in cuaderno.Instrumentos.OrderBy(i => i.InstrumentoId))
            {
                worksheet.Cell(fila, columna).Value = $"{instrumento.NombreInstrumento} ({instrumento.PorcentajePonderacion:F1}%)";
                columna++;
            }

            worksheet.Cell(fila, columna).Value = "Total (ponderado)";

            // Estilo del encabezado
            var encabezadoRango = worksheet.Range(fila, 1, fila, columna);
            encabezadoRango.Style.Font.Bold = true;
            encabezadoRango.Style.Fill.BackgroundColor = XLColor.LightGray;
            encabezadoRango.Style.Border.OutsideBorder = XLBorderStyleValues.Medium;

            // Datos de estudiantes
            fila++;
            foreach (var estudiante in cuaderno.EstudiantesCalificaciones.OrderBy(e => e.NombreCompleto))
            {
                columna = 1;
                worksheet.Cell(fila, columna++).Value = estudiante.NumeroIdentificacion;
                worksheet.Cell(fila, columna++).Value = estudiante.NombreCompleto;
                worksheet.Cell(fila, columna++).Value = estudiante.CorreoElectronico;

                foreach (var instrumento in cuaderno.Instrumentos.OrderBy(i => i.InstrumentoId))
                {
                    if (estudiante.NotasPorInstrumento.ContainsKey(instrumento.InstrumentoId) &&
                        estudiante.NotasPorInstrumento[instrumento.InstrumentoId].NotaInstrumento.HasValue)
                    {
                        var nota = estudiante.NotasPorInstrumento[instrumento.InstrumentoId].NotaInstrumento.Value;
                        worksheet.Cell(fila, columna).Value = nota;
                        worksheet.Cell(fila, columna).Style.NumberFormat.Format = "0.0";
                    }
                    else
                    {
                        worksheet.Cell(fila, columna).Value = "-";
                    }
                    columna++;
                }

                // Total ponderado
                if (estudiante.NotaFinalPonderada.HasValue)
                {
                    worksheet.Cell(fila, columna).Value = estudiante.NotaFinalPonderada.Value;
                    worksheet.Cell(fila, columna).Style.NumberFormat.Format = "0.0";
                    
                    // Colorear según aprobación
                    if (estudiante.Aprobado)
                    {
                        worksheet.Cell(fila, columna).Style.Font.FontColor = XLColor.DarkGreen;
                    }
                    else
                    {
                        worksheet.Cell(fila, columna).Style.Font.FontColor = XLColor.DarkRed;
                    }
                }
                else
                {
                    worksheet.Cell(fila, columna).Value = "-";
                }

                fila++;
            }

            // Agregar estad�sticas al final
            fila += 2;
            worksheet.Cell(fila, 1).Value = "ESTADISTICAS GENERALES";
            worksheet.Cell(fila, 1).Style.Font.Bold = true;
            
            fila++;
            worksheet.Cell(fila, 1).Value = $"Total estudiantes: {cuaderno.TotalEstudiantes}";
            fila++;
            worksheet.Cell(fila, 1).Value = $"Estudiantes con evaluaciones: {cuaderno.EstudiantesConEvaluaciones}";
            fila++;
            worksheet.Cell(fila, 1).Value = $"Promedio general: {cuaderno.Estadisticas.PromedioGeneral:F2}";
            fila++;
            worksheet.Cell(fila, 1).Value = $"% Aprobación: {cuaderno.Estadisticas.PorcentajeAprobacion:F1}%";

            // Autoajustar columnas
            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        /// <summary>
        /// Obtiene estadísticas rápidas para el dashboard con información de ponderaciones
        /// </summary>
        public async Task<Dictionary<string, object>> ObtenerEstadisticasRapidasAsync(int? materiaId, int? periodoAcademicoId)
        {
            var estadisticas = new Dictionary<string, object>();

            try
            {
                if (!materiaId.HasValue || !periodoAcademicoId.HasValue)
                {
                    return estadisticas;
                }

                var cuaderno = await GenerarCuadernoCalificadorAsync(materiaId, periodoAcademicoId);

                estadisticas["TotalEstudiantes"] = cuaderno.TotalEstudiantes;
                estadisticas["EstudiantesConEvaluaciones"] = cuaderno.EstudiantesConEvaluaciones;
                estadisticas["PromedioGeneral"] = cuaderno.Estadisticas.PromedioGeneral;
                estadisticas["PorcentajeAprobacion"] = cuaderno.Estadisticas.PorcentajeAprobacion;
                estadisticas["TotalInstrumentos"] = cuaderno.Instrumentos.Count;
                estadisticas["PonderacionValida"] = cuaderno.PonderacionesValidas;
                estadisticas["PonderacionTotal"] = cuaderno.PonderacionTotal;

                // Agregar informaci�n detallada de ponderaciones por instrumento
                var ponderacionesPorInstrumento = cuaderno.Instrumentos.ToDictionary(
                    i => i.NombreInstrumento,
                    i => i.PorcentajePonderacion
                );
                estadisticas["PonderacionesPorInstrumento"] = ponderacionesPorInstrumento;

                return estadisticas;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo estadísticas rápidas");
                return estadisticas;
            }
        }

        /// <summary>
        /// M�todo de diagn�stico para verificar las ponderaciones configuradas
        /// </summary>
        public async Task<Dictionary<string, object>> DiagnosticarPonderacionesAsync(int materiaId)
        {
            try
            {
                var diagnostico = new Dictionary<string, object>();

                // Obtener todos los instrumentos de la materia
                var instrumentosMaterias = await _context.InstrumentoMaterias
                    .Include(im => im.InstrumentoEvaluacion)
                        .ThenInclude(ie => ie.InstrumentoRubricas)
                            .ThenInclude(ir => ir.Rubrica)
                    .Where(im => im.MateriaId == materiaId && im.InstrumentoEvaluacion.Activo)
                    .ToListAsync();

                var instrumentosInfo = new List<object>();
                decimal ponderacionTotalMateria = 0;

                foreach (var instrumentoMateria in instrumentosMaterias)
                {
                    var instrumento = instrumentoMateria.InstrumentoEvaluacion;
                    var ponderacionInstrumento = instrumento.InstrumentoRubricas.Sum(ir => ir.Ponderacion);
                    ponderacionTotalMateria += ponderacionInstrumento;

                    var rubricasInfo = instrumento.InstrumentoRubricas.Select(ir => new
                    {
                        RubricaId = ir.RubricaId,
                        NombreRubrica = ir.Rubrica.NombreRubrica,
                        Ponderacion = ir.Ponderacion
                    }).ToList();

                    instrumentosInfo.Add(new
                    {
                        InstrumentoId = instrumento.InstrumentoId,
                        NombreInstrumento = instrumento.Nombre,
                        PonderacionInstrumento = ponderacionInstrumento,
                        TotalRubricas = instrumento.InstrumentoRubricas.Count,
                        Rubricas = rubricasInfo,
                        PonderacionRubricasSuma100 = Math.Abs(instrumento.InstrumentoRubricas.Sum(ir => ir.Ponderacion) - 100m) < 0.01m
                    });
                }

                diagnostico["MateriaId"] = materiaId;
                diagnostico["TotalInstrumentos"] = instrumentosMaterias.Count;
                diagnostico["PonderacionTotalMateria"] = ponderacionTotalMateria;
                diagnostico["PonderacionMateriaSuma100"] = Math.Abs(ponderacionTotalMateria - 100m) < 0.01m;
                diagnostico["Instrumentos"] = instrumentosInfo;

                return diagnostico;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en diagn�stico de ponderaciones para materia {MateriaId}", materiaId);
                return new Dictionary<string, object> { ["Error"] = ex.Message };
            }
        }
    }
}