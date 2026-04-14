using RubricasApp.Web.Models;

namespace RubricasApp.Web.Services
{
    public interface IHorarioService
    {
        Task<Leccion?> ObtenerLeccionPorIdAsync(int idLeccion);
        Task<Leccion?> ObtenerHorarioPorIdAsync(int idLeccion);
        Task<IEnumerable<Leccion>> ObtenerTodosHorariosAsync();
        Task<IEnumerable<Leccion>> ObtenerLeccionesPorGrupoAsync(int idGrupo, bool soloActivas = true);
        Task<IEnumerable<Leccion>> ObtenerHorariosPorGrupoAsync(int idGrupo);
        Task<IEnumerable<Leccion>> ObtenerLeccionesPorGrupoYDiaAsync(int idGrupo, int diaSemana, bool soloActivas = true);
        Task<IEnumerable<Leccion>> ObtenerHorariosPorGrupoYDiaAsync(int idGrupo, DayOfWeek diaSemana);
        Task<IEnumerable<Leccion>> ObtenerLeccionesPorMateriaAsync(int materiaId, bool soloActivas = true);
        Task<IEnumerable<Leccion>> ObtenerHorariosPorMateriaAsync(int materiaId);
        Task<IEnumerable<Leccion>> ObtenerHorariosPorDiaAsync(DayOfWeek diaSemana);
        Task<Leccion?> ObtenerLeccionPorBloqueAsync(int idGrupo, int materiaId, int diaSemana, int numeroBloque);
        Task<(bool Exito, string Mensaje, Leccion? Leccion)> CrearLeccionAsync(Leccion leccion);
        Task<(bool Exito, string Mensaje, Leccion? Leccion)> CrearHorarioAsync(Leccion leccion, string usuarioId);
        Task<(bool Exito, string Mensaje)> ActualizarLeccionAsync(Leccion leccion);
        Task<(bool Exito, string Mensaje)> ActualizarHorarioAsync(Leccion leccion, string usuarioId);
        Task<(bool Exito, string Mensaje)> EliminarLeccionAsync(int idLeccion, bool eliminacionFisica = false);
        Task<(bool Exito, string Mensaje)> EliminarHorarioAsync(int idLeccion, string usuarioId);
        Task<(bool Exito, string Mensaje)> ActivarDesactivarHorarioAsync(int idLeccion, bool activo, string usuarioId);
        Task<List<string>> ValidarHorarioAsync(Leccion leccion);
        Task<List<Leccion>> ObtenerConflictosHorarioAsync(Leccion leccion);
        Task<bool> ValidarSolapamientoHorarioAsync(int idGrupo, int diaSemana, TimeSpan horaInicio, TimeSpan horaFin, int? idLeccionExcluir = null);
        Task<bool> ValidarHorarioValidoAsync(TimeSpan horaInicio, TimeSpan horaFin);
        Task<Dictionary<int, List<Leccion>>> ObtenerHorarioSemanalGrupoAsync(int idGrupo);
    }
}
