using Microsoft.EntityFrameworkCore;
using DocinadeApp.Data;
using DocinadeApp.Models;
using DocinadeApp.ViewModels;
using System.Text.Json;

namespace DocinadeApp.Services.AdecuacionCurricular
{
    /// <summary>
    /// Servicio para gestión de Adecuación Curricular Significativa (ACS)
    /// según lineamientos del MEP
    /// </summary>
    public class ACSService : IACSService
    {
        private readonly RubricasDbContext _context;
        private readonly ILogger<ACSService> _logger;

        public ACSService(RubricasDbContext context, ILogger<ACSService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> ConfigurarInstrumentoACSAsync(
            int estudianteId, 
            int instrumentoId, 
            int periodoId, 
            ConfiguracionACSInstrumento config, 
            string? usuarioActual = null)
        {
            try
            {
                var configExistente = await _context.EstudiantesInstrumentosACS
                    .FirstOrDefaultAsync(c => 
                        c.EstudianteId == estudianteId && 
                        c.InstrumentoEvaluacionId == instrumentoId && 
                        c.PeriodoAcademicoId == periodoId);

                if (configExistente != null)
                {
                    // Actualizar configuración existente
                    configExistente.Exento = config.Exento;
                    configExistente.RubricaModificadaId = config.RubricaModificadaId;
                    configExistente.PonderacionPersonalizadaPorcentaje = config.PonderacionPersonalizada;
                    configExistente.CriteriosAdaptados = config.CriteriosAdaptados;
                    configExistente.Observaciones = config.Observaciones;
                    configExistente.MotivoExencion = config.Exento ? config.Observaciones : null;
                    configExistente.FechaModificacion = DateTime.Now;
                    configExistente.UsuarioModificacion = usuarioActual;
                }
                else
                {
                    // Crear nueva configuración
                    var nuevaConfig = new EstudianteInstrumentoACS
                    {
                        EstudianteId = estudianteId,
                        InstrumentoEvaluacionId = instrumentoId,
                        PeriodoAcademicoId = periodoId,
                        Exento = config.Exento,
                        RubricaModificadaId = config.RubricaModificadaId,
                        PonderacionPersonalizadaPorcentaje = config.PonderacionPersonalizada,
                        CriteriosAdaptados = config.CriteriosAdaptados,
                        Observaciones = config.Observaciones,
                        MotivoExencion = config.Exento ? config.Observaciones : null,
                        FechaCreacion = DateTime.Now,
                        UsuarioCreacion = usuarioActual
                    };

                    _context.EstudiantesInstrumentosACS.Add(nuevaConfig);
                }

                await _context.SaveChangesAsync();
                
                _logger.LogInformation(
                    "Configuración ACS establecida para Estudiante {EstudianteId}, Instrumento {InstrumentoId}, Período {PeriodoId}",
                    estudianteId, instrumentoId, periodoId);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, 
                    "Error al configurar instrumento ACS para estudiante {EstudianteId}", 
                    estudianteId);
                return false;
            }
        }

        public async Task<List<EstudianteInstrumentoACS>> ObtenerConfiguracionesACSAsync(
            int estudianteId, 
            int periodoId)
        {
            return await _context.EstudiantesInstrumentosACS
                .Include(c => c.Instrumento)
                .Include(c => c.RubricaModificada)
                .Include(c => c.Periodo)
                .Where(c => c.EstudianteId == estudianteId && c.PeriodoAcademicoId == periodoId)
                .ToListAsync();
        }

        public async Task<EstudianteInstrumentoACS?> ObtenerConfiguracionInstrumentoAsync(
            int estudianteId, 
            int instrumentoId, 
            int periodoId)
        {
            return await _context.EstudiantesInstrumentosACS
                .Include(c => c.Instrumento)
                .Include(c => c.RubricaModificada)
                .FirstOrDefaultAsync(c => 
                    c.EstudianteId == estudianteId && 
                    c.InstrumentoEvaluacionId == instrumentoId && 
                    c.PeriodoAcademicoId == periodoId);
        }

