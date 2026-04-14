using DocinadeApp.Models;
using DocinadeApp.ViewModels;

namespace DocinadeApp.Services.AdecuacionCurricular
{
    /// <summary>
    /// Interfaz para el servicio de Adecuación Curricular Significativa (ACS)
    /// </summary>
    public interface IACSService
    {
        /// <summary>
        /// Configura un instrumento específico para un estudiante con ACS
        /// </summary>
        Task<bool> ConfigurarInstrumentoACSAsync(int estudianteId, int instrumentoId, int periodoId, ConfiguracionACSInstrumento config, string? usuarioActual = null);
        
        /// <summary>
        /// Obtiene todas las configuraciones ACS de un estudiante para un período
        /// </summary>
        Task<List<EstudianteInstrumentoACS>> ObtenerConfiguracionesACSAsync(int estudianteId, int periodoId);
        
        /// <summary>
        /// Obtiene la configuración ACS específica de un estudiante para un instrumento
        /// </summary>
        Task<EstudianteInstrumentoACS?> ObtenerConfiguracionInstrumentoAsync(int estudianteId, int instrumentoId, int periodoId);
        
        /// <summary>
        /// Clona una rúbrica existente y la adapta para un estudiante específico
        /// </summary>
        Task<Rubrica?> ClonarRubricaParaEstudianteAsync(int rubricaOriginalId, int estudianteId, string motivoCambio, string? usuarioActual = null);
        
        /// <summary>
        /// Elimina todas las configuraciones ACS de un estudiante
        /// </summary>
        Task<bool> EliminarConfiguracionesACSAsync(int estudianteId, string? motivo = null);
        
        /// <summary>
        /// Aplica configuraciones ACS a períodos anteriores de manera retroactiva
        /// </summary>
        Task<bool> AplicarACSPeriodosAnterioresAsync(int estudianteId, int periodoActualId);
        
        /// <summary>
        /// Genera un respaldo de las configuraciones ACS de un estudiante antes de eliminarlas
        /// </summary>
        Task<string> RespaldarDatosACSAsync(int estudianteId);
        
        /// <summary>
        /// Obtiene estadísticas de estudiantes con ACS en la institución
        /// </summary>
        Task<ReporteEstudiantesACSViewModel> ObtenerEstadisticasACSAsync(int? periodoId = null);
        
        /// <summary>
        /// Valida si un estudiante puede ser marcado con ACS
        /// </summary>
        Task<(bool esValido, string? mensajeError)> ValidarCambioACSAsync(int estudianteId, string nuevoTipoAdecuacion);
        
        /// <summary>
        /// Obtiene el ViewModel de configuración ACS para un estudiante
        /// </summary>
        Task<EstudianteACSConfigViewModel?> ObtenerViewModelConfiguracionAsync(int estudianteId, int periodoId, int? materiaId = null);
        
        /// <summary>
        /// Guarda la configuración completa de ACS desde el formulario
        /// </summary>
        Task<bool> GuardarConfiguracionCompletaAsync(EstudianteACSConfigViewModel viewModel, string? usuarioActual = null);
    }
}
