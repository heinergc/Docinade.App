using static DocinadeApp.Models.Audit.AuditActionTypes;
using DocinadeApp.Models;

namespace DocinadeApp.Services
{
    /// <summary>
    /// Interfaz para el servicio de configuración del sistema
    /// </summary>
    public interface IConfiguracionService
    {
        /// <summary>
        /// Verifica si el registro de usuarios está abierto
        /// </summary>
        Task<bool> EsRegistroAbiertoAsync();

        /// <summary>
        /// Obtiene el modo actual de registro de usuarios
        /// </summary>
        Task<ModoRegistroUsuarios> ObtenerModoRegistroAsync();

        /// <summary>
        /// Actualiza el modo de registro de usuarios
        /// </summary>
        Task ActualizarModoRegistroAsync(ModoRegistroUsuarios modo, string userId);

        /// <summary>
        /// Obtiene el mensaje personalizado cuando el registro está cerrado
        /// </summary>
        Task<string> ObtenerMensajeRegistroCerradoAsync();

        /// <summary>
        /// Establece si el registro de usuarios está abierto o cerrado
        /// </summary>
        Task<bool> EstablecerRegistroAbiertoAsync(bool abierto, string userId);

        /// <summary>
        /// Obtiene una configuración específica por clave
        /// </summary>
        Task<string?> ObtenerConfiguracionAsync(string clave);

        /// <summary>
        /// Obtiene una configuración específica por clave con valor por defecto
        /// </summary>
        Task<string> ObtenerConfiguracionAsync(string clave, string valorPorDefecto);

        /// <summary>
        /// Establece una configuración específica
        /// </summary>
        Task<bool> EstablecerConfiguracionAsync(string clave, string valor, string? descripcion = null, string? userId = null);

        /// <summary>
        /// Actualiza una configuración del sistema
        /// </summary>
        Task ActualizarConfiguracionAsync(string clave, string valor, string? descripcion = null, string? userId = null);

        /// <summary>
        /// Obtiene todas las configuraciones del sistema
        /// </summary>
        Task<Dictionary<string, string>> ObtenerTodasConfiguracionesAsync();

        /// <summary>
        /// Verifica si una configuración existe
        /// </summary>
        Task<bool> ExisteConfiguracionAsync(string clave);
        Task<List<ConfiguracionSistema>> ObtenerTodasLasConfiguracionesAsync();

    }
}