using Microsoft.EntityFrameworkCore;
using DocinadeApp.Data;
using DocinadeApp.Models;
using DocinadeApp.Models.Academic;
using DocinadeApp.ViewModels.Academic;

namespace DocinadeApp.Services.Academic
{
    public class MateriaRepository : IMateriaRepository
    {
        private readonly RubricasDbContext _context;

        public MateriaRepository(RubricasDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Materia>> GetAllAsync()
        {
            return await _context.Materias
                .Include(m => m.MateriaRubricas)
                .Include(m => m.Ofertas)
                .OrderBy(m => m.Codigo)
                .ToListAsync();
        }

        public async Task<Materia?> GetByIdAsync(int id)
        {
            return await _context.Materias
                .Include(m => m.MateriaRubricas)
                    .ThenInclude(mr => mr.Rubrica)
                .Include(m => m.Ofertas)
                    .ThenInclude(o => o.PeriodoAcademico)
                .Include(m => m.Prerequisitos)
                    .ThenInclude(p => p.Requisito)
                .FirstOrDefaultAsync(m => m.MateriaId == id);
        }

        public async Task<Materia?> GetByCodigoAsync(string codigo)
        {
            return await _context.Materias
                .FirstOrDefaultAsync(m => m.Codigo == codigo);
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

        public async Task<bool> CodigoExistsAsync(string codigo, int? excludeId = null)
        {
            var query = _context.Materias.Where(m => m.Codigo == codigo);
            if (excludeId.HasValue)
            {
                query = query.Where(m => m.MateriaId != excludeId.Value);
            }
            return await query.AnyAsync();
        }

        public async Task<(IEnumerable<Materia> items, int totalCount)> GetPagedAsync(
            int page, int pageSize, string? searchTerm = null, string? sortBy = null)
        {
            var query = _context.Materias
                .Include(m => m.MateriaRubricas)
                .Include(m => m.Ofertas)
                .AsQueryable();

            // Filtrar por término de búsqueda
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(m => m.Codigo.Contains(searchTerm) || m.Nombre.Contains(searchTerm));
            }

            // Ordenar
            query = sortBy?.ToLower() switch
            {
                "nombre" => query.OrderBy(m => m.Nombre),
                "creditos" => query.OrderBy(m => m.Creditos),
                "fecha" => query.OrderBy(m => m.FechaCreacion),
                _ => query.OrderBy(m => m.Codigo)
            };

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<IEnumerable<Rubrica>> GetRubricasAsync(int materiaId)
        {
            return await _context.MateriaRubricas
                .Where(mr => mr.MateriaId == materiaId)
                .Include(mr => mr.Rubrica)
                .Select(mr => mr.Rubrica)
                .ToListAsync();
        }

        public async Task AsignarRubricasAsync(int materiaId, IEnumerable<int> rubricaIds, string? observaciones = null)
        {
            // Eliminar asignaciones existentes
            var existentes = await _context.MateriaRubricas
                .Where(mr => mr.MateriaId == materiaId)
                .ToListAsync();
            _context.MateriaRubricas.RemoveRange(existentes);

            // Agregar nuevas asignaciones
            foreach (var rubricaId in rubricaIds)
            {
                _context.MateriaRubricas.Add(new MateriaRubrica
                {
                    MateriaId = materiaId,
                    RubricaId = rubricaId,
                    FechaAsignacion = DateTime.Now,
                    Observaciones = observaciones
                });
            }

            await _context.SaveChangesAsync();
        }

        public async Task QuitarRubricaAsync(int materiaId, int rubricaId)
        {
            var relacion = await _context.MateriaRubricas
                .FirstOrDefaultAsync(mr => mr.MateriaId == materiaId && mr.RubricaId == rubricaId);
            
            if (relacion != null)
            {
                _context.MateriaRubricas.Remove(relacion);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<MateriaPeriodo>> GetOfertasAsync(int materiaId)
        {
            return await _context.MateriaPeriodos
                .Where(mp => mp.MateriaId == materiaId)
                .Include(mp => mp.PeriodoAcademico)
                .OrderByDescending(mp => mp.FechaCreacion)
                .ToListAsync();
        }

        public async Task<MateriaPeriodo> CrearOfertaAsync(int materiaId, int periodoId, int cupo, string estado)
        {
            var oferta = new MateriaPeriodo
            {
                MateriaId = materiaId,
                PeriodoAcademicoId = periodoId,
                Cupo = cupo,
                Estado = estado,
                FechaCreacion = DateTime.Now
            };

            _context.MateriaPeriodos.Add(oferta);
            await _context.SaveChangesAsync();
            return oferta;
        }

        public async Task<bool> CerrarOfertaAsync(int ofertaId)
        {
            var oferta = await _context.MateriaPeriodos.FindAsync(ofertaId);
            if (oferta == null || oferta.Estado == "Cerrada") return false;

            oferta.Estado = "Cerrada";
            await _context.SaveChangesAsync();
            return true;
        }
    }

    public class RubricaRepository : IRubricaRepository
    {
        private readonly RubricasDbContext _context;

        public RubricaRepository(RubricasDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Rubrica>> GetAllAsync()
        {
            return await _context.Rubricas
                .Include(r => r.MateriaRubricas)
                .OrderBy(r => r.NombreRubrica)
                .ToListAsync();
        }

        public async Task<Rubrica?> GetByIdAsync(int id)
        {
            return await _context.Rubricas
                .Include(r => r.MateriaRubricas)
                    .ThenInclude(mr => mr.Materia)
                .Include(r => r.ItemsEvaluacion)
                .FirstOrDefaultAsync(r => r.IdRubrica == id);
        }

        public async Task<Rubrica?> GetByCodigoAsync(string codigo)
        {
            return await _context.Rubricas
                .FirstOrDefaultAsync(r => r.NombreRubrica == codigo);
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

        public async Task<bool> CodigoExistsAsync(string codigo, int? excludeId = null)
        {
            var query = _context.Rubricas.Where(r => r.NombreRubrica == codigo);
            if (excludeId.HasValue)
            {
                query = query.Where(r => r.IdRubrica != excludeId.Value);
            }
            return await query.AnyAsync();
        }

        public async Task<(IEnumerable<Rubrica> items, int totalCount)> GetPagedAsync(
            int page, int pageSize, string? searchTerm = null, string? sortBy = null)
        {
            var query = _context.Rubricas
                .Include(r => r.MateriaRubricas)
                .AsQueryable();

            // Filtrar por término de búsqueda
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(r => r.NombreRubrica.Contains(searchTerm) || (r.Descripcion != null && r.Descripcion.Contains(searchTerm)));
            }

            // Ordenar
            query = sortBy?.ToLower() switch
            {
                "titulo" => query.OrderBy(r => r.NombreRubrica),
                "fecha" => query.OrderBy(r => r.FechaCreacion),
                "vigente" => query.OrderBy(r => r.EsPublica),
                _ => query.OrderBy(r => r.NombreRubrica)
            };

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<IEnumerable<Materia>> GetMateriasAsync(int rubricaId)
        {
            return await _context.MateriaRubricas
                .Where(mr => mr.RubricaId == rubricaId)
                .Include(mr => mr.Materia)
                .Select(mr => mr.Materia)
                .ToListAsync();
        }
    }

    public class PeriodoAcademicoRepository : IPeriodoAcademicoRepository
    {
        private readonly RubricasDbContext _context;

        public PeriodoAcademicoRepository(RubricasDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PeriodoAcademico>> GetAllAsync()
        {
            return await _context.PeriodosAcademicos
                .Include(p => p.Ofertas)
                .OrderByDescending(p => p.Anio)
                .ThenBy(p => p.Ciclo)
                .ToListAsync();
        }

        public async Task<PeriodoAcademico?> GetByIdAsync(int id)
        {
            // 🔧 CORRECCIÓN: Remover Include problemático que causa error SQLite
            // Error original: "no such column: m.PeriodoAcademicoId1"
            // Esto sucede por conflictos en los nombres de FK generados por EF Core
            
            return await _context.PeriodosAcademicos
                .FirstOrDefaultAsync(p => p.Id == id);
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

        public async Task<(IEnumerable<PeriodoAcademico> items, int totalCount)> GetPagedAsync(
            int page, int pageSize, string? searchTerm = null, string? sortBy = null)
        {
            var query = _context.PeriodosAcademicos
                .Include(p => p.Ofertas)
                .AsQueryable();

            // Filtrar por término de búsqueda
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                if (int.TryParse(searchTerm, out int year))
                {
                    query = query.Where(p => p.Anio == year || p.Ciclo.Contains(searchTerm));
                }
                else
                {
                    query = query.Where(p => p.Ciclo.Contains(searchTerm) || 
                                            p.Nombre.Contains(searchTerm) || 
                                            p.Codigo.Contains(searchTerm));
                }
            }

            // Ordenar
            query = sortBy?.ToLower() switch
            {
                "ciclo" => query.OrderBy(p => p.Ciclo),
                "inicio" => query.OrderBy(p => p.FechaInicio),
                "activo" => query.OrderByDescending(p => p.Activo),
                _ => query.OrderByDescending(p => p.Año).ThenBy(p => p.Ciclo)
            };

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<IEnumerable<PeriodoAcademico>> GetActivosAsync()
        {
            return await _context.PeriodosAcademicos
                .Where(p => p.Activo)
                .OrderByDescending(p => p.Anio)
                .ThenBy(p => p.Ciclo)
                .ToListAsync();
        }

        public async Task<bool> ActivarAsync(int id)
        {
            var periodo = await _context.PeriodosAcademicos.FindAsync(id);
            if (periodo == null) return false;

            // Desactivar otros períodos si es necesario (o mantener múltiples activos)
            periodo.Activo = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<MateriaPeriodo>> GetOfertasAsync(int periodoId)
        {
            return await _context.MateriaPeriodos
                .Where(mp => mp.PeriodoAcademicoId == periodoId)
                .Include(mp => mp.Materia)
                .OrderBy(mp => mp.Materia.Codigo)
                .ToListAsync();
        }

        public async Task<bool> ValidarFechasAsync(DateTime fechaInicio, DateTime fechaFin, int? excludeId = null)
        {
            if (fechaInicio >= fechaFin) return false;

            var query = _context.PeriodosAcademicos.AsQueryable();
            if (excludeId.HasValue)
            {
                query = query.Where(p => p.Id != excludeId.Value);
            }

            // Verificar que no se solape con otros períodos
            var solapamiento = await query.AnyAsync(p =>
                (fechaInicio >= p.FechaInicio && fechaInicio <= p.FechaFin) ||
                (fechaFin >= p.FechaInicio && fechaFin <= p.FechaFin) ||
                (fechaInicio <= p.FechaInicio && fechaFin >= p.FechaFin));

            return !solapamiento;
        }

        // Métodos adicionales requeridos por la interfaz
        public async Task<IQueryable<PeriodoAcademico>> GetAllQueryableAsync()
        {
            return _context.PeriodosAcademicos.AsQueryable();
        }

        public async Task<PagedResult<PeriodoAcademico>> GetPagedAsync(int page, int pageSize, string? searchTerm = null)
        {
            var query = _context.PeriodosAcademicos.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(p => p.Codigo.Contains(searchTerm) ||
                                        p.Nombre.Contains(searchTerm) ||
                                        p.Ciclo.Contains(searchTerm));
            }

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderByDescending(p => p.Anio)
                .ThenBy(p => p.Ciclo)
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

        public async Task<PeriodoAcademico?> GetPeriodoActualAsync()
        {
            var fechaActual = DateTime.Now;
            return await _context.PeriodosAcademicos
                .Where(p => p.Activo && p.FechaInicio <= fechaActual && p.FechaFin >= fechaActual)
                .OrderByDescending(p => p.FechaInicio)
                .FirstOrDefaultAsync();
        }
    }
}