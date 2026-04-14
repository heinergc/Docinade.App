using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DocinadeApp.Models.Identity;

namespace DocinadeApp.Models
{
    public class GrupoEstudiante
    {
        public int GrupoId { get; set; }
        
        [Required(ErrorMessage = "El código es requerido")]
        [StringLength(20, ErrorMessage = "El código no puede exceder 20 caracteres")]
        [Display(Name = "Código")]
        public string Codigo { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; } = string.Empty;
        
        [StringLength(500, ErrorMessage = "La descripción no puede exceder 500 caracteres")]
        [Display(Name = "Descripción")]
        public string? Descripcion { get; set; }
        
        // ?? NUEVA IMPLEMENTACIÓN: Usar catálogo de tipos en lugar de enum
        [Required(ErrorMessage = "El tipo de grupo es requerido")]
        [Display(Name = "Tipo de Grupo")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un tipo de grupo v�lido")]
        public int IdTipoGrupo { get; set; }
        
        // ?? COMPATIBILIDAD: Mantener enum para compatibilidad con código existente
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

        [Range(1, 200, ErrorMessage = "La capacidad máxima debe estar entre 1 y 200")]
        [Display(Name = "Capacidad Máxima")]
        public int? CapacidadMaxima { get; set; }
        
        [Required(ErrorMessage = "El período académico es requerido")]
        [Display(Name = "Período Académico")]
        public int PeriodoAcademicoId { get; set; }
        
        [Display(Name = "Institución")]
        public int? InstitucionId { get; set; }
        
        [Required]
        [StringLength(20)]
        [Display(Name = "Estado")]
        public EstadoGrupo Estado { get; set; } = EstadoGrupo.Activo;
        
        [Display(Name = "Fecha de Creación")]
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        
        [Display(Name = "Fecha de Modificación")]
        public DateTime? FechaModificacion { get; set; }
        
        [StringLength(450)]
        [Display(Name = "Creado Por")]
        public string? CreadoPorId { get; set; }
        
        [StringLength(1000, ErrorMessage = "Las observaciones no pueden exceder 1000 caracteres")]
        [Display(Name = "Observaciones")]
        public string? Observaciones { get; set; }
        
        // ?? NUEVA: Navigation property para el cat�logo de tipos
        [ForeignKey("IdTipoGrupo")]
        public virtual TipoGrupoCatalogo? TipoGrupoCatalogo { get; set; }
        
        // Navigation properties existentes
        [ForeignKey("PeriodoAcademicoId")]
        public virtual PeriodoAcademico? PeriodoAcademico { get; set; }
                [ForeignKey("InstitucionId")]
        public virtual Institucion? Institucion { get; set; }
                [ForeignKey("CreadoPorId")]
        public virtual ApplicationUser? CreadoPor { get; set; }
        
        public virtual ICollection<EstudianteGrupo> EstudianteGrupos { get; set; } = new List<EstudianteGrupo>();
        public virtual ICollection<GrupoMateria> GrupoMaterias { get; set; } = new List<GrupoMateria>();
        
        // Propiedades calculadas
        [NotMapped]
        [Display(Name = "Cantidad de Estudiantes")]
        public int CantidadEstudiantes => EstudianteGrupos?.Count(eg => eg.Estado == EstadoAsignacion.Activo) ?? 0;
        
        [NotMapped]
        [Display(Name = "Estado Completo")]
        public bool EstaCompleto => CapacidadMaxima.HasValue && CantidadEstudiantes >= CapacidadMaxima.Value;
        
        [NotMapped]
        [Display(Name = "Espacios Disponibles")]
        public int EspaciosDisponibles => CapacidadMaxima.HasValue ? Math.Max(0, CapacidadMaxima.Value - CantidadEstudiantes) : int.MaxValue;
        
        [NotMapped]
        [Display(Name = "Estado de Capacidad")]
        public string EstadoCapacidad
        {
            get
            {
                if (!CapacidadMaxima.HasValue) return "Sin l�mite";
                if (EstaCompleto) return "Completo";
                return $"{EspaciosDisponibles} disponibles";
            }
        }

        // ?? NUEVA: Propiedad para mostrar el nombre del tipo desde el cat�logo
        [NotMapped]
        [Display(Name = "Tipo de Grupo")]
        public string TipoGrupoDisplay => TipoGrupoCatalogo?.Nombre ?? TipoGrupo.ToString();
    }
    
    // ?? MANTENER: Enum para compatibilidad con c�digo existente
    public enum TipoGrupo
    {
        [Display(Name = "Sección")]
        Seccion = 1,
        
        [Display(Name = "Nivel")]
        Nivel = 2,
        
        [Display(Name = "Modalidad")]
        Modalidad = 3,
        
        [Display(Name = "Personalizado")]
        Custom = 4
    }
    
    public enum EstadoGrupo
    {
        [Display(Name = "Activo")]
        Activo = 1,
        
        [Display(Name = "Inactivo")]
        Inactivo = 2,
        
        [Display(Name = "Cerrado")]
        Cerrado = 3,
        
        [Display(Name = "Suspendido")]
        Suspendido = 4
    }
}