using System.ComponentModel.DataAnnotations;

namespace RubricasApp.Web.DTOs
{
    public class EstudianteEmpadronamientoDto
    {
        public int IdEstudiante { get; set; }
        
        [StringLength(20, ErrorMessage = "El número de ID no puede exceder 20 caracteres")]
        [Display(Name = "Número de ID")]
        public string? NumeroId { get; set; }
        
        // Datos personales complementarios
        [Display(Name = "Fecha de Nacimiento")]
        public DateTime? FechaNacimiento { get; set; }
        
        [StringLength(20, ErrorMessage = "El género no puede exceder 20 caracteres")]
        [Display(Name = "Género")]
        public string? Genero { get; set; }
        
        [StringLength(50, ErrorMessage = "La nacionalidad no puede exceder 50 caracteres")]
        [Display(Name = "Nacionalidad")]
        public string? Nacionalidad { get; set; }
        
        [StringLength(30, ErrorMessage = "El estado civil no puede exceder 30 caracteres")]
        [Display(Name = "Estado Civil")]
        public string? EstadoCivil { get; set; }
        
        // Contacto y residencia
        [StringLength(50, ErrorMessage = "La provincia no puede exceder 50 caracteres")]
        [Display(Name = "Provincia")]
        public string? Provincia { get; set; }
        
        [StringLength(50, ErrorMessage = "El cantón no puede exceder 50 caracteres")]
        [Display(Name = "Cantón")]
        public string? Canton { get; set; }
        
        [StringLength(50, ErrorMessage = "El distrito no puede exceder 50 caracteres")]
        [Display(Name = "Distrito")]
        public string? Distrito { get; set; }
        
        [StringLength(100, ErrorMessage = "El barrio no puede exceder 100 caracteres")]
        [Display(Name = "Barrio")]
        public string? Barrio { get; set; }
        
        [StringLength(500, ErrorMessage = "Las señas no pueden exceder 500 caracteres")]
        [Display(Name = "Señas")]
        public string? Senas { get; set; }
        
        [StringLength(20, ErrorMessage = "El teléfono alterno no puede exceder 20 caracteres")]
        [Display(Name = "Teléfono Alterno")]
        public string? TelefonoAlterno { get; set; }
        
        [StringLength(100, ErrorMessage = "El correo alterno no puede exceder 100 caracteres")]
        [EmailAddress(ErrorMessage = "El correo alterno no es válido")]
        [Display(Name = "Correo Alterno")]
        public string? CorreoAlterno { get; set; }
        
        // Responsables
        [StringLength(100, ErrorMessage = "El nombre del padre no puede exceder 100 caracteres")]
        [Display(Name = "Nombre del Padre")]
        public string? NombrePadre { get; set; }
        
        [StringLength(100, ErrorMessage = "El nombre de la madre no puede exceder 100 caracteres")]
        [Display(Name = "Nombre de la Madre")]
        public string? NombreMadre { get; set; }
        
        [StringLength(100, ErrorMessage = "El nombre del tutor no puede exceder 100 caracteres")]
        [Display(Name = "Nombre del Tutor")]
        public string? NombreTutor { get; set; }
        
        [StringLength(100, ErrorMessage = "El contacto de emergencia no puede exceder 100 caracteres")]
        [Display(Name = "Contacto de Emergencia")]
        public string? ContactoEmergencia { get; set; }
        
        [StringLength(20, ErrorMessage = "El teléfono de emergencia no puede exceder 20 caracteres")]
        [Display(Name = "Teléfono de Emergencia")]
        public string? TelefonoEmergencia { get; set; }
        
        [StringLength(50, ErrorMessage = "La relación de emergencia no puede exceder 50 caracteres")]
        [Display(Name = "Relación de Emergencia")]
        public string? RelacionEmergencia { get; set; }
        
        // Salud
        [StringLength(500, ErrorMessage = "Las alergias no pueden exceder 500 caracteres")]
        [Display(Name = "Alergias")]
        public string? Alergias { get; set; }
        
        [StringLength(500, ErrorMessage = "Las condiciones médicas no pueden exceder 500 caracteres")]
        [Display(Name = "Condiciones Médicas")]
        public string? CondicionesMedicas { get; set; }
        
        [StringLength(500, ErrorMessage = "Los medicamentos no pueden exceder 500 caracteres")]
        [Display(Name = "Medicamentos")]
        public string? Medicamentos { get; set; }
        
        [StringLength(100, ErrorMessage = "El seguro médico no puede exceder 100 caracteres")]
        [Display(Name = "Seguro Médico")]
        public string? SeguroMedico { get; set; }
        
        [StringLength(100, ErrorMessage = "El centro médico habitual no puede exceder 100 caracteres")]
        [Display(Name = "Centro Médico Habitual")]
        public string? CentroMedicoHabitual { get; set; }
        
        // Historial académico
        [StringLength(100, ErrorMessage = "La institución de procedencia no puede exceder 100 caracteres")]
        [Display(Name = "Institución de Procedencia")]
        public string? InstitucionProcedencia { get; set; }
        
        [StringLength(50, ErrorMessage = "El último nivel cursado no puede exceder 50 caracteres")]
        [Display(Name = "Último Nivel Cursado")]
        public string? UltimoNivelCursado { get; set; }
        
        [Range(0, 100, ErrorMessage = "El promedio anterior debe estar entre 0 y 100")]
        [Display(Name = "Promedio Anterior")]
        public decimal? PromedioAnterior { get; set; }
        
        [StringLength(500, ErrorMessage = "Las adaptaciones previas no pueden exceder 500 caracteres")]
        [Display(Name = "Adaptaciones Previas")]
        public string? AdaptacionesPrevias { get; set; }
        
        // Estado del proceso
        [StringLength(50, ErrorMessage = "La etapa actual no puede exceder 50 caracteres")]
        [Display(Name = "Etapa Actual")]
        public string? EtapaActual { get; set; }
        
        [Display(Name = "Fecha de Etapa")]
        public DateTime? FechaEtapa { get; set; }
        
        [StringLength(100, ErrorMessage = "El usuario de etapa no puede exceder 100 caracteres")]
        [Display(Name = "Usuario de Etapa")]
        public string? UsuarioEtapa { get; set; }
        
        [Display(Name = "Notas Internas")]
        public string? NotasInternas { get; set; }
        
        // Campos de auditoría (solo lectura en DTO)
        [Display(Name = "Fecha de Creación")]
        public DateTime FechaCreacion { get; set; }
        
        [Display(Name = "Fecha de Modificación")]
        public DateTime? FechaModificacion { get; set; }
        
        [Display(Name = "Usuario de Creación")]
        public string? UsuarioCreacion { get; set; }
        
        [Display(Name = "Usuario de Modificación")]
        public string? UsuarioModificacion { get; set; }
    }
}