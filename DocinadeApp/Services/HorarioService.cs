using Microsoft.EntityFrameworkCore;
using DocinadeApp.Data;
using DocinadeApp.Models;

namespace DocinadeApp.Services
{
    public class HorarioService : IHorarioService
    {
        private readonly RubricasDbContext _context;
        private readonly ILogger<HorarioService> _logger;

        public HorarioService(RubricasDbContext context, ILogger<HorarioService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Leccion?> ObtenerLeccionPorIdAsync(int idLeccion)
        {
            try
            {
                return await _context.Lecciones
                    .Include(l => l.Grupo)
                    .Include(l => l.Materia)
                    .FirstOrDefaultAsync(l => l.IdLeccion == idLeccion);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener lección con ID {IdLeccion}", idLeccion);
                return null;
            }
        }

        public async Task<IEnumerable<Leccion>> ObtenerLeccionesPorGrupoAsync(int idGrupo, bool soloActivas = true)
        {
            try
            {
                var query = _context.Lecciones
                    .Include(l => l.Materia)
                    .Include(l => l.Grupo)
                    .Where(l => l.IdGrupo == idGrupo);

                if (soloActivas)
                    query = query.Where(l => l.Activa);

                return await query
                    .OrderBy(l => l.DiaSemana)
                    .ThenBy(l => l.NumeroBloque)
                    .ThenBy(l => l.HoraInicio)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener lecciones del grupo {IdGrupo}", idGrupo);
                return Enumerable.Empty<Leccion>();
            }
        }

        public async Task<IEnumerable<Leccion>> ObtenerLeccionesPorGrupoYDiaAsync(int idGrupo, int diaSemana, bool soloActivas = true)
        {
            try
            {
                var query = _context.Lecciones
                    .Include(l => l.Materia)
                    .Include(l => l.Grupo)
                    .Where(l => l.IdGrupo == idGrupo && (int)l.DiaSemana == diaSemana);

                if (soloActivas)
                    query = query.Where(l => l.Activa);

                return await query
                    .OrderBy(l => l.NumeroBloque)
                    .ThenBy(l => l.HoraInicio)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener lecciones del grupo {IdGrupo} para el día {DiaSemana}", idGrupo, diaSemana);
                return Enumerable.Empty<Leccion>();
            }
        }

        public async Task<IEnumerable<Leccion>> ObtenerLeccionesPorMateriaAsync(int materiaId, bool soloActivas = true)
        {
            try
            {
                var query = _context.Lecciones
                    .Include(l => l.Grupo)
                    .Include(l => l.Materia)
                    .Where(l => l.MateriaId == materiaId);

                if (soloActivas)
                    query = query.Where(l => l.Activa);

                return await query
                    .OrderBy(l => l.Grupo.Nombre)
                    .ThenBy(l => l.DiaSemana)
                    .ThenBy(l => l.NumeroBloque)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener lecciones de la materia {MateriaId}", materiaId);
                return Enumerable.Empty<Leccion>();
            }
        }

        public async Task<Leccion?> ObtenerLeccionPorBloqueAsync(int idGrupo, int materiaId, int diaSemana, int numeroBloque)
        {
            try
            {
                return await _context.Lecciones
                    .Include(l => l.Grupo)
                    .Include(l => l.Materia)
                    .FirstOrDefaultAsync(l => 
                        l.IdGrupo == idGrupo && 
                        l.MateriaId == materiaId && 
                        (int)l.DiaSemana == diaSemana && 
                        l.NumeroBloque == numeroBloque &&
                        l.Activa);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener lección por bloque");
                return null;
            }
        }

        public async Task<(bool Exito, string Mensaje, Leccion? Leccion)> CrearLeccionAsync(Leccion leccion)
        {
            try
            {
                // Validar horario válido
                if (!await ValidarHorarioValidoAsync(leccion.HoraInicio, leccion.HoraFin))
                {
                    return (false, "La hora de fin debe ser mayor que la hora de inicio", null);
                }

                // Validar que no exista duplicado
                var duplicado = await _context.Lecciones
                    .AnyAsync(l => 
                        l.IdGrupo == leccion.IdGrupo && 
                        l.MateriaId == leccion.MateriaId && 
                        l.DiaSemana == leccion.DiaSemana && 
                        l.NumeroBloque == leccion.NumeroBloque);

                if (duplicado)
                {
                    return (false, "Ya existe una lección con ese grupo, materia, día y bloque", null);
                }

                // Validar solapamiento de horarios
                if (await ValidarSolapamientoHorarioAsync(leccion.IdGrupo, (int)leccion.DiaSemana, leccion.HoraInicio, leccion.HoraFin))
                {
                    return (false, "El horario se solapa con otra lección existente para este grupo y día", null);
                }

                leccion.FechaCreacion = DateTime.Now;
                leccion.Activa = true;

                _context.Lecciones.Add(leccion);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Lección creada: Grupo {IdGrupo}, Materia {MateriaId}, Día {DiaSemana}, Bloque {NumeroBloque}", 
                    leccion.IdGrupo, leccion.MateriaId, leccion.DiaSemana, leccion.NumeroBloque);

                return (true, "Lección creada exitosamente", leccion);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear lección");
                return (false, $"Error al crear lección: {ex.Message}", null);
            }
        }

