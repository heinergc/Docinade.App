using Microsoft.EntityFrameworkCore;
using RubricasApp.Web.Data;
using RubricasApp.Web.Models;
using RubricasApp.Web.Models.Academic;
using RubricasApp.Web.Services.Academic;
using RubricasApp.Web.ViewModels.Academic;

namespace RubricasApp.Web.Services.Academic
{
    public class PeriodoAcademicoRepositoryImpl : IPeriodoAcademicoRepository
    {
        private readonly RubricasDbContext _context;

        public PeriodoAcademicoRepositoryImpl(RubricasDbContext context)
        {
            _context = context;
        }

        public async Task<IQueryable<PeriodoAcademico>> GetAllQueryableAsync()
        {
            return _context.PeriodosAcademicos.AsQueryable();
        }

        public async Task<PeriodoAcademico?> GetByIdAsync(int id)
        {
            // 🔧 CORRECCIÓN TEMPORAL: Simplificar la consulta para evitar el error de SQLite
            // El error "no such column: m.PeriodoAcademicoId1" indica problemas con las relaciones
            
            try
            {
                // Paso 1: Obtener el período académico sin relaciones complejas
                var periodo = await _context.PeriodosAcademicos
                    .FirstOrDefaultAsync(p => p.Id == id);
                
                if (periodo == null)
                {
                    return null;
                }
                
                // Paso 2: Cargar las ofertas por separado para evitar conflictos de FK
                var ofertas = await _context.MateriaPeriodos
                    .Where(mp => mp.PeriodoAcademicoId == id)
                    .Include(mp => mp.Materia)
                    .ToListAsync();
                
                // Paso 3: Cargar los estudiantes por separado
                var estudiantes = await _context.Estudiantes
                    .Where(e => e.PeriodoAcademicoId == id)
                    .ToListAsync();
                
                // Crear el objeto con las propiedades de navegación cargadas manualmente
                // Nota: Para que esto funcione completamente, necesitaríamos asignar las colecciones
                // al objeto periodo, pero por ahora esto evitará el error SQLite
                
                return periodo;
            }
            catch (Exception ex)
            {
                // Log del error específico para debugging
                Console.WriteLine($"❌ Error en PeriodoAcademicoRepositoryImpl.GetByIdAsync: {ex.Message}");
                
                // Fallback: intentar consulta más simple
                return await _context.PeriodosAcademicos
                    .Where(p => p.Id == id)
                    .FirstOrDefaultAsync();
            }
        }

        public async Task<IEnumerable<PeriodoAcademico>> GetAllAsync()
        {
            return await _context.PeriodosAcademicos
                .Include(p => p.Ofertas)
                .OrderByDescending(p => p.Anio)
                .ThenByDescending(p => p.Ciclo ?? "")
                .ToListAsync();
        }

        public async Task<PagedResult<PeriodoAcademico>> GetPagedAsync(int page, int pageSize, string? searchTerm = null, int? tipoFiltro = null)
        {
            var query = _context.PeriodosAcademicos.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(p => 
                    (p.Nombre != null && p.Nombre.Contains(searchTerm)) ||
                    (p.Codigo != null && p.Codigo.Contains(searchTerm)) ||
                    (p.Ciclo != null && p.Ciclo.Contains(searchTerm)) ||
                    p.Anio.ToString().Contains(searchTerm));
            }

            // Filtro por tipo de período
            if (tipoFiltro.HasValue)
            {
                query = query.Where(p => (int)p.Tipo == tipoFiltro.Value);
            }

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderByDescending(p => p.Anio)
                .ThenByDescending(p => p.Ciclo ?? "")
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<PeriodoAcademico>
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<IEnumerable<string>> GetTiposUnicosAsync()
        {
            var tiposNumericos = await _context.PeriodosAcademicos
                .Select(p => (int)p.Tipo)
                .Distinct()
                .OrderBy(t => t)
                .ToListAsync();

            // Convertir los números a nombres descriptivos según el enum TipoPeriodo
            return tiposNumericos.Select(t => t switch
            {
                1 => "Semestre",
                2 => "Cuatrimestre", 
                3 => "Trimestre",
                4 => "Anual",
                5 => "Personalizado",
                _ => $"Tipo {t}"
            });
        }

