using DocinadeApp.Models;

namespace DocinadeApp.Services
{
    public interface IExcepcionService
    {
        Task RegistrarExcepcionAsync(
            string operacion,
            Exception ex,
            string? usuario = null,
            string? ipAddress = null,
            string? urlSolicitada = null,
            string? metodoHttp = null,
            string? parametrosEntrada = null,
            string severidad = "Error");

        Task<List<ExcepcionSistema>> ObtenerExcepcionesRecientesAsync(int cantidad = 50);
        Task<ExcepcionSistema?> ObtenerExcepcionPorIdAsync(int id);
        Task MarcarComoResueltaAsync(int id, string notasResolucion);
    }
}
