using System.ComponentModel.DataAnnotations;

namespace RubricasApp.Web.ViewModels
{
    /// <summary>
    /// ViewModel para el flujo de auto-empadronamiento público multi-step
    /// Combina datos de Estudiante y EstudianteEmpadronamiento
    /// </summary>
    public class EmpadronamientoPublicoViewModel
    {
        // ======== PASO 1: DATOS BÁSICOS DEL ESTUDIANTE ========
        
        [Required(ErrorMessage = "El número de cédula es requerido")]
        [StringLength(20, ErrorMessage = "El número de cédula no puede exceder 20 caracteres")]
        [Display(Name = "Número de Cédula")]
        public string NumeroId { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        [Display(Name = "Nombre(s)")]
        public string Nombre { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Los apellidos son requeridos")]
        [StringLength(100, ErrorMessage = "Los apellidos no pueden exceder 100 caracteres")]
        [Display(Name = "Apellido(s)")]
        public string Apellidos { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "La fecha de nacimiento es requerida")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Nacimiento")]
        public DateTime? FechaNacimiento { get; set; }
        
        [Required(ErrorMessage = "El género es requerido")]
        [StringLength(20)]
        [Display(Name = "Género")]
        public string Genero { get; set; } = string.Empty;
        
        [StringLength(50)]
        [Display(Name = "Nacionalidad")]
        public string? Nacionalidad { get; set; }
        
        [StringLength(30)]
        [Display(Name = "Estado Civil")]
        public string? EstadoCivil { get; set; }
        
        // ======== PASO 2: CONTACTO Y RESIDENCIA ========
        
        [Required(ErrorMessage = "El correo electrónico es requerido")]
        [EmailAddress(ErrorMessage = "Formato de correo electrónico inválido")]
        [StringLength(100)]
        [Display(Name = "Correo Electrónico Principal")]
        public string DireccionCorreo { get; set; } = string.Empty;
        
        [EmailAddress(ErrorMessage = "Formato de correo electrónico inválido")]
        [StringLength(100)]
        [Display(Name = "Correo Electrónico Alterno")]
        public string? CorreoAlterno { get; set; }
        
        [Required(ErrorMessage = "El teléfono es requerido")]
        [Phone(ErrorMessage = "Formato de teléfono inválido")]
        [StringLength(20)]
        [Display(Name = "Teléfono Principal")]
        public string TelefonoAlterno { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "La provincia es requerida")]
        [StringLength(50)]
        [Display(Name = "Provincia")]
        public string Provincia { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "El cantón es requerido")]
        [StringLength(50)]
        [Display(Name = "Cantón")]
        public string Canton { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "El distrito es requerido")]
        [StringLength(50)]
        [Display(Name = "Distrito")]
        public string Distrito { get; set; } = string.Empty;
        
        [StringLength(100)]
        [Display(Name = "Barrio")]
        public string? Barrio { get; set; }
        
        [Required(ErrorMessage = "Las señas exactas son requeridas")]
        [StringLength(500)]
        [Display(Name = "Señas Exactas de Domicilio")]
        [DataType(DataType.MultilineText)]
        public string Senas { get; set; } = string.Empty;
        
        // ======== PASO 3: RESPONSABLES Y CONTACTO DE EMERGENCIA ========
        
        [StringLength(100)]
        [Display(Name = "Nombre Completo del Padre")]
        public string? NombrePadre { get; set; }
        
        [StringLength(100)]
        [Display(Name = "Nombre Completo de la Madre")]
        public string? NombreMadre { get; set; }
        
        [StringLength(100)]
        [Display(Name = "Nombre Completo del Tutor Legal")]
        public string? NombreTutor { get; set; }
        
        [Required(ErrorMessage = "El contacto de emergencia es requerido")]
        [StringLength(100)]
        [Display(Name = "Nombre de Contacto de Emergencia")]
        public string ContactoEmergencia { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "El teléfono de emergencia es requerido")]
        [Phone(ErrorMessage = "Formato de teléfono inválido")]
        [StringLength(20)]
        [Display(Name = "Teléfono de Emergencia")]
        public string TelefonoEmergencia { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "La relación con el contacto de emergencia es requerida")]
        [StringLength(50)]
        [Display(Name = "Relación (Padre, Madre, Hermano/a, etc.)")]
        public string RelacionEmergencia { get; set; } = string.Empty;
        
        // ======== PASO 4: INFORMACIÓN ACADÉMICA ========
        
        [StringLength(100)]
        [Display(Name = "Institución de Procedencia")]
        public string? InstitucionProcedencia { get; set; }
        
        [StringLength(50)]
        [Display(Name = "Último Nivel Cursado")]
        public string? UltimoNivelCursado { get; set; }
        
        [Range(0, 100, ErrorMessage = "El promedio debe estar entre 0 y 100")]
        [Display(Name = "Promedio del Último Año")]
        public decimal? PromedioAnterior { get; set; }
        
        [StringLength(500)]
        [Display(Name = "Adaptaciones o Apoyos Educativos Previos")]
        [DataType(DataType.MultilineText)]
        public string? AdaptacionesPrevias { get; set; }
        
        // ======== PASO 5: INFORMACIÓN DE SALUD ========
        
        [StringLength(500)]
        [Display(Name = "Alergias Conocidas")]
        [DataType(DataType.MultilineText)]
        public string? Alergias { get; set; }
        
        [StringLength(500)]
        [Display(Name = "Condiciones Médicas")]
        [DataType(DataType.MultilineText)]
        public string? CondicionesMedicas { get; set; }
        
        [StringLength(500)]
        [Display(Name = "Medicamentos que Toma Regularmente")]
        [DataType(DataType.MultilineText)]
        public string? Medicamentos { get; set; }
        
        [StringLength(100)]
        [Display(Name = "Compañía de Seguro Médico")]
        public string? SeguroMedico { get; set; }
        
        [StringLength(100)]
        [Display(Name = "Centro Médico o Clínica Habitual")]
        public string? CentroMedicoHabitual { get; set; }
        
        // ======== CAMPOS DE CONTROL ========
        
        [Display(Name = "Paso Actual")]
        public int PasoActual { get; set; } = 1;
        
        [Display(Name = "Total de Pasos")]
        public int TotalPasos { get; set; } = 5;
        
        [Display(Name = "Acepto términos y condiciones")]
        [Range(typeof(bool), "true", "true", ErrorMessage = "Debe aceptar los términos y condiciones")]
        public bool AceptaTerminos { get; set; }
        
        // Campos para el estudiante principal
        [StringLength(100)]
        [Display(Name = "Institución")]
        public string? Institucion { get; set; }
        
        [Display(Name = "Año")]
        public int Anio { get; set; } = DateTime.Now.Year;
        
        [Display(Name = "Período Académico")]
        public int? PeriodoAcademicoId { get; set; }
        
        // Para tracking de sesión
        public string? SessionId { get; set; }
        
        public DateTime FechaInicio { get; set; } = DateTime.Now;
    }
}
