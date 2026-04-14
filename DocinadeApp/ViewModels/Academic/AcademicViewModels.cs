using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DocinadeApp.ViewModels.Academic
{
    // ============== SHARED CLASSES ==============
    
    public class PagedResult<T>
    {
        public IEnumerable<T> Items { get; set; } = new List<T>();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int PageNumber => Page; // Alias para compatibilidad
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public bool HasPreviousPage => Page > 1;
        public bool HasNextPage => Page < TotalPages;
    }

    public class OperationResult
    {
        public bool Success { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public string? Message { get; set; }
        
        public static OperationResult CreateSuccess(string message) => new OperationResult { Success = true, Message = message };
        public static OperationResult CreateFailure(string message) => new OperationResult { Success = false, Message = message };
    }

    // ============== MATERIA VIEWMODELS ==============

    public class MateriaListVm
    {
        public IEnumerable<MateriaItemVm> Items { get; set; } = new List<MateriaItemVm>();
        public PaginacionVm Paginacion { get; set; } = new PaginacionVm();
        public string? Busqueda { get; set; }
        public string? Orden { get; set; }

        // Propiedades para compatibilidad con vistas que usan esto como item individual
        public int MateriaId { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string? Tipo { get; set; }
        public int Creditos { get; set; }
        public bool Activa { get; set; }
        public DateTime FechaCreacion { get; set; }
        public int TotalRubricas { get; set; }
        public int NumeroRubricas => TotalRubricas; // Alias para compatibilidad
        public int TotalOfertas { get; set; }
        public int NumeroOfertas => TotalOfertas; // Alias para compatibilidad
        public int TotalInstrumentos { get; set; }
        public int NumeroInstrumentos => TotalInstrumentos; // Alias para compatibilidad
    }

    public class MateriaItemVm
    {
        public int MateriaId { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string? Tipo { get; set; }
        public int Creditos { get; set; }
        public bool Activa { get; set; }
        public DateTime FechaCreacion { get; set; }
        public int TotalRubricas { get; set; }
        public int NumeroRubricas => TotalRubricas; // Alias para compatibilidad
        public int TotalOfertas { get; set; }
        public int NumeroOfertas => TotalOfertas; // Alias para compatibilidad
        public int TotalInstrumentos { get; set; }
        public int NumeroInstrumentos => TotalInstrumentos; // Alias para compatibilidad
    }

    public class MateriaEditVm
    {
        public int? MateriaId { get; set; }

        [Required(ErrorMessage = "El código es obligatorio.")]
        [StringLength(20, ErrorMessage = "El código no puede tener más de 20 caracteres.")]
        [Display(Name = "Código")]
        public string Codigo { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(120, ErrorMessage = "El nombre no puede tener más de 120 caracteres.")]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "Los créditos son obligatorios.")]
        [Range(0, 10, ErrorMessage = "Los créditos deben estar entre 0 y 10.")]
        [Display(Name = "Créditos")]
        public int Creditos { get; set; }

        [Display(Name = "Activa")]
        public bool Activa { get; set; } = true;

        [Display(Name = "Descripción")]
        [StringLength(500)]
        public string? Descripcion { get; set; }

        [StringLength(50)]
        [Display(Name = "Tipo")]
        public string? Tipo { get; set; }
    }

    public class MateriaDetalleVm
    {
        public int MateriaId { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public int Creditos { get; set; }
        public bool Activa { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string? Descripcion { get; set; }
        public List<RubricaItemVm> RubricasAsignadas { get; set; } = new List<RubricaItemVm>();
        public List<MateriaPeriodoItemVm> Ofertas { get; set; } = new List<MateriaPeriodoItemVm>();
    }

    // ============== RUBRICA VIEWMODELS ==============

    public class RubricaListVm
    {
        public IEnumerable<RubricaItemVm> Items { get; set; } = new List<RubricaItemVm>();
        public PaginacionVm Paginacion { get; set; } = new PaginacionVm();
        public string? Busqueda { get; set; }
        public string? Orden { get; set; }
    }

    public class RubricaItemVm
    {
        public int IdRubrica { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string Titulo { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public bool Vigente { get; set; }
        public DateTime FechaCreacion { get; set; }
        public int TotalMaterias { get; set; }
    }

    public class RubricaEditVm
    {
        public int? IdRubrica { get; set; }

        [Required(ErrorMessage = "El código es obligatorio.")]
        [StringLength(20, ErrorMessage = "El código no puede tener más de 20 caracteres.")]
        [Display(Name = "Código")]
        public string Codigo { get; set; } = string.Empty;

        [Required(ErrorMessage = "El título es obligatorio.")]
        [StringLength(120, ErrorMessage = "El título no puede tener más de 120 caracteres.")]
        [Display(Name = "Título")]
        public string Titulo { get; set; } = string.Empty;

        [StringLength(1000)]
        [Display(Name = "Descripción")]
        public string? Descripcion { get; set; }

        [Display(Name = "Vigente")]
        public bool Vigente { get; set; } = true;
    }

    public class RubricaDetalleVm
    {
        public int IdRubrica { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string Titulo { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public bool Vigente { get; set; }
        public DateTime FechaCreacion { get; set; }
        public List<MateriaItemVm> MateriasAsignadas { get; set; } = new List<MateriaItemVm>();
    }

    // ============== PERIODO ACADEMICO VIEWMODELS ==============

    public class PeriodoListVm
    {
        public IEnumerable<PeriodoItemVm> Items { get; set; } = new List<PeriodoItemVm>();
        public PaginacionVm Paginacion { get; set; } = new PaginacionVm();
        public string? Busqueda { get; set; }
        public string? Orden { get; set; }

        // Propiedades para compatibilidad con vistas que usan esto como item individual
        public int Id { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public int Anio { get; set; }
        public string Ciclo { get; set; } = string.Empty;
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public bool Activo { get; set; }
        public int TotalOfertas { get; set; }
        public int NumeroOfertas => TotalOfertas; // Alias para compatibilidad
    }

    public class PeriodoItemVm
    {
        public int Id { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public int Anio { get; set; }
        public string Ciclo { get; set; } = string.Empty;
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public bool Activo { get; set; }
        public int TotalOfertas { get; set; }
        public int NumeroOfertas => TotalOfertas; // Alias para compatibilidad
    }

    public class PeriodoEditVm
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = "El año es obligatorio.")]
        [Range(2020, 2100, ErrorMessage = "El año debe estar entre 2020 y 2100.")]
        [Display(Name = "Año")]
        public int Anio { get; set; }

        [Required(ErrorMessage = "El ciclo es obligatorio.")]
        [StringLength(10, ErrorMessage = "El ciclo no puede tener más de 10 caracteres.")]
        [Display(Name = "Ciclo")]
        public string Ciclo { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha de inicio es obligatoria.")]
        [Display(Name = "Fecha de Inicio")]
        public DateTime FechaInicio { get; set; }

        [Required(ErrorMessage = "La fecha de fin es obligatoria.")]
        [Display(Name = "Fecha de Fin")]
        public DateTime FechaFin { get; set; }

        [Display(Name = "Activo")]
        public bool Activo { get; set; } = false;

        // Propiedades adicionales para compatibilidad
        [StringLength(20)]
        [Display(Name = "Código")]
        public string Codigo { get; set; } = string.Empty;

        [StringLength(120)]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(20)]
        [Display(Name = "Tipo")]
        public string Tipo { get; set; } = string.Empty;

        [Display(Name = "Número de Período")]
        public int NumeroPeriodo { get; set; }
    }

    public class PeriodoDetalleVm
    {
        public int Id { get; set; }
        public int Anio { get; set; }
        public string Ciclo { get; set; } = string.Empty;
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public bool Activo { get; set; }
        public List<MateriaPeriodoItemVm> Ofertas { get; set; } = new List<MateriaPeriodoItemVm>();
    }

    // ============== ASIGNACION VIEWMODELS ==============

    public class AsignarRubricasVm
    {
        public int MateriaId { get; set; }
        public string MateriaNombre { get; set; } = string.Empty;
        public List<int> RubricaIds { get; set; } = new List<int>();
        public List<RubricaSelectVm> RubricasDisponibles { get; set; } = new List<RubricaSelectVm>();
        public List<AsignacionRubricaVm> RubricasAsignadas { get; set; } = new List<AsignacionRubricaVm>();
        
        [StringLength(500)]
        [Display(Name = "Observaciones")]
        public string? Observaciones { get; set; }
    }

    public class CrearOfertaMateriaVm
    {
        public int MateriaId { get; set; }
        public string MateriaNombre { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Debe seleccionar un período académico.")]
        [Display(Name = "Período Académico")]
        public int PeriodoId { get; set; }
        public string PeriodoNombre { get; set; } = string.Empty;

        [Range(1, 500, ErrorMessage = "El cupo debe estar entre 1 y 500.")]
        [Display(Name = "Cupo")]
        public int Cupo { get; set; } = 30;

        [StringLength(20)]
        [Display(Name = "Estado")]
        public string Estado { get; set; } = "Disponible";
    }

    public class MateriaPeriodoItemVm
    {
        public int Id { get; set; }
        public int MateriaId { get; set; }
        public string MateriaCodigo { get; set; } = string.Empty;
        public string MateriaNombre { get; set; } = string.Empty;
        public int PeriodoAcademicoId { get; set; }
        public string PeriodoNombre { get; set; } = string.Empty;
        public int Cupo { get; set; }
        public string Estado { get; set; } = string.Empty;
        public DateTime? FechaPublicacion { get; set; }
        public DateTime FechaCreacion { get; set; }
    }

    // ============== UTILITY VIEWMODELS ==============

    public class PaginacionVm
    {
        public int PaginaActual { get; set; } = 1;
        public int ItemsPorPagina { get; set; } = 10;
        public int TotalItems { get; set; }
        public int TotalPaginas => (int)Math.Ceiling((double)TotalItems / ItemsPorPagina);
        public bool TienePaginaAnterior => PaginaActual > 1;
        public bool TienePaginaSiguiente => PaginaActual < TotalPaginas;
    }

    // ============== VIEWMODELS ADICIONALES ==============
    
    // ViewModels para servicios específicos
    public class MateriaVm
    {
        public int MateriaId { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string? Tipo { get; set; }
        public string? Descripcion { get; set; }
        public int Creditos { get; set; }
        public bool Activa { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public string? Estado { get; set; }
    }

    public class CrearMateriaVm
    {
        [Required(ErrorMessage = "El código es obligatorio.")]
        [StringLength(20, ErrorMessage = "El código no puede tener más de 20 caracteres.")]
        [Display(Name = "Código")]
        public string Codigo { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(120, ErrorMessage = "El nombre no puede tener más de 120 caracteres.")]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(50)]
        [Display(Name = "Tipo")]
        public string? Tipo { get; set; }

        [StringLength(500)]
        [Display(Name = "Descripción")]
        public string? Descripcion { get; set; }

        [Required(ErrorMessage = "Los créditos son obligatorios.")]
        [Range(0, 10, ErrorMessage = "Los créditos deben estar entre 0 y 10.")]
        [Display(Name = "Créditos")]
        public int Creditos { get; set; }

        [Display(Name = "Activa")]
        public bool Activa { get; set; } = true;
    }

    public class EditarMateriaVm
    {
        [Required(ErrorMessage = "El código es obligatorio.")]
        [StringLength(20, ErrorMessage = "El código no puede tener más de 20 caracteres.")]
        [Display(Name = "Código")]
        public string Codigo { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(120, ErrorMessage = "El nombre no puede tener más de 120 caracteres.")]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(50)]
        [Display(Name = "Tipo")]
        public string? Tipo { get; set; }

        [StringLength(500)]
        [Display(Name = "Descripción")]
        public string? Descripcion { get; set; }

        [Required(ErrorMessage = "Los créditos son obligatorios.")]
        [Range(0, 10, ErrorMessage = "Los créditos deben estar entre 0 y 10.")]
        [Display(Name = "Créditos")]
        public int Creditos { get; set; }

        [Display(Name = "Activa")]
        public bool Activa { get; set; } = true;
    }

    public class AsignacionRubricaVm
    {
        public int RubricaId { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string Titulo { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public DateTime FechaAsignacion { get; set; }
        public string? Observaciones { get; set; }
    }

    public class OfertaMateriaVm
    {
        public int Id { get; set; }
        public int MateriaId { get; set; }
        public string MateriaCodigo { get; set; } = string.Empty;
        public string MateriaNombre { get; set; } = string.Empty;
        public int PeriodoAcademicoId { get; set; }
        public string PeriodoNombre { get; set; } = string.Empty;
        public int Cupo { get; set; }
        public string Estado { get; set; } = string.Empty;
        public DateTime? FechaPublicacion { get; set; }
        public DateTime FechaCreacion { get; set; }
    }

    public class PeriodoVm
    {
        public int Id { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public int Anio { get; set; }
        public string Ciclo { get; set; } = string.Empty;
        public int NumeroPeriodo { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public string? Estado { get; set; }
    }

    public class CrearPeriodoVm
    {
        [Required(ErrorMessage = "El código es obligatorio.")]
        [StringLength(20, ErrorMessage = "El código no puede tener más de 20 caracteres.")]
        [Display(Name = "Código")]
        public string Codigo { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(120, ErrorMessage = "El nombre no puede tener más de 120 caracteres.")]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El tipo es obligatorio.")]
        [StringLength(20)]
        [Display(Name = "Tipo")]
        public string Tipo { get; set; } = string.Empty;

        [Required(ErrorMessage = "El año es obligatorio.")]
        [Range(2020, 2100, ErrorMessage = "El año debe estar entre 2020 y 2100.")]
        [Display(Name = "Año")]
        public int Anio { get; set; }

        [Required(ErrorMessage = "El ciclo es obligatorio.")]
        [StringLength(10)]
        [Display(Name = "Ciclo")]
        public string Ciclo { get; set; } = string.Empty;

        [Range(1, 4, ErrorMessage = "El número de período debe estar entre 1 y 4.")]
        [Display(Name = "Número de Período")]
        public int NumeroPeriodo { get; set; }

        [Required(ErrorMessage = "La fecha de inicio es obligatoria.")]
        [Display(Name = "Fecha de Inicio")]
        public DateTime FechaInicio { get; set; }

        [Required(ErrorMessage = "La fecha de fin es obligatoria.")]
        [Display(Name = "Fecha de Fin")]
        public DateTime FechaFin { get; set; }

        [Display(Name = "Activo")]
        public bool Activo { get; set; } = true;
    }

    public class EditarPeriodoVm
    {
        public int? Id { get; set; }
        
        [Required(ErrorMessage = "El código es obligatorio.")]
        [StringLength(20, ErrorMessage = "El código no puede tener más de 20 caracteres.")]
        [Display(Name = "Código")]
        public string Codigo { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(120, ErrorMessage = "El nombre no puede tener más de 120 caracteres.")]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El tipo es obligatorio.")]
        [StringLength(20)]
        [Display(Name = "Tipo")]
        public string Tipo { get; set; } = string.Empty;

        [Required(ErrorMessage = "El año es obligatorio.")]
        [Range(2020, 2100, ErrorMessage = "El año debe estar entre 2020 y 2100.")]
        [Display(Name = "Año")]
        public int Anio { get; set; }
        
        // Alias para compatibilidad con vista
        public int Año
        {
            get => Anio;
            set => Anio = value;
        }

        [Required(ErrorMessage = "El ciclo es obligatorio.")]
        [StringLength(10)]
        [Display(Name = "Ciclo")]
        public string Ciclo { get; set; } = string.Empty;

        [Range(1, 4, ErrorMessage = "El número de período debe estar entre 1 y 4.")]
        [Display(Name = "Número de Período")]
        public int NumeroPeriodo { get; set; }

        [Required(ErrorMessage = "La fecha de inicio es obligatoria.")]
        [Display(Name = "Fecha de Inicio")]
        public DateTime FechaInicio { get; set; }

        [Required(ErrorMessage = "La fecha de fin es obligatoria.")]
        [Display(Name = "Fecha de Fin")]
        public DateTime FechaFin { get; set; }

        [Display(Name = "Activo")]
        public bool Activo { get; set; } = true;
    }

    public class RubricaSelectVm
    {
        public int Id { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string Titulo { get; set; } = string.Empty;
        public bool Seleccionada { get; set; }
    }
}