        public async Task<(bool Exito, string Mensaje)> ActualizarLeccionAsync(Leccion leccion)
        {
            try
            {
                var leccionExistente = await _context.Lecciones.FindAsync(leccion.IdLeccion);
                if (leccionExistente == null)
                {
                    return (false, "Lección no encontrada");
                }

                // Validar horario válido
                if (!await ValidarHorarioValidoAsync(leccion.HoraInicio, leccion.HoraFin))
                {
                    return (false, "La hora de fin debe ser mayor que la hora de inicio");
                }

                // Validar duplicado (excluyendo la lección actual)
                var duplicado = await _context.Lecciones
                    .AnyAsync(l => 
                        l.IdLeccion != leccion.IdLeccion &&
                        l.IdGrupo == leccion.IdGrupo && 
                        l.MateriaId == leccion.MateriaId && 
                        l.DiaSemana == leccion.DiaSemana && 
                        l.NumeroBloque == leccion.NumeroBloque);

                if (duplicado)
                {
                    return (false, "Ya existe otra lección con ese grupo, materia, día y bloque");
                }

                // Validar solapamiento
                if (await ValidarSolapamientoHorarioAsync(leccion.IdGrupo, (int)leccion.DiaSemana, leccion.HoraInicio, leccion.HoraFin, leccion.IdLeccion))
                {
                    return (false, "El horario se solapa con otra lección existente");
                }

                leccionExistente.IdGrupo = leccion.IdGrupo;
                leccionExistente.MateriaId = leccion.MateriaId;
                leccionExistente.NumeroBloque = leccion.NumeroBloque;
                leccionExistente.DiaSemana = leccion.DiaSemana;
                leccionExistente.HoraInicio = leccion.HoraInicio;
                leccionExistente.HoraFin = leccion.HoraFin;
                leccionExistente.Observaciones = leccion.Observaciones;
                leccionExistente.FechaModificacion = DateTime.Now;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Lección actualizada: ID {IdLeccion}", leccion.IdLeccion);
                return (true, "Lección actualizada exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar lección {IdLeccion}", leccion.IdLeccion);
                return (false, $"Error al actualizar lección: {ex.Message}");
            }
        }

        public async Task<(bool Exito, string Mensaje)> EliminarLeccionAsync(int idLeccion, bool eliminacionFisica = false)
        {
            try
            {
                var leccion = await _context.Lecciones.FindAsync(idLeccion);
                if (leccion == null)
                {
                    return (false, "Lección no encontrada");
                }

                // Verificar si tiene asistencias registradas
                var tieneAsistencias = await _context.Asistencias.AnyAsync(a => a.IdLeccion == idLeccion);
                
                if (tieneAsistencias && eliminacionFisica)
                {
                    return (false, "No se puede eliminar la lección porque tiene registros de asistencia asociados");
                }

                if (eliminacionFisica)
                {
                    _context.Lecciones.Remove(leccion);
                    _logger.LogInformation("Lección eliminada físicamente: ID {IdLeccion}", idLeccion);
                }
                else
                {
                    leccion.Activa = false;
                    leccion.FechaModificacion = DateTime.Now;
                    _logger.LogInformation("Lección desactivada: ID {IdLeccion}", idLeccion);
                }

                await _context.SaveChangesAsync();
                return (true, eliminacionFisica ? "Lección eliminada exitosamente" : "Lección desactivada exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar lección {IdLeccion}", idLeccion);
                return (false, $"Error al eliminar lección: {ex.Message}");
            }
        }

