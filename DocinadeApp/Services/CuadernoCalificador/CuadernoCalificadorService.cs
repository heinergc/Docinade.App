using Microsoft.EntityFrameworkCore;
using RubricasApp.Web.Data;
using RubricasApp.Web.Models;
using System.Text;
using ClosedXML.Excel;

namespace RubricasApp.Web.Services.CuadernoCalificador
{
    public class CuadernoCalificadorService : ICuadernoCalificadorService
    {
        private readonly RubricasDbContext _context;

        public CuadernoCalificadorService(RubricasDbContext context)
        {
            _context = context;
        }

        public async Task<CuadernoCalificadorViewModel> ObtenerCuadernoAsync(int cuadernoId)
        {
            var cuaderno = await _context.CuadernosCalificadores
                .Include(c => c.Materia)
                .Include(c => c.PeriodoAcademico)
                .Include(c => c.CuadernoInstrumentos)
                    .ThenInclude(ci => ci.Rubrica)
                .FirstOrDefaultAsync(c => c.Id == cuadernoId);

            if (cuaderno == null)
                throw new ArgumentException($"No se encontró el cuaderno con ID {cuadernoId}");

            var viewModel = new CuadernoCalificadorViewModel
            {
                CuadernoId = cuaderno.Id,
                NombreCuaderno = cuaderno.Nombre,
                NombreMateria = cuaderno.Materia.Nombre,
                NombrePeriodoAcademico = $"{cuaderno.PeriodoAcademico.Anio} - {cuaderno.PeriodoAcademico.Codigo}",
                FechaCreacion = cuaderno.FechaCreacion,
                Estado = cuaderno.Estado,
                Instrumentos = cuaderno.CuadernoInstrumentos.Select(ci => new InstrumentoCalificacionInfo
                {
                    CuadernoInstrumentoId = ci.Id,
                    RubricaId = ci.RubricaId,
                    NombreInstrumento = ci.Rubrica.NombreRubrica,
                    NombreRubrica = ci.Rubrica.Titulo,
                    PonderacionPorcentaje = ci.PonderacionPorcentaje,
                    Orden = ci.Orden,
                    EsObligatorio = ci.EsObligatorio
                }).OrderBy(i => i.Orden).ToList()
            };

            // Obtener calificaciones de estudiantes
            viewModel.CalificacionesEstudiantes = await ObtenerCalificacionesEstudiantesAsync(cuadernoId);

            // Generar estadísticas
            viewModel.Estadisticas = await GenerarEstadisticasAsync(cuadernoId);

            return viewModel;
        }

        public async Task<List<CalificacionEstudiante>> ObtenerCalificacionesEstudiantesAsync(int cuadernoId)
        {
            var cuaderno = await _context.CuadernosCalificadores
                .Include(c => c.CuadernoInstrumentos)
                .Include(c => c.PeriodoAcademico)
                .FirstOrDefaultAsync(c => c.Id == cuadernoId);

            if (cuaderno == null)
                return new List<CalificacionEstudiante>();

            // Obtener estudiantes del período académico
            var estudiantes = await _context.Estudiantes
                .Where(e => e.PeriodoAcademicoId == cuaderno.PeriodoAcademicoId)
                .ToListAsync();

            // Obtener todas las evaluaciones relacionadas con las rúbricas del cuaderno
            var rubricasIds = cuaderno.CuadernoInstrumentos.Select(ci => ci.RubricaId).ToList();
            
            var evaluaciones = await _context.Evaluaciones
                .Where(e => rubricasIds.Contains(e.IdRubrica))
                .GroupBy(e => new { e.IdEstudiante, e.IdRubrica })
                .Select(g => new
                {
                    EstudianteId = g.Key.IdEstudiante,
                    RubricaId = g.Key.IdRubrica,
                    TotalPuntos = g.OrderByDescending(e => e.FechaEvaluacion).First().TotalPuntos
                })
                .ToListAsync();

            var calificacionesEstudiantes = new List<CalificacionEstudiante>();

            foreach (var estudiante in estudiantes)
            {
                var calificacionEstudiante = new CalificacionEstudiante
                {
                    EstudianteId = estudiante.IdEstudiante,
                    NombreCompleto = $"{estudiante.Apellidos}, {estudiante.Nombre}",
                    NumeroId = estudiante.NumeroId,
                    CalificacionesPorInstrumento = new Dictionary<int, decimal?>()
                };

                decimal? totalPonderado = 0;
                decimal sumaPonderaciones = 0;
                bool tieneTodasLasCalificaciones = true;

                foreach (var instrumento in cuaderno.CuadernoInstrumentos)
                {
                    var evaluacion = evaluaciones.FirstOrDefault(e => 
                        e.EstudianteId == estudiante.IdEstudiante && 
                        e.RubricaId == instrumento.RubricaId);

                    if (evaluacion != null)
                    {
                        calificacionEstudiante.CalificacionesPorInstrumento[instrumento.Id] = evaluacion.TotalPuntos;
                        totalPonderado += evaluacion.TotalPuntos * (instrumento.PonderacionPorcentaje / 100);
                        sumaPonderaciones += instrumento.PonderacionPorcentaje;
                    }
                    else
                    {
                        calificacionEstudiante.CalificacionesPorInstrumento[instrumento.Id] = null;
                        tieneTodasLasCalificaciones = false;
                    }
                }

                calificacionEstudiante.TotalPonderado = tieneTodasLasCalificaciones ? totalPonderado : null;
                calificacionEstudiante.Estado = tieneTodasLasCalificaciones ? "COMPLETO" : "PENDIENTE";

                calificacionesEstudiantes.Add(calificacionEstudiante);
            }

            return calificacionesEstudiantes.OrderBy(c => c.NombreCompleto).ToList();
        }

