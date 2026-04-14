using DocinadeApp.Models;

namespace DocinadeApp.Services.CuadernoCalificador
{
    public interface ICuadernoCalificadorService
    {
        Task<CuadernoCalificadorViewModel> ObtenerCuadernoAsync(int cuadernoId);
        Task<List<CalificacionEstudiante>> ObtenerCalificacionesEstudiantesAsync(int cuadernoId);
        Task<bool> ValidarPonderacionesAsync(int cuadernoId);
        Task<decimal> CalcularNotaPonderadaEstudianteAsync(int estudianteId, int cuadernoId);
        Task<EstadisticasCuaderno> GenerarEstadisticasAsync(int cuadernoId);
        Task<int> CrearCuadernoAsync(CrearCuadernoViewModel model);
        Task<bool> ActualizarPonderacionesAsync(int cuadernoId, List<InstrumentoConfiguracion> ponderaciones);
        Task<byte[]> ExportarExcelAsync(int cuadernoId);
        Task<byte[]> GenerarReportePDFAsync(int cuadernoId);
        Task<List<Models.CuadernoCalificador>> ObtenerCuadernosPorFiltroAsync(int? materiaId, int? periodoAcademicoId);
        Task<ConfigurarInstrumentosViewModel> ObtenerConfiguracionInstrumentosAsync(int cuadernoId);
        Task<bool> GuardarConfiguracionInstrumentosAsync(ConfigurarInstrumentosViewModel model);
        Task<bool> CerrarCuadernoAsync(int cuadernoId);
    }
}