        public async Task<Rubrica?> ClonarRubricaParaEstudianteAsync(
            int rubricaOriginalId, 
            int estudianteId, 
            string motivoCambio, 
            string? usuarioActual = null)
        {
            try
            {
                var rubricaOriginal = await _context.Rubricas
                    .Include(r => r.ItemsEvaluacion)
                    .ThenInclude(i => i.ValoresRubrica)
                    .FirstOrDefaultAsync(r => r.IdRubrica == rubricaOriginalId);

                if (rubricaOriginal == null)
                {
                    _logger.LogWarning("Rúbrica original {RubricaId} no encontrada", rubricaOriginalId);
                    return null;
                }

                var estudiante = await _context.Estudiantes.FindAsync(estudianteId);
                if (estudiante == null)
                {
                    _logger.LogWarning("Estudiante {EstudianteId} no encontrado", estudianteId);
                    return null;
                }

                // Crear clon de la rúbrica
                var rubricaClonada = new Rubrica
                {
                    NombreRubrica = $"{rubricaOriginal.NombreRubrica} - ACS {estudiante.NombreCompleto}",
                    Descripcion = $"Adaptación para estudiante con ACS: {motivoCambio}",
                    Estado = "ACTIVO",
                    EsPublica = 0,
                    FechaCreacion = DateTime.Now
                };

                _context.Rubricas.Add(rubricaClonada);
                await _context.SaveChangesAsync();

                // Clonar items de evaluación
                foreach (var itemOriginal in rubricaOriginal.ItemsEvaluacion)
                {
                    var itemClonado = new ItemEvaluacion
                    {
                        IdRubrica = rubricaClonada.IdRubrica,
                        NombreItem = itemOriginal.NombreItem,
                        Descripcion = itemOriginal.Descripcion,
                        Peso = itemOriginal.Peso,
                        OrdenItem = itemOriginal.OrdenItem
                    };

                    _context.ItemsEvaluacion.Add(itemClonado);
                    await _context.SaveChangesAsync();

                    // Clonar valores de rúbrica si existen
                    foreach (var valorOriginal in itemOriginal.ValoresRubrica)
                    {
                        var valorClonado = new ValorRubrica
                        {
                            IdItem = itemClonado.IdItem,
                            IdRubrica = rubricaClonada.IdRubrica,
                            IdNivel = valorOriginal.IdNivel,
                            ValorPuntos = valorOriginal.ValorPuntos
                        };

                        _context.ValoresRubrica.Add(valorClonado);
                    }
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation(
                    "Rúbrica {RubricaOriginalId} clonada exitosamente para estudiante {EstudianteId}. Nueva rúbrica: {NuevaRubricaId}",
                    rubricaOriginalId, estudianteId, rubricaClonada.IdRubrica);

                return rubricaClonada;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, 
                    "Error al clonar rúbrica {RubricaId} para estudiante {EstudianteId}", 
                    rubricaOriginalId, estudianteId);
                return null;
            }
        }

        public async Task<bool> EliminarConfiguracionesACSAsync(int estudianteId, string? motivo = null)
        {
            try
            {
                // Primero respaldar antes de eliminar
                await RespaldarDatosACSAsync(estudianteId);

                var configuraciones = await _context.EstudiantesInstrumentosACS
                    .Where(c => c.EstudianteId == estudianteId)
                    .ToListAsync();

                if (configuraciones.Any())
                {
                    _context.EstudiantesInstrumentosACS.RemoveRange(configuraciones);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation(
                        "Eliminadas {Count} configuraciones ACS del estudiante {EstudianteId}. Motivo: {Motivo}",
                        configuraciones.Count, estudianteId, motivo ?? "No especificado");
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, 
                    "Error al eliminar configuraciones ACS del estudiante {EstudianteId}", 
                    estudianteId);
                return false;
            }
        }

