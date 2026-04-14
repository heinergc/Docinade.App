using DocinadeApp.Models;

namespace DocinadeApp.Services.Auditoria
{
    /// <summary>
    /// Interfaz para el servicio de auditoría
    /// </summary>
    public interface IAuditoriaService
    {
        /// <summary>
        /// Registra una operación de auditoría
        /// </summary>
        Task RegistrarOperacionAsync(string tipoOperacion, string tablaAfectada, int registroId, 
            string descripcion, string usuarioId, string? motivo = null, 
            object? datosAnteriores = null, object? datosNuevos = null,
            string? direccionIP = null, string? userAgent = null);

        /// <summary>
        /// Registra una operación exitosa
        /// </summary>
        Task RegistrarExitoAsync(string tipoOperacion, string tablaAfectada, int registroId, 
            string descripcion, string usuarioId, string? motivo = null, 
            object? datosAnteriores = null, object? datosNuevos = null,
            string? direccionIP = null, string? userAgent = null);

        /// <summary>
        /// Registra una operación fallida
        /// </summary>
        Task RegistrarErrorAsync(string tipoOperacion, string tablaAfectada, int registroId, 
            string descripcion, string usuarioId, string mensajeError, string? motivo = null,
            string? direccionIP = null, string? userAgent = null);

        /// <summary>
        /// Obtiene el historial de auditoría para un registro específico
        /// </summary>
        Task<List<AuditoriaOperacion>> ObtenerHistorialAsync(string tablaAfectada, int registroId);

        /// <summary>
        /// Obtiene el historial de auditoría para un usuario específico
        /// </summary>
        Task<List<AuditoriaOperacion>> ObtenerHistorialUsuarioAsync(string usuarioId, DateTime? fechaDesde = null, DateTime? fechaHasta = null);

        /// <summary>
        /// Obtiene estadísticas de auditoría
        /// </summary>
        Task<Dictionary<string, int>> ObtenerEstadisticasAsync(DateTime? fechaDesde = null, DateTime? fechaHasta = null);

        /// <summary>
        /// Obtiene el historial de auditoría con filtros y paginación
        /// </summary>
        Task<List<AuditoriaOperacion>> ObtenerHistorialAsync(int? tipoOperacion = null, string? usuario = null, 
            DateTime? fechaDesde = null, DateTime? fechaHasta = null, int pageNumber = 1, int pageSize = 20);

        /// <summary>
        /// Obtiene una operación específica por ID
        /// </summary>
        Task<AuditoriaOperacion?> ObtenerOperacionAsync(int id);

        /// <summary>
        /// Obtiene el historial de auditoría para una tabla específica
        /// </summary>
        Task<List<AuditoriaOperacion>> ObtenerHistorialTablaAsync(string tablaAfectada, string? registroId = null);
    }
}