        public async Task<PeriodoAcademico> CreateAsync(PeriodoAcademico periodo)
        {
            _context.PeriodosAcademicos.Add(periodo);
            await _context.SaveChangesAsync();
            return periodo;
        }

        public async Task<PeriodoAcademico> UpdateAsync(PeriodoAcademico periodo)
        {
            _context.Entry(periodo).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return periodo;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var periodo = await _context.PeriodosAcademicos.FindAsync(id);
            if (periodo == null) return false;

            _context.PeriodosAcademicos.Remove(periodo);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.PeriodosAcademicos.AnyAsync(p => p.Id == id);
        }

        public async Task<bool> ExistsByCodigoAsync(string codigo, int? excludeId = null)
        {
            var query = _context.PeriodosAcademicos.Where(p => p.Codigo == codigo);
            if (excludeId.HasValue)
            {
                query = query.Where(p => p.Id != excludeId.Value);
            }
            return await query.AnyAsync();
        }

        public async Task<bool> ExistsByAnioYCicloAsync(int anio, string ciclo, int? excludeId = null)
        {
            var query = _context.PeriodosAcademicos.Where(p => p.Anio == anio && p.Ciclo == ciclo);
            if (excludeId.HasValue)
            {
                query = query.Where(p => p.Id != excludeId.Value);
            }
            return await query.AnyAsync();
        }

        public async Task<IEnumerable<PeriodoAcademico>> GetActivosAsync()
        {
            return await _context.PeriodosAcademicos
                .Where(p => p.Activo)
                .OrderByDescending(p => p.Anio)
                .ThenByDescending(p => p.Ciclo ?? "")
                .ToListAsync();
        }

        public async Task<PeriodoAcademico?> GetPeriodoActualAsync()
        {
            return await _context.PeriodosAcademicos
                .Where(p => p.Activo && p.FechaInicio <= DateTime.Now && p.FechaFin >= DateTime.Now)
                .FirstOrDefaultAsync();
        }
    }
    public class RubricaRepositoryImpl : IRubricaRepository
    {
        private readonly RubricasDbContext _context;

        public RubricaRepositoryImpl(RubricasDbContext context)
        {
            _context = context;
        }

        public async Task<(IEnumerable<Rubrica> items, int totalCount)> GetPagedAsync(
            int page, int pageSize, string? searchTerm = null, string? sortBy = null)
        {
            var query = _context.Rubricas.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(r => r.Titulo.Contains(searchTerm) ||
                                        r.Descripcion!.Contains(searchTerm));
            }

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderBy(r => r.Titulo)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<IEnumerable<Rubrica>> GetAllAsync()
        {
            return await _context.Rubricas
                .OrderBy(r => r.Titulo)
                .ToListAsync();
        }

        public async Task<Rubrica?> GetByIdAsync(int id)
        {
            return await _context.Rubricas.FindAsync(id);
        }

        public async Task<Rubrica> CreateAsync(Rubrica rubrica)
        {
            _context.Rubricas.Add(rubrica);
            await _context.SaveChangesAsync();
            return rubrica;
        }

        public async Task<Rubrica> UpdateAsync(Rubrica rubrica)
        {
            _context.Entry(rubrica).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return rubrica;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var rubrica = await _context.Rubricas.FindAsync(id);
            if (rubrica == null) return false;

            _context.Rubricas.Remove(rubrica);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Rubricas.AnyAsync(r => r.IdRubrica == id);
        }

        public async Task<Rubrica?> GetByCodigoAsync(string codigo)
        {
            return await _context.Rubricas
                .FirstOrDefaultAsync(r => r.NombreRubrica == codigo);
        }

        public async Task<bool> CodigoExistsAsync(string codigo, int? excludeId = null)
        {
            var query = _context.Rubricas.Where(r => r.NombreRubrica == codigo);
            if (excludeId.HasValue)
            {
                query = query.Where(r => r.IdRubrica != excludeId.Value);
            }
            return await query.AnyAsync();
        }

