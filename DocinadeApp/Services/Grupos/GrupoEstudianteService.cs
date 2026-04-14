using Microsoft.EntityFrameworkCore;
using RubricasApp.Web.Data;
using RubricasApp.Web.Models;
using RubricasApp.Web.ViewModels.Grupos;
using RubricasApp.Web.Services.Auditoria;
using OfficeOpenXml;

namespace RubricasApp.Web.Services.Grupos
{
    public class GrupoEstudianteService : IGrupoEstudianteService
    {
        private readonly RubricasDbContext _context;
        private readonly ILogger<GrupoEstudianteService> _logger;
        private readonly IAuditoriaService _auditoriaService;

        public GrupoEstudianteService(RubricasDbContext context, ILogger<GrupoEstudianteService> logger, IAuditoriaService auditoriaService)
        {
            _context = context;
            _logger = logger;
            _auditoriaService = auditoriaService;
        }

        public async Task<IEnumerable<GrupoEstudiante>> ObtenerGruposAsync(FiltrosGrupoViewModel? filtros = null)
        {
            var query = _context.GruposEstudiantes
                .Include(g => g.PeriodoAcademico)
                .Include(g => g.CreadoPor)
                .Include(g => g.TipoGrupoCatalogo) // ?? NUEVA: Incluir catálogo de tipos
                .Include(g => g.Institucion)
                .Include(g => g.EstudianteGrupos.Where(eg => eg.Estado == EstadoAsignacion.Activo))
                .Include(g => g.GrupoMaterias.Where(gm => gm.Estado == EstadoAsignacion.Activo))
                    .ThenInclude(gm => gm.Materia)
                .AsQueryable();

            if (filtros != null)
            {
                if (filtros.PeriodoAcademicoId.HasValue)
                    query = query.Where(g => g.PeriodoAcademicoId == filtros.PeriodoAcademicoId.Value);

                if (filtros.TipoGrupo.HasValue)
                    query = query.Where(g => g.TipoGrupo == filtros.TipoGrupo.Value);

                if (filtros.Estado.HasValue)
                    query = query.Where(g => g.Estado == filtros.Estado.Value);

                if (!string.IsNullOrWhiteSpace(filtros.Nivel))
                    query = query.Where(g => g.Nivel != null && g.Nivel.Contains(filtros.Nivel));

                if (!string.IsNullOrWhiteSpace(filtros.Codigo))
                    query = query.Where(g => g.Codigo.Contains(filtros.Codigo));

                if (!string.IsNullOrWhiteSpace(filtros.Nombre))
                    query = query.Where(g => g.Nombre.Contains(filtros.Nombre));

                if (filtros.SoloConEspacio)
                    query = query.Where(g => g.CapacidadMaxima == null || 
                        g.EstudianteGrupos.Count(eg => eg.Estado == EstadoAsignacion.Activo) < g.CapacidadMaxima);

                if (filtros.MateriaId.HasValue)
                    query = query.Where(g => g.GrupoMaterias.Any(gm => gm.MateriaId == filtros.MateriaId.Value && gm.Estado == EstadoAsignacion.Activo));
                
                if (filtros.InstitucionId.HasValue)
                    query = query.Where(g => g.InstitucionId == filtros.InstitucionId.Value);
            }

            return await query.OrderBy(g => g.TipoGrupo).ThenBy(g => g.Codigo).ToListAsync();
        }

        public async Task<GrupoEstudiante?> ObtenerGrupoPorIdAsync(int grupoId)
        {
            return await _context.GruposEstudiantes
                .Include(g => g.PeriodoAcademico)
                .Include(g => g.CreadoPor)
                .Include(g => g.TipoGrupoCatalogo) // ?? NUEVA: Incluir catálogo de tipos
                .Include(g => g.Institucion)
                .Include(g => g.EstudianteGrupos.Where(eg => eg.Estado == EstadoAsignacion.Activo))
                    .ThenInclude(eg => eg.Estudiante)
                .Include(g => g.GrupoMaterias.Where(gm => gm.Estado == EstadoAsignacion.Activo))
                    .ThenInclude(gm => gm.Materia)
                .FirstOrDefaultAsync(g => g.GrupoId == grupoId);
        }

