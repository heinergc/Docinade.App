using DocinadeApp.DTOs.Calificador;

namespace DocinadeApp.Services.Calificador
{
    public interface ICalificadorService
    {
        Task<CuadernoCalificadorDto> GenerarCuadernoAsync(CalificadorQueryDto query);
        Task<List<CalificadorColumnDto>> ObtenerColumnasAsync(int materiaId, int periodoAcademicoId);
        Task<bool> ValidarParametrosAsync(int materiaId, int periodoAcademicoId);
    }
}
