using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // ?? CORRECCI�N: Agregar para NotMapped
using DocinadeApp.Models;
using Microsoft.AspNetCore.Mvc.Rendering; // ?? CORRECCI�N: Usar SelectListItem del framework

namespace DocinadeApp.ViewModels.Grupos
{
    public class GrupoEstudianteIndexViewModel
    {
        public List<GrupoEstudianteListItemViewModel> Grupos { get; set; } = new List<GrupoEstudianteListItemViewModel>();
        public FiltrosGrupoViewModel Filtros { get; set; } = new FiltrosGrupoViewModel();
        public PaginacionViewModel Paginacion { get; set; } = new PaginacionViewModel();
        public string? PeriodoActual { get; set; }
        public Dictionary<TipoGrupo, int> EstadisticasPorTipo { get; set; } = new Dictionary<TipoGrupo, int>();
    }

    public class GrupoEstudianteListItemViewModel
    {
        public int GrupoId { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public TipoGrupo TipoGrupo { get; set; }
        public string TipoGrupoDisplay { get; set; } = string.Empty;
        public string? Nivel { get; set; }
        public int? CapacidadMaxima { get; set; }
        public EstadoGrupo Estado { get; set; }
        public string EstadoDisplay { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; }
        public string? CreadoPor { get; set; }
        public int CantidadEstudiantes { get; set; }
        public bool EstaCompleto { get; set; }
        public int EspaciosDisponibles { get; set; }
        public string EstadoCapacidad { get; set; } = string.Empty;
        public List<string> Materias { get; set; } = new List<string>();
        public int? InstitucionId { get; set; }
        public string? InstitucionNombre { get; set; }
    }

    public class FiltrosGrupoViewModel
    {
        [Display(Name = "Per�odo Acad�mico")]
        public int? PeriodoAcademicoId { get; set; }

        [Display(Name = "Tipo de Grupo")]
        public TipoGrupo? TipoGrupo { get; set; }

        [Display(Name = "Estado")]
        public EstadoGrupo? Estado { get; set; }

        [Display(Name = "Nivel")]
        public string? Nivel { get; set; }

        [Display(Name = "C�digo")]
        public string? Codigo { get; set; }

        [Display(Name = "Nombre")]
        public string? Nombre { get; set; }

        [Display(Name = "Solo grupos con capacidad disponible")]
        public bool SoloConEspacio { get; set; }

        [Display(Name = "Materia")]
        public int? MateriaId { get; set; }
        
        [Display(Name = "Institución")]
        public int? InstitucionId { get; set; }
    }

    public class CrearGrupoViewModel
    {
        [Required(ErrorMessage = "El c�digo es requerido")]
        [StringLength(20, ErrorMessage = "El c�digo no puede exceder 20 caracteres")]
        [Display(Name = "Codigo")]
        public string Codigo { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "La descripcion no puede exceder 500 caracteres")]
        [Display(Name = "Descripcion")]
        public string? Descripcion { get; set; }

