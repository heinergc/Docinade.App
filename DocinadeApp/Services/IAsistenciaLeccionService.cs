using RubricasApp.Web.Models;

namespace RubricasApp.Web.Services
{
    public interface IAsistenciaLeccionService
    {
        Task<Asistencia?> ObtenerAsistenciaPorIdAsync(int asistenciaId);
        Task<IEnumerable<Asistencia>> ObtenerAsistenciasPorLeccionAsync(int idLeccion, DateTime fecha);
        Task<IEnumerable<Asistencia>> ObtenerAsistenciasPorEstudianteAsync(int estudianteId, DateTime? fechaInicio = null, DateTime? fechaFin = null);
        Task<IEnumerable<Asistencia>> ObtenerAsistenciasPorGrupoYFechaAsync(int grupoId, DateTime fecha);
        Task<(bool Exito, string Mensaje, Asistencia? Asistencia)> RegistrarAsistenciaAsync(Asistencia asistencia, string usuarioId);
        Task<(bool Exito, string Mensaje)> ActualizarAsistenciaAsync(Asistencia asistencia, string usuarioId);
        Task<(bool Exito, string Mensaje)> RegistrarAsistenciasMasivasAsync(List<Asistencia> asistencias, string usuarioId);
        Task<Dictionary<string, int>> ObtenerEstadisticasAsistenciaEstudianteAsync(int estudianteId, DateTime? fechaInicio = null, DateTime? fechaFin = null);
        Task<decimal> CalcularPorcentajeAsistenciaAsync(int estudianteId, int? grupoId = null, int? materiaId = null, DateTime? fechaInicio = null, DateTime? fechaFin = null);
        Task<IEnumerable<Asistencia>> ObtenerAusenciasEstudianteAsync(int estudianteId, bool incluirJustificadas = false, DateTime? fechaInicio = null, DateTime? fechaFin = null);
        Task<Dictionary<int, Dictionary<string, int>>> ObtenerResumenAsistenciaGrupoAsync(int grupoId, DateTime fechaInicio, DateTime fechaFin);
        Task<bool> ValidarTransicionEstadoAsync(string estadoActual, string nuevoEstado);
    }
}