        public async Task<bool> ValidarPonderacionesAsync(int cuadernoId)
        {
            var instrumentos = await _context.CuadernoInstrumentos
                .Where(ci => ci.CuadernoCalificadorId == cuadernoId)
                .ToListAsync();

            var totalPonderacion = instrumentos.Sum(i => i.PonderacionPorcentaje);
            return totalPonderacion == 100m;
        }

        public async Task<decimal> CalcularNotaPonderadaEstudianteAsync(int estudianteId, int cuadernoId)
        {
            var calificaciones = await ObtenerCalificacionesEstudiantesAsync(cuadernoId);
            var estudiante = calificaciones.FirstOrDefault(c => c.EstudianteId == estudianteId);
            
            return estudiante?.TotalPonderado ?? 0;
        }

        public async Task<EstadisticasCuaderno> GenerarEstadisticasAsync(int cuadernoId)
        {
            var calificaciones = await ObtenerCalificacionesEstudiantesAsync(cuadernoId);
            
            var estadisticas = new EstadisticasCuaderno
            {
                TotalEstudiantes = calificaciones.Count,
                EstudiantesEvaluados = calificaciones.Count(c => c.Estado == "COMPLETO"),
                EstudiantesPendientes = calificaciones.Count(c => c.Estado == "PENDIENTE")
            };

            var notasCompletas = calificaciones
                .Where(c => c.TotalPonderado.HasValue)
                .Select(c => c.TotalPonderado.Value)
                .ToList();

            if (notasCompletas.Any())
            {
                estadisticas.PromedioGeneral = notasCompletas.Average();
                estadisticas.NotaMaxima = notasCompletas.Max();
                estadisticas.NotaMinima = notasCompletas.Min();

                // Distribución de notas
                estadisticas.DistribucionNotas = new Dictionary<string, int>
                {
                    ["9.0 - 10.0"] = notasCompletas.Count(n => n >= 9.0m),
                    ["8.0 - 8.9"] = notasCompletas.Count(n => n >= 8.0m && n < 9.0m),
                    ["7.0 - 7.9"] = notasCompletas.Count(n => n >= 7.0m && n < 8.0m),
                    ["6.0 - 6.9"] = notasCompletas.Count(n => n >= 6.0m && n < 7.0m),
                    ["< 6.0"] = notasCompletas.Count(n => n < 6.0m)
                };
            }

            return estadisticas;
        }

        public async Task<int> CrearCuadernoAsync(CrearCuadernoViewModel model)
        {
            var cuaderno = new Models.CuadernoCalificador
            {
                Nombre = model.Nombre,
                MateriaId = model.MateriaId,
                PeriodoAcademicoId = model.PeriodoAcademicoId,
                FechaCreacion = DateTime.Now,
                Estado = "ACTIVO"
            };

            _context.CuadernosCalificadores.Add(cuaderno);
            await _context.SaveChangesAsync();

            return cuaderno.Id;
        }

