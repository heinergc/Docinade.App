using DocinadeApp.Data;
using DocinadeApp.Models;
using Microsoft.EntityFrameworkCore;

namespace DocinadeApp.Services
{
    public class ExcepcionService : IExcepcionService
    {
        private readonly RubricasDbContext _context;
        private readonly ILogger<ExcepcionService> _logger;

        public ExcepcionService(RubricasDbContext context, ILogger<ExcepcionService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task RegistrarExcepcionAsync(
            string operacion,
            Exception ex,
            string? usuario = null,
            string? ipAddress = null,
            string? urlSolicitada = null,
            string? metodoHttp = null,
            string? parametrosEntrada = null,
            string severidad = "Error")
        {
            try
            {
                var excepcion = new ExcepcionSistema
                {
                    Operacion = operacion.Length > 500 ? operacion.Substring(0, 500) : operacion,
                    TipoExcepcion = ex.GetType().Name.Length > 100 ? ex.GetType().Name.Substring(0, 100) : ex.GetType().Name,
                    MensajeError = ex.Message,
                    StackTrace = ex.StackTrace,
                    Usuario = usuario?.Length > 50 ? usuario.Substring(0, 50) : usuario,
                    IpAddress = ipAddress?.Length > 45 ? ipAddress.Substring(0, 45) : ipAddress,
                    UrlSolicitada = urlSolicitada?.Length > 500 ? urlSolicitada.Substring(0, 500) : urlSolicitada,
                    MetodoHttp = metodoHttp?.Length > 10 ? metodoHttp.Substring(0, 10) : metodoHttp,
                    ParametrosEntrada = parametrosEntrada,
                    FechaHora = DateTime.UtcNow,
                    Severidad = severidad?.Length > 50 ? severidad.Substring(0, 50) : severidad,
                    Resuelta = false
                };

                _context.ExcepcionesSistema.Add(excepcion);
                await _context.SaveChangesAsync();

                _logger.LogInformation(
                    "[INFO] Excepción registrada en BD - ID: {Id}, Operación: {Operacion}, Tipo: {Tipo}",
                    excepcion.Id,
                    operacion,
                    ex.GetType().Name);
            }
            catch (Exception dbEx)
            {
                // Si falla el registro en BD, al menos logueamos
                _logger.LogError(dbEx,
                    "[ERROR] Falló el registro de excepción en BD. Excepción original: {OperacionOriginal} - {MensajeOriginal}",
                    operacion,
                    ex.Message);
            }
        }

        public async Task<List<ExcepcionSistema>> ObtenerExcepcionesRecientesAsync(int cantidad = 50)
        {
            return await _context.ExcepcionesSistema
                .OrderByDescending(e => e.FechaHora)
                .Take(cantidad)
                .ToListAsync();
        }

        public async Task<ExcepcionSistema?> ObtenerExcepcionPorIdAsync(int id)
        {
            return await _context.ExcepcionesSistema
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task MarcarComoResueltaAsync(int id, string notasResolucion)
        {
            var excepcion = await _context.ExcepcionesSistema.FindAsync(id);
            if (excepcion != null)
            {
                excepcion.Resuelta = true;
                excepcion.FechaResolucion = DateTime.UtcNow;
                excepcion.NotasResolucion = notasResolucion?.Length > 1000 
                    ? notasResolucion.Substring(0, 1000) 
                    : notasResolucion;

                await _context.SaveChangesAsync();
            }
        }
    }
}
