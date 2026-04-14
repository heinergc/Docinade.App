using System.ComponentModel.DataAnnotations;

namespace RubricasApp.Web.ViewModels.Conducta
{
    /// <summary>
    /// ViewModel para mostrar la nota de conducta de un estudiante
    /// </summary>
    public class NotaConductaViewModel
    {
        public int IdNotaConducta { get; set; }
        public int IdEstudiante { get; set; }
        public string NombreEstudiante { get; set; } = string.Empty;
        public string NumeroId { get; set; } = string.Empty;
        public string Grupo { get; set; } = string.Empty;
        
        public int IdPeriodo { get; set; }
        public string NombrePeriodo { get; set; } = string.Empty;
        
        public decimal NotaInicial { get; set; }
        public decimal TotalRebajos { get; set; }
        public decimal NotaFinal { get; set; }
        
        public string Estado { get; set; } = string.Empty; // Aprobado, Riesgo, Aplazado
        public string EstadoColor { get; set; } = "success"; // success, warning, danger
        
        public bool RequiereProgramaAcciones { get; set; }
        public bool DecisionProfesionalAplicada { get; set; }
        
        public decimal NotaMinimaAprobacion { get; set; }
        public decimal NotaRiesgoInicio { get; set; }
        
        public int CantidadBoletas { get; set; }
        public List<BoletaConductaListViewModel> Boletas { get; set; } = new();
        
        public ProgramaAccionesViewModel? ProgramaAcciones { get; set; }
        public DecisionProfesionalViewModel? DecisionProfesional { get; set; }
        
        public DateTime FechaCalculo { get; set; }
        public DateTime? FechaUltimaActualizacion { get; set; }
    }

    /// <summary>
    /// ViewModel para estudiantes en riesgo o aplazados
    /// </summary>
    public class EstudiantesRiesgoViewModel
    {
        public string Periodo { get; set; } = string.Empty;
        public List<EstudianteRiesgoItem> Estudiantes { get; set; } = new();
    }

    public class EstudianteRiesgoItem
    {
        public int IdEstudiante { get; set; }
        public string NombreCompleto { get; set; } = string.Empty;
        public string NumeroId { get; set; } = string.Empty;
        public string Grupo { get; set; } = string.Empty;
        public string ProfesorGuia { get; set; } = string.Empty;
        
        public decimal NotaFinal { get; set; }
        public string Estado { get; set; } = string.Empty;
        public int CantidadFaltas { get; set; }
        public int CantidadBoletas { get; set; }
        public int TotalRebajos { get; set; }
        public DateTime? FechaUltimaBoleta { get; set; }
        
        public bool TieneProgramaAcciones { get; set; }
        public string? EstadoPrograma { get; set; }
        
        public bool DecisionProfesionalAplicada { get; set; }
    }

    /// <summary>
    /// ViewModel para el cálculo/recálculo de la nota
    /// </summary>
    public class CalcularNotaConductaViewModel
    {
        public int IdEstudiante { get; set; }
        public int IdPeriodo { get; set; }
        
        public decimal NotaMinimaAprobacion { get; set; }
        
        // Resultado del cálculo
        public decimal NotaInicial { get; set; } = 100;
        public decimal TotalRebajos { get; set; }
        public decimal NotaFinal { get; set; }
        public string Estado { get; set; } = string.Empty;
        public bool RequiereProgramaAcciones { get; set; }
    }

    /// <summary>
    /// ViewModel para mostrar la nota individual del estudiante con boletas
    /// </summary>
    public class EstudianteNotaConductaViewModel
    {
        public int IdEstudiante { get; set; }
        public int IdPeriodo { get; set; }
        public string NombreEstudiante { get; set; } = string.Empty;
        public string NumeroIdentificacion { get; set; } = string.Empty;
        public string NombreGrupo { get; set; } = string.Empty;
        public string NombrePeriodo { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty; // "Aprobado", "En Riesgo", "Aplazado"
        public decimal NotaActual { get; set; }
        public decimal NotaFinal { get; set; } // Alias
        public decimal NotaMinima { get; set; } // Nota mínima de aprobación (70.0)
        public int TotalBoletas { get; set; }
        public int CantidadBoletas { get; set; } // Alias
        public int TotalRebajos { get; set; }
        public int BoletasAnuladas { get; set; }
        public int RebajosAnulados { get; set; }
        public int NotificacionesEnviadas { get; set; }
        public DateTime? FechaUltimaBoleta { get; set; }
        public string NombreProfesorGuia { get; set; } = string.Empty;
        public bool TieneProgramaAcciones { get; set; }
        public int? IdProgramaAcciones { get; set; }
        public bool TieneDecisionProfesional { get; set; }
        public int? IdDecisionProfesional { get; set; }
        public List<BoletaConductaListViewModel> Boletas { get; set; } = new();
    }

