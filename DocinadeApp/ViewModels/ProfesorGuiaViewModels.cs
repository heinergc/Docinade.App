using System.ComponentModel.DataAnnotations;

namespace RubricasApp.Web.ViewModels
{
    /// <summary>
    /// ViewModel para listar profesores guía
    /// </summary>
    public class ProfesorGuiaViewModel
    {
        public int Id { get; set; }
        
        public int ProfesorId { get; set; }
        
        [Display(Name = "Profesor")]
        public string NombreProfesor { get; set; } = string.Empty;
        
        [Display(Name = "Email")]
        public string? EmailProfesor { get; set; }
        
        public int GrupoId { get; set; }
        
        [Display(Name = "Grupo")]
        public string NombreGrupo { get; set; } = string.Empty;
        
        [Display(Name = "Código")]
        public string? CodigoGrupo { get; set; }
        
        [Display(Name = "Período Académico")]
        public string PeriodoAcademico { get; set; } = string.Empty;
        
        [Display(Name = "Fecha de Asignación")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = false)]
        public DateTime FechaAsignacion { get; set; }
        
        [Display(Name = "Fecha de Inicio")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = false)]
        public DateTime? FechaInicio { get; set; }
        
        [Display(Name = "Fecha de Fin")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = false)]
        public DateTime? FechaFin { get; set; }
        
        [Display(Name = "Estado")]
        public bool Estado { get; set; }
        
        [Display(Name = "Observaciones")]
        public string? Observaciones { get; set; }
        
        [Display(Name = "Estado")]
        public string EstadoTexto => Estado ? "Activo" : "Inactivo";
    }
    
    /// <summary>
    /// ViewModel para crear un nuevo profesor guía
    /// </summary>
    public class ProfesorGuiaCreateViewModel
    {
        [Required(ErrorMessage = "Debe seleccionar un profesor")]
        [Display(Name = "Profesor")]
        public int ProfesorId { get; set; }
        
        [Required(ErrorMessage = "Debe seleccionar un grupo")]
        [Display(Name = "Grupo")]
        public int GrupoId { get; set; }
        
        [Display(Name = "Fecha de Inicio")]
        [DataType(DataType.Date)]
        public DateTime? FechaInicio { get; set; }
        
        [Display(Name = "Fecha de Fin")]
        [DataType(DataType.Date)]
        public DateTime? FechaFin { get; set; }
        
        [Display(Name = "Observaciones")]
        [MaxLength(500, ErrorMessage = "Las observaciones no pueden exceder los 500 caracteres")]
        [DataType(DataType.MultilineText)]
        public string? Observaciones { get; set; }
    }
    
    /// <summary>
    /// ViewModel para editar un profesor guía
    /// </summary>
    public class ProfesorGuiaEditViewModel
    {
        public int Id { get; set; }
        
        [Display(Name = "Profesor")]
        public int ProfesorId { get; set; }
        
        [Display(Name = "Profesor")]
        public string NombreProfesor { get; set; } = string.Empty;
        
        [Display(Name = "Grupo")]
        public int GrupoId { get; set; }
        
        [Display(Name = "Grupo")]
        public string NombreGrupo { get; set; } = string.Empty;
        
        [Display(Name = "Fecha de Asignación")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime FechaAsignacion { get; set; }
        
        [Display(Name = "Fecha de Inicio")]
        [DataType(DataType.Date)]
        public DateTime? FechaInicio { get; set; }
        
        [Display(Name = "Fecha de Fin")]
        [DataType(DataType.Date)]
        public DateTime? FechaFin { get; set; }
        
        [Display(Name = "Estado Activo")]
        public bool Estado { get; set; }
        
        [Display(Name = "Observaciones")]
        [MaxLength(500, ErrorMessage = "Las observaciones no pueden exceder los 500 caracteres")]
        [DataType(DataType.MultilineText)]
        public string? Observaciones { get; set; }
    }
}
