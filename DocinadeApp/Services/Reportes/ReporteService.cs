using Microsoft.EntityFrameworkCore;
using RubricasApp.Web.Data;
using RubricasApp.Web.ViewModels.Reportes;
using ClosedXML.Excel;
using System.Text;

namespace RubricasApp.Web.Services.Reportes
{
    public class ReporteService : IReporteService
    {
        private readonly RubricasDbContext _context;
        private readonly ILogger<ReporteService> _logger;

        public ReporteService(RubricasDbContext context, ILogger<ReporteService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ReporteArbolViewModel> GenerarReporteArbolAsync(FiltrosReporte filtros)
        {
            try
            {
                _logger.LogInformation("Generando reporte en árbol con filtros: {@Filtros}", filtros);

                // 1. Ejecutar consulta jerarquica
                var datosRaw = await EjecutarConsultaJerarquicaAsync(filtros);

                if (!datosRaw.Any())
                {
                    return new ReporteArbolViewModel
                    {
                        FiltrosAplicados = filtros,
                        ResumenGeneral = new ResumenGeneral()
                    };
                }

                // 2. Procesar y agrupar datos por materia
                var materiasAgrupadas = datosRaw
                    .Where(d => d.MateriaId > 0)
                    .GroupBy(d => new { d.MateriaId, d.CodigoMateria, d.NombreMateria, d.EstadoMateria })
                    .Select(mg => new MateriaNode
                    {
                        MateriaId = mg.Key.MateriaId,
                        Codigo = mg.Key.CodigoMateria,
                        Nombre = mg.Key.NombreMateria,
                        Estado = mg.Key.EstadoMateria,
                        Periodos = ProcesarPeriodos(mg),
                        
                        // Calcular estadísticas agregadas de la materia
                        TotalPeriodos = mg.Where(x => x.PeriodoId.HasValue).Select(x => x.PeriodoId).Distinct().Count(),
                        TotalEstudiantes = mg.Where(x => x.PeriodoId.HasValue).GroupBy(x => x.PeriodoId).Sum(g => g.Max(x => x.CantidadEstudiantes)),
                        TotalInstrumentos = mg.Where(x => x.InstrumentoId.HasValue).Select(x => x.InstrumentoId).Distinct().Count(),
                        TotalRubricas = mg.Where(x => x.RubricaId.HasValue).Select(x => x.RubricaId).Distinct().Count(),
                        TotalEvaluaciones = mg.Sum(x => x.CantidadEvaluaciones),
                        PorcentajeCompletitud = CalcularCompletitudMateria(mg),
                        PromedioMateria = mg.Where(x => x.PromedioCalificacion.HasValue).Average(x => x.PromedioCalificacion) ?? 0
                    })
                    .OrderBy(m => m.Codigo)
                    .ToList();

                // 3. Calcular resumen general
                var resumen = CalcularResumenGeneral(datosRaw);

                var reporte = new ReporteArbolViewModel
                {
                    ResumenGeneral = resumen,
                    Materias = materiasAgrupadas,
                    FiltrosAplicados = filtros,
                    TotalNodos = ContarNodos(new ReporteArbolViewModel { Materias = materiasAgrupadas })
                };

                _logger.LogInformation("Reporte generado exitosamente. Materias: {Materias}, Nodos: {Nodos}", 
                    materiasAgrupadas.Count, reporte.TotalNodos);

                return reporte;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generando reporte en arbol");
                throw;
            }
        }

        public async Task<List<DatoJerarquico>> EjecutarConsultaJerarquicaAsync(FiltrosReporte filtros)
        {
            var query = from m in _context.Materias
                        
                        // Relación con Períodos (via InstrumentoMaterias)
                        join im in _context.InstrumentoMaterias on m.MateriaId equals im.MateriaId into imGroup
                        from im in imGroup.DefaultIfEmpty()
                        
                        join pa in _context.PeriodosAcademicos on im.PeriodoAcademicoId equals pa.Id into paGroup
                        from pa in paGroup.DefaultIfEmpty()
                        
                        // Instrumentos de Evaluación
                        join ie in _context.InstrumentosEvaluacion on im.InstrumentoEvaluacionId equals ie.InstrumentoId into ieGroup
                        from ie in ieGroup.DefaultIfEmpty()
                        
                        // Rúbricas asociadas a Instrumentos
                        join ir in _context.InstrumentoRubricas on ie.InstrumentoId equals ir.InstrumentoEvaluacionId into irGroup
                        from ir in irGroup.DefaultIfEmpty()
                        
                        join r in _context.Rubricas on ir.RubricaId equals r.IdRubrica into rGroup
                        from r in rGroup.DefaultIfEmpty()
                        
                        // Cuadernos Configurados (alternativa)
                        join cc in _context.CuadernosCalificadores on new { m.MateriaId, PeriodoId = pa.Id } 
                            equals new { cc.MateriaId, PeriodoId = cc.PeriodoAcademicoId } into ccGroup
                        from cc in ccGroup.DefaultIfEmpty()
                        
                        join ci in _context.CuadernoInstrumentos on new { CuadernoId = cc.Id, RubricaId = r.IdRubrica }
                            equals new { CuadernoId = ci.CuadernoCalificadorId, ci.RubricaId } into ciGroup
                        from ci in ciGroup.DefaultIfEmpty()
                        
                        where m.Activa &&
                              (!filtros.SoloActivos || pa == null || pa.Activo) &&
                              (!filtros.SoloActivos || ie == null || ie.Activo) &&
                              (filtros.MateriaId == null || m.MateriaId == filtros.MateriaId) &&
                              (filtros.PeriodoId == null || pa.Id == filtros.PeriodoId) &&
                              (filtros.InstrumentoId == null || ie.InstrumentoId == filtros.InstrumentoId)
                        
                        group new { m, pa, ie, ir, r, cc, ci } by new
                        {
                            // Nivel Materia
                            m.MateriaId,
                            m.Codigo,
                            m.Nombre,
                            m.Estado,
                            
                            // Nivel Período
                            PeriodoId = pa != null ? pa.Id : (int?)null,
                            CodigoPeriodo = pa != null ? pa.Codigo : null,
                            NombrePeriodo = pa != null ? pa.Nombre : null,
                            
                            // Nivel Instrumento
                            InstrumentoId = ie != null ? ie.InstrumentoId : (int?)null,
                            NombreInstrumento = ie != null ? ie.Nombre : null,
                            DescripcionInstrumento = ie != null ? ie.Descripcion : null,
                            
                            // Nivel Rúbrica
                            RubricaId = r != null ? r.IdRubrica : (int?)null,
                            NombreRubrica = r != null ? r.NombreRubrica : null,
                            TituloRubrica = r != null ? r.Titulo : null,
                            EstadoRubrica = r != null ? r.Estado : null,
                            
                            // Ponderaciones
                            Ponderacion = ci != null ? ci.PonderacionPorcentaje : (ir != null ? ir.Ponderacion : 0),
                            OrdenInstrumento = ci != null ? ci.Orden : (ir != null ? ir.OrdenPresentacion ?? 999 : 999),
                            TipoCuaderno = cc != null ? "CUADERNO_CONFIGURADO" : "DINAMICO"
                        } into g
                        
                        select new DatoJerarquico
                        {
                            // Nivel Materia
                            MateriaId = g.Key.MateriaId,
                            CodigoMateria = g.Key.Codigo,
                            NombreMateria = g.Key.Nombre,
                            EstadoMateria = g.Key.Estado,
                            
                            // Nivel Período
                            PeriodoId = g.Key.PeriodoId,
                            CodigoPeriodo = g.Key.CodigoPeriodo,
                            NombrePeriodo = g.Key.NombrePeriodo,
                            PeriodoCompleto = g.Key.PeriodoId != null ? 
                                g.Where(x => x.pa != null).Select(x => $"{x.pa.Anio}-{x.pa.Codigo}").FirstOrDefault() : null,
                            
                            // Estudiantes del período
                            CantidadEstudiantes = g.Key.PeriodoId != null ?
                                _context.Estudiantes.Count(e => e.PeriodoAcademicoId == g.Key.PeriodoId) : 0,
                            
                            // Nivel Instrumento
                            InstrumentoId = g.Key.InstrumentoId,
                            NombreInstrumento = g.Key.NombreInstrumento,
                            DescripcionInstrumento = g.Key.DescripcionInstrumento,
                            OrdenInstrumento = g.Key.OrdenInstrumento,
                            
                            // Nivel R�brica
                            RubricaId = g.Key.RubricaId,
                            NombreRubrica = g.Key.NombreRubrica,
                            TituloRubrica = g.Key.TituloRubrica,
                            EstadoRubrica = g.Key.EstadoRubrica,
                            Ponderacion = g.Key.Ponderacion,
                            
                            // Estadísticas de evaluaciones (se calculan después por separado por performance)
                            CantidadEvaluaciones = 0,
                            EvaluacionesFinalizadas = 0,
                            EvaluacionesBorrador = 0,
                            PromedioCalificacion = null,
                            NotaMaxima = null,
                            NotaMinima = null,
                            
                            // Metadatos
                            TipoCuaderno = g.Key.TipoCuaderno
                        };

            var resultado = await query.ToListAsync();

            // Calcular estadísticas de evaluaciones por separado para mejor performance
            await CalcularEstadisticasEvaluacionesAsync(resultado, filtros);

            return resultado;
        }

        private async Task CalcularEstadisticasEvaluacionesAsync(List<DatoJerarquico> datos, FiltrosReporte filtros)
        {
            var rubricasIds = datos.Where(d => d.RubricaId.HasValue).Select(d => d.RubricaId.Value).Distinct().ToList();
            
            if (!rubricasIds.Any()) return;

            var estadisticasEvaluaciones = await (
                from e in _context.Evaluaciones
                join est in _context.Estudiantes on e.IdEstudiante equals est.IdEstudiante
                where rubricasIds.Contains(e.IdRubrica) &&
                      (filtros.EstadoEvaluacion == null || 
                       filtros.EstadoEvaluacion == "TODAS" ||
                       e.Estado == filtros.EstadoEvaluacion) &&
                      (filtros.FechaDesde == null || e.FechaEvaluacion >= filtros.FechaDesde) &&
                      (filtros.FechaHasta == null || e.FechaEvaluacion <= filtros.FechaHasta)
                group e by e.IdRubrica into g
                select new
                {
                    RubricaId = g.Key,
                    TotalEvaluaciones = g.Count(),
                    EvaluacionesFinalizadas = g.Count(x => x.Estado == "FINALIZADA"),
                    EvaluacionesBorrador = g.Count(x => x.Estado == "BORRADOR"),
                    PromedioCalificacion = g.Where(x => x.TotalPuntos.HasValue).Average(x => x.TotalPuntos),
                    NotaMaxima = g.Where(x => x.TotalPuntos.HasValue).Max(x => x.TotalPuntos),
                    NotaMinima = g.Where(x => x.TotalPuntos.HasValue).Min(x => x.TotalPuntos)
                }
            ).ToListAsync();

            // Aplicar estad�sticas a los datos
            foreach (var dato in datos.Where(d => d.RubricaId.HasValue))
            {
                var estadistica = estadisticasEvaluaciones.FirstOrDefault(e => e.RubricaId == dato.RubricaId.Value);
                if (estadistica != null)
                {
                    dato.CantidadEvaluaciones = estadistica.TotalEvaluaciones;
                    dato.EvaluacionesFinalizadas = estadistica.EvaluacionesFinalizadas;
                    dato.EvaluacionesBorrador = estadistica.EvaluacionesBorrador;
                    dato.PromedioCalificacion = estadistica.PromedioCalificacion;
                    dato.NotaMaxima = estadistica.NotaMaxima;
                    dato.NotaMinima = estadistica.NotaMinima;
                }
            }
        }

        private List<PeriodoNode> ProcesarPeriodos(IGrouping<dynamic, DatoJerarquico> materiaGroup)
        {
            return materiaGroup
                .Where(p => p.PeriodoId.HasValue)
                .GroupBy(p => new { p.PeriodoId, p.CodigoPeriodo, p.NombrePeriodo, p.PeriodoCompleto })
                .Select(pg => new PeriodoNode
                {
                    PeriodoId = pg.Key.PeriodoId.Value,
                    Codigo = pg.Key.CodigoPeriodo ?? "",
                    Nombre = pg.Key.NombrePeriodo ?? "",
                    PeriodoCompleto = pg.Key.PeriodoCompleto ?? "",
                    CantidadEstudiantes = pg.Max(x => x.CantidadEstudiantes),
                    Instrumentos = ProcesarInstrumentos(pg),
                    
                    // Calcular estadísticas del período
                    TotalInstrumentos = pg.Where(x => x.InstrumentoId.HasValue).Select(x => x.InstrumentoId).Distinct().Count(),
                    TotalRubricas = pg.Where(x => x.RubricaId.HasValue).Select(x => x.RubricaId).Distinct().Count(),
                    TotalEvaluaciones = pg.Sum(x => x.CantidadEvaluaciones),
                    EvaluacionesEsperadas = pg.Where(x => x.RubricaId.HasValue).Count() * pg.Max(x => x.CantidadEstudiantes),
                    EvaluacionesRealizadas = pg.Sum(x => x.CantidadEvaluaciones),
                    PorcentajeCompletitud = CalcularCompletitudPeriodo(pg),
                    PromedioPeriodo = pg.Where(x => x.PromedioCalificacion.HasValue).Average(x => x.PromedioCalificacion) ?? 0
                })
                .OrderBy(p => p.PeriodoCompleto)
                .ToList();
        }

        private List<InstrumentoNode> ProcesarInstrumentos(IGrouping<dynamic, DatoJerarquico> periodoGroup)
        {
            return periodoGroup
                .Where(i => i.InstrumentoId.HasValue)
                .GroupBy(i => new { i.InstrumentoId, i.NombreInstrumento, i.DescripcionInstrumento, i.OrdenInstrumento, i.TipoCuaderno })
                .Select(ig => new InstrumentoNode
                {
                    InstrumentoId = ig.Key.InstrumentoId.Value,
                    Nombre = ig.Key.NombreInstrumento ?? "",
                    Descripcion = ig.Key.DescripcionInstrumento ?? "",
                    Orden = ig.Key.OrdenInstrumento,
                    TipoCuaderno = ig.Key.TipoCuaderno,
                    Rubricas = ProcesarRubricas(ig),
                    
                    // Calcular estad�sticas del instrumento
                    TotalRubricas = ig.Where(x => x.RubricaId.HasValue).Select(x => x.RubricaId).Distinct().Count(),
                    TotalEvaluaciones = ig.Sum(x => x.CantidadEvaluaciones),
                    PonderacionTotal = ig.Sum(x => x.Ponderacion),
                    PromedioInstrumento = ig.Where(x => x.PromedioCalificacion.HasValue).Average(x => x.PromedioCalificacion) ?? 0
                })
                .OrderBy(i => i.Orden).ThenBy(i => i.Nombre)
                .ToList();
        }

        private List<RubricaNode> ProcesarRubricas(IGrouping<dynamic, DatoJerarquico> instrumentoGroup)
        {
            return instrumentoGroup
                .Where(r => r.RubricaId.HasValue)
                .GroupBy(r => new { r.RubricaId, r.NombreRubrica, r.TituloRubrica, r.EstadoRubrica })
                .Select(rg => 
                {
                    var primeraRubrica = rg.First();
                    var totalEvaluaciones = rg.Sum(x => x.CantidadEvaluaciones);
                    var evaluacionesFinalizadas = rg.Sum(x => x.EvaluacionesFinalizadas);
                    
                    return new RubricaNode
                    {
                        RubricaId = rg.Key.RubricaId.Value,
                        Nombre = rg.Key.NombreRubrica ?? "",
                        Titulo = rg.Key.TituloRubrica ?? "",
                        Estado = rg.Key.EstadoRubrica ?? "",
                        Ponderacion = primeraRubrica.Ponderacion,
                        CantidadEvaluaciones = totalEvaluaciones,
                        EvaluacionesFinalizadas = evaluacionesFinalizadas,
                        EvaluacionesBorrador = rg.Sum(x => x.EvaluacionesBorrador),
                        PorcentajeAvance = totalEvaluaciones > 0 ? (decimal)evaluacionesFinalizadas / totalEvaluaciones * 100 : 0,
                        PromedioCalificacion = primeraRubrica.PromedioCalificacion,
                        NotaMaxima = primeraRubrica.NotaMaxima,
                        NotaMinima = primeraRubrica.NotaMinima,
                        EstudiantesEvaluados = rg.Sum(x => x.EvaluacionesFinalizadas + x.EvaluacionesBorrador),
                        EstudiantesPendientes = 0 // Se calcular�a con una consulta adicional si es necesario
                    };
                })
                .OrderBy(r => r.Nombre)
                .ToList();
        }

        private ResumenGeneral CalcularResumenGeneral(List<DatoJerarquico> datos)
        {
            if (!datos.Any())
            {
                return new ResumenGeneral();
            }

            var totalEvaluaciones = datos.Sum(d => d.CantidadEvaluaciones);
            var totalEvaluacionesFinalizadas = datos.Sum(d => d.EvaluacionesFinalizadas);

            return new ResumenGeneral
            {
                TotalMaterias = datos.Select(d => d.MateriaId).Distinct().Count(),
                TotalPeriodos = datos.Where(d => d.PeriodoId.HasValue).Select(d => d.PeriodoId).Distinct().Count(),
                TotalInstrumentos = datos.Where(d => d.InstrumentoId.HasValue).Select(d => d.InstrumentoId).Distinct().Count(),
                TotalRubricas = datos.Where(d => d.RubricaId.HasValue).Select(d => d.RubricaId).Distinct().Count(),
                TotalEvaluaciones = totalEvaluaciones,
                TotalEstudiantes = datos.Where(d => d.PeriodoId.HasValue)
                    .GroupBy(d => d.PeriodoId)
                    .Sum(g => g.Max(x => x.CantidadEstudiantes)),
                PorcentajeCompletitudGeneral = totalEvaluaciones > 0 ? (decimal)totalEvaluacionesFinalizadas / totalEvaluaciones * 100 : 0,
                PromedioGeneralSistema = datos.Where(d => d.PromedioCalificacion.HasValue)
                    .Average(d => d.PromedioCalificacion) ?? 0
            };
        }

        private decimal CalcularCompletitudMateria(IGrouping<dynamic, DatoJerarquico> materiaGroup)
        {
            var totalEvaluaciones = materiaGroup.Sum(x => x.CantidadEvaluaciones);
            var evaluacionesFinalizadas = materiaGroup.Sum(x => x.EvaluacionesFinalizadas);
            
            return totalEvaluaciones > 0 ? (decimal)evaluacionesFinalizadas / totalEvaluaciones * 100 : 0;
        }

        private decimal CalcularCompletitudPeriodo(IGrouping<dynamic, DatoJerarquico> periodoGroup)
        {
            var totalEvaluaciones = periodoGroup.Sum(x => x.CantidadEvaluaciones);
            var evaluacionesFinalizadas = periodoGroup.Sum(x => x.EvaluacionesFinalizadas);
            
            return totalEvaluaciones > 0 ? (decimal)evaluacionesFinalizadas / totalEvaluaciones * 100 : 0;
        }

        public int ContarNodos(ReporteArbolViewModel reporte)
        {
            var totalNodos = reporte.Materias.Count; // Nodos de materia
            
            foreach (var materia in reporte.Materias)
            {
                totalNodos += materia.Periodos.Count; // Nodos de per�odo
                
                foreach (var periodo in materia.Periodos)
                {
                    totalNodos += periodo.Instrumentos.Count; // Nodos de instrumento
                    
                    foreach (var instrumento in periodo.Instrumentos)
                    {
                        totalNodos += instrumento.Rubricas.Count; // Nodos de r�brica
                    }
                }
            }
            
            return totalNodos;
        }

        public async Task<byte[]> ExportarReporteAExcelAsync(FiltrosReporte filtros)
        {
            var reporte = await GenerarReporteArbolAsync(filtros);

            using var workbook = new XLWorkbook();
            
            // Hoja de resumen
            var hojaResumen = workbook.Worksheets.Add("Resumen General");
            CrearHojaResumen(hojaResumen, reporte.ResumenGeneral);
            
            // Hoja de datos jer�rquicos
            var hojaDatos = workbook.Worksheets.Add("Datos Jer�rquicos");
            CrearHojaDatosJerarquicos(hojaDatos, reporte.Materias);

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        private void CrearHojaResumen(IXLWorksheet hoja, ResumenGeneral resumen)
        {
            hoja.Cell(1, 1).Value = "RESUMEN GENERAL DEL SISTEMA";
            hoja.Cell(1, 1).Style.Font.Bold = true;
            hoja.Cell(1, 1).Style.Font.FontSize = 16;

            var fila = 3;
            hoja.Cell(fila++, 1).Value = "Total Materias:";
            hoja.Cell(fila - 1, 2).Value = resumen.TotalMaterias;
            
            hoja.Cell(fila++, 1).Value = "Total Periodos:";
            hoja.Cell(fila - 1, 2).Value = resumen.TotalPeriodos;
            
            hoja.Cell(fila++, 1).Value = "Total Instrumentos:";
            hoja.Cell(fila - 1, 2).Value = resumen.TotalInstrumentos;
            
            hoja.Cell(fila++, 1).Value = "Total Rúbricas:";
            hoja.Cell(fila - 1, 2).Value = resumen.TotalRubricas;
            
            hoja.Cell(fila++, 1).Value = "Total Evaluaciones:";
            hoja.Cell(fila - 1, 2).Value = resumen.TotalEvaluaciones;
            
            hoja.Cell(fila++, 1).Value = "Total Estudiantes:";
            hoja.Cell(fila - 1, 2).Value = resumen.TotalEstudiantes;
            
            hoja.Cell(fila++, 1).Value = "% Completitud General:";
            hoja.Cell(fila - 1, 2).Value = $"{resumen.PorcentajeCompletitudGeneral:F1}%";
            
            hoja.Cell(fila++, 1).Value = "Promedio General:";
            hoja.Cell(fila - 1, 2).Value = resumen.PromedioGeneralSistema;

            hoja.Columns().AdjustToContents();
        }

        private void CrearHojaDatosJerarquicos(IXLWorksheet hoja, List<MateriaNode> materias)
        {
            // Headers
            var columnas = new[] { "Materia", "Periodo", "Instrumento", "Rúbrica", "Ponderación", 
                                 "Evaluaciones", "Finalizadas", "Borrador", "Promedio", "Estado" };
            
            for (int i = 0; i < columnas.Length; i++)
            {
                hoja.Cell(1, i + 1).Value = columnas[i];
                hoja.Cell(1, i + 1).Style.Font.Bold = true;
                hoja.Cell(1, i + 1).Style.Fill.BackgroundColor = XLColor.LightGray;
            }

            var fila = 2;
            foreach (var materia in materias)
            {
                foreach (var periodo in materia.Periodos)
                {
                    foreach (var instrumento in periodo.Instrumentos)
                    {
                        foreach (var rubrica in instrumento.Rubricas)
                        {
                            hoja.Cell(fila, 1).Value = $"[{materia.Codigo}] {materia.Nombre}";
                            hoja.Cell(fila, 2).Value = periodo.PeriodoCompleto;
                            hoja.Cell(fila, 3).Value = instrumento.Nombre;
                            hoja.Cell(fila, 4).Value = rubrica.Nombre;
                            hoja.Cell(fila, 5).Value = $"{rubrica.Ponderacion:F1}%";
                            hoja.Cell(fila, 6).Value = rubrica.CantidadEvaluaciones;
                            hoja.Cell(fila, 7).Value = rubrica.EvaluacionesFinalizadas;
                            hoja.Cell(fila, 8).Value = rubrica.EvaluacionesBorrador;
                            hoja.Cell(fila, 9).Value = rubrica.PromedioCalificacion?.ToString("F1") ?? "-";
                            hoja.Cell(fila, 10).Value = rubrica.Estado;
                            fila++;
                        }
                    }
                }
            }

            hoja.Columns().AdjustToContents();
        }

        public async Task<byte[]> ExportarReporteACSVAsync(FiltrosReporte filtros)
        {
            var reporte = await GenerarReporteArbolAsync(filtros);
            var csv = new StringBuilder();

            // Header
            csv.AppendLine("Materia,Periodo,Instrumento,Rubrica,Ponderacion,Evaluaciones,Finalizadas,Borrador,Promedio,Estado");

            // Datos
            foreach (var materia in reporte.Materias)
            {
                foreach (var periodo in materia.Periodos)
                {
                    foreach (var instrumento in periodo.Instrumentos)
                    {
                        foreach (var rubrica in instrumento.Rubricas)
                        {
                            csv.AppendLine($"\"{materia.Codigo} - {materia.Nombre}\"," +
                                         $"\"{periodo.PeriodoCompleto}\"," +
                                         $"\"{instrumento.Nombre}\"," +
                                         $"\"{rubrica.Nombre}\"," +
                                         $"{rubrica.Ponderacion:F1}," +
                                         $"{rubrica.CantidadEvaluaciones}," +
                                         $"{rubrica.EvaluacionesFinalizadas}," +
                                         $"{rubrica.EvaluacionesBorrador}," +
                                         $"{rubrica.PromedioCalificacion?.ToString("F1") ?? ""}," +
                                         $"\"{rubrica.Estado}\"");
                        }
                    }
                }
            }

            return Encoding.UTF8.GetBytes(csv.ToString());
        }

        public async Task<Dictionary<string, object>> ObtenerEstadisticasRapidasAsync()
        {
            var filtros = new FiltrosReporte { SoloActivos = true };
            var reporte = await GenerarReporteArbolAsync(filtros);

            return new Dictionary<string, object>
            {
                ["totalMaterias"] = reporte.ResumenGeneral.TotalMaterias,
                ["totalPeriodos"] = reporte.ResumenGeneral.TotalPeriodos,
                ["totalInstrumentos"] = reporte.ResumenGeneral.TotalInstrumentos,
                ["totalRubricas"] = reporte.ResumenGeneral.TotalRubricas,
                ["totalEvaluaciones"] = reporte.ResumenGeneral.TotalEvaluaciones,
                ["totalEstudiantes"] = reporte.ResumenGeneral.TotalEstudiantes,
                ["porcentajeCompletitud"] = reporte.ResumenGeneral.PorcentajeCompletitudGeneral,
                ["promedioGeneral"] = reporte.ResumenGeneral.PromedioGeneralSistema,
                ["totalNodos"] = reporte.TotalNodos
            };
        }
    }
}