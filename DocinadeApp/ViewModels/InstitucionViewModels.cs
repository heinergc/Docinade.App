using System.ComponentModel.DataAnnotations;
using RubricasApp.Web.Models;

namespace RubricasApp.Web.ViewModels
{
    /// <summary>
    /// ViewModel para listar instituciones
    /// </summary>
    public class InstitucionListViewModel
    {
        public int Id { get; set; }
        
        public string Nombre { get; set; } = string.Empty;
        
        public string? CodigoMEP { get; set; }
        
        public string? DireccionRegional { get; set; }
        
        public string? CircuitoEscolar { get; set; }
        
        public string TipoInstitucion { get; set; } = string.Empty;
        
        public bool Estado { get; set; }
        
        public DateTime FechaCreacion { get; set; }
    }

    /// <summary>
    /// ViewModel para crear una nueva institución
    /// </summary>
    public class InstitucionCreateViewModel
    {
        [Required(ErrorMessage = "El nombre de la institución es requerido")]
        [MaxLength(200, ErrorMessage = "El nombre no puede exceder 200 caracteres")]
        [Display(Name = "Nombre de la Institución")]
        public string Nombre { get; set; } = string.Empty;

        [MaxLength(20, ErrorMessage = "Las siglas no pueden exceder 20 caracteres")]
        [Display(Name = "Siglas")]
        public string? Siglas { get; set; }

        [Required(ErrorMessage = "El tipo de institución es requerido")]
        [MaxLength(50, ErrorMessage = "El tipo no puede exceder 50 caracteres")]
        [Display(Name = "Tipo de Institución")]
        public string TipoInstitucion { get; set; } = string.Empty;

        [MaxLength(20, ErrorMessage = "El código MEP no puede exceder 20 caracteres")]
        [Display(Name = "Código MEP")]
        public string? CodigoMEP { get; set; }

        [MaxLength(200, ErrorMessage = "La dirección regional no puede exceder 200 caracteres")]
        [Display(Name = "Dirección Regional")]
        public string? DireccionRegional { get; set; }

        [MaxLength(100, ErrorMessage = "El circuito escolar no puede exceder 100 caracteres")]
        [Display(Name = "Circuito Escolar")]
        public string? CircuitoEscolar { get; set; }

        [MaxLength(20, ErrorMessage = "El teléfono no puede exceder 20 caracteres")]
        [Display(Name = "Teléfono")]
        [Phone(ErrorMessage = "El formato del teléfono no es válido")]
        public string? Telefono { get; set; }

        [MaxLength(150, ErrorMessage = "El email no puede exceder 150 caracteres")]
        [Display(Name = "Correo Electrónico")]
        [EmailAddress(ErrorMessage = "El formato del correo no es válido")]
        public string? Email { get; set; }

        [MaxLength(200, ErrorMessage = "El sitio web no puede exceder 200 caracteres")]
        [Display(Name = "Sitio Web")]
        [Url(ErrorMessage = "El formato del sitio web no es válido")]
        public string? SitioWeb { get; set; }

        [MaxLength(300, ErrorMessage = "La dirección no puede exceder 300 caracteres")]
        [Display(Name = "Dirección Física")]
        public string? Direccion { get; set; }

        [Display(Name = "Estado")]
        public bool Estado { get; set; } = true;
    }

    /// <summary>
    /// ViewModel para editar una institución existente
    /// </summary>
    public class InstitucionEditViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre de la institución es requerido")]
        [MaxLength(200, ErrorMessage = "El nombre no puede exceder 200 caracteres")]
        [Display(Name = "Nombre de la Institución")]
        public string Nombre { get; set; } = string.Empty;

        [MaxLength(20, ErrorMessage = "Las siglas no pueden exceder 20 caracteres")]
        [Display(Name = "Siglas")]
        public string? Siglas { get; set; }

        [Required(ErrorMessage = "El tipo de institución es requerido")]
        [MaxLength(50, ErrorMessage = "El tipo no puede exceder 50 caracteres")]
        [Display(Name = "Tipo de Institución")]
        public string TipoInstitucion { get; set; } = string.Empty;

        [MaxLength(20, ErrorMessage = "El código MEP no puede exceder 20 caracteres")]
        [Display(Name = "Código MEP")]
        public string? CodigoMEP { get; set; }

        [MaxLength(200, ErrorMessage = "La dirección regional no puede exceder 200 caracteres")]
        [Display(Name = "Dirección Regional")]
        public string? DireccionRegional { get; set; }

        [MaxLength(100, ErrorMessage = "El circuito escolar no puede exceder 100 caracteres")]
        [Display(Name = "Circuito Escolar")]
        public string? CircuitoEscolar { get; set; }

        [MaxLength(20, ErrorMessage = "El teléfono no puede exceder 20 caracteres")]
        [Display(Name = "Teléfono")]
        [Phone(ErrorMessage = "El formato del teléfono no es válido")]
        public string? Telefono { get; set; }

        [MaxLength(150, ErrorMessage = "El email no puede exceder 150 caracteres")]
        [Display(Name = "Correo Electrónico")]
        [EmailAddress(ErrorMessage = "El formato del correo no es válido")]
        public string? Email { get; set; }

        [MaxLength(200, ErrorMessage = "El sitio web no puede exceder 200 caracteres")]
        [Display(Name = "Sitio Web")]
        [Url(ErrorMessage = "El formato del sitio web no es válido")]
        public string? SitioWeb { get; set; }

        [MaxLength(300, ErrorMessage = "La dirección no puede exceder 300 caracteres")]
        [Display(Name = "Dirección Física")]
        public string? Direccion { get; set; }

        [Display(Name = "Estado")]
        public bool Estado { get; set; }

        [Display(Name = "Fecha de Creación")]
        public DateTime FechaCreacion { get; set; }
    }

    /// <summary>
    /// ViewModel para ver detalles de una institución
    /// </summary>
    public class InstitucionDetailsViewModel
    {
        public int Id { get; set; }
        
        public string Nombre { get; set; } = string.Empty;
        
        public string? Siglas { get; set; }
        
        public string TipoInstitucion { get; set; } = string.Empty;
        
        public string? CodigoMEP { get; set; }
        
        public string? DireccionRegional { get; set; }
        
        public string? CircuitoEscolar { get; set; }
        
        public string? Telefono { get; set; }
        
        public string? Email { get; set; }
        
        public string? SitioWeb { get; set; }
        
        public string? Direccion { get; set; }
        
        public bool Estado { get; set; }
        
        public DateTime FechaCreacion { get; set; }
        
        public int CantidadFacultades { get; set; }
        
        public List<FacultadSimpleViewModel> Facultades { get; set; } = new();
    }

    /// <summary>
    /// ViewModel simple para facultades
    /// </summary>
    public class FacultadSimpleViewModel
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Codigo { get; set; }
        public bool Estado { get; set; }
    }
}