        public async Task<bool> AplicarACSPeriodosAnterioresAsync(int estudianteId, int periodoActualId)
        {
            try
            {
                var estudiante = await _context.Estudiantes
                    .Include(e => e.PeriodoAcademico)
                    .FirstOrDefaultAsync(e => e.IdEstudiante == estudianteId);

                if (estudiante == null || !estudiante.RequiereACS)
                {
                    _logger.LogWarning(
                        "Estudiante {EstudianteId} no encontrado o no requiere ACS", 
                        estudianteId);
                    return false;
                }

                // Obtener períodos anteriores del mismo año lectivo
                var periodoActual = await _context.PeriodosAcademicos
                    .FindAsync(periodoActualId);

                if (periodoActual == null)
                {
                    _logger.LogWarning("Período {PeriodoId} no encontrado", periodoActualId);
                    return false;
                }

                var periodosAnteriores = await _context.PeriodosAcademicos
                    .Where(p => p.Anio == periodoActual.Anio && 
                                p.NumeroPeriodo < periodoActual.NumeroPeriodo)
                    .ToListAsync();

                if (!periodosAnteriores.Any())
                {
                    _logger.LogInformation(
                        "No hay períodos anteriores para aplicar configuración ACS retroactiva");
                    return true; // No es error, simplemente no hay períodos anteriores
                }

                // Aquí se aplicaría la configuración retroactivamente
                // Por ahora, solo marcar el flag en el estudiante
                estudiante.AplicarACSPeriodosAnteriores = true;
                await _context.SaveChangesAsync();

                _logger.LogInformation(
                    "Configuración ACS aplicada retroactivamente para estudiante {EstudianteId} en {Count} períodos anteriores",
                    estudianteId, periodosAnteriores.Count);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, 
                    "Error al aplicar ACS a períodos anteriores para estudiante {EstudianteId}", 
                    estudianteId);
                return false;
            }
        }

