using RubricasApp.Web.DTOs.Calificador;

namespace RubricasApp.Web.Services.Calificador
{
    public interface ICalificadorService
    {
        Task<CuadernoCalificadorDto> GenerarCuadernoAsync(CalificadorQueryDto query);
        Task<List<CalificadorColumnDto>> ObtenerColumnasAsync(int materiaId, int periodoAcademicoId);
        Task<bool> ValidarParametrosAsync(int materiaId, int periodoAcademicoId);
    }
}
