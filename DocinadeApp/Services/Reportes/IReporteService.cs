using DocinadeApp.ViewModels.Reportes;

namespace DocinadeApp.Services.Reportes
{
    public interface IReporteService
    {
        /// <summary>
        /// Genera el reporte en �rbol completo del sistema de evaluaciones
        /// </summary>
        Task<ReporteArbolViewModel> GenerarReporteArbolAsync(FiltrosReporte filtros);
        
        /// <summary>
        /// Ejecuta la consulta jer�rquica principal
        /// </summary>
        Task<List<DatoJerarquico>> EjecutarConsultaJerarquicaAsync(FiltrosReporte filtros);
        
        /// <summary>
        /// Exporta el reporte a Excel
        /// </summary>
        Task<byte[]> ExportarReporteAExcelAsync(FiltrosReporte filtros);
        
        /// <summary>
        /// Exporta el reporte a CSV plano
        /// </summary>
        Task<byte[]> ExportarReporteACSVAsync(FiltrosReporte filtros);
        
        /// <summary>
        /// Obtiene estad�sticas r�pidas del sistema
        /// </summary>
        Task<Dictionary<string, object>> ObtenerEstadisticasRapidasAsync();
        
        /// <summary>
        /// Cuenta el total de nodos en el �rbol
        /// </summary>
        int ContarNodos(ReporteArbolViewModel reporte);
    }
}