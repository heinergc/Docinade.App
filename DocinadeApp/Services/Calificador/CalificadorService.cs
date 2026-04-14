using Microsoft.EntityFrameworkCore;
using RubricasApp.Web.Data;
using RubricasApp.Web.DTOs.Calificador;
using RubricasApp.Web.Models;

namespace RubricasApp.Web.Services.Calificador
{
    public class CalificadorService : ICalificadorService
    {
        private readonly RubricasDbContext _context;
        private readonly ILogger<CalificadorService> _logger;
        
        public CalificadorService(RubricasDbContext context, ILogger<CalificadorService> logger)
        {
            _context = context;
            _logger = logger;
        }
        
        public async Task<CuadernoCalificadorDto> GenerarCuadernoAsync(CalificadorQueryDto query)
        {
            _logger.LogInformation($"Generando cuaderno calificador para Materia: {query.MateriaId}, Periodo: {query.PeriodoAcademicoId}");
            
            // Validar par�metros
            if (!await ValidarParametrosAsync(query.MateriaId, query.PeriodoAcademicoId))
            {
                throw new ArgumentException("Los par�metros de consulta no son v�lidos");
            }
            
            var cuaderno = new CuadernoCalificadorDto();
            
            // Obtener informaci�n b�sica
            await CargarInformacionBasicaAsync(cuaderno, query.MateriaId, query.PeriodoAcademicoId);
            
            // Obtener columnas din�micas (Instrumento ? R�brica)
            cuaderno.Columnas = await ObtenerColumnasAsync(query.MateriaId, query.PeriodoAcademicoId);
            
            // Obtener estudiantes de la materia en el periodo
            var estudiantes = await ObtenerEstudiantesAsync(query.MateriaId, query.PeriodoAcademicoId, query.IncluirInactivos);
            
            // Generar filas del cuaderno
            cuaderno.Filas = await GenerarFilasAsync(estudiantes, cuaderno.Columnas, query);
            
            // Calcular estad�sticas
            cuaderno.Estadisticas = CalcularEstadisticas(cuaderno);
            
            _logger.LogInformation($"Cuaderno generado exitosamente. {cuaderno.Filas.Count} estudiantes, {cuaderno.Columnas.Count} columnas");
            
            return cuaderno;
        }
        
        public async Task<List<CalificadorColumnDto>> ObtenerColumnasAsync(int materiaId, int periodoAcademicoId)
        {
            var columnas = await _context.InstrumentoMaterias
                .Where(im => im.MateriaId == materiaId && im.PeriodoAcademicoId == periodoAcademicoId)
                .SelectMany(im => _context.InstrumentoRubricas
                    .Where(ir => ir.InstrumentoEvaluacionId == im.InstrumentoEvaluacionId)
                    .Select(ir => new CalificadorColumnDto
                    {
                        InstrumentoId = ir.InstrumentoEvaluacionId,
                        InstrumentoNombre = ir.InstrumentoEvaluacion.Nombre,
                        RubricaId = ir.RubricaId,
                        RubricaNombre = ir.Rubrica.NombreRubrica,
                        Ponderacion = ir.Ponderacion,
                        OrdenPresentacion = ir.OrdenPresentacion ?? 0
                    }))
                .OrderBy(c => c.InstrumentoId)
                .ThenBy(c => c.OrdenPresentacion)
                .ToListAsync();
                
            // Si no hay ponderaciones definidas, aplicar ponderaciones por defecto
            if (columnas.Any() && columnas.All(c => c.Ponderacion == 0))
            {
                AplicarPonderacionesPorDefecto(columnas);
            }
            
            return columnas;
        }
        
        public async Task<bool> ValidarParametrosAsync(int materiaId, int periodoAcademicoId)
        {
            var materiaExiste = await _context.Materias.AnyAsync(m => m.MateriaId == materiaId && m.Activa);
            var periodoExiste = await _context.PeriodosAcademicos.AnyAsync(p => p.Id == periodoAcademicoId && p.Activo);
            
            return materiaExiste && periodoExiste;
        }
        