        public async Task<ResultadoOperacion<GrupoEstudiante>> CrearGrupoAsync(CrearGrupoViewModel modelo, string usuarioId)
        {
            try
            {
                // Validar c�digo �nico en el per�odo
                if (!await ValidarCodigoUnicoAsync(modelo.Codigo, modelo.PeriodoAcademicoId))
                {
                    return ResultadoOperacion<GrupoEstudiante>.Error("Ya existe un grupo con este c�digo en el per�odo acad�mico seleccionado");
                }

                var grupo = new GrupoEstudiante
                {
                    Codigo = modelo.Codigo,
                    Nombre = modelo.Nombre,
                    Descripcion = modelo.Descripcion,
                    TipoGrupo = modelo.TipoGrupo, // ?? MANTENER: Compatibilidad enum
                    IdTipoGrupo = modelo.IdTipoGrupo, // ?? USAR: ID del cat�logo
                    Nivel = modelo.Nivel,
                    CapacidadMaxima = modelo.CapacidadMaxima,
                    PeriodoAcademicoId = modelo.PeriodoAcademicoId,                    InstitucionId = modelo.InstitucionId,                    CreadoPorId = usuarioId,
                    Observaciones = modelo.Observaciones,
                    Estado = EstadoGrupo.Activo,
                    FechaCreacion = DateTime.Now
                };

                _context.GruposEstudiantes.Add(grupo);
                await _context.SaveChangesAsync();

                // Asignar materias si se especificaron
                if (modelo.MateriasSeleccionadas.Any())
                {
                    await AsignarMateriasAsync(grupo.GrupoId, modelo.MateriasSeleccionadas, usuarioId);
                }

                _logger.LogInformation("Grupo {Codigo} creado por usuario {UserId}", grupo.Codigo, usuarioId);
                return ResultadoOperacion<GrupoEstudiante>.Exito(grupo, "Grupo creado exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear grupo {Codigo}", modelo.Codigo);
                return ResultadoOperacion<GrupoEstudiante>.Error("Error interno al crear el grupo");
            }
        }

        public async Task<ResultadoOperacion<GrupoEstudiante>> ActualizarGrupoAsync(EditarGrupoViewModel modelo, string usuarioId)
        {
            try
            {
                var grupo = await _context.GruposEstudiantes.FindAsync(modelo.GrupoId);
                if (grupo == null)
                {
                    return ResultadoOperacion<GrupoEstudiante>.Error("Grupo no encontrado");
                }

                // Validar codigo unico si cambio
                if (grupo.Codigo != modelo.Codigo && !await ValidarCodigoUnicoAsync(modelo.Codigo, modelo.PeriodoAcademicoId, modelo.GrupoId))
                {
                    return ResultadoOperacion<GrupoEstudiante>.Error("Ya existe un grupo con este codigo en el periodo academico seleccionado");
                }

                // Validar capacidad si se reduce
                if (modelo.CapacidadMaxima.HasValue && grupo.CantidadEstudiantes > modelo.CapacidadMaxima.Value)
                {
                    return ResultadoOperacion<GrupoEstudiante>.Error($"No se puede reducir la capacidad a {modelo.CapacidadMaxima} porque ya hay {grupo.CantidadEstudiantes} estudiantes asignados");
                }

                grupo.Codigo = modelo.Codigo;
                grupo.Nombre = modelo.Nombre;
                grupo.Descripcion = modelo.Descripcion;
                grupo.TipoGrupo = modelo.TipoGrupo; // ?? MANTENER: Compatibilidad enum
                grupo.IdTipoGrupo = modelo.IdTipoGrupo; // ?? USAR: ID del cat�logo
                grupo.Nivel = modelo.Nivel;
                grupo.CapacidadMaxima = modelo.CapacidadMaxima;                grupo.InstitucionId = modelo.InstitucionId;                grupo.Observaciones = modelo.Observaciones;
                grupo.FechaModificacion = DateTime.Now;

                _context.GruposEstudiantes.Update(grupo);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Grupo {Codigo} actualizado por usuario {UserId}", grupo.Codigo, usuarioId);
                return ResultadoOperacion<GrupoEstudiante>.Exito(grupo, "Grupo actualizado exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar grupo {GrupoId}", modelo.GrupoId);
                return ResultadoOperacion<GrupoEstudiante>.Error("Error interno al actualizar el grupo");
            }
        }