    /// <summary>
    /// ViewModel para estudiantes aplazados
    /// </summary>
    public class EstudianteAplazadoViewModel
    {
        public int IdEstudiante { get; set; }
        public string NombreCompleto { get; set; } = string.Empty;
        public string NumeroIdentificacion { get; set; } = string.Empty;
        public string NombreGrupo { get; set; } = string.Empty;
        public string NombreProfesorGuia { get; set; } = string.Empty;
        public decimal NotaFinal { get; set; }
        public int TotalRebajos { get; set; } // Nueva propiedad
        public int BoletasEmitidas { get; set; } // Alias
        public int CantidadBoletas { get; set; }
        public DateTime? FechaUltimaBoleta { get; set; }
        public DateTime? FechaUltimaFalta { get; set; } // Alias
        public bool TieneProgramaAcciones { get; set; }
        public bool TienePrograma { get; set; } // Alias
        public string? EstadoProgramaAcciones { get; set; }
        public string? EstadoPrograma { get; set; } // Alias
        public bool DecisionTomada { get; set; }
        public bool TieneDecision { get; set; } // Alias
        public string? DecisionAplicada { get; set; } // Nueva propiedad
        public DateTime? FechaDecision { get; set; } // Nueva propiedad
    }

    /// <summary>
    /// ViewModel para estudiantes en riesgo
    /// </summary>
    public class EstudianteRiesgoViewModel
    {
        public int IdEstudiante { get; set; }
        public string NombreCompleto { get; set; } = string.Empty;
        public string NumeroIdentificacion { get; set; } = string.Empty;
        public string NombreGrupo { get; set; } = string.Empty;
        public string NombreProfesorGuia { get; set; } = string.Empty;
        public decimal NotaFinal { get; set; }
        public decimal NotaActual { get; set; } // Alias
        public int TotalRebajos { get; set; } // Nueva propiedad
        public int BoletasEmitidas { get; set; } // Alias
        public int CantidadBoletas { get; set; }
        public DateTime? FechaUltimaBoleta { get; set; }
        public string? UltimoIncidente { get; set; } // Nueva propiedad
        public DateTime? FechaUltimoIncidente { get; set; } // Nueva propiedad
        public bool TienePrograma { get; set; } // Nueva propiedad
    }

    /// <summary>
    /// ViewModel para aplicar decisión profesional
    /// </summary>
    public class AplicarDecisionProfesionalViewModel
    {
        public int IdEstudiante { get; set; }
        public string NombreEstudiante { get; set; } = string.Empty;
        public string NumeroIdentificacion { get; set; } = string.Empty;
        public string NombreGrupo { get; set; } = string.Empty;
        public string NombrePeriodo { get; set; } = string.Empty;
        public decimal NotaActual { get; set; }
        public int TotalBoletas { get; set; }
        public int TotalRebajos { get; set; }
        public string NombreProfesorGuia { get; set; } = string.Empty;
        public int IdPeriodo { get; set; }
        
        // Campos del formulario
        public string NumeroActa { get; set; } = string.Empty;
        public DateTime FechaReunion { get; set; }
        public string MiembrosComite { get; set; } = string.Empty;
        public string JustificacionPedagogica { get; set; } = string.Empty;
        public string CircunstanciasEspeciales { get; set; } = string.Empty;
        public string AccionesRealizadas { get; set; } = string.Empty;
        public string CompromisosEstudiante { get; set; } = string.Empty;
        public string AcuerdosComite { get; set; } = string.Empty;
        public decimal NotaAjustada { get; set; }
        public string? ConsideracionesAdicionales { get; set; }
        public string? ObservacionesAdicionales { get; set; } // Alias
        public string Decision { get; set; } = string.Empty; // "Aprobar Conducta", "Mantener Aplazado", etc.
    }