        // ?? NUEVA IMPLEMENTACI�N: Usar cat�logo de tipos
        [Required(ErrorMessage = "El tipo de grupo es requerido")]
        [Display(Name = "Tipo de Grupo")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un tipo de grupo válido")]
        public int IdTipoGrupo { get; set; }

        // ?? COMPATIBILIDAD: Mantener enum para compatibilidad con c�digo existente
        [NotMapped]
        public TipoGrupo TipoGrupo
        {
            get => IdTipoGrupo switch
            {
                1 => TipoGrupo.Seccion,
                2 => TipoGrupo.Nivel,
                3 => TipoGrupo.Modalidad,
                4 => TipoGrupo.Custom,
                _ => TipoGrupo.Custom
            };
            set => IdTipoGrupo = value switch
            {
                TipoGrupo.Seccion => 1,
                TipoGrupo.Nivel => 2,
                TipoGrupo.Modalidad => 3,
                TipoGrupo.Custom => 4,
                _ => 4
            };
        }

        [StringLength(50, ErrorMessage = "El nivel no puede exceder 50 caracteres")]
        [Display(Name = "Nivel")]
        public string? Nivel { get; set; }

        [Range(1, 200, ErrorMessage = "La capacidad m�xima debe estar entre 1 y 200")]
        [Display(Name = "Capacidad M�xima")]
        public int? CapacidadMaxima { get; set; }

        [Required(ErrorMessage = "El período académico es requerido")]
        [Display(Name = "Período Académico")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un período académico válido")]
        public int PeriodoAcademicoId { get; set; }        
        [Display(Name = "Institución")]
        public int? InstitucionId { get; set; }
        [StringLength(1000, ErrorMessage = "Las observaciones no pueden exceder 1000 caracteres")]
        [Display(Name = "Observaciones")]
        public string? Observaciones { get; set; }

        [Display(Name = "Materias asociadas")]
        public List<int> MateriasSeleccionadas { get; set; } = new List<int>();

        // ?? CORRECCIÓN: Usar SelectListItem del framework para dropdowns
        public List<SelectListItem> PeriodosDisponibles { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> MateriasDisponibles { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> TiposGrupo { get; set; } = new List<SelectListItem>();        public List<SelectListItem> InstitucionesDisponibles { get; set; } = new List<SelectListItem>();    }

    public class EditarGrupoViewModel : CrearGrupoViewModel
    {
        public int GrupoId { get; set; }
        
        [Display(Name = "Estado")]
        public EstadoGrupo Estado { get; set; }
        
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public string? CreadoPor { get; set; }
        public int CantidadEstudiantes { get; set; }
        public int CantidadEstudiantesActuales { get; set; }
        public bool TieneEstudiantesAsignados { get; set; }
    }

    public class DetalleGrupoViewModel
    {
        public int GrupoId { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public TipoGrupo TipoGrupo { get; set; }
        public string TipoGrupoDisplay { get; set; } = string.Empty;
        public string? Nivel { get; set; }
        public int? CapacidadMaxima { get; set; }
        public EstadoGrupo Estado { get; set; }
        public string EstadoDisplay { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public string? CreadoPor { get; set; }
        public string? Observaciones { get; set; }
        
        // Información del período
        public int PeriodoAcademicoId { get; set; }
        public string PeriodoAcademico { get; set; } = string.Empty;
                // Información de la institución
        public int? InstitucionId { get; set; }
        public string? InstitucionNombre { get; set; }
                // Estadísticas
        public int CantidadEstudiantes { get; set; }
        public bool EstaCompleto { get; set; }
        public int EspaciosDisponibles { get; set; }
        public string EstadoCapacidad { get; set; } = string.Empty;
        
        // Estudiantes y materias
        public List<EstudianteEnGrupoViewModel> Estudiantes { get; set; } = new List<EstudianteEnGrupoViewModel>();
        public List<MateriaEnGrupoViewModel> Materias { get; set; } = new List<MateriaEnGrupoViewModel>();
    }

    public class EstudianteEnGrupoViewModel
    {
        public int EstudianteId { get; set; }
        public string NumeroId { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime FechaAsignacion { get; set; }
        public EstadoAsignacion Estado { get; set; }
        public string EstadoDisplay { get; set; } = string.Empty;
        public bool EsGrupoPrincipal { get; set; }
        public string? MotivoAsignacion { get; set; }
        public string? AsignadoPor { get; set; }
    }

    public class MateriaEnGrupoViewModel
    {
        public int MateriaId { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public DateTime FechaAsignacion { get; set; }
        public EstadoAsignacion Estado { get; set; }
        public string EstadoDisplay { get; set; } = string.Empty;
        public string? Observaciones { get; set; }
    }

    public class AsignarEstudiantesViewModel
    {
        public int GrupoId { get; set; }
        public string GrupoNombre { get; set; } = string.Empty;
        public string GrupoCodigo { get; set; } = string.Empty;
        public int? CapacidadMaxima { get; set; }
        public int EstudiantesActuales { get; set; }
        public int EspaciosDisponibles { get; set; }

        [Display(Name = "Estudiantes a asignar")]
        public List<int> EstudiantesSeleccionados { get; set; } = new List<int>();

        [StringLength(200, ErrorMessage = "El motivo no puede exceder 200 caracteres")]
        [Display(Name = "Motivo de asignaci�n")]
        public string? MotivoAsignacion { get; set; }

        [Display(Name = "Es grupo principal")]
        public bool EsGrupoPrincipal { get; set; } = true;

        // Estudiantes disponibles (no est�n en este grupo)
        public List<EstudianteDisponibleViewModel> EstudiantesDisponibles { get; set; } = new List<EstudianteDisponibleViewModel>();

        // Filtros para estudiantes
        public string? FiltroNombre { get; set; }
        public string? FiltroNumeroId { get; set; }
        public string? FiltroEmail { get; set; }
    }

    public class EstudianteDisponibleViewModel
    {
        public int EstudianteId { get; set; }
        public string NumeroId { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? GrupoActual { get; set; } // Grupo principal actual si tiene
        public bool TieneGrupoPrincipal { get; set; }
    }

    public class GrupoEstadisticasViewModel
    {
        public int TotalGrupos { get; set; }
        public int GruposActivos { get; set; }
        public int TotalEstudiantes { get; set; }
        public int EstudiantesSinGrupo { get; set; }
        public Dictionary<TipoGrupo, int> GruposPorTipo { get; set; } = new Dictionary<TipoGrupo, int>();
        public Dictionary<EstadoGrupo, int> GruposPorEstado { get; set; } = new Dictionary<EstadoGrupo, int>();
        public List<GrupoConMasEstudiantesViewModel> GruposConMasEstudiantes { get; set; } = new List<GrupoConMasEstudiantesViewModel>();
        public List<GrupoCompletoViewModel> GruposCompletos { get; set; } = new List<GrupoCompletoViewModel>();
    }

    public class GrupoConMasEstudiantesViewModel
    {
        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public int CantidadEstudiantes { get; set; }
        public int? CapacidadMaxima { get; set; }
        public double PorcentajeOcupacion { get; set; }
    }

    public class GrupoCompletoViewModel
    {
        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public int CapacidadMaxima { get; set; }
        public DateTime? FechaCompletado { get; set; }
    }

    public class PaginacionViewModel
    {
        public int PaginaActual { get; set; } = 1;
        public int ElementosPorPagina { get; set; } = 20;
        public int TotalElementos { get; set; }
        public int TotalPaginas => (int)Math.Ceiling((double)TotalElementos / ElementosPorPagina);
        public bool TienePaginaAnterior => PaginaActual > 1;
        public bool TienePaginaSiguiente => PaginaActual < TotalPaginas;
    }
}