        public async Task<ResultadoOperacion> EliminarGrupoAsync(int grupoId, string usuarioId, string? motivo = null, string? direccionIP = null, string? userAgent = null)
        {
            try
            {
                var grupo = await _context.GruposEstudiantes
                    .Include(g => g.EstudianteGrupos)
                    .Include(g => g.GrupoMaterias)
                    .FirstOrDefaultAsync(g => g.GrupoId == grupoId);

                if (grupo == null)
                {
                    await _auditoriaService.RegistrarErrorAsync(
                        TiposOperacionAuditoria.DELETE, 
                        "GruposEstudiantes", 
                        grupoId, 
                        "Intento de eliminar grupo inexistente", 
                        usuarioId, 
                        "Grupo no encontrado", 
                        motivo, 
                        direccionIP, 
                        userAgent);
                    return ResultadoOperacion.Error("Grupo no encontrado");
                }

                // Verificar si hay estudiantes asignados
                if (grupo.EstudianteGrupos.Any(eg => eg.Estado == EstadoAsignacion.Activo))
                {
                    var mensaje = "No se puede eliminar un grupo que tiene estudiantes asignados. Desasigne primero todos los estudiantes.";
                    await _auditoriaService.RegistrarErrorAsync(
                        TiposOperacionAuditoria.DELETE, 
                        "GruposEstudiantes", 
                        grupoId, 
                        "Intento de eliminar grupo con estudiantes asignados", 
                        usuarioId, 
                        mensaje, 
                        motivo, 
                        direccionIP, 
                        userAgent);
                    return ResultadoOperacion.Error(mensaje);
                }

                // Guardar datos anteriores para auditoría
                var datosAnteriores = new
                {
                    grupo.GrupoId,
                    grupo.Codigo,
                    grupo.Nombre,
                    grupo.Descripcion,
                    grupo.TipoGrupo,
                    grupo.Nivel,
                    grupo.CapacidadMaxima,
                    grupo.Estado,
                    grupo.FechaCreacion,
                    grupo.Observaciones,
                    CantidadMaterias = grupo.GrupoMaterias.Count(gm => gm.Estado == EstadoAsignacion.Activo)
                };

                // Marcar como inactivo en lugar de eliminar físicamente
                grupo.Estado = EstadoGrupo.Inactivo;
                grupo.FechaModificacion = DateTime.Now;

                // Inactivar relaciones con materias
                foreach (var grupoMateria in grupo.GrupoMaterias.Where(gm => gm.Estado == EstadoAsignacion.Activo))
                {
                    grupoMateria.Estado = EstadoAsignacion.Inactivo;
                }

                _context.GruposEstudiantes.Update(grupo);
                await _context.SaveChangesAsync();

                // Registrar auditoría exitosa
                await _auditoriaService.RegistrarExitoAsync(
                    TiposOperacionAuditoria.DELETE, 
                    "GruposEstudiantes", 
                    grupoId, 
                    $"Grupo '{grupo.Codigo} - {grupo.Nombre}' eliminado (inactivado) exitosamente", 
                    usuarioId, 
                    motivo, 
                    datosAnteriores, 
                    new { Estado = "Inactivo", FechaEliminacion = DateTime.Now }, 
                    direccionIP, 
                    userAgent);

                _logger.LogInformation("✅ Grupo {Codigo} eliminado (inactivado) por usuario {UserId} con motivo: {Motivo}", 
                    grupo.Codigo, usuarioId, motivo ?? "No especificado");
                
                return ResultadoOperacion.Exito("Grupo eliminado exitosamente");
            }
            catch (Exception ex)
            {
                await _auditoriaService.RegistrarErrorAsync(
                    TiposOperacionAuditoria.DELETE, 
                    "GruposEstudiantes", 
                    grupoId, 
                    "Error interno al eliminar grupo", 
                    usuarioId, 
                    ex.Message, 
                    motivo, 
                    direccionIP, 
                    userAgent);

                _logger.LogError(ex, "💥 Error al eliminar grupo {GrupoId}", grupoId);
                return ResultadoOperacion.Error("Error interno al eliminar el grupo");
            }
        }

