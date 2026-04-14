using System.ComponentModel.DataAnnotations;
using DocinadeApp.Models;

namespace DocinadeApp.ViewModels.Grupos
{
    // ViewModel para la lista principal
    public class TipoGrupoListViewModel
    {
        public int IdTipoGrupo { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public DateTime FechaRegistro { get; set; }
        public string Estado { get; set; } = string.Empty;
        public bool EsActivo { get; set; }
        public int CantidadGrupos { get; set; }
        public bool PuedeEliminar { get; set; }
    }

    // ViewModel para los detalles
    public class TipoGrupoDetailsViewModel
    {
        public int IdTipoGrupo { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public DateTime FechaRegistro { get; set; }
        public string Estado { get; set; } = string.Empty;
        public bool EsActivo { get; set; }
        public List<GrupoAsociadoViewModel> GruposAsociados { get; set; } = new List<GrupoAsociadoViewModel>();
    }

    // ViewModel para crear
    public class CreateTipoGrupoViewModel
    {
        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El estado es requerido")]
        [Display(Name = "Estado")]
        public string Estado { get; set; } = "Activo";
    }

    // ViewModel para editar
    public class EditTipoGrupoViewModel : CreateTipoGrupoViewModel
    {
        public int IdTipoGrupo { get; set; }
        public DateTime FechaRegistro { get; set; }
    }

    // ViewModel para eliminar
    public class DeleteTipoGrupoViewModel
    {
        public int IdTipoGrupo { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public DateTime FechaRegistro { get; set; }
        public string Estado { get; set; } = string.Empty;
        public int CantidadGrupos { get; set; }
        public bool PuedeEliminar { get; set; }
        public List<string> GruposAsociados { get; set; } = new List<string>();
    }

    // ViewModel para grupos asociados
    public class GrupoAsociadoViewModel
    {
        public int GrupoId { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string PeriodoAcademico { get; set; } = string.Empty;
        public EstadoGrupo Estado { get; set; }
        public string EstadoDisplay { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; }
    }
}