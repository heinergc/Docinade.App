using RubricasApp.Web.ViewModels;

namespace RubricasApp.Web.Services.CuadernoCalificador
{
    public interface ICuadernoCalificadorDinamicoService
    {
        /// <summary>
        /// Genera el cuaderno calificador dinámicamente basado en las evaluaciones existentes
        /// </summary>
        Task<CuadernoCalificadorDinamicoViewModel> GenerarCuadernoCalificadorAsync(int? materiaId, int? periodoAcademicoId);
        
        /// <summary>
        /// Obtiene las materias disponibles para filtrar
        /// </summary>
        Task<List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>> ObtenerMateriasDisponiblesAsync();
        
        /// <summary>
        /// Obtiene los períodos académicos disponibles para filtrar
        /// </summary>
        Task<List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>> ObtenerPeriodosDisponiblesAsync();
        
        /// <summary>
        /// Exporta el cuaderno a Excel
        /// </summary>
        Task<byte[]> ExportarCuadernoAExcelAsync(int? materiaId, int? periodoAcademicoId);
        
        /// <summary>
        /// Obtiene estadísticas rápidas para el dashboard
        /// </summary>
        Task<Dictionary<string, object>> ObtenerEstadisticasRapidasAsync(int? materiaId, int? periodoAcademicoId);
        
        /// <summary>
        /// Diagnostica las ponderaciones configuradas para una materia
        /// </summary>
        Task<Dictionary<string, object>> DiagnosticarPonderacionesAsync(int materiaId);
    }
}