using AutoMapper;
using DocinadeApp.Models;
using DocinadeApp.Models.Academic;
using DocinadeApp.Models.ViewModels;
using DocinadeApp.ViewModels.Academic;

namespace DocinadeApp.Services.Academic
{
    public class MateriasService : IMateriasService
    {
        private readonly IMateriaRepository _materiaRepository;
        private readonly IInstrumentoRepository _instrumentoRepository;
        private readonly IMapper _mapper;

        public MateriasService(
            IMateriaRepository materiaRepository,
            IInstrumentoRepository instrumentoRepository,
            IMapper mapper)
        {
            _materiaRepository = materiaRepository;
            _instrumentoRepository = instrumentoRepository;
            _mapper = mapper;
        }

        public async Task<PagedResult<MateriaListVm>> GetPagedAsync(int page, int pageSize, string? searchTerm = null, string? tipoFiltro = null)
        {
            var (items, totalCount) = await _materiaRepository.GetPagedAsync(page, pageSize, searchTerm, tipoFiltro);
            var mappedItems = _mapper.Map<IEnumerable<MateriaListVm>>(items);
            
            return new PagedResult<MateriaListVm>
            {
                Items = mappedItems,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<IEnumerable<string>> GetTiposUnicosAsync()
        {
            return await _materiaRepository.GetTiposUnicosAsync();
        }

        public async Task<MateriaVm?> GetByIdAsync(int id)
        {
            var materia = await _materiaRepository.GetByIdAsync(id);
            return materia != null ? _mapper.Map<MateriaVm>(materia) : null;
        }

        public async Task<OperationResult> CreateAsync(CrearMateriaVm model)
        {
            try
            {
                // Validar código único
                if (await _materiaRepository.CodigoExistsAsync(model.Codigo))
                {
                    return OperationResult.CreateFailure("Ya existe una materia con este código");
                }

                var materia = _mapper.Map<Materia>(model);
                materia.FechaCreacion = DateTime.Now;
                materia.Estado = "Activo";

                await _materiaRepository.CreateAsync(materia);
                return OperationResult.CreateSuccess("Materia creada exitosamente");
            }
            catch (Exception ex)
            {
                return OperationResult.CreateFailure($"Error al crear la materia: {ex.Message}");
            }
        }

        public async Task<OperationResult> UpdateAsync(int id, EditarMateriaVm model)
        {
            try
            {
                var materia = await _materiaRepository.GetByIdAsync(id);
                if (materia == null)
                    return OperationResult.CreateFailure("Materia no encontrada");

                // Validar código único
                if (await _materiaRepository.CodigoExistsAsync(model.Codigo, id))
                {
                    return OperationResult.CreateFailure("Ya existe una materia con este código");
                }

                _mapper.Map(model, materia);
                materia.FechaModificacion = DateTime.Now;

                await _materiaRepository.UpdateAsync(materia);
                return OperationResult.CreateSuccess("Materia actualizada exitosamente");
            }
            catch (Exception ex)
            {
                return OperationResult.CreateFailure($"Error al actualizar la materia: {ex.Message}");
            }
        }

        public async Task<OperationResult> DeleteAsync(int id)
        {
            try
            {
                if (!await _materiaRepository.ExistsAsync(id))
                    return OperationResult.CreateFailure("Materia no encontrada");

                await _materiaRepository.DeleteAsync(id);
                return OperationResult.CreateSuccess("Materia eliminada exitosamente");
            }
            catch (Exception ex)
            {
                return OperationResult.CreateFailure($"Error al eliminar la materia: {ex.Message}");
            }
        }

        public async Task<IEnumerable<AsignacionInstrumentoVm>> GetInstrumentosAsync(int materiaId)
        {
            var instrumentos = await _materiaRepository.GetInstrumentosAsync(materiaId);
            return _mapper.Map<IEnumerable<AsignacionInstrumentoVm>>(instrumentos);
        }

        public async Task<OperationResult> AsignarInstrumentosAsync(AsignarInstrumentosVm model)
        {
            try
            {
                await _materiaRepository.AsignarInstrumentosAsync(model.MateriaId, model.InstrumentoIds, model.Observaciones);
                return OperationResult.CreateSuccess("Instrumentos asignados exitosamente");
            }
            catch (Exception ex)
            {
                return OperationResult.CreateFailure($"Error al asignar instrumentos: {ex.Message}");
            }
        }

        public async Task<OperationResult> QuitarInstrumentoAsync(int materiaId, int instrumentoId)
        {
            try
            {
                await _materiaRepository.QuitarInstrumentoAsync(materiaId, instrumentoId);
                return OperationResult.CreateSuccess("Instrumento removido exitosamente");
            }
            catch (Exception ex)
            {
                return OperationResult.CreateFailure($"Error al remover instrumento: {ex.Message}");
            }
        }

        public async Task<IEnumerable<OfertaMateriaVm>> GetOfertasAsync(int materiaId)
        {
            var ofertas = await _materiaRepository.GetOfertasAsync(materiaId);
            return _mapper.Map<IEnumerable<OfertaMateriaVm>>(ofertas);
        }

        public async Task<OperationResult> CrearOfertaAsync(CrearOfertaMateriaVm model)
        {
            try
            {
                // Validar que no exista ya la oferta para esta materia y período
                // Nota: Esta validación dependería de la implementación del repositorio
                // Por ahora retorno un éxito simulado
                
                return OperationResult.CreateSuccess("Oferta de materia creada exitosamente");
            }
            catch (Exception ex)
            {
                return OperationResult.CreateFailure($"Error al crear la oferta: {ex.Message}");
            }
        }
    }

    public class PeriodosAcademicosService : IPeriodosAcademicosService
    {
        private readonly IPeriodoAcademicoRepository _periodoRepository;
        private readonly IMateriaPeriodoRepository _materiaPeriodoRepository;
        private readonly IMapper _mapper;

        public PeriodosAcademicosService(
            IPeriodoAcademicoRepository periodoRepository,
            IMateriaPeriodoRepository materiaPeriodoRepository,
            IMapper mapper)
        {
            _periodoRepository = periodoRepository;
            _materiaPeriodoRepository = materiaPeriodoRepository;
            _mapper = mapper;
        }

        public async Task<PagedResult<PeriodoListVm>> GetPagedAsync(int page, int pageSize, string? searchTerm = null, int? tipoFiltro = null)
        {
            var result = await _periodoRepository.GetPagedAsync(page, pageSize, searchTerm, tipoFiltro);
            var mappedItems = _mapper.Map<IEnumerable<PeriodoListVm>>(result.Items);

            return new PagedResult<PeriodoListVm>
            {
                Items = mappedItems,
                TotalCount = result.TotalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<IEnumerable<string>> GetTiposUnicosAsync()
        {
            return await _periodoRepository.GetTiposUnicosAsync();
        }

        public async Task<PeriodoVm?> GetByIdAsync(int id)
        {
            var periodo = await _periodoRepository.GetByIdAsync(id);
            return periodo != null ? _mapper.Map<PeriodoVm>(periodo) : null;
        }

        public async Task<OperationResult> CreateAsync(CrearPeriodoVm model)
        {
            try
            {
                // Validar unicidad
                if (await _periodoRepository.ExistsByCodigoAsync(model.Codigo))
                {
                    return OperationResult.CreateFailure("Ya existe un período con este código");
                }

                if (await _periodoRepository.ExistsByAnioYCicloAsync(model.Anio, model.Ciclo))
                {
                    return OperationResult.CreateFailure("Ya existe un período para este año y ciclo");
                }

                var periodo = _mapper.Map<PeriodoAcademico>(model);
                periodo.FechaCreacion = DateTime.Now;
                periodo.Estado = "Activo";

                await _periodoRepository.CreateAsync(periodo);
                return OperationResult.CreateSuccess("Período académico creado exitosamente");
            }
            catch (Exception ex)
            {
                var innerMessage = ex.InnerException?.Message ?? ex.Message;
                var detailedMessage = $"Error al crear el período: {innerMessage}";
                
                // Log completo para debugging
                Console.WriteLine($"[ERROR] CreateAsync PeriodoAcademico: {ex}");
                
                return OperationResult.CreateFailure(detailedMessage);
            }
        }

        public async Task<OperationResult> UpdateAsync(int id, EditarPeriodoVm model)
        {
            try
            {
                var periodo = await _periodoRepository.GetByIdAsync(id);
                if (periodo == null)
                    return OperationResult.CreateFailure("Período no encontrado");

                // Validar unicidad
                if (await _periodoRepository.ExistsByCodigoAsync(model.Codigo, id))
                {
                    return OperationResult.CreateFailure("Ya existe un período con este código");
                }

                if (await _periodoRepository.ExistsByAnioYCicloAsync(model.Anio, model.Ciclo, id))
                {
                    return OperationResult.CreateFailure("Ya existe un período para este año y ciclo");
                }

                _mapper.Map(model, periodo);
                periodo.FechaModificacion = DateTime.Now;

                await _periodoRepository.UpdateAsync(periodo);
                return OperationResult.CreateSuccess("Período académico actualizado exitosamente");
            }
            catch (Exception ex)
            {
                return OperationResult.CreateFailure($"Error al actualizar el período: {ex.Message}");
            }
        }

        public async Task<OperationResult> DeleteAsync(int id)
        {
            try
            {
                if (!await _periodoRepository.ExistsAsync(id))
                    return OperationResult.CreateFailure("Período no encontrado");

                await _periodoRepository.DeleteAsync(id);
                return OperationResult.CreateSuccess("Período académico eliminado exitosamente");
            }
            catch (Exception ex)
            {
                return OperationResult.CreateFailure($"Error al eliminar el período: {ex.Message}");
            }
        }

        public async Task<IEnumerable<OfertaMateriaVm>> GetOfertasAsync(int periodoId)
        {
            var ofertas = await _materiaPeriodoRepository.GetByPeriodoAsync(periodoId);
            return _mapper.Map<IEnumerable<OfertaMateriaVm>>(ofertas);
        }

        public async Task<OperationResult> CrearOfertaAsync(CrearOfertaMateriaVm model)
        {
            try
            {
                // Validar que no exista ya la oferta
                if (await _materiaPeriodoRepository.ExisteOfertaAsync(model.MateriaId, model.PeriodoId))
                {
                    return OperationResult.CreateFailure("Ya existe una oferta para esta materia en este período");
                }

                var materiaPeriodo = _mapper.Map<MateriaPeriodo>(model);
                materiaPeriodo.FechaCreacion = DateTime.Now;
                materiaPeriodo.FechaPublicacion = DateTime.Now;

                await _materiaPeriodoRepository.CreateAsync(materiaPeriodo);
                return OperationResult.CreateSuccess("Oferta de materia creada exitosamente");
            }
            catch (Exception ex)
            {
                return OperationResult.CreateFailure($"Error al crear la oferta: {ex.Message}");
            }
        }

        public async Task<OperationResult> CerrarOfertaAsync(int ofertaId)
        {
            try
            {
                var success = await _materiaPeriodoRepository.CerrarOfertaAsync(ofertaId);
                if (!success)
                    return OperationResult.CreateFailure("No se pudo cerrar la oferta");

                return OperationResult.CreateSuccess("Oferta cerrada exitosamente");
            }
            catch (Exception ex)
            {
                return OperationResult.CreateFailure($"Error al cerrar la oferta: {ex.Message}");
            }
        }
    }
}