        public async Task<ResultadoOperacion> AsignarEstudiantesAsync(int grupoId, List<int> estudianteIds, string usuarioId, string? motivo = null, bool esGrupoPrincipal = true)
        {
            try
            {
                var grupo = await _context.GruposEstudiantes.FindAsync(grupoId);
                if (grupo == null)
                {
                    return ResultadoOperacion.Error("Grupo no encontrado");
                }

                // Validar capacidad
                if (!await ValidarCapacidadGrupoAsync(grupoId, estudianteIds.Count))
                {
                    return ResultadoOperacion.Error("La asignación excederá la capacidad máxima del grupo");
                }

                // Validar que los estudiantes existan y no estén ya asignados
                var erroresValidacion = await ValidarAsignacionEstudiantesAsync(grupoId, estudianteIds);
                if (erroresValidacion.Any())
                {
                    return ResultadoOperacion.Error("Errores de validación encontrados", erroresValidacion);
                }

                var asignaciones = new List<EstudianteGrupo>();
                foreach (var estudianteId in estudianteIds)
                {
                    var asignacion = new EstudianteGrupo
                    {
                        EstudianteId = estudianteId,
                        GrupoId = grupoId,
                        AsignadoPorId = usuarioId,
                        MotivoAsignacion = motivo,
                        EsGrupoPrincipal = esGrupoPrincipal,
                        Estado = EstadoAsignacion.Activo,
                        FechaAsignacion = DateTime.Now
                    };
                    asignaciones.Add(asignacion);
                }

                _context.EstudianteGrupos.AddRange(asignaciones);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Asignados {Count} estudiantes al grupo {GrupoId} por usuario {UserId}", estudianteIds.Count, grupoId, usuarioId);
                return ResultadoOperacion.Exito($"Se asignaron exitosamente {estudianteIds.Count} estudiantes al grupo");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al asignar estudiantes al grupo {GrupoId}", grupoId);
                return ResultadoOperacion.Error("Error interno al asignar estudiantes");
            }
        }

        public async Task<ResultadoOperacion> DesasignarEstudianteAsync(int grupoId, int estudianteId, string usuarioId, string? motivo = null)
        {
            try
            {
                var asignacion = await _context.EstudianteGrupos
                    .FirstOrDefaultAsync(eg => eg.GrupoId == grupoId && eg.EstudianteId == estudianteId && eg.Estado == EstadoAsignacion.Activo);

                if (asignacion == null)
                {
                    return ResultadoOperacion.Error("El estudiante no est� asignado a este grupo");
                }

                asignacion.Estado = EstadoAsignacion.Inactivo;
                asignacion.FechaDesasignacion = DateTime.Now;

                _context.EstudianteGrupos.Update(asignacion);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Estudiante {EstudianteId} desasignado del grupo {GrupoId} por usuario {UserId}", estudianteId, grupoId, usuarioId);
                return ResultadoOperacion.Exito("Estudiante desasignado exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al desasignar estudiante {EstudianteId} del grupo {GrupoId}", estudianteId, grupoId);
                return ResultadoOperacion.Error("Error interno al desasignar estudiante");
            }
        }

