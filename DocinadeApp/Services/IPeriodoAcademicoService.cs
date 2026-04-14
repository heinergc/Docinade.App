using RubricasApp.Web.Models;

namespace RubricasApp.Web.Services
{
    public interface IPeriodoAcademicoService
    {
        /// <summary>
        /// Obtiene el período académico activo para un usuario específico
        /// </summary>
        Task<PeriodoAcademico?> GetPeriodoActivoAsync(string userId);
        
        /// <summary>
        /// Establece el período académico activo para un usuario
        /// </summary>
        Task SetPeriodoActivoAsync(string userId, int periodoId);
        
        /// <summary>
        /// Obtiene todos los períodos académicos disponibles
        /// </summary>
        Task<IEnumerable<PeriodoAcademico>> GetPeriodosDisponiblesAsync();
        
        /// <summary>
        /// Obtiene el período académico por defecto (más reciente)
        /// </summary>
        Task<PeriodoAcademico?> GetPeriodoDefaultAsync();
        
        /// <summary>
        /// Verifica si un usuario tiene un período seleccionado
        /// </summary>
        bool TienePeriodoSeleccionado(string userId);
        
        /// <summary>
        /// Obtiene el período desde la sesión actual
        /// </summary>
        PeriodoAcademico? GetPeriodoActivoDeSesion();
        
        /// <summary>
        /// Establece el período en la sesión actual
        /// </summary>
        void SetPeriodoActivoEnSesion(PeriodoAcademico periodo);
        
        /// <summary>
        /// Limpia el período de la sesión
        /// </summary>
        void LimpiarPeriodoActivoDeSesion();
        
        /// <summary>
        /// Verifica si el usuario tiene permisos para acceder a un período específico
        /// </summary>
        Task<bool> UsuarioPuedeAccederPeriodo(string userId, int periodoId);
    }
}