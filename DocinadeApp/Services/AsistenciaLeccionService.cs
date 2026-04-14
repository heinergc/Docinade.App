using Microsoft.EntityFrameworkCore;
using DocinadeApp.Data;
using DocinadeApp.Models;

namespace DocinadeApp.Services
{
    public class AsistenciaLeccionService : IAsistenciaLeccionService
    {
        private readonly RubricasDbContext _context;
        private readonly ILogger<AsistenciaLeccionService> _logger;

        public AsistenciaLeccionService(RubricasDbContext context, ILogger<AsistenciaLeccionService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Asistencia?> ObtenerAsistenciaPorIdAsync(int asistenciaId)
        {
            try
            {
                return await _context.Asistencias
                    .Include(a => a.Estudiante)
                    .Include(a => a.Grupo)
                    .Include(a => a.Materia)
                    .Include(a => a.Leccion)
                    .FirstOrDefaultAsync(a => a.AsistenciaId == asistenciaId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener asistencia con ID {AsistenciaId}", asistenciaId);
                return null;
            }
        }

        public async Task<IEnumerable<Asistencia>> ObtenerAsistenciasPorLeccionAsync(int idLeccion, DateTime fecha)
        {
            try
            {
                return await _context.Asistencias
                    .Include(a => a.Estudiante)
                    .Include(a => a.Grupo)
                    .Include(a => a.Materia)
                    .Include(a => a.Leccion)
                    .Where(a => a.IdLeccion == idLeccion && a.Fecha.Date == fecha.Date)
                    .OrderBy(a => a.Estudiante.Apellidos)
                    .ThenBy(a => a.Estudiante.Nombre)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener asistencias de lección {IdLeccion} para fecha {Fecha}", idLeccion, fecha);
                return Enumerable.Empty<Asistencia>();
            }
        }

        public async Task<IEnumerable<Asistencia>> ObtenerAsistenciasPorEstudianteAsync(int estudianteId, DateTime? fechaInicio = null, DateTime? fechaFin = null)
        {
            try
            {
                var query = _context.Asistencias
                    .Include(a => a.Grupo)
                    .Include(a => a.Materia)
                    .Include(a => a.Leccion)
                    .Where(a => a.EstudianteId == estudianteId);

                if (fechaInicio.HasValue)
                    query = query.Where(a => a.Fecha >= fechaInicio.Value);

                if (fechaFin.HasValue)
                    query = query.Where(a => a.Fecha <= fechaFin.Value);

                return await query
                    .OrderByDescending(a => a.Fecha)
                    .ThenBy(a => a.Leccion != null ? a.Leccion.NumeroBloque : 0)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener asistencias del estudiante {EstudianteId}", estudianteId);
                return Enumerable.Empty<Asistencia>();
            }
        }

        public async Task<IEnumerable<Asistencia>> ObtenerAsistenciasPorGrupoYFechaAsync(int grupoId, DateTime fecha)
        {
            try
            {
                return await _context.Asistencias
                    .Include(a => a.Estudiante)
                    .Include(a => a.Materia)
                    .Include(a => a.Leccion)
                    .Where(a => a.GrupoId == grupoId && a.Fecha.Date == fecha.Date)
                    .OrderBy(a => a.Leccion != null ? a.Leccion.NumeroBloque : 0)
                    .ThenBy(a => a.Estudiante.Apellidos)
                    .ThenBy(a => a.Estudiante.Nombre)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener asistencias del grupo {GrupoId} para fecha {Fecha}", grupoId, fecha);
                return Enumerable.Empty<Asistencia>();
            }
        }

        public async Task<(bool Exito, string Mensaje, Asistencia? Asistencia)> RegistrarAsistenciaAsync(Asistencia asistencia, string usuarioId)
        {
            try
            {
                // Verificar si ya existe un registro para este estudiante/grupo/lección/fecha
                var registroExistente = await _context.Asistencias
                    .FirstOrDefaultAsync(a => 
                        a.EstudianteId == asistencia.EstudianteId &&
                        a.GrupoId == asistencia.GrupoId &&
                        a.IdLeccion == asistencia.IdLeccion &&
                        a.Fecha.Date == asistencia.Fecha.Date);

                if (registroExistente != null)
                {
                    // Actualizar registro existente según especificación MEP
                    if (!await ValidarTransicionEstadoAsync(registroExistente.Estado, asistencia.Estado))
                    {
                        return (false, $"Transición de estado no válida: {registroExistente.Estado} -> {asistencia.Estado}", null);
                    }

                    registroExistente.Estado = asistencia.Estado;
                    registroExistente.Observaciones = asistencia.Observaciones;
                    registroExistente.Justificacion = asistencia.Justificacion;
                    registroExistente.HoraLlegada = asistencia.HoraLlegada;
                    registroExistente.EsModificacion = true;
                    registroExistente.FechaModificacion = DateTime.Now;
                    registroExistente.ModificadoPorId = usuarioId;

                    await _context.SaveChangesAsync();

                    _logger.LogInformation("Asistencia actualizada: Estudiante {EstudianteId}, Lección {IdLeccion}, Estado {Estado}", 
                        asistencia.EstudianteId, asistencia.IdLeccion, asistencia.Estado);

                    return (true, "Asistencia actualizada exitosamente", registroExistente);
                }
                else
                {
                    // Crear nuevo registro
                    asistencia.FechaRegistro = DateTime.Now;
                    asistencia.RegistradoPorId = usuarioId;
                    asistencia.EsModificacion = false;

                    _context.Asistencias.Add(asistencia);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("Asistencia registrada: Estudiante {EstudianteId}, Lección {IdLeccion}, Estado {Estado}", 
                        asistencia.EstudianteId, asistencia.IdLeccion, asistencia.Estado);

                    return (true, "Asistencia registrada exitosamente", asistencia);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al registrar asistencia");
                return (false, $"Error al registrar asistencia: {ex.Message}", null);
            }
        }

        public async Task<(bool Exito, string Mensaje)> ActualizarAsistenciaAsync(Asistencia asistencia, string usuarioId)
        {
            try
            {
                var asistenciaExistente = await _context.Asistencias.FindAsync(asistencia.AsistenciaId);
                if (asistenciaExistente == null)
                {
                    return (false, "Registro de asistencia no encontrado");
                }

                // Validar transición de estado
                if (!await ValidarTransicionEstadoAsync(asistenciaExistente.Estado, asistencia.Estado))
                {
                    return (false, $"Transición de estado no válida: {asistenciaExistente.Estado} -> {asistencia.Estado}");
                }

                asistenciaExistente.Estado = asistencia.Estado;
                asistenciaExistente.Observaciones = asistencia.Observaciones;
                asistenciaExistente.Justificacion = asistencia.Justificacion;
                asistenciaExistente.HoraLlegada = asistencia.HoraLlegada;
                asistenciaExistente.EsModificacion = true;
                asistenciaExistente.FechaModificacion = DateTime.Now;
                asistenciaExistente.ModificadoPorId = usuarioId;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Asistencia actualizada: ID {AsistenciaId}", asistencia.AsistenciaId);
                return (true, "Asistencia actualizada exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar asistencia {AsistenciaId}", asistencia.AsistenciaId);
                return (false, $"Error al actualizar asistencia: {ex.Message}");
            }
        }

        public async Task<(bool Exito, string Mensaje)> RegistrarAsistenciasMasivasAsync(List<Asistencia> asistencias, string usuarioId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            
            try
            {
                int registradas = 0;
                int actualizadas = 0;

                foreach (var asistencia in asistencias)
                {
                    var resultado = await RegistrarAsistenciaAsync(asistencia, usuarioId);
                    if (resultado.Exito)
                    {
                        if (resultado.Asistencia?.EsModificacion ?? false)
                            actualizadas++;
                        else
                            registradas++;
                    }
                }

                await transaction.CommitAsync();

                _logger.LogInformation("Asistencias masivas procesadas: {Registradas} nuevas, {Actualizadas} actualizadas", 
                    registradas, actualizadas);

                return (true, $"Procesadas exitosamente: {registradas} nuevas, {actualizadas} actualizadas");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error al registrar asistencias masivas");
                return (false, $"Error al procesar asistencias: {ex.Message}");
            }
        }

        public async Task<Dictionary<string, int>> ObtenerEstadisticasAsistenciaEstudianteAsync(int estudianteId, DateTime? fechaInicio = null, DateTime? fechaFin = null)
        {
            try
            {
                var query = _context.Asistencias
                    .Where(a => a.EstudianteId == estudianteId);

                if (fechaInicio.HasValue)
                    query = query.Where(a => a.Fecha >= fechaInicio.Value);

                if (fechaFin.HasValue)
                    query = query.Where(a => a.Fecha <= fechaFin.Value);

                var asistencias = await query.ToListAsync();

                return new Dictionary<string, int>
                {
                    { "Total", asistencias.Count },
                    { "Presente", asistencias.Count(a => a.Estado == "P") },
                    { "Ausente", asistencias.Count(a => a.Estado == "A") },
                    { "Tardanza", asistencias.Count(a => a.Estado == "T") },
                    { "Justificada", asistencias.Count(a => a.Estado == "AJ") },
                    { "SinMarcar", asistencias.Count(a => a.Estado == "N") }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener estadísticas de asistencia del estudiante {EstudianteId}", estudianteId);
                return new Dictionary<string, int>();
            }
        }

        public async Task<decimal> CalcularPorcentajeAsistenciaAsync(int estudianteId, int? grupoId = null, int? materiaId = null, DateTime? fechaInicio = null, DateTime? fechaFin = null)
        {
            try
            {
                var query = _context.Asistencias
                    .Where(a => a.EstudianteId == estudianteId && a.Estado != "N");

                if (grupoId.HasValue)
                    query = query.Where(a => a.GrupoId == grupoId.Value);

                if (materiaId.HasValue)
                    query = query.Where(a => a.MateriaId == materiaId.Value);

                if (fechaInicio.HasValue)
                    query = query.Where(a => a.Fecha >= fechaInicio.Value);

                if (fechaFin.HasValue)
                    query = query.Where(a => a.Fecha <= fechaFin.Value);

                var total = await query.CountAsync();
                if (total == 0) return 0;

                var presentes = await query.CountAsync(a => a.Estado == "P" || a.Estado == "T");
                
                return Math.Round((decimal)presentes / total * 100, 2);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al calcular porcentaje de asistencia");
                return 0;
            }
        }

        public async Task<IEnumerable<Asistencia>> ObtenerAusenciasEstudianteAsync(int estudianteId, bool incluirJustificadas = false, DateTime? fechaInicio = null, DateTime? fechaFin = null)
        {
            try
            {
                var query = _context.Asistencias
                    .Include(a => a.Materia)
                    .Include(a => a.Grupo)
                    .Include(a => a.Leccion)
                    .Where(a => a.EstudianteId == estudianteId);

                if (incluirJustificadas)
                    query = query.Where(a => a.Estado == "A" || a.Estado == "AJ");
                else
                    query = query.Where(a => a.Estado == "A");

                if (fechaInicio.HasValue)
                    query = query.Where(a => a.Fecha >= fechaInicio.Value);

                if (fechaFin.HasValue)
                    query = query.Where(a => a.Fecha <= fechaFin.Value);

                return await query
                    .OrderByDescending(a => a.Fecha)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener ausencias del estudiante {EstudianteId}", estudianteId);
                return Enumerable.Empty<Asistencia>();
            }
        }

        public async Task<Dictionary<int, Dictionary<string, int>>> ObtenerResumenAsistenciaGrupoAsync(int grupoId, DateTime fechaInicio, DateTime fechaFin)
        {
            try
            {
                var asistencias = await _context.Asistencias
                    .Include(a => a.Estudiante)
                    .Where(a => a.GrupoId == grupoId && a.Fecha >= fechaInicio && a.Fecha <= fechaFin)
                    .ToListAsync();

                return asistencias
                    .GroupBy(a => a.EstudianteId)
                    .ToDictionary(
                        g => g.Key,
                        g => new Dictionary<string, int>
                        {
                            { "Total", g.Count() },
                            { "Presente", g.Count(a => a.Estado == "P") },
                            { "Ausente", g.Count(a => a.Estado == "A") },
                            { "Tardanza", g.Count(a => a.Estado == "T") },
                            { "Justificada", g.Count(a => a.Estado == "AJ") }
                        }
                    );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener resumen de asistencia del grupo {GrupoId}", grupoId);
                return new Dictionary<int, Dictionary<string, int>>();
            }
        }

        public Task<bool> ValidarTransicionEstadoAsync(string estadoActual, string nuevoEstado)
        {
            // Según especificación MEP, todas las transiciones son válidas
            // pero se puede personalizar según reglas específicas
            
            // Regla: No se puede cambiar de Justificada a Ausente sin justificar
            if (estadoActual == "AJ" && nuevoEstado == "A")
                return Task.FromResult(false);

            // Todas las demás transiciones son válidas
            return Task.FromResult(true);
        }
    }
}