    /// <summary>
    /// ViewModel para crear programa de acciones
    /// </summary>
    public class CrearProgramaAccionesViewModel
    {
        public int IdEstudiante { get; set; }
        public string NombreEstudiante { get; set; } = string.Empty;
        public string NumeroIdentificacion { get; set; } = string.Empty;
        public string NombreGrupo { get; set; } = string.Empty;
        public string NombrePeriodo { get; set; } = string.Empty;
        public decimal NotaActual { get; set; }
        public decimal NotaConducta { get; set; } // Alias
        public int IdPeriodo { get; set; }
        public int IdNotaConducta { get; set; }
        
        // Campos del programa
        public string TituloPrograma { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string ObjetivosEspecificos { get; set; } = string.Empty;
        public string Objetivos { get; set; } = string.Empty; // Alias
        public string ActividadesARealizar { get; set; } = string.Empty;
        public string ActividadesPropuestas { get; set; } = string.Empty; // Alias
        public string? CompromisosEstudiante { get; set; }
        public string? CompromisosFamilia { get; set; }
        public string? CriteriosEvaluacion { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFinPrevista { get; set; }
        public string? ResponsableSupervisionId { get; set; }
        public string? IdSupervisor { get; set; } // Alias
        public string? MetasEsperadas { get; set; }
        public string? RecursosNecesarios { get; set; }
    }

    /// <summary>
    /// ViewModel simplificado para crear programa (usado en CRUD)
    /// </summary>
    public class CrearProgramaViewModel
    {
        public int IdEstudiante { get; set; }
        public int IdPeriodo { get; set; }
        
        [Required(ErrorMessage = "El título es requerido")]
        [StringLength(200, ErrorMessage = "El título no puede exceder 200 caracteres")]
        public string TituloPrograma { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "La descripción es requerida")]
        [StringLength(2000, ErrorMessage = "La descripción no puede exceder 2000 caracteres")]
        public string Descripcion { get; set; } = string.Empty;
        
        [StringLength(2000)]
        public string? ObjetivosEspecificos { get; set; }
        
        [StringLength(2000)]
        public string? ActividadesARealizar { get; set; }
        
        [StringLength(2000)]
        public string? CompromisosEstudiante { get; set; }
        
        [StringLength(2000)]
        public string? CompromisosFamilia { get; set; }
        
        [StringLength(2000)]
        public string? CriteriosEvaluacion { get; set; }
        
        [Required(ErrorMessage = "La fecha de inicio es requerida")]
        public DateTime FechaInicio { get; set; }
        
        [Required(ErrorMessage = "La fecha de fin prevista es requerida")]
        public DateTime FechaFinPrevista { get; set; }
        
        [Required(ErrorMessage = "El responsable de supervisión es requerido")]
        public string ResponsableSupervisionId { get; set; } = string.Empty;
        
        public string? Estado { get; set; }
    }

    /// <summary>
    /// ViewModel para editar programa
    /// </summary>
    public class EditarProgramaViewModel
    {
        public int IdPrograma { get; set; }
        public int IdEstudiante { get; set; }
        public int IdPeriodo { get; set; }
        public string NombreEstudiante { get; set; } = string.Empty;
        public string NombrePeriodo { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "El título es requerido")]
        [StringLength(200)]
        public string TituloPrograma { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "La descripción es requerida")]
        [StringLength(2000)]
        public string Descripcion { get; set; } = string.Empty;
        
        [StringLength(2000)]
        public string? ObjetivosEspecificos { get; set; }
        
        [StringLength(2000)]
        public string? ActividadesARealizar { get; set; }
        
        [StringLength(2000)]
        public string? CompromisosEstudiante { get; set; }
        
        [StringLength(2000)]
        public string? CompromisosFamilia { get; set; }
        
        [StringLength(2000)]
        public string? CriteriosEvaluacion { get; set; }
        
        [Required]
        public DateTime FechaInicio { get; set; }
        
        [Required]
        public DateTime FechaFinPrevista { get; set; }
        
        public DateTime? FechaFinReal { get; set; }
        
        [Required]
        public string ResponsableSupervisionId { get; set; } = string.Empty;
        
        [StringLength(2000)]
        public string? ObservacionesSupervision { get; set; }
        
        [Required]
        public string Estado { get; set; } = "Pendiente";
        
        public string? ResultadoFinal { get; set; }
    }

    /// <summary>
    /// ViewModel para eliminar programa
    /// </summary>
    public class EliminarProgramaViewModel
    {
        public int IdPrograma { get; set; }
        public string TituloPrograma { get; set; } = string.Empty;
        public string NombreEstudiante { get; set; } = string.Empty;
        public string NombrePeriodo { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; }
        public string NombreSupervisor { get; set; } = string.Empty;
    }
}

