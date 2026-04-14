using DocinadeApp.Models;
using DocinadeApp.Models.Academic;
using DocinadeApp.Models.ViewModels;
using DocinadeApp.ViewModels.Academic;

namespace DocinadeApp.Services.Academic
{
    public interface IMateriasService
    {
        Task<PagedResult<MateriaListVm>> GetPagedAsync(int page, int pageSize, string? searchTerm = null, string? tipoFiltro = null);
        Task<MateriaVm?> GetByIdAsync(int id);
        Task<OperationResult> CreateAsync(CrearMateriaVm model);
        Task<OperationResult> UpdateAsync(int id, EditarMateriaVm model);
        Task<OperationResult> DeleteAsync(int id);
        Task<IEnumerable<AsignacionInstrumentoVm>> GetInstrumentosAsync(int materiaId);
        Task<OperationResult> AsignarInstrumentosAsync(AsignarInstrumentosVm model);
        Task<OperationResult> QuitarInstrumentoAsync(int materiaId, int instrumentoId);
        Task<IEnumerable<OfertaMateriaVm>> GetOfertasAsync(int materiaId);
        Task<OperationResult> CrearOfertaAsync(CrearOfertaMateriaVm model);
        Task<IEnumerable<string>> GetTiposUnicosAsync();
    }

    public interface IPeriodosAcademicosService
    {
        Task<PagedResult<PeriodoListVm>> GetPagedAsync(int page, int pageSize, string? searchTerm = null, int? tipoFiltro = null);
        Task<PeriodoVm?> GetByIdAsync(int id);
        Task<OperationResult> CreateAsync(CrearPeriodoVm model);
        Task<OperationResult> UpdateAsync(int id, EditarPeriodoVm model);
        Task<OperationResult> DeleteAsync(int id);
        Task<IEnumerable<OfertaMateriaVm>> GetOfertasAsync(int periodoId);
        Task<OperationResult> CrearOfertaAsync(CrearOfertaMateriaVm model);
        Task<OperationResult> CerrarOfertaAsync(int ofertaId);
        Task<IEnumerable<string>> GetTiposUnicosAsync();
    }
}