using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RubricasApp.Web.Data;
using RubricasApp.Web.Models;
using RubricasApp.Web.Models.Identity;

namespace RubricasApp.Web.Services.Auditoria
{
    /// <summary>
    /// Servicio para manejar las operaciones de auditoría del sistema
    /// </summary>
    public class AuditoriaService : IAuditoriaService
    {
        private readonly RubricasDbContext _context;
        private readonly ILogger<AuditoriaService> _logger;

        public AuditoriaService(RubricasDbContext context, ILogger<AuditoriaService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task RegistrarOperacionAsync(string tipoOperacion, string tablaAfectada, int registroId, 
            string descripcion, string usuarioId, string? motivo = null, 
            object? datosAnteriores = null, object? datosNuevos = null,
            string? direccionIP = null, string? userAgent = null)
        {
            await RegistrarExitoAsync(tipoOperacion, tablaAfectada, registroId, descripcion, usuarioId, 
                motivo, datosAnteriores, datosNuevos, direccionIP, userAgent);
        }

        public async Task RegistrarExitoAsync(string tipoOperacion, string tablaAfectada, int registroId, 
            string descripcion, string usuarioId, string? motivo = null, 
            object? datosAnteriores = null, object? datosNuevos = null,
            string? direccionIP = null, string? userAgent = null)
        {
            try
            {
                var auditoria = new AuditoriaOperacion
                {
                    TipoOperacion = tipoOperacion,
                    TablaAfectada = tablaAfectada,
                    RegistroId = registroId,
                    Descripcion = descripcion,
                    Motivo = motivo,
                    UsuarioId = usuarioId,
                    DireccionIP = direccionIP,
                    UserAgent = userAgent,
                    FechaOperacion = DateTime.Now,
                    OperacionExitosa = true,
                    DatosAnteriores = datosAnteriores != null ? JsonConvert.SerializeObject(datosAnteriores, Formatting.Indented) : null,
                    DatosNuevos = datosNuevos != null ? JsonConvert.SerializeObject(datosNuevos, Formatting.Indented) : null
                };

                _context.AuditoriasOperaciones.Add(auditoria);
                await _context.SaveChangesAsync();

                _logger.LogInformation("📋 Auditoría registrada: {TipoOperacion} en {Tabla} - Registro {Id} por usuario {Usuario}", 
                    tipoOperacion, tablaAfectada, registroId, usuarioId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "💥 Error al registrar auditoría: {TipoOperacion} en {Tabla} - Registro {Id}", 
                    tipoOperacion, tablaAfectada, registroId);
                // No re-lanzamos la excepción para evitar que falle la operación principal
            }
        }

        public async Task RegistrarErrorAsync(string tipoOperacion, string tablaAfectada, int registroId, 
            string descripcion, string usuarioId, string mensajeError, string? motivo = null,
            string? direccionIP = null, string? userAgent = null)
        {
            try
            {
                var auditoria = new AuditoriaOperacion
                {
                    TipoOperacion = tipoOperacion,
                    TablaAfectada = tablaAfectada,
                    RegistroId = registroId,
                    Descripcion = descripcion,
                    Motivo = motivo,
                    UsuarioId = usuarioId,
                    DireccionIP = direccionIP,
                    UserAgent = userAgent,
                    FechaOperacion = DateTime.Now,
                    OperacionExitosa = false,
                    MensajeError = mensajeError
                };

                _context.AuditoriasOperaciones.Add(auditoria);
                await _context.SaveChangesAsync();

                _logger.LogWarning("📋 Auditoría de error registrada: {TipoOperacion} en {Tabla} - Registro {Id} por usuario {Usuario}: {Error}", 
                    tipoOperacion, tablaAfectada, registroId, usuarioId, mensajeError);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "💥 Error al registrar auditoría de error: {TipoOperacion} en {Tabla} - Registro {Id}", 
                    tipoOperacion, tablaAfectada, registroId);
            }
        }

