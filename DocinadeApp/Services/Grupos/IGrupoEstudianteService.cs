using DocinadeApp.Models;
using DocinadeApp.ViewModels.Grupos;

namespace DocinadeApp.Services.Grupos
{
    public interface IGrupoEstudianteService
    {
        // Operaciones CRUD b�sicas
        Task<IEnumerable<GrupoEstudiante>> ObtenerGruposAsync(FiltrosGrupoViewModel? filtros = null);
        Task<GrupoEstudiante?> ObtenerGrupoPorIdAsync(int grupoId);
        Task<ResultadoOperacion<GrupoEstudiante>> CrearGrupoAsync(CrearGrupoViewModel modelo, string usuarioId);
        Task<ResultadoOperacion<GrupoEstudiante>> ActualizarGrupoAsync(EditarGrupoViewModel modelo, string usuarioId);
        Task<ResultadoOperacion> EliminarGrupoAsync(int grupoId, string usuarioId, string? motivo = null, string? direccionIP = null, string? userAgent = null);

        // Gesti�n de estudiantes en grupos
        Task<ResultadoOperacion> AsignarEstudiantesAsync(int grupoId, List<int> estudianteIds, string usuarioId, string? motivo = null, bool esGrupoPrincipal = true);
        Task<ResultadoOperacion> DesasignarEstudianteAsync(int grupoId, int estudianteId, string usuarioId, string? motivo = null);
        Task<ResultadoOperacion> TransferirEstudianteAsync(int estudianteId, int grupoOrigenId, int grupoDestinoId, string usuarioId, string? motivo = null);

        // Gesti�n de materias en grupos
        Task<ResultadoOperacion> AsignarMateriasAsync(int grupoId, List<int> materiaIds, string usuarioId);
        Task<ResultadoOperacion> DesasignarMateriaAsync(int grupoId, int materiaId, string usuarioId);

        // Consultas espec�ficas
        Task<List<EstudianteDisponibleViewModel>> ObtenerEstudiantesDisponiblesAsync(int grupoId, int periodoAcademicoId, string? filtroNombre = null);
        Task<List<GrupoEstudiante>> ObtenerGruposPorEstudianteAsync(int estudianteId);
        Task<List<GrupoEstudiante>> ObtenerGruposPorMateriaAsync(int materiaId, int periodoAcademicoId);
        Task<GrupoEstadisticasViewModel> ObtenerEstadisticasAsync(int? periodoAcademicoId = null);

        // Validaciones
        Task<bool> ValidarCapacidadGrupoAsync(int grupoId, int cantidadNuevosEstudiantes);
        Task<bool> ValidarCodigoUnicoAsync(string codigo, int periodoAcademicoId, int? grupoIdExcluir = null);
        Task<List<string>> ValidarAsignacionEstudiantesAsync(int grupoId, List<int> estudianteIds);

        // Reportes y exportaciones
        Task<byte[]> ExportarGruposExcelAsync(FiltrosGrupoViewModel? filtros = null);
        Task<byte[]> ExportarEstudiantesPorGrupoExcelAsync(int grupoId);
    }

    public class ResultadoOperacion
    {
        public bool Exitoso { get; set; }
        public string Mensaje { get; set; } = string.Empty;
        public List<string> Errores { get; set; } = new List<string>();
        public object? Datos { get; set; }

        public static ResultadoOperacion Exito(string mensaje = "Operaci�n realizada con �xito", object? datos = null)
        {
            return new ResultadoOperacion
            {
                Exitoso = true,
                Mensaje = mensaje,
                Datos = datos
            };
        }

        public static ResultadoOperacion Error(string mensaje, List<string>? errores = null)
        {
            return new ResultadoOperacion
            {
                Exitoso = false,
                Mensaje = mensaje,
                Errores = errores ?? new List<string>()
            };
        }
    }

    public class ResultadoOperacion<T> : ResultadoOperacion
    {
        public T? Resultado { get; set; }

        public static ResultadoOperacion<T> Exito(T resultado, string mensaje = "Operaci�n realizada con �xito")
        {
            return new ResultadoOperacion<T>
            {
                Exitoso = true,
                Mensaje = mensaje,
                Resultado = resultado,
                Datos = resultado
            };
        }

        public static new ResultadoOperacion<T> Error(string mensaje, List<string>? errores = null)
        {
            return new ResultadoOperacion<T>
            {
                Exitoso = false,
                Mensaje = mensaje,
                Errores = errores ?? new List<string>()
            };
        }
    }
}