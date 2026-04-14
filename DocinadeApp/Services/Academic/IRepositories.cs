using DocinadeApp.Models;
using DocinadeApp.Models.Academic;
using DocinadeApp.ViewModels.Academic;

namespace DocinadeApp.Services.Academic
{
    public interface IMateriaRepository
    {
        // CRUD básico
        Task<IEnumerable<Materia>> GetAllAsync();
        Task<Materia?> GetByIdAsync(int id);
        Task<Materia?> GetByCodigoAsync(string codigo);
        Task<Materia> CreateAsync(Materia materia);
        Task<Materia> UpdateAsync(Materia materia);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<bool> CodigoExistsAsync(string codigo, int? excludeId = null);

        // Búsqueda y paginación
        Task<(IEnumerable<Materia> items, int totalCount)> GetPagedAsync(
            int page, int pageSize, string? searchTerm = null, string? tipoFiltro = null, string? sortBy = null);
        
        // Obtener tipos únicos de materias
        Task<IEnumerable<string>> GetTiposUnicosAsync();

        // Relaciones con Instrumentos
        Task<IEnumerable<InstrumentoEvaluacion>> GetInstrumentosAsync(int materiaId);
        Task AsignarInstrumentosAsync(int materiaId, IEnumerable<int> instrumentoIds, string? observaciones = null);
        Task QuitarInstrumentoAsync(int materiaId, int instrumentoId);

        // Relaciones con Períodos
        Task<IEnumerable<MateriaPeriodo>> GetOfertasAsync(int materiaId);
        Task<MateriaPeriodo> CrearOfertaAsync(int materiaId, int periodoId, int cupo, string estado);
        Task<bool> CerrarOfertaAsync(int ofertaId);
    }

    public interface IRubricaRepository
    {
        // CRUD básico
        Task<IEnumerable<Rubrica>> GetAllAsync();
        Task<Rubrica?> GetByIdAsync(int id);
        Task<Rubrica?> GetByCodigoAsync(string codigo);
        Task<Rubrica> CreateAsync(Rubrica rubrica);
        Task<Rubrica> UpdateAsync(Rubrica rubrica);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<bool> CodigoExistsAsync(string codigo, int? excludeId = null);

        // Búsqueda y paginación
        Task<(IEnumerable<Rubrica> items, int totalCount)> GetPagedAsync(
            int page, int pageSize, string? searchTerm = null, string? sortBy = null);

        // Relaciones con Materias
        Task<IEnumerable<Materia>> GetMateriasAsync(int rubricaId);
    }

    public interface IPeriodoAcademicoRepository
    {
        Task<IQueryable<PeriodoAcademico>> GetAllQueryableAsync();
        Task<PeriodoAcademico?> GetByIdAsync(int id);
        Task<IEnumerable<PeriodoAcademico>> GetAllAsync();
        Task<PagedResult<PeriodoAcademico>> GetPagedAsync(int page, int pageSize, string? searchTerm = null, int? tipoFiltro = null);
        Task<PeriodoAcademico> CreateAsync(PeriodoAcademico periodo);
        Task<PeriodoAcademico> UpdateAsync(PeriodoAcademico periodo);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<bool> ExistsByCodigoAsync(string codigo, int? excludeId = null);
        Task<bool> ExistsByAnioYCicloAsync(int anio, string ciclo, int? excludeId = null);
        Task<IEnumerable<PeriodoAcademico>> GetActivosAsync();
        Task<PeriodoAcademico?> GetPeriodoActualAsync();
        Task<IEnumerable<string>> GetTiposUnicosAsync();
    }

    public interface IMateriaPeriodoRepository
    {
        Task<IQueryable<MateriaPeriodo>> GetAllQueryableAsync();
        Task<MateriaPeriodo?> GetByIdAsync(int id);
        Task<IEnumerable<MateriaPeriodo>> GetAllAsync();
        Task<PagedResult<MateriaPeriodo>> GetPagedAsync(int page, int pageSize, string? searchTerm = null);
        Task<MateriaPeriodo> CreateAsync(MateriaPeriodo materiaPeriodo);
        Task<MateriaPeriodo> UpdateAsync(MateriaPeriodo materiaPeriodo);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<IEnumerable<MateriaPeriodo>> GetByPeriodoAsync(int periodoId);
        Task<IEnumerable<MateriaPeriodo>> GetByMateriaAsync(int materiaId);
        Task<MateriaPeriodo?> GetByMateriaYPeriodoAsync(int materiaId, int periodoId);
        Task<bool> ExisteOfertaAsync(int materiaId, int periodoId);
        Task<bool> CerrarOfertaAsync(int ofertaId);
    }

    public interface IInstrumentoRepository
    {
        Task<IQueryable<InstrumentoEvaluacion>> GetAllQueryableAsync();
        Task<InstrumentoEvaluacion?> GetByIdAsync(int id);
        Task<IEnumerable<InstrumentoEvaluacion>> GetAllAsync();
        Task<PagedResult<InstrumentoEvaluacion>> GetPagedAsync(int page, int pageSize, string? searchTerm = null);
        Task<InstrumentoEvaluacion> CreateAsync(InstrumentoEvaluacion instrumento);
        Task<InstrumentoEvaluacion> UpdateAsync(InstrumentoEvaluacion instrumento);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<IEnumerable<InstrumentoEvaluacion>> ObtenerInstrumentosDeMateriaAsync(int materiaId);
        Task<IEnumerable<Materia>> ObtenerMateriasPorInstrumentoAsync(int instrumentoId);
        Task AsignarInstrumentosAMateriaAsync(int materiaId, IEnumerable<int> instrumentoIds, string? observaciones = null);
        Task QuitarInstrumentoDeMateriaAsync(int materiaId, int instrumentoId);
        Task<bool> ExisteRelacionAsync(int materiaId, int instrumentoId);
    }
}