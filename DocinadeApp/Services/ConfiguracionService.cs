using Microsoft.EntityFrameworkCore;
using RubricasApp.Web.Data;
using RubricasApp.Web.Models;

namespace RubricasApp.Web.Services
{
    /// <summary>
    /// Implementación del servicio de configuración del sistema
    /// </summary>
    public class ConfiguracionService : IConfiguracionService
    {
        private readonly RubricasDbContext _context;
        private readonly ILogger<ConfiguracionService> _logger;

        // Constantes para las claves de configuración
        public const string REGISTRO_ABIERTO = "MODO_REGISTRO"; //"REGISTRO_ABIERTO";
        public const string MENSAJE_REGISTRO_CERRADO = "MENSAJE_REGISTRO_CERRADO";
        public const string NOMBRE_APLICACION = "NOMBRE_APLICACION";
        public const string VERSION_APLICACION = "VERSION_APLICACION";

        public ConfiguracionService(RubricasDbContext context, ILogger<ConfiguracionService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> EsRegistroAbiertoAsync()
        {
            try
            {
                var config = await _context.ConfiguracionesSistema
                    .FirstOrDefaultAsync(c => c.Clave == REGISTRO_ABIERTO);

                if (config == null)
                {
                    // Crear configuración por defecto (registro cerrado)
                    await EstablecerRegistroAbiertoAsync(false, "System");
                    return false;
                }

                // Verificar si es un enum o un booleano
                if (Enum.TryParse<ModoRegistroUsuarios>(config.Valor, out var modo))
                {
                    return modo == ModoRegistroUsuarios.Abierto;
                }
                
                // Fallback para valores booleanos
                return bool.TryParse(config.Valor, out var resultado) && resultado;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verificando si el registro está abierto");
                return false; // Por seguridad, asumir que está cerrado si hay error
            }
        }

        public async Task<bool> EstablecerRegistroAbiertoAsync(bool abierto, string userId)
        {
            try
            {
                var modoRegistro = abierto ? ModoRegistroUsuarios.Abierto : ModoRegistroUsuarios.Cerrado;
                await EstablecerConfiguracionAsync(REGISTRO_ABIERTO, modoRegistro.ToString(), 
                    "Controla si el registro de nuevos usuarios está habilitado", userId);
                
                _logger.LogInformation("Registro de usuarios {Estado} por {Usuario}",
                    abierto ? "habilitado" : "deshabilitado", userId);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error estableciendo estado del registro");
                return false;
            }
        }

        public async Task<string?> ObtenerConfiguracionAsync(string clave)
        {
            try
            {
                var config = await _context.ConfiguracionesSistema
                    .FirstOrDefaultAsync(c => c.Clave == clave);

                return config?.Valor;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo configuración {Clave}", clave);
                return null;
            }
        }

        public async Task<string> ObtenerConfiguracionAsync(string clave, string valorPorDefecto)
        {
            try
            {
                var valor = await ObtenerConfiguracionAsync(clave);
                return valor ?? valorPorDefecto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo configuración {Clave}, usando valor por defecto: {ValorPorDefecto}", clave, valorPorDefecto);
                return valorPorDefecto;
            }
        }

        public async Task<bool> EstablecerConfiguracionAsync(string clave, string valor, string? descripcion = null, string? userId = null)
        {
            try
            {
                var config = await _context.ConfiguracionesSistema
                    .FirstOrDefaultAsync(c => c.Clave == clave);

                if (config == null)
                {
                    config = new ConfiguracionSistema
                    {
                        Clave = clave,
                        Valor = valor,
                        Descripcion = descripcion ?? $"Configuración para {clave}",
                        UsuarioModificacion = userId ?? "System",
                        FechaCreacion = DateTime.UtcNow,
                        FechaModificacion = DateTime.UtcNow
                    };
                    _context.ConfiguracionesSistema.Add(config);
                }
                else
                {
                    config.Valor = valor;
                    if (!string.IsNullOrEmpty(descripcion))
                        config.Descripcion = descripcion;
                    config.UsuarioModificacion = userId ?? "System";
                    config.FechaModificacion = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error estableciendo configuración {Clave}", clave);
                return false;
            }
        }

        public async Task<Dictionary<string, string>> ObtenerTodasConfiguracionesAsync()
        {
            try
            {
                var configuraciones = await _context.ConfiguracionesSistema
                    .ToDictionaryAsync(c => c.Clave, c => c.Valor);

                return configuraciones;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo todas las configuraciones");
                return new Dictionary<string, string>();
            }
        }

        public async Task<List<ConfiguracionSistema>> ObtenerTodasLasConfiguracionesAsync()
        {
            try
            {
                return await _context.ConfiguracionesSistema
                    .OrderBy(c => c.Clave)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo todas las configuraciones como lista");
                return new List<ConfiguracionSistema>();
            }
        }

        public async Task<bool> ExisteConfiguracionAsync(string clave)
        {
            try
            {
                return await _context.ConfiguracionesSistema
                    .AnyAsync(c => c.Clave == clave);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verificando existencia de configuración {Clave}", clave);
                return false;
            }
        }

        public async Task<ModoRegistroUsuarios> ObtenerModoRegistroAsync()
        {
            try
            {
                var valor = await ObtenerConfiguracionAsync(REGISTRO_ABIERTO);
                if (Enum.TryParse<ModoRegistroUsuarios>(valor, out var modo))
                {
                    return modo;
                }
                return ModoRegistroUsuarios.Cerrado;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo modo de registro");
                return ModoRegistroUsuarios.Cerrado;
            }
        }

        public async Task ActualizarModoRegistroAsync(ModoRegistroUsuarios modo, string usuario)
        {
            await EstablecerConfiguracionAsync(REGISTRO_ABIERTO, modo.ToString(), "Modo de registro de usuarios", usuario);
        }

        public async Task<string> ObtenerMensajeRegistroCerradoAsync()
        {
            try
            {
                // CORREGIDO: Obtener el mensaje específico de registro cerrado, no el modo de registro
                var mensaje = await ObtenerConfiguracionAsync("MENSAJE_REGISTRO_CERRADO");
                return mensaje ?? "El registro de usuarios está temporalmente cerrado.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo mensaje de registro cerrado");
                return "El registro de usuarios está temporalmente cerrado.";
            }
        }

        public async Task ActualizarConfiguracionAsync(string clave, string valor, string? descripcion, string usuario)
        {
            await EstablecerConfiguracionAsync(clave, valor, descripcion, usuario);
        }
    }
}