        public async Task<IEnumerable<Materia>> GetMateriasAsync(int rubricaId)
        {
            // TODO: Implementar con el nuevo modelo de InstrumentoRubrica
            return new List<Materia>();
        }
    }
    public class MateriaPeriodoRepositoryImpl : IMateriaPeriodoRepository
    {
        private readonly RubricasDbContext _context;

        public MateriaPeriodoRepositoryImpl(RubricasDbContext context)
        {
            _context = context;
        }

        public async Task<IQueryable<MateriaPeriodo>> GetAllQueryableAsync()
        {
            return _context.MateriaPeriodos
                .Include(mp => mp.Materia)
                .Include(mp => mp.PeriodoAcademico)
                .AsQueryable();
        }

        public async Task<MateriaPeriodo?> GetByIdAsync(int id)
        {
            return await _context.MateriaPeriodos
                .Include(mp => mp.Materia)
                .Include(mp => mp.PeriodoAcademico)
                .FirstOrDefaultAsync(mp => mp.Id == id);
        }

        public async Task<IEnumerable<MateriaPeriodo>> GetAllAsync()
        {
            return await _context.MateriaPeriodos
                .Include(mp => mp.Materia)
                .Include(mp => mp.PeriodoAcademico)
                .OrderByDescending(mp => mp.PeriodoAcademico.Anio)
                .ThenBy(mp => mp.Materia.Nombre)
                .ToListAsync();
        }

        public async Task<PagedResult<MateriaPeriodo>> GetPagedAsync(int page, int pageSize, string? searchTerm = null)
        {
            var query = _context.MateriaPeriodos
                .Include(mp => mp.Materia)
                .Include(mp => mp.PeriodoAcademico)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(mp => mp.Materia.Nombre.Contains(searchTerm) ||
                                         mp.Materia.Codigo.Contains(searchTerm) ||
                                         mp.PeriodoAcademico.Nombre.Contains(searchTerm) ||
                                         mp.Estado.Contains(searchTerm));
            }

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderByDescending(mp => mp.PeriodoAcademico.Anio)
                .ThenBy(mp => mp.Materia.Nombre)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<MateriaPeriodo>
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<MateriaPeriodo> CreateAsync(MateriaPeriodo materiaPeriodo)
        {
            _context.MateriaPeriodos.Add(materiaPeriodo);
            await _context.SaveChangesAsync();
            return materiaPeriodo;
        }

        public async Task<MateriaPeriodo> UpdateAsync(MateriaPeriodo materiaPeriodo)
        {
            _context.Entry(materiaPeriodo).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return materiaPeriodo;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var materiaPeriodo = await _context.MateriaPeriodos.FindAsync(id);
            if (materiaPeriodo == null) return false;

            _context.MateriaPeriodos.Remove(materiaPeriodo);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.MateriaPeriodos.AnyAsync(mp => mp.Id == id);
        }

        public async Task<IEnumerable<MateriaPeriodo>> GetByPeriodoAsync(int periodoId)
        {
            return await _context.MateriaPeriodos
                .Include(mp => mp.Materia)
                .Include(mp => mp.PeriodoAcademico)
                .Where(mp => mp.PeriodoAcademicoId == periodoId)
                .OrderBy(mp => mp.Materia.Nombre)
                .ToListAsync();
        }

        public async Task<IEnumerable<MateriaPeriodo>> GetByMateriaAsync(int materiaId)
        {
            return await _context.MateriaPeriodos
                .Include(mp => mp.Materia)
                .Include(mp => mp.PeriodoAcademico)
                .Where(mp => mp.MateriaId == materiaId)
                .OrderByDescending(mp => mp.PeriodoAcademico.Anio)
                .ThenByDescending(mp => mp.PeriodoAcademico.Ciclo)
                .ToListAsync();
        }

        public async Task<bool> ExisteOfertaAsync(int materiaId, int periodoId)
        {
            return await _context.MateriaPeriodos
                .AnyAsync(mp => mp.MateriaId == materiaId && mp.PeriodoAcademicoId == periodoId);
        }