        public async Task<List<AuditoriaOperacion>> ObtenerHistorialAsync(string tablaAfectada, int registroId)
        {
            try
            {
                return await _context.AuditoriasOperaciones
                    .Where(a => a.TablaAfectada == tablaAfectada && a.RegistroId == registroId)
                    .Include(a => a.Usuario)
                    .OrderByDescending(a => a.FechaOperacion)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener historial de auditoría para {Tabla} - Registro {Id}", tablaAfectada, registroId);
                return new List<AuditoriaOperacion>();
            }
        }

        public async Task<List<AuditoriaOperacion>> ObtenerHistorialUsuarioAsync(string usuarioId, DateTime? fechaDesde = null, DateTime? fechaHasta = null)
        {
            try
            {
                var query = _context.AuditoriasOperaciones
                    .Where(a => a.UsuarioId == usuarioId);

                if (fechaDesde.HasValue)
                    query = query.Where(a => a.FechaOperacion >= fechaDesde.Value);

                if (fechaHasta.HasValue)
                    query = query.Where(a => a.FechaOperacion <= fechaHasta.Value);

                return await query
                    .Include(a => a.Usuario)
                    .OrderByDescending(a => a.FechaOperacion)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener historial de auditoría para usuario {Usuario}", usuarioId);
                return new List<AuditoriaOperacion>();
            }
        }

        public async Task<Dictionary<string, int>> ObtenerEstadisticasAsync(DateTime? fechaDesde = null, DateTime? fechaHasta = null)
        {
            try
            {
                var query = _context.AuditoriasOperaciones.AsQueryable();

                if (fechaDesde.HasValue)
                    query = query.Where(a => a.FechaOperacion >= fechaDesde.Value);

                if (fechaHasta.HasValue)
                    query = query.Where(a => a.FechaOperacion <= fechaHasta.Value);

                return await query
                    .GroupBy(a => a.TipoOperacion)
                    .Select(g => new { TipoOperacion = g.Key, Cantidad = g.Count() })
                    .ToDictionaryAsync(x => x.TipoOperacion, x => x.Cantidad);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener estadísticas de auditoría");
                return new Dictionary<string, int>();
            }
        }

        public async Task<List<AuditoriaOperacion>> ObtenerHistorialAsync(int? tipoOperacion = null, string? usuario = null, 
            DateTime? fechaDesde = null, DateTime? fechaHasta = null, int pageNumber = 1, int pageSize = 20)
        {
            try
            {
                var query = _context.AuditoriasOperaciones
                    .Include(a => a.Usuario)
                    .AsQueryable();

                if (tipoOperacion.HasValue)
                    query = query.Where(a => a.TipoOperacion == tipoOperacion.ToString());

                if (!string.IsNullOrEmpty(usuario))
                    query = query.Where(a => a.UsuarioId.Contains(usuario) || 
                                            (a.Usuario != null && ((ApplicationUser)a.Usuario).Nombre.Contains(usuario)));

                if (fechaDesde.HasValue)
                    query = query.Where(a => a.FechaOperacion >= fechaDesde.Value);

                if (fechaHasta.HasValue)
                    query = query.Where(a => a.FechaOperacion <= fechaHasta.Value);

                return await query
                    .OrderByDescending(a => a.FechaOperacion)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener historial de auditoría con filtros");
                return new List<AuditoriaOperacion>();
            }
        }

        public async Task<AuditoriaOperacion?> ObtenerOperacionAsync(int id)
        {
            try
            {
                return await _context.AuditoriasOperaciones
                    .Include(a => a.Usuario)
                    .FirstOrDefaultAsync(a => a.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener operación de auditoría con ID {Id}", id);
                return null;
            }
        }

        public async Task<List<AuditoriaOperacion>> ObtenerHistorialTablaAsync(string tablaAfectada, string? registroId = null)
        {
            try
            {
                var query = _context.AuditoriasOperaciones
                    .Where(a => a.TablaAfectada == tablaAfectada)
                    .Include(a => a.Usuario)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(registroId))
                {
                    if (int.TryParse(registroId, out int id))
                        query = query.Where(a => a.RegistroId == id);
                }

                return await query
                    .OrderByDescending(a => a.FechaOperacion)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener historial de tabla {Tabla} - Registro {Id}", tablaAfectada, registroId);
                return new List<AuditoriaOperacion>();
            }
        }
    }
}