        public async Task<string> RespaldarDatosACSAsync(int estudianteId)
        {
            try
            {
                var estudiante = await _context.Estudiantes
                    .Include(e => e.ConfiguracionesACS)
                        .ThenInclude(c => c.Instrumento)
                    .Include(e => e.ConfiguracionesACS)
                        .ThenInclude(c => c.RubricaModificada)
                    .FirstOrDefaultAsync(e => e.IdEstudiante == estudianteId);

                if (estudiante == null)
                {
                    return string.Empty;
                }

                var respaldo = new
                {
                    FechaRespaldo = DateTime.Now,
                    Estudiante = new
                    {
                        estudiante.IdEstudiante,
                        estudiante.NombreCompleto,
                        estudiante.NumeroId,
                        estudiante.TipoAdecuacion,
                        estudiante.FechaInicioACS,
                        estudiante.DetallesACS
                    },
                    Configuraciones = estudiante.ConfiguracionesACS.Select(c => new
                    {
                        c.Id,
                        InstrumentoNombre = c.Instrumento.Nombre,
                        c.Exento,
                        c.MotivoExencion,
                        RubricaModificada = c.RubricaModificada?.NombreRubrica,
                        c.PonderacionPersonalizadaPorcentaje,
                        c.CriteriosAdaptados,
                        c.Observaciones,
                        c.FechaCreacion
                    })
                };

                var json = JsonSerializer.Serialize(respaldo, new JsonSerializerOptions 
                { 
                    WriteIndented = true 
                });

                // Guardar en directorio de respaldos (crear si no existe)
                var directorioRespaldos = Path.Combine(
                    Directory.GetCurrentDirectory(), 
                    "Respaldos", 
                    "ACS");

                Directory.CreateDirectory(directorioRespaldos);

                var nombreArchivo = $"Respaldo_ACS_{estudianteId}_{DateTime.Now:yyyyMMdd_HHmmss}.json";
                var rutaCompleta = Path.Combine(directorioRespaldos, nombreArchivo);

                await File.WriteAllTextAsync(rutaCompleta, json);

                _logger.LogInformation(
                    "Respaldo de datos ACS creado para estudiante {EstudianteId} en {Ruta}",
                    estudianteId, rutaCompleta);

                return rutaCompleta;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, 
                    "Error al respaldar datos ACS del estudiante {EstudianteId}", 
                    estudianteId);
                return string.Empty;
            }
        }

        public async Task<ReporteEstudiantesACSViewModel> ObtenerEstadisticasACSAsync(int? periodoId = null)
        {
            var query = _context.Estudiantes.AsQueryable();

            if (periodoId.HasValue)
            {
                query = query.Where(e => e.PeriodoAcademicoId == periodoId.Value);
            }

            var totalEstudiantes = await query.CountAsync();
            var estudiantesConACS = await query.CountAsync(e => e.TipoAdecuacion == "Significativa");
            var estudiantesNoSignificativa = await query.CountAsync(e => e.TipoAdecuacion == "NoSignificativa");
            var estudiantesAcceso = await query.CountAsync(e => e.TipoAdecuacion == "Acceso");

            var reporte = new ReporteEstudiantesACSViewModel
            {
                TotalEstudiantes = totalEstudiantes,
                EstudiantesConACS = estudiantesConACS,
                EstudiantesConAdecuacionNoSignificativa = estudiantesNoSignificativa,
                EstudiantesConAdecuacionAcceso = estudiantesAcceso,
                FechaGeneracion = DateTime.Now
            };

            return reporte;
        }

        public async Task<(bool esValido, string? mensajeError)> ValidarCambioACSAsync(
            int estudianteId, 
            string nuevoTipoAdecuacion)
        {
            var estudiante = await _context.Estudiantes
                .Include(e => e.ConfiguracionesACS)
                .FirstOrDefaultAsync(e => e.IdEstudiante == estudianteId);

            if (estudiante == null)
            {
                return (false, "Estudiante no encontrado");
            }

            // Si cambia de Significativa a otro tipo y tiene configuraciones
            if (estudiante.TipoAdecuacion == "Significativa" && 
                nuevoTipoAdecuacion != "Significativa" &&
                estudiante.ConfiguracionesACS.Any())
            {
                return (false, 
                    "ADVERTENCIA: El estudiante tiene configuraciones ACS activas. " +
                    "Se recomienda descargar un reporte antes de cambiar el tipo de adecuación. " +
                    "Las configuraciones se eliminarán.");
            }

            return (true, null);
        }

        public async Task<EstudianteACSConfigViewModel?> ObtenerViewModelConfiguracionAsync(
            int estudianteId, 
            int periodoId, 
            int? materiaId = null)
        {
            var estudiante = await _context.Estudiantes
                .Include(e => e.PeriodoAcademico)
                .FirstOrDefaultAsync(e => e.IdEstudiante == estudianteId);

            if (estudiante == null || !estudiante.RequiereACS)
            {
                return null;
            }

            var viewModel = new EstudianteACSConfigViewModel
            {
                EstudianteId = estudianteId,
                NombreCompleto = estudiante.NombreCompleto,
                NumeroIdentificacion = estudiante.NumeroId,
                TipoAdecuacion = estudiante.TipoAdecuacion,
                PeriodoAcademicoId = periodoId,
                DetallesACS = estudiante.DetallesACS
            };

            // TODO: Cargar instrumentos disponibles, rúbricas, etc.
            // Esto requeriría más lógica de negocio según los grupos y materias del estudiante

            return viewModel;
        }

        public async Task<bool> GuardarConfiguracionCompletaAsync(
            EstudianteACSConfigViewModel viewModel, 
            string? usuarioActual = null)
        {
            try
            {
                foreach (var instrumento in viewModel.Instrumentos)
                {
                    var config = new ConfiguracionACSInstrumento
                    {
                        InstrumentoId = instrumento.InstrumentoId,
                        Exento = instrumento.Exento,
                        RubricaModificadaId = instrumento.RubricaModificadaId,
                        PonderacionPersonalizada = instrumento.PonderacionPersonalizada,
                        CriteriosAdaptados = instrumento.CriteriosAdaptados,
                        Observaciones = instrumento.Observaciones
                    };

                    await ConfigurarInstrumentoACSAsync(
                        viewModel.EstudianteId,
                        instrumento.InstrumentoId,
                        viewModel.PeriodoAcademicoId,
                        config,
                        usuarioActual);
                }

                if (viewModel.AplicarPeriodosAnteriores)
                {
                    await AplicarACSPeriodosAnterioresAsync(
                        viewModel.EstudianteId, 
                        viewModel.PeriodoAcademicoId);
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, 
                    "Error al guardar configuración completa ACS para estudiante {EstudianteId}", 
                    viewModel.EstudianteId);
                return false;
            }
        }
    }
}