        public async Task<bool> CerrarOfertaAsync(int ofertaId)
        {
            var oferta = await _context.MateriaPeriodos.FindAsync(ofertaId);
            if (oferta == null) return false;

            oferta.Estado = "Cerrada";
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<MateriaPeriodo?> GetByMateriaYPeriodoAsync(int materiaId, int periodoId)
        {
            return await _context.MateriaPeriodos
                .Include(mp => mp.Materia)
                .Include(mp => mp.PeriodoAcademico)
                .FirstOrDefaultAsync(mp => mp.MateriaId == materiaId && mp.PeriodoAcademicoId == periodoId);
        }
    }

    public class InstrumentoRepositoryImpl : IInstrumentoRepository
    {
    private readonly RubricasDbContext _context;

    public InstrumentoRepositoryImpl(RubricasDbContext context)
    {
    _context = context;
    }

    public async Task<IQueryable<InstrumentoEvaluacion>> GetAllQueryableAsync()
    {
    return _context.InstrumentosEvaluacion
    .AsQueryable();
    }

    public async Task<InstrumentoEvaluacion?> GetByIdAsync(int id)
        {
        return await _context.InstrumentosEvaluacion
            .FirstOrDefaultAsync(ie => ie.InstrumentoId == id);
    }

    public async Task<IEnumerable<InstrumentoEvaluacion>> GetAllAsync()
    {
        return await _context.InstrumentosEvaluacion
                .Where(ie => ie.Activo)
            .OrderBy(ie => ie.Nombre)
            .ToListAsync();
    }

    public async Task<PagedResult<InstrumentoEvaluacion>> GetPagedAsync(int page, int pageSize, string? searchTerm = null)
    {
    var query = _context.InstrumentosEvaluacion.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
            {
            query = query.Where(ie => ie.Nombre.Contains(searchTerm) ||
                                     ie.Descripcion!.Contains(searchTerm));
    }

    var totalCount = await query.CountAsync();
    var items = await query
                .OrderBy(ie => ie.Nombre)
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
    .ToListAsync();

    return new PagedResult<InstrumentoEvaluacion>
    {
                Items = items,
        TotalCount = totalCount,
        Page = page,
    PageSize = pageSize
    };
    }

    public async Task<InstrumentoEvaluacion> CreateAsync(InstrumentoEvaluacion instrumento)
        {
    _context.InstrumentosEvaluacion.Add(instrumento);
    await _context.SaveChangesAsync();
    return instrumento;
    }

    public async Task<InstrumentoEvaluacion> UpdateAsync(InstrumentoEvaluacion instrumento)
    {
        _context.Entry(instrumento).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        return instrumento;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var instrumento = await _context.InstrumentosEvaluacion.FindAsync(id);
            if (instrumento == null) return false;

        _context.InstrumentosEvaluacion.Remove(instrumento);
    await _context.SaveChangesAsync();
    return true;
    }

        public async Task<bool> ExistsAsync(int id)
    {
        return await _context.InstrumentosEvaluacion
        .AnyAsync(ie => ie.InstrumentoId == id);
    }

    public async Task<IEnumerable<InstrumentoEvaluacion>> ObtenerInstrumentosDeMateriaAsync(int materiaId)
        {
    return await _context.InstrumentoMaterias
        .Where(im => im.MateriaId == materiaId)
        .Select(im => im.InstrumentoEvaluacion)
            .ToListAsync();
        }

    public async Task<IEnumerable<Materia>> ObtenerMateriasPorInstrumentoAsync(int instrumentoId)
    {
    return await _context.InstrumentoMaterias
            .Where(im => im.InstrumentoEvaluacionId == instrumentoId)
                .Select(im => im.Materia)
            .ToListAsync();
    }

    public async Task AsignarInstrumentosAMateriaAsync(int materiaId, IEnumerable<int> instrumentoIds, string? observaciones = null)
    {
    // Obtener asignaciones existentes
        var asignacionesExistentes = await _context.InstrumentoMaterias
                .Where(im => im.MateriaId == materiaId)
            .ToListAsync();

    // Eliminar asignaciones que ya no están en la nueva lista
    var instrumentosAEliminar = asignacionesExistentes
    .Where(im => !instrumentoIds.Contains(im.InstrumentoEvaluacionId))
    .ToList();

            if (instrumentosAEliminar.Any())
        {
            _context.InstrumentoMaterias.RemoveRange(instrumentosAEliminar);
    }

    // Agregar nuevas asignaciones
    var instrumentosExistentesIds = asignacionesExistentes.Select(im => im.InstrumentoEvaluacionId).ToList();
            var nuevosInstrumentos = instrumentoIds
        .Where(iid => !instrumentosExistentesIds.Contains(iid))
        .Select(iid => new InstrumentoMateria
    {
        MateriaId = materiaId,
                    InstrumentoEvaluacionId = iid,
            FechaAsignacion = DateTime.Now,
            Observaciones = observaciones
    })
        .ToList();

    if (nuevosInstrumentos.Any())
    {
        _context.InstrumentoMaterias.AddRange(nuevosInstrumentos);
    }

    await _context.SaveChangesAsync();
    }