        public async Task<ResultadoOperacion> TransferirEstudianteAsync(int estudianteId, int grupoOrigenId, int grupoDestinoId, string usuarioId, string? motivo = null)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Desasignar del grupo origen
                var resultadoDesasignacion = await DesasignarEstudianteAsync(grupoOrigenId, estudianteId, usuarioId, motivo);
                if (!resultadoDesasignacion.Exitoso)
                {
                    return resultadoDesasignacion;
                }

                // Asignar al grupo destino
                var resultadoAsignacion = await AsignarEstudiantesAsync(grupoDestinoId, new List<int> { estudianteId }, usuarioId, motivo);
                if (!resultadoAsignacion.Exitoso)
                {
                    return resultadoAsignacion;
                }

                await transaction.CommitAsync();
                _logger.LogInformation("Estudiante {EstudianteId} transferido del grupo {GrupoOrigenId} al grupo {GrupoDestinoId} por usuario {UserId}", 
                    estudianteId, grupoOrigenId, grupoDestinoId, usuarioId);
                return ResultadoOperacion.Exito("Estudiante transferido exitosamente");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error al transferir estudiante {EstudianteId} del grupo {GrupoOrigenId} al {GrupoDestinoId}", 
                    estudianteId, grupoOrigenId, grupoDestinoId);
                return ResultadoOperacion.Error("Error interno al transferir estudiante");
            }
        }

        public async Task<ResultadoOperacion> AsignarMateriasAsync(int grupoId, List<int> materiaIds, string usuarioId)
        {
            try
            {
                var grupo = await _context.GruposEstudiantes.FindAsync(grupoId);
                if (grupo == null)
                {
                    return ResultadoOperacion.Error("Grupo no encontrado");
                }

                // Verificar que las materias no est�n ya asignadas
                var materiasExistentes = await _context.GrupoMaterias
                    .Where(gm => gm.GrupoId == grupoId && materiaIds.Contains(gm.MateriaId) && gm.Estado == EstadoAsignacion.Activo)
                    .Select(gm => gm.MateriaId)
                    .ToListAsync();

                var nuevasMaterias = materiaIds.Except(materiasExistentes).ToList();
                if (!nuevasMaterias.Any())
                {
                    return ResultadoOperacion.Error("Todas las materias seleccionadas ya est�n asignadas al grupo");
                }

                var asignaciones = nuevasMaterias.Select(materiaId => new GrupoMateria
                {
                    GrupoId = grupoId,
                    MateriaId = materiaId,
                    Estado = EstadoAsignacion.Activo,
                    FechaAsignacion = DateTime.Now
                }).ToList();

                _context.GrupoMaterias.AddRange(asignaciones);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Asignadas {Count} materias al grupo {GrupoId}", nuevasMaterias.Count, grupoId);
                return ResultadoOperacion.Exito($"Se asignaron {nuevasMaterias.Count} materias al grupo");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al asignar materias al grupo {GrupoId}", grupoId);
                return ResultadoOperacion.Error("Error interno al asignar materias");
            }
        }

        public async Task<ResultadoOperacion> DesasignarMateriaAsync(int grupoId, int materiaId, string usuarioId)
        {
            try
            {
                var asignacion = await _context.GrupoMaterias
                    .FirstOrDefaultAsync(gm => gm.GrupoId == grupoId && gm.MateriaId == materiaId && gm.Estado == EstadoAsignacion.Activo);

                if (asignacion == null)
                {
                    return ResultadoOperacion.Error("La materia no est� asignada a este grupo");
                }

                asignacion.Estado = EstadoAsignacion.Inactivo;
                _context.GrupoMaterias.Update(asignacion);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Materia {MateriaId} desasignada del grupo {GrupoId}", materiaId, grupoId);
                return ResultadoOperacion.Exito("Materia desasignada exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al desasignar materia {MateriaId} del grupo {GrupoId}", materiaId, grupoId);
                return ResultadoOperacion.Error("Error interno al desasignar materia");
            }
        }

        public async Task<List<EstudianteDisponibleViewModel>> ObtenerEstudiantesDisponiblesAsync(int grupoId, int periodoAcademicoId, string? filtroNombre = null)
        {
            var query = _context.Estudiantes
                .Where(e => e.PeriodoAcademicoId == periodoAcademicoId)
                .Where(e => !_context.EstudianteGrupos.Any(eg => eg.EstudianteId == e.IdEstudiante && eg.GrupoId == grupoId && eg.Estado == EstadoAsignacion.Activo))
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filtroNombre))
            {
                query = query.Where(e => e.Nombre.Contains(filtroNombre) || e.Apellidos.Contains(filtroNombre) || e.NumeroId.Contains(filtroNombre));
            }

            return await query
                .Select(e => new EstudianteDisponibleViewModel
                {
                    EstudianteId = e.IdEstudiante,
                    NumeroId = e.NumeroId,
                    NombreCompleto = e.NombreCompleto,
                    Email = e.DireccionCorreo,
                    GrupoActual = _context.EstudianteGrupos
                        .Where(eg => eg.EstudianteId == e.IdEstudiante && eg.EsGrupoPrincipal && eg.Estado == EstadoAsignacion.Activo)
                        .Select(eg => eg.Grupo!.Codigo)
                        .FirstOrDefault(),
                    TieneGrupoPrincipal = _context.EstudianteGrupos
                        .Any(eg => eg.EstudianteId == e.IdEstudiante && eg.EsGrupoPrincipal && eg.Estado == EstadoAsignacion.Activo)
                })
                .OrderBy(e => e.NombreCompleto)
                .ToListAsync();
        }

        public async Task<List<GrupoEstudiante>> ObtenerGruposPorEstudianteAsync(int estudianteId)
        {
            return await _context.EstudianteGrupos
                .Where(eg => eg.EstudianteId == estudianteId && eg.Estado == EstadoAsignacion.Activo)
                .Select(eg => eg.Grupo!)
                .Include(g => g.PeriodoAcademico)
                .ToListAsync();
        }

        public async Task<List<GrupoEstudiante>> ObtenerGruposPorMateriaAsync(int materiaId, int periodoAcademicoId)
        {
            return await _context.GrupoMaterias
                .Where(gm => gm.MateriaId == materiaId && gm.Estado == EstadoAsignacion.Activo)
                .Where(gm => gm.Grupo!.PeriodoAcademicoId == periodoAcademicoId)
                .Select(gm => gm.Grupo!)
                .Include(g => g.EstudianteGrupos.Where(eg => eg.Estado == EstadoAsignacion.Activo))
                .ToListAsync();
        }

        public async Task<GrupoEstadisticasViewModel> ObtenerEstadisticasAsync(int? periodoAcademicoId = null)
        {
            var query = _context.GruposEstudiantes.AsQueryable();
            if (periodoAcademicoId.HasValue)
            {
                query = query.Where(g => g.PeriodoAcademicoId == periodoAcademicoId.Value);
            }

            var grupos = await query
                .Include(g => g.EstudianteGrupos.Where(eg => eg.Estado == EstadoAsignacion.Activo))
                .ToListAsync();

            var totalEstudiantes = await _context.Estudiantes
                .Where(e => !periodoAcademicoId.HasValue || e.PeriodoAcademicoId == periodoAcademicoId.Value)
                .CountAsync();

            var estudiantesConGrupo = await _context.EstudianteGrupos
                .Where(eg => eg.Estado == EstadoAsignacion.Activo)
                .Where(eg => !periodoAcademicoId.HasValue || eg.Grupo!.PeriodoAcademicoId == periodoAcademicoId.Value)
                .Select(eg => eg.EstudianteId)
                .Distinct()
                .CountAsync();

            return new GrupoEstadisticasViewModel
            {
                TotalGrupos = grupos.Count,
                GruposActivos = grupos.Count(g => g.Estado == EstadoGrupo.Activo),
                TotalEstudiantes = totalEstudiantes,
                EstudiantesSinGrupo = totalEstudiantes - estudiantesConGrupo,
                GruposPorTipo = grupos.GroupBy(g => g.TipoGrupo).ToDictionary(g => g.Key, g => g.Count()),
                GruposPorEstado = grupos.GroupBy(g => g.Estado).ToDictionary(g => g.Key, g => g.Count()),
                GruposConMasEstudiantes = grupos
                    .Where(g => g.CantidadEstudiantes > 0)
                    .OrderByDescending(g => g.CantidadEstudiantes)
                    .Take(5)
                    .Select(g => new GrupoConMasEstudiantesViewModel
                    {
                        Codigo = g.Codigo,
                        Nombre = g.Nombre,
                        CantidadEstudiantes = g.CantidadEstudiantes,
                        CapacidadMaxima = g.CapacidadMaxima,
                        PorcentajeOcupacion = g.CapacidadMaxima.HasValue ? (double)g.CantidadEstudiantes / g.CapacidadMaxima.Value * 100 : 0
                    })
                    .ToList(),
                GruposCompletos = grupos
                    .Where(g => g.EstaCompleto)
                    .Select(g => new GrupoCompletoViewModel
                    {
                        Codigo = g.Codigo,
                        Nombre = g.Nombre,
                        CapacidadMaxima = g.CapacidadMaxima!.Value
                    })
                    .ToList()
            };
        }

        public async Task<bool> ValidarCapacidadGrupoAsync(int grupoId, int cantidadNuevosEstudiantes)
        {
            var grupo = await _context.GruposEstudiantes
                .Include(g => g.EstudianteGrupos.Where(eg => eg.Estado == EstadoAsignacion.Activo))
                .FirstOrDefaultAsync(g => g.GrupoId == grupoId);

            if (grupo?.CapacidadMaxima == null)
                return true; // Sin l�mite de capacidad

            return grupo.CantidadEstudiantes + cantidadNuevosEstudiantes <= grupo.CapacidadMaxima.Value;
        }

        public async Task<bool> ValidarCodigoUnicoAsync(string codigo, int periodoAcademicoId, int? grupoIdExcluir = null)
        {
            var query = _context.GruposEstudiantes
                .Where(g => g.Codigo == codigo && g.PeriodoAcademicoId == periodoAcademicoId);

            if (grupoIdExcluir.HasValue)
            {
                query = query.Where(g => g.GrupoId != grupoIdExcluir.Value);
            }

            return !await query.AnyAsync();
        }

        public async Task<List<string>> ValidarAsignacionEstudiantesAsync(int grupoId, List<int> estudianteIds)
        {
            var errores = new List<string>();

            // Verificar que los estudiantes existan
            var estudiantesExistentes = await _context.Estudiantes
                .Where(e => estudianteIds.Contains(e.IdEstudiante))
                .Select(e => e.IdEstudiante)
                .ToListAsync();

            var estudiantesNoExistentes = estudianteIds.Except(estudiantesExistentes).ToList();
            if (estudiantesNoExistentes.Any())
            {
                errores.Add($"Los siguientes estudiantes no existen: {string.Join(", ", estudiantesNoExistentes)}");
            }

            // Verificar que no est�n ya asignados al grupo
            var estudiantesYaAsignados = await _context.EstudianteGrupos
                .Where(eg => eg.GrupoId == grupoId && estudianteIds.Contains(eg.EstudianteId) && eg.Estado == EstadoAsignacion.Activo)
                .Select(eg => eg.EstudianteId)
                .ToListAsync();

            if (estudiantesYaAsignados.Any())
            {
                errores.Add($"Los siguientes estudiantes ya est�n asignados al grupo: {string.Join(", ", estudiantesYaAsignados)}");
            }

            return errores;
        }

        public async Task<byte[]> ExportarGruposExcelAsync(FiltrosGrupoViewModel? filtros = null)
        {
            var grupos = await ObtenerGruposAsync(filtros);

            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Grupos");

            // Headers
            worksheet.Cells[1, 1].Value = "C�digo";
            worksheet.Cells[1, 2].Value = "Nombre";
            worksheet.Cells[1, 3].Value = "Tipo";
            worksheet.Cells[1, 4].Value = "Nivel";
            worksheet.Cells[1, 5].Value = "Capacidad";
            worksheet.Cells[1, 6].Value = "Estudiantes";
            worksheet.Cells[1, 7].Value = "Estado";
            worksheet.Cells[1, 8].Value = "Per�odo";
            worksheet.Cells[1, 9].Value = "Fecha Creaci�n";

            // Data
            int row = 2;
            foreach (var grupo in grupos)
            {
                worksheet.Cells[row, 1].Value = grupo.Codigo;
                worksheet.Cells[row, 2].Value = grupo.Nombre;
                worksheet.Cells[row, 3].Value = grupo.TipoGrupo.ToString();
                worksheet.Cells[row, 4].Value = grupo.Nivel;
                worksheet.Cells[row, 5].Value = grupo.CapacidadMaxima?.ToString() ?? "Sin l�mite";
                worksheet.Cells[row, 6].Value = grupo.CantidadEstudiantes;
                worksheet.Cells[row, 7].Value = grupo.Estado.ToString();
                worksheet.Cells[row, 8].Value = grupo.PeriodoAcademico?.Nombre;
                worksheet.Cells[row, 9].Value = grupo.FechaCreacion.ToString("dd/MM/yyyy");
                row++;
            }

            // Format
            worksheet.Cells[1, 1, 1, 9].Style.Font.Bold = true;
            worksheet.Cells.AutoFitColumns();

            return await package.GetAsByteArrayAsync();
        }

        public async Task<byte[]> ExportarEstudiantesPorGrupoExcelAsync(int grupoId)
        {
            var grupo = await ObtenerGrupoPorIdAsync(grupoId);
            if (grupo == null)
                throw new ArgumentException("Grupo no encontrado", nameof(grupoId));

            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add($"Estudiantes - {grupo.Codigo}");

            // Headers
            worksheet.Cells[1, 1].Value = "N�mero ID";
            worksheet.Cells[1, 2].Value = "Nombre Completo";
            worksheet.Cells[1, 3].Value = "Email";
            worksheet.Cells[1, 4].Value = "Fecha Asignaci�n";
            worksheet.Cells[1, 5].Value = "Grupo Principal";
            worksheet.Cells[1, 6].Value = "Estado";

            // Data
            int row = 2;
            foreach (var estudianteGrupo in grupo.EstudianteGrupos.Where(eg => eg.Estado == EstadoAsignacion.Activo))
            {
                worksheet.Cells[row, 1].Value = estudianteGrupo.Estudiante?.NumeroId;
                worksheet.Cells[row, 2].Value = estudianteGrupo.Estudiante?.NombreCompleto;
                worksheet.Cells[row, 3].Value = estudianteGrupo.Estudiante?.DireccionCorreo;
                worksheet.Cells[row, 4].Value = estudianteGrupo.FechaAsignacion.ToString("dd/MM/yyyy");
                worksheet.Cells[row, 5].Value = estudianteGrupo.EsGrupoPrincipal ? "S�" : "No";
                worksheet.Cells[row, 6].Value = estudianteGrupo.Estado.ToString();
                row++;
            }

            // Format
            worksheet.Cells[1, 1, 1, 6].Style.Font.Bold = true;
            worksheet.Cells.AutoFitColumns();

            return await package.GetAsByteArrayAsync();
        }
    }
}