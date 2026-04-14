using DocinadeApp.Models;
using DocinadeApp.ViewModels.Conducta;

namespace DocinadeApp.Services
{
    public interface IConductaService
    {
        // Configuración
        Task<decimal> ObtenerNotaMinimaAprobacionAsync();
        Task ActualizarNotaMinimaAprobacionAsync(decimal notaMinima);

        // Boletas de Conducta
        Task<int> RegistrarBoletaAsync(BoletaConducta boleta);
        Task<BoletaConducta?> ObtenerBoletaPorIdAsync(int idBoleta);
        Task<List<BoletaConducta>> ObtenerBoletasPorEstudianteAsync(int idEstudiante, int idPeriodo);
        Task<List<BoletaConducta>> ObtenerBoletasPorGrupoAsync(int idGrupo, int idPeriodo);
        Task AnularBoletaAsync(int idBoleta, string usuarioId, string motivo);

        // Nota de Conducta
        Task<NotaConducta> CalcularNotaConductaAsync(int idEstudiante, int idPeriodo);
        Task<NotaConducta?> ObtenerNotaConductaAsync(int idEstudiante, int idPeriodo);
        Task<List<EstudianteRiesgoItem>> ObtenerEstudiantesEnRiesgoAsync(int idPeriodo);
        Task<List<EstudianteRiesgoItem>> ObtenerEstudiantesAplazadosAsync(int idPeriodo);

        // Programas de Acciones
        Task<int> CrearProgramaAccionesAsync(ProgramaAccionesInstitucional programa);
        Task ActualizarProgramaAccionesAsync(ProgramaAccionesInstitucional programa);
        Task<ProgramaAccionesInstitucional?> ObtenerProgramaAccionesPorIdAsync(int idPrograma);
        Task VerificarProgramaAccionesAsync(int idPrograma, string resultadoFinal, bool aprobarConducta, string conclusiones, string verificadoPorId);

        // Decisión Profesional
        Task<int> AplicarDecisionProfesionalAsync(DecisionProfesionalConducta decision);
        Task<DecisionProfesionalConducta?> ObtenerDecisionProfesionalPorIdAsync(int idDecision);

        // Notificaciones
        Task NotificarProfesorGuiaAsync(int idBoleta);
        Task<int?> ObtenerProfesorGuiaDeEstudianteAsync(int idEstudiante);
        Task<string?> ObtenerNombreProfesorGuiaAsync(int idEstudiante);

        // Reportes
        Task<DashboardConductaViewModel> ObtenerDashboardConductaAsync(int idPeriodo);
        Task<HistorialBoletasEstudianteViewModel> ObtenerHistorialBoletasEstudianteAsync(int idEstudiante, int idPeriodo);
        Task<RegistroFaltasGrupoViewModel> ObtenerRegistroFaltasGrupoAsync(int idGrupo, int idPeriodo);
    }
}
