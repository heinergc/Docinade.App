using RubricasApp.Web.ViewModels.Reportes;

namespace RubricasApp.Web.Services.Reportes
{
    public interface IReporteService
    {
        /// <summary>
        /// Genera el reporte en árbol completo del sistema de evaluaciones
        /// </summary>
        Task<ReporteArbolViewModel> GenerarReporteArbolAsync(FiltrosReporte filtros);
        
        /// <summary>
        /// Ejecuta la consulta jerárquica principal
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
        /// Obtiene estadísticas rápidas del sistema
        /// </summary>
        Task<Dictionary<string, object>> ObtenerEstadisticasRapidasAsync();
        
        /// <summary>
        /// Cuenta el total de nodos en el árbol
        /// </summary>
        int ContarNodos(ReporteArbolViewModel reporte);
    }
}