    public async Task QuitarInstrumentoDeMateriaAsync(int materiaId, int instrumentoId)
    {
    var relacion = await _context.InstrumentoMaterias
    .FirstOrDefaultAsync(im => im.MateriaId == materiaId && im.InstrumentoEvaluacionId == instrumentoId);

    if (relacion != null)
    {
    _context.InstrumentoMaterias.Remove(relacion);
        await _context.SaveChangesAsync();
            }
    }

        public async Task<bool> ExisteRelacionAsync(int materiaId, int instrumentoId)
    {
        return await _context.InstrumentoMaterias
        .AnyAsync(im => im.MateriaId == materiaId && im.InstrumentoEvaluacionId == instrumentoId);
    }
    }

    public class MateriaRepositoryImpl : IMateriaRepository
    {
        private readonly RubricasDbContext _context;

        public MateriaRepositoryImpl(RubricasDbContext context)
        {
            _context = context;
        }

        public async Task<IQueryable<Materia>> GetAllQueryableAsync()
        {
            return _context.Materias.AsQueryable();
        }

        public async Task<Materia?> GetByIdAsync(int id)
        {
            return await _context.Materias
                .Include(m => m.InstrumentoMaterias)
                    .ThenInclude(im => im.InstrumentoEvaluacion)
                .Include(m => m.Ofertas)
                    .ThenInclude(o => o.PeriodoAcademico)
                .FirstOrDefaultAsync(m => m.MateriaId == id);
        }

        public async Task<IEnumerable<Materia>> GetAllAsync()
        {
            return await _context.Materias
                .Include(m => m.InstrumentoMaterias)
                .Include(m => m.Ofertas)
                .OrderBy(m => m.Nombre)
                .ToListAsync();
        }

        public async Task<(IEnumerable<Materia> items, int totalCount)> GetPagedAsync(int page, int pageSize, string? searchTerm = null, string? tipoFiltro = null, string? sortBy = null)
        {
            var query = _context.Materias.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(m => m.Nombre.Contains(searchTerm) ||
                                        m.Codigo.Contains(searchTerm) ||
                                        m.Descripcion!.Contains(searchTerm));
            }

            // Filtro por tipo de materia
            if (!string.IsNullOrWhiteSpace(tipoFiltro))
            {
                query = query.Where(m => m.Tipo == tipoFiltro);
            }

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderBy(m => m.Nombre)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<IEnumerable<string>> GetTiposUnicosAsync()
        {
            return await _context.Materias
                .Where(m => !string.IsNullOrEmpty(m.Tipo))
                .Select(m => m.Tipo!)
                .Distinct()
                .OrderBy(t => t)
                .ToListAsync();
        }

        public async Task<Materia> CreateAsync(Materia materia)
        {
            _context.Materias.Add(materia);
            await _context.SaveChangesAsync();
            return materia;
        }

        public async Task<Materia> UpdateAsync(Materia materia)
        {
            _context.Entry(materia).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return materia;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var materia = await _context.Materias.FindAsync(id);
            if (materia == null) return false;

            _context.Materias.Remove(materia);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Materias.AnyAsync(m => m.MateriaId == id);
        }

        public async Task<bool> ExistsByCodigoAsync(string codigo, int? excludeId = null)
        {
            var query = _context.Materias.Where(m => m.Codigo == codigo);
            if (excludeId.HasValue)
            {
                query = query.Where(m => m.MateriaId != excludeId.Value);
            }
            return await query.AnyAsync();
        }