        private async Task CargarInformacionBasicaAsync(CuadernoCalificadorDto cuaderno, int materiaId, int periodoAcademicoId)
        {
            var materia = await _context.Materias.FindAsync(materiaId);
            var periodo = await _context.PeriodosAcademicos.FindAsync(periodoAcademicoId);
            
            cuaderno.MateriaId = materiaId;
            cuaderno.MateriaNombre = materia?.Nombre ?? "Materia no encontrada";
            cuaderno.PeriodoAcademicoId = periodoAcademicoId;
            cuaderno.PeriodoAcademicoNombre = periodo?.NombreCompleto ?? "Periodo no encontrado";
        }
        
        private async Task<List<Estudiante>> ObtenerEstudiantesAsync(int materiaId, int periodoAcademicoId, bool incluirInactivos)
        {
            var query = _context.Estudiantes.AsQueryable();
            
            // Filtrar estudiantes que est�n en el periodo especificado
            query = query.Where(e => e.PeriodoAcademicoId == periodoAcademicoId);
            
            // Para incluir inactivos, normalmente se filtrar�an por alg�n estado, 
            // pero como el modelo no tiene propiedad Activo, incluimos todos
            // Si en el futuro se agrega estado, se puede ajustar aqu�
            
            return await query
                .OrderBy(e => e.Apellidos)
                .ThenBy(e => e.Nombre)
                .ToListAsync();
        }
        
        private async Task<List<CalificadorRowDto>> GenerarFilasAsync(
            List<Estudiante> estudiantes, 
            List<CalificadorColumnDto> columnas, 
            CalificadorQueryDto query)
        {
            var filas = new List<CalificadorRowDto>();
            
            foreach (var estudiante in estudiantes)
            {
                var fila = new CalificadorRowDto
                {
                    EstudianteId = estudiante.IdEstudiante,
                    EstudianteNombre = $"{estudiante.Apellidos}, {estudiante.Nombre}",
                    NumeroId = estudiante.NumeroId,
                    Estado = "ACTIVO" // Por defecto, ya que el modelo no tiene estado
                };
                
                // Obtener calificaciones del estudiante
                await CargarCalificacionesEstudianteAsync(fila, estudiante.IdEstudiante, columnas, query);
                
                // Calcular total final
                fila.TotalFinal = CalcularTotalFinal(fila, columnas);
                
                filas.Add(fila);
            }
            
            return filas;
        }
        
        private async Task CargarCalificacionesEstudianteAsync(
            CalificadorRowDto fila, 
            int estudianteId, 
            List<CalificadorColumnDto> columnas,
            CalificadorQueryDto query)
        {
            // Obtener todas las evaluaciones del estudiante para las r�bricas relevantes
            var rubricaIds = columnas.Select(c => c.RubricaId).ToList();
            
            var evaluaciones = await _context.Evaluaciones
                .Where(e => e.IdEstudiante == estudianteId && 
                           rubricaIds.Contains(e.IdRubrica) &&
                           e.Estado == "FINALIZADA")
                .ToListAsync();
            
            // Agrupar por instrumento para calcular calificaciones agregadas
            var calificacionesPorInstrumento = new Dictionary<int, List<decimal>>();
            
            foreach (var columna in columnas)
            {
                var evaluacion = evaluaciones.FirstOrDefault(e => e.IdRubrica == columna.RubricaId);
                decimal calificacion = evaluacion?.TotalPuntos ?? query.ValorPorDefecto;
                
                // Normalizar a escala 0-100 si es necesario
                if (calificacion > 100)
                {
                    calificacion = Math.Min(calificacion, 100);
                }
                
                fila.CalificacionesPorInstrumentoRubrica[columna.ClaveColumna] = calificacion;
                
                // Agregar a calificaciones por instrumento
                if (!calificacionesPorInstrumento.ContainsKey(columna.InstrumentoId))
                {
                    calificacionesPorInstrumento[columna.InstrumentoId] = new List<decimal>();
                }
                calificacionesPorInstrumento[columna.InstrumentoId].Add(calificacion);
            }
            
            // Calcular calificaciones agregadas por instrumento
            foreach (var kvp in calificacionesPorInstrumento)
            {
                decimal calificacionInstrumento = query.ModoCalculo switch
                {
                    "PROMEDIO" => kvp.Value.Average(),
                    "SUMA" => Math.Min(kvp.Value.Sum(), 100),
                    "MEJOR_NOTA" => kvp.Value.Max(),
                    _ => kvp.Value.Average()
                };
                
                fila.CalificacionesPorInstrumento[kvp.Key.ToString()] = calificacionInstrumento;
            }
        }
        
