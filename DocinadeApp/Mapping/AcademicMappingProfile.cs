using AutoMapper;
using RubricasApp.Web.Models;
using RubricasApp.Web.Models.Academic;
using RubricasApp.Web.ViewModels.Academic;

namespace RubricasApp.Web.Mapping
{
    public class AcademicMappingProfile : Profile
    {
        public AcademicMappingProfile()
        {
            CreateMap<string, TipoPeriodo>().ConvertUsing(tipo => ParseTipoPeriodo(tipo));

            // ============== MATERIA MAPPINGS ==============

            // Mapeos para listas de materias
            CreateMap<Materia, MateriaListVm>()
                .ForMember(dest => dest.TotalInstrumentos, opt => opt.MapFrom(src => src.InstrumentoMaterias.Count));

            // Mapeo para vista detallada de materia
            CreateMap<Materia, MateriaVm>()
                .ForMember(dest => dest.Activa, opt => opt.MapFrom(src => src.Activa))
                .ForMember(dest => dest.Estado, opt => opt.MapFrom(src => src.Estado ?? "Activo"));

            // Mapeos desde ViewModels a entidad
            CreateMap<CrearMateriaVm, Materia>()
                .ForMember(dest => dest.MateriaId, opt => opt.Ignore())
                .ForMember(dest => dest.FechaCreacion, opt => opt.Ignore())
                .ForMember(dest => dest.FechaModificacion, opt => opt.Ignore())
                .ForMember(dest => dest.InstrumentoMaterias, opt => opt.Ignore())
                .ForMember(dest => dest.Ofertas, opt => opt.Ignore());

            CreateMap<EditarMateriaVm, Materia>()
                .ForMember(dest => dest.MateriaId, opt => opt.Ignore())
                .ForMember(dest => dest.FechaCreacion, opt => opt.Ignore())
                .ForMember(dest => dest.FechaModificacion, opt => opt.Ignore())
                .ForMember(dest => dest.InstrumentoMaterias, opt => opt.Ignore())
                .ForMember(dest => dest.Ofertas, opt => opt.Ignore());

            // Mapeo desde entidad a ViewModel de edición
            CreateMap<Materia, EditarMateriaVm>();

            CreateMap<Materia, MateriaItemVm>()
                .ForMember(dest => dest.TotalRubricas, opt => opt.MapFrom(src => 0)) // Legacy: ya no usamos rubricas directas
                .ForMember(dest => dest.TotalOfertas, opt => opt.MapFrom(src => src.Ofertas.Count))
                .ForMember(dest => dest.TotalInstrumentos, opt => opt.MapFrom(src => src.InstrumentoMaterias.Count));

            CreateMap<Materia, MateriaEditVm>()
                .ForMember(dest => dest.MateriaId, opt => opt.MapFrom(src => (int?)src.MateriaId));

            CreateMap<MateriaEditVm, Materia>()
                .ForMember(dest => dest.MateriaId, opt => opt.MapFrom(src => src.MateriaId ?? 0))
                .ForMember(dest => dest.FechaCreacion, opt => opt.Ignore());

            CreateMap<Materia, MateriaDetalleVm>()
                .ForMember(dest => dest.RubricasAsignadas, opt => opt.MapFrom(src => 
                    src.InstrumentoMaterias.Select(im => im.InstrumentoEvaluacion)))
                .ForMember(dest => dest.Ofertas, opt => opt.MapFrom(src => src.Ofertas));

            // ============== RUBRICA MAPPINGS ==============

            // Mapeo para asignaciones de rúbricas
            CreateMap<Rubrica, AsignacionRubricaVm>();

            CreateMap<Rubrica, RubricaItemVm>()
                .ForMember(dest => dest.TotalMaterias, opt => opt.MapFrom(src => 0)); // TODO: Implementar con InstrumentoRubrica

            CreateMap<Rubrica, RubricaEditVm>()
                .ForMember(dest => dest.IdRubrica, opt => opt.MapFrom(src => (int?)src.IdRubrica));

            CreateMap<RubricaEditVm, Rubrica>()
                .ForMember(dest => dest.IdRubrica, opt => opt.MapFrom(src => src.IdRubrica ?? 0))
                .ForMember(dest => dest.FechaCreacion, opt => opt.Ignore())
                .ForMember(dest => dest.NombreRubrica, opt => opt.MapFrom(src => src.Titulo))
                .ForMember(dest => dest.Estado, opt => opt.MapFrom(src => src.Vigente ? "ACTIVO" : "INACTIVO"));

            CreateMap<Rubrica, RubricaDetalleVm>()
                .ForMember(dest => dest.MateriasAsignadas, opt => opt.MapFrom(src => 
                    new List<Materia>())); // TODO: Implementar con InstrumentoRubrica

            // ============== PERIODO ACADEMICO MAPPINGS ==============

            // Mapeo para listas de períodos
            CreateMap<PeriodoAcademico, PeriodoListVm>()
                .ForMember(dest => dest.Tipo, opt => opt.MapFrom(src => src.Tipo.ToString()))
                .ForMember(dest => dest.Anio, opt => opt.MapFrom(src => src.Anio));

            // Mapeo para vista de período individual
            CreateMap<PeriodoAcademico, PeriodoVm>()
                .ForMember(dest => dest.Tipo, opt => opt.MapFrom(src => src.Tipo.ToString()))
                .ForMember(dest => dest.Anio, opt => opt.MapFrom(src => src.Anio));

            // Mapeo desde ViewModel de creación a entidad
            CreateMap<CrearPeriodoVm, PeriodoAcademico>()
                .ForMember(dest => dest.Tipo, opt => opt.MapFrom(src => ParseTipoPeriodo(src.Tipo)))
                .ForMember(dest => dest.Codigo, opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.Codigo) ? "" : src.Codigo))
                .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.Nombre) ? "" : src.Nombre))
                .ForMember(dest => dest.Ciclo, opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.Ciclo) ? "" : src.Ciclo))
                .ForMember(dest => dest.Anio, opt => opt.MapFrom(src => src.Anio))
                .ForMember(dest => dest.FechaCreacion, opt => opt.Ignore())
                .ForMember(dest => dest.FechaModificacion, opt => opt.Ignore())
                .ForMember(dest => dest.Estado, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Estudiantes, opt => opt.Ignore())
                .ForMember(dest => dest.Ofertas, opt => opt.Ignore());

            // Mapeo desde ViewModel de edición a entidad
            CreateMap<EditarPeriodoVm, PeriodoAcademico>()
                .ForMember(dest => dest.Tipo, opt => opt.MapFrom(src => ParseTipoPeriodo(src.Tipo)))
                .ForMember(dest => dest.Codigo, opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.Codigo) ? "" : src.Codigo))
                .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.Nombre) ? "" : src.Nombre))
                .ForMember(dest => dest.Ciclo, opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.Ciclo) ? "" : src.Ciclo))
                .ForMember(dest => dest.Anio, opt => opt.MapFrom(src => src.Anio))
                .ForMember(dest => dest.FechaCreacion, opt => opt.Ignore())
                .ForMember(dest => dest.FechaModificacion, opt => opt.Ignore())
                .ForMember(dest => dest.Estado, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Estudiantes, opt => opt.Ignore())
                .ForMember(dest => dest.Ofertas, opt => opt.Ignore());

            // Mapeo desde entidad a ViewModel de edición
            CreateMap<PeriodoAcademico, EditarPeriodoVm>()
                .ForMember(dest => dest.Tipo, opt => opt.MapFrom(src => src.Tipo.ToString()))
                .ForMember(dest => dest.Anio, opt => opt.MapFrom(src => src.Anio));

            CreateMap<PeriodoAcademico, PeriodoItemVm>()
                .ForMember(dest => dest.Anio, opt => opt.MapFrom(src => src.Anio))
                .ForMember(dest => dest.Tipo, opt => opt.MapFrom(src => src.Tipo.ToString()))
                .ForMember(dest => dest.TotalOfertas, opt => opt.MapFrom(src => src.Ofertas.Count));

            CreateMap<PeriodoAcademico, PeriodoEditVm>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => (int?)src.Id));

            CreateMap<PeriodoEditVm, PeriodoAcademico>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id ?? 0))
                .ForMember(dest => dest.Anio, opt => opt.MapFrom(src => src.Anio)) // Compatibilidad
                .ForMember(dest => dest.Codigo, opt => opt.MapFrom(src => $"{src.Anio}-{src.Ciclo}"))
                .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => $"{src.Ciclo} {src.Anio}"))
                .ForMember(dest => dest.Tipo, opt => opt.MapFrom(src => 
                    src.Ciclo.ToLower().Contains("cuatri") ? TipoPeriodo.Cuatrimestre :
                    src.Ciclo.ToLower().Contains("semes") ? TipoPeriodo.Semestre :
                    TipoPeriodo.Cuatrimestre))
                .ForMember(dest => dest.NumeroPeriodo, opt => opt.MapFrom(src => 
                    src.Ciclo.Contains("I") && !src.Ciclo.Contains("II") ? 1 :
                    src.Ciclo.Contains("II") ? 2 : 3));

            CreateMap<PeriodoAcademico, PeriodoDetalleVm>()
                .ForMember(dest => dest.Anio, opt => opt.MapFrom(src => src.Anio))
                .ForMember(dest => dest.Ofertas, opt => opt.MapFrom(src => src.Ofertas));

            // ============== MATERIA PERIODO MAPPINGS ==============

            CreateMap<MateriaPeriodo, MateriaPeriodoItemVm>()
                .ForMember(dest => dest.MateriaCodigo, opt => opt.MapFrom(src => src.Materia.Codigo))
                .ForMember(dest => dest.MateriaNombre, opt => opt.MapFrom(src => src.Materia.Nombre))
                .ForMember(dest => dest.PeriodoNombre, opt => opt.MapFrom(src => 
                    $"{src.PeriodoAcademico.Anio} - {src.PeriodoAcademico.Ciclo}"));

            // Mapeo para ofertas de materias
            CreateMap<MateriaPeriodo, OfertaMateriaVm>()
                .ForMember(dest => dest.MateriaCodigo, opt => opt.MapFrom(src => src.Materia.Codigo))
                .ForMember(dest => dest.MateriaNombre, opt => opt.MapFrom(src => src.Materia.Nombre))
                .ForMember(dest => dest.PeriodoNombre, opt => opt.MapFrom(src => 
                    $"{src.PeriodoAcademico.Anio} - {src.PeriodoAcademico.Ciclo}"));

            CreateMap<CrearOfertaMateriaVm, MateriaPeriodo>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.FechaCreacion, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.FechaPublicacion, opt => opt.Ignore())
                .ForMember(dest => dest.Observaciones, opt => opt.Ignore());

            // ============== CROSS-REFERENCE MAPPINGS ==============

            // Para mostrar materias en detalles de rúbricas
            CreateMap<Materia, MateriaItemVm>()
                .ForMember(dest => dest.TotalRubricas, opt => opt.MapFrom(src => src.InstrumentoMaterias.Count))
                .ForMember(dest => dest.TotalOfertas, opt => opt.MapFrom(src => src.Ofertas.Count));

            // Para mostrar rúbricas en detalles de materias
            CreateMap<Rubrica, RubricaItemVm>()
                .ForMember(dest => dest.TotalMaterias, opt => opt.MapFrom(src => 0)); // TODO: Implementar con InstrumentoRubrica
        }

        private static TipoPeriodo ParseTipoPeriodo(string tipo)
        {
            if (Enum.TryParse<TipoPeriodo>(tipo, true, out var result))
            {
                return result;
            }
            return TipoPeriodo.Semestre; // Valor por defecto
        }
    }
}