        public async Task<bool> ValidarSolapamientoHorarioAsync(int idGrupo, int diaSemana, TimeSpan horaInicio, TimeSpan horaFin, int? idLeccionExcluir = null)
        {
            try
            {
                var query = _context.Lecciones
                    .Where(l => l.IdGrupo == idGrupo && (int)l.DiaSemana == diaSemana && l.Activa);

                if (idLeccionExcluir.HasValue)
                {
                    query = query.Where(l => l.IdLeccion != idLeccionExcluir.Value);
                }

                var leccionesDelDia = await query.ToListAsync();

                // Verificar solapamiento: dos bloques se solapan si uno empieza antes de que el otro termine
                return leccionesDelDia.Any(l => 
                    (horaInicio >= l.HoraInicio && horaInicio < l.HoraFin) ||  // Inicia durante otra lección
                    (horaFin > l.HoraInicio && horaFin <= l.HoraFin) ||         // Termina durante otra lección
                    (horaInicio <= l.HoraInicio && horaFin >= l.HoraFin));      // Envuelve completamente otra lección
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al validar solapamiento de horario");
                return true; // En caso de error, asumimos que hay solapamiento por seguridad
            }
        }

        public Task<bool> ValidarHorarioValidoAsync(TimeSpan horaInicio, TimeSpan horaFin)
        {
            return Task.FromResult(horaFin > horaInicio);
        }