        public async Task<IEnumerable<Materia>> GetByPeriodoAsync(int periodoId)
        {
            return await _context.Materias
                .Where(m => m.Ofertas.Any(o => o.PeriodoAcademicoId == periodoId))
                .Include(m => m.InstrumentoMaterias)
                .ToListAsync();
        }

        public async Task<IEnumerable<Materia>> GetByInstrumentoAsync(int instrumentoId)
        {
            return await _context.Materias
                .Where(m => m.InstrumentoMaterias.Any(im => im.InstrumentoEvaluacionId == instrumentoId))
                .Include(m => m.Ofertas)
                .ToListAsync();
        }

        public async Task<IEnumerable<Materia>> SearchAsync(string searchTerm)
        {
            return await _context.Materias
                .Where(m => m.Nombre.Contains(searchTerm) ||
                           m.Codigo.Contains(searchTerm) ||
                           m.Descripcion!.Contains(searchTerm))
                .OrderBy(m => m.Nombre)
                .ToListAsync();
        }

        // Métodos adicionales de la interfaz
        public async Task<Materia?> GetByCodigoAsync(string codigo)
        {
            return await _context.Materias
                .Include(m => m.InstrumentoMaterias)
                .Include(m => m.Ofertas)
                .FirstOrDefaultAsync(m => m.Codigo == codigo);
        }

        public async Task<bool> CodigoExistsAsync(string codigo, int? excludeId = null)
        {
            return await ExistsByCodigoAsync(codigo, excludeId);
        }

        public async Task<IEnumerable<InstrumentoEvaluacion>> GetInstrumentosAsync(int materiaId)
        {
            return await _context.InstrumentoMaterias
                .Where(im => im.MateriaId == materiaId)
                .Select(im => im.InstrumentoEvaluacion)
                .ToListAsync();
        }

        public async Task AsignarInstrumentosAsync(int materiaId, IEnumerable<int> instrumentoIds, string? observaciones = null)
        {
            // Eliminar asignaciones existentes
            var existentes = await _context.InstrumentoMaterias
                .Where(im => im.MateriaId == materiaId)
                .ToListAsync();
            _context.InstrumentoMaterias.RemoveRange(existentes);

            // Agregar nuevas asignaciones
            var nuevas = instrumentoIds.Select(iid => new InstrumentoMateria
            {
                MateriaId = materiaId,
                InstrumentoEvaluacionId = iid,
                FechaAsignacion = DateTime.Now,
                Observaciones = observaciones
            });

            _context.InstrumentoMaterias.AddRange(nuevas);
            await _context.SaveChangesAsync();
        }

        public async Task QuitarInstrumentoAsync(int materiaId, int instrumentoId)
        {
            var relacion = await _context.InstrumentoMaterias
                .FirstOrDefaultAsync(im => im.MateriaId == materiaId && im.InstrumentoEvaluacionId == instrumentoId);

            if (relacion != null)
            {
                _context.InstrumentoMaterias.Remove(relacion);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<MateriaPeriodo>> GetOfertasAsync(int materiaId)
        {
            return await _context.MateriaPeriodos
                .Where(mp => mp.MateriaId == materiaId)
                .Include(mp => mp.PeriodoAcademico)
                .ToListAsync();
        }

        public async Task<MateriaPeriodo> CrearOfertaAsync(int materiaId, int periodoId, int cuposMaximos, string seccion)
        {
            var oferta = new MateriaPeriodo
            {
                MateriaId = materiaId,
                PeriodoAcademicoId = periodoId,
                Cupo = cuposMaximos,
                Observaciones = seccion, // Usar observaciones para almacenar la sección
                Estado = "Abierta",
                FechaCreacion = DateTime.Now
            };

            _context.MateriaPeriodos.Add(oferta);
            await _context.SaveChangesAsync();
            return oferta;
        }

        public async Task<bool> CerrarOfertaAsync(int ofertaId)
        {
            var oferta = await _context.MateriaPeriodos.FindAsync(ofertaId);
            if (oferta == null) return false;

            oferta.Estado = "Cerrada";
            // Nota: No hay FechaModificacion en el modelo actual
            await _context.SaveChangesAsync();
            return true;
        }
    }
}