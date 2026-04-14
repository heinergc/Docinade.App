using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DocinadeApp.Models
{
    public class Estudiante
    {
        public int IdEstudiante { get; set; }
        
        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "El apellido es requerido")]
        [StringLength(100, ErrorMessage = "El apellido no puede exceder 100 caracteres")]
        [Display(Name = "Apellido(s)")]
        public string Apellidos { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "El número de ID es requerido")]
        [StringLength(20, ErrorMessage = "El número de ID no puede exceder 20 caracteres")]
        [Display(Name = "Número de ID")]
        public string NumeroId { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "La dirección de correo es requerida")]
        [EmailAddress(ErrorMessage = "La dirección de correo no es válida")]
        [StringLength(100, ErrorMessage = "La dirección de correo no puede exceder 100 caracteres")]
        [Display(Name = "Dirección de correo")]
        public string DireccionCorreo { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "La institución es requerida")]
        [StringLength(100, ErrorMessage = "La institución no puede exceder 100 caracteres")]
        [Display(Name = "Institución")]
        public string Institucion { get; set; } = string.Empty;
        
        [StringLength(50, ErrorMessage = "Los grupos no pueden exceder 50 caracteres")]
        [Display(Name = "Grupos")]
        public string? Grupos { get; set; }
        
        [Required(ErrorMessage = "El año es requerido")]
        [Range(2020, 2035, ErrorMessage = "El año debe estar entre 2020 y 2035")]
        [Display(Name = "Año")]
        public int Anio { get; set; }
        
        [Required(ErrorMessage = "El período académico es requerido")]
        [Display(Name = "Período Académico")]
        public int PeriodoAcademicoId { get; set; }
        
        [Display(Name = "Estado")]
        public int Estado { get; set; } = 1; // 1 = Activo, 0 = Inactivo
        
        // Campos para Adecuación Curricular Significativa (ACS)
        [Required]
        [StringLength(20)]
        [Display(Name = "Tipo de Adecuación")]
        public string TipoAdecuacion { get; set; } = "NoPresenta"; 
        // Valores: "NoPresenta", "Acceso", "NoSignificativa", "Significativa"
        
        [Display(Name = "Fecha Inicio ACS")]
        public DateTime? FechaInicioACS { get; set; }
        
        [Display(Name = "Período Inicio ACS")]
        public int? PeriodoInicioACSId { get; set; }
        
        [Display(Name = "Aplicar ACS a Períodos Anteriores")]
        public bool AplicarACSPeriodosAnteriores { get; set; } = false;
        
        [Display(Name = "Detalles ACS")]
        [StringLength(500)]
        public string? DetallesACS { get; set; }
        
        [Display(Name = "Nombre Completo")]
        [NotMapped]
        public string NombreCompleto => $"{Nombre} {Apellidos}";
        
        [Display(Name = "Estado Texto")]
        [NotMapped]
        public string EstadoTexto => Estado == 1 ? "Activo" : "Inactivo";
        
        [Display(Name = "Está Activo")]
        [NotMapped]
        public bool EstaActivo
        {
            get => Estado == 1;
            set => Estado = value ? 1 : 0;
        }
        
        [Display(Name = "Requiere ACS")]
        [NotMapped]
        public bool RequiereACS => TipoAdecuacion == "Significativa";
        
        // Navigation properties
        [ForeignKey("PeriodoAcademicoId")]
        public virtual PeriodoAcademico? PeriodoAcademico { get; set; }
        
        [ForeignKey("PeriodoInicioACSId")]
        public virtual PeriodoAcademico? PeriodoInicioACS { get; set; }
        
        public virtual ICollection<Evaluacion> Evaluaciones { get; set; } = new List<Evaluacion>();
        
        public virtual ICollection<EstudianteInstrumentoACS> ConfiguracionesACS { get; set; } = new List<EstudianteInstrumentoACS>();
    }
}