        public async Task<Dictionary<int, List<Leccion>>> ObtenerHorarioSemanalGrupoAsync(int idGrupo)
        {
            try
            {
                var lecciones = await ObtenerLeccionesPorGrupoAsync(idGrupo, true);
                
                // Agrupar por día de la semana
                return lecciones
                    .GroupBy(l => l.DiaSemana)
                    .ToDictionary(g => (int)g.Key, g => g.ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener horario semanal del grupo {IdGrupo}", idGrupo);
                return new Dictionary<int, List<Leccion>>();
            }
        }

        // ===== MÉTODOS ALIAS PARA COMPATIBILIDAD CON CONTROLADORES =====

        public Task<Leccion?> ObtenerHorarioPorIdAsync(int idLeccion) => ObtenerLeccionPorIdAsync(idLeccion);

        public async Task<IEnumerable<Leccion>> ObtenerTodosHorariosAsync()
        {
            try
            {
                return await _context.Lecciones
                    .Include(l => l.Grupo)
                    .Include(l => l.Materia)
                    .OrderBy(l => l.Grupo.Nombre)
                    .ThenBy(l => l.DiaSemana)
                    .ThenBy(l => l.NumeroBloque)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los horarios");
                return Enumerable.Empty<Leccion>();
            }
        }

        public Task<IEnumerable<Leccion>> ObtenerHorariosPorGrupoAsync(int idGrupo) 
            => ObtenerLeccionesPorGrupoAsync(idGrupo, true);

        public async Task<IEnumerable<Leccion>> ObtenerHorariosPorGrupoYDiaAsync(int idGrupo, DayOfWeek diaSemana)
            => await ObtenerLeccionesPorGrupoYDiaAsync(idGrupo, (int)diaSemana, true);

        public Task<IEnumerable<Leccion>> ObtenerHorariosPorMateriaAsync(int materiaId) 
            => ObtenerLeccionesPorMateriaAsync(materiaId, true);

        public async Task<IEnumerable<Leccion>> ObtenerHorariosPorDiaAsync(DayOfWeek diaSemana)
        {
            try
            {
                return await _context.Lecciones
                    .Include(l => l.Grupo)
                    .Include(l => l.Materia)
                    .Where(l => l.DiaSemana == diaSemana && l.Activa)
                    .OrderBy(l => l.Grupo.Nombre)
                    .ThenBy(l => l.NumeroBloque)
                    .ThenBy(l => l.HoraInicio)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener horarios del día {DiaSemana}", diaSemana);
                return Enumerable.Empty<Leccion>();
            }
        }

        public async Task<(bool Exito, string Mensaje, Leccion? Leccion)> CrearHorarioAsync(Leccion leccion, string usuarioId)
        {
            var resultado = await CrearLeccionAsync(leccion);
            return resultado;
        }

        public async Task<(bool Exito, string Mensaje)> ActualizarHorarioAsync(Leccion leccion, string usuarioId)
        {
            return await ActualizarLeccionAsync(leccion);
        }

        public async Task<(bool Exito, string Mensaje)> EliminarHorarioAsync(int idLeccion, string usuarioId)
        {
            return await EliminarLeccionAsync(idLeccion, eliminacionFisica: false);
        }

        public async Task<(bool Exito, string Mensaje)> ActivarDesactivarHorarioAsync(int idLeccion, bool activo, string usuarioId)
        {
            try
            {
                var leccion = await _context.Lecciones.FindAsync(idLeccion);
                if (leccion == null)
                {
                    return (false, "Lección no encontrada");
                }

                leccion.Activa = activo;
                leccion.FechaModificacion = DateTime.Now;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Lección {Accion}: ID {IdLeccion}", activo ? "activada" : "desactivada", idLeccion);
                return (true, $"Lección {(activo ? "activada" : "desactivada")} exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cambiar estado de lección {IdLeccion}", idLeccion);
                return (false, $"Error al cambiar estado: {ex.Message}");
            }
        }

        public async Task<List<string>> ValidarHorarioAsync(Leccion leccion)
        {
            var errores = new List<string>();

            try
            {
                // Validar horario válido
                if (!await ValidarHorarioValidoAsync(leccion.HoraInicio, leccion.HoraFin))
                {
                    errores.Add("La hora de fin debe ser mayor que la hora de inicio");
                }

                // Validar que el grupo exista
                var grupoExiste = await _context.GruposEstudiantes.AnyAsync(g => g.GrupoId == leccion.IdGrupo);
                if (!grupoExiste)
                {
                    errores.Add("El grupo especificado no existe");
                }

                // Validar que la materia exista
                var materiaExiste = await _context.Materias.AnyAsync(m => m.MateriaId == leccion.MateriaId);
                if (!materiaExiste)
                {
                    errores.Add("La materia especificada no existe");
                }

                // Validar número de bloque
                if (leccion.NumeroBloque < 1 || leccion.NumeroBloque > 12)
                {
                    errores.Add("El número de bloque debe estar entre 1 y 12");
                }

                // Validar que no exista duplicado
                var duplicadoQuery = _context.Lecciones
                    .Where(l => 
                        l.IdGrupo == leccion.IdGrupo && 
                        l.MateriaId == leccion.MateriaId && 
                        l.DiaSemana == leccion.DiaSemana && 
                        l.NumeroBloque == leccion.NumeroBloque);

                if (leccion.IdLeccion > 0)
                {
                    duplicadoQuery = duplicadoQuery.Where(l => l.IdLeccion != leccion.IdLeccion);
                }

                if (await duplicadoQuery.AnyAsync())
                {
                    errores.Add("Ya existe una lección con ese grupo, materia, día y bloque");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al validar horario");
                errores.Add($"Error al validar: {ex.Message}");
            }

            return errores;
        }

        public async Task<List<Leccion>> ObtenerConflictosHorarioAsync(Leccion leccion)
        {
            try
            {
                var query = _context.Lecciones
                    .Include(l => l.Grupo)
                    .Include(l => l.Materia)
                    .Where(l => 
                        l.IdGrupo == leccion.IdGrupo && 
                        l.DiaSemana == leccion.DiaSemana && 
                        l.Activa);

                if (leccion.IdLeccion > 0)
                {
                    query = query.Where(l => l.IdLeccion != leccion.IdLeccion);
                }

                var leccionesDelDia = await query.ToListAsync();

                // Filtrar por solapamiento de horarios
                var conflictos = leccionesDelDia.Where(l => 
                    (leccion.HoraInicio >= l.HoraInicio && leccion.HoraInicio < l.HoraFin) ||
                    (leccion.HoraFin > l.HoraInicio && leccion.HoraFin <= l.HoraFin) ||
                    (leccion.HoraInicio <= l.HoraInicio && leccion.HoraFin >= l.HoraFin)
                ).ToList();

                return conflictos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener conflictos de horario");
                return new List<Leccion>();
            }
        }
    }
}