        public async Task<bool> ActualizarPonderacionesAsync(int cuadernoId, List<InstrumentoConfiguracion> ponderaciones)
        {
            var instrumentosExistentes = await _context.CuadernoInstrumentos
                .Where(ci => ci.CuadernoCalificadorId == cuadernoId)
                .ToListAsync();

            // Eliminar instrumentos marcados para eliminar
            var instrumentosAEliminar = instrumentosExistentes
                .Where(ie => ponderaciones.Any(p => p.Id == ie.Id && p.Eliminar))
                .ToList();

            _context.CuadernoInstrumentos.RemoveRange(instrumentosAEliminar);

            // Actualizar instrumentos existentes
            foreach (var ponderacion in ponderaciones.Where(p => p.Id > 0 && !p.Eliminar))
            {
                var instrumento = instrumentosExistentes.FirstOrDefault(ie => ie.Id == ponderacion.Id);
                if (instrumento != null)
                {
                    instrumento.PonderacionPorcentaje = ponderacion.PonderacionPorcentaje;
                    instrumento.Orden = ponderacion.Orden;
                    instrumento.EsObligatorio = ponderacion.EsObligatorio;
                }
            }

            // Agregar nuevos instrumentos
            foreach (var ponderacion in ponderaciones.Where(p => p.Id == 0))
            {
                var nuevoInstrumento = new Models.CuadernoInstrumento
                {
                    CuadernoCalificadorId = cuadernoId,
                    RubricaId = ponderacion.RubricaId,
                    PonderacionPorcentaje = ponderacion.PonderacionPorcentaje,
                    Orden = ponderacion.Orden,
                    EsObligatorio = ponderacion.EsObligatorio
                };

                _context.CuadernoInstrumentos.Add(nuevoInstrumento);
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<byte[]> ExportarExcelAsync(int cuadernoId)
        {
            var cuaderno = await ObtenerCuadernoAsync(cuadernoId);

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Calificaciones");

            // Encabezado
            worksheet.Cell(1, 1).Value = "CUADERNO CALIFICADOR";
            worksheet.Cell(1, 1).Style.Font.FontSize = 16;
            worksheet.Cell(1, 1).Style.Font.Bold = true;

            worksheet.Cell(2, 1).Value = $"Materia: {cuaderno.NombreMateria}";
            worksheet.Cell(3, 1).Value = $"Período: {cuaderno.NombrePeriodoAcademico}";
            worksheet.Cell(4, 1).Value = $"Fecha: {DateTime.Now:dd/MM/yyyy}";

            // Headers de tabla
            int fila = 6;
            int columna = 1;

            worksheet.Cell(fila, columna++).Value = "Estudiante";

            foreach (var instrumento in cuaderno.Instrumentos.OrderBy(i => i.Orden))
            {
                worksheet.Cell(fila, columna).Value = $"{instrumento.NombreInstrumento} ({instrumento.PonderacionPorcentaje}%)";
                columna++;
            }

            worksheet.Cell(fila, columna).Value = "TOTAL PONDERADO";

            // Estilo del encabezado
            var encabezadoRango = worksheet.Range(fila, 1, fila, columna);
            encabezadoRango.Style.Font.Bold = true;
            encabezadoRango.Style.Fill.BackgroundColor = XLColor.LightGray;

            // Datos
            fila++;
            foreach (var estudiante in cuaderno.CalificacionesEstudiantes.OrderBy(e => e.NombreCompleto))
            {
                columna = 1;
                worksheet.Cell(fila, columna++).Value = estudiante.NombreCompleto;

                foreach (var instrumento in cuaderno.Instrumentos.OrderBy(i => i.Orden))
                {
                    var nota = estudiante.CalificacionesPorInstrumento.ContainsKey(instrumento.CuadernoInstrumentoId)
                        ? estudiante.CalificacionesPorInstrumento[instrumento.CuadernoInstrumentoId]
                        : null;

                    worksheet.Cell(fila, columna++).Value = nota?.ToString("0.00") ?? "-";
                }

                worksheet.Cell(fila, columna).Value = estudiante.TotalPonderado?.ToString("0.00") ?? "-";
                fila++;
            }

            // Formateo
            worksheet.RangeUsed().SetAutoFilter();
            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        public async Task<byte[]> GenerarReportePDFAsync(int cuadernoId)
        {
            // Implementación básica - se puede mejorar con una librería de PDF como iText7
            var cuaderno = await ObtenerCuadernoAsync(cuadernoId);
            
            var html = GenerarHtmlReporte(cuaderno);
            
            // Por ahora retornamos el HTML como bytes
            // En una implementación real, usar una librería como wkhtmltopdf o Puppeteer
            return Encoding.UTF8.GetBytes(html);
        }

        private string GenerarHtmlReporte(CuadernoCalificadorViewModel cuaderno)
        {
            var html = new StringBuilder();
            html.AppendLine("<html><head><title>Cuaderno Calificador</title>");
            html.AppendLine("<style>table { border-collapse: collapse; width: 100%; } th, td { border: 1px solid #ddd; padding: 8px; text-align: center; } th { background-color: #f2f2f2; }</style>");
            html.AppendLine("</head><body>");
            html.AppendLine($"<h1>CUADERNO CALIFICADOR</h1>");
            html.AppendLine($"<p><strong>Materia:</strong> {cuaderno.NombreMateria}</p>");
            html.AppendLine($"<p><strong>Período:</strong> {cuaderno.NombrePeriodoAcademico}</p>");
            html.AppendLine($"<p><strong>Fecha:</strong> {DateTime.Now:dd/MM/yyyy}</p>");
            
            html.AppendLine("<table>");
            html.AppendLine("<tr><th>Estudiante</th>");
            
            foreach (var instrumento in cuaderno.Instrumentos.OrderBy(i => i.Orden))
            {
                html.AppendLine($"<th>{instrumento.NombreInstrumento}<br>({instrumento.PonderacionPorcentaje}%)</th>");
            }
            
            html.AppendLine("<th>TOTAL PONDERADO</th></tr>");
            
            foreach (var estudiante in cuaderno.CalificacionesEstudiantes.OrderBy(e => e.NombreCompleto))
            {
                html.AppendLine($"<tr><td>{estudiante.NombreCompleto}</td>");
                
                foreach (var instrumento in cuaderno.Instrumentos.OrderBy(i => i.Orden))
                {
                    var nota = estudiante.CalificacionesPorInstrumento.ContainsKey(instrumento.CuadernoInstrumentoId)
                        ? estudiante.CalificacionesPorInstrumento[instrumento.CuadernoInstrumentoId]
                        : null;
                    
                    html.AppendLine($"<td>{nota?.ToString("0.00") ?? "-"}</td>");
                }
                
                html.AppendLine($"<td><strong>{estudiante.TotalPonderado?.ToString("0.00") ?? "-"}</strong></td></tr>");
            }
            
            html.AppendLine("</table>");
            html.AppendLine("</body></html>");
            
            return html.ToString();
        }

        public async Task<List<Models.CuadernoCalificador>> ObtenerCuadernosPorFiltroAsync(int? materiaId, int? periodoAcademicoId)
        {
            var query = _context.CuadernosCalificadores
                .Include(c => c.Materia)
                .Include(c => c.PeriodoAcademico)
                .AsQueryable();

            if (materiaId.HasValue)
            {
                query = query.Where(c => c.MateriaId == materiaId.Value);
            }

            if (periodoAcademicoId.HasValue)
            {
                query = query.Where(c => c.PeriodoAcademicoId == periodoAcademicoId.Value);
            }

            return await query
                .OrderByDescending(c => c.FechaCreacion)
                .ToListAsync();
        }

        public async Task<ConfigurarInstrumentosViewModel> ObtenerConfiguracionInstrumentosAsync(int cuadernoId)
        {
            var cuaderno = await _context.CuadernosCalificadores
                .Include(c => c.Materia)
                .Include(c => c.CuadernoInstrumentos)
                    .ThenInclude(ci => ci.Rubrica)
                .FirstOrDefaultAsync(c => c.Id == cuadernoId);

            if (cuaderno == null)
                throw new ArgumentException($"No se encontró el cuaderno con ID {cuadernoId}");

            var viewModel = new ConfigurarInstrumentosViewModel
            {
                CuadernoId = cuaderno.Id,
                NombreCuaderno = cuaderno.Nombre,
                NombreMateria = cuaderno.Materia.Nombre,
                Instrumentos = cuaderno.CuadernoInstrumentos.Select(ci => new InstrumentoConfiguracion
                {
                    Id = ci.Id,
                    RubricaId = ci.RubricaId,
                    NombreRubrica = ci.Rubrica.NombreRubrica,
                    PonderacionPorcentaje = ci.PonderacionPorcentaje,
                    Orden = ci.Orden,
                    EsObligatorio = ci.EsObligatorio
                }).OrderBy(i => i.Orden).ToList()
            };

            // Obtener rúbricas disponibles
            var rubricasUsadas = cuaderno.CuadernoInstrumentos.Select(ci => ci.RubricaId).ToList();
            viewModel.RubricasDisponibles = await _context.Rubricas
                .Where(r => !rubricasUsadas.Contains(r.IdRubrica) && r.Estado == "ACTIVO")
                .Select(r => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Value = r.IdRubrica.ToString(),
                    Text = $"{r.NombreRubrica} - {r.Titulo}"
                })
                .ToListAsync();

            return viewModel;
        }

        public async Task<bool> GuardarConfiguracionInstrumentosAsync(ConfigurarInstrumentosViewModel model)
        {
            return await ActualizarPonderacionesAsync(model.CuadernoId, model.Instrumentos);
        }

        public async Task<bool> CerrarCuadernoAsync(int cuadernoId)
        {
            var cuaderno = await _context.CuadernosCalificadores
                .FirstOrDefaultAsync(c => c.Id == cuadernoId);

            if (cuaderno == null)
                return false;

            cuaderno.Estado = "CERRADO";
            cuaderno.FechaCierre = DateTime.Now;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}