        private decimal CalcularTotalFinal(CalificadorRowDto fila, List<CalificadorColumnDto> columnas)
        {
            decimal total = 0;
            var instrumentosUnicos = columnas.GroupBy(c => c.InstrumentoId).ToList();
            
            foreach (var grupo in instrumentosUnicos)
            {
                var instrumentoId = grupo.Key;
                var ponderacion = grupo.First().Ponderacion;
                
                if (fila.CalificacionesPorInstrumento.TryGetValue(instrumentoId.ToString(), out decimal calificacionInstrumento))
                {
                    total += calificacionInstrumento * (ponderacion / 100m);
                }
            }
            
            return Math.Round(total, 2);
        }
        
        private void AplicarPonderacionesPorDefecto(List<CalificadorColumnDto> columnas)
        {
            var instrumentosUnicos = columnas.GroupBy(c => c.InstrumentoId).ToList();
            var totalInstrumentos = instrumentosUnicos.Count;
            
            if (totalInstrumentos == 0) return;
            
            // Ponderaciones por defecto seg�n especificaci�n
            var ponderacionesPorDefecto = new Dictionary<int, decimal>();
            
            if (totalInstrumentos >= 3)
            {
                // Tarea1 = 30%, Tarea2 = 30%, Proyecto1 = 40%
                var instrumentoIds = instrumentosUnicos.Select(g => g.Key).OrderBy(id => id).ToList();
                ponderacionesPorDefecto[instrumentoIds[0]] = 30m;
                if (instrumentoIds.Count > 1) ponderacionesPorDefecto[instrumentoIds[1]] = 30m;
                if (instrumentoIds.Count > 2) ponderacionesPorDefecto[instrumentoIds[2]] = 40m;
                
                // Distribuir el resto equitativamente
                var restantes = instrumentoIds.Skip(3).ToList();
                if (restantes.Any())
                {
                    decimal ponderacionRestante = 0m;
                    foreach (var resto in restantes)
                    {
                        ponderacionesPorDefecto[resto] = ponderacionRestante;
                    }
                }
            }
            else
            {
                // Distribuir equitativamente
                decimal ponderacionUniforme = 100m / totalInstrumentos;
                foreach (var grupo in instrumentosUnicos)
                {
                    ponderacionesPorDefecto[grupo.Key] = ponderacionUniforme;
                }
            }
            
            // Aplicar ponderaciones
            foreach (var columna in columnas)
            {
                if (ponderacionesPorDefecto.TryGetValue(columna.InstrumentoId, out decimal ponderacion))
                {
                    columna.Ponderacion = ponderacion;
                }
            }
            
            _logger.LogWarning($"Se aplicaron ponderaciones por defecto para {totalInstrumentos} instrumentos");
        }
        
        private CalificadorEstadisticasDto CalcularEstadisticas(CuadernoCalificadorDto cuaderno)
        {
            var estadisticas = new CalificadorEstadisticasDto
            {
                TotalEstudiantes = cuaderno.Filas.Count,
                TotalInstrumentos = cuaderno.Columnas.GroupBy(c => c.InstrumentoId).Count(),
                TotalRubricas = cuaderno.Columnas.Count
            };
            
            if (cuaderno.Filas.Any())
            {
                var totales = cuaderno.Filas.Select(f => f.TotalFinal).ToList();
                estadisticas.PromedioGeneral = Math.Round(totales.Average(), 2);
                estadisticas.NotaMaxima = totales.Max();
                estadisticas.NotaMinima = totales.Min();
                
                // Contar estudiantes con todas las notas vs pendientes
                foreach (var fila in cuaderno.Filas)
                {
                    bool tienePendientes = false;
                    foreach (var columna in cuaderno.Columnas)
                    {
                        if (fila.CalificacionesPorInstrumentoRubrica.TryGetValue(columna.ClaveColumna, out decimal nota))
                        {
                            if (nota == 0) // Asumiendo que 0 significa pendiente
                            {
                                tienePendientes = true;
                                break;
                            }
                        }
                    }
                    
                    if (tienePendientes)
                    {
                        estadisticas.EstudiantesConNotasPendientes++;
                    }
                    else
                    {
                        estadisticas.EstudiantesConTodasLasNotas++;
                    }
                }
            }
            
            return estadisticas;
        }
    }
}