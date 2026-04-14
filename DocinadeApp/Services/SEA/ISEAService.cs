using RubricasApp.Web.Models.SEA;
using RubricasApp.Web.ViewModels.SEA;

namespace RubricasApp.Web.Services.SEA
{
    /// <summary>
    /// Servicio para generación de reportes SEA (Sistema de Evaluación MEP)
    /// </summary>
    public interface ISEAService
    {
        /// <summary>
        /// Genera el reporte completo SEA para una materia y periodo
        /// Combina calificaciones del cuaderno calificador con datos de asistencia
        /// </summary>
        /// <param name="materiaId">ID de la materia. Null = todas las materias</param>
        /// <param name="periodoAcademicoId">ID del periodo académico. Null = periodo activo</param>
        /// <returns>ViewModel con reporte completo</returns>
        Task<SEAReporteViewModel> GenerarReporteSEAAsync(int? materiaId, int? periodoAcademicoId);

        /// <summary>
        /// Calcula porcentajes de asistencia por estudiante para un periodo y materia
        /// </summary>
        /// <param name="materiaId">ID de la materia</param>
        /// <param name="periodoId">ID del periodo académico</param>
        /// <returns>Diccionario con EstudianteId → Porcentaje de asistencia</returns>
        Task<Dictionary<int, AsistenciaDetalle>> CalcularAsistenciasPorEstudianteAsync(int materiaId, int periodoId);

        /// <summary>
        /// Exporta el reporte SEA en formato CSV compatible con sistema MEP
        /// </summary>
        /// <param name="materiaId">ID de la materia</param>
        /// <param name="periodoId">ID del periodo académico</param>
        /// <param name="redondeo">Opción de redondeo para las notas</param>
        /// <returns>Archivo CSV como array de bytes (UTF-8 con BOM)</returns>
        Task<byte[]> ExportarCSV_SEAAsync(int? materiaId, int? periodoId, OpcionRedondeoSEA redondeo);

        /// <summary>
        /// Exporta el reporte SEA en formato PDF
        /// </summary>
        Task<byte[]> ExportarPDF_SEAAsync(int? materiaId, int? periodoId, OpcionRedondeoSEA redondeo);

        /// <summary>
        /// Exporta el reporte SEA en formato Excel
        /// </summary>
        Task<byte[]> ExportarExcel_SEAAsync(int? materiaId, int? periodoId, OpcionRedondeoSEA redondeo);

        /// <summary>
        /// Valida formatos de IDs de estudiantes según estándar de cédula MEP
        /// </summary>
        /// <param name="estudianteIds">Lista de IDs de estudiantes a validar</param>
        /// <returns>Diccionario con EstudianteId → Lista de advertencias</returns>
        Task<Dictionary<int, List<string>>> ValidarIDsEstudiantesAsync(List<int> estudianteIds);

        /// <summary>
        /// Obtiene el mapeo de InstrumentosEvaluacion a Componentes SEA para una materia
        /// </summary>
        /// <param name="materiaId">ID de la materia</param>
        /// <returns>Diccionario con InstrumentoId → Componente SEA</returns>
        Task<Dictionary<int, string>> ObtenerMapeoComponentesAsync(int materiaId);

        /// <summary>
        /// Guarda o actualiza la configuración de componentes SEA para una materia
        /// </summary>
        Task<bool> GuardarConfiguracionAsync(int materiaId, List<ConfiguracionComponenteItem> configuraciones);

        /// <summary>
        /// Obtiene la configuración actual de componentes SEA para una materia
        /// </summary>
        Task<ConfiguracionSEAViewModel> ObtenerConfiguracionAsync(int materiaId);

        /// <summary>
        /// Valida que las ponderaciones de componentes SEA sumen 100%
        /// </summary>
        Task<(bool esValida, decimal total, string mensaje)> ValidarPonderacionesAsync(int materiaId);
    }

    /// <summary>
    /// Detalle de asistencia de un estudiante
    /// </summary>
    public class AsistenciaDetalle
    {
        public int TotalLecciones { get; set; }
        public int Presentes { get; set; }
        public int Ausentes { get; set; }
        public int Tardanzas { get; set; }
        public int Justificadas { get; set; }
        public decimal Porcentaje => TotalLecciones > 0 
            ? Math.Round((decimal)Presentes / TotalLecciones * 100, 2) 
            : 0;
    }
}
