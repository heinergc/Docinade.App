using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DocinadeApp.Models
{
    public class EstudianteEmpadronamiento
    {
        [Key]
        [ForeignKey("Estudiante")]
        public int IdEstudiante { get; set; }
        
        [StringLength(20)]
        [Display(Name = "Número de ID")]
        public string? NumeroId { get; set; }
        
        // Datos personales complementarios
        [Display(Name = "Fecha de Nacimiento")]
        public DateTime? FechaNacimiento { get; set; }
        
        [StringLength(20)]
        [Display(Name = "Género")]
        public string? Genero { get; set; }
        
        [StringLength(50)]
        [Display(Name = "Nacionalidad")]
        public string? Nacionalidad { get; set; }
        
        [StringLength(30)]
        [Display(Name = "Estado Civil")]
        public string? EstadoCivil { get; set; }
        
        // Contacto y residencia
        [StringLength(50)]
        [Display(Name = "Provincia")]
        public string? Provincia { get; set; }
        
        [StringLength(50)]
        [Display(Name = "Cantón")]
        public string? Canton { get; set; }
        
        [StringLength(50)]
        [Display(Name = "Distrito")]
        public string? Distrito { get; set; }
        
        [StringLength(100)]
        [Display(Name = "Barrio")]
        public string? Barrio { get; set; }
        
        [StringLength(500)]
        [Display(Name = "Señas")]
        public string? Senas { get; set; }
        
        [StringLength(20)]
        [Display(Name = "Teléfono Alterno")]
        public string? TelefonoAlterno { get; set; }
        
        [StringLength(100)]
        [EmailAddress]
        [Display(Name = "Correo Alterno")]
        public string? CorreoAlterno { get; set; }
        
        // Responsables
        [StringLength(100)]
        [Display(Name = "Nombre del Padre")]
        public string? NombrePadre { get; set; }
        
        [StringLength(100)]
        [Display(Name = "Nombre de la Madre")]
        public string? NombreMadre { get; set; }
        
        [StringLength(100)]
        [Display(Name = "Nombre del Tutor")]
        public string? NombreTutor { get; set; }
        
        [StringLength(100)]
        [Display(Name = "Contacto de Emergencia")]
        public string? ContactoEmergencia { get; set; }
        
        [StringLength(20)]
        [Display(Name = "Teléfono de Emergencia")]
        public string? TelefonoEmergencia { get; set; }
        
        [StringLength(50)]
        [Display(Name = "Relación de Emergencia")]
        public string? RelacionEmergencia { get; set; }
        
        // Salud
        [StringLength(500)]
        [Display(Name = "Alergias")]
        public string? Alergias { get; set; }
        
        [StringLength(500)]
        [Display(Name = "Condiciones Médicas")]
        public string? CondicionesMedicas { get; set; }
        
        [StringLength(500)]
        [Display(Name = "Medicamentos")]
        public string? Medicamentos { get; set; }
        
        [StringLength(100)]
        [Display(Name = "Seguro Médico")]
        public string? SeguroMedico { get; set; }
        
        [StringLength(100)]
        [Display(Name = "Centro Médico Habitual")]
        public string? CentroMedicoHabitual { get; set; }
        
        // Historial académico
        [StringLength(100)]
        [Display(Name = "Institución de Procedencia")]
        public string? InstitucionProcedencia { get; set; }
        
        [StringLength(50)]
        [Display(Name = "Último Nivel Cursado")]
        public string? UltimoNivelCursado { get; set; }
        
        [Display(Name = "Promedio Anterior")]
        public decimal? PromedioAnterior { get; set; }
        
        [StringLength(500)]
        [Display(Name = "Adaptaciones Previas")]
        public string? AdaptacionesPrevias { get; set; }
        
        // Documentación
        [Display(Name = "Documentos Recibidos")]
        public string? DocumentosRecibidosJson { get; set; }
        
        [Display(Name = "Documentos Pendientes")]
        public string? DocumentosPendientesJson { get; set; }
        
        [Display(Name = "Fecha de Entrega de Documentos")]
        public DateTime? FechaEntregaDocumentos { get; set; }
        
        [Display(Name = "Fecha de Vencimiento de Póliza")]
        public DateTime? FechaVencimientoPoliza { get; set; }
        
        // Estado del proceso
        [StringLength(50)]
        [Display(Name = "Etapa Actual")]
        public string? EtapaActual { get; set; }
        
        [Display(Name = "Fecha de Etapa")]
        public DateTime? FechaEtapa { get; set; }
        
        [StringLength(100)]
        [Display(Name = "Usuario de Etapa")]
        public string? UsuarioEtapa { get; set; }
        
        [Display(Name = "Notas Internas")]
        public string? NotasInternas { get; set; }
        
        // Auditoría
        [Display(Name = "Fecha de Creación")]
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        
        [Display(Name = "Fecha de Modificación")]
        public DateTime? FechaModificacion { get; set; }
        
        [StringLength(100)]
        [Display(Name = "Usuario de Creación")]
        public string? UsuarioCreacion { get; set; }
        
        [StringLength(100)]
        [Display(Name = "Usuario de Modificación")]
        public string? UsuarioModificacion { get; set; }
        
        // Navigation property
        public virtual Estudiante? Estudiante { get